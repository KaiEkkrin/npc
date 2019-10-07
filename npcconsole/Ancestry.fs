namespace NpcConsole

open NpcConsole.Attributes

module Ancestry =
    let ancestry = "Ancestry"
    let dwarf = "Dwarf"
    let elf = "Elf"

    // How to require a particular character level (and no more)
    let level n c = c.Level >= (n * 1<Level>)

    // How to require a feat with a particular name
    let hasFeat n c =
        match List.tryFind (fun (f: Feat) -> f.Name = n) c.Feats with
        | Some _ -> true
        | None -> false

    // How to define an ancestry feat
    let ancestryFeat prereq name ancestry = {
        Name = name
        Category = ancestry
        MeetsPrerequisites = (fun c ->
            match c.Ancestry with
            | Some a when a.Name = ancestry -> (prereq c)
            | _ -> false
        )
        Improvements = []
    }

    let dwarfAncestryFeats = [
        ancestryFeat (level 1) "Dwarven Lore" dwarf |> Feats.improve [
            Improve.skillOr Skills.crafting Skills.regularSkills ProficiencyRank.Trained
            Improve.skillOr Skills.religion Skills.regularSkills ProficiencyRank.Trained
            Improve.skills [Skills.lore "Dwarven"] ProficiencyRank.Trained
        ] // TODO encode crafting or religion choice
        ancestryFeat (level 1) "Dwarven Weapon Familiarity" dwarf // TODO encode weapon proficiencies
        ancestryFeat (level 1) "Rock Runner" dwarf
        ancestryFeat (level 1) "Stonecunning" dwarf
        ancestryFeat (level 1) "Unburdened Iron" dwarf
        ancestryFeat (level 1) "Vengeful Hatred" dwarf
        ancestryFeat (fun c -> (level 5 c) && (hasFeat "Rock Runner" c)) "Boulder Roll" dwarf
        ancestryFeat (fun c -> (level 5 c) && (hasFeat "Dwarven Weapon Familiarity" c)) "Dwarven Weapon Cunning" dwarf
        ancestryFeat (level 9) "Mountain's Stoutness" dwarf
        ancestryFeat (level 9) "Stonewalker" dwarf
        ancestryFeat (fun c -> (level 13 c) && (hasFeat "Dwarven Weapon Familiarity" c)) "Dwarven Weapon Expertise" dwarf
    ]

    let elfAncestryFeats = [
        ancestryFeat (level 1) "Ancestral Longevity" elf
        ancestryFeat (level 1) "Elven Lore" elf
        ancestryFeat (level 1) "Elven Weapon Familiarity" elf
        ancestryFeat (level 1) "Forlorn" elf
        ancestryFeat (level 1) "Nimble Elf" elf
        ancestryFeat (level 1) "Otherworldly Magic" elf
        ancestryFeat (level 1) "Unwavering Mien" elf
        ancestryFeat (level 5) "Ageless Patience" elf
        ancestryFeat (fun c -> (level 5 c) && (hasFeat "Elven Weapon Familiarity" c)) "Elven Weapon Elegance" elf
        ancestryFeat (level 9) "Elf Step" elf
        ancestryFeat (fun c -> (level 9 c) && (hasFeat "Ancestral Longevity" c)) "Expert Longevity" elf
        ancestryFeat (fun c -> (level 13 c) && (hasFeat "Expert Longevity" c)) "Universal Longevity" elf
        ancestryFeat (fun c -> (level 13 c) && (hasFeat "Elven Weapon Familiarity" c)) "Elven Weapon Expertise" elf
    ]

    let ancestries = [
        {
            Ancestry.Name = dwarf
            Improvements = [
                Improve.hitPoints 10
                Improve.size Medium
                Improve.speed 20<Feet>
                Improve.singleAbility Constitution
                Improve.singleAbility Wisdom
                Improve.anyAbility 1
                Improve.abilityFlaw Charisma
                Feats.darkvision
                Improve.feats dwarfAncestryFeats 1 ancestry
            ]
        }
        {
            Ancestry.Name = elf
            Improvements = [
                Improve.hitPoints 6
                Improve.size Medium
                Improve.speed 30<Feet>
                Improve.singleAbility Dexterity
                Improve.singleAbility Wisdom
                Improve.anyAbility 1
                Improve.abilityFlaw Constitution
                Feats.lowLightVision
                Improve.feats elfAncestryFeats 1 ancestry
            ]
        }
    ]
