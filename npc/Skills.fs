namespace Npc

open Npc.Attributes

module Skills =
    // Perception
    let perception = { Name = "Perception"; KeyAbility = Wisdom }

    // Saving throws
    let fortitudeSave = { Name = "Fortitude"; KeyAbility = Constitution }
    let reflexSave = { Name = "Reflex"; KeyAbility = Dexterity }
    let willSave = { Name = "Will"; KeyAbility = Wisdom }

    let saves = [ fortitudeSave; reflexSave; willSave ]

    // Things listed as "proper skills"
    let skill name ka = { Name = name; KeyAbility = ka }
    let acrobatics = skill "Acrobatics" Dexterity
    let arcana = skill "Arcana" Intelligence
    let athletics = skill "Athletics" Strength
    let crafting = skill "Crafting" Intelligence
    let deception = skill "Deception" Charisma
    let diplomacy = skill "Diplomacy" Charisma
    let intimidation = skill "Intimidation" Charisma
    let lore v = skill (sprintf "Lore (%s)" v) Intelligence
    let medicine = skill "Medicine" Wisdom
    let nature = skill "Nature" Wisdom
    let occultism = skill "Occultism" Intelligence
    let performance = skill "Performance" Charisma
    let religion = skill "Religion" Wisdom
    let society = skill "Society" Intelligence
    let stealth = skill "Stealth" Dexterity
    let survival = skill "Survival" Wisdom
    let thievery = skill "Thievery" Dexterity

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
            |> Map.filter (fun sk _ -> sk.Name.StartsWith "Lore")
            |> Map.toSeq
            |> Seq.map fst
            |> Set.ofSeq

        regularSkills
        |> Set.ofList
        |> Set.union loreSkills
        |> Set.toList
        |> List.sortBy (fun sk -> sk.Name)

    // How to add one or more skills at a particular proficiency
    let add (skills: Skill list) prof count = {
        Prompt = sprintf "%A skill" prof
        Choices = skills |> List.map (fun sk -> AddSkill (sk, prof))
        Count = Some count
    }

    // How to increase one skill out of a list
    let increase (skills: Skill list) = {
        Prompt = "Skill increase"
        Choices = skills |> List.map IncreaseSkill
        Count = Some 1
    }