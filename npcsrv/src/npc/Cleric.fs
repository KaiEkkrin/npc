namespace Npc.Classes

open Npc
open Npc.Attributes
open Npc.Classes.ClassBasics
open Npc.FeatReq

module Cleric =

    let healingFont = classFeat Cleric 1 NoReq "Healing Font" 119 []
    let harmfulFont = classFeat Cleric 1 NoReq "Harmful Font" 119 []
    let divineFonts = [healingFont; harmfulFont]

    let domainInitiate = classFeat Cleric 1 NoReq "Domain Initiate" 121 [] // TODO choose domain; take multiple times

    let clericFeats = [
        // TODO deadly simplicity here
        domainInitiate
        classFeat Cleric 1 (FeatReq "Harmful Font") "Harming Hands" 121 []
        classFeat Cleric 1 (FeatReq "Healing Font") "Healing Hands" 121 []
        classFeat Cleric 1 NoReq "Holy Castigation" 121 [] // TODO require alignment
        classFeat Cleric 1 NoReq "Reach Spell" 122 []
        classFeatWith Cleric 2 NoReq "Cantrip Expansion" 122 [
            Improve2.spell (0, 2)
        ]
        classFeat Cleric 2 NoReq "Communal Healing" 122 []
        classFeat Cleric 2 NoReq "Emblazon Armament" 122 [] // TODO apply bonus on character sheet
        classFeat Cleric 2 NoReq "Sap Life" 122 []
        classFeat Cleric 2 NoReq "Turn Undead" 122 []
        classFeat Cleric 2 NoReq "Versatile Font" 122 []
        classFeat Cleric 4 NoReq "Channel Smite" 122 []
        classFeat Cleric 4 (FeatReq "Harmful Font") "Command Undead" 123 [] // TODO require alignment
        classFeat Cleric 4 NoReq "Directed Channel" 123 []
        classFeat Cleric 4 (FeatReq "Communal Healing") "Improved Communal Healing" 123 []
        classFeat Cleric 4 (FeatReq "Harmful Font") "Necrotic Infusion" 123 [] // TODO require alignment
        classFeat Cleric 6 NoReq "Cast Down" 123 []
        classFeat Cleric 6 NoReq "Divine Weapon" 123 []
        classFeat Cleric 6 NoReq "Selective Energy" 123 []
        classFeat Cleric 6 NoReq "Steady Spellcasting" 123 []
        classFeatWith Cleric 8 (FeatReq "Domain Initiate") "Advanced Domain" 123 [
            Improve2.pool ("Focus", 1)
        ]
        classFeat Cleric 8 NoReq "Align Armament" 124 []
        classFeat Cleric 8 (FeatReq "Healing Font") "Channeled Succor" 124 []
        classFeat Cleric 8 NoReq "Cremate Undead" 124 []
        classFeat Cleric 8 (FeatReq "Emblazon Armament") "Emblazon Energy" 124 [] // TODO choice
        classFeat Cleric 10 (FeatReq "Holy Castigation") "Castigating Weapon" 125 []
        classFeat Cleric 10 (FeatReq "Healing Font") "Heroic Recovery" 125 [] // TODO alignment
        classFeat Cleric 10 (FeatReq "Harmful Font" >&& FeatReq "Command Undead") "Improved Command Undead" 125 []
        classFeat Cleric 10 NoReq "Replenishment of War" 125 [] // TODO require expert in deity's favored weapon
    ]

    let addClericFeat = Improve2.feat "Cleric feat" clericFeats 1

    // First Doctrine is tacked onto the Doctrines themselves.  Subsequent ones
    // are added as a choice between the two.
    let cloisteredCleric = classFeatWith Cleric 1 NoReq "Cloistered Cleric" 119 [
        Feats.forceAdd domainInitiate
    ]
    let warpriest = classFeatWith Cleric 1 NoReq "Warpriest" 120 [
        Improve2.skill (Char2.armorSkill LightArmor) Trained
        Improve2.skill (Char2.armorSkill MediumArmor) Trained
        Improve2.skill Skills.fortitudeSave Expert
        Feats.forceAdd Feats.shieldBlock
        // TODO deadly simplicity if applicable
    ]

    let cloisteredCleric2 = classFeatWith Cleric 3 (FeatReq "Cloistered Cleric") "Cloistered Cleric 2" 119 [
        Improve2.skill Skills.fortitudeSave Expert
    ]
    let cloisteredCleric3 = classFeatWith Cleric 7 (FeatReq "Cloistered Cleric 2") "Cloistered Cleric 3" 119 [
        increaseSpellSkill Divine Expert
    ]

    let warpriest2 = classFeatWith Cleric 3 (FeatReq "Warpriest") "Warpriest 2" 120 [
        Weapons.improveSkill (MartialWeapon, Melee) Trained
        Weapons.improveSkill (MartialWeapon, Ranged) Trained
    ]

    let warpriest3 = classFeatWith Cleric 3 (FeatReq "Warpriest 2") "Warpriest 3" 120 [
        // TODO this is wrong, but I'm using it as an approximation for now.
        // I really want to apply it to the deity's favoured weapon.
        // Also, critical specialization effect.
        Weapons.improveSkill (SimpleWeapon, Melee) Expert
        Weapons.improveSkill (SimpleWeapon, Ranged) Expert
    ]

    let cleric = [
        AddClass (Cleric, [
            Improve2.feat "Ability boost" (spellcastingClassAbilityBoostFeats Cleric Divine [Wisdom]) 1
            Improve2.hitPointsPerLevel 8
            Improve2.skill Skills.perception Trained
            Improve2.skill Skills.fortitudeSave Trained
            Improve2.skill Skills.reflexSave Trained
            Improve2.skill Skills.willSave Expert
            Improve2.skill Skills.religion Trained
            Improve2.skillsBasedOnInt 3 Skills.regularSkills // I rolled the deity-based skill into this one
            Weapons.improveSkill (SimpleWeapon, Melee) Trained
            Weapons.improveSkill (SimpleWeapon, Ranged) Trained
            Weapons.improveSkill (Unarmed, Melee) Trained
            Improve2.skill (Char2.armorSkill Unarmored) Trained
            // TODO Choose deific weapon
            Improve2.pool ("Focus", 1)
            Improve2.spell (0, 5)
            Improve2.spell (1, 2)
            Improve2.feat "Divine Font" divineFonts 1
            Improve2.feat "Doctrine" [cloisteredCleric; warpriest] 1
        ])
        LevelUp (Cleric, 2<Level>, [
            addClericFeat
            Feats.addSkillFeat
            Improve2.spell (1, 1)
        ])
        LevelUp (Cleric, 3<Level>, [
            Feats.addGeneralFeat
            Improve2.feat "Second Doctrine" [cloisteredCleric2; warpriest2] 1
            Skills.increase Skills.regularSkills
            Improve2.spell (2, 2)
        ])
        LevelUp (Cleric, 4<Level>, [
            addClericFeat
            Feats.addSkillFeat
            Improve2.spell (2, 1)
        ])
        LevelUp (Cleric, 5<Level>, [
            Improve2.anyAbilityBoost 4
            Feats.forceAdd Feats.alertness
            Ancestry.addAncestryFeat
            Skills.increase Skills.regularSkills
            Improve2.spell (3, 2)
        ])
        LevelUp (Cleric, 6<Level>, [
            addClericFeat
            Feats.addSkillFeat
            Improve2.spell (3, 1)
        ])
        LevelUp (Cleric, 7<Level>, [
            Feats.addGeneralFeat
            Skills.increase Skills.regularSkills
            Improve2.feat "Third doctrine" [cloisteredCleric3; warpriest3] 1
            Improve2.spell (4, 2)
        ])
        LevelUp (Cleric, 8<Level>, [
            addClericFeat
            Feats.addSkillFeat
            Improve2.spell (4, 1)
        ])
        LevelUp (Cleric, 9<Level>, [
            Ancestry.addAncestryFeat
            Feats.forceAdd Feats.resolve
            Skills.increase Skills.regularSkills
            Improve2.spell (5, 2)
        ])
        LevelUp (Cleric, 10<Level>, [
            Improve2.anyAbilityBoost 4
            addClericFeat
            Feats.addSkillFeat
            Improve2.spell (5, 1)
        ])
    ]
