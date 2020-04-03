namespace Npc.Classes

open Npc
open Npc.Attributes
open Npc.Classes.ClassBasics
open Npc.FeatReq

module Sorcerer =
    let bloodline name page tr imps =
        classFeatWith Sorcerer 1 NoReq (sprintf "%s Bloodline" name) page ([
            Improve2.feat "Ability boost" (spellcastingClassAbilityBoostFeats Sorcerer tr [Charisma]) 1
        ] @ imps)

    let bloodlines = [
        bloodline "Aberrant" 195 Occult [
            Improve2.skill Skills.intimidation Trained
            Improve2.skill Skills.occultism Trained
        ]
        bloodline "Angelic" 195 Divine [
            Improve2.skill Skills.diplomacy Trained
            Improve2.skill Skills.religion Trained
        ]
        bloodline "Demonic" 195 Divine [
            Improve2.skill Skills.intimidation Trained
            Improve2.skill Skills.religion Trained
        ]
        bloodline "Diabolic" 196 Divine [
            Improve2.skill Skills.deception Trained
            Improve2.skill Skills.religion Trained
        ]
        bloodline "Draconic" 196 Arcane [
            Improve2.skill Skills.arcana Trained
            Improve2.skill Skills.intimidation Trained
        ]
        bloodline "Elemental" 196 Primal [
            Improve2.skill Skills.intimidation Trained
            Improve2.skill Skills.nature Trained
        ]
        bloodline "Fey" 197 Primal [
            Improve2.skill Skills.deception Trained
            Improve2.skill Skills.nature Trained
        ]
        bloodline "Hag" 197 Occult [
            Improve2.skill Skills.deception Trained
            Improve2.skill Skills.occultism Trained
        ]
        bloodline "Imperial" 198 Arcane [
            Improve2.skill Skills.arcana Trained
            Improve2.skill Skills.society Trained
        ]
        bloodline "Undead" 198 Divine [
            Improve2.skill Skills.intimidation Trained
            Improve2.skill Skills.religion Trained
        ]
    ]

    // Increases any one Charisma-based spell skill, since bloodlines mean we could have any:
    let increaseAnySpellSkill prof =
        let choices =
            [Arcane; Divine; Occult; Primal]
            |> List.map (fun tr -> IncreaseSkill (Skills.spellSkill (tr, Charisma), prof))
        {
            Prompt = "Spell skill increase"
            Choices = choices
            Count = Some 1
        }

    let signatureSpells = classFeat Sorcerer 3 NoReq "Signature Spells" 193 [] // TODO include spell list for this
    let magicalFortitude = classFeatWith Sorcerer 5 NoReq "Magical Fortitude" 194 [
        Improve2.skill Skills.fortitudeSave Expert
    ]
    let expertSpellcaster = classFeatWith Sorcerer 7 NoReq "Expert Spellcaster" 194 [
        increaseClassSkill Sorcerer Expert
        increaseAnySpellSkill Expert
    ]

    let sorcererFeats = [
        classFeat Sorcerer 1 NoReq "Counterspell" 198 []
        classFeat Sorcerer 1 NoReq "Dangerous Sorcery" 198 []
        classFeat Sorcerer 1 NoReq "Familiar" 198 [] // TODO add familiars
        classFeat Sorcerer 1 NoReq "Reach Spell" 198 []
        classFeat Sorcerer 1 NoReq "Widen Spell" 198 []
        classFeat Sorcerer 2 NoReq "Cantrip Expansion" 198 [] // TODO spell repertoire
        classFeat Sorcerer 2 (FeatReq "Familiar") "Enhanced Familiar" 198 []
        classFeat Sorcerer 4 (SkillReq (Skills.spellSkill (Arcane, Charisma), Trained)) "Arcane Evolution" 199 []
        classFeat Sorcerer 4 NoReq "Bespell Weapon" 199 []
        classFeat Sorcerer 4 (SkillReq (Skills.spellSkill (Divine, Charisma), Trained)) "Divine Evolution" 199 []
        classFeat Sorcerer 4 (SkillReq (Skills.spellSkill (Occult, Charisma), Trained)) "Occult Evolution" 199 []
        classFeat Sorcerer 4 (SkillReq (Skills.spellSkill (Primal, Charisma), Trained)) "Primal Evolution" 199 []
        classFeatWith Sorcerer 6 NoReq "Advanced Bloodline" 200 [
            Improve2.pool ("Focus", 1)
        ]
        classFeat Sorcerer 6 NoReq "Steady Spellcasting" 200 []
        classFeat Sorcerer 8 NoReq "Bloodline Resistance" 200 []
        classFeat Sorcerer 8 NoReq "Crossblooded Evolution" 200 [] // TODO spell repertoire
        classFeatWith Sorcerer 10 NoReq "Greater Bloodline" 200 [
            Improve2.pool ("Focus", 1)
        ]
        classFeat Sorcerer 10 NoReq "Overwhelming Energy" 200 []
        classFeat Sorcerer 10 NoReq "Quickened Casting" 200 []
    ]

    let addSorcererFeat = Improve2.feat "Sorcerer feat" sorcererFeats 1

    let sorcerer = [
        AddClass (Sorcerer, [
            // Sorcerer spellcasting class is determined by bloodline
            Improve2.feat "Bloodline" bloodlines 1
            Improve2.hitPointsPerLevel 6
            Improve2.skill Skills.perception Trained
            Improve2.skill Skills.fortitudeSave Trained
            Improve2.skill Skills.reflexSave Trained
            Improve2.skill Skills.willSave Expert
            Improve2.skillsBasedOnInt 2 Skills.regularSkills
            Weapons.improveSkill (SimpleWeapon, Melee) Trained
            Weapons.improveSkill (SimpleWeapon, Ranged) Trained
            Weapons.improveSkill (Unarmed, Melee) Trained
            Improve2.skill (Char2.armorSkill Unarmored) Trained
            Improve2.spell (0, 5)
            Improve2.spell (1, 3)
        ])
        LevelUp (Sorcerer, 2<Level>, [
            Feats.addSkillFeat
            addSorcererFeat
            Improve2.spell (1, 1)
        ])
        LevelUp (Sorcerer, 3<Level>, [
            Feats.addGeneralFeat
            Feats.forceAdd signatureSpells
            Skills.increase Skills.regularSkills
            Improve2.spell (2, 3)
        ])
        LevelUp (Sorcerer, 4<Level>, [
            Feats.addSkillFeat
            addSorcererFeat
            Improve2.spell (2, 1)
        ])
        LevelUp (Sorcerer, 5<Level>, [
            Improve2.anyAbilityBoost 4
            Ancestry.addAncestryFeat
            Feats.forceAdd magicalFortitude
            Skills.increase Skills.regularSkills
            Improve2.spell (3, 3)
        ])
        LevelUp (Sorcerer, 6<Level>, [
            Feats.addSkillFeat
            addSorcererFeat
            Improve2.spell (3, 1)
        ])
        LevelUp (Sorcerer, 7<Level>, [
            Feats.forceAdd expertSpellcaster
            Feats.addGeneralFeat
            Skills.increase Skills.regularSkills
            Improve2.spell (4, 3)
        ])
        LevelUp (Sorcerer, 8<Level>, [
            Feats.addSkillFeat
            addSorcererFeat
            Improve2.spell (4, 1)
        ])
        LevelUp (Sorcerer, 9<Level>, [
            Ancestry.addAncestryFeat
            Feats.forceAdd Feats.lightningReflexes
            Skills.increase Skills.regularSkills
            Improve2.spell (5, 3)
        ])
        LevelUp (Sorcerer, 10<Level>, [
            Improve2.anyAbilityBoost 4
            Feats.addSkillFeat
            addSorcererFeat
            Improve2.spell (5, 1)
        ])
    ]