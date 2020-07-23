    title: Использование dotless из C# и особенности компиляции под CLI
    description: Интересные подробности, исследованные во время переписывания на C# кода для работы с dotless.
---

В конце прошлого года я написал [пост про dotless][post-dotless], в котором я
описал один из способов использования этой библиотеки.

Только что товарищ Elayven [попросил][comment] помочь ему и сконвертировать код
из этого поста на C#. Ну что ж, без проблем, код-то ведь там простейший, и
ничего F#-специфичного в нём нету, правильно?

Оказывается, нет, неправильно.

Посмотрим внимательно на пару строчек из этого кода:

```fsharp
open dotless.Core
open dotless.Core.configuration

let config = DotlessConfiguration()
let locator = ContainerFactory().GetContainer(config)
let engine = (locator.GetService (typeof<ILessEngine>)) :?> ILessEngine
```

Здесь мы видим несколько вызовов конструкторов и методов. Казалось бы, что может
пойти не так? Хорошо, перепишем код на C# (создав новое консольное приложение и
подключив туда библиотеку dotless):

```csharp
using dotless.Core;
using dotless.Core.configuration;

namespace Dotless.Console
{
    class Program
    {
        static void Main()
        {
            var config = new DotlessConfiguration();
            var locator = new ContainerFactory().GetContainer(config);
            var engine = (ILessEngine) locator.GetService(typeof(ILessEngine));
        }
    }
}
```

Этот код не скомпилируется, и заругается на вызов метода `GetContainer(config)`
вот таким вот благим матом:

```
error CS0122: 'ContainerFactory.GetContainer(DotlessConfiguration)' is inaccessible due to its protection level
```

Я набирал код в Visual Studio, и она вообще-то не должна бы мне показывать
недоступные методы, так что я полез в исходники (прямиком из декомпилятора, чтоб
уменьшить вероятность того, что я смотрю исходники не от той версии библиотеки).
И что же, в исходниках чёрным по белому написано:

```csharp
namespace dotless.Core
{
    public class ContainerFactory
    {
        // ...
        public IServiceLocator GetContainer(DotlessConfiguration configuration)
        {
          // ...
        }
        // ...
    }
}
```

Класс и метод — публично доступные, проблем быть не должно. Какое-то время я
провозился с `ildasm`, почитал [код библиотеки на GitHub][dotless-source], но не
увидел причины такому странному поведению. Однако же, стоило мне перейти в
декомпилированную версию интерфейса `IServiceLocator` — и я увидел следующее:

```csharp
using System;
using System.Collections.Generic;

namespace Microsoft.Practices.ServiceLocation
{
    internal interface IServiceLocator : IServiceProvider
    {
        object GetInstance(Type serviceType);
        object GetInstance(Type serviceType, string key);
        IEnumerable<object> GetAllInstances(Type serviceType);
        TService GetInstance<TService>();
        TService GetInstance<TService>(string key);
        IEnumerable<TService> GetAllInstances<TService>();
    }
}
```

Постойте, что же это получается: `internal`-интерфейс использован в
`public`-методе? Так вообще бывает?

На этом месте я уже поискал `IServiceLocator` в багтрекере библиотеки и
наткнулся на [вот это интересное обсуждение][issue-179]. Окончательные бинарники
dotless формируются с помощью ILMerge, который помечает общедоступные элементы
присоединяемых библиотек как `internal`. В обсуждении я вычитал альтернативный
вариант решения моей проблемы без обращения к этому не вполне поддерживаемому
API. Код решения я [залил][dotless-console] в отдельный репозиторий на GitHub.

Какие же выводы мы можем сделать из всего перечисленного?

Начнём с того, что в обычной ситуации компилятор C# не позволяет создавать такие
сборки, как dotless. Использовать внутренний тип в публичном API не получится.
Например, такой код не скомпилируется:

```csharp
internal class Internal {}
public class Public {
    public void Foo(Internal i) {} // error CS0051: Inconsistent accessibility
}
```

В то же время, ILMerge, очевидно, без проблем создаёт такие сборки.

При этом компилятор C# не умеет _вообще_ ссылаться на внутренние типы,
используемые в публичном API. Поэтому он не позволяет мне обратиться к
(публичному) методу, который возвращает объект `internal`-типа.

Компилятор F#, очевидно, без проблем на этот тип ссылается (аллоцирует
локальную переменную этого типа и позволяет даже вызвать у неё методы).

Кто же прав? Валиден ли такой код в CLI? Нас может рассудить только
спецификация. Открываем настольную книгу каждого .NET-программиста — [ECMA-335:
Common Language Infrastructure (CLI)][ecma-335] — которая, к счастью, бесплатно
имеется в публичном доступе (_увы, в отличие от стандартов некоторых других
языков программирования_). Нас интересует раздел 1.8.5.3, который достаточно
ясно говорит, что типы, объявленные со спецификатором `internal` (или `private`
в терминологии CIL) снаружи сборки использовать нельзя. Попробую отправить баг
разработчикам компилятора F# — выходит, что он в таких случаях генерирует код,
не совместимый с моделью CIL (который, хоть и выполняется на Full CLR, но легко
может отвалиться на .NET Core или Mono — такие случаи уже бывали).

[comment]: posts/2015-11-08-using-dotless-api.html#comment-2887368608
[dotless-console]: https://github.com/ForNeVeR/Dotless.Console/blob/2c5af803056faa991ce16a1c9655eccfc73ec258/src/Dotless.Console/Program.cs#L20-L27
[dotless-source]: https://github.com/dotless/dotless
[ecma-335]: http://www.ecma-international.org/publications/files/ECMA-ST/ECMA-335.pdf
[issue-179]: https://github.com/dotless/dotless/issues/179
[post-dotless]: posts/2015-11-08-using-dotless-api.html
