namespace NpcConsole

open NpcConsole.Attributes

module Ancestry =
    let ancestry = "Ancestry"
    let dwarf = "Dwarf"
    let elf = "Elf"

    let hasNone c = Option.isNone c.Ancestry
    let hasHeritage h c = match c.Heritage with | Some h2 when h2 = h -> true | _ -> false

    let noHeritage = "None", (fun _ -> true), fun c -> c, []
    let heritage name imps = name, (fun c -> Option.isNone c.Heritage), fun c -> { c with Heritage = Some name }, imps
    let ancestryFeat = Feats.feat AncestryFeat

    let dwarfHeritages = [
        heritage "Ancient-Blooded" []
        heritage "Death Warden" []
        heritage "Forge" []
        heritage "Rock" []
        heritage "Strong-Blooded" []
    ]

    let dwarfAncestryFeats = [
        ancestryFeat (Improve.hasLevel 1) "Dwarven Lore" [
            Improve.skillOr Skills.crafting Skills.regularSkills Trained
            Improve.skillOr Skills.religion Skills.regularSkills Trained
            Improve.skill (Skills.lore "Dwarven") Trained
        ]
        ancestryFeat (Improve.hasLevel 1) "Dwarven Weapon Familiarity" [] // TODO encode weapon proficiencies
        ancestryFeat (Improve.hasLevel 1) "Rock Runner" []
        ancestryFeat (Improve.hasLevel 1) "Stonecunning" []
        ancestryFeat (Improve.hasLevel 1) "Unburdened Iron" [] // TODO reduce speed reduction from armor
        ancestryFeat (Improve.hasLevel 1) "Vengeful Hatred" []
        ancestryFeat (Feats.req 5 ["Rock Runner"]) "Boulder Roll" []
        ancestryFeat (Feats.req 5 ["Dwarven Weapon Familiarity"]) "Dwarven Weapon Cunning" []
        ancestryFeat (Improve.hasLevel 9) "Mountain's Stoutness" []
        ancestryFeat (Improve.hasLevel 9) "Stonewalker" []
        ancestryFeat (Feats.req 13 ["Dwarven Weapon Familiarity"]) "Dwarven Weapon Expertise" []
    ]

    let elfHeritages = [
        heritage "Arctic" [] // TODO include resistances
        heritage "Cavern" [
            Feats.forceAdd Feats.darkvision
        ]
        heritage "Seer" []
        heritage "Whisper" []
        heritage "Woodland" []
    ]

    let elfAncestryFeats = [
        ancestryFeat (Improve.hasLevel 1) "Ancestral Longevity" []
        ancestryFeat (Improve.hasLevel 1) "Elven Lore" [
            Improve.skillOr Skills.arcana Skills.regularSkills Trained
            Improve.skillOr Skills.arcana Skills.regularSkills Trained
            Improve.skill (Skills.lore "Elven") Trained
        ]
        ancestryFeat (Improve.hasLevel 1) "Elven Weapon Familiarity" [] // TODO encode weapon proficiencies
        ancestryFeat (Improve.hasLevel 1) "Forlorn" []
        ancestryFeat (Improve.hasLevel 1) "Nimble Elf" [
            Improve.speed 5<Feet>
        ]
        ancestryFeat (Improve.hasLevel 1) "Otherworldly Magic" []
        ancestryFeat (Improve.hasLevel 1) "Unwavering Mien" []
        ancestryFeat (Improve.hasLevel 5) "Ageless Patience" []
        ancestryFeat (Feats.req 5 ["Elven Weapon Familiarity"]) "Elven Weapon Elegance" []
        ancestryFeat (Improve.hasLevel 9) "Elf Step" []
        ancestryFeat (Feats.req 9 ["Ancestral Longevity"]) "Expert Longevity" []
        ancestryFeat (Feats.req 13 ["Expert Longevity"]) "Universal Longevity" []
        ancestryFeat (Feats.req 13 ["Elven Weapon Familiarity"]) "Elven Weapon Expertise" []
    ]

    let gnomeHeritages = [
        heritage "Chameleon" []
        heritage "Fey-Touched" []
        heritage "Sensate" []
        heritage "Umbral" [
            Feats.forceAdd Feats.darkvision
        ]
        heritage "Wellspring" []
    ]

    let gnomeAncestryFeats = [
        ancestryFeat (Improve.hasLevel 1) "Animal Accomplice" []
        ancestryFeat (Improve.hasLevel 1) "Burrow Elocutionist" []
        ancestryFeat (Improve.hasLevel 1) "Fey Fellowship" []
        ancestryFeat (Improve.hasLevel 1) "First World Magic" []
        ancestryFeat (Improve.hasLevel 1) "Gnome Obsession" [] // TODO encode choosing a lore skill
        ancestryFeat (Improve.hasLevel 1) "Gnome Weapon Familiarity" [] // TODO encode weapon proficiencies
        ancestryFeat (Improve.hasLevel 1) "Illusion Sense" []
        ancestryFeat (Feats.req 5 ["Burrow Elocutionist"]) "Animal Elocutionist" []
        ancestryFeat (Improve.hasLevel 5) "Energized Font" [] // TODO encode one of several feats
        ancestryFeat (Feats.req 5 ["Gnome Weapon Familiarity"]) "Gnome Weapon Innovator" []
        ancestryFeat (Improve.hasLevel 9) "First World Adept" [] // TODO funny requirement again
        ancestryFeat (Improve.hasLevel 9) "Vivacious Conduit" []
        ancestryFeat (Feats.req 13 ["Gnome Weapon Familiarity"]) "Gnome Weapon Expertise" []
    ]

    let goblinHeritages = [
        heritage "Charhide" []
        heritage "Irongut" []
        heritage "Razortooth" []
        heritage "Snow" [] // TODO resistances
        heritage "Unbreakable" [
            Improve.hitPoints 4
        ]
    ]

    let goblinAncestryFeats = [
        ancestryFeat (Improve.hasLevel 1) "Burn It!" []
        ancestryFeat (Improve.hasLevel 1) "City Scavenger" []
        ancestryFeat (Improve.hasLevel 1) "Goblin Lore" [
            Improve.skillOr Skills.nature Skills.regularSkills Trained
            Improve.skillOr Skills.stealth Skills.regularSkills Trained
            Improve.skill (Skills.lore "Goblin") Trained
        ]
        ancestryFeat (Improve.hasLevel 1) "Goblin Scuttle" []
        ancestryFeat (Improve.hasLevel 1) "Goblin Song" []
        ancestryFeat (Improve.hasLevel 1) "Goblin Weapon Familiarity" [] // TODO weapon skills
        ancestryFeat (Improve.hasLevel 1) "Junk Tinker" []
        ancestryFeat (Improve.hasLevel 1) "Rough Rider" [
            Feats.forceAdd Feats.ride
        ]
        ancestryFeat (Improve.hasLevel 1) "Very Sneaky" []
        ancestryFeat (Feats.req 5 ["Goblin Weapon Familiarity"]) "Goblin Weapon Frenzy" []
        ancestryFeat (Improve.hasLevel 9) "Cave Climber" []
        ancestryFeat (Feats.req 9 ["Goblin Scuttle"]) "Skittering Scuttle" []
        ancestryFeat (Feats.req 13 ["Goblin Weapon Familiarity"]) "Goblin Weapon Expertise" []
        ancestryFeat (Feats.req 13 ["Very Sneaky"]) "Very, Very Sneaky" []
    ]

    let halflingHeritages = [
        heritage "Gutsy" []
        heritage "Hillock" []
        heritage "Nomadic" [] // TODO languages
        heritage "Twilight" [
            Feats.forceAdd Feats.lowLightVision
        ]
        heritage "Wildwood" []
    ]

    let halflingAncestryFeats = [
        ancestryFeat (Improve.hasLevel 1) "Distracting Shadows" []
        ancestryFeat (Improve.hasLevel 1) "Halfling Lore" [
            Improve.skillOr Skills.acrobatics Skills.regularSkills Trained
            Improve.skillOr Skills.stealth Skills.regularSkills Trained
            Improve.skill (Skills.lore "Halfling") Trained
        ]
        ancestryFeat (Improve.hasLevel 1) "Halfling Luck" []
        ancestryFeat (Improve.hasLevel 1) "Halfling Weapon Familiarity" [] // TODO weapon skills
        ancestryFeat (Improve.hasLevel 1) "Sure Feet" []
        ancestryFeat (Improve.hasLevel 1) "Titan Slinger" []
        ancestryFeat (Improve.hasLevel 1) "Unfettered Halfling" []
        ancestryFeat (Improve.hasLevel 1) "Watchful Halfling" []
        ancestryFeat (Improve.hasLevel 5) "Cultural Adaptability" []
        ancestryFeat (Feats.req 5 ["Halfling Weapon Familiarity"]) "Halfling Weapon Trickster" []
        ancestryFeat (Feats.req 9 ["Halfling Luck"]) "Guiding Luck" []
        ancestryFeat (Improve.hasLevel 9) "Irrepressible" []
        ancestryFeat (Feats.req 13 ["Distracting Shadows"]) "Ceaseless Shadows" []
        ancestryFeat (Feats.req 13 ["Halfling Weapon Familiarity"]) "Halfling Weapon Expertise" []
    ]

    let humanHeritages = [
        noHeritage
        heritage "Half-elf" []
        heritage "Half-orc" []
    ]

    let humanAncestryFeats = [
        ancestryFeat (Improve.hasLevel 1) "Adapted Cantrip" [] // TODO require spellcasting
        ancestryFeat (Improve.hasLevel 1) "Cooperative Nature" []
        ancestryFeat (Improve.hasLevel 1) "General Training" [
            Improve.addFeats Feats.generalFeats 1 // TODO Defer choice; select multiple times
        ]
        ancestryFeat (Improve.hasLevel 1) "Haughty Obstinacy" []
        ancestryFeat (Improve.hasLevel 1) "Natural Ambition" [] // TODO deferred choice of class feat
        ancestryFeat (Improve.hasLevel 1) "Natural Skill" [
            Improve.skills Skills.regularSkills Trained 2
        ]
        ancestryFeat (Improve.hasLevel 1) "Unconventional Weaponry" [] // TODO weapon skills
        ancestryFeat (Feats.req 5 ["Adapted Cantrip"]) "Adaptive Adept" [] // TODO require 3rd level spells
        ancestryFeat (Improve.hasLevel 5) "Clever Improviser" []
        ancestryFeat (Feats.req 9 ["Cooperative Nature"]) "Cooperative Soul" []
        ancestryFeat (Feats.req 9 ["Clever Improviser"]) "Incredible Improvisation" []
        ancestryFeat (Improve.hasLevel 9) "Multitalented" [] // TODO multiclass feats
        ancestryFeat (Feats.req 13 ["Unconventional Weaponry"]) "Unconventional Expertise" []

        // Half-elf ancestry feats
        ancestryFeat (Improve.require [hasHeritage "Half-elf"; fun c -> c.Level = 1<Level>]) "Elf Atavism" []
        ancestryFeat (Improve.require [hasHeritage "Half-elf"; Improve.hasLevel 5]) "Inspire Imitation" []
        ancestryFeat (Improve.require [hasHeritage "Half-elf"; Improve.hasLevel 5]) "Supernatural Charm" []

        // Half-orc ancestry feats
        ancestryFeat (hasHeritage "Half-orc") "Monstrous Peacemaker" []
        ancestryFeat (hasHeritage "Half-orc") "Orc Ferocity" []
        ancestryFeat (Improve.require [hasHeritage "Half-orc"; Feats.req 1 ["Low-light vision"]]) "Orc Sight" []
        ancestryFeat (hasHeritage "Half-orc") "Orc Superstition" []
        ancestryFeat (hasHeritage "Half-orc") "Orc Weapon Familiarity" [] // TODO weapon proficiencies
        ancestryFeat (Improve.require [hasHeritage "Half-orc"; Feats.req 5 ["Orc Weapon Proficiency"]]) "Orc Weapon Carnage" []
        ancestryFeat (Improve.require [hasHeritage "Half-orc"; Improve.hasLevel 5]) "Victorious Vigor" []
        ancestryFeat (Improve.require [hasHeritage "Half-orc"; Feats.req 9 ["Orc Superstition"]]) "Pervasive Superstition" []
        ancestryFeat (Improve.require [hasHeritage "Half-orc"; Feats.req 13 ["Orc Ferocity"]]) "Incredible Ferocity" []
        ancestryFeat (Improve.require [hasHeritage "Half-orc"; Feats.req 13 ["Orc Weapon Familiarity"]]) "Orc Weapon Expertise" []
    ]

    // TODO add heritage, and all manner of other things :)
    let ancestries = {
        Prompt = "Ancestry"
        Choices = [
            "Dwarf", hasNone, (fun c -> { c with Ancestry = Some "Dwarf" }, [
                Improve.hitPoints 10
                Improve.size Medium
                Improve.speed 20<Feet>
                Improve.abilityFlaw Charisma
                Improve.singleAbility Constitution
                Improve.singleAbility Wisdom
                Improve.ability [Strength; Dexterity; Intelligence; Charisma] 1
                Feats.forceAdd Feats.darkvision
                Improve.heritage dwarfHeritages
                Improve.addFeats dwarfAncestryFeats 1
            ])
            "Elf", hasNone, (fun c -> { c with Ancestry = Some "Elf" }, [
                Improve.hitPoints 6
                Improve.size Medium
                Improve.speed 30<Feet>
                Improve.abilityFlaw Constitution
                Improve.singleAbility Dexterity
                Improve.singleAbility Wisdom
                Improve.ability [Strength; Constitution; Intelligence; Charisma] 1
                Feats.forceAdd Feats.lowLightVision
                Improve.heritage elfHeritages
                Improve.addFeats elfAncestryFeats 1
            ])
            "Gnome", hasNone, (fun c -> { c with Ancestry = Some "Gnome" }, [
                Improve.hitPoints 8
                Improve.size Small
                Improve.speed 25<Feet>
                Improve.abilityFlaw Strength
                Improve.singleAbility Constitution
                Improve.singleAbility Charisma
                Improve.ability [Strength; Dexterity; Intelligence; Wisdom] 1
                Feats.forceAdd Feats.lowLightVision
                Improve.heritage gnomeHeritages
                Improve.addFeats gnomeAncestryFeats 1
            ])
            "Goblin", hasNone, (fun c -> { c with Ancestry = Some "Goblin" }, [
                Improve.hitPoints 6
                Improve.size Small
                Improve.speed 25<Feet>
                Improve.abilityFlaw Wisdom
                Improve.singleAbility Dexterity
                Improve.singleAbility Charisma
                Improve.ability [Strength; Constitution; Intelligence; Wisdom] 1
                Feats.forceAdd Feats.darkvision
                Improve.heritage goblinHeritages
                Improve.addFeats goblinAncestryFeats 1
            ])
            "Halfling", hasNone, (fun c -> { c with Ancestry = Some "Halfling" }, [
                Improve.hitPoints 6
                Improve.size Small
                Improve.speed 25<Feet>
                Improve.abilityFlaw Strength
                Improve.singleAbility Dexterity
                Improve.singleAbility Wisdom
                Improve.ability [Strength; Constitution; Intelligence; Charisma] 1
                Feats.forceAdd Feats.keenEyes
                Improve.heritage halflingHeritages
                Improve.addFeats halflingAncestryFeats 1
            ])
            "Human", hasNone, (fun c -> { c with Ancestry = Some "Human" }, [
                Improve.hitPoints 8
                Improve.size Medium
                Improve.speed 25<Feet>
                Improve.anyAbility 2
                Improve.heritage humanHeritages
                Improve.addFeats humanAncestryFeats 1
            ])
        ]
        Count = 1
    }
