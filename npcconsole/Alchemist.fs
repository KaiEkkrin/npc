namespace NpcConsole.Classes

open NpcConsole
open NpcConsole.Attributes
open NpcConsole.Classes.ClassBasics

module Alchemist =

    let alchemistResearchFields = [
        classFeat Alchemist 1 [] "Bomber" 73 []
        classFeat Alchemist 1 [] "Chirurgeon" 73 []
        classFeat Alchemist 1 [] "Mutagenist" 73 []
    ]

    let alchemicalAlacrity = classFeat Alchemist 15 [] "Alchemical Alacrity" 75 []
    let alchemicalExpertise = classFeat Alchemist 9 [] "Alchemical Expertise" 75 [
        classSkill Alchemist Expert
    ]
    let alchemicalMastery = classFeat Alchemist 17 [] "Alchemical Mastery" 76 [
        classSkill Alchemist Master
    ]
    let alchemicalWeaponExpertise = classFeat Alchemist 7 [] "Alchemical Weapon Expertise" 74 [
        Weapons.improveSkill (SimpleWeapon, Melee) Expert
        Weapons.improveSkill (SimpleWeapon, Ranged) Expert
        Improve.skill (Skills.weaponSkill Weapons.alchemicalBomb) Expert
    ]
    let doubleBrew = classFeat Alchemist 9 [] "Double Brew" 75 []
    let fieldDiscovery = classFeat Alchemist 5 [] "Field Discovery" 74 []
    let greaterFieldDiscovery = classFeat Alchemist 13 [] "Greater Field Discovery" 75 []
    let perpetualInfusions = classFeat Alchemist 7 [] "Perpetual Infusions" 74 []
    let perpetualPerfection = classFeat Alchemist 17 [] "Perpetual Perfection" 76 []
    let perpetualPotency = classFeat Alchemist 11 [] "Perpetual Potency" 75 []

    let alchemistFeats = [
        classFeat Alchemist 1 [] "Alchemical Familiar" 76 []
        classFeat Alchemist 1 [] "Alchemical Savant" 76 []
        classFeat Alchemist 1 [] "Far Lobber" 76 []
        classFeat Alchemist 1 [] "Quick Bomber" 76 []
        classFeat Alchemist 2 [] "Poison Resistance" 77 [] // TODO resistances
        classFeat Alchemist 2 [] "Revivifying Mutagen" 77 []
        classFeat Alchemist 2 [] "Smoke Bomb" 77 []
        classFeat Alchemist 4 [] "Calculated Splash" 77 []
        classFeat Alchemist 4 [] "Efficient Alchemy" 77 []
        classFeat Alchemist 4 [] "Enduring Alchemy" 78 []
        classFeat Alchemist 6 [] "Combine Elixirs" 78 []
        classFeat Alchemist 6 [] "Debilitating Bomb" 78 []
        classFeat Alchemist 6 [] "Directional Bombs" 78 []
        classFeat Alchemist 8 [] "Feral Mutagen" 79 []
        classFeat Alchemist 8 [] "Powerful Alchemy" 79 []
        classFeat Alchemist 8 [] "Sticky Bomb" 79 []
        classFeat Alchemist 10 [] "Elastic Mutagen" 79 []
        classFeat Alchemist 10 [Improve.hasFeat "Calculated Splash"] "Expanded Splash" 79 []
        classFeat Alchemist 10 [Improve.hasFeat "Debilitating Bomb"] "Greater Debilitating Bomb" 79 []
        classFeat Alchemist 10 [] "Merciful Elixir" 79 []
        classFeat Alchemist 10 [Improve.hasFeat "Powerful Alchemy"] "Potent Poisoner" 79 []
        classFeat Alchemist 12 [] "Extend Elixir" 79 []
        classFeat Alchemist 12 [] "Invincible Mutagen" 79 []
        classFeat Alchemist 12 [Improve.hasFeat "Far Lobber"] "Uncanny Bombs" 79 []
        classFeat Alchemist 14 [] "Glib Mutagen" 80 []
        classFeat Alchemist 14 [Improve.hasFeat "Merciful Elixir"] "Greater Merciful Elixir" 80 []
        classFeat Alchemist 14 [Improve.hasFeat "Greater Debilitating Bomb"] "True Debilitating Bomb" 80 []
        classFeat Alchemist 16 [Improve.hasFeat "Extend Elixir"] "Eternal Elixir" 80 []
        classFeat Alchemist 16 [] "Exploitive Bomb" 80 []
        classFeat Alchemist 16 [] "Genius Mutagen" 81 []
        classFeat Alchemist 16 [Improve.hasFeat "Extend Elixir"] "Persistent Mutagen" 81 []
        classFeat Alchemist 18 [] "Improbable Elixirs" 81 []
        classFeat Alchemist 18 [] "Mindblank Mutagen" 81 []
        classFeat Alchemist 18 [] "Miracle Worker" 81 []
        classFeat Alchemist 18 [] "Perfect Debilitation" 81 []
        classFeat Alchemist 20 [] "Craft Philosopher's Stone" 81 []
        classFeat Alchemist 20 [Improve.hasFeat "Expanded Splash"] "Mega Bomb" 81 []
        classFeat Alchemist 20 [] "Perfect Mutagen" 81 []
    ]

    let alchemist c =
        match c.Level with 
        | 1<Level> ->
            { c with Class = Some Alchemist }, [
                Improve.addFeats (classAbilityBoostFeats Alchemist [Intelligence]) 1
                Improve.hitPointsPerLevel 8
                Improve.skill Skills.perception Trained
                Improve.skill Skills.fortitudeSave Expert
                Improve.skill Skills.reflexSave Expert
                Improve.skill Skills.willSave Trained
                Improve.skill Skills.crafting Trained
                Improve.skills Skills.regularSkills Trained ((modValue Intelligence c) + 3)
                Weapons.improveSkill (SimpleWeapon, Melee) Trained
                Weapons.improveSkill (SimpleWeapon, Ranged) Trained
                Weapons.improveSkill (Unarmed, Melee) Trained
                Improve.skill (Skills.weaponSkill Weapons.alchemicalBomb) Trained
                Improve.skill (Skills.armorSkill LightArmor) Trained
                Improve.skill (Skills.armorSkill Unarmored) Trained
                Improve.pool ("Formulas", 6)
                Feats.forceAdd Feats.alchemicalCrafting
                Improve.addFeats alchemistResearchFields 1
                Improve.addFeats alchemistFeats 1
            ]
        | 2<Level> -> c, [
            Improve.pool ("Formulas", 2)
            Improve.addFeats alchemistFeats 1
            Improve.addFeats Feats.skillFeats 1
            ]
        | 3<Level> -> c, [
            Improve.pool ("Formulas", 2)
            Improve.addFeats Feats.generalFeats 1
            Skills.increase 1
            ]
        | 4<Level> -> c, [
            Improve.pool ("Formulas", 2)
            Improve.addFeats alchemistFeats 1
            Improve.addFeats Feats.skillFeats 1
            ]
        | 5<Level> -> c, [
            Improve.pool ("Formulas", 2)
            Improve.anyAbility 4
            Improve.addFeats Ancestry.ancestryFeats 1
            Feats.forceAdd fieldDiscovery
            Skills.increase 1
            ]
        | 6<Level> -> c, [
            Improve.pool ("Formulas", 2)
            Improve.addFeats alchemistFeats 1
            Improve.addFeats Feats.skillFeats 1
            ]
        | 7<Level> -> c, [
            Improve.pool ("Formulas", 2)
            Feats.forceAdd alchemicalWeaponExpertise
            Improve.addFeats Feats.generalFeats 1
            Feats.forceAdd Feats.ironWill
            Feats.forceAdd perpetualInfusions
            Skills.increase 1
            ]
        | 8<Level> -> c, [
            Improve.pool ("Formulas", 2)
            Improve.addFeats alchemistFeats 1
            Improve.addFeats Feats.skillFeats 1
            ]
        | 9<Level> -> c, [
            Improve.pool ("Formulas", 2)
            Feats.forceAdd alchemicalExpertise
            Feats.forceAdd Feats.alertness
            Improve.addFeats Ancestry.ancestryFeats 1
            Feats.forceAdd doubleBrew
            Skills.increase 1
            ]
        | 10<Level> -> c, [
            Improve.pool ("Formulas", 2)
            Improve.anyAbility 4
            Improve.addFeats alchemistFeats 1
            Improve.addFeats Feats.skillFeats 1
            ]
        | 11<Level> -> c, [
            Improve.pool ("Formulas", 2)
            Improve.addFeats Feats.generalFeats 1
            Feats.forceAdd Feats.juggernaut
            Feats.forceAdd perpetualPotency
            Skills.increase 1
            ]
        | 12<Level> -> c, [
            Improve.pool ("Formulas", 2)
            Improve.addFeats alchemistFeats 1
            Improve.addFeats Feats.skillFeats 1
            ]
        | 13<Level> -> c, [
            Improve.pool ("Formulas", 2)
            Improve.addFeats Ancestry.ancestryFeats 1
            Feats.forceAdd greaterFieldDiscovery
            Feats.forceAdd Feats.lightArmorExpertise
            Skills.increase 1
            Feats.forceAdd Feats.weaponSpecialization
            ]
        | 14<Level> -> c, [
            Improve.pool ("Formulas", 2)
            Improve.addFeats alchemistFeats 1
            Improve.addFeats Feats.skillFeats 1
            ]
        | 15<Level> -> c, [
            Improve.pool ("Formulas", 2)
            Improve.anyAbility 4
            Feats.forceAdd alchemicalAlacrity
            Feats.forceAdd Feats.evasion
            Improve.addFeats Feats.generalFeats 1
            Skills.increase 1
            ]
        | 16<Level> -> c, [
            Improve.pool ("Formulas", 2)
            Improve.addFeats alchemistFeats 1
            Improve.addFeats Feats.skillFeats 1
            ]
        | 17<Level> -> c, [
            Improve.pool ("Formulas", 2)
            Feats.forceAdd alchemicalMastery
            Improve.addFeats Ancestry.ancestryFeats 1
            Feats.forceAdd perpetualPerfection
            Skills.increase 1
            ]
        | 18<Level> -> c, [
            Improve.pool ("Formulas", 2)
            Improve.addFeats alchemistFeats 1
            Improve.addFeats Feats.skillFeats 1
            ]
        | 19<Level> -> c, [
            Improve.pool ("Formulas", 2)
            Improve.addFeats Feats.generalFeats 1
            Feats.forceAdd Feats.lightArmorMastery
            Skills.increase 1
            ]
        | 20<Level> -> c, [
            Improve.pool ("Formulas", 2)
            Improve.anyAbility 4
            Improve.addFeats alchemistFeats 1
            Improve.addFeats Feats.skillFeats 1
            ]
        | _ -> failwithf "Bad level: %d" c.Level
