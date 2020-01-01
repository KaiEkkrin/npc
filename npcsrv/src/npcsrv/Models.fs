module Npcsrv.Models

open System
open Giraffe
open Npc
open Npc.Attributes

// TODO remove the giraffe test page and all that :)
type Message =
    {
        Text : string
    }

// A character build request
[<CLIMutable>]
type CharacterBuildRequest = {
    Name: string
    Level: int
}
with
    member this.HasErrors () =
        if this.Name = "" then Some "No name specified."
        elif this.Level <= 0 || this.Level > 20 then Some "Invalid level specified."
        else None

    interface IModelValidation<CharacterBuildRequest> with
        member this.Validate () =
            match this.HasErrors () with
            | Some e -> text e |> RequestErrors.badRequest |> Error
            | None -> Ok this

// An in-progress character build.
type CharacterBuild = {
    Id: string
    Character: Character
    Improvements: Improvement2 list
}
with
    static member Create name level =
        let c, imps = Build.start name (level * 1<Level>)
        {
            Id = Guid.NewGuid().ToString()
            Character = c
            Improvements = imps
        }

// Mongo query types.
// See https://medium.com/@leocavalcante/rest-api-with-mongodb-and-f-on-net-core-605a2336f264
// for how this kind of thing is done
type CharacterBuildSave = CharacterBuild -> CharacterBuild

[<CLIMutable>]
type CharacterBuildCriteria = { Id: string }

type CharacterBuildFind = CharacterBuildCriteria -> CharacterBuild []
