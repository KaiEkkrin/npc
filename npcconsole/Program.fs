// Learn more about F# at http://fsharp.org

namespace NpcConsole

open System
open System.IO
open System.Text.Json
open System.Text.Json.Serialization
open Npc
open Npc.Attributes

// I have no clue why this isn't in the standard library
type ResultBuilder () =
    member this.Bind (x, f) =
        match x with
        | Ok y -> f y
        | Error e -> Error e

    member this.Return x = Ok x
    member this.ReturnFrom x = x

module Interactive =
    // Prompts interactively for one choice out of many.
    let showPrompt (prompt, choices) =
        printfn "Choose %s:" prompt

        // Number all the choices, starting at 1:
        let numberedChoices = choices |> List.mapi (fun i ch -> i + 1, ch)
        numberedChoices |> List.iter (fun (n, ch) -> printfn "%4d. %A" n ch)

        // Read in the first intelligible number the user types that
        // maps to a choice, and return that:
        seq { while true do yield Console.ReadLine () }
        |> Seq.choose (fun l -> match Int32.TryParse l with | true, n -> Some n | _ -> None)
        |> Seq.pick (fun n -> List.tryPick (fun (n2, ch) -> if n2 = n then Some ch else None) numberedChoices)

    // Builds a character interactively.
    let rec build (chosen, c, imps) =
        match Build.build (chosen, c, imps) with
        | MakeChoice (p, chs, c, imps) -> prompt (p, chs, c, imps)
        | BadChoice (p, chs, c, imps) ->
            printfn "Bad choice, try again."
            prompt (p, chs, c, imps)
        | CompletedCharacter c -> c

    and prompt (p, chs, c, imps) =
        let ch = showPrompt (p, chs)
        build (Some ch, c, imps)

    // Shows a character sheet in a text writer.
    let showCharacter (f, c) =
        let sheet = CharacterSheet.create c

        // I'll right justify the names and left justify the values:
        let maxNameLength =
            sheet
            |> List.collect (fun s -> s.Items)
            |> List.fold (fun acc (n, _) -> max acc n.Length) 0

        for s in sheet do
            if s.Title.Length > 0 then fprintfn f "%s" s.Title
            for (n, v) in s.Items do
                fprintfn f "%s : %s" (n.PadLeft maxNameLength) v

// Arguments
type Args = {
    Name: string option
    Level: int<Level>
    JsonIn: string option
    JsonOut: string option
    Output: string option
}

module Program =
    let result = ResultBuilder ()
    let serializerOptions =
        let o = JsonSerializerOptions ()
        o.Converters.Add (JsonFSharpConverter ())
        o

    let parseArgs argv =
        let rec parse argv args =
            match argv with
            | [] -> Ok args
            | "--name"::name::argvs -> { args with Args.Name = Some name } |> parse argvs
            | "--level"::l::argvs -> // TODO custom pattern thingy?
                match Int32.TryParse l with
                | true, level -> { args with Args.Level = level * 1<Level> } |> parse argvs
                | _ -> sprintf "Invalid option : --level %s" l |> Error
            | "--json-in"::f::argvs -> { args with Args.JsonIn = Some f } |> parse argvs
            | "--json-out"::f::argvs -> { args with Args.JsonOut = Some f } |> parse argvs
            | "--out"::o::argvs -> { args with Args.Output = Some o } |> parse argvs
            | arg::_ -> sprintf "Invalid option : %s" arg |> Error
        parse (List.ofArray argv) { Name = None; Level = 1<Level>; JsonIn = None; JsonOut = None; Output = None }

    let usage () =
        printfn "Usage:"
        printfn "--name <name>"
        printfn "    Specify a character name."
        printfn "--level <level>"
        printfn "    Specify a character level (default 1)."
        printfn "--json-in <filename>"
        printfn "    Specify a JSON input filename (with character build output)."
        printfn "--json-out <filename>"
        printfn "    Specify a JSON output filename (we save character build output here)."
        printfn "--out <filename>"
        printfn "    Specify an output file (default none)."
        0

    let readJsonIn (f: string) =
        try
            use sr = new StreamReader (f)
            let json = sr.ReadToEnd ()
            JsonSerializer.Deserialize<Character * Improvement2 list> (json, serializerOptions) |> Ok
        with
            | e -> sprintf "Error reading %s : %s" f e.Message |> Error

    let writeJsonOut (f: string) o =
        let json = JsonSerializer.Serialize<Character * Improvement2 list> (o, serializerOptions)
        try
            use sw = new StreamWriter (f)
            sw.Write json
            Ok ()
        with
            | e -> sprintf "Error writing %s : %s" f e.Message |> Error

    let showCharacter args c =
        match args.Output with
        | Some o ->
            use f = new StreamWriter (o)
            Interactive.showCharacter (f :> TextWriter, c)
        | None ->
            Interactive.showCharacter (Console.Out, c)

    let getStartingPoint args = result {
        match args.JsonIn, args.Name, args.Level with
        | Some f, _, lv ->
            let! c, imps = readJsonIn f 
            if lv > c.Level then
                let levelUp = Build.levelUp lv c
                return (c, List.append imps levelUp)
            else return (c, imps)
        | None, Some name, lv -> return Build.start name lv
        | _ -> return! Error "Need a character name or a json input"
    }

    let run argv = result {
        let! args = parseArgs argv
        let! start, imps = getStartingPoint args
        let c = Interactive.build (None, start, imps)

        // Write out the output, if requested
        let! written =
            match args.JsonOut with
            | Some f -> writeJsonOut f (c, [])
            | None -> Ok ()

        do showCharacter args c
        return written
    }

    [<EntryPoint>]
    let main argv =
        match run argv with
        | Ok _ -> 0
        | Error e ->
            printfn "%s" e
            usage ()
