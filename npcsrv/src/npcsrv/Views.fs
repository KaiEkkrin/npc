module Npcsrv.Views

open Giraffe
open Npcsrv.Models

// ---------------------------------
// Views
// ---------------------------------

module Views =
    open GiraffeViewEngine

    let layout (content: XmlNode list) =
        html [] [
            head [] [
                title []  [ encodedText "npcsrv" ]
                link [ _rel  "stylesheet"
                       _type "text/css"
                       _href "/main.css" ]
            ]
            body [] content
        ]

    let partial () =
        h1 [] [ encodedText "npcsrv" ]

    // The index view will list existing characters and prompt
    // for a name for a new one
    // TODO Require login, and all that
    let index (model : Message) =
        [
            partial()
            p [] [ encodedText model.Text ]
        ] |> layout