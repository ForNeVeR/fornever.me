    title: Малоизвестные подробности маршаллинга строк в .NET
    description: Исследуем особенности маршаллинга System.String в качестве полей структуры и напрямую
    commentUrl: https://fornever.me/posts/2017-09-20-clr-string-marshalling.html
---

Сегодня в телеграм-чате [.NET Group][net-group] было обсуждение маршаллинга
строк в .NET. Один из участников обсуждения наткнулся на интересный баг: при
попытке маршаллить структуру, содержащую `StringBuilder`, он получал исключение:

```
Cannot marshal field 'FieldName' of type 'StructName': Struct or class fields
cannot be of type StringBuilder. The same effect can usually be achieved by
using a String field and preinitializing it to a string with length matching the
length of the appropriate buffer.
```

Это меня очень удивило: раньше я считал, что для передачи каких-то мутабельных
буферов в нативный код *обязательно* нужно использовать `StringBuilder` — и тут
вот на тебе, исключение из CLR официально заявляет нам обратное — «Используйте
тут иммутабельные строки для мутабельных буферов».

Пришлось с этим поразбираться подробнее. Результатами исследований с
удовольствием делюсь с читателем.

Для начала напишем простую программу на C++, которая будет мутировать строки
(здесь и далее я использую диалект Visual C++ для экспорта функций из DLL; вы
можете использовать любой другой диалект или нативный язык, от него в данном
сценарии ничего особо не зависит):

```cpp
#include <cwchar>
#include <xutility>

extern "C" __declspec(dllexport) void MutateString(wchar_t *string)
{
    std::reverse(string, wcschr(string, '\0'));
}
```

Эта программа переворачивает переданную строку на месте, т. е. мутирует её.

Скомпилируем эту программу с именем `Program1.dll` и положим в рабочий каталог.
Теперь попробуем её вызвать из C#. Сначала я покажу _правильный_ способ вызова,
который вам ничего не сломает:

```csharp
using System;
using System.Text;
using System.Runtime.InteropServices;

class Program
{
    [DllImport("Project1.dll", CharSet = CharSet.Unicode)]
    private static extern void MutateString(StringBuilder foo);

    static void Main()
    {
        var myString = new StringBuilder("Hello World 1");
        MutateString(myString);

        Console.WriteLine(myString.ToString()); // => 1 dlroW olleH
        Console.WriteLine("Hello World 1");     // => Hello World 1
    }
}
```

В этом коде всё сработало отлично: метод изменил строку в буфере `myString`,
последующее выражение `Console.WriteLine("Hello World 1")` работает как
ожидается.

А теперь посмотрим на _неправильный_ вариант кода, который работает неожиданным
образом:

```csharp
using System;
using System.Runtime.InteropServices;

class Program
{
    [DllImport("Project1.dll", CharSet = CharSet.Unicode)]
    private static extern void MutateString(string foo);

    static void Main()
    {
        var myString = "Hello World 1";
        MutateString(myString);

        Console.WriteLine(myString.ToString()); // => 1 dlroW olleH
        Console.WriteLine("Hello World 1");     // => 1 dlroW olleH
    }
}
```

В этом случае мы передали в нативный метод указатель на строку, которая
находится в пуле интернированных строк CLR. Это такой набор константных строк,
которые собираются и дедуплицируются в момент компиляции программы (т. е.,
например, строка `"Hello World 1"` в этом пуле находится в единственном
экземпляре).

Поменяв переданную строку, наш нативный метод **поменял константу во всём
коде**. Это, конечно же, очень плохо, и может запросто сломать любую программу
самым неожиданным способом. **Делать так не нужно, этот код плохой.**

Отлично. Вооружившись этим знанием, попробуем проработать более продвинутый
сценарий: пусть нативный код хочет, чтобы ему передали _структуру_. Вот
модифицированная версия программы на C++:

```cpp
#include <cwchar>
#include <xutility>

extern "C" __declspec(dllexport) void MutateString(wchar_t *string)
{
    std::reverse(string, wcschr(string, '\0'));
}

struct S
{
    wchar_t *field;
};

extern "C" __declspec(dllexport) void MutateStruct(S *s)
{
    MutateString(s->field);
}
```

К старой функции добавилась новая, принимающая указатель на структуру `S`,
которая содержит только лишь один указатель на строку.

Наученные опытом, попробуем написать код со `StringBuilder`:

```csharp
using System;
using System.Runtime.InteropServices;
using System.Text;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
struct S
{
    public StringBuilder field;
}

class Program
{
    [DllImport("Project1.dll", CharSet = CharSet.Unicode)]
    private static extern void MutateStruct(ref S foo);

    static void Main()
    {
        S s = new S();
        s.field = "Hello World 2";
        MutateStruct(ref s);

        Console.WriteLine(s.field.ToString());
        Console.WriteLine("Hello World 2");
    }
}
```

И вот тут нас ждёт облом: при запуске этого кода в консоль будет выведена
ошибка.

```
System.TypeLoadException: Cannot marshal field 'field' of type 'S': Struct or
class fields cannot be of type StringBuilder. The same effect can usually be
achieved by using a String field and preinitializing it to a string with length
matching the length of the appropriate buffer.
```

Хорошо. Раз уж рантайм предлагает — давайте попробуем последовать его
рекомендации, и заменим `StringBuilder` на строку.

```csharp
using System;
using System.Runtime.InteropServices;
using System.Text;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
struct S
{
    public string field;
}

class Program
{
    [DllImport("Project1.dll", CharSet = CharSet.Unicode)]
    private static extern void MutateStruct(ref S foo);

    static void Main()
    {
        S s = new S();
        s.field = "Hello World 2";
        MutateStruct(ref s);

        Console.WriteLine(s.field);         // => 2 dlroW olleH
        Console.WriteLine("Hello World 2"); // => Hello World 2
    }
}
```

Как видите — теперь всё в порядке. Но почему?

Насколько мне удалось понять из релевантной документации, в .NET есть
специальный режим маршаллинга для строк, но актуален он только при передаче
строки как параметра метода. Почитать об этом режиме можно [в разделе
«System.String and System.Text.StringBuilder» статьи «Copying and
Pinning»][copying-and-pinning].

А вот при помещении строки в поле структуры включается обычный режим
маршаллинга. Строка не является blittable, а, следовательно, не является и
структура, которая её содержит. Поэтому такая структура при маршаллинге будет
копироваться вместе со строкой. Про это можно почитать в статье [Blittable and
Non-Blittable Types][blittable].

**Итак**:

1. При передаче строк в нативные функции, которые могут их мутировать, по прямой
   ссылке, используйте `StringBuilder`.
2. При передаче строк в нативные функции в качестве полей структур не бойтесь
   использовать `string`.

[blittable]: https://docs.microsoft.com/en-us/dotnet/framework/interop/blittable-and-non-blittable-types
[copying-and-pinning]: https://docs.microsoft.com/en-us/dotnet/framework/interop/copying-and-pinning#systemstring-and-systemtextstringbuilder
[net-group]: https://t.me/dotnetgroup
