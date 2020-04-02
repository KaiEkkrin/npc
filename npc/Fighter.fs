namespace Npc

open Npc
open Npc.Attributes
open Npc.Classes.ClassBasics
open Npc.FeatReq

module Fighter =
    let bravery = classFeatWith Fighter 3 NoReq "Bravery" 142 [
        Improve2.skill Skills.willSave Expert
        // TODO success becomes critical success vs fear, etc.
    ]

    // TODO make this; weapon groups are complicated, it'll need a bit of debugging too :)
    let fighterWeaponMastery = classFeat Fighter 5 NoReq "Fighter Weapon Mastery" 143 []

    let battlefieldSurveyor = classFeatWith Fighter 7 NoReq "Battlefield Surveyor" 143 [
        Improve2.skill Skills.perception Master
        // TODO +2 circumstance bonus to Perception for Initiative checks
    ]

    let fighterFeatsRequiring8thLevelOrLower = [
        classFeat Fighter 1 NoReq "Double Slice" 144 [] // TODO require dual wield; implement that :)
        classFeat Fighter 1 NoReq "Exacting Strike" 144 []
        classFeat Fighter 1 NoReq "Point-Blank Shot" 144 []
        classFeat Fighter 1 NoReq "Power Attack" 144 []
        classFeat Fighter 1 NoReq "Reactive Shield" 145 []
        classFeat Fighter 1 NoReq "Snagging Strike" 145 []
        classFeat Fighter 1 NoReq "Sudden Charge" 145 []
        classFeat Fighter 2 NoReq "Aggressive Block" 145 []
        classFeat Fighter 2 NoReq "Assisting Shot" 145 []
        classFeat Fighter 2 NoReq "Brutish Shove" 145 [] // TODO require 2-handed weapon
        classFeat Fighter 2 NoReq "Combat Grab" 146 [] // TODO require one hand free
        classFeat Fighter 2 NoReq "Dueling Parry" 146 [] // TODO require one hand free, etc
        classFeat Fighter 2 NoReq "Intimidating Strike" 146 []
        classFeat Fighter 2 NoReq "Lunge" 146 []
        classFeat Fighter 4 NoReq "Double Shot" 146 [] // TODO require ranged weapon with reload 0
        classFeat Fighter 4 NoReq "Dual-Handed Assault" 146 [] // TODO require one hand free
        classFeat Fighter 4 (SkillReq (Skills.athletics, Trained)) "Knockdown" 146 []
        classFeat Fighter 4 (FeatReq "Aggressive Block" >|| FeatReq "Brutish Shove") "Powerful Shove" 146 []
        classFeat Fighter 4 NoReq "Quick Reversal" 146 []
        classFeat Fighter 4 NoReq "Shielded Stride" 146 []
        classFeat Fighter 4 NoReq "Swipe" 147 []
        classFeat Fighter 4 NoReq "Twin Parry" 147 [] // TODO require dual wield
        classFeat Fighter 6 NoReq "Advanced Weapon Training" 147 [] // TODO implement weapon groups
        classFeat Fighter 6 NoReq "Advantageous Assault" 147 []
        classFeat Fighter 6 (SkillReq (Skills.athletics, Trained)) "Disarming Stance" 147 [] // TODO require one hand free
        classFeat Fighter 6 (FeatReq "Power Attack") "Furious Focus" 148 []
        classFeat Fighter 6 NoReq "Guardian's Deflection" 148 []
        classFeat Fighter 6 NoReq "Reflexive Shield" 148 []
        classFeat Fighter 6 NoReq "Revealing Stab" 148 [] // TODO require piercing melee weapon
        classFeat Fighter 6 NoReq "Shatter Defenses" 149 []
        classFeat Fighter 6 (FeatReq "Shield Block") "Shield Warden" 149 []
        classFeat Fighter 6 (FeatReq "Double Shot") "Triple Shot" 149 []
        classFeat Fighter 8 (SkillReq (Skills.perception, Master)) "Blind-Fight" 149 []
        classFeat Fighter 8 (FeatReq "Dueling Parry") "Dueling Riposte" 149 []
        classFeat Fighter 8 NoReq "Felling Strike" 149 []
        classFeat Fighter 8 NoReq "Incredible Aim" 149 []
        classFeat Fighter 8 NoReq "Mobile Shot Stance" 149 []
        classFeat Fighter 8 NoReq "Positioning Assault" 149 []
        classFeat Fighter 8 (FeatReq "Shield Block" >&& FeatReq "Reactive Shield") "Quick Shield Block" 149 []
        classFeat Fighter 8 NoReq "Sudden Leap" 149 []
    ]

    let fighterFeats = fighterFeatsRequiring8thLevelOrLower @ [
        classFeat Fighter 10 NoReq "Agile Grace" 150 []
        classFeat Fighter 10 NoReq "Certain Strike" 150 []
        classFeat Fighter 10 NoReq "Combat Reflexes" 150 []
        classFeat Fighter 10 NoReq "Debilitating Shot" 150 []
        classFeat Fighter 10 (SkillReq (Skills.athletics, Trained)) "Disarming Twist" 150 []
        classFeat Fighter 10 NoReq "Disruptive Stance" 150 []
        classFeat Fighter 10 NoReq "Fearsome Brute" 151 []
        classFeat Fighter 10 (FeatReq "Knockdown") "Improved Knockdown" 151 []
        classFeat Fighter 10 NoReq "Mirror Shield" 151 []
        classFeat Fighter 10 (FeatReq "Twin Parry") "Twin Riposte" 151 []
    ]

    let combatFlexibility = classFeatWith Fighter 9 NoReq "Combat Flexibility" 143 [
        Improve2.feat "Fighter feat" fighterFeatsRequiring8thLevelOrLower 1
    ]

    let addFighterFeat = Improve2.feat "Fighter feat" fighterFeats 1

    let fighter = [
        AddClass (Fighter, [
            Improve2.feat "Ability boost" (classAbilityBoostFeats Fighter [Strength; Dexterity]) 1
            Improve2.hitPointsPerLevel 10
            Improve2.skill Skills.perception Expert
            Improve2.skill Skills.fortitudeSave Expert
            Improve2.skill Skills.reflexSave Expert
            Improve2.skill Skills.willSave Trained
            Improve2.skills [Skills.athletics; Skills.acrobatics] Trained 1
            Improve2.skillsBasedOnInt 3 Skills.regularSkills
            Weapons.improveSkill (SimpleWeapon, Melee) Expert
            Weapons.improveSkill (SimpleWeapon, Ranged) Expert
            Weapons.improveSkill (MartialWeapon, Melee) Expert
            Weapons.improveSkill (MartialWeapon, Ranged) Expert
            Weapons.improveSkill (AdvancedWeapon, Melee) Trained
            Weapons.improveSkill (AdvancedWeapon, Ranged) Trained
            Weapons.improveSkill (Unarmed, Melee) Expert
            Improve2.skill (Char2.armorSkill LightArmor) Trained
            Improve2.skill (Char2.armorSkill MediumArmor) Trained
            Improve2.skill (Char2.armorSkill HeavyArmor) Trained
            Improve2.skill (Char2.armorSkill Unarmored) Trained
            Feats.forceAdd Feats.attackOfOpportunity
            Feats.forceAdd Feats.shieldBlock
            addFighterFeat
        ])
        LevelUp (Fighter, 2<Level>, [
            addFighterFeat
            Feats.addSkillFeat
        ])
        LevelUp (Fighter, 3<Level>, [
            Feats.forceAdd bravery
            Feats.addGeneralFeat
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Fighter, 4<Level>, [
            addFighterFeat
            Feats.addSkillFeat
        ])
        LevelUp (Fighter, 5<Level>, [
            Improve2.anyAbilityBoost 4
            Ancestry.addAncestryFeat
            Feats.forceAdd fighterWeaponMastery
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Fighter, 6<Level>, [
            addFighterFeat
            Feats.addSkillFeat
        ])
        LevelUp (Fighter, 7<Level>, [
            Feats.forceAdd battlefieldSurveyor
            Feats.addGeneralFeat
            Skills.increase Skills.regularSkills
            Feats.forceAdd Feats.weaponSpecialization
        ])
        LevelUp (Fighter, 8<Level>, [
            addFighterFeat
            Feats.addSkillFeat
        ])
        LevelUp (Fighter, 9<Level>, [
            Ancestry.addAncestryFeat
            Feats.forceAdd combatFlexibility
            Feats.forceAdd Feats.juggernaut
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Fighter, 10<Level>, [
            Improve2.anyAbilityBoost 4
            addFighterFeat
            Feats.addSkillFeat
        ])
    ]