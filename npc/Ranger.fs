namespace Npc.Classes

open Npc
open Npc.Attributes
open Npc.Classes.ClassBasics
open Npc.FeatReq

module Ranger =

    let huntPrey = classFeat Ranger 1 NoReq "Hunt Prey" 168 []
    let huntersEdge = classFeat Ranger 1 NoReq "Hunter's Edge" 168 []
    let tracklessStep = classFeat Ranger 5 NoReq "Trackless Step" 169 []
    let weaponExpertise = classFeatWith Ranger 5 NoReq "Weapon Expertise" 169 [
        Weapons.improveSkill (SimpleWeapon, Melee) Expert
        Weapons.improveSkill (SimpleWeapon, Ranged) Expert
        Weapons.improveSkill (MartialWeapon, Melee) Expert
        Weapons.improveSkill (MartialWeapon, Ranged) Expert
    ]
    let vigilantSenses = classFeatWith Ranger 7 NoReq "Vigilant Senses" 169 [
        Improve2.skill Skills.perception Master
    ]
    let naturesEdge = classFeat Ranger 9 NoReq "Nature's Edge" 169 []
    let rangerExpertise = classFeatWith Ranger 9 NoReq "Ranger Expertise" 169 [
        classSkill Ranger Expert
    ]

    let rangerFeats = [
        classFeat Ranger 1 NoReq "Animal Companion" 170 []
        classFeat Ranger 1 NoReq "Crossbow Ace" 171 []
        classFeat Ranger 1 NoReq "Hunted Shot" 171 []
        classFeat Ranger 1 NoReq "Monster Hunter" 171 []
        classFeat Ranger 1 NoReq "Twin Takedown" 171 []
        classFeat Ranger 2 NoReq "Favored Terrain" 171 [] // TODO terrain specific bonuses
        classFeat Ranger 2 NoReq "Hunter's Aim" 172 []
        classFeat Ranger 2 (FeatReq "Monster Hunter") "Monster Warden" 172 []
        classFeat Ranger 2 NoReq "Quick Draw" 172 []
        classFeat Ranger 2 NoReq "Wild Empathy" 172 []
        classFeat Ranger 4 (FeatReq "Animal Companion") "Companion's Cry" 172 []
        classFeat Ranger 4 NoReq "Disrupt Prey" 172 []
        classFeat Ranger 4 NoReq "Far Shot" 172 [] // TODO double weapons' ranged increments (character sheet?)
        classFeat Ranger 4 NoReq "Favored Enemy" 172 [] // TODO choose one; apply bonus
        classFeat Ranger 4 NoReq "Running Reload" 172 []
        classFeat Ranger 4 NoReq "Scout's Warning" 172 []
        classFeat Ranger 4 (SkillReq (Skills.crafting, Expert) >&& FeatReq "Snare Crafting") "Snare Specialist" 172 []
        classFeat Ranger 4 NoReq "Twin Parry" 172 [] // TODO implement dual wielding
        classFeat Ranger 6 (FeatReq "Animal Companion") "Mature Animal Companion" 173 []
        classFeat Ranger 6 (SkillReq (Skills.crafting, Expert) >&& FeatReq "Snare Crafting") "Quick Snares" 173 []
        classFeat Ranger 6 NoReq "Skirmish Strike" 173 []
        classFeat Ranger 6 NoReq "Snap Shot" 173 []
        classFeat Ranger 6 (SkillReq (Skills.survival, Expert) >&& FeatReq "Experienced Tracker") "Swift Tracker" 173 []
        classFeat Ranger 8 (SkillReq (Skills.perception, Master)) "Blind-Fight" 173 []
        classFeat Ranger 8 (FeatReq "Weapon Specialization") "Deadly Aim" 174 []
        classFeat Ranger 8 NoReq "Hazard Finder" 174 []
        classFeat Ranger 8 (SkillReq (Skills.crafting, Master) >&& FeatReq "Snare Crafting") "Powerful Snares" 174 []
        classFeat Ranger 8 (SkillReq (Skills.survival, Master) >&& FeatReq "Wild Stride" >&& FeatReq "Favored Terrain") "Terrain Master" 174 []
        classFeat Ranger 8 NoReq "Warden's Boon" 174 []
        classFeat Ranger 10 (SkillReq (Skills.stealth, Master)) "Camouflage" 174 []
        classFeat Ranger 10 (FeatReq "Mature Animal Companion") "Incredible Companion" 175 []
        classFeat Ranger 10 (SkillReq (Skills.nature, Master) >&& FeatReq "Monster Hunter") "Master Monster Hunter" 175 []
        classFeat Ranger 10 NoReq "Penetrating Shot" 175 []
        classFeat Ranger 10 (FeatReq "Twin Parry") "Twin Riposte" 175 [] // TODO implement dual wielding
        classFeat Ranger 10 (SkillReq (Skills.stealth, Master)) "Warden's Step" 175 []
    ]

    let addRangerFeat = Improve2.feat "Ranger feat" rangerFeats 1

    let ranger c =
        match c.Level with 
        | 1<Level> ->
            { c with Class = Some Ranger }, [
                Improve2.feat "Ability boost" (classAbilityBoostFeats Ranger [Strength; Dexterity]) 1
                Improve2.hitPointsPerLevel 10
                Improve2.skill Skills.perception Expert
                Improve2.skill Skills.fortitudeSave Expert
                Improve2.skill Skills.reflexSave Expert
                Improve2.skill Skills.willSave Trained
                Improve2.skill Skills.nature Trained
                Improve2.skill Skills.survival Trained
                Improve2.skills Skills.regularSkills Trained ((modValue Intelligence c) + 4)
                Weapons.improveSkill (SimpleWeapon, Melee) Trained
                Weapons.improveSkill (SimpleWeapon, Ranged) Trained
                Weapons.improveSkill (MartialWeapon, Melee) Trained
                Weapons.improveSkill (MartialWeapon, Ranged) Trained
                Weapons.improveSkill (Unarmed, Melee) Trained
                Improve2.skill (Char2.armorSkill LightArmor) Trained
                Improve2.skill (Char2.armorSkill MediumArmor) Trained
                Improve2.skill (Char2.armorSkill Unarmored) Trained
                Feats.forceAdd huntPrey
                Feats.forceAdd huntersEdge
                addRangerFeat
            ]
        | 2<Level> -> c, [
            addRangerFeat
            Feats.addSkillFeat
            ]
        | 3<Level> -> c, [
            Feats.addGeneralFeat
            Feats.forceAdd Feats.ironWill
            Skills.increase Skills.regularSkills
            ]
        | 4<Level> -> c, [
            addRangerFeat
            Feats.addSkillFeat
            ]
        | 5<Level> -> c, [
            Improve2.anyAbilityBoost 4
            Ancestry.addAncestryFeat
            Skills.increase Skills.regularSkills
            Feats.forceAdd tracklessStep
            Feats.forceAdd weaponExpertise
            ]
        | 6<Level> -> c, [
            addRangerFeat
            Feats.addSkillFeat
            ]
        | 7<Level> -> c, [
            Feats.forceAdd Feats.evasion
            Feats.addGeneralFeat
            Skills.increase Skills.regularSkills
            Feats.forceAdd vigilantSenses
            Feats.forceAdd Feats.weaponSpecialization
            ]
        | 8<Level> -> c, [
            addRangerFeat
            Feats.addSkillFeat
            ]
        | 9<Level> -> c, [
            Ancestry.addAncestryFeat
            Feats.forceAdd naturesEdge
            Feats.forceAdd rangerExpertise
            Skills.increase Skills.regularSkills
            ]
        | 10<Level> -> c, [
            Improve2.anyAbilityBoost 4
            addRangerFeat
            Feats.addSkillFeat
            ]
        | _ -> failwithf "Bad level: %d" c.Level