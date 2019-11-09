namespace NpcConsole.Classes

open NpcConsole
open NpcConsole.Attributes
open NpcConsole.Classes.ClassBasics

module Ranger =

    let huntPrey = classFeat Ranger 1 [] "Hunt Prey" 168 []
    let huntersEdge = classFeat Ranger 1 [] "Hunter's Edge" 168 []
    let tracklessStep = classFeat Ranger 5 [] "Trackless Step" 169 []
    let weaponExpertise = classFeat Ranger 5 [] "Weapon Expertise" 169 [
        Weapons.improveSkill (SimpleWeapon, Melee) Expert
        Weapons.improveSkill (SimpleWeapon, Ranged) Expert
        Weapons.improveSkill (MartialWeapon, Melee) Expert
        Weapons.improveSkill (MartialWeapon, Ranged) Expert
    ]
    let vigilantSenses = classFeat Ranger 7 [] "Vigilant Senses" 169 [
        Improve.skill Skills.perception Master
    ]
    let naturesEdge = classFeat Ranger 9 [] "Nature's Edge" 169 []
    let rangerExpertise = classFeat Ranger 9 [] "Ranger Expertise" 169 [
        classSkill Ranger Expert
    ]

    let rangerFeats = [
        classFeat Ranger 1 [] "Animal Companion" 170 []
        classFeat Ranger 1 [] "Crossbow Ace" 171 []
        classFeat Ranger 1 [] "Hunted Shot" 171 []
        classFeat Ranger 1 [] "Monster Hunter" 171 []
        classFeat Ranger 1 [] "Twin Takedown" 171 []
        classFeat Ranger 2 [] "Favored Terrain" 171 [] // TODO terrain specific bonuses
        classFeat Ranger 2 [] "Hunter's Aim" 172 []
        classFeat Ranger 2 [Improve.hasFeat "Monster Hunter"] "Monster Warden" 172 []
        classFeat Ranger 2 [] "Quick Draw" 172 []
        classFeat Ranger 2 [] "Wild Empathy" 172 []
        classFeat Ranger 4 [Improve.hasFeat "Animal Companion"] "Companion's Cry" 172 []
        classFeat Ranger 4 [] "Disrupt Prey" 172 []
        classFeat Ranger 4 [] "Far Shot" 172 [] // TODO double weapons' ranged increments (character sheet?)
        classFeat Ranger 4 [] "Favored Enemy" 172 [] // TODO choose one; apply bonus
        classFeat Ranger 4 [] "Running Reload" 172 []
        classFeat Ranger 4 [] "Scout's Warning" 172 []
        classFeat Ranger 4 [Improve.hasSkill Skills.crafting Expert; Improve.hasFeat "Snare Crafting"] "Snare Specialist" 172 []
        classFeat Ranger 4 [] "Twin Parry" 172 [] // TODO implement dual wielding
        classFeat Ranger 6 [Improve.hasFeat "Animal Companion"] "Mature Animal Companion" 173 []
        classFeat Ranger 6 [Improve.hasSkill Skills.crafting Expert; Improve.hasFeat "Snare Crafting"] "Quick Snares" 173 []
        classFeat Ranger 6 [] "Skirmish Strike" 173 []
        classFeat Ranger 6 [] "Snap Shot" 173 []
        classFeat Ranger 6 [Improve.hasSkill Skills.survival Expert; Improve.hasFeat "Experienced Tracker"] "Swift Tracker" 173 []
        classFeat Ranger 8 [Improve.hasSkill Skills.perception Master] "Blind-Fight" 173 []
        classFeat Ranger 8 [Improve.hasFeat "Weapon Specialization"] "Deadly Aim" 174 []
        classFeat Ranger 8 [] "Hazard Finder" 174 []
        classFeat Ranger 8 [Improve.hasSkill Skills.crafting Master; Improve.hasFeat "Snare Crafting"] "Powerful Snares" 174 []
        classFeat Ranger 8 [Improve.hasSkill Skills.survival Master; Improve.hasFeat "Wild Stride"; Improve.hasFeat "Favored Terrain"] "Terrain Master" 174 []
        classFeat Ranger 8 [] "Warden's Boon" 174 []
        classFeat Ranger 10 [Improve.hasSkill Skills.stealth Master] "Camouflage" 174 []
        classFeat Ranger 10 [Improve.hasFeat "Mature Animal Companion"] "Incredible Companion" 175 []
        classFeat Ranger 10 [Improve.hasSkill Skills.nature Master; Improve.hasFeat "Monster Hunter"] "Master Monster Hunter" 175 []
        classFeat Ranger 10 [] "Penetrating Shot" 175 [] // TODO require ranged weapon
        classFeat Ranger 10 [Improve.hasFeat "Twin Parry"] "Twin Riposte" 175 [] // TODO implement dual wielding
        classFeat Ranger 10 [Improve.hasSkill Skills.stealth Master] "Warden's Step" 175 []
    ]

    let ranger c =
        match c.Level with 
        | 1<Level> ->
            { c with Class = Some Ranger }, [
                Improve.addFeats (classAbilityBoostFeats Ranger [Strength; Dexterity]) 1
                Improve.hitPointsPerLevel 10
                Improve.skill Skills.perception Expert
                Improve.skill Skills.fortitudeSave Expert
                Improve.skill Skills.reflexSave Expert
                Improve.skill Skills.willSave Trained
                Improve.skill Skills.nature Trained
                Improve.skill Skills.survival Trained
                Improve.skills Skills.regularSkills Trained ((modValue Intelligence c) + 4)
                Weapons.improveSkill (SimpleWeapon, Melee) Trained
                Weapons.improveSkill (SimpleWeapon, Ranged) Trained
                Weapons.improveSkill (MartialWeapon, Melee) Trained
                Weapons.improveSkill (MartialWeapon, Ranged) Trained
                Weapons.improveSkill (Unarmed, Melee) Trained
                Improve.skill (Skills.armorSkill LightArmor) Trained
                Improve.skill (Skills.armorSkill MediumArmor) Trained
                Improve.skill (Skills.armorSkill Unarmored) Trained
                Feats.forceAdd huntPrey
                Feats.forceAdd huntersEdge
                Improve.addFeats rangerFeats 1
            ]
        | 2<Level> -> c, [
            Improve.addFeats rangerFeats 1
            Improve.addFeats Feats.skillFeats 1
            ]
        | 3<Level> -> c, [
            Improve.addFeats Feats.generalFeats 1
            Feats.forceAdd Feats.ironWill
            Skills.increase 1
            ]
        | 4<Level> -> c, [
            Improve.addFeats rangerFeats 1
            Improve.addFeats Feats.skillFeats 1
            ]
        | 5<Level> -> c, [
            Improve.anyAbility 4
            Improve.addFeats Ancestry.ancestryFeats 1
            Skills.increase 1
            Feats.forceAdd tracklessStep
            Feats.forceAdd weaponExpertise
            ]
        | 6<Level> -> c, [
            Improve.addFeats rangerFeats 1
            Improve.addFeats Feats.skillFeats 1
            ]
        | 7<Level> -> c, [
            Feats.forceAdd Feats.evasion
            Improve.addFeats Feats.generalFeats 1
            Skills.increase 1
            Feats.forceAdd vigilantSenses
            Feats.forceAdd Feats.weaponSpecialization
            ]
        | 8<Level> -> c, [
            Improve.addFeats rangerFeats 1
            Improve.addFeats Feats.skillFeats 1
            ]
        | 9<Level> -> c, [
            Improve.addFeats Ancestry.ancestryFeats 1
            Feats.forceAdd naturesEdge
            Feats.forceAdd rangerExpertise
            Skills.increase 1
            ]
        | 10<Level> -> c, [
            Improve.anyAbility 4
            Improve.addFeats rangerFeats 1
            Improve.addFeats Feats.skillFeats 1
            ]
        | _ -> failwithf "Bad level: %d" c.Level