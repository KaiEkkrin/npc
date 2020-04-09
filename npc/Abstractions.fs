namespace Npc

open System
open Npc.Attributes

// System-agnostic types and interfaces suitable for calling through from the Web application and
// for unit testing.

type PromptModel = {
    Prompt: string
    Choices: string list
}

type IBuildAbstraction =
    // True if this build has a prompt ready, else false (it's complete).
    abstract member CanPrompt : unit -> bool

    // Gets the next prompt from this build.
    abstract member Prompt : unit -> PromptModel

    // Continues the build by choosing the identified option from the prompt.
    abstract member Continue : string -> IBuildAbstraction

    // Creates the summary text to store in a build record.
    abstract member Summarise : unit -> string

    // Creates a character sheet from this build output.
    abstract member CreateCharacterSheet : unit -> CharacterSubHeading list

type IBuildDriver =
    // Starts a new character build by name and level.
    abstract member Create : string * int -> IBuildAbstraction

type BuildAbstraction(bo: BuildOutput) =
    let build bo choice =
        let choose (chs: Change2 list) = chs |> List.find (fun ch -> ch.AsString = choice) |> Some
        match bo with
        | MakeChoice (_, chs, c, imps) -> Build.build (choose chs, c, imps)
        | BadChoice (_, chs, c, imps) -> Build.build (choose chs, c, imps)
        | _ -> raise <| InvalidOperationException("Character already completed")

    interface IBuildAbstraction with
        member this.CanPrompt() =
            match bo with | CompletedCharacter _ -> false | _ -> true

        member this.Prompt() =
            match bo with
            | MakeChoice (prompt, chs, _, _) -> {
                Prompt = prompt
                Choices = chs |> List.map (fun ch -> ch.AsString)
                }
            | BadChoice (prompt, chs, _, _) -> {
                Prompt = prompt
                Choices = chs |> List.map (fun ch -> ch.AsString)
                }
            | _ -> failwith "Cannot create prompt for a completed character"

        member this.Continue(choice: string) = build bo choice |> BuildAbstraction :> IBuildAbstraction

        member this.Summarise() =
            match bo with
            | MakeChoice (_, _, c, _) -> CharacterSheet.printSummary c |> snd
            | BadChoice (_, _, c, _) -> CharacterSheet.printSummary c |> snd
            | CompletedCharacter c -> CharacterSheet.printSummary c |> snd

        member this.CreateCharacterSheet() =
            match bo with
            | MakeChoice (_, _, c, _) -> CharacterSheet.create c
            | BadChoice (_, _, c, _) -> CharacterSheet.create c
            | CompletedCharacter c -> CharacterSheet.create c

type BuildDriver() =
    interface IBuildDriver with
        member this.Create(name: string, level: int) =
            if level < 1 || level > 20 then raise <| ArgumentOutOfRangeException("level", "Level must be between 1 and 20")
            else
                let c, imps = Build.start name (level * 1<Level>)
                Build.build (None, c, imps) |> BuildAbstraction :> IBuildAbstraction
