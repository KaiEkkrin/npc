namespace Npc.Classes

open Npc
open Npc.Attributes
open Npc.Classes.ClassBasics
open Npc.FeatReq

module Champion =

    let champion = [
        AddClass (Champion, [
            Improve2.feat "Ability boost" (spellcastingClassAbilityBoostFeats Champion Divine [Strength; Dexterity]) 1
            Improve2.hitPointsPerLevel 10
            Improve2.skill Skills.perception Trained
            Improve2.skill Skills.fortitudeSave Expert
            Improve2.skill Skills.reflexSave Trained
            Improve2.skill Skills.willSave Expert
            Improve2.skill Skills.religion Trained
            Improve2.skillsBasedOnInt 3 Skills.regularSkills // I rolled the deity-based skill into this one
            Weapons.improveSkill (SimpleWeapon, Melee) Trained
            Weapons.improveSkill (SimpleWeapon, Ranged) Trained
            Weapons.improveSkill (MartialWeapon, Melee) Trained
            Weapons.improveSkill (MartialWeapon, Ranged) Trained
            Weapons.improveSkill (Unarmed, Melee) Trained
            Improve2.skill (Char2.armorSkill LightArmor) Trained
            Improve2.skill (Char2.armorSkill MediumArmor) Trained
            Improve2.skill (Char2.armorSkill HeavyArmor) Trained
            Improve2.skill (Char2.armorSkill Unarmored) Trained
        ])
    ]