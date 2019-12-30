namespace Npc.Classes

open Npc
open Npc.Attributes
open Npc.Classes.ClassBasics
open Npc.FeatReq

module Druid =

    let druidSpellSkill = Skills.spellSkill (Primal, Wisdom)

    let animalCompanion = classFeat Druid 1 (FeatReq "Animal") "Animal Companion" 133 []
    let druidWeaponExpertise = classFeatWith Druid 11 NoReq "Druid Weapon Expertise" 133 [
        Weapons.improveSkill (SimpleWeapon, Melee) Expert
        Weapons.improveSkill (SimpleWeapon, Ranged) Expert
        Weapons.improveSkill (Unarmed, Melee) Expert
    ]
    let expertSpellcaster = classFeatWith Druid 7 NoReq "Expert Spellcaster" 98 [
        classSkill Druid Expert
        Improve2.skill druidSpellSkill Expert
    ]
    let leshyFamiliar = classFeat Druid 1 (FeatReq "Leaf") "Leshy Familiar" 133 []
    let resolve = classFeat Druid 11 NoReq "Resolve" 133 []
    let stormBorn = classFeat Druid 1 (FeatReq "Storm") "Storm Born" 134 []
    let wildShape = classFeat Druid 1 (FeatReq "Wild") "Wild Shape" 134 []
    let wildEmpathy = classFeat Druid 1 NoReq "Wild Empathy" 132 []

    let druidFeats = [
        animalCompanion
        leshyFamiliar
        classFeat Druid 1 NoReq "Reach Spell" 134 []
        stormBorn
        classFeat Druid 1 NoReq "Widen Spell" 134 []
        wildShape
        classFeat Druid 2 NoReq "Call of the Wild" 134 []
        classFeat Druid 2 NoReq "Enhanced Familiar" 134 [] // TODO requires a familiar
        classFeat Druid 2 NoReq "Order Explorer" 134 [] // TODO how to add an order feat without adding the order itself
        classFeat Druid 2 NoReq "Poison Resistance" 135 [] // TODO resistances
        classFeat Druid 4 (AbilityReq (Strength, 14<Score>) >&& FeatReq "Wild Shape") "Form Control" 135 []
        classFeat Druid 4 (FeatReq "Animal Companion") "Mature Animal Companion" 135 []
        classFeat Druid 4 (FeatReq "Order Explorer") "Order Magic" 135 [] // TODO multiple times, etc
        classFeat Druid 4 (FeatReq "Wild Shape") "Thousand Faces" 135 []
        classFeat Druid 4 (FeatReq "Leaf") "Woodland Stride" 135 []
        classFeat Druid 6 (FeatReq "Leaf") "Green Empathy" 136 []
        classFeat Druid 6 (FeatReq "Wild Shape") "Insect Shape" 136 []
        classFeat Druid 6 NoReq "Steady Spellcasting" 136 []
        classFeat Druid 6 (FeatReq "Storm") "Storm Retribution" 136 [] // TODO has tempest surge spell
        classFeat Druid 8 (FeatReq "Wild Shape") "Ferocious Shape" 136 []
        classFeat Druid 8 NoReq "Fey Caller" 136 []
        classFeat Druid 8 (FeatReq "Mature Animal Companion") "Incredible Companion" 137 []
        classFeat Druid 8 (FeatReq "Wild Shape") "Soaring Shape" 137 []
        classFeatWith Druid 8 (FeatReq "Storm") "Wind Caller" 137 [
            Improve2.pool ("Focus", 1)
        ]
        classFeat Druid 10 (FeatReq "Wild Shape") "Elemental Shape" 137 []
        classFeat Druid 10 NoReq "Healing Transformation" 137 []
        classFeat Druid 10 NoReq "Overwhelming Energy" 137 []
        classFeat Druid 10 (FeatReq "Leaf" >|| FeatReq "Wild Shape") "Plant Shape" 138 []
        classFeat Druid 10 (FeatReq "Animal Companion") "Side by Side" 138 []
        classFeat Druid 12 (FeatReq "Soaring Shape") "Dragon Shape" 138 []
        classFeat Druid 12 (FeatReq "Green Empathy") "Green Tongue" 138 []
        classFeat Druid 12 NoReq "Primal Focus" 138 []
        classFeat Druid 12 (FeatReq "Call of the Wild") "Primal Summons" 138 []
    ]

    let druidOrders = [
        classFeatWith Druid 1 NoReq "Animal" 131 [
            Improve2.skill Skills.athletics Trained
            Feats.forceAdd animalCompanion
            // TODO Heal animal spell
        ]
        classFeatWith Druid 1 NoReq "Leaf" 131 [
            Improve2.skill Skills.diplomacy Trained
            Feats.forceAdd leshyFamiliar
            Improve2.pool ("Focus", 1)
        ]
        classFeatWith Druid 1 NoReq "Storm" 132 [
            Improve2.skill Skills.acrobatics Trained
            Feats.forceAdd stormBorn
            Improve2.pool ("Focus", 1)
        ]
        classFeatWith Druid 1 NoReq "Wild" 132 [
            Improve2.skill Skills.intimidation Trained
            Feats.forceAdd wildShape
        ]
    ]

    let addDruidFeat = Improve2.feat "Druid feat" druidFeats 1

    let druid = AddClass (Druid, [
        1<Level>, [
            Improve2.feat "Ability boost" (classAbilityBoostFeats Druid [Wisdom]) 1
            Improve2.hitPointsPerLevel 8
            Improve2.skill Skills.perception Trained
            Improve2.skill Skills.fortitudeSave Trained
            Improve2.skill Skills.reflexSave Trained
            Improve2.skill Skills.willSave Expert
            Improve2.skill Skills.nature Trained
            Improve2.skillsBasedOnInt 2 Skills.regularSkills
            Weapons.improveSkill (SimpleWeapon, Melee) Trained
            Weapons.improveSkill (SimpleWeapon, Ranged) Trained
            Weapons.improveSkill (Unarmed, Melee) Trained
            Improve2.skill (Char2.armorSkill LightArmor) Trained
            Improve2.skill (Char2.armorSkill MediumArmor) Trained
            Improve2.skill (Char2.armorSkill Unarmored) Trained
            Improve2.spellSkill (Skills.spellSkill (Primal, Wisdom))
            Improve2.pool ("Focus", 1)
            Improve2.spell (0, 4)
            Improve2.spell (1, 2)
            Improve2.feat "Order" druidOrders 1
            Feats.forceAdd Feats.shieldBlock
            Feats.forceAdd wildEmpathy
        ]
        2<Level>, [
            addDruidFeat
            Feats.addSkillFeat
            Improve2.spell (1, 1)
        ]
        3<Level>, [
            Feats.forceAdd Feats.alertness
            Feats.addGeneralFeat
            Feats.forceAdd Feats.greatFortitude
            Skills.increase Skills.regularSkills
            Improve2.spell (2, 2)
        ]
        4<Level>, [
            addDruidFeat
            Feats.addSkillFeat
            Improve2.spell (2, 1)
        ]
        5<Level>, [
            Improve2.anyAbilityBoost 4
            Ancestry.addAncestryFeat
            Feats.forceAdd Feats.lightningReflexes
            Skills.increase Skills.regularSkills
            Improve2.spell (3, 2)
        ]
        6<Level>, [
            addDruidFeat
            Feats.addSkillFeat
            Improve2.spell (3, 1)
        ]
        7<Level>, [
            Feats.forceAdd expertSpellcaster
            Feats.addGeneralFeat
            Skills.increase Skills.regularSkills
            Improve2.spell (4, 2)
        ]
        8<Level>, [
            addDruidFeat
            Feats.addSkillFeat
            Improve2.spell (4, 1)
        ]
        9<Level>, [
            Ancestry.addAncestryFeat
            Skills.increase Skills.regularSkills
            Improve2.spell (5, 2)
        ]
        10<Level>, [
            Improve2.anyAbilityBoost 4
            addDruidFeat
            Feats.addSkillFeat
            Improve2.spell (5, 1)
        ]
        11<Level>, [
            Feats.forceAdd druidWeaponExpertise
            Feats.addGeneralFeat
            Feats.forceAdd resolve
            Skills.increase Skills.regularSkills
            Improve2.spell (6, 2)
        ]
        12<Level>, [
            addDruidFeat
            Feats.addSkillFeat
            Improve2.spell (6, 1)
        ]
        13<Level>, [
            Ancestry.addAncestryFeat
            Feats.forceAdd Feats.mediumArmorExpertise
            Skills.increase Skills.regularSkills
            Feats.forceAdd Feats.weaponSpecialization
            Improve2.spell (7, 2)
        ]
    ])