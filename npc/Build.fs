namespace Npc

open System.IO
open Npc.Attributes

// Describes the interactive UI with the player.
type IInteraction =
    interface

    // Offers the player, under a prompt, a selection of options so that
    // they can pick one (returned).
    abstract member Prompt : string * string list -> string

    // Shows the final character to the player.
    abstract member Show : TextWriter * Character -> unit

    end

// This builder wraps up the UI interaction and lets us express how to
// build and improve a character more cleanly (I hope!)
// - The wrapped expression is a (Character, Improvement list), which includes a character
// and their stack of improvements
// - The unwrapped expression is a Character, with the desired improvement(s)
// applied
type Builder (interact: IInteraction) =
    // Prompts for one choice out of several:
    let prompt title chs =
        let named = chs |> List.map (fun ch -> sprintf "%A" ch, ch)
        let mapped = named |> Map.ofList
        let chosen = interact.Prompt (title, named |> List.map fst)
        Map.find chosen mapped

    // Improves a character, taking the improvements in order until
    // we run out
    let rec improve imps c =
        match imps with
        | [] -> c
        | imp::left ->
            // Work out what choices we have
            let possible = imp.Choices |> List.filter (fun ch -> ch.CanApply c)
            let possibleCount = List.length possible

            // Work out which one we will apply
            let chosen =
                match imp.Count, possible with
                | None, [] -> None
                | None, ch::_ -> Some ch
                | Some 0, _ -> None
                | Some n, ch::_ when n = possibleCount -> Some ch
                | Some n, _ when n > possibleCount ->
                    failwithf "%s : Wanted %d, only %d possible" imp.Prompt n possibleCount
                | Some _, chs ->
                    // TODO I need to be able to split this into "show next prompt"
                    // and "apply given choice (from that prompt)"
                    Some (prompt imp.Prompt chs)

            // Do it and either continue with this improvement, or move on
            // to the next one
            match chosen with
            | None -> improve left c
            | Some ch ->
                let updated, more = ch.Apply c
                let continued = {
                    Prompt = imp.Prompt
                    Choices = imp.Choices |> List.filter (fun ch2 -> ch2 <> ch)
                    Count = match imp.Count with | Some n -> Some (n - 1) | None -> None
                }
                improve (List.concat [more; [continued]; left]) updated

    // The canonical ability order (useful for display)
    static member AbilityOrder = [Strength; Dexterity; Constitution; Intelligence; Wisdom; Charisma]

    // Builds a level up of an existing character to a given level, returning
    // the required improvements.
    member this.LevelUp newLv c =
        let oldLv = c.Level
        List.unfold (fun lv ->
            if lv <= newLv then Some (Classes.All.classes, lv + 1<Level>)
            else None) oldLv

    // Starts a character build, emitting (character, list of improvements
    // that need applying)
    member this.Start name level =
        let c = {
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
            MeleeWeapon = None
            RangedWeapon = None
            Armor = None
            SpellSkill = None
            Spells = Map.empty
            Pools = Map.empty
        }
        let firstLevel = [
            Ancestry.ancestries
            Background.backgrounds
            Classes.All.classes
            Armors.addArmor
            Improve2.single Weapons.addMeleeWeapon
            Improve2.single Weapons.addRangedWeapon
        ]
        let otherLevels = c |> this.LevelUp (level - 1<Level>)
        c, List.append firstLevel otherLevels

    // Builds a complete character using our interact, given a pair
    // (character, improvement list).
    member this.Build (c, imps) = improve imps c
