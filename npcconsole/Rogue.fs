namespace NpcConsole.Classes

open NpcConsole
open NpcConsole.Attributes
open NpcConsole.Classes.ClassBasics

module Rogue =

    // TODO sneak attack (damage dice); maybe change the Class discriminated union
    // so that it includes things like sneak attack dice, focus points etc etc?

    // Because the rogue's key ability score can vary by racket, it's assigned
    // through the racket rather than directly when you choose the Rogue class
    let rogueRackets = [
        classFeat Rogue 1 [] "Ruffian" 180 [
            Improve.skill Skills.intimidation Trained
            Improve.skill (Skills.armorSkill MediumArmor) Trained
            Improve.addFeats (classAbilityBoostFeats Rogue [Strength; Dexterity]) 1
            // TODO when gaining expert/master light armor gain expert/master medium
            // armor as well
        ]
        classFeat Rogue 1 [] "Scoundrel" 180 [
            Improve.skill Skills.deception Trained
            Improve.skill Skills.diplomacy Trained
            Improve.addFeats (classAbilityBoostFeats Rogue [Dexterity; Charisma]) 1
        ]
        classFeat Rogue 1 [] "Thief" 180 [
            // TODO add Dexterity not Strength to damage when attacking with a
            // finesse weapon
            Improve.skill Skills.thievery Trained
            Improve.addFeats (classAbilityBoostFeats Rogue [Dexterity]) 1
        ]
    ]

    let surpriseAttack = classFeat Rogue 1 [] "Surprise Attack" 181 []
    let rogueFeats = [
        classFeat Rogue 1 [] "Nimble Dodge" 183 []
        classFeat Rogue 1 [] "Trap Finder" 183 []
        classFeat Rogue 1 [] "Twin Feint" 183 []
        classFeat Rogue 1 [Improve.hasSkill Skills.intimidation Trained] "You're Next" 183 []
    ]

    let rogue c =
        match c.Level with 
        | 1<Level> ->
            { c with Class = Some Rogue }, [
                Improve.hitPointsPerLevel 8
                Improve.skill Skills.perception Expert
                Improve.skill Skills.fortitudeSave Trained
                Improve.skill Skills.reflexSave Expert
                Improve.skill Skills.willSave Expert
                Improve.skill Skills.stealth Trained
                Weapons.improveSkill (SimpleWeapon, Melee) Trained
                Weapons.improveSkill (SimpleWeapon, Ranged) Trained
                Weapons.improveSkill (Unarmed, Melee) Trained
                Improve.skill (Skills.weaponSkill Weapons.rapier) Trained
                Improve.skill (Skills.weaponSkill Weapons.sap) Trained
                Improve.skill (Skills.weaponSkill Weapons.shortbow) Trained
                Improve.skill (Skills.weaponSkill Weapons.shortsword) Trained
                Improve.skill (Skills.armorSkill LightArmor) Trained
                Improve.skill (Skills.armorSkill Unarmored) Trained
                Improve.addFeats rogueRackets 1
                Improve.skills Skills.regularSkills Trained ((modValue Intelligence c) + 7)
                Feats.forceAdd surpriseAttack
                Improve.addFeats rogueFeats 1
            ]
        | _ -> failwithf "Bad level: %d" c.Level