namespace Npc

open System
open Npc.Attributes

// A character sheet is a baking-down of a character into a printable list of
// items under sub-headings.
// Should be good for various display UIs (?)
type CharacterSubHeading = {
    Title: string option
    Items: (string * string) list
}

module CharacterSheet =
    let formatLevel c = Some (sprintf "Level %d" c.Level)
    let formatClass c = match c.Class with | Some cl -> Some (sprintf "%A" cl) | None -> None

    let one (n, v) = n, v.ToString ()
    let many (n, vs) = n, String.Join (" ", vs |> List.map (fun v -> v.ToString ()))

    let printTitle c =
        let elements =
            [formatLevel c; c.Heritage; c.Ancestry; formatClass c]
            |> List.choose id
        many ("Is a", elements)

    let printSkill c sk =
        let rank = (Derive.rank sk c).ToString ()
        let bonus = Derive.bonus sk c
        one (sk.Name, sprintf "%+3d %15s (%15s)" bonus rank (sk.KeyAbility.ToString ()))

    let printDC c sk =
        let rank = (Derive.rank sk c).ToString ()
        let dc = Derive.bonus sk c |> Derive.dc
        one ((sprintf "%s DC" sk.Name), sprintf "%3d %15s (%15s)" dc rank (sk.KeyAbility.ToString ()))

    let printArmorClass c = [
        match c.Armor with
        | Some a ->
            let rank = (Derive.rank a.Skill c).ToString ()
            let ac = Derive.armorClass c
            yield one ("Armor Class", sprintf "%3d %15s" ac rank)
            yield one ("Armor Type", a.Name)
            yield many ("Armor Traits", a.Traits |> List.map (fun t -> sprintf "%A" t))
        | None -> ()
    ]
    
    let printWeaponStats (weapon, c) = [
        match weapon with
        | Some w ->
            // TODO Dexterity bonus on melee weapon attack, sometimes
            let sk = Char2.weaponSkill w
            yield printSkill c sk
            // TODO When do ranged weapons gain a damage bonus?  From what?  I forget...
            let damageModifier =
                match w.Type with
                | Melee -> Map.find Strength c.Abilities |> Derive.modifier
                | Ranged -> 0<Modifier>
            let damageSpecializationBonus =
                match (c.Feats |> List.tryFind (fun f -> f.Name = "Weapon Specialization"), Map.tryFind sk c.Skills) with
                | Some _, Some Legendary -> 4<Modifier>
                | Some _, Some Master -> 3<Modifier>
                | Some _, Some Expert -> 2<Modifier>
                | _, _ -> 0<Modifier>
            yield one (sprintf "%s damage" w.Name, sprintf "%A %+2d (%A)" w.Damage (damageModifier + damageSpecializationBonus) w.DamageType)
            match w.Range with
            | Some r -> yield one (sprintf "%s Range" w.Name, sprintf "%3d" r)
            | None -> ()
            if w.Reload > 0<Actions> then yield one (sprintf "%s Reload" w.Name, sprintf "%3d" w.Reload) else ()
            yield many (sprintf "%s traits" w.Name, w.Traits |> List.map (fun t -> sprintf "%A" t))
        | None -> ()
    ]

    let printClassDC c = [
        match Map.tryPick (fun (sk: Skill) _ -> if sk.Name.Contains ("Class") then Some sk else None) c.Skills with
        | Some sk -> yield printDC c sk
        | None -> ()
    ]

    let printSpells (c, sk) = [
        yield printSkill c sk
        yield printDC c sk
        let spells = c.Spells |> Map.filter (fun _ v -> v > 0) |> Map.toSeq
        for (level, count) in spells do
            let lvstr =
                match level with
                | 0<Level> -> "Cantrip"
                | 1<Level> -> "1st"
                | 2<Level> -> "2nd"
                | 3<Level> -> "3rd"
                | l -> sprintf "%dth" l
            yield (lvstr, sprintf "%d" count)
    ]

    let buildBasics c = {
        Title = None
        Items = [
            yield one ("Name", c.Name)
            yield printTitle c
            match c.Background with | Some bg -> yield one ("Background", bg) | None -> ()
            yield one ("Hit Points", sprintf "%d" (Derive.hitPoints c))
            match c.Size with | Some sz -> yield one ("Size", sz) | None -> ()
            yield one ("Speed", Derive.speed c)
        ]
    }

    let buildAbilities c = {
        Title = Some "Abilities"
        Items = [
            for ab in Builder.AbilityOrder do
                let score = Map.find ab c.Abilities
                yield one (ab.ToString (), sprintf "%4d (%+2d)" score (Derive.modifier score))
        ]
    }

    let buildArmor c = {
        Title = Some "Armor"
        Items = printArmorClass c
    }

    let buildMeleeWeapon c = {
        Title = Some "Melee weapon"
        Items = printWeaponStats (c.MeleeWeapon, c)
    }

    let buildRangedWeapon c = {
        Title = Some "Ranged weapon"
        Items = printWeaponStats (c.RangedWeapon, c)
    }

    let buildDifficultyClasses c = {
        Title = Some "Difficulty classes"
        Items = printClassDC c
    }

    let buildSaves c = {
        Title = Some "Saves"
        Items = Skills.saves |> List.map (printSkill c)
    }

    let buildSkills c = {
        Title = Some "Skills"
        Items = [
            yield printSkill c Skills.perception
            yield! Skills.regularSkillsForCharacter c |> List.map (printSkill c)
        ]
    }

    let buildSpells c = [
        match c.SpellSkill with
        | Some sk ->
            yield {
                Title = Some sk.Name
                Items = printSpells (c, sk)
            }
        | None -> ()
    ]

    let buildPools c = [
        let entries =
            c.Pools
            |> Map.filter (fun _ v -> v > 0)
            |> Map.toList
            |> List.map (fun (p, v) -> (p, sprintf "%d" v))
        if not (List.isEmpty entries)
        then
            yield {
                Title = Some "Pools"
                Items = entries
            }
    ]

    let buildFeats c = {
        Title = Some "Feats"
        Items = c.Feats |> List.sortBy (fun f -> f.Name) |> List.map (fun f ->
                one (f.Name, sprintf "page %3d" f.Page))
    }

    let create (c: Character) = [
        yield buildBasics c
        yield buildAbilities c
        yield buildArmor c
        yield buildMeleeWeapon c
        yield buildRangedWeapon c
        yield buildDifficultyClasses c
        yield buildSaves c
        yield buildSkills c
        yield! buildSpells c
        yield! buildPools c
        yield buildFeats c
    ]
