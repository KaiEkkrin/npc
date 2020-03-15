namespace Npc.Classes

open Npc
open Npc.Attributes
open Npc.Classes.ClassBasics
open Npc.FeatReq

module Bard =

    let bardicLoreSkill = { Name = "Bardic Lore"; KeyAbility = Intelligence }
    let bardicLore = classFeatWith Bard 1 (FeatReq "Enigma") "Bardic Lore" 99 [
        Improve2.skill bardicLoreSkill Trained // TODO make it expert upon Legendary proficiency in Occultism
    ]
    let enigma = classFeatWith Bard 1 NoReq "Enigma" 97 [
        Feats.forceAdd bardicLore // TODO also add to spell repertoire
    ]
    let expertSpellcaster = classFeatWith Bard 7 NoReq "Expert Spellcaster" 98 [
        increaseClassSkill Bard
        increaseSpellSkill Occult
    ]
    let lingeringComposition = classFeatWith Bard 1 (FeatReq "Maestro") "Lingering Composition" 99 [
        Improve2.pool ("Focus", 1)
    ]
    let maestro = classFeatWith Bard 1 NoReq "Maestro" 97 [
            Feats.forceAdd lingeringComposition
        ]
    let signatureSpells = classFeat Bard 1 NoReq "Signature Spells" 98 []
    let versatilePerformance = classFeat Bard 1 (FeatReq "Polymath") "Versatile Performance" 100 []
    let polymath = classFeatWith Bard 1 NoReq "Polymath" 97 [
            Feats.forceAdd versatilePerformance
        ]

    let bardMuses = [enigma; maestro; polymath]

    let bardFeats = [
        bardicLore
        lingeringComposition
        classFeat Bard 1 NoReq "Reach Spell" 99 []
        versatilePerformance
        classFeat Bard 2 NoReq "Cantrip Expansion" 100 []
        classFeat Bard 2 (FeatReq "Polymath") "Esoteric Polymath" 100 []
        classFeat Bard 2 (FeatReq "Maestro") "Inspire Competence" 100 [] // TODO allow spontaneous casters to choose their exact spells on level up
        classFeatWith Bard 2 (FeatReq "Enigma") "Loremaster's Etude" 100 [
            Improve2.pool ("Focus", 1)
        ]
        classFeatWith Bard 2 NoReq "Multifarious Muse" 100 [ // TODO take this feat multiple times
            Improve2.feat "Muse" bardMuses 1 // TODO also add one feat out of the ones thus unlocked
        ]
        classFeat Bard 4 (FeatReq "Maestro") "Inspire Defense" 100 []
        classFeat Bard 4 NoReq "Melodious Spell" 101 []
        classFeat Bard 4 NoReq "Triple Time" 101 []
        classFeat Bard 4 (FeatReq "Polymath") "Versatile Signature" 101 []
        classFeat Bard 6 NoReq "Dirge of Doom" 101 []
        classFeat Bard 6 (FeatReq "Maestro") "Harmonize" 101 []
        classFeat Bard 6 NoReq "Steady Spellcasting" 101 []
        classFeat Bard 8 (FeatReq "Polymath" >&& SkillReq (Skills.occultism, Master)) "Eclectic Skill" 101 [] // TODO change untrained skill checks
        classFeatWith Bard 8 (FeatReq "Maestro") "Inspire Heroics" 102 [
            Improve2.pool ("Focus", 1)
        ]
        classFeat Bard 8 (FeatReq "Enigma") "Know-it-all" 102 []
        classFeat Bard 10 NoReq "House of Imaginary Walls" 102 []
        classFeat Bard 10 NoReq "Quickened Casting" 102 []
        classFeat Bard 10 (FeatReq "Polymath") "Unusual Composition" 102 []
    ]

    let addBardFeat = Improve2.feat "Bard feat" bardFeats 1

    let bard = [
        AddClass (Bard, [
            Improve2.feat "Ability boost" (spellcastingClassAbilityBoostFeats Bard Occult [Charisma]) 1
            Improve2.hitPointsPerLevel 8
            Improve2.skill Skills.perception Expert
            Improve2.skill Skills.fortitudeSave Trained
            Improve2.skill Skills.reflexSave Trained
            Improve2.skill Skills.willSave Expert
            Improve2.skill Skills.occultism Trained
            Improve2.skill Skills.performance Trained
            Improve2.skillsBasedOnInt 4 Skills.regularSkills
            Weapons.improveSkill (SimpleWeapon, Melee) Trained
            Weapons.improveSkill (SimpleWeapon, Ranged) Trained
            Weapons.improveSkill (Unarmed, Melee) Trained
            Improve2.skill (Char2.weaponSkill Weapons.longsword) Trained
            Improve2.skill (Char2.weaponSkill Weapons.rapier) Trained
            Improve2.skill (Char2.weaponSkill Weapons.sap) Trained
            Improve2.skill (Char2.weaponSkill Weapons.shortbow) Trained
            Improve2.skill (Char2.weaponSkill Weapons.whip) Trained
            Improve2.skill (Char2.armorSkill LightArmor) Trained
            Improve2.skill (Char2.armorSkill Unarmored) Trained
            Improve2.pool ("Focus", 1)
            Improve2.spell (0, 5)
            Improve2.spell (1, 2)
            Improve2.feat "Muse" bardMuses 1
        ])
        LevelUp (Bard, 2<Level>, [
            addBardFeat
            Feats.addSkillFeat
            Improve2.spell (1, 1)
        ])
        LevelUp (Bard, 3<Level>, [
            Feats.addGeneralFeat
            Feats.forceAdd Feats.lightningReflexes
            Feats.forceAdd signatureSpells
            Skills.increase Skills.regularSkills
            Improve2.spell (2, 2)
        ])
        LevelUp (Bard, 4<Level>, [
            addBardFeat
            Feats.addSkillFeat
            Improve2.spell (2, 1)
        ])
        LevelUp (Bard, 5<Level>, [
            Improve2.anyAbilityBoost 4
            Ancestry.addAncestryFeat
            Skills.increase Skills.regularSkills
            Improve2.spell (3, 2)
        ])
        LevelUp (Bard, 6<Level>, [
            addBardFeat
            Feats.addSkillFeat
            Improve2.spell (3, 1)
        ])
        LevelUp (Bard, 7<Level>, [
            Feats.forceAdd expertSpellcaster
            Feats.addGeneralFeat
            Skills.increase Skills.regularSkills
            Improve2.spell (4, 2)
        ])
        LevelUp (Bard, 8<Level>, [
            addBardFeat
            Feats.addSkillFeat
            Improve2.spell (4, 1)
        ])
        LevelUp (Bard, 9<Level>, [
            Ancestry.addAncestryFeat
            Feats.forceAdd Feats.greatFortitude
            Feats.forceAdd Feats.resolve
            Skills.increase Skills.regularSkills
            Improve2.spell (5, 2)
        ])
        LevelUp (Bard, 10<Level>, [
            Improve2.anyAbilityBoost 4
            addBardFeat
            Feats.addSkillFeat
            Improve2.spell (5, 1)
        ])
    ]
