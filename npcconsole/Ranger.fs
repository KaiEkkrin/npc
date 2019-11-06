namespace NpcConsole.Classes

open NpcConsole
open NpcConsole.Attributes
open NpcConsole.Classes.ClassBasics

module Ranger =

    let huntPrey = classFeat Ranger 1 [] "Hunt Prey" 168 []
    let huntersEdge = classFeat Ranger 1 [] "Hunter's Edge" 168 []
    let rangerFeats = [
        classFeat Ranger 1 [] "Animal Companion" 170 []
        classFeat Ranger 1 [] "Crossbow Ace" 171 []
        classFeat Ranger 1 [] "Hunted Shot" 171 []
        classFeat Ranger 1 [] "Monster Hunter" 171 []
        classFeat Ranger 1 [] "Twin Takedown" 171 []
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
        | _ -> failwithf "Bad level: %d" c.Level