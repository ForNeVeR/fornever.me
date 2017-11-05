module ForneverMind.Tests.CommonTests

open Xunit

open ForneverMind

[<Fact>]
let ``Path checks are passed``() =
    Assert.True(Common.pathIsInsideDirectory "aaa" "aaa\\bbb\\ccc")
    Assert.False(Common.pathIsInsideDirectory "aaa" "aaa\\..\\bbb")

[<Fact>]
let ``Default language is English``() =
    Assert.Equal("en", Common.defaultLanguage)

[<Fact>]
let ``Supported languages are English and Russian``() =
    Assert.Equal<string>([| "en"; "ru" |], Common.supportedLanguages)
