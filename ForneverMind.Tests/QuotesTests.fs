namespace ForneverMind.Tests

open System
open System.Net
open System.Threading.Tasks

open EvilPlanner.Core
open Newtonsoft.Json
open Xunit

open ForneverMind.Tests.IntegrationTestUtil

[<Collection(IntegrationTests)>]
type QuotesTests() =

    [<Fact>]
    member _.``Quote controller should return 404 if there's no such quote``(): Task = withWebApp(fun client -> task {
        let dateWithoutQuote = DateTime(2010, 1, 1).ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)
        let! response = client.GetAsync $"/plans/quote/{dateWithoutQuote}"
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode)
    })

    [<Fact>]
    member _.``Quote controller should return an old quote``(): Task = withWebApp(fun client -> task {
        let dateWithoutQuote = DateTime(2015, 10, 9).ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)
        let! response = client.GetAsync $"/plans/quote/{dateWithoutQuote}"
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
    member _.``Quote controller should return an new quote for today even if it wasn't set beforehand``(): unit =
        // TODO: Check that there's no quote in the database
        // TODO: Request the quote, check it exists
        // TODO: Check a new quote has been saved
        Assert.True false
