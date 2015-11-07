namespace ForneverMind.Tests

open Xunit

open ForneverMind

type public CommonTests () =
    [<Fact>]
    member __.PathCheckTests () =
        Assert.True (Common.pathIsInsideDirectory "aaa" "aaa\\bbb\\ccc")
        Assert.False (Common.pathIsInsideDirectory "aaa" "aaa\\..\\bbb")
