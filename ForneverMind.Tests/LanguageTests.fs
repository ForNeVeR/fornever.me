module ForneverMind.Tests.LanguageTests

open Freya.Types.Http
open Freya.Types.Language
open Xunit

open ForneverMind

[<Fact>]
let ``getLanguage returns None for empty language list``() =
    Assert.Equal(None, Language.getLanguage (AcceptLanguage []))

[<Fact>]
let ``getLanguage selects the language with greater priority``() =
    let languages =
        [ AcceptableLanguage(Range ["en"], Some(Weight 0.0))
          AcceptableLanguage(Range ["ru"], Some(Weight 1.0)) ]
    Assert.Equal(Some "ru", Language.getLanguage (AcceptLanguage languages))

[<Fact>]
let ``getLanguage ignores the additional locale info``() =
    let languages =
        [ AcceptableLanguage(Range ["en"], Some(Weight 0.0))
          AcceptableLanguage(Range ["ru-RU"], Some(Weight 1.0)) ]
    Assert.Equal(Some "ru", Language.getLanguage (AcceptLanguage languages))

[<Fact>]
let ``getLanguage returns default language for Any``() =
    let languages =
        [ AcceptableLanguage(Any, None) ]
    Assert.Equal(Some "en", Language.getLanguage (AcceptLanguage languages))
