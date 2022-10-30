[<Xunit.Collection(ForneverMind.Tests.IntegrationTestUtil.IntegrationTests)>]
module ForneverMind.Tests.QuotesTests

open System
open System.Net
open System.Net.Http
open System.Threading.Tasks

open Newtonsoft.Json
open Xunit

open EvilPlanner.Core
open EvilPlanner.Logic
open ForneverMind.Tests.IntegrationTestUtil

let private getQuote (client: HttpClient) (date: DateTime) =
    let dateString = date.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)
    client.GetAsync $"/plans/quote/{dateString}"

[<Fact>]
let ``Quote controller should return 404 if there's no such quote``(): Task = withWebApp(fun client -> task {
    let dateWithoutQuote = DateTime(2010, 1, 1)
    let! response = getQuote client dateWithoutQuote
    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode)
})

[<Fact>]
let ``Quote controller should return an old quote``(): Task = withWebApp(fun client -> task {
    let oldDateWithQuote = DateTime(2015, 10, 9)
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
        let today = today.ToDateTime(TimeOnly.FromTimeSpan TimeSpan.Zero, DateTimeKind.Utc)

        // Check that there's no quote in the database:
        let quote = database.ReadOnlyTransaction(QuoteLogic.getDailyQuote today)
        Assert.Equal(None, quote)

        let! response = (getQuote client today)
        response.EnsureSuccessStatusCode() |> ignore

        let quote = database.ReadOnlyTransaction(QuoteLogic.getDailyQuote today)
        Assert.NotEqual(None, quote)
    })
