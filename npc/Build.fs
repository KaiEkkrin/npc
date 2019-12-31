namespace Npc

open Npc.Attributes

// We build characters out of a base character and a list of improvements
// by applying the improvements in order until we reach one that needs
// user interaction.  Thus, a character build step can either return
// an incomplete character with the prompt required, a complete character,
// or an error if the response to an interactive request isn't valid:
type BuildOutput =
    MakeChoice of string * Change2 list * Character * Improvement2 list // choose one of the changes
    | BadChoice of string * Change2 list * Character * Improvement2 list // try again
    | CompletedCharacter of Character

module Build =
    // Improves a character, taking the improvements in order until
    // we run out.  `choice` is the latest user input.
    let rec build (choice, c, imps) =
        match choice, imps with
        | _, [] -> CompletedCharacter c
        | Some ch, imp::left when List.contains ch imp.Choices && ch.CanApply c -> apply (ch, c, imp, left)
        | _, imp::left ->
            // Work out what choices we have
            let possible = imp.Choices |> List.filter (fun ch -> ch.CanApply c)
            let possibleCount = List.length possible

            match imp.Count, possible with
            | None, [] -> build (None, c, left)
            | None, ch::_ -> apply (ch, c, imp, left)
            | Some 0, _ -> build (None, c, left)
            | Some n, ch::_ when n = possibleCount -> apply (ch, c, imp, left)
            | Some n, _ when n > possibleCount ->
                failwithf "%s : Wanted %d, only %d possible" imp.Prompt n possibleCount
            | Some _, chs ->
                let choiceResult = if Option.isSome choice then BadChoice else MakeChoice
                choiceResult (imp.Prompt, chs, c, imps)

    and apply (ch, c, imp, left) =
        let updated, more = ch.Apply c
        let continued = {
            Prompt = imp.Prompt
            Choices = imp.Choices |> List.filter (fun ch2 -> ch2 <> ch)
            Count = match imp.Count with | Some n -> Some (n - 1) | None -> None
        }
        build (None, updated, List.concat [more; [continued]; left])

    // The canonical ability order (useful for display)
    let abilityOrder = [Strength; Dexterity; Constitution; Intelligence; Wisdom; Charisma]

    // Builds a level up of an existing character to a given level, returning
    // the required improvements.
    let levelUp newLv c =
        let oldLv = c.Level
        List.unfold (fun lv ->
            if lv <= newLv then Some (Classes.All.classes, lv + 1<Level>)
            else None) oldLv

    // Starts a character build, emitting (character, list of improvements
    // that need applying)
    let start name level =
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
            Abilities = abilityOrder |> List.map (fun a -> a, 10<Score>) |> Map.ofList
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
        let otherLevels = c |> levelUp (level - 1<Level>)
        c, List.append firstLevel otherLevels
