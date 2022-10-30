[<Xunit.Collection(ForneverMind.TestFramework.IntegrationTestUtil.IntegrationTests)>]
module ForneverMind.Tests.QuotesTests

open System
open System.Globalization
open System.Net
open System.Net.Http
open System.Threading.Tasks

open ForneverMind.TestFramework
open Newtonsoft.Json
open Xunit

open EvilPlanner.Core
open EvilPlanner.Logic
open ForneverMind.TestFramework.IntegrationTestUtil

let private getQuote (client: HttpClient) (date: DateOnly) =
    let dateString = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
    client.GetAsync $"/plans/quote/{dateString}"

[<Fact>]
let ``Quote controller should return 404 if there's no such quote``(): Task = withWebApp(fun client -> task {
    let dateWithoutQuote = DateOnly(2010, 1, 1)
    let! response = getQuote client dateWithoutQuote
    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode)
})

[<Fact>]
let ``Quote controller should return 400 if it's unable to parse the date``(): Task = withWebApp(fun client -> task {
    let dateString = "invalid_date"
    let! response = client.GetAsync $"/plans/quote/{dateString}"
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode)
})

[<Fact>]
let ``Quote controller should return an old quote``(): Task = withWebApp(fun client -> task {
    let oldDateWithQuote = DateOnly(2015, 10, 9)
    let! response = getQuote client oldDateWithQuote
    let! content = response.EnsureSuccessStatusCode().Content.ReadAsStringAsync()
    let quote = JsonConvert.DeserializeObject<Quotation> content
    Assert.Equal({
        id = 69L
        text =
            "I will not have a son.  Although his eventual and surely laughable plan to overthrow me will fail, it " +
            "could provide a fatal distraction at a crucial moment."
        source = "The Evil Overlord List"
        sourceUrl = "http://legendspbem.angelfire.com/eviloverlordlist.html"
    }, quote)
})

[<Fact>]
let ``Quote controller should return an new quote for today even if it wasn't set beforehand``(): Task =
    let today = DateOnly(2022, 10, 5)
    withWebAppData today (fun database client -> task {
        database.ReadWriteTransaction StorageUtils.clearDailyQuotes

        // Check that there's no quote in the database:
        let quote = database.ReadOnlyTransaction(QuoteLogic.getDailyQuote today)
        Assert.Equal(None, quote)

        let! response = getQuote client today
        response.EnsureSuccessStatusCode() |> ignore

        let quote = database.ReadOnlyTransaction(QuoteLogic.getDailyQuote today)
        Assert.NotEqual(None, quote)
    })
