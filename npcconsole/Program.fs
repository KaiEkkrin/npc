// Learn more about F# at http://fsharp.org

namespace NpcConsole

open System
open System.IO

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

        member this.Show (f, c) =
            let sheet = CharacterSheet.create c

            // I'll right justify the names and left justify the values:
            let maxNameLength =
                sheet
                |> List.collect (fun s -> s.Items)
                |> List.fold (fun acc (n, _) -> max acc n.Length) 0

            for s in sheet do
                match s.Title with | Some t -> fprintfn f "%s" t | None -> ()
                for (n, v) in s.Items do
                    fprintfn f "%s : %s" (n.PadLeft maxNameLength) v

// Arguments
type Args = {
    Name: string option
    Level: int
    Output: string option
}

type ArgParseResult = | Parsed of Args | Failed of string

module Program =
    let parseArgs argv =
        let rec parse argv args =
            match argv with
            | [] -> Parsed args
            | "--name"::name::argvs -> { args with Args.Name = Some name } |> parse argvs
            | "--level"::l::argvs -> // TODO custom pattern thingy?
                match Int32.TryParse l with
                | true, level -> { args with Args.Level = level } |> parse argvs
                | _ -> Failed "--level"
            | "--out"::o::argvs -> { args with Args.Output = Some o } |> parse argvs
            | arg::_ -> Failed arg
        parse (List.ofArray argv) { Name = None; Level = 1; Output = None }

    let usage () =
        printfn "Usage:"
        printfn "--name <name>"
        printfn "    Specify a character name."
        printfn "--level <level>"
        printfn "    Specify a character level (default 1)."
        printfn "--out <filename>"
        printfn "    Specify an output file (default none)."
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
                let c = build.Build (args.Name.Value, args.Level)
                match args.Output with
                | Some o ->
                    use f = new StreamWriter (o)
                    interact.Show (f :> TextWriter, c)
                | None ->
                    interact.Show (Console.Out, c)
                0
