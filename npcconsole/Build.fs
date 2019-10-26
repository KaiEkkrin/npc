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
        if imp.Count = 0 then c // No improvements to apply
        else
            // Create a list of the names of the choices that can actually be
            // applied to this character:
            let applicable = imp.Choices |> List.filter (fun (_, req, _) -> req c)
            let applicCount = List.length applicable

            // Get the next one to be applied to the character, interrogating the user if need be
            let chosenName =
                if applicCount < imp.Count then failwithf "%s : Needed at least %d choices, have %d" imp.Prompt imp.Count applicCount
                elif applicCount = imp.Count then applicable |> List.head |> fun (n, _, _) -> n
                else interact.Prompt (imp.Prompt, (applicable |> List.map (fun (n, _, _) -> n)))

            let chosenFunc = applicable |> List.pick (fun (n, _, fn) -> if n = chosenName then Some fn else None)

            // Apply that improvement, and then continue to prompt for the rest.
            // As we go, we'll edit the prompts of sub-improvements so that the user
            // can see the breadcrumb trail.  (TODO make this a list for formatting elsewhere?)
            let remaining = imp.Choices |> List.filter (fun (n, _, _) -> n <> chosenName )
            (chosenFunc c)
            |> (fun (c2, i2s) -> c2, i2s |> List.map (fun i2 -> { i2 with Prompt = sprintf "%s -> %s" imp.Prompt i2.Prompt }))
            >>= (prompt { imp with Choices = remaining; Count = imp.Count - 1 })

    // Character-improvement bind.
    and (>>=) (c, i) f =
        match i with
        | [] -> f c // no more improvements to apply
        | imp::imps ->
            // Prompt for this improvement, then apply the others:
            (prompt imp c, imps) >>= f

    // The canonical ability order (useful for display)
    static member AbilityOrder = [Strength; Dexterity; Constitution; Intelligence; Wisdom; Charisma]

    // Creates a starting-level character.
    member this.Start name =
        ({
            Name = name
            Ancestry = None
            Heritage = None
            Background = None
            Class = None
            Level = 1<Level>
            HitPoints = { Flat = 0; PerLevel = 0 }
            Size = None
            Speed = 0<Feet>
            Abilities = Builder.AbilityOrder |> List.map (fun a -> a, 10<Score>) |> Map.ofList
            Skills = Map.empty
            Feats = []
            Weapons = Weapons.all
            Armor = None
        }, [
            Ancestry.ancestries
            Background.backgrounds
            Classes.classes
            Improve.addArmor Armors.allArmors
        ])
        >>= id

    // TODO level-up a character, etc.

