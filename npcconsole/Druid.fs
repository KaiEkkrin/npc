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