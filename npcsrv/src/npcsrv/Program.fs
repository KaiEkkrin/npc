module Npcsrv.App

open System
open System.Collections
open System.Globalization
open System.IO
open FSharp.Control.Tasks.V2
open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Npcsrv.Models
open Npcsrv.Views

// ---------------------------------
// Web app
// ---------------------------------

let indexHandler (name : string) =
    let greetings = sprintf "Hello %s, from Giraffe!" name
    let model     = { Text = greetings }
    let view      = Views.index model
    htmlView view

let parsingError err = RequestErrors.BAD_REQUEST err

let createHandler (req: CharacterBuildRequest) next (ctx: HttpContext) =
    let bld = CharacterBuild.Create req.Name req.Level
    let save = ctx.GetService<CharacterBuildSave>()
    let response = save bld |> CharacterBuildResponse.Of ""
    json response next ctx

let idHandler id next (ctx: HttpContext) =
    let find = ctx.GetService<CharacterBuildFind>()
    let bld = find { Id = Some id; Name = None }
    let responses = bld |> Array.map (CharacterBuildResponse.Of "")
    match Array.tryExactlyOne responses with
    | Some response -> json response next ctx
    | _ -> (setStatusCode 404 >=> text "Not found") next ctx

let improveHandler (req: CharacterImproveRequest) next (ctx: HttpContext) =
    let find = ctx.GetService<CharacterBuildFind>()
    let bld = find { Id = Some req.Id; Name = None }
    match Array.tryExactlyOne bld with
    | Some bld ->
        let save = ctx.GetService<CharacterBuildSave>()
        let _, updated = CharacterBuildResponse.Update (req.Choice) bld
        let response = save updated |> (CharacterBuildResponse.Of "")
        json response next ctx
    | _ -> (setStatusCode 404 >=> text "Not found") next ctx

let findHandler req next (ctx: HttpContext) =
    let find = ctx.GetService<CharacterBuildFind>()
    let bld = find req
    let responses = bld |> Array.map (CharacterBuildResponse.Of "")
    json responses next ctx

let deleteHandler id next (ctx: HttpContext) =
    let delete = ctx.GetService<CharacterBuildDelete>()
    match delete id with
    | true -> (text "") next ctx
    | false -> (setStatusCode 404 >=> text "Not found") next ctx

let webApp =
    choose [
        GET >=>
            choose [
                //route "/" >=> indexHandler "world"
                routef "/hello/%s" indexHandler
                route "/find" >=> bindQuery<CharacterBuildCriteria> (Some CultureInfo.InvariantCulture) findHandler
                routef "/%s" idHandler
            ]
        POST >=>
            choose [
                route "/create" >=> bindJson<CharacterBuildRequest> (validateModel createHandler)
                route "/improve" >=> bindJson<CharacterImproveRequest> improveHandler
            ]
        DELETE >=>
            choose [
                routef "/%s" deleteHandler
            ]
        setStatusCode 404 >=> text "Not Found" ]

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

let configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins("http://localhost:8080")
           .AllowAnyMethod()
           .AllowAnyHeader()
           |> ignore

let configureApp (app : IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IWebHostEnvironment>()
    (match env.IsDevelopment() with
    | true  -> app.UseDeveloperExceptionPage()
    | false -> app.UseGiraffeErrorHandler errorHandler)
        .UseHttpsRedirection()
        .UseCors(configureCors)
        .UseStaticFiles()
        .UseGiraffe(webApp)

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore

    let inMemory = Hashtable()
    services.AddSingleton<CharacterBuildFind>(InMemory.find inMemory) |> ignore
    services.AddSingleton<CharacterBuildSave>(InMemory.save inMemory) |> ignore
    services.AddSingleton<CharacterBuildDelete>(InMemory.delete inMemory) |> ignore

let configureLogging (builder : ILoggingBuilder) =
    builder.AddFilter(fun l -> l.Equals LogLevel.Error)
           .AddConsole()
           .AddDebug() |> ignore

[<EntryPoint>]
let main _ =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot     = Path.Combine(contentRoot, "WebRoot")
    WebHostBuilder()
        .UseKestrel()
        .UseContentRoot(contentRoot)
        .UseIISIntegration()
        .UseWebRoot(webRoot)
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(configureServices)
        .ConfigureLogging(configureLogging)
        .Build()
        .Run()
    0