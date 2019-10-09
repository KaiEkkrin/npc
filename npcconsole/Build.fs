namespace NpcConsole

open NpcConsole.Attributes

// Describes the interactive UI with the player.
type IInteraction =
    interface

    // Offers the player, under a prompt, a selection of options so that
    // they can pick one (returned).
    abstract member Prompt : string * string list -> string

    // Shows the final character to the player.
    abstract member Show : Character -> unit

    end

// This builder wraps up the UI interaction and lets us express how to
// build and improve a character more cleanly (I hope!)
// - The wrapped expression is a (Character, Improvement list), which includes a character
// and their stack of improvements
// - The unwrapped expression is a Character, with the desired improvement(s)
// applied
type Builder (interact: IInteraction) =
    // How to prompt the user which improvement(s) they want and apply them
    // recursively.
    let rec prompt imp c =
        // Create a list of the names of the choices that can actually be
        // applied to this character:
        let applicable = imp.Choices |> List.filter (fun (_, req, _) -> req c)
        let applicCount = List.length applicable

        // Get the next one to be applied to the character, interrogating the user if need be
        let chosenName =
            if applicCount < imp.Count then failwithf "%s : Needed at least %d choices, have %d" imp.Prompt imp.Count applicCount
            elif applicCount = imp.Count then applicable |> List.head |> fun (n, _, _) -> n
            else interact.Prompt (imp.Prompt, (applicable |> List.map (fun (n, _, _) -> n)))

        let chosenFunc =
            applicable
            |> List.choose (fun (n, _, fn) -> if n = chosenName then Some fn else None)
            |> List.head

        // Apply that improvement, and then continue to prompt for the rest:
        (chosenFunc c) >>= (prompt { imp with Choices = imp.Choices |> List.filter (fun (n, _, _) -> n <> chosenName ) })

    // Character-improvement monad.
    and (>>=) (c, i) f =
        match i with
        | [] -> f c // no more improvements to apply
        | imp::imps ->
            // Prompt for this improvement, then apply the others:
            (prompt imp c, imps) >>= f

    // Creates a starting-level character.
    member this.Start name =
        ({
            Name = name
            Ancestry = None
            Heritage = None
            Background = None
            Class = None
            Level = 1<Level>
            HitPoints = 0
            Size = None
            Speed = 0<Feet>
            Abilities = [Strength; Dexterity; Constitution; Intelligence; Wisdom; Charisma] |> List.map (fun a -> a, 10<Score>) |> Map.ofList
            Skills = Map.empty
            Feats = []
        }, [
            Ancestry.ancestries
        ])
        >>= id

    // TODO level-up a character, etc.

