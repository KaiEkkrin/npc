namespace Npc

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
