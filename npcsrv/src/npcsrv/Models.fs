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

// A character improvement request
[<CLIMutable>]
type CharacterImproveRequest = {
    Id: string
    Choice: string
}

// A character build.
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

// We don't reply with a whole character build but rather, this build
// response containing a character sheet and possibly the next question
type CharacterBuildResponsePrompt = {
    Prompt: string
    Choices: string list
}

type CharacterBuildResponse = {
    Id: string
    Sheet: CharacterSubHeading list
    Prompt: CharacterBuildResponsePrompt option
    Error: string // empty for no error
}
with
    static member Update (choiceStr: string) (bld: CharacterBuild) =
        let promptOf (p, chs: Change2 list) = Some {
            Prompt = p
            Choices = chs |> List.map (fun ch -> ch.AsString)
        }

        let prompted err p = {
            Id = bld.Id
            Sheet = CharacterSheet.create bld.Character
            Prompt = p
            Error = err
        }

        let doBuild choice =
            match Build.build (choice, bld.Character, bld.Improvements) with
            | MakeChoice (p, chs, c, imps) ->
                promptOf (p, chs) |> (prompted ""), { bld with Character = c; Improvements = imps }
            | BadChoice (p, chs, c, imps) ->
                promptOf (p, chs) |> (prompted "Bad choice"), { bld with Character = c; Improvements = imps }
            | CompletedCharacter c -> (prompted "" None), { bld with Character = c; Improvements = [] }

        match choiceStr, bld.Improvements with
        | "", _ -> doBuild None
        | _, [] -> doBuild None
        | str, imp::_ ->
            match imp.Choices |> List.tryFind (fun ch -> ch.AsString = str) with
            | None -> (prompted (sprintf "Invalid choice: %s" str) None), bld
            | Some ch -> doBuild (Some ch)

    static member Of choiceStr bld = CharacterBuildResponse.Update choiceStr bld |> fst

// Mongo query types.
// See https://medium.com/@leocavalcante/rest-api-with-mongodb-and-f-on-net-core-605a2336f264
// for how this kind of thing is done
type CharacterBuildSave = CharacterBuild -> CharacterBuild

[<CLIMutable>]
type CharacterBuildCriteria = { Id: string }

type CharacterBuildFind = CharacterBuildCriteria -> CharacterBuild []
