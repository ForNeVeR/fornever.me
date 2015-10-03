module EvilPlanner.Tests.Program

open EvilPlanner.Data

[<EntryPoint>]
let main _ =
    Migrations.Configuration.EnableAutoMigration ()
    try
        let success = Async.RunSynchronously <| Concurrency.test ()
        if success
        then
            printfn "success" 
            0
        else
            printfn "fail"
            1
    with
        ex ->
            printfn "%A" ex
            1
