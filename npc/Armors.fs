namespace Npc

open Npc.Attributes

module Armors =
    // -- ARMOR GROUPS --
    let leather = "Leather"
    let composite = "Composite"
    let chain = "Chain"
    let plate = "Plate"

    // -- ARMOR TRAITS --
    let bulwark = "Bulwark"
    let comfort = "Comfort"
    let flexible = "Flexible"
    let noisy = "Noisy"

    let armor (name, cat, bonus, dexCap, speedPenalty, strength, group, traits) = {
        Name = name
        Skill = Char2.armorSkill cat
        Bonus = bonus * 1<Modifier>
        DexCap = match dexCap with | Some dc -> Some (dc * 1<Modifier>) | None -> None
        SpeedPenalty = speedPenalty * 1<Feet>
        Strength = strength * 1<Score>
        Group = group
        Traits = traits
    }

    let unarmored = [
        armor ("No armor", Unarmored, 0, None, 0, 0, None, [])
        armor ("Explorer's clothing", Unarmored, 0, Some 5, 0, 0, None, [comfort])
    ]

    let lightArmors = [
        armor ("Padded armor", LightArmor, 1, Some 3, 0, 10, None, [comfort])
        armor ("Leather", LightArmor, 1, Some 4, 0, 10, None, [])
        armor ("Studded leather", LightArmor, 2, Some 3, 0, 12, None, [])
        armor ("Chain shirt", LightArmor, 2, Some 3, 0, 12, None, [flexible; noisy])
    ]

    let mediumArmors = [
        armor ("Hide", MediumArmor, 3, Some 2, 5, 14, Some leather, [])
        armor ("Scale mail", MediumArmor, 3, Some 2, 5, 14, Some composite, [])
        armor ("Chain mail", MediumArmor, 4, Some 1, 5, 16, Some chain, [flexible; noisy])
        armor ("Breastplate", MediumArmor, 4, Some 1, 5, 16, Some plate, [])
    ]

    let heavyArmors = [
        armor ("Splint mail", HeavyArmor, 5, Some 1, 10, 16, Some composite, [])
        armor ("Half plate", HeavyArmor, 5, Some 1, 10, 16, Some plate, [])
        armor ("Full plate", HeavyArmor, 6, Some 0, 10, 18, Some plate, [bulwark])
    ]

    let allArmors = List.concat [unarmored; lightArmors; mediumArmors; heavyArmors]
    let addArmor = {
        Prompt = "Armor"
        Choices = allArmors |> List.map AddArmor
        Count = Some 1
    }