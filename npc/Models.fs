namespace Npc

// Model classes helpful for using the character builder from C#.

type PromptModel = {
    Prompt: string
    Choices: string list
}
with
    static member CanCreate (bo: BuildOutput) =
        match bo with | CompletedCharacter _ -> false | _ -> true

    static member Create (bo: BuildOutput) =
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
