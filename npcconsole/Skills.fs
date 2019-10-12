namespace NpcConsole

open NpcConsole.Attributes

module Skills =
    // Perception
    let perception = { Name = "Perception"; Type = Perception; KeyAbility = Wisdom }

    // Saving throws
    let fortitudeSave = { Name = "Fortitude"; Type = SavingThrow; KeyAbility = Constitution }
    let reflexSave = { Name = "Reflex"; Type = SavingThrow; KeyAbility = Dexterity }
    let willSave = { Name = "Will"; Type = SavingThrow; KeyAbility = Wisdom }

    // Things listed as "proper skills"
    let regularSkill name ka = { Name = name; Type = RegularSkill; KeyAbility = ka }
    let acrobatics = regularSkill "Acrobatics" Dexterity
    let arcana = regularSkill "Arcana" Intelligence
    let athletics = regularSkill "Athletics" Strength
    let crafting = regularSkill "Crafting" Intelligence
    let deception = regularSkill "Deception" Charisma
    let diplomacy = regularSkill "Diplomacy" Charisma
    let intimidation = regularSkill "Intimidation" Charisma
    let lore v = regularSkill (sprintf "Lore (%s)" v) Intelligence
    let medicine = regularSkill "Medicine" Wisdom
    let nature = regularSkill "Nature" Wisdom
    let occultism = regularSkill "Occultism" Intelligence
    let performance = regularSkill "Performance" Charisma
    let religion = regularSkill "Religion" Wisdom
    let society = regularSkill "Society" Intelligence
    let stealth = regularSkill "Stealth" Dexterity
    let survival = regularSkill "Survival" Wisdom
    let thievery = regularSkill "Thievery" Dexterity

    let regularSkills = [
        acrobatics
        arcana
        athletics
        crafting
        deception
        diplomacy
        intimidation
        medicine
        nature
        occultism
        performance
        religion
        society
        stealth
        survival
        thievery
    ]

    // Gets all a character's regular skills and their lore skills,
    // for display.  (We want to show all regular skills including
    // untrained, but only the specific Lore skills they have proficiency
    // in.)
    let regularSkillsForCharacter c =
        let loreSkills =
            c.Skills
            |> Map.filter (fun sk _ -> sk.Type = RegularSkill && sk.Name.StartsWith "Lore")
            |> Map.toSeq
            |> Seq.map fst
            |> Set.ofSeq

        regularSkills
        |> Set.ofList
        |> Set.union loreSkills
        |> Set.toList
        |> List.sortBy (fun sk -> sk.Name)

    let weaponSkill (category, ty) =
        let catName =
            match category with
            | Unarmed -> "Unarmed"
            | SimpleWeapon -> "Simple weapons"
            | MartialWeapon -> "Martial weapons"
            | AdvancedWeapon -> "Advanced weapons"
        {
            Name = sprintf "%s (%A)" catName ty
            Type = WeaponProficiency
            KeyAbility = match ty with | Melee -> Strength | Ranged -> Dexterity // TODO admit finesse weapons
        }

    let alchemicalBombs = {
        Name = "Alchemical Bombs"
        Type = WeaponProficiency
        KeyAbility = Dexterity
    }

    let armorSkill category = {
        Name = match category with | Unarmored -> "Unarmored" | c -> sprintf "%A armor" c
        Type = ArmorProficiency
        KeyAbility = Dexterity // TODO cap
    }

    let classSkill (cl, ab) = {
        Name = sprintf "%A Class" cl
        Type = ClassSkill
        KeyAbility = ab
    }
