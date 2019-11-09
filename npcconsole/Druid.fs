namespace NpcConsole.Classes

open NpcConsole
open NpcConsole.Attributes
open NpcConsole.Classes.ClassBasics

module Druid =

    let druidSpellSkill = Skills.spellSkill (Primal, Wisdom)

    let animalCompanion = classFeat Druid 1 [Improve.hasFeat "Animal"] "Animal Companion" 133 []
    let expertSpellcaster = classFeat Druid 7 [] "Expert Spellcaster" 98 [
        classSkill Druid Expert
        Improve.skill druidSpellSkill Expert
    ]
    let leshyFamiliar = classFeat Druid 1 [Improve.hasFeat "Leaf"] "Leshy Familiar" 133 []
    let stormBorn = classFeat Druid 1 [Improve.hasFeat "Storm"] "Storm Born" 134 []
    let wildShape = classFeat Druid 1 [Improve.hasFeat "Wild"] "Wild Shape" 134 []
    let wildEmpathy = classFeat Druid 1 [] "Wild Empathy" 132 []

    let druidFeats = [
        animalCompanion
        leshyFamiliar
        classFeat Druid 1 [] "Reach Spell" 134 []
        stormBorn
        classFeat Druid 1 [] "Widen Spell" 134 []
        wildShape
        classFeat Druid 2 [] "Call of the Wild" 134 []
        classFeat Druid 2 [] "Enhanced Familiar" 134 [] // TODO requires a familiar
        classFeat Druid 2 [] "Order Explorer" 134 [] // TODO how to add an order feat without adding the order itself
        classFeat Druid 2 [] "Poison Resistance" 135 [] // TODO resistances
        classFeat Druid 4 [Improve.hasAbilityScore Strength 14<Score>; Improve.hasFeat "Wild Shape"] "Form Control" 135 []
        classFeat Druid 4 [Improve.hasFeat "Animal Companion"] "Mature Animal Companion" 135 []
        classFeat Druid 4 [Improve.hasFeat "Order Explorer"] "Order Magic" 135 [] // TODO multiple times, etc
        classFeat Druid 4 [Improve.hasFeat "Wild Shape"] "Thousand Faces" 135 []
        classFeat Druid 4 [Improve.hasFeat "Leaf"] "Woodland Stride" 135 []
        classFeat Druid 6 [Improve.hasFeat "Leaf"] "Green Empathy" 136 []
        classFeat Druid 6 [Improve.hasFeat "Wild Shape"] "Insect Shape" 136 []
        classFeat Druid 6 [] "Steady Spellcasting" 136 []
        classFeat Druid 6 [Improve.hasFeat "Storm"] "Storm Retribution" 136 [] // TODO has tempest surge spell
        classFeat Druid 8 [Improve.hasFeat "Wild Shape"] "Ferocious Shape" 136 []
        classFeat Druid 8 [] "Fey Caller" 136 []
        classFeat Druid 8 [Improve.hasFeat "Mature Animal Companion"] "Incredible Companion" 137 []
        classFeat Druid 8 [Improve.hasFeat "Wild Shape"] "Soaring Shape" 137 []
        classFeat Druid 8 [Improve.hasFeat "Storm"] "Wind Caller" 137 [
            Improve.pool ("Focus", 1)
        ]
        classFeat Druid 10 [Improve.hasFeat "Wild Shape"] "Elemental Shape" 137 []
        classFeat Druid 10 [] "Healing Transformation" 137 []
        classFeat Druid 10 [] "Overwhelming Energy" 137 []
        classFeat Druid 10 [Improve.hasOneFeatOf ["Leaf"; "Wild Shape"]] "Plant Shape" 138 []
        classFeat Druid 10 [Improve.hasFeat "Animal Companion"] "Side by Side" 138 []
    ]

    let druidOrders = [
        classFeat Druid 1 [] "Animal" 131 [
            Improve.skill Skills.athletics Trained
            Feats.forceAdd animalCompanion
            // TODO Heal animal spell
        ]
        classFeat Druid 1 [] "Leaf" 131 [
            Improve.skill Skills.diplomacy Trained
            Feats.forceAdd leshyFamiliar
            Improve.pool ("Focus", 1)
        ]
        classFeat Druid 1 [] "Storm" 132 [
            Improve.skill Skills.acrobatics Trained
            Feats.forceAdd stormBorn
            Improve.pool ("Focus", 1)
        ]
        classFeat Druid 1 [] "Wild" 132 [
            Improve.skill Skills.intimidation Trained
            Feats.forceAdd wildShape
        ]
    ]

    let druid c =
        match c.Level with 
        | 1<Level> ->
            { c with Class = Some Druid }, [
                Improve.addFeats (classAbilityBoostFeats Druid [Wisdom]) 1
                Improve.hitPointsPerLevel 8
                Improve.skill Skills.perception Trained
                Improve.skill Skills.fortitudeSave Trained
                Improve.skill Skills.reflexSave Trained
                Improve.skill Skills.willSave Expert
                Improve.skill Skills.nature Trained
                Improve.skills Skills.regularSkills Trained ((modValue Intelligence c) + 2)
                Weapons.improveSkill (SimpleWeapon, Melee) Trained
                Weapons.improveSkill (SimpleWeapon, Ranged) Trained
                Weapons.improveSkill (Unarmed, Melee) Trained
                Improve.skill (Skills.armorSkill LightArmor) Trained
                Improve.skill (Skills.armorSkill MediumArmor) Trained
                Improve.skill (Skills.armorSkill Unarmored) Trained
                Improve.spellSkill (Skills.spellSkill (Primal, Wisdom))
                Improve.pool ("Focus", 1)
                Improve.spell (0, 4)
                Improve.spell (1, 2)
                Improve.addFeats druidOrders 1
                Feats.forceAdd Feats.shieldBlock
                Feats.forceAdd wildEmpathy
            ]
        | 2<Level> -> c, [
            Improve.addFeats druidFeats 1
            Improve.addFeats Feats.skillFeats 1
            Improve.spell (1, 1)
            ]
        | 3<Level> -> c, [
            Feats.forceAdd Feats.alertness
            Improve.addFeats Feats.generalFeats 1
            Feats.forceAdd Feats.greatFortitude
            Skills.increase 1
            Improve.spell (2, 2)
            ]
        | 4<Level> -> c, [
            Improve.addFeats druidFeats 1
            Improve.addFeats Feats.skillFeats 1
            Improve.spell (2, 1)
            ]
        | 5<Level> -> c, [
            Improve.anyAbility 4
            Improve.addFeats Ancestry.ancestryFeats 1
            Feats.forceAdd Feats.lightningReflexes
            Skills.increase 1
            Improve.spell (3, 2)
            ]
        | 6<Level> -> c, [
            Improve.addFeats druidFeats 1
            Improve.addFeats Feats.skillFeats 1
            Improve.spell (3, 1)
            ]
        | 7<Level> -> c, [
            Feats.forceAdd expertSpellcaster
            Improve.addFeats Feats.generalFeats 1
            Skills.increase 1
            Improve.spell (4, 2)
            ]
        | 8<Level> -> c, [
            Improve.addFeats druidFeats 1
            Improve.addFeats Feats.skillFeats 1
            Improve.spell (4, 1)
            ]
        | 9<Level> -> c, [
            Improve.addFeats Ancestry.ancestryFeats 1
            Skills.increase 1
            Improve.spell (5, 2)
            ]
        | 10<Level> -> c, [
            Improve.anyAbility 4
            Improve.addFeats druidFeats 1
            Improve.addFeats Feats.skillFeats 1
            Improve.spell (5, 1)
            ]
        | _ -> failwithf "Bad level: %d" c.Level