module Hw6.App
open Hw6
open Parser
open Calculator
open MaybeBuilder
open System
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open System.Net
open Microsoft.FSharp.Control.WebExtensions

let calculatorHandler: HttpHandler =
    fun next ctx ->
        
        let result = maybe {
            let! val1 = ctx.GetQueryStringValue "value1"
            let! val2 = ctx.GetQueryStringValue "value2"
            let! operation = ctx.GetQueryStringValue "operation"
            let args = [|val1; operation; val2;|]
            let! calculation = parseCalcArguments args
            return calculation
        }    
        
        match result with
        | Ok ok -> (setStatusCode 200 >=> text (ok.ToString())) next ctx
        | Error error -> (setStatusCode 400 >=> text error) next ctx

let webApp =
    choose [
        GET >=> choose [
             route "/" >=> text "Use //calculate?value1=<VAL1>&operation=<OPERATION>&value2=<VAL2>"
             route "/calculate" >=> calculatorHandler
        ]
        setStatusCode 404 >=> text "Not Found" 
    ]
    
type Startup() =
    member _.ConfigureServices (services : IServiceCollection) =
        services.AddGiraffe() |> ignore

    member _.Configure (app : IApplicationBuilder) (_ : IHostEnvironment) (_ : ILoggerFactory) =
        app.UseGiraffe webApp
        
let readLineAsync =
    async {
        try
            return Some <| Async.RunSynchronously(async { return Console.ReadLine() })
        with | _ ->
            return None
    }    

let rec runConsoleApp =
     async {
            let! message = readLineAsync
            match message with
            | Some data ->
                let options = data.Split[|' '|]
                //route $"/" >=> calculatorHandler
                //redirectTo true $"/"
                //webApp >=> GET >=> route $"/calculate?value1={options[0]}&operation={options[1]}&value2={options[2]}" >=> calculatorHandler
                route $"/calculate?value1={options[0]}&operation={options[1]}&value2={options[2]}" >=> calculatorHandler
                //redirectTo true $"/calculate?value1={options[0]}&operation={options[1]}&value2={options[2]}"
            | None -> printfn "Chat Session Ended"
            return! runConsoleApp
        }

let runWeb =
       async {
        Host
            .CreateDefaultBuilder()
            .ConfigureWebHostDefaults(fun whBuilder -> whBuilder.UseStartup<Startup>() |> ignore)
            .Build()
            .Run()   
       }
       
 
[<EntryPoint>]
let main _ =
        
        let task =
            [ runConsoleApp
              runWeb ]
            |> Async.Parallel
            |> Async.Ignore
        try
                Async.RunSynchronously (computation=task)
        with
           | ex -> printfn "%s" (ex.Message);
       
        0
     
    