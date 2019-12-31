namespace Npc.Classes

open Npc
open Npc.Attributes
open Npc.Classes.ClassBasics

module Alchemist =

    let alchemistResearchFields = [
        classFeat Alchemist 1 NoReq "Bomber" 73 []
        classFeat Alchemist 1 NoReq "Chirurgeon" 73 []
        classFeat Alchemist 1 NoReq "Mutagenist" 73 []
    ]

    let alchemicalAlacrity = classFeat Alchemist 15 NoReq "Alchemical Alacrity" 75 []
    let alchemicalExpertise = classFeatWith Alchemist 9 NoReq "Alchemical Expertise" 75 [
        classSkill Alchemist Expert
    ]
    let alchemicalMastery = classFeatWith Alchemist 17 NoReq "Alchemical Mastery" 76 [
        classSkill Alchemist Master
    ]
    let alchemicalWeaponExpertise = classFeatWith Alchemist 7 NoReq "Alchemical Weapon Expertise" 74 [
        Weapons.improveSkill (SimpleWeapon, Melee) Expert
        Weapons.improveSkill (SimpleWeapon, Ranged) Expert
        Improve2.skill (Char2.weaponSkill Weapons.alchemicalBomb) Expert
    ]
    let doubleBrew = classFeat Alchemist 9 NoReq "Double Brew" 75 []
    let fieldDiscovery = classFeat Alchemist 5 NoReq "Field Discovery" 74 []
    let greaterFieldDiscovery = classFeat Alchemist 13 NoReq "Greater Field Discovery" 75 []
    let perpetualInfusions = classFeat Alchemist 7 NoReq "Perpetual Infusions" 74 []
    let perpetualPerfection = classFeat Alchemist 17 NoReq "Perpetual Perfection" 76 []
    let perpetualPotency = classFeat Alchemist 11 NoReq "Perpetual Potency" 75 []

    let alchemistFeats = [
        classFeat Alchemist 1 NoReq "Alchemical Familiar" 76 []
        classFeat Alchemist 1 NoReq "Alchemical Savant" 76 []
        classFeat Alchemist 1 NoReq "Far Lobber" 76 []
        classFeat Alchemist 1 NoReq "Quick Bomber" 76 []
        classFeat Alchemist 2 NoReq "Poison Resistance" 77 [] // TODO resistances
        classFeat Alchemist 2 NoReq "Revivifying Mutagen" 77 []
        classFeat Alchemist 2 NoReq "Smoke Bomb" 77 []
        classFeat Alchemist 4 NoReq "Calculated Splash" 77 []
        classFeat Alchemist 4 NoReq "Efficient Alchemy" 77 []
        classFeat Alchemist 4 NoReq "Enduring Alchemy" 78 []
        classFeat Alchemist 6 NoReq "Combine Elixirs" 78 []
        classFeat Alchemist 6 NoReq "Debilitating Bomb" 78 []
        classFeat Alchemist 6 NoReq "Directional Bombs" 78 []
        classFeat Alchemist 8 NoReq "Feral Mutagen" 79 []
        classFeat Alchemist 8 NoReq "Powerful Alchemy" 79 []
        classFeat Alchemist 8 NoReq "Sticky Bomb" 79 []
        classFeat Alchemist 10 NoReq "Elastic Mutagen" 79 []
        classFeat Alchemist 10 (FeatReq "Calculated Splash") "Expanded Splash" 79 []
        classFeat Alchemist 10 (FeatReq "Debilitating Bomb") "Greater Debilitating Bomb" 79 []
        classFeat Alchemist 10 NoReq "Merciful Elixir" 79 []
        classFeat Alchemist 10 (FeatReq "Powerful Alchemy") "Potent Poisoner" 79 []
        classFeat Alchemist 12 NoReq "Extend Elixir" 79 []
        classFeat Alchemist 12 NoReq "Invincible Mutagen" 79 []
        classFeat Alchemist 12 (FeatReq "Far Lobber") "Uncanny Bombs" 79 []
        classFeat Alchemist 14 NoReq "Glib Mutagen" 80 []
        classFeat Alchemist 14 (FeatReq "Merciful Elixir") "Greater Merciful Elixir" 80 []
        classFeat Alchemist 14 (FeatReq "Greater Debilitating Bomb") "True Debilitating Bomb" 80 []
        classFeat Alchemist 16 (FeatReq "Extend Elixir") "Eternal Elixir" 80 []
        classFeat Alchemist 16 NoReq "Exploitive Bomb" 80 []
        classFeat Alchemist 16 NoReq "Genius Mutagen" 81 []
        classFeat Alchemist 16 (FeatReq "Extend Elixir") "Persistent Mutagen" 81 []
        classFeat Alchemist 18 NoReq "Improbable Elixirs" 81 []
        classFeat Alchemist 18 NoReq "Mindblank Mutagen" 81 []
        classFeat Alchemist 18 NoReq "Miracle Worker" 81 []
        classFeat Alchemist 18 NoReq "Perfect Debilitation" 81 []
        classFeat Alchemist 20 NoReq "Craft Philosopher's Stone" 81 []
        classFeat Alchemist 20 (FeatReq "Expanded Splash") "Mega Bomb" 81 []
        classFeat Alchemist 20 NoReq "Perfect Mutagen" 81 []
    ]

    let addAlchemistFeat = Improve2.feat "Alchemist feat" alchemistFeats 1
    let increaseFormulaPool = Improve2.pool ("Formulas", 2)

    let alchemist = [
        AddClass (Alchemist, [
            Improve2.feat "Ability boost" (classAbilityBoostFeats Alchemist [Intelligence]) 1
            Improve2.hitPointsPerLevel 8
            Improve2.skill Skills.perception Trained
            Improve2.skill Skills.fortitudeSave Expert
            Improve2.skill Skills.reflexSave Expert
            Improve2.skill Skills.willSave Trained
            Improve2.skill Skills.crafting Trained
            Improve2.skillsBasedOnInt 3 Skills.regularSkills
            Weapons.improveSkill (SimpleWeapon, Melee) Trained
            Weapons.improveSkill (SimpleWeapon, Ranged) Trained
            Weapons.improveSkill (Unarmed, Melee) Trained
            Improve2.skill (Char2.weaponSkill Weapons.alchemicalBomb) Trained
            Improve2.skill (Char2.armorSkill LightArmor) Trained
            Improve2.skill (Char2.armorSkill Unarmored) Trained
            Improve2.pool ("Formulas", 6)
            Feats.forceAdd Feats.alchemicalCrafting
            Improve2.feat "Research field" alchemistResearchFields 1
            addAlchemistFeat
        ])
        LevelUp (Alchemist, 2<Level>, [
            increaseFormulaPool
            addAlchemistFeat
            Feats.addSkillFeat
        ])
        LevelUp (Alchemist, 3<Level>, [
            increaseFormulaPool
            Feats.addGeneralFeat
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Alchemist, 4<Level>, [
            increaseFormulaPool
            addAlchemistFeat
            Feats.addSkillFeat
        ])
        LevelUp (Alchemist, 5<Level>, [
            increaseFormulaPool
            Improve2.anyAbilityBoost 4
            Ancestry.addAncestryFeat
            Feats.forceAdd fieldDiscovery
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Alchemist, 6<Level>, [
            increaseFormulaPool
            addAlchemistFeat
            Feats.addSkillFeat
        ])
        LevelUp (Alchemist, 7<Level>, [
            increaseFormulaPool
            Feats.forceAdd alchemicalWeaponExpertise
            Feats.addGeneralFeat
            Feats.forceAdd Feats.ironWill
            Feats.forceAdd perpetualInfusions
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Alchemist, 8<Level>, [
            increaseFormulaPool
            addAlchemistFeat
            Feats.addSkillFeat
        ])
        LevelUp (Alchemist, 9<Level>, [
            increaseFormulaPool
            Feats.forceAdd alchemicalExpertise
            Feats.forceAdd Feats.alertness
            Ancestry.addAncestryFeat
            Feats.forceAdd doubleBrew
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Alchemist, 10<Level>, [
            increaseFormulaPool
            Improve2.anyAbilityBoost 4
            addAlchemistFeat
            Feats.addSkillFeat
        ])
        LevelUp (Alchemist, 11<Level>, [
            increaseFormulaPool
            Feats.addGeneralFeat
            Feats.forceAdd Feats.juggernaut
            Feats.forceAdd perpetualPotency
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Alchemist, 12<Level>, [
            increaseFormulaPool
            addAlchemistFeat
            Feats.addSkillFeat
        ])
        LevelUp (Alchemist, 13<Level>, [
            increaseFormulaPool
            Ancestry.addAncestryFeat
            Feats.forceAdd greaterFieldDiscovery
            Feats.forceAdd Feats.lightArmorExpertise
            Skills.increase Skills.regularSkills
            Feats.forceAdd Feats.weaponSpecialization
        ])
        LevelUp (Alchemist, 14<Level>, [
            increaseFormulaPool
            addAlchemistFeat
            Feats.addSkillFeat
        ])
        LevelUp (Alchemist, 15<Level>, [
            increaseFormulaPool
            Improve2.anyAbilityBoost 4
            Feats.forceAdd alchemicalAlacrity
            Feats.forceAdd Feats.evasion
            Feats.addGeneralFeat
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Alchemist, 16<Level>, [
            increaseFormulaPool
            addAlchemistFeat
            Feats.addSkillFeat
        ])
        LevelUp (Alchemist, 17<Level>, [
            increaseFormulaPool
            Feats.forceAdd alchemicalMastery
            Ancestry.addAncestryFeat
            Feats.forceAdd perpetualPerfection
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Alchemist, 18<Level>, [
            increaseFormulaPool
            addAlchemistFeat
            Feats.addSkillFeat
        ])
        LevelUp (Alchemist, 19<Level>, [
            increaseFormulaPool
            Feats.addGeneralFeat
            Feats.forceAdd Feats.lightArmorMastery
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Alchemist, 20<Level>, [
            increaseFormulaPool
            Improve2.anyAbilityBoost 4
            addAlchemistFeat
            Feats.addSkillFeat
        ])
    ]
