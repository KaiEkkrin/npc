namespace Npc

open Npc
open Npc.Attributes
open Npc.Classes.ClassBasics
open Npc.FeatReq

module Barbarian =
    // TODO Provide a separate stats block for the things that are different while raging?
    let rage = classFeat Barbarian 1 NoReq "Rage" 84 []

    let barbarianFeats = [
        classFeat Barbarian 1 NoReq "Acute Vision" 88 []
        classFeat Barbarian 1 NoReq "Moment of Clarity" 88 []
        classFeat Barbarian 1 NoReq "Raging Intimidation" 88 []
        classFeat Barbarian 1 NoReq "Raging Thrower" 88 []
        classFeat Barbarian 1 NoReq "Sudden Charge" 88 []
        classFeat Barbarian 2 (FeatReq "AcuteVision" >|| FeatReq "Darkvision") "Acute Scent" 88 []
        classFeat Barbarian 2 NoReq "Furious Finish" 89 []
        classFeat Barbarian 2 NoReq "No Escape" 89 []
        classFeat Barbarian 2 NoReq "Second Wind" 89 []
        classFeat Barbarian 2 NoReq "Shake it Off" 89 []
        classFeat Barbarian 4 NoReq "Fast Movement" 89 []
        classFeat Barbarian 4 (SkillReq (Skills.athletics, Expert)) "Raging Athlete" 89 []
        classFeat Barbarian 4 NoReq "Swipe" 89 []
        classFeat Barbarian 4 NoReq "Wounded Rage" 89 []
        classFeat Barbarian 6 (FeatReq "Animal Instinct") "Animal Skin" 90 []
        classFeat Barbarian 6 NoReq "Attack of Opportunity" 90 []
        classFeat Barbarian 6 (SkillReq (Skills.athletics, Expert)) "Brutal Bully" 90 []
        classFeat Barbarian 6 NoReq "Cleave" 90 []
        classFeat Barbarian 6 (FeatReq "Dragon Instinct") "Dragon's Rage Breath" 90 []
        classFeat Barbarian 6 (FeatReq "Giant Instinct") "Giant's Stature" 91 [] // TODO require Medium size or smaller
        classFeat Barbarian 6 (FeatReq "Spirit Instinct") "Spirits' Interference" 91 []
        classFeat Barbarian 8 (FeatReq "Animal Instinct") "Animal Rage" 91 []
        classFeat Barbarian 8 (SkillReq (Skills.athletics, Master)) "Furious Bully" 91 [] // TODO circumstance bonus
        classFeat Barbarian 8 NoReq "Renewed Vigor" 91 []
        classFeat Barbarian 8 NoReq "Share Rage" 91 []
        classFeat Barbarian 8 NoReq "Sudden Leap" 91 []
        classFeat Barbarian 8 NoReq "Thrash" 91 []
        classFeat Barbarian 10 NoReq "Come and Get Me" 91 []
        classFeat Barbarian 10 NoReq "Furious Sprint" 91 []
        classFeat Barbarian 10 (FeatReq "Cleave") "Great Cleave" 91 []
        classFeat Barbarian 10 NoReq "Knockback" 91 []
        classFeat Barbarian 10 (FeatReq "Intimidating Glare") "Terrifying Howl" 91 []
    ]

    let addBarbarianFeat = Improve2.feat "Barbarian feat" barbarianFeats 1

    let instincts = [
        // TODO Apply the more complicated effects of these, including animal choices etc.
        classFeat Barbarian 1 NoReq "Animal Instinct" 86 []
        classFeat Barbarian 1 NoReq "Dragon Instinct" 86 []
        classFeatWith Barbarian 1 NoReq "Fury Instinct" 87 [
            addBarbarianFeat
        ]
        classFeat Barbarian 1 NoReq "Giant Instinct" 87 [] // TODO larger weapons...
        classFeat Barbarian 1 NoReq "Spirit Instinct" 87 []
    ]

    let denyAdvantage = classFeat Barbarian 3 NoReq "Deny Advantage" 84 []
    let brutality = classFeatWith Barbarian 5 NoReq "Brutality" 85 [
        Weapons.improveSkill (SimpleWeapon, Melee) Expert
        Weapons.improveSkill (SimpleWeapon, Ranged) Expert
        Weapons.improveSkill (MartialWeapon, Melee) Expert
        Weapons.improveSkill (MartialWeapon, Ranged) Expert
        Weapons.improveSkill (Unarmed, Melee) Expert
    ]
    let ragingResistance = classFeat Barbarian 9 NoReq "Raging Resistance" 85 [] // TODO add resistance stat

    let barbarian = [
        AddClass (Barbarian, [
            Improve2.feat "Ability boost" (classAbilityBoostFeats Barbarian [Strength]) 1
            Improve2.hitPointsPerLevel 12
            Improve2.skill Skills.perception Expert
            Improve2.skill Skills.fortitudeSave Expert
            Improve2.skill Skills.reflexSave Trained
            Improve2.skill Skills.willSave Expert
            Improve2.skill Skills.athletics Trained
            Improve2.skillsBasedOnInt 3 Skills.regularSkills
            Weapons.improveSkill (SimpleWeapon, Melee) Trained
            Weapons.improveSkill (SimpleWeapon, Ranged) Trained
            Weapons.improveSkill (MartialWeapon, Melee) Trained
            Weapons.improveSkill (MartialWeapon, Ranged) Trained
            Weapons.improveSkill (Unarmed, Melee) Trained
            Improve2.skill (Char2.armorSkill LightArmor) Trained
            Improve2.skill (Char2.armorSkill MediumArmor) Trained
            Improve2.skill (Char2.armorSkill Unarmored) Trained
            Feats.forceAdd rage
            Improve2.feat "Barbarian instinct" instincts 1
            addBarbarianFeat
        ])
        LevelUp (Barbarian, 2<Level>, [
            addBarbarianFeat
            Feats.addSkillFeat
        ])
        LevelUp (Barbarian, 3<Level>, [
            Feats.forceAdd denyAdvantage
            Feats.addGeneralFeat
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Barbarian, 4<Level>, [
            addBarbarianFeat
            Feats.addSkillFeat
        ])
        LevelUp (Barbarian, 5<Level>, [
            Improve2.anyAbilityBoost 4
            Ancestry.addAncestryFeat
            Feats.forceAdd brutality
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Barbarian, 6<Level>, [
            addBarbarianFeat
            Feats.addSkillFeat
        ])
        LevelUp (Barbarian, 7<Level>, [
            Feats.addGeneralFeat
            Feats.forceAdd Feats.juggernaut
            Skills.increase Skills.regularSkills
            Feats.forceAdd Feats.weaponSpecialization
        ])
        LevelUp (Barbarian, 8<Level>, [
            addBarbarianFeat
            Feats.addSkillFeat
        ])
        LevelUp (Barbarian, 9<Level>, [
            Ancestry.addAncestryFeat
            Feats.forceAdd Feats.lightningReflexes
            Feats.forceAdd ragingResistance
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Barbarian, 10<Level>, [
            Improve2.anyAbilityBoost 4
            addBarbarianFeat
            Feats.addSkillFeat
        ])
    ]