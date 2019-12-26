namespace Npc.Classes

open Npc
open Npc.Attributes
open Npc.Classes.ClassBasics

module Rogue =

    // For now I'll express sneak attack as this named pool, because
    // it's convenient to do things that way :)
    let sneakAttack = "Sneak attack d6"

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

    let denyAdvantage = classFeat Rogue 3 [] "Deny Advantage" 181 []
    let surpriseAttack = classFeat Rogue 1 [] "Surprise Attack" 181 []
    let weaponTricks = classFeat Rogue 5 [] "Weapon Tricks" 182 [
        Weapons.improveSkill (SimpleWeapon, Melee) Expert
        Weapons.improveSkill (SimpleWeapon, Ranged) Expert
        Improve.skill (Skills.weaponSkill Weapons.rapier) Expert
        Improve.skill (Skills.weaponSkill Weapons.sap) Expert
        Improve.skill (Skills.weaponSkill Weapons.shortbow) Expert
        Improve.skill (Skills.weaponSkill Weapons.shortsword) Expert
    ]
    let vigilantSenses = classFeat Rogue 7 [] "Vigilant Senses" 182 [
        Improve.skill Skills.perception Master
    ]
    let debilitatingStrike = classFeat Rogue 9 [] "Debilitating Strike" 182 []

    let rogueFeats = [
        classFeat Rogue 1 [] "Nimble Dodge" 183 []
        classFeat Rogue 1 [] "Trap Finder" 183 []
        classFeat Rogue 1 [] "Twin Feint" 183 []
        classFeat Rogue 1 [Improve.hasSkill Skills.intimidation Trained] "You're Next" 183 []
        classFeat Rogue 2 [Improve.hasFeat "Ruffian"] "Brutal Beating" 183 []
        classFeat Rogue 2 [Improve.hasFeat "Scoundrel"] "Brutal Beating" 183 []
        classFeat Rogue 2 [] "Minor Magic" 184 [] // TODO choose a magical tradition, etc
        classFeat Rogue 2 [] "Mobility" 184 []
        classFeat Rogue 2 [] "Quick Draw" 184 []
        classFeat Rogue 2 [Improve.hasFeat "Thief"] "Unbalancing Blow" 184 []
        classFeat Rogue 4 [] "Battle Assessment" 184 []
        classFeat Rogue 4 [] "Dread Striker" 184 []
        classFeat Rogue 4 [] "Magical Trickster" 185 []
        classFeat Rogue 4 [] "Poison Weapon" 185 []
        classFeat Rogue 4 [] "Reactive Pursuit" 185 []
        classFeat Rogue 4 [] "Sabotage" 185 []
        classFeat Rogue 4 [] "Scout's Warning" 186 []
        classFeat Rogue 6 [] "Gang Up" 186 []
        classFeat Rogue 6 [] "Light Step" 186 []
        classFeat Rogue 6 [] "Skirmish Strike" 186 []
        classFeat Rogue 6 [] "Twist the Knife" 186 []
        classFeat Rogue 8 [Improve.hasSkill Skills.perception Master] "Blind-Fight" 186 []
        classFeat Rogue 8 [] "Delay Trap" 186 []
        classFeat Rogue 8 [Improve.hasFeat "Poison Weapon"] "Improved Poison Weapon" 187 []
        classFeat Rogue 8 [Improve.hasFeat "Nimble Dodge"] "Nimble Roll" 187 []
        classFeat Rogue 8 [] "Opportune Backstab" 187 []
        classFeat Rogue 8 [] "Sidestep" 187 []
        classFeat Rogue 8 [] "Sly Striker" 187 []
        classFeat Rogue 10 [Improve.hasFeat "Thief"; Improve.hasFeat "Debilitating Strike"] "Precise Debilitations" 187 []
        classFeat Rogue 10 [Improve.hasSkill Skills.stealth Master] "Sneak Savant" 187 []
        classFeat Rogue 10 [Improve.hasFeat "Scoundrel"; Improve.hasFeat "Debilitating Strike"] "Tactical Debilitations" 187 []
        classFeat Rogue 10 [Improve.hasFeat "Ruffian"; Improve.hasFeat "Debilitating Strike"] "Vicious Debilitations" 187 []
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
                Improve.pool (sneakAttack, 1)
            ]
        | 2<Level> -> c, [
            Improve.addFeats rogueFeats 1
            Improve.addFeats Feats.skillFeats 1
            Skills.increase 1
            ]
        | 3<Level> -> c, [
            Feats.forceAdd denyAdvantage
            Improve.addFeats Feats.generalFeats 1
            Improve.addFeats Feats.skillFeats 1
            Skills.increase 1
            ]
        | 4<Level> -> c, [
            Improve.addFeats rogueFeats 1
            Improve.addFeats Feats.skillFeats 1
            Skills.increase 1
            ]
        | 5<Level> -> c, [
            Improve.anyAbility 4
            Improve.addFeats Ancestry.ancestryFeats 1
            Improve.addFeats Feats.skillFeats 1
            Skills.increase 1
            Improve.pool (sneakAttack, 1)
            Feats.forceAdd weaponTricks
            ]
        | 6<Level> -> c, [
            Improve.addFeats rogueFeats 1
            Improve.addFeats Feats.skillFeats 1
            Skills.increase 1
            ]
        | 7<Level> -> c, [
            Feats.forceAdd Feats.evasion
            Improve.addFeats Feats.generalFeats 1
            Improve.addFeats Feats.skillFeats 1
            Skills.increase 1
            Feats.forceAdd vigilantSenses
            Feats.forceAdd Feats.weaponSpecialization
            ]
        | 8<Level> -> c, [
            Improve.addFeats rogueFeats 1
            Improve.addFeats Feats.skillFeats 1
            Skills.increase 1
            ]
        | 9<Level> -> c, [
            Improve.addFeats Ancestry.ancestryFeats 1
            Feats.forceAdd debilitatingStrike
            Feats.forceAdd Feats.greatFortitude
            Improve.addFeats Feats.skillFeats 1
            Skills.increase 1
            ]
        | 10<Level> -> c, [
            Improve.anyAbility 4
            Improve.addFeats rogueFeats 1
            Improve.addFeats Feats.skillFeats 1
            Skills.increase 1
            ]
        | _ -> failwithf "Bad level: %d" c.Level