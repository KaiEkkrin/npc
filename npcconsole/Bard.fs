namespace NpcConsole.Classes

open NpcConsole
open NpcConsole.Attributes
open NpcConsole.Classes.ClassBasics

module Bard =

    let bardSpellSkill = Skills.spellSkill (Occult, Charisma)

    let bardicLoreSkill = { Name = "Bardic Lore"; KeyAbility = Intelligence }
    let bardicLore = classFeat Bard 1 [Improve.hasFeat "Enigma"] "Bardic Lore" 99 [
        Improve.skill bardicLoreSkill Trained // TODO make it expert upon Legendary proficiency in Occultism
    ]
    let enigma = classFeat Bard 1 [] "Enigma" 97 [
            Feats.forceAdd bardicLore // TODO also add to spell repertoire
        ]
    let expertSpellcaster = classFeat Bard 7 [] "Expert Spellcaster" 98 [
        classSkill Bard Expert
        Improve.skill bardSpellSkill Expert
    ]
    let lingeringComposition = classFeat Bard 1 [Improve.hasFeat "Maestro"] "Lingering Composition" 99 [
        Improve.pool ("Focus", 1)
    ]
    let maestro = classFeat Bard 1 [] "Maestro" 97 [
            Feats.forceAdd lingeringComposition
        ]
    let signatureSpells = classFeat Bard 1 [] "Signature Spells" 98 []
    let versatilePerformance = classFeat Bard 1 [Improve.hasFeat "Polymath"] "Versatile Performance" 100 []
    let polymath = classFeat Bard 1 [] "Polymath" 97 [
            Feats.forceAdd versatilePerformance
        ]

    let bardMuses = [enigma; maestro; polymath]

    let bardFeats = [
        bardicLore
        lingeringComposition
        classFeat Bard 1 [] "Reach Spell" 99 []
        versatilePerformance
        classFeat Bard 2 [] "Cantrip Expansion" 100 []
        classFeat Bard 2 [Improve.hasFeat "Polymath"] "Esoteric Polymath" 100 []
        classFeat Bard 2 [Improve.hasFeat "Maestro"] "Inspire Competence" 100 [] // TODO allow spontaneous casters to choose their exact spells on level up
        classFeat Bard 2 [Improve.hasFeat "Enigma"] "Loremaster's Etude" 100 [
            Improve.pool ("Focus", 1)
        ]
        classFeat Bard 2 [] "Multifarious Muse" 100 [ // TODO take this feat multiple times
            Improve.addFeats bardMuses 1 // TODO also add one feat out of the ones thus unlocked
        ]
        classFeat Bard 4 [Improve.hasFeat "Maestro"] "Inspire Defense" 100 []
        classFeat Bard 4 [] "Melodious Spell" 101 []
        classFeat Bard 4 [] "Triple Time" 101 []
        classFeat Bard 4 [Improve.hasFeat "Polymath"] "Versatile Signature" 101 []
        classFeat Bard 6 [] "Dirge of Doom" 101 []
        classFeat Bard 6 [Improve.hasFeat "Maestro"] "Harmonize" 101 []
        classFeat Bard 6 [] "Steady Spellcasting" 101 []
        classFeat Bard 8 [Improve.hasFeat "Polymath"; Improve.hasSkill Skills.occultism Master] "Eclectic Skill" 101 [] // TODO change untrained skill checks
        classFeat Bard 8 [Improve.hasFeat "Maestro"] "Inspire Heroics" 102 [
            Improve.pool ("Focus", 1)
        ]
        classFeat Bard 8 [Improve.hasFeat "Enigma"] "Know-it-all" 102 []
        classFeat Bard 10 [] "House of Imaginary Walls" 102 []
        classFeat Bard 10 [] "Quickened Casting" 102 []
        classFeat Bard 10 [Improve.hasFeat "Polymath"] "Unusual Composition" 102 []
    ]

    let bard c =
        match c.Level with 
        | 1<Level> ->
            { c with Class = Some Bard }, [
                Improve.addFeats (classAbilityBoostFeats Bard [Charisma]) 1
                Improve.hitPointsPerLevel 8
                Improve.skill Skills.perception Expert
                Improve.skill Skills.fortitudeSave Trained
                Improve.skill Skills.reflexSave Trained
                Improve.skill Skills.willSave Expert
                Improve.skill Skills.occultism Trained
                Improve.skill Skills.performance Trained
                Improve.skills Skills.regularSkills Trained ((modValue Intelligence c) + 4)
                Weapons.improveSkill (SimpleWeapon, Melee) Trained
                Weapons.improveSkill (SimpleWeapon, Ranged) Trained
                Weapons.improveSkill (Unarmed, Melee) Trained
                Improve.skill (Skills.weaponSkill Weapons.longsword) Trained
                Improve.skill (Skills.weaponSkill Weapons.rapier) Trained
                Improve.skill (Skills.weaponSkill Weapons.sap) Trained
                Improve.skill (Skills.weaponSkill Weapons.shortbow) Trained
                Improve.skill (Skills.weaponSkill Weapons.whip) Trained
                Improve.skill (Skills.armorSkill LightArmor) Trained
                Improve.skill (Skills.armorSkill Unarmored) Trained
                Improve.spellSkill (Skills.spellSkill (Occult, Charisma))
                Improve.pool ("Focus", 1)
                Improve.spell (0, 5)
                Improve.spell (1, 2)
                Improve.addFeats bardMuses 1
            ]
        | 2<Level> -> c, [
            Improve.addFeats bardFeats 1
            Improve.addFeats Feats.skillFeats 1
            Improve.spell (1, 1)
            ]
        | 3<Level> -> c, [
            Improve.addFeats Feats.generalFeats 1
            Feats.forceAdd Feats.lightningReflexes
            Feats.forceAdd signatureSpells
            Skills.increase 1
            Improve.spell (2, 2)
            ]
        | 4<Level> -> c, [
            Improve.addFeats bardFeats 1
            Improve.addFeats Feats.skillFeats 1
            Improve.spell (2, 1)
            ]
        | 5<Level> -> c, [
            Improve.anyAbility 4
            Improve.addFeats Ancestry.ancestryFeats 1
            Skills.increase 1
            Improve.spell (3, 2)
            ]
        | 6<Level> -> c, [
            Improve.addFeats bardFeats 1
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
            Improve.addFeats bardFeats 1
            Improve.addFeats Feats.skillFeats 1
            Improve.spell (4, 1)
            ]
        | 9<Level> -> c, [
            Improve.addFeats Ancestry.ancestryFeats 1
            Feats.forceAdd Feats.greatFortitude
            Feats.forceAdd Feats.resolve
            Skills.increase 1
            Improve.spell (5, 2)
            ]
        | 10<Level> -> c, [
            Improve.anyAbility 4
            Improve.addFeats bardFeats 1
            Improve.addFeats Feats.skillFeats 1
            Improve.spell (5, 1)
            ]
        | _ -> failwithf "Bad level: %d" c.Level
