namespace NpcConsole

open NpcConsole.Attributes

module Ancestry =
    let ancestry = "Ancestry"
    let dwarf = "Dwarf"
    let elf = "Elf"

    let hasNone c = match c.Ancestry with | Some _ -> false | None -> true

    // How to define an ancestry feat
    let ancestryFeat = Feats.feat AncestryFeat

    let dwarfAncestryFeats = [
        ancestryFeat (Improve.hasLevel 1) "Dwarven Lore" [
            Improve.skillOr Skills.crafting Skills.regularSkills ProficiencyRank.Trained
            Improve.skillOr Skills.religion Skills.regularSkills ProficiencyRank.Trained
            Improve.skills [Skills.lore "Dwarven"] ProficiencyRank.Trained
        ] // TODO encode crafting or religion choice
        ancestryFeat (Improve.hasLevel 1) "Dwarven Weapon Familiarity" [] // TODO encode weapon proficiencies
        ancestryFeat (Improve.hasLevel 1) "Rock Runner" []
        ancestryFeat (Improve.hasLevel 1) "Stonecunning" []
        ancestryFeat (Improve.hasLevel 1) "Unburdened Iron" []
        ancestryFeat (Improve.hasLevel 1) "Vengeful Hatred" []
        ancestryFeat (Feats.req 5 ["Rock Runner"]) "Boulder Roll" []
        ancestryFeat (Feats.req 5 ["Dwarven Weapon Familiarity"]) "Dwarven Weapon Cunning" []
        ancestryFeat (Improve.hasLevel 9) "Mountain's Stoutness" []
        ancestryFeat (Improve.hasLevel 9) "Stonewalker" []
        ancestryFeat (Feats.req 13 ["Dwarven Weapon Familiarity"]) "Dwarven Weapon Expertise" []
    ]

    let elfAncestryFeats = [
        ancestryFeat (Improve.hasLevel 1) "Ancestral Longevity" []
        ancestryFeat (Improve.hasLevel 1) "Elven Lore" []
        ancestryFeat (Improve.hasLevel 1) "Elven Weapon Familiarity" []
        ancestryFeat (Improve.hasLevel 1) "Forlorn" []
        ancestryFeat (Improve.hasLevel 1) "Nimble Elf" []
        ancestryFeat (Improve.hasLevel 1) "Otherworldly Magic" []
        ancestryFeat (Improve.hasLevel 1) "Unwavering Mien" []
        ancestryFeat (Improve.hasLevel 5) "Ageless Patience" []
        ancestryFeat (Feats.req 5 ["Elven Weapon Familiarity"]) "Elven Weapon Elegance" []
        ancestryFeat (Improve.hasLevel 9) "Elf Step" []
        ancestryFeat (Feats.req 9 ["Ancestral Longevity"]) "Expert Longevity" []
        ancestryFeat (Feats.req 13 ["Expert Longevity"]) "Universal Longevity" []
        ancestryFeat (Feats.req 13 ["Elven Weapon Familiarity"]) "Elven Weapon Expertise" []
    ]

    // TODO add heritage, and all manner of other things :)
    let ancestries = {
        Prompt = "Ancestry"
        Choices = [
            "Dwarf", hasNone, (fun c -> { c with Ancestry = Some "Dwarf" }, [
                Improve.hitPoints 10
                Improve.size Medium
                Improve.speed 20<Feet>
                Improve.singleAbility Constitution
                Improve.singleAbility Wisdom
                Improve.anyAbility 1
                Improve.abilityFlaw Charisma
                Feats.darkvision
                Improve.addFeats dwarfAncestryFeats 1
            ])
            "Elf", hasNone, (fun c -> {c with Ancestry = Some "Elf" }, [
                Improve.hitPoints 6
                Improve.size Medium
                Improve.speed 30<Feet>
                Improve.singleAbility Dexterity
                Improve.singleAbility Wisdom
                Improve.anyAbility 1
                Improve.abilityFlaw Constitution
                Feats.lowLightVision
                Improve.addFeats elfAncestryFeats 1
            ])
        ]
        Count = 1
    }
