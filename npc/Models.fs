namespace Npc

open System
open System.Text.Json
open System.Text.Json.Serialization
open Npc.Attributes

// Model classes helpful for using the character builder from C#.

type ChoiceModel = {
    Index: int
    Choice: string
}

type PromptModel = {
    Prompt: string
    Choices: ChoiceModel list
}
with
    static member MakeChoiceModels chs = chs |> List.mapi (fun i ch -> {
        Index = i
        Choice = sprintf "%A" ch
    })

    static member CanCreate (bo: BuildOutput) =
        match bo with | CompletedCharacter _ -> false | _ -> true

    static member Create (bo: BuildOutput) =
        match bo with
        | MakeChoice (prompt, chs, _, _) -> {
            Prompt = prompt
            Choices = PromptModel.MakeChoiceModels chs
            }
        | BadChoice (prompt, chs, _, _) -> {
            Prompt = prompt
            Choices = PromptModel.MakeChoiceModels chs
            }
        | _ -> failwith "Cannot create prompt for a completed character"

// A helper interface so that we can run character builds from the C#.
type IBuildDriver =
    // Starts a new character build by name and level.
    abstract member Create : string * int -> BuildOutput

    // Continues a character build by choosing the numbered option.
    abstract member Continue : BuildOutput * int -> BuildOutput

    // Deserializes build output.
    abstract member Deserialize : string -> BuildOutput

    // Serializes build output.
    abstract member Serialize : BuildOutput -> string

type BuildDriver() =
    let serializerOptions =
        let o = JsonSerializerOptions()
        o.Converters.Add(JsonFSharpConverter())
        o

    interface IBuildDriver with
        member this.Create(name: string, level: int) =
            if level < 1 || level > 20 then raise <| ArgumentOutOfRangeException("level", "Level must be between 1 and 20")
            else
                let c, imps = Build.start name (level * 1<Level>)
                Build.build (None, c, imps)

        member this.Continue(bo: BuildOutput, index: int) =
            match bo with
            | MakeChoice (_, chs, c, imps) -> Build.build (Some chs.[index], c, imps)
            | BadChoice (_, chs, c, imps) -> Build.build (Some chs.[index], c, imps)
            | _ -> raise <| InvalidOperationException("Character already completed")

        member this.Deserialize(str: string) =
            JsonSerializer.Deserialize<BuildOutput>(str, serializerOptions)

        member this.Serialize(bo: BuildOutput) =
            JsonSerializer.Serialize<BuildOutput>(bo, serializerOptions)
