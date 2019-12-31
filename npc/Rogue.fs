namespace Npc.Classes

open Npc
open Npc.Attributes
open Npc.Classes.ClassBasics
open Npc.FeatReq

module Rogue =

    // For now I'll express sneak attack as this named pool, because
    // it's convenient to do things that way :)
    let sneakAttack = "Sneak attack d6"

    // Because the rogue's key ability score can vary by racket, it's assigned
    // through the racket rather than directly when you choose the Rogue class
    let rogueRackets = [
        classFeatWith Rogue 1 NoReq "Ruffian" 180 [
            Improve2.skill Skills.intimidation Trained
            Improve2.skill (Char2.armorSkill MediumArmor) Trained
            Improve2.feat "Ability boost" (classAbilityBoostFeats Rogue [Strength; Dexterity]) 1
            // TODO when gaining expert/master light armor gain expert/master medium
            // armor as well
        ]
        classFeatWith Rogue 1 NoReq "Scoundrel" 180 [
            Improve2.skill Skills.deception Trained
            Improve2.skill Skills.diplomacy Trained
            Improve2.feat "Ability boost" (classAbilityBoostFeats Rogue [Dexterity; Charisma]) 1
        ]
        classFeatWith Rogue 1 NoReq "Thief" 180 [
            // TODO add Dexterity not Strength to damage when attacking with a
            // finesse weapon
            Improve2.skill Skills.thievery Trained
            Improve2.feat "Ability boost" (classAbilityBoostFeats Rogue [Dexterity]) 1
        ]
    ]

    let denyAdvantage = classFeat Rogue 3 NoReq "Deny Advantage" 181 []
    let surpriseAttack = classFeat Rogue 1 NoReq "Surprise Attack" 181 []
    let weaponTricks = classFeatWith Rogue 5 NoReq "Weapon Tricks" 182 [
        Weapons.improveSkill (SimpleWeapon, Melee) Expert
        Weapons.improveSkill (SimpleWeapon, Ranged) Expert
        Improve2.skill (Char2.weaponSkill Weapons.rapier) Expert
        Improve2.skill (Char2.weaponSkill Weapons.sap) Expert
        Improve2.skill (Char2.weaponSkill Weapons.shortbow) Expert
        Improve2.skill (Char2.weaponSkill Weapons.shortsword) Expert
    ]
    let vigilantSenses = classFeatWith Rogue 7 NoReq "Vigilant Senses" 182 [
        Improve2.skill Skills.perception Master
    ]
    let debilitatingStrike = classFeat Rogue 9 NoReq "Debilitating Strike" 182 []

    let rogueFeats = [
        classFeat Rogue 1 NoReq "Nimble Dodge" 183 []
        classFeat Rogue 1 NoReq "Trap Finder" 183 []
        classFeat Rogue 1 NoReq "Twin Feint" 183 []
        classFeat Rogue 1 (SkillReq (Skills.intimidation, Trained)) "You're Next" 183 []
        classFeat Rogue 2 (FeatReq "Ruffian") "Brutal Beating" 183 []
        classFeat Rogue 2 (FeatReq "Scoundrel") "Brutal Beating" 183 []
        classFeat Rogue 2 NoReq "Minor Magic" 184 [] // TODO choose a magical tradition, etc
        classFeat Rogue 2 NoReq "Mobility" 184 []
        classFeat Rogue 2 NoReq "Quick Draw" 184 []
        classFeat Rogue 2 (FeatReq "Thief") "Unbalancing Blow" 184 []
        classFeat Rogue 4 NoReq "Battle Assessment" 184 []
        classFeat Rogue 4 NoReq "Dread Striker" 184 []
        classFeat Rogue 4 NoReq "Magical Trickster" 185 []
        classFeat Rogue 4 NoReq "Poison Weapon" 185 []
        classFeat Rogue 4 NoReq "Reactive Pursuit" 185 []
        classFeat Rogue 4 NoReq "Sabotage" 185 []
        classFeat Rogue 4 NoReq "Scout's Warning" 186 []
        classFeat Rogue 6 NoReq "Gang Up" 186 []
        classFeat Rogue 6 NoReq "Light Step" 186 []
        classFeat Rogue 6 NoReq "Skirmish Strike" 186 []
        classFeat Rogue 6 NoReq "Twist the Knife" 186 []
        classFeat Rogue 8 (SkillReq (Skills.perception, Master)) "Blind-Fight" 186 []
        classFeat Rogue 8 NoReq "Delay Trap" 186 []
        classFeat Rogue 8 (FeatReq "Poison Weapon") "Improved Poison Weapon" 187 []
        classFeat Rogue 8 (FeatReq "Nimble Dodge") "Nimble Roll" 187 []
        classFeat Rogue 8 NoReq "Opportune Backstab" 187 []
        classFeat Rogue 8 NoReq "Sidestep" 187 []
        classFeat Rogue 8 NoReq "Sly Striker" 187 []
        classFeat Rogue 10 (FeatReq "Thief" >&& FeatReq "Debilitating Strike") "Precise Debilitations" 187 []
        classFeat Rogue 10 (SkillReq (Skills.stealth, Master)) "Sneak Savant" 187 []
        classFeat Rogue 10 (FeatReq "Scoundrel" >&& FeatReq "Debilitating Strike") "Tactical Debilitations" 187 []
        classFeat Rogue 10 (FeatReq "Ruffian" >&& FeatReq "Debilitating Strike") "Vicious Debilitations" 187 []
    ]

    let addRogueFeat = Improve2.feat "Rogue feat" rogueFeats 1

    let rogue = [
        AddClass (Rogue, [
            Improve2.hitPointsPerLevel 8
            Improve2.skill Skills.perception Expert
            Improve2.skill Skills.fortitudeSave Trained
            Improve2.skill Skills.reflexSave Expert
            Improve2.skill Skills.willSave Expert
            Improve2.skill Skills.stealth Trained
            Weapons.improveSkill (SimpleWeapon, Melee) Trained
            Weapons.improveSkill (SimpleWeapon, Ranged) Trained
            Weapons.improveSkill (Unarmed, Melee) Trained
            Improve2.skill (Char2.weaponSkill Weapons.rapier) Trained
            Improve2.skill (Char2.weaponSkill Weapons.sap) Trained
            Improve2.skill (Char2.weaponSkill Weapons.shortbow) Trained
            Improve2.skill (Char2.weaponSkill Weapons.shortsword) Trained
            Improve2.skill (Char2.armorSkill LightArmor) Trained
            Improve2.skill (Char2.armorSkill Unarmored) Trained
            Improve2.feat "Racket" rogueRackets 1
            Improve2.skillsBasedOnInt 7 Skills.regularSkills
            Feats.forceAdd surpriseAttack
            addRogueFeat
            Improve2.pool (sneakAttack, 1)
        ])
        LevelUp (Rogue, 2<Level>, [
            addRogueFeat
            Feats.addSkillFeat
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Rogue, 3<Level>, [
            Feats.forceAdd denyAdvantage
            Feats.addGeneralFeat
            Feats.addSkillFeat
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Rogue, 4<Level>, [
            addRogueFeat
            Feats.addSkillFeat
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Rogue, 5<Level>, [
            Improve2.anyAbilityBoost 4
            Ancestry.addAncestryFeat
            Feats.addSkillFeat
            Skills.increase Skills.regularSkills
            Improve2.pool (sneakAttack, 1)
            Feats.forceAdd weaponTricks
        ])
        LevelUp (Rogue, 6<Level>, [
            addRogueFeat
            Feats.addSkillFeat
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Rogue, 7<Level>, [
            Feats.forceAdd Feats.evasion
            Feats.addGeneralFeat
            Feats.addSkillFeat
            Skills.increase Skills.regularSkills
            Feats.forceAdd vigilantSenses
            Feats.forceAdd Feats.weaponSpecialization
        ])
        LevelUp (Rogue, 8<Level>, [
            addRogueFeat
            Feats.addSkillFeat
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Rogue, 9<Level>, [
            Ancestry.addAncestryFeat
            Feats.forceAdd debilitatingStrike
            Feats.forceAdd Feats.greatFortitude
            Feats.addSkillFeat
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Rogue, 10<Level>, [
            Improve2.anyAbilityBoost 4
            addRogueFeat
            Feats.addSkillFeat
            Skills.increase Skills.regularSkills
        ])
    ]