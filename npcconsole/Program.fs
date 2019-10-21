// Learn more about F# at http://fsharp.org

namespace NpcConsole

open System
open NpcConsole.Attributes

// A console interaction.
type ConsoleInteract () = 
    let formatLevel c = Some (sprintf "Level %d" c.Level)
    let formatClass c = match c.Class with | Some cl -> Some (sprintf "%A" cl) | None -> None
    let print (name, value) = printfn "%30s: %s" name value

    let printTitle c =
        let elements =
            [formatLevel c; c.Heritage; c.Ancestry; formatClass c]
            |> List.choose id
        print ("Is a", String.Join (" ", elements))

    let printSkill c sk =
        let rank = (Derive.rank sk c).ToString ()
        let bonus = Derive.bonus sk c
        print (sk.Name, sprintf "%+3d %15s (%15s)" bonus rank (sk.KeyAbility.ToString ()))

    let printDC c sk =
        let rank = (Derive.rank sk c).ToString ()
        let dc = Derive.bonus sk c |> Derive.dc
        print ((sprintf "%s DC" sk.Name), sprintf "%3d %15s (%15s)" dc rank (sk.KeyAbility.ToString ()))

    let printClassDC c =
        match Map.tryPick (fun (sk: Skill) _ -> if sk.Name.Contains ("Class") then Some sk else None) c.Skills with
        | Some sk -> printDC c sk
        | None -> ()

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
            print ("Name", c.Name)
            printTitle c
            if Option.isSome c.Background then print ("Background", c.Background.Value)
            print ("Hit Points", sprintf "%d" (Derive.hitPoints c))
            if Option.isSome c.Size then print ("Size", c.Size.Value.ToString ())
            print ("Speed", c.Speed.ToString ())
            printfn "Abilities:"
            Builder.AbilityOrder |> List.iter (fun ab ->
                let score = Map.find ab c.Abilities
                print (ab.ToString (), sprintf "%4d (%+2d)" score (Derive.modifier score))
            )
            printfn "Difficulty Classes:"
            // TODO armor class here
            printClassDC c
            printfn "Saves:"
            Skills.saves |> List.iter (printSkill c)
            // TODO Armor, saving throws, weapon skills and that kind of thing
            // Regular skills, in alphabetical order
            printfn "Skills:"
            printSkill c Skills.perception
            Skills.regularSkillsForCharacter c |> List.iter (printSkill c)
            // All feats, in alphabetical order
            // TODO separate class features and that kind of thing?  Show in order
            // of what level they were taken at, instead?  Show page numbers (that
            // would be really useful for checking what they all mean!)
            printfn "Feats:"
            c.Feats |> List.sortBy (fun f -> f.Name) |> List.iter (fun f ->
                print (f.Name, sprintf "page %3d" f.Page)
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
