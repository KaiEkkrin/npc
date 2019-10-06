// Learn more about F# at http://fsharp.org

namespace NpcConsole

open System
open NpcConsole.Attributes

// A console interaction.
type ConsoleInteract () = 
    interface IInteraction with
        member this.Prompt (prompt, choices, count) =
            printfn "Choose %d of %s:" count prompt

            // Number all the choices, starting at 1:
            let numberedChoices = choices |> List.mapi (fun i ch -> i + 1, ch)
            numberedChoices |> List.iter (fun (n, ch) -> printfn "%4d. %s" n ch)

            // Read them all in.  Make sure we can't read duplicate choices.
            seq { while true do yield Console.ReadLine () }
            |> Seq.scan (fun (chosen, ncs) l ->
                match Int32.TryParse l with
                | false, _ -> (chosen, ncs)
                | true, n ->
                    match List.tryFind (fun (m, ch) -> m = n) ncs with
                    | None -> (chosen, ncs)
                    | Some (m, ch) -> (ch::chosen, List.filter (fun (m, ch) -> m <> n) ncs)
            ) ([], numberedChoices)
            |> Seq.find (fun (chosen, ncs) -> (List.length chosen) = count)
            |> fst

        member this.Show c =
            printfn ""
            printfn "%s" c.Name
            printfn "Level %d %s" c.Level c.Ancestry.Value.Name
            printfn "  %15s %d" "Hit Points" c.HitPoints
            printfn "  %15s %A" "Size" c.Size.Value
            printfn "  %15s %d" "Speed" c.Speed
            printfn "Abilities:"
            Interact.abilityOrder |> List.iter (fun ab ->
                let score = Map.find ab c.Abilities
                printfn "  %15s %4d (%+2d)" (ab.ToString ()) score (Derive.modifier score)
            )
            printfn "Feats:"
            c.Feats |> List.sortBy (fun f -> f.Name) |> List.iter (fun f ->
                printfn "  %30s [%s]" f.Name f.Category
            )

// Arguments
type Args = {
    Name: string option
}

type ArgParseResult = | Parsed of Args | Failed of string

module Program =
    let parseArgs argv =
        let rec parse argv args =
            match argv with
            | [] -> Parsed args
            | "--name"::name::argvs -> { args with Args.Name = Some name } |> parse argvs
            | arg::argvs -> Failed arg
        parse (List.ofArray argv) { Name = None }

    let usage () =
        printfn "Usage:"
        printfn "--name <name>"
        printfn "    Specify a character name."
        0

    [<EntryPoint>]
    let main argv =
        printfn "Hello World from F#!  %A" argv
        match (parseArgs argv) with
        | Failed str ->
            printfn "Unrecognised argument: %s" str
            usage ()
        | Parsed args ->
            if args.Name = None then
                printfn "No name specified"
                usage ()
            else
                // We create a base character, and then interact with the user to
                // offer them options, thus
                let interact = ConsoleInteract () :> IInteraction
                let c = Build.start args.Name.Value |> Build.build interact
                interact.Show c
                0
