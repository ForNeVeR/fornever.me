    title: Фильтры исключений в CLR
    description: Описание механизма фильтров исключений, доступного для некоторых языков CLR.
---

*Это слегка модифицированная версия статьи, опубликованной [на хабрахабре](http://habrahabr.ru/post/192144/).*

Сегодня мы рассмотрим один из механизмов CLR, который напрямую недоступен для разработчиков на языке C# — фильтры исключений.

Опрос среди моих знакомых программистов на C# показал, что они (само собой) никогда этим механизмом не пользовались и даже не знают о его существовании. Поэтому предлагаю всем любознательным ознакомиться с текстом статьи.

Итак, фильтры исключений — это механизм, который позволяет блоку `catch` декларировать предусловия, которым должно удовлетворять исключение, дабы быть пойманным данным блоком. Этот механизм работает _не совсем так же_, как выполнение проверок внутри блока `catch`.

Нас ожидает код на VB.NET, F#, CIL и C#, а также проверка различных декомпиляторов на обработку механизма фильтров.

## Откуда есть пошли фильтры исключений
Фильтры исключений встроены в среду CLR и являются одним из механизмов, который среда использует при обработке исключения. Последовательность выглядит следующим образом:

![Бросание исключения, поиск подходящего блока catch, выполнение блоков catch и
finally](./images/2013-09-01-exception-diagram.png)

На этапе поиска подходящего блока `catch` CLR выполняет обход своего внутреннего стека обработчиков исключений, а также  выполняет фильтры исключений. Обратите внимание — это происходит **до** выполнения кода в блоке `finally`. Мы обсудим этот момент позже.

### Как это выглядит на VB.NET
Фильтры исключений нативно поддерживаются языком VB.NET. Вот пример того, как выглядит код, использующий фильтры:

```monobasic
Sub FilterException()
    Try
        Dim exception As New Exception
        exception.Data.Add("foo", "bar1")

        Console.WriteLine("Throwing")
        Throw exception
    Catch ex As Exception When Filter(ex) ' здесь фильтр
        Console.WriteLine("Caught")
    Finally
        Console.WriteLine("Finally")
    End Try

End Sub

Function Filter(exception As Exception) As Boolean
    Console.WriteLine("Filtering")
    Return exception.Data.Item("foo").Equals("bar")
End Function
```

При выполнении данного кода будет выдана следующая цепочка сообщений:

```
Throwing
Filtering
Caught
Finally
```

### Как это выглядит в F\#
При подготовке статьи я нашёл в интернете [информацию](http://blogs.msdn.com/b/dotnet/archive/2009/08/25/the-good-and-the-bad-of-exception-filters.aspx) о том, что F# поддерживает фильтры исключений. Что ж, проверим это. Вот пример кода:

```fsharp
open System

let filter (ex : Exception) =
    printfn "Filtering"
    ex.Data.["foo"] :?> string = "bar"

let filterException() =
    try
        let ex = Exception()
        ex.Data.["foo"] <- "bar"
        printfn "Throwing"
        raise ex
    with // дальше фильтр
    | :? Exception as ex when filter(ex) -> printfn "Caught"

[<EntryPoint>]
let main argv =
    filterException()
    0
```

Этот код компилируется без фильтров, с обычным `catch [mscorlib]System.Object`. Мне так и не удалось заставить компилятор F# сделать фильтр исключений. Если вам известны альтернативные способы это сделать — добро пожаловать в комментарии.

### Как это выглядит в CIL
CIL (Common Intermediate Language) — это аналог низкоуровневого языка ассемблера для .NET-машины. Скомпилированные сборки можно дизассемблировать в этот язык с помощью инструмента `ildasm`, и собирать обратно с помощью `ilasm`, которые поставляются вместе с .NET.

Приведу фрагмент кода на VB.NET, каким я его увидел в `ildasm`:

```
.method public static void  FilterException() cil managed
{
  // Code size       110 (0x6e)
  .maxstack  3
  .locals init ([0] class [mscorlib]System.Exception exception,
           [1] class [mscorlib]System.Exception ex)
  IL_0000:  nop
  IL_0001:  nop
  .try
  {
    .try
    {
      IL_0002:  newobj     instance void [mscorlib]System.Exception::.ctor()
      IL_0007:  stloc.0
      IL_0008:  ldloc.0
      IL_0009:  callvirt   instance class [mscorlib]System.Collections.IDictionary [mscorlib]System.Exception::get_Data()
      IL_000e:  ldstr      "foo"
      IL_0013:  ldstr      "bar"
      IL_0018:  callvirt   instance void [mscorlib]System.Collections.IDictionary::Add(object,
                                                                                       object)
      IL_001d:  nop
      IL_001e:  ldstr      "Throwing"
      IL_0023:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0028:  nop
      IL_0029:  ldloc.0
      IL_002a:  throw
      IL_002b:  leave.s    IL_006b
    }  // end .try
    filter
    {
      IL_002d:  isinst     [mscorlib]System.Exception
      IL_0032:  dup
      IL_0033:  brtrue.s   IL_0039
      IL_0035:  pop
      IL_0036:  ldc.i4.0
      IL_0037:  br.s       IL_0049
      IL_0039:  dup
      IL_003a:  stloc.1
      IL_003b:  call       void [Microsoft.VisualBasic]Microsoft.VisualBasic.CompilerServices.ProjectData::SetProjectError(class [mscorlib]System.Exception)
      IL_0040:  ldloc.1
      IL_0041:  call       bool FilterSamples.VbNetFilter::Filter(class [mscorlib]System.Exception)
      IL_0046:  ldc.i4.0
      IL_0047:  cgt.un
      IL_0049:  endfilter
    }  // end filter
    {  // handler
      IL_004b:  pop
      IL_004c:  ldstr      "Caught"
      IL_0051:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0056:  nop
      IL_0057:  call       void [Microsoft.VisualBasic]Microsoft.VisualBasic.CompilerServices.ProjectData::ClearProjectError()
      IL_005c:  leave.s    IL_006b
    }  // end handler
  }  // end .try
  finally
  {
    IL_005e:  nop
    IL_005f:  ldstr      "Finally"
    IL_0064:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0069:  nop
    IL_006a:  endfinally
  }  // end handler
  IL_006b:  nop
  IL_006c:  nop
  IL_006d:  ret
} // end of method VbNetFilter::FilterException
```

Как видно, компилятор VB.NET, конечно, сильно расписал наш код в виде CIL. Больше всего нас интересует блок `filter`:

```
filter
{
  // Проверяем, что брошенный объект является экземпляром System.Exception:
  IL_002d:  isinst     [mscorlib]System.Exception
  IL_0032:  dup
  IL_0033:  brtrue.s   IL_0039
  IL_0035:  pop
  IL_0036:  ldc.i4.0
  // Если нет - то выходим:
  IL_0037:  br.s       IL_0049
  IL_0039:  dup

  // Тут какой-то служебный вызов:
  IL_003a:  stloc.1
  IL_003b:  call       void [Microsoft.VisualBasic]Microsoft.VisualBasic.CompilerServices.ProjectData::SetProjectError(class [mscorlib]System.Exception)

  // Вызываем функцию, которую мы определили как фильтр:
  IL_0040:  ldloc.1
  IL_0041:  call       bool FilterSamples.VbNetFilter::Filter(class [mscorlib]System.Exception)
  IL_0046:  ldc.i4.0
  IL_0047:  cgt.un
  IL_0049:  endfilter
}  // end filter
```

Итак, компилятор вынес в блок фильтра проверку типа исключения, а также вызов нашей функции. Если в конце выполнения блока фильтра на стеке лежит значение `1`, то соответствующий этому фильтру блок `catch` будет выполнен; иначе — нет.

Стоит отметить, что компилятор C# проверки типов не выносит в блок `filter`, а использует специальную CIL-конструкцию `catch` с указанием типа. То есть, компилятор C# не использует механизм `filter` вообще.

Кстати говоря, для генерации этого блока можно использовать метод [ILGenerator.BeginExceptFilterBlock](http://msdn.microsoft.com/en-us/library/system.reflection.emit.ilgenerator.beginexceptfilterblock.aspx) (это может пригодиться, если вы пишете свой компилятор).

### Как это выглядит в декомпиляторе
В этом разделе я попробую декомпилировать полученный код с помощью нескольких известных инструментов и посмотреть, что из этого получится.

Актуальный на момент написания статьи [JetBrains dotPeek](http://www.jetbrains.com/decompiler/) версии 1.1  при попытке декомпиляции сборки с фильтром радостно сообщил следующее:

```cs
public static void FilterException()
{
  // ISSUE: unable to decompile the method.
}
```

[.NET Reflector](http://www.red-gate.com/products/dotnet-development/reflector/) версии 8.2 поступил более адекватно и что-то смог декомпилировать в C#:

```cs
public static void FilterException()
{
    try
    {
        Exception exception = new Exception();
        exception.Data.Add("foo", "bar");
        Console.WriteLine("Throwing");
        throw exception;
    }
    catch when (?)
    {
        Console.WriteLine("Caught");
        ProjectData.ClearProjectError();
    }
    finally
    {
        Console.WriteLine("Finally");
    }
}
```

Что ж, неплохо — хотя код и некомпилируемый, но по нему хотя бы заметно наличие фильтра. То, что фильтр не был расшифрован, можно списать на недостатки C#-транслятора. Попробуем то же самое с транслятором в VB.NET:

```
Public Shared Sub FilterException()
    Try
        Dim exception As New Exception
        exception.Data.Add("foo", "bar")
        Console.WriteLine("Throwing")
        Throw exception
    Catch obj1 As Object When (?)
        Console.WriteLine("Caught")
        ProjectData.ClearProjectError
    Finally
        Console.WriteLine("Finally")
    End Try
End Sub
```

Увы, попытка точно так же провалилась — декомпилятор почему-то не смог определить имя фильтрующей функции (хотя, как мы видели выше, `ildasm` с этим прекрасно справился).

Могу только предположить, что рассмотренные инструменты пока плохо работают с кодом фильтров .NET 4.5.

## Чем это отличается от проверок в теле блока `catch`
Рассмотрим фрагмент кода, *почти* аналогичный коду на VB.NET:

```cs
static void FilterException()
{
    try
    {
        var exception = new Exception();
        exception.Data["foo"] = "bar";
        Console.WriteLine("Throwing");
        throw exception;
    }
    catch (Exception exception)
    {
        if (!Filter(exception))
        {
            throw;
        }

        Console.WriteLine("Caught");
    }
}

static bool Filter(Exception exception)
{
    return exception.Data["foo"].Equals("bar");
}
```

А теперь попробуем найти разницу в поведении между примерами на C# и VB.NET. Всё достаточно просто: выражение `throw;` в C# теряет номер строки в стеке. Если изменить фильтр так, чтобы он возвращал `false`, то приложение упадёт с сообщением

```
Unhandled Exception: System.Exception: Exception of type 'System.Exception' was thrown.
   at CSharpFilter.Program.FilterException() in CSharpFilter\Program.cs:line 25
   at CSharpFilter.Program.Main(String[] args) in CSharpFilter\Program.cs:line 9
```

Судя по стеку, исключение было сгенерировано на 25 строке (строка `throw;`), а не на строке 19 (`throw exception;`). Код на VB.NET в таких же условиях показывает изначальное место выпадения исключения.

Изначально я ошибочно написал, что `throw;` теряет весь стек, но в комментариях подсказали, что это действительно совсем не так. Происходит лишь незначительная модификация номера строки в стеке. Причём на mono это не воспроизводится — стек исключения там не меняется после `throw;` (спасибо пользователю Хабрахабра [**kekekeks**](http://habrahabr.ru/users/kekekeks/) за эти подробности).

## О безопасности
Эрик Липперт [в своём блоге](http://blogs.msdn.com/b/ericlippert/archive/2004/09/01/224064.aspx) рассматривает ситуацию, когда фильтры исключений позволяют вредоносной стороне выполнить свой код с повышенными привилегиями в некоторых случаях.

Коротко: если вы выполняете временное повышение привилегий для какого-то внешнего и потенциально разрушительного кода, то нельзя полагаться на `finally`, т.к. перед выполнением блока `finally` могут быть вызваны фильтры исключений, расположенные выше по стеку вызовов (а злоумышленник может вытворять в коде этих фильтров всё, что ему вздумается). Помните — поиск подходящего блока `catch` всегда выполняется **до** выполнения блока `finally`.

## Заключение
Сегодня мы рассмотрели один из редко встречаемых программистами на C# механизмов среды CLR. Сам я не пишу на VB.NET, но считаю, что эта информация может быть полезна всем разработчикам .NET-платформы. Ну а если вы занимаетесь разработкой языков, компиляторов или декомпиляторов для этой платформы, то вам тем более эта информация пригодится.

**PS.** Код, картинки и текст статьи [выложены на github](https://github.com/ForNeVeR/ClrExceptionFilters).
