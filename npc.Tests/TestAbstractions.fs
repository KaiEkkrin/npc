namespace Npc.Tests

open System
open Npc
open Xunit.Abstractions

type TestBuildAbstraction(output: ITestOutputHelper, name: string, level: int, choices: string list) =
    interface IBuildAbstraction with
        member this.CanPrompt() = true
        member this.Prompt() = raise <| NotImplementedException()
        member this.Continue(choice: string) = TestBuildAbstraction(output, name, level, choice::choices) :> IBuildAbstraction

        member this.Summarise() =
            let summary =
                match choices with
                | [] -> ""
                | _ -> choices |> List.reduce (fun a b -> sprintf "%s %s" b a)
            output.WriteLine ("Summarising {0} at level {1} as {2}", name, level, summary)
            summary

        member this.CreateCharacterSheet() =
            [{
                Title = "Basics"
                Items = ["Name", name; "Level", sprintf "%d" level]
            }] @ (choices |> List.rev |> List.map (fun ch -> {
                Title = ch
                Items = ["1", ch]
            }))

type TestBuildDriver(output: ITestOutputHelper) =
    interface IBuildDriver with
        member this.Create(name: string, level: int) =
            TestBuildAbstraction(output, name, level, []) :> IBuildAbstraction
