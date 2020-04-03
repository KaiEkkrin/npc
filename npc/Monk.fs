namespace Npc

open Npc
open Npc.Attributes
open Npc.Classes.ClassBasics
open Npc.FeatReq

module Monk =
    let flurryOfBlows = classFeat Monk 1 NoReq "Flurry of Blows" 156 []

    // TODO Make this increase unarmed damage from 1d4 to 1d6.
    // Show unarmed attack on all character sheets, as a separate thing from the
    // chosen weapon.
    let powerfulFist = classFeat Monk 1 NoReq "Powerful Fist" 156 []
    let incredibleMovement10 = classFeat Monk 3 NoReq "Incredible Movement (+10 Feet)" 156 [
        IncreaseSpeed 10<Feet>
    ]
    let mysticStrikes = classFeat Monk 3 NoReq "Mystic Strikes" 156 []
    let expertStrikes = classFeatWith Monk 5 NoReq "Expert Strikes" 157 [
        Weapons.improveSkill (SimpleWeapon, Melee) Expert
        Weapons.improveSkill (SimpleWeapon, Ranged) Expert
        Weapons.improveSkill (Unarmed, Melee) Expert
    ]
    let incredibleMovement15 = classFeat Monk 7 (FeatReq "Incredible Movement (+10 Feet)") "Incredible Movement (+15 Feet)" 156 [
        IncreaseSpeed 5<Feet>
    ]

    // TODO Path to Perfection also adds successes count as critical successes with the chosen skill
    let pathToPerfection = classFeatWith Monk 7 NoReq "Path to Perfection" 157 [{
        Prompt = "Path to Perfection"
        Choices = [Skills.fortitudeSave; Skills.reflexSave; Skills.willSave] |> List.map (fun sk -> IncreaseSkill (sk, Master))
        Count = Some 1
    }]
    let metalStrikes = classFeat Monk 9 NoReq "Metal Strikes" 157 []
    let monkExpertise = classFeatWith Monk 9 NoReq "Monk Expertise" 157 [
        increaseClassSkill Monk Expert
        // TODO Ki expertise thingy if applicable
    ]

    let monkFeats = [
        classFeat Monk 1 NoReq "Crane Stance" 158 [] // TODO unarmored only; add separate unarmed attack and armor character sheet sub-headings in the stance?
        classFeat Monk 1 NoReq "Dragon Stance" 158 [] // TODO ditto
        classFeatWith Monk 1 NoReq "Ki Rush" 158 [
            Improve2.pool ("Focus", 1)
        ]
        classFeatWith Monk 1 NoReq "Ki Strike" 158 [
            Improve2.pool ("Focus", 1)
        ]
        classFeatWith Monk 1 NoReq "Monastic Weaponry" 158 [
            // TODO Specifically monk weapons; when Unarmed increases to Expert, these increase
            // to Expert as well
            Weapons.improveSkill (MartialWeapon, Melee) Trained
            Weapons.improveSkill (MartialWeapon, Ranged) Trained
        ]
        classFeat Monk 1 NoReq "Mountain Stance" 159 []
        classFeat Monk 1 NoReq "Tiger Stance" 159 []
        classFeat Monk 1 NoReq "Wolf Stance" 159 []
        classFeat Monk 2 NoReq "Brawling Focus" 160 [] // TODO add critical specialization effects
        classFeat Monk 2 NoReq "Crushing Grab" 160 []
        classFeat Monk 2 NoReq "Dancing Leaf" 160 []
        classFeat Monk 2 (FeatReq "Ki Strike") "Elemental Fist" 160 []
        classFeat Monk 2 (FeatReq "Flurry of Blows") "Stunning Fist" 160 []
        classFeat Monk 4 NoReq "Deflect Arrow" 160 []
        classFeat Monk 4 (SkillReq (Skills.athletics, Expert)) "Flurry of Maneuvers" 160 []
        classFeat Monk 4 NoReq "Flying Kick" 160 []
        classFeat Monk 4 NoReq "Guarded Movement" 160 []
        classFeat Monk 4 NoReq "Stand Still" 160 []
        classFeatWith Monk 4 (PoolReq ("Focus", 1)) "Wholeness of Body" 160 [ // detecting a focus pool is good enough for detecting Ki for now :)
            Improve2.pool ("Focus", 1)
        ]
        classFeatWith Monk 6 (FeatReq "Incredible Movement (+10 Feet)" >&& PoolReq ("Focus", 1)) "Abundant Step" 160 [
            Improve2.pool ("Focus", 1)
        ]
        classFeat Monk 6 (FeatReq "Crane Stance") "Crane Flutter" 161 []
        classFeat Monk 6 (FeatReq "Dragon Stance") "Dragon Roar" 161 []
        classFeatWith Monk 6 (PoolReq ("Focus", 1)) "Ki Blast" 161 [
            Improve2.pool ("Focus", 1)
        ]
        classFeat Monk 6 (FeatReq "Mountain Stance") "Mountain Stronghold" 161 []
        classFeat Monk 6 (FeatReq "Tiger Stance") "Tiger Slash" 161 []
        classFeat Monk 6 NoReq "Water Step" 162 []
        classFeat Monk 6 NoReq "Whirling Throw" 162 []
        classFeat Monk 6 (FeatReq "Wolf Stance") "Wolf Drag" 162 []
        classFeat Monk 8 (FeatReq "Deflect Arrow") "Arrow Snatching" 162 []
        classFeat Monk 8 NoReq "Ironblood Stance" 163 [] // TODO require unarmoured
        classFeat Monk 8 (SkillReq (Skills.athletics, Master)) "Mixed Maneuver" 163 []
        classFeat Monk 8 NoReq "Tangled Forest Stance" 163 [] // TODO require unarmoured
        classFeat Monk 8 NoReq "Wall Run" 163 []
        classFeatWith Monk 8 (PoolReq ("Focus", 1)) "Wild Winds Initiate" 163 [
            Improve2.pool ("Focus", 1)
        ]
        classFeat Monk 10 NoReq "Knockback Strike" 163 []
        classFeat Monk 10 NoReq "Sleeper Hold" 163 []
        classFeatWith Monk 10 (PoolReq ("Focus", 1)) "Wind Jump" 163 [
            Improve2.pool ("Focus", 1)
        ]
        classFeat Monk 10 NoReq "Winding Flow" 163 []
    ]

    let addMonkFeat = Improve2.feat "Monk feat" monkFeats 1

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
            Feats.forceAdd flurryOfBlows
            addMonkFeat
            Feats.forceAdd powerfulFist
        ])
        LevelUp (Monk, 2<Level>, [
            addMonkFeat
            Feats.addSkillFeat
        ])
        LevelUp (Monk, 3<Level>, [
            Feats.addGeneralFeat
            Feats.forceAdd incredibleMovement10
            Feats.forceAdd mysticStrikes
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Monk, 4<Level>, [
            addMonkFeat
            Feats.addSkillFeat
        ])
        LevelUp (Monk, 5<Level>, [
            Improve2.anyAbilityBoost 4
            Feats.forceAdd Feats.alertness
            Ancestry.addAncestryFeat
            Feats.forceAdd expertStrikes
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Monk, 6<Level>, [
            addMonkFeat
            Feats.addSkillFeat
        ])
        LevelUp (Monk, 7<Level>, [
            Feats.addGeneralFeat
            Feats.forceAdd incredibleMovement15
            Feats.forceAdd pathToPerfection
            Skills.increase Skills.regularSkills
            Feats.forceAdd Feats.weaponSpecialization
        ])
        LevelUp (Monk, 8<Level>, [
            addMonkFeat
            Feats.addSkillFeat
        ])
        LevelUp (Monk, 9<Level>, [
            Ancestry.addAncestryFeat
            Feats.forceAdd metalStrikes
            Feats.forceAdd monkExpertise
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Monk, 10<Level>, [
            Improve2.anyAbilityBoost 4
            addMonkFeat
            Feats.addSkillFeat
        ])
    ]