    title: Утечка объектов Microsoft.CSharp.RuntimeBinder.Semantics при передаче dynamic-аргументов в методы
    description: Обсуждение утечки памяти при использовании dynamic и способов устранения утечки
    commentUrl: https://fornever.me/posts/2017-02-12-runtimebinder-memory-leak.html
---

Недавно в Telegram-чат [pro.net][] обратился человек, у которого в программе
была утечка памяти. Утечка показалась мне довольно интересной, потому что
происходила на стыке COM и `dynamic`. Как постоянные читатели моего блога уже,
наверное, знают, я [являюсь сторонником][com-post] именно такого взаимодействия
с COM-библиотеками.

Напишем тестовую программу, которая воспроизводит утечку:

```csharp
using System;

namespace ConsoleApplication3
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = Type.GetTypeFromProgID("WScript.Shell");
            for (int i = 0; i < 100000; ++i)
            {
                if (i % 1000 == 0)
                {
                    Console.WriteLine(i);
                }

                dynamic o = Activator.CreateInstance(t);
                Leak(o);
            }

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        private static void Leak(dynamic o)
        {
        }
    }
}
```

Да, вот так вот просто — если передавать COM-объект в любой метод как `dynamic`,
то начинают утекать объекты из пространства имён
`Microsoft.CSharp.RuntimeBinder.Semantics`:

![Скриншот окна dotMemory с утёкшими объектами][dotmemory]

Каждую секунду вытекает около 1 мегабайта объектов, и никакие магические пассы с
`GC.Collect()` не помогают от них избавиться. Microsoft, похоже, в курсе
проблемы, поскольку в баг-трекере Roslyn на сегодняшний день есть [незакрытый
баг][roslyn-bug] по этой теме. Однако же, ответственные сотрудники предлагали
этот баг перепостить в другой репозиторий, а в том репозитории я описания этого
бага уже не нашёл.

Описание проблемы можно найти в интернете; вот [пара][so-1] [ссылок][so-2] на
похожие вопросы на StackOverflow.

Проблема касается не только пользовательского кода — метод `Leak` можно с таким
же успехом заменить, например, на `Marshal.ReleaseComObject`.

Я провёл несколько экспериментов, и на текущий момент полагаю, что нашёл
более-менее действенное решение (поломать его и устроить утечку не получилось),
которое при этом является универсальным.

Итак, проблема заключается в том, что мы передаём ссылку на `dynamic`-объект в
любой метод. Если перед передачей конвертировать объект в `object`, то проблемы
не происходит. Вот как можно исправить пример кода выше:

```csharp
using System;

namespace ConsoleApplication3
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = Type.GetTypeFromProgID("WScript.Shell");
            for (int i = 0; i < 100000; ++i)
            {
                if (i % 1000 == 0)
                {
                    Console.WriteLine(i);
                }

                dynamic o = Activator.CreateInstance(t);
                object o1 = o;
                Leak(o1);
            }

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        private static void Leak(dynamic o)
        {
        }
    }
}
```

При этом существенно, что _принимающий метод всё ещё может объявлять параметр
как `dynamic`_; важно только лишь то, что `dynamic` не передаётся аргументом ни
в одной _точке вызова_ метода.

Таким образом, на сегодняшний день я могу рекомендовать избегать передачи
`dynamic`-переменных в любые методы, а вместо этого всегда приводить их к
конкретным типам перед передачей куда-либо. На основании проведённых опытов пока
можно сказать, что это гарантирует отсутствие описанной утечки.

[dotmemory]: images/2017-02-12-dotmemory.png

[com-post]: posts/2015-12-12-portable-com-usage_ru.html
[pro.net]: https://telegram.me/joinchat/BYlFbD6uHawWMCImmbPIDw
[roslyn-bug]: https://github.com/dotnet/roslyn/issues/2887
[so-1]: http://stackoverflow.com/q/33080252/2684760
[so-2]: http://stackoverflow.com/q/33259334/2684760
