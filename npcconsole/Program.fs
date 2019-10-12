// Learn more about F# at http://fsharp.org

namespace NpcConsole

open System
open NpcConsole.Attributes

// A console interaction.
type ConsoleInteract () = 
    interface IInteraction with
        member this.Prompt (prompt, choices) =
            printfn "Choose %s:" prompt

            // Number all the choices, starting at 1:
            let numberedChoices = choices |> List.mapi (fun i ch -> i + 1, ch)
            numberedChoices |> List.iter (fun (n, ch) -> printfn "%4d. %s" n ch)

            // Read in the first intelligible number the user types that
            // maps to a choice, and return that:
            seq { while true do yield Console.ReadLine () }
            |> Seq.choose (fun l -> match Int32.TryParse l with | true, n -> Some n | _ -> None)
            |> Seq.pick (fun n -> List.tryPick (fun (n2, ch) -> if n2 = n then Some ch else None) numberedChoices)

        member this.Show c =
            printfn ""
            printfn "%s" c.Name
            if Option.isSome c.Heritage
            then printfn "Level %d %s %s" c.Level c.Heritage.Value c.Ancestry.Value
            else printfn "Level %d %s" c.Level c.Ancestry.Value
            printfn "  %15s %d" "Hit Points" c.HitPoints
            printfn "  %15s %A" "Size" c.Size.Value
            printfn "  %15s %d" "Speed" c.Speed
            printfn "Abilities:"
            Builder.AbilityOrder |> List.iter (fun ab ->
                let score = Map.find ab c.Abilities
                printfn "  %15s %4d (%+3d)" (ab.ToString ()) score (Derive.modifier score)
            )
            // TODO Armor, saving throws, weapon skills and that kind of thing
            // Regular skills, in alphabetical order
            printfn "Skills:"
            Skills.regularSkillsForCharacter c |> List.iter (fun sk ->
                let rank = Derive.rank sk c
                let bonus = Derive.bonus sk c
                printfn "  %20s %+3d %15s (%15s)" sk.Name bonus (rank.ToString ()) (sk.KeyAbility.ToString ())
            )
            // All feats, in alphabetical order
            printfn "Feats:"
            c.Feats |> List.sortBy (fun f -> f.Name) |> List.iter (fun f ->
                printfn "  %30s [%A]" f.Name f.Category
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
                let build = Builder interact
                let c = build.Start args.Name.Value
                interact.Show c
                0
