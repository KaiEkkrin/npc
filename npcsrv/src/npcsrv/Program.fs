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

let createHandler req =
    let bld = CharacterBuild.Create req.Name req.Level
    fun next (ctx: HttpContext) ->
        let save = ctx.GetService<CharacterBuildSave>()
        let response = save bld |> CharacterBuildResponse.Of ""
        json response next ctx

let idHandler id =
    let doFind next (ctx: HttpContext) =
        let find = ctx.GetService<CharacterBuildFind>()
        let bld = find { Id = id }
        let responses = bld |> Array.map (CharacterBuildResponse.Of "")
        match Array.length responses with
        | 1 -> json responses next ctx
        | _ -> (setStatusCode 404 >=> text "Not found") next ctx
    doFind

let improveHandler (req: CharacterImproveRequest) =
    fun next (ctx: HttpContext) ->
        let find = ctx.GetService<CharacterBuildFind>()
        let bld = find { Id = req.Id }
        match Array.length bld with
        | 1 ->
            let save = ctx.GetService<CharacterBuildSave>()
            let _, updated = CharacterBuildResponse.Update (req.Choice) bld.[0]
            let response = save updated |> (CharacterBuildResponse.Of "")
            json response next ctx
        | _ -> (setStatusCode 404 >=> text "Not found") next ctx

let findHandler req =
    let doFind next (ctx: HttpContext) =
        let find = ctx.GetService<CharacterBuildFind>()
        let bld = find req
        let responses = bld |> Array.map (CharacterBuildResponse.Of "")
        json responses next ctx
    doFind

let webApp =
    choose [
        GET >=>
            choose [
                //route "/" >=> indexHandler "world"
                routef "/%s" idHandler
                routef "/hello/%s" indexHandler
                route "/find" >=> bindQuery<CharacterBuildCriteria> (Some CultureInfo.InvariantCulture) findHandler
            ]
        POST >=>
            choose [
                route "/create" >=> bindJson<CharacterBuildRequest> (validateModel createHandler)
                route "/improve" >=> bindJson<CharacterImproveRequest> improveHandler
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