namespace Npc.Classes

open Npc
open Npc.Attributes
open Npc.Classes.ClassBasics
open Npc.FeatReq

module Wizard =
    let metamagicWizardFeats = [
        classFeat Wizard 1 NoReq "Reach Spell" 210 []
        classFeat Wizard 1 NoReq "Widen Spell" 210 []
        classFeat Wizard 2 NoReq "Conceal Spell" 210 []
        classFeat Wizard 4 (FeatReq "Conceal Spell") "Silent Spell" 210 []
        classFeat Wizard 8 NoReq "Bond Conservation" 211 []
        classFeat Wizard 10 NoReq "Overwhelming Energy" 211 []
        classFeat Wizard 10 NoReq "Quickened Casting" 211 []
    ]

    let wizardFeats = metamagicWizardFeats @ [
        classFeat Wizard 1 NoReq "Counterspell" 209 []
        classFeat Wizard 1 NoReq "Eschew Materials" 209 []
        classFeat Wizard 1 NoReq "Familiar" 209 [] // TODO familiar stats
        classFeatWith Wizard 1 (FeatReq "Universal Wizard") "Hand of the Apprentice" 209 [
            Improve2.pool ("Focus", 1)
        ]
        classFeatWith Wizard 2 NoReq "Cantrip Expansion" 209 [
            Improve2.spell (0, 2)
        ]
        classFeat Wizard 2 (FeatReq "Familiar") "Enhanced Familiar" 210 []
        classFeat Wizard 4 NoReq "Bespell Weapon" 210 []
        classFeat Wizard 4 NoReq "Linked Focus" 210 []
        classFeat Wizard 6 NoReq "Spell Penetration" 210 []
        classFeat Wizard 6 NoReq "Steady Spellcasting" 211 []
        classFeatWith Wizard 8 NoReq "Advanced School Spell" 211 [
            Improve2.pool ("Focus", 1)
        ]
        classFeatWith Wizard 8 (FeatReq "Hand of the Apprentice") "Universal Versatility" 211 [
            Improve2.pool ("Focus", 1)
        ]
        classFeat Wizard 10 (SkillReq (Skills.crafting, Expert)) "Scroll Savant" 212 []
    ]

    let addMetamagicWizardFeat = Improve2.feat "Metamagic wizard feat" metamagicWizardFeats 1
    let addWizardFeat = Improve2.feat "Wizard feat" wizardFeats 1

    let arcaneSchools = [
        classFeatWith Wizard 1 NoReq "Abjuration Specialization" 207 [
            Improve2.pool ("Focus", 1)
        ]
        classFeatWith Wizard 1 NoReq "Conjuration Specialization" 207 [
            Improve2.pool ("Focus", 1)
        ]
        classFeatWith Wizard 1 NoReq "Divination Specialization" 208 [
            Improve2.pool ("Focus", 1)
        ]
        classFeatWith Wizard 1 NoReq "Enchantment Specialization" 208 [
            Improve2.pool ("Focus", 1)
        ]
        classFeatWith Wizard 1 NoReq "Evocation Specialization" 208 [
            Improve2.pool ("Focus", 1)
        ]
        classFeatWith Wizard 1 NoReq "Illusion Specialization" 208 [
            Improve2.pool ("Focus", 1)
        ]
        classFeatWith Wizard 1 NoReq "Necromancy Specialization" 208 [
            Improve2.pool ("Focus", 1)
        ]
        classFeatWith Wizard 1 NoReq "Transmutation Specialization" 208 [
            Improve2.pool ("Focus", 1)
        ]
        classFeatWith Wizard 1 NoReq "Universal Wizard" 209 [
            Improve2.pool ("Drain Bonded Item", 1)
            addWizardFeat
        ]
    ]

    // "Drain Bonded Item" improves with spellcasting level
    let improvedDrainBondedItem lv = {
        Prompt = sprintf "Drain Bonded Item %d" lv
        Choices = [
            classFeatWith Wizard 1 (FeatReq "Universal Wizard") (sprintf "Drain Bonded Item %d" lv) 209 [
                Improve2.pool ("Drain Bonded Item", 1)
            ]
        ]
        Count = None
    }

    let arcaneTheses = [
        classFeat Wizard 1 NoReq "Improved Familiar Attunement" 205 [] // TODO familiar stats
        classFeatWith Wizard 1 NoReq "Metamagical Experimentation" 205 [
            addMetamagicWizardFeat
        ]
        classFeat Wizard 1 NoReq "Spell Blending" 206 []
        classFeat Wizard 1 NoReq "Spell Substitution" 206 []
    ]

    let expertSpellcaster = classFeatWith Wizard 7 NoReq "Expert Spellcaster" 207 [
        increaseClassSkill Wizard Expert
        increaseSpellSkill Arcane Expert
    ]

    let magicalFortitude = classFeatWith Wizard 9 NoReq "Magical Fortitude" 207 [
        Improve2.skill Skills.fortitudeSave Expert
    ]

    let wizard = [
        AddClass (Wizard, [
            Improve2.feat "Ability boost" (spellcastingClassAbilityBoostFeats Wizard Arcane [Intelligence]) 1
            Improve2.hitPointsPerLevel 6
            Improve2.skill Skills.perception Trained
            Improve2.skill Skills.fortitudeSave Trained
            Improve2.skill Skills.reflexSave Trained
            Improve2.skill Skills.willSave Expert
            Improve2.skill Skills.arcana Trained
            Improve2.skillsBasedOnInt 2 Skills.regularSkills
            Improve2.skill (Char2.weaponSkill Weapons.club) Trained
            Improve2.skill (Char2.weaponSkill Weapons.crossbow) Trained
            Improve2.skill (Char2.weaponSkill Weapons.dagger) Trained
            Improve2.skill (Char2.weaponSkill Weapons.heavyCrossbow) Trained
            Improve2.skill (Char2.weaponSkill Weapons.staff) Trained
            Weapons.improveSkill (Unarmed, Melee) Trained
            Improve2.skill (Char2.armorSkill Unarmored) Trained
            Improve2.feat "Arcane School" arcaneSchools 1
            Improve2.feat "Arcane Thesis" arcaneTheses 1
            addWizardFeat
            Improve2.spell (0, 5)
            Improve2.spell (1, 2)
        ])
        LevelUp (Wizard, 2<Level>, [
            Feats.addSkillFeat
            addWizardFeat
            Improve2.spell (1, 1)
        ])
        LevelUp (Wizard, 3<Level>, [
            Feats.addGeneralFeat
            Skills.increase Skills.regularSkills
            Improve2.spell (2, 2)
            improvedDrainBondedItem 2
        ])
        LevelUp (Wizard, 4<Level>, [
            Feats.addSkillFeat
            addWizardFeat
            Improve2.spell (2, 1)
        ])
        LevelUp (Wizard, 5<Level>, [
            Improve2.anyAbilityBoost 4
            Ancestry.addAncestryFeat
            Feats.forceAdd Feats.lightningReflexes
            Skills.increase Skills.regularSkills
            Improve2.spell (3, 2)
            improvedDrainBondedItem 3
        ])
        LevelUp (Wizard, 6<Level>, [
            Feats.addSkillFeat
            addWizardFeat
            Improve2.spell (3, 1)
        ])
        LevelUp (Wizard, 7<Level>, [
            Feats.forceAdd expertSpellcaster
            Feats.addGeneralFeat
            Skills.increase Skills.regularSkills
            Improve2.spell (4, 2)
            improvedDrainBondedItem 4
        ])
        LevelUp (Wizard, 8<Level>, [
            Feats.addSkillFeat
            addWizardFeat
            Improve2.spell (4, 1)
        ])
        LevelUp (Wizard, 9<Level>, [
            Ancestry.addAncestryFeat
            Feats.forceAdd magicalFortitude
            Skills.increase Skills.regularSkills
            Improve2.spell (5, 2)
            improvedDrainBondedItem 5
        ])
        LevelUp (Wizard, 10<Level>, [
            Improve2.anyAbilityBoost 4
            Feats.addSkillFeat
            addWizardFeat
            Improve2.spell (5, 1)
        ])
    ]