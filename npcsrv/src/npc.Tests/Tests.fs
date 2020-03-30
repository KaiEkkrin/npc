module Tests

open System
open Npc
open Npc.Attributes
open Xunit
open Xunit.Abstractions

type BuildTests (output: ITestOutputHelper) =
    static member TestBuilds : obj [] list =
        // We'll try 100 random builds at each level:
        let rng = (Guid.Parse "12914db8-7bcc-4ecb-bcc7-bbc0921611d4").GetHashCode() |> Random
        [1..10]
        |> List.collect (fun level ->
            [1..100] |> List.map (fun _ -> [| level; rng.Next() |]) 
        )

    [<Theory; MemberData("TestBuilds")>]
    member this.``A character can always be built successfully`` (level: int, seed: int) =
        let rng = Random seed
        let name = sprintf "Test %d" seed
        let makeChoice chs =
            let index = List.length chs |> rng.Next
            chs.[index]

        let rec doBuild (ch, c, imps) =
            match Build.build (ch, c, imps) with
            | MakeChoice (prompt, chs, c, imps) ->
                let choice = makeChoice chs // randomly pick one
                output.WriteLine ("  At {0,-20} : choosing option {1}", prompt, choice)
                doBuild (Some choice, c, imps)
            | BadChoice (p, _, _, _) -> failwithf "Bad choice at %s" p // This test should make only good choices
            | CompletedCharacter c -> c

        output.WriteLine ("Building {0} at level {1}...", name, level)
        try
            let c, imps = Build.start name (level * 1<Level>)
            let cc = doBuild (None, c, imps)
            Assert.True(true)
        with
        | BuildException (c, imp) ->
            let str = Build.formatBuildException (c, imp)
            output.WriteLine ("Build exception : {0} at level {1} failed : {2}", name, level, str)
            reraise ()
