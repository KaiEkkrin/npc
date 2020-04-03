namespace Npc

open Npc
open Npc.Attributes
open Npc.Classes.ClassBasics
open Npc.FeatReq

module Monk =
    let monk = [
        AddClass (Monk, [
            Improve2.feat "Ability boost" (classAbilityBoostFeats Monk [Strength; Dexterity]) 1
            Improve2.hitPointsPerLevel 10
            Improve2.skill Skills.perception Trained
            Improve2.skill Skills.fortitudeSave Expert
            Improve2.skill Skills.reflexSave Expert
            Improve2.skill Skills.willSave Expert
            Improve2.skillsBasedOnInt 4 Skills.regularSkills
            Weapons.improveSkill (SimpleWeapon, Melee) Trained
            Weapons.improveSkill (SimpleWeapon, Ranged) Trained
            Weapons.improveSkill (Unarmed, Melee) Trained
            Improve2.skill (Char2.armorSkill Unarmored) Expert
        ])
    ]