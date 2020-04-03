namespace Npc

open System
open Npc.Attributes

// A helper interface so that we can run character builds from the C#.
type IBuildDriver =
    // Starts a new character build by name and level.
    abstract member Create : string * int -> BuildOutput

    // Continues a character build by choosing the identified option.
    abstract member Continue : BuildOutput * string -> BuildOutput

    // Constructs a character by applying the array of choices in order.
    abstract member Construct : BuildOutput * string seq -> BuildOutput

    // Creates the summary text to store in a build record.
    abstract member Summarise : BuildOutput -> string

type BuildDriver() =
    let build bo choice =
        let choose (chs: Change2 list) = chs |> List.find (fun ch -> ch.AsString = choice) |> Some
        match bo with
        | MakeChoice (_, chs, c, imps) -> Build.build (choose chs, c, imps)
        | BadChoice (_, chs, c, imps) -> Build.build (choose chs, c, imps)
        | _ -> raise <| InvalidOperationException("Character already completed")

    interface IBuildDriver with
        member this.Create(name: string, level: int) =
            if level < 1 || level > 20 then raise <| ArgumentOutOfRangeException("level", "Level must be between 1 and 20")
            else
                let c, imps = Build.start name (level * 1<Level>)
                Build.build (None, c, imps)

        member this.Continue(bo: BuildOutput, choice: string) = build bo choice
        member this.Construct(bo: BuildOutput, choices: string seq) = choices |> Seq.fold build bo

        member this.Summarise(bo: BuildOutput) =
            match bo with
            | MakeChoice (_, _, c, _) -> CharacterSheet.printSummary c |> snd
            | BadChoice (_, _, c, _) -> CharacterSheet.printSummary c |> snd
            | CompletedCharacter c -> CharacterSheet.printSummary c |> snd
