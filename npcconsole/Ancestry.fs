namespace NpcConsole

open NpcConsole.Attributes

module Ancestry =
    let ancestry = "Ancestry"
    let dwarf = "Dwarf"
    let elf = "Elf"
    let gnome = "Gnome"
    let goblin = "Goblin"
    let halfling = "Halfling"
    let human = "Human"
    let halfElf = "Half-elf"
    let halfOrc = "Half-orc"

    let hasNone c = Option.isNone c.Ancestry
    let hasHeritage h c = match c.Heritage with | Some h2 when h2 = h -> true | _ -> false

    // Helps us recategorise weapons in the usual pattern
    let recategorise (cat, tr) =
        Improve.recategorise (fun w -> w.Category = cat && List.contains tr w.Traits)

    let noHeritage = "None", (fun _ -> true), fun c -> c, []
    let heritage name imps = name, (fun c -> Option.isNone c.Heritage), fun c -> { c with Heritage = Some name }, imps
    let ancestryFeat ancestry level reqs name page (imps: Improvement list) =
        let allReqs = [
            yield Improve.hasAncestry ancestry
            yield! reqs
        ]
        Feats.feat level allReqs name page imps

    let dwarfHeritages = [
        heritage "Ancient-Blooded" []
        heritage "Death Warden" []
        heritage "Forge" []
        heritage "Rock" []
        heritage "Strong-Blooded" []
    ]

    let dwarfAncestryFeats = [
        ancestryFeat dwarf 1 [] "Dwarven Lore" 36 [
            Improve.skillOr Skills.crafting Skills.regularSkills Trained
            Improve.skillOr Skills.religion Skills.regularSkills Trained
            Improve.skill (Skills.lore "Dwarven") Trained
        ]
        ancestryFeat dwarf 1 [] "Dwarven Weapon Familiarity" 36 [
            Improve.skill (Skills.weaponSkill Weapons.battleAxe) Trained
            Improve.skill (Skills.weaponSkill Weapons.warhammer) Trained
            // TODO gain access to uncommon dwarf weapons (?)
            recategorise (MartialWeapon, Weapons.dwarf) SimpleWeapon
            recategorise (AdvancedWeapon, Weapons.dwarf) MartialWeapon
        ]
        ancestryFeat dwarf 1 [] "Rock Runner" 36 []
        ancestryFeat dwarf 1 [] "Stonecunning" 36 []
        ancestryFeat dwarf 1 [] "Unburdened Iron" 36 [] // TODO reduce speed reduction from armor
        ancestryFeat dwarf 1 [] "Vengeful Hatred" 36 []
        ancestryFeat dwarf 5 [Improve.hasFeat "Rock Runner"] "Boulder Roll" 36 []
        ancestryFeat dwarf 5 [Improve.hasFeat "Dwarven Weapon Familiarity"] "Dwarven Weapon Cunning" 37 []
        ancestryFeat dwarf 9 [] "Mountain's Stoutness" 37 [
            Improve.hitPointsPerLevel 1
        ]
        ancestryFeat dwarf 9 [] "Stonewalker" 37 []
        ancestryFeat dwarf 13 [Improve.hasFeat "Dwarven Weapon Familiarity"] "Dwarven Weapon Expertise" 37 [
            // TODO gain expert in dwarven weapons when gaining expert in another type
        ]
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
        ancestryFeat elf 1 [] "Ancestral Longevity" 40 []
        ancestryFeat elf 1 [] "Elven Lore" 40 [
            Improve.skillOr Skills.arcana Skills.regularSkills Trained
            Improve.skillOr Skills.arcana Skills.regularSkills Trained
            Improve.skill (Skills.lore "Elven") Trained
        ]
        ancestryFeat elf 1 [] "Elven Weapon Familiarity" 40 [
            Improve.skill (Skills.weaponSkill Weapons.compositeLongbow) Trained
            Improve.skill (Skills.weaponSkill Weapons.compositeShortbow) Trained
            Improve.skill (Skills.weaponSkill Weapons.longbow) Trained
            Improve.skill (Skills.weaponSkill Weapons.longsword) Trained
            Improve.skill (Skills.weaponSkill Weapons.rapier) Trained
            Improve.skill (Skills.weaponSkill Weapons.shortbow) Trained
            // TODO gain access to uncommon elf weapons (?)
            recategorise (MartialWeapon, Weapons.elf) SimpleWeapon
            recategorise (AdvancedWeapon, Weapons.elf) MartialWeapon
        ]
        ancestryFeat elf 1 [] "Forlorn" 40 []
        ancestryFeat elf 1 [] "Nimble Elf" 40 [
            Improve.speed 5<Feet>
        ]
        ancestryFeat elf 1 [] "Otherworldly Magic" 40 []
        ancestryFeat elf 1 [] "Unwavering Mien" 40 []
        ancestryFeat elf 5 [] "Ageless Patience" 40 []
        ancestryFeat elf 5 [Improve.hasFeat "Elven Weapon Familiarity"] "Elven Weapon Elegance" 41 []
        ancestryFeat elf 9 [] "Elf Step" 41 []
        ancestryFeat elf 9 [Improve.hasFeat "Ancestral Longevity"] "Expert Longevity" 41 []
        ancestryFeat elf 13 [Improve.hasFeat "Expert Longevity"] "Universal Longevity" 41 []
        ancestryFeat elf 13 [Improve.hasFeat "Elven Weapon Familiarity"] "Elven Weapon Expertise" 41 [] // TODO fix this
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
        ancestryFeat gnome 1 [] "Animal Accomplice" 44 []
        ancestryFeat gnome 1 [] "Burrow Elocutionist" 44 []
        ancestryFeat gnome 1 [] "Fey Fellowship" 44 []
        ancestryFeat gnome 1 [] "First World Magic" 44 []
        ancestryFeat gnome 1 [] "Gnome Obsession" 44 [] // TODO encode choosing a lore skill
        ancestryFeat gnome 1 [] "Gnome Weapon Familiarity" 44 [
            Improve.skill (Skills.weaponSkill Weapons.glaive) Trained
            Improve.skill (Skills.weaponSkill Weapons.kukri) Trained
            // TODO access to uncommon weapons
            recategorise (MartialWeapon, Weapons.gnome) SimpleWeapon
            recategorise (AdvancedWeapon, Weapons.gnome) MartialWeapon
        ]
        ancestryFeat gnome 1 [] "Illusion Sense" 44 []
        ancestryFeat gnome 5 [Improve.hasFeat "Burrow Elocutionist"] "Animal Elocutionist" 45 []
        ancestryFeat gnome 5 [] "Energized Font" 45 [] // TODO encode one of several feats
        ancestryFeat gnome 5 [Improve.hasFeat "Gnome Weapon Familiarity"] "Gnome Weapon Innovator" 45 []
        ancestryFeat gnome 9 [] "First World Adept" 45 [] // TODO funny requirement again
        ancestryFeat gnome 9 [] "Vivacious Conduit" 45 []
        ancestryFeat gnome 13 [Improve.hasFeat "Gnome Weapon Familiarity"] "Gnome Weapon Expertise" 45 [] // TODO this
    ]

    let goblinHeritages = [
        heritage "Charhide" []
        heritage "Irongut" []
        heritage "Razortooth" []
        heritage "Snow" [] // TODO resistances
        heritage "Unbreakable" [
            Improve.hitPointsFlat 4
        ]
    ]

    let goblinAncestryFeats = [
        ancestryFeat goblin 1 [] "Burn It!" 48 []
        ancestryFeat goblin 1 [] "City Scavenger" 48 []
        ancestryFeat goblin 1 [] "Goblin Lore" 48 [
            Improve.skillOr Skills.nature Skills.regularSkills Trained
            Improve.skillOr Skills.stealth Skills.regularSkills Trained
            Improve.skill (Skills.lore "Goblin") Trained
        ]
        ancestryFeat goblin 1 [] "Goblin Scuttle" 48 []
        ancestryFeat goblin 1 [] "Goblin Song" 48 []
        ancestryFeat goblin 1 [] "Goblin Weapon Familiarity" 48 [
            Improve.skill (Skills.weaponSkill Weapons.dogslicer) Trained
            Improve.skill (Skills.weaponSkill Weapons.horsechopper) Trained
            // TODO uncommon
            recategorise (MartialWeapon, Weapons.goblin) SimpleWeapon
            recategorise (AdvancedWeapon, Weapons.goblin) MartialWeapon
        ]
        ancestryFeat goblin 1 [] "Junk Tinker" 48 []
        ancestryFeat goblin 1 [] "Rough Rider" 48 [
            Feats.forceAdd Feats.ride
        ]
        ancestryFeat goblin 1 [] "Very Sneaky" 49 []
        ancestryFeat goblin 5 [Improve.hasFeat "Goblin Weapon Familiarity"] "Goblin Weapon Frenzy" 49 []
        ancestryFeat goblin 9 [] "Cave Climber" 49 []
        ancestryFeat goblin 9 [Improve.hasFeat "Goblin Scuttle"] "Skittering Scuttle" 49 []
        ancestryFeat goblin 13 [Improve.hasFeat "Goblin Weapon Familiarity"] "Goblin Weapon Expertise" 49 [] // TODO this
        ancestryFeat goblin 13 [Improve.hasFeat "Very Sneaky"] "Very, Very Sneaky" 49 []
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
        ancestryFeat halfling 1 [] "Distracting Shadows" 52 []
        ancestryFeat halfling 1 [] "Halfling Lore" 52 [
            Improve.skillOr Skills.acrobatics Skills.regularSkills Trained
            Improve.skillOr Skills.stealth Skills.regularSkills Trained
            Improve.skill (Skills.lore "Halfling") Trained
        ]
        ancestryFeat halfling 1 [] "Halfling Luck" 52 []
        ancestryFeat halfling 1 [] "Halfling Weapon Familiarity" 52 [
            Improve.skill (Skills.weaponSkill Weapons.halflingSlingStaff) Trained
            Improve.skill (Skills.weaponSkill Weapons.shortsword) Trained
            Improve.skill (Skills.weaponSkill Weapons.sling) Trained
            // TODO uncommon weapons
            recategorise (MartialWeapon, Weapons.halfling) SimpleWeapon
            recategorise (AdvancedWeapon, Weapons.halfling) MartialWeapon
        ]
        ancestryFeat halfling 1 [] "Sure Feet" 52 []
        ancestryFeat halfling 1 [] "Titan Slinger" 52 []
        ancestryFeat halfling 1 [] "Unfettered Halfling" 52 []
        ancestryFeat halfling 1 [] "Watchful Halfling" 52 []
        ancestryFeat halfling 5 [] "Cultural Adaptability" 53 []
        ancestryFeat halfling 5 [Improve.hasFeat "Halfling Weapon Familiarity"] "Halfling Weapon Trickster" 53 []
        ancestryFeat halfling 9 [Improve.hasFeat "Halfling Luck"] "Guiding Luck" 53 []
        ancestryFeat halfling 9 [] "Irrepressible" 53 []
        ancestryFeat halfling 13 [Improve.hasFeat "Distracting Shadows"] "Ceaseless Shadows" 53 []
        ancestryFeat halfling 13 [Improve.hasFeat "Halfling Weapon Familiarity"] "Halfling Weapon Expertise" 53 [] // TODO this
    ]

    let humanHeritages = [
        noHeritage
        heritage halfElf []
        heritage halfOrc []
    ]

    let humanAncestryFeats = [
        ancestryFeat human 1 [] "Adapted Cantrip" 57 [] // TODO require spellcasting
        ancestryFeat human 1 [] "Cooperative Nature" 57 []
        ancestryFeat human 1 [] "General Training" 57 [
            Improve.addFeats Feats.generalFeats 1 // TODO Defer choice; select multiple times
        ]
        ancestryFeat human 1 [] "Haughty Obstinacy" 57 []
        ancestryFeat human 1 [] "Natural Ambition" 57 [] // TODO deferred choice of class feat
        ancestryFeat human 1 [] "Natural Skill" 57 [
            Improve.skills Skills.regularSkills Trained 2
        ]
        ancestryFeat human 1 [] "Unconventional Weaponry" 57 [] // TODO weapon skills
        ancestryFeat human 5 [Improve.hasFeat "Adapted Cantrip"] "Adaptive Adept" 57 [] // TODO require 3rd level spells
        ancestryFeat human 5 [] "Clever Improviser" 57 []
        ancestryFeat human 9 [Improve.hasFeat "Cooperative Nature"] "Cooperative Soul" 57 []
        ancestryFeat human 9 [Improve.hasFeat "Clever Improviser"] "Incredible Improvisation" 57 []
        ancestryFeat human 9 [] "Multitalented" 58 [] // TODO multiclass feats
        ancestryFeat human 13 [Improve.hasFeat "Unconventional Weaponry"] "Unconventional Expertise" 58 []

        // Half-elf ancestry feats
        ancestryFeat human 1 [hasHeritage halfElf; fun c -> c.Level = 1<Level>] "Elf Atavism" 58 []
        ancestryFeat human 5 [hasHeritage halfElf; Improve.hasLevel 5] "Inspire Imitation" 58 []
        ancestryFeat human 5 [hasHeritage halfElf; Improve.hasLevel 5] "Supernatural Charm" 58 []

        // Half-orc ancestry feats
        ancestryFeat human 1 [hasHeritage halfOrc] "Monstrous Peacemaker" 59 []
        ancestryFeat human 1 [hasHeritage "Half-orc"] "Orc Ferocity" 59 []
        ancestryFeat human 1 [hasHeritage "Half-orc"; Improve.hasFeat "Low-light vision"] "Orc Sight" 59 []
        ancestryFeat human 1 [hasHeritage "Half-orc"] "Orc Superstition" 59 []
        ancestryFeat human 1 [hasHeritage "Half-orc"] "Orc Weapon Familiarity" 59 [] // TODO weapon proficiencies
        ancestryFeat human 5 [hasHeritage "Half-orc"; Improve.hasFeat "Orc Weapon Proficiency"] "Orc Weapon Carnage" 59 []
        ancestryFeat human 5 [hasHeritage "Half-orc"] "Victorious Vigor" 59 []
        ancestryFeat human 9 [hasHeritage "Half-orc"; Improve.hasFeat "Orc Superstition"] "Pervasive Superstition" 59 []
        ancestryFeat human 13 [hasHeritage "Half-orc"; Improve.hasFeat "Orc Ferocity"] "Incredible Ferocity" 59 []
        ancestryFeat human 13 [hasHeritage "Half-orc"; Improve.hasFeat "Orc Weapon Familiarity"] "Orc Weapon Expertise" 59 []
    ]

    // TODO add heritage, and all manner of other things :)
    let ancestries = {
        Prompt = "Ancestry"
        Choices = [
            dwarf, hasNone, (fun c -> { c with Ancestry = Some dwarf }, [
                Improve.hitPointsFlat 10
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
            elf, hasNone, (fun c -> { c with Ancestry = Some elf }, [
                Improve.hitPointsFlat 6
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
            gnome, hasNone, (fun c -> { c with Ancestry = Some gnome }, [
                Improve.hitPointsFlat 8
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
            goblin, hasNone, (fun c -> { c with Ancestry = Some goblin }, [
                Improve.hitPointsFlat 6
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
            halfling, hasNone, (fun c -> { c with Ancestry = Some halfling }, [
                Improve.hitPointsFlat 6
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
            human, hasNone, (fun c -> { c with Ancestry = Some human }, [
                Improve.hitPointsFlat 8
                Improve.size Medium
                Improve.speed 25<Feet>
                Improve.anyAbility 2
                Improve.heritage humanHeritages
                Improve.addFeats humanAncestryFeats 1
            ])
        ]
        Count = 1
    }

    // The ancestry feats all have their matching ancestries as prerequisites, so this whole list
    // can be applied to characters leveling up and gaining extra ones
    let ancestryFeats = List.concat [dwarfAncestryFeats; elfAncestryFeats; gnomeAncestryFeats; goblinAncestryFeats; halflingAncestryFeats; humanAncestryFeats]
