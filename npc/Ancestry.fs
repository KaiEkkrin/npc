namespace Npc

open Npc.Attributes
open Npc.FeatReq

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

    let heritage h imps = AddHeritage (h, imps)

    let ancestryFeatWith ancestry level reqs name page imps =
        let allReqs = (AncestryReq ancestry) >&& reqs
        Feats.featWith level allReqs name page imps

    let ancestryFeat ancestry level reqs name page changes =
        let allReqs = (AncestryReq ancestry) >&& reqs
        Feats.feat level allReqs name page changes

    let dwarfHeritages = [
        heritage "Ancient-Blooded" []
        heritage "Death Warden" []
        heritage "Forge" []
        heritage "Rock" []
        heritage "Strong-Blooded" []
    ]

    let dwarfAncestryFeats = [
        ancestryFeat dwarf 1 NoReq "Dwarven Lore" 36 [
            AddSkillOr (Skills.crafting, Skills.regularSkills, Trained)
            AddSkillOr (Skills.religion, Skills.regularSkills, Trained)
            AddSkill ((Skills.lore "Dwarven"), Trained)
        ]
        ancestryFeat dwarf 1 NoReq "Dwarven Weapon Familiarity" 36 [
            AddSkill ((Char2.weaponSkill Weapons.battleAxe), Trained)
            AddSkill ((Char2.weaponSkill Weapons.warhammer), Trained)
            // TODO gain access to uncommon dwarf weapons (?)
            Recategorise (MartialWeapon, Weapons.dwarf, SimpleWeapon)
            Recategorise (AdvancedWeapon, Weapons.dwarf, MartialWeapon)
        ]
        ancestryFeat dwarf 1 NoReq "Rock Runner" 36 []
        ancestryFeat dwarf 1 NoReq "Stonecunning" 36 []
        ancestryFeat dwarf 1 NoReq "Unburdened Iron" 36 [] // TODO reduce speed reduction from armor
        ancestryFeat dwarf 1 NoReq "Vengeful Hatred" 36 []
        ancestryFeat dwarf 5 (FeatReq "Rock Runner") "Boulder Roll" 36 []
        ancestryFeat dwarf 5 (FeatReq "Dwarven Weapon Familiarity") "Dwarven Weapon Cunning" 37 []
        ancestryFeat dwarf 9 NoReq "Mountain's Stoutness" 37 [
            IncreaseHitPointsPerLevel 1
        ]
        ancestryFeat dwarf 9 NoReq "Stonewalker" 37 []
        ancestryFeat dwarf 13 (FeatReq "Dwarven Weapon Familiarity") "Dwarven Weapon Expertise" 37 [
            // TODO gain expert in dwarven weapons when gaining expert in another type
        ]
    ]

    let elfHeritages = [
        heritage "Arctic" [] // TODO include resistances
        AddHeritage ("Cavern", [
            Feats.forceAdd Feats.darkvision
        ])
        heritage "Seer" []
        heritage "Whisper" []
        heritage "Woodland" []
    ]

    let elfAncestryFeats = [
        ancestryFeat elf 1 NoReq "Ancestral Longevity" 40 []
        ancestryFeat elf 1 NoReq "Elven Lore" 40 [
            AddSkillOr (Skills.arcana, Skills.regularSkills, Trained)
            AddSkillOr (Skills.arcana, Skills.regularSkills, Trained)
            AddSkill ((Skills.lore "Elven"), Trained)
        ]
        ancestryFeat elf 1 NoReq "Elven Weapon Familiarity" 40 [
            AddSkill ((Char2.weaponSkill Weapons.compositeLongbow), Trained)
            AddSkill ((Char2.weaponSkill Weapons.compositeShortbow), Trained)
            AddSkill ((Char2.weaponSkill Weapons.longbow), Trained)
            AddSkill ((Char2.weaponSkill Weapons.longsword), Trained)
            AddSkill ((Char2.weaponSkill Weapons.rapier), Trained)
            AddSkill ((Char2.weaponSkill Weapons.shortbow), Trained)
            // TODO gain access to uncommon elf weapons (?)
            Recategorise (MartialWeapon, Weapons.elf, SimpleWeapon)
            Recategorise (AdvancedWeapon, Weapons.elf, MartialWeapon)
        ]
        ancestryFeat elf 1 NoReq "Forlorn" 40 []
        ancestryFeat elf 1 NoReq "Nimble Elf" 40 [
            IncreaseSpeed 5<Feet>
        ]
        ancestryFeat elf 1 NoReq "Otherworldly Magic" 40 []
        ancestryFeat elf 1 NoReq "Unwavering Mien" 40 []
        ancestryFeat elf 5 NoReq "Ageless Patience" 40 []
        ancestryFeat elf 5 (FeatReq "Elven Weapon Familiarity") "Elven Weapon Elegance" 41 []
        ancestryFeat elf 9 NoReq "Elf Step" 41 []
        ancestryFeat elf 9 (FeatReq "Ancestral Longevity") "Expert Longevity" 41 []
        ancestryFeat elf 13 (FeatReq "Expert Longevity") "Universal Longevity" 41 []
        ancestryFeat elf 13 (FeatReq "Elven Weapon Familiarity") "Elven Weapon Expertise" 41 [] // TODO fix this
    ]

    let gnomeHeritages = [
        AddHeritage ("Chameleon", [])
        AddHeritage ("Fey-Touched", [])
        AddHeritage ("Sensate", [])
        AddHeritage ("Umbral", [
            Feats.forceAdd Feats.darkvision
        ])
        AddHeritage ("Wellspring", [])
    ]

    let gnomeAncestryFeats = [
        ancestryFeat gnome 1 NoReq "Animal Accomplice" 44 []
        ancestryFeat gnome 1 NoReq "Burrow Elocutionist" 44 []
        ancestryFeat gnome 1 NoReq "Fey Fellowship" 44 []
        ancestryFeat gnome 1 NoReq "First World Magic" 44 []
        ancestryFeat gnome 1 NoReq "Gnome Obsession" 44 [] // TODO encode choosing a lore skill
        ancestryFeat gnome 1 NoReq "Gnome Weapon Familiarity" 44 [
            AddSkill ((Char2.weaponSkill Weapons.glaive), Trained)
            AddSkill ((Char2.weaponSkill Weapons.kukri), Trained)
            // TODO access to uncommon weapons
            Recategorise (MartialWeapon, Weapons.gnome, SimpleWeapon)
            Recategorise (AdvancedWeapon, Weapons.gnome, MartialWeapon)
        ]
        ancestryFeat gnome 1 NoReq "Illusion Sense" 44 []
        ancestryFeat gnome 5 (FeatReq "Burrow Elocutionist") "Animal Elocutionist" 45 []
        ancestryFeat gnome 5 NoReq "Energized Font" 45 [] // TODO encode one of several feats
        ancestryFeat gnome 5 (FeatReq "Gnome Weapon Familiarity") "Gnome Weapon Innovator" 45 []
        ancestryFeat gnome 9 NoReq "First World Adept" 45 [] // TODO funny requirement again
        ancestryFeat gnome 9 NoReq "Vivacious Conduit" 45 []
        ancestryFeat gnome 13 (FeatReq "Gnome Weapon Familiarity") "Gnome Weapon Expertise" 45 [] // TODO this
    ]

    let goblinHeritages = [
        heritage "Charhide" []
        heritage "Irongut" []
        heritage "Razortooth" []
        heritage "Snow" [] // TODO resistances
        heritage "Unbreakable" [
            Improve2.single (IncreaseHitPointsFlat 4)
        ]
    ]

    let goblinAncestryFeats = [
        ancestryFeat goblin 1 NoReq "Burn It!" 48 []
        ancestryFeat goblin 1 NoReq "City Scavenger" 48 []
        ancestryFeat goblin 1 NoReq "Goblin Lore" 48 [
            AddSkillOr (Skills.nature, Skills.regularSkills, Trained)
            AddSkillOr (Skills.stealth, Skills.regularSkills, Trained)
            AddSkill ((Skills.lore "Goblin"), Trained)
        ]
        ancestryFeat goblin 1 NoReq "Goblin Scuttle" 48 []
        ancestryFeat goblin 1 NoReq "Goblin Song" 48 []
        ancestryFeat goblin 1 NoReq "Goblin Weapon Familiarity" 48 [
            AddSkill ((Char2.weaponSkill Weapons.dogslicer), Trained)
            AddSkill ((Char2.weaponSkill Weapons.horsechopper), Trained)
            // TODO uncommon
            Recategorise (MartialWeapon, Weapons.goblin, SimpleWeapon)
            Recategorise (AdvancedWeapon, Weapons.goblin, MartialWeapon)
        ]
        ancestryFeat goblin 1 NoReq "Junk Tinker" 48 []
        ancestryFeat goblin 1 NoReq "Rough Rider" 48 [
            Feats.ride
        ]
        ancestryFeat goblin 1 NoReq "Very Sneaky" 49 []
        ancestryFeat goblin 5 (FeatReq "Goblin Weapon Familiarity") "Goblin Weapon Frenzy" 49 []
        ancestryFeat goblin 9 NoReq "Cave Climber" 49 []
        ancestryFeat goblin 9 (FeatReq "Goblin Scuttle") "Skittering Scuttle" 49 []
        ancestryFeat goblin 13 (FeatReq "Goblin Weapon Familiarity") "Goblin Weapon Expertise" 49 [] // TODO this
        ancestryFeat goblin 13 (FeatReq "Very Sneaky") "Very, Very Sneaky" 49 []
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
        ancestryFeat halfling 1 NoReq "Distracting Shadows" 52 []
        ancestryFeat halfling 1 NoReq "Halfling Lore" 52 [
            AddSkillOr (Skills.acrobatics, Skills.regularSkills, Trained)
            AddSkillOr (Skills.stealth, Skills.regularSkills, Trained)
            AddSkill ((Skills.lore "Halfling"), Trained)
        ]
        ancestryFeat halfling 1 NoReq "Halfling Luck" 52 []
        ancestryFeat halfling 1 NoReq "Halfling Weapon Familiarity" 52 [
            AddSkill ((Char2.weaponSkill Weapons.halflingSlingStaff), Trained)
            AddSkill ((Char2.weaponSkill Weapons.shortsword), Trained)
            AddSkill ((Char2.weaponSkill Weapons.sling), Trained)
            // TODO uncommon weapons
            Recategorise (MartialWeapon, Weapons.halfling, SimpleWeapon)
            Recategorise (AdvancedWeapon, Weapons.halfling, MartialWeapon)
        ]
        ancestryFeat halfling 1 NoReq "Sure Feet" 52 []
        ancestryFeat halfling 1 NoReq "Titan Slinger" 52 []
        ancestryFeat halfling 1 NoReq "Unfettered Halfling" 52 []
        ancestryFeat halfling 1 NoReq "Watchful Halfling" 52 []
        ancestryFeat halfling 5 NoReq "Cultural Adaptability" 53 []
        ancestryFeat halfling 5 (FeatReq "Halfling Weapon Familiarity") "Halfling Weapon Trickster" 53 []
        ancestryFeat halfling 9 (FeatReq "Halfling Luck") "Guiding Luck" 53 []
        ancestryFeat halfling 9 NoReq "Irrepressible" 53 []
        ancestryFeat halfling 13 (FeatReq "Distracting Shadows") "Ceaseless Shadows" 53 []
        ancestryFeat halfling 13 (FeatReq "Halfling Weapon Familiarity") "Halfling Weapon Expertise" 53 [] // TODO this
    ]

    let humanHeritages = [
        NoChange "No heritage"
        heritage halfElf []
        heritage halfOrc []
    ]

    let humanAncestryFeats = [
        ancestryFeat human 1 NoReq "Adapted Cantrip" 57 [] // TODO require spellcasting
        ancestryFeat human 1 NoReq "Cooperative Nature" 57 []
        ancestryFeatWith human 1 NoReq "General Training" 57 [
            { Prompt = "General feat"; Choices = Feats.generalFeats; Count = Some 1 } // TODO Defer choice; select multiple times
        ]
        ancestryFeat human 1 NoReq "Haughty Obstinacy" 57 []
        ancestryFeat human 1 NoReq "Natural Ambition" 57 [] // TODO deferred choice of class feat
        ancestryFeatWith human 1 NoReq "Natural Skill" 57 [
            Skills.add Skills.regularSkills Trained 2
        ]
        ancestryFeat human 1 NoReq "Unconventional Weaponry" 57 [] // TODO weapon skills
        ancestryFeat human 5 (FeatReq "Adapted Cantrip") "Adaptive Adept" 57 [] // TODO require 3rd level spells
        ancestryFeat human 5 NoReq "Clever Improviser" 57 []
        ancestryFeat human 9 (FeatReq "Cooperative Nature") "Cooperative Soul" 57 []
        ancestryFeat human 9 (FeatReq "Clever Improviser") "Incredible Improvisation" 57 []
        ancestryFeat human 9 NoReq "Multitalented" 58 [] // TODO multiclass feats
        ancestryFeat human 13 (FeatReq "Unconventional Weaponry") "Unconventional Expertise" 58 []

        // Half-elf ancestry feats
        ancestryFeat human 1 (HeritageReq halfElf) "Elf Atavism" 58 []
        ancestryFeat human 5 (HeritageReq halfElf) "Inspire Imitation" 58 []
        ancestryFeat human 5 (HeritageReq halfElf) "Supernatural Charm" 58 []

        // Half-orc ancestry feats
        ancestryFeat human 1 (HeritageReq halfOrc) "Monstrous Peacemaker" 59 []
        ancestryFeat human 1 (HeritageReq halfOrc) "Orc Ferocity" 59 []
        ancestryFeat human 1 (HeritageReq halfOrc >&& FeatReq "Low-light vision") "Orc Sight" 59 []
        ancestryFeat human 1 (HeritageReq halfOrc) "Orc Superstition" 59 []
        ancestryFeat human 1 (HeritageReq halfOrc) "Orc Weapon Familiarity" 59 [] // TODO weapon proficiencies
        ancestryFeat human 5 (HeritageReq halfOrc >&& FeatReq "Orc Weapon Proficiency") "Orc Weapon Carnage" 59 []
        ancestryFeat human 5 (HeritageReq halfOrc) "Victorious Vigor" 59 []
        ancestryFeat human 9 (HeritageReq halfOrc >&& FeatReq "Orc Superstition") "Pervasive Superstition" 59 []
        ancestryFeat human 13 (HeritageReq halfOrc >&& FeatReq "Orc Ferocity") "Incredible Ferocity" 59 []
        ancestryFeat human 13 (HeritageReq halfOrc >&& FeatReq "Orc Weapon Familiarity") "Orc Weapon Expertise" 59 []
    ]

    open Npc.Improve2
    let ancestries = {
        Prompt = "Ancestry"
        Choices = [
            AddAncestry (dwarf, [
                hitPointsFlat 10
                size Medium
                speed 20<Feet>
                abilityFlaw [Charisma] 1
                abilityBoost [Constitution] 1
                abilityBoost [Wisdom] 1
                abilityBoost [Strength; Dexterity; Intelligence; Charisma] 1
                Feats.forceAdd Feats.darkvision
                heritage dwarfHeritages
                feat "Ancestry" dwarfAncestryFeats 1
            ])
            AddAncestry (elf, [
                hitPointsFlat 6
                size Medium
                speed 30<Feet>
                abilityFlaw [Constitution] 1
                abilityBoost [Dexterity] 1
                abilityBoost [Wisdom] 1
                abilityBoost [Strength; Constitution; Intelligence; Charisma] 1
                Feats.forceAdd Feats.lowLightVision
                heritage elfHeritages
                feat "Ancestry" elfAncestryFeats 1
            ])
            AddAncestry (gnome, [
                hitPointsFlat 8
                size Small
                speed 25<Feet>
                abilityFlaw [Strength] 1
                abilityBoost [Constitution] 1
                abilityBoost [Charisma] 1
                abilityBoost [Strength; Dexterity; Intelligence; Wisdom] 1
                Feats.forceAdd Feats.lowLightVision
                heritage gnomeHeritages
                feat "Ancestry" gnomeAncestryFeats 1
            ])
            AddAncestry(goblin, [
                hitPointsFlat 6
                size Small
                speed 25<Feet>
                abilityFlaw [Wisdom] 1
                abilityBoost [Dexterity] 1
                abilityBoost [Charisma] 1
                abilityBoost [Strength; Constitution; Intelligence; Wisdom] 1
                Feats.forceAdd Feats.darkvision
                heritage goblinHeritages
                feat "Ancestry" goblinAncestryFeats 1
            ])
            AddAncestry(halfling, [
                hitPointsFlat 6
                size Small
                speed 25<Feet>
                abilityFlaw [Strength] 1
                abilityBoost [Dexterity] 1
                abilityBoost [Wisdom] 1
                abilityBoost [Strength; Constitution; Intelligence; Charisma] 1
                Feats.forceAdd Feats.keenEyes
                heritage halflingHeritages
                feat "Ancestry" halflingAncestryFeats 1
            ])
            AddAncestry(human, [
                hitPointsFlat 8
                size Medium
                speed 25<Feet>
                anyAbilityBoost 2
                heritage humanHeritages
                feat "Ancestry" humanAncestryFeats 1
            ])
        ]
        Count = Some 1
    }

    // The ancestry feats all have their matching ancestries as prerequisites, so this whole list
    // can be applied to characters leveling up and gaining extra ones
    let ancestryFeats = List.concat [dwarfAncestryFeats; elfAncestryFeats; gnomeAncestryFeats; goblinAncestryFeats; halflingAncestryFeats; humanAncestryFeats]
    let addAncestryFeat = feat "Ancestry feat" ancestryFeats 1