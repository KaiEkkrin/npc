namespace NpcConsole

open NpcConsole.Attributes

module Classes =
    let modValue ab c =
        Map.find ab c.Abilities
        |> Derive.modifier
        |> (fun m -> m / 1<Modifier>)

    let classFeat cl level reqs name page (imps: Improvement list) =
        let allReqs = [
            yield Improve.hasClass cl
            yield! reqs
        ]
        Feats.feat level allReqs name page imps

    // I create the class ability boost as a feat like this so that I can
    // look it up later, for those classes that let you choose and change
    // based on what you choose.
    // It also always adds the class DC skill at Trained.
    let classAbilityBoostName cl ability = sprintf "%A (%A)" cl ability
    let classAbilityBoostFeats cl abilities =
        abilities |> List.map (fun a -> classFeat cl 1 [] (classAbilityBoostName cl a) 0 [
            Improve.singleAbility a
            Improve.skill (Skills.classSkill (cl, a)) Trained
        ])

    // -- ALCHEMIST --

    let alchemistResearchFields = [
        classFeat Alchemist 1 [] "Bomber" 73 []
        classFeat Alchemist 1 [] "Chirurgeon" 73 []
        classFeat Alchemist 1 [] "Mutagenist" 73 []
    ]

    let alchemistFeats = [
        classFeat Alchemist 1 [] "Alchemical Familiar" 76 []
        classFeat Alchemist 1 [] "Alchemical Savant" 76 []
        classFeat Alchemist 1 [] "Far Lobber" 76 []
        classFeat Alchemist 1 [] "Quick Bomber" 76 []
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
                Improve.skill (Skills.weaponSkill (SimpleWeapon, Melee)) Trained
                Improve.skill (Skills.weaponSkill (SimpleWeapon, Ranged)) Trained
                Improve.skill (Skills.weaponSkill (Unarmed, Melee)) Trained
                Improve.skill Skills.alchemicalBombs Trained
                Improve.skill (Skills.armorSkill LightArmor) Trained
                Improve.skill (Skills.armorSkill Unarmored) Trained
                Feats.forceAdd Feats.alchemicalCrafting
                Improve.addFeats alchemistResearchFields 1
                Improve.addFeats alchemistFeats 1
            ]
        | _ -> failwithf "Bad level: %d" c.Level

    // -- BARBARIAN --
    
    // -- BARD --

    let bardicLoreSkill = { Name = "Bardic Lore"; KeyAbility = Intelligence }
    let bardicLore = classFeat Bard 1 [Improve.hasFeat "Enigma"] "Bardic Lore" 99 [
        Improve.skill bardicLoreSkill Trained // TODO make it expert upon Legendary proficiency in Occultism
    ]
    let lingeringComposition = classFeat Bard 1 [Improve.hasFeat "Maestro"] "Lingering Composition" 99 []
    let versatilePerformance = classFeat Bard 1 [Improve.hasFeat "Polymath"] "Versatile Performance" 100 []

    let bardFeats = [
        bardicLore
        lingeringComposition
        classFeat Bard 1 [] "Reach Spell" 99 []
        versatilePerformance
    ]

    let bardMuses = [
        classFeat Bard 1 [] "Enigma" 97 [
            Feats.forceAdd bardicLore // TODO also add to spell repertoire
        ]
        classFeat Bard 1 [] "Maestro" 97 [
            Feats.forceAdd lingeringComposition
        ]
        classFeat Bard 1 [] "Polymath" 97 [
            Feats.forceAdd versatilePerformance
        ]
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
                Improve.skill (Skills.weaponSkill (SimpleWeapon, Melee)) Trained
                Improve.skill (Skills.weaponSkill (SimpleWeapon, Ranged)) Trained
                Improve.skill (Skills.weaponSkill (Unarmed, Melee)) Trained
                // TODO specific weapons (longsword, ...)
                Improve.skill (Skills.armorSkill LightArmor) Trained
                Improve.skill (Skills.armorSkill Unarmored) Trained
                Improve.addFeats bardMuses 1
            ]
        | _ -> failwithf "Bad level: %d" c.Level

    // -- CHAMPION --

    // -- CLERIC --

    // -- DRUID --

    let animalCompanion = classFeat Druid 1 [Improve.hasFeat "Animal"] "Animal Companion" 133 []
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
            // Increase focus pool by 1, etc
        ]
        classFeat Druid 1 [] "Storm" 132 [
            Improve.skill Skills.acrobatics Trained
            Feats.forceAdd stormBorn
            // Increase focus pool, etc
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
                Improve.skill (Skills.weaponSkill (SimpleWeapon, Melee)) Trained
                Improve.skill (Skills.weaponSkill (SimpleWeapon, Ranged)) Trained
                Improve.skill (Skills.weaponSkill (Unarmed, Melee)) Trained
                Improve.skill (Skills.armorSkill LightArmor) Trained
                Improve.skill (Skills.armorSkill MediumArmor) Trained
                Improve.skill (Skills.armorSkill Unarmored) Trained
                Improve.addFeats druidOrders 1
                Feats.forceAdd Feats.shieldBlock
                Feats.forceAdd wildEmpathy
            ]
        | _ -> failwithf "Bad level: %d" c.Level

    // -- FIGHTER --

    // -- MONK --

    // -- RANGER --

    let huntPrey = classFeat Ranger 1 [] "Hunt Prey" 168 []
    let huntersEdge = classFeat Ranger 1 [] "Hunter's Edge" 168 []
    let rangerFeats = [
        classFeat Ranger 1 [] "Animal Companion" 170 []
        classFeat Ranger 1 [] "Crossbow Ace" 171 []
        classFeat Ranger 1 [] "Hunted Shot" 171 []
        classFeat Ranger 1 [] "Monster Hunter" 171 []
        classFeat Ranger 1 [] "Twin Takedown" 171 []
    ]

    let ranger c =
        match c.Level with 
        | 1<Level> ->
            { c with Class = Some Ranger }, [
                Improve.addFeats (classAbilityBoostFeats Ranger [Strength; Dexterity]) 1
                Improve.hitPointsPerLevel 10
                Improve.skill Skills.perception Expert
                Improve.skill Skills.fortitudeSave Expert
                Improve.skill Skills.reflexSave Expert
                Improve.skill Skills.willSave Trained
                Improve.skill Skills.nature Trained
                Improve.skill Skills.survival Trained
                Improve.skills Skills.regularSkills Trained ((modValue Intelligence c) + 4)
                Improve.skill (Skills.weaponSkill (SimpleWeapon, Melee)) Trained
                Improve.skill (Skills.weaponSkill (SimpleWeapon, Ranged)) Trained
                Improve.skill (Skills.weaponSkill (MartialWeapon, Melee)) Trained
                Improve.skill (Skills.weaponSkill (MartialWeapon, Ranged)) Trained
                Improve.skill (Skills.weaponSkill (Unarmed, Melee)) Trained
                Improve.skill (Skills.armorSkill LightArmor) Trained
                Improve.skill (Skills.armorSkill MediumArmor) Trained
                Improve.skill (Skills.armorSkill Unarmored) Trained
                Feats.forceAdd huntPrey
                Feats.forceAdd huntersEdge
                Improve.addFeats rangerFeats 1
            ]
        | _ -> failwithf "Bad level: %d" c.Level

    // -- ROGUE --

    // TODO sneak attack (damage dice); maybe change the Class discriminated union
    // so that it includes things like sneak attack dice, focus points etc etc?

    // Because the rogue's key ability score can vary by racket, it's assigned
    // through the racket rather than directly when you choose the Rogue class
    let rogueRackets = [
        classFeat Rogue 1 [] "Ruffian" 180 [
            Improve.skill Skills.intimidation Trained
            Improve.skill (Skills.armorSkill MediumArmor) Trained
            Improve.addFeats (classAbilityBoostFeats Rogue [Strength; Dexterity]) 1
            // TODO when gaining expert/master light armor gain expert/master medium
            // armor as well
        ]
        classFeat Rogue 1 [] "Scoundrel" 180 [
            Improve.skill Skills.deception Trained
            Improve.skill Skills.diplomacy Trained
            Improve.addFeats (classAbilityBoostFeats Rogue [Dexterity; Charisma]) 1
        ]
        classFeat Rogue 1 [] "Thief" 180 [
            // TODO add Dexterity not Strength to damage when attacking with a
            // finesse weapon
            Improve.skill Skills.thievery Trained
            Improve.addFeats (classAbilityBoostFeats Rogue [Dexterity]) 1
        ]
    ]

    let surpriseAttack = classFeat Rogue 1 [] "Surprise Attack" 181 []
    let rogueFeats = [
        classFeat Rogue 1 [] "Nimble Dodge" 183 []
        classFeat Rogue 1 [] "Trap Finder" 183 []
        classFeat Rogue 1 [] "Twin Feint" 183 []
        classFeat Rogue 1 [Improve.hasSkill Skills.intimidation Trained] "You're Next" 183 []
    ]

    let rogue c =
        match c.Level with 
        | 1<Level> ->
            { c with Class = Some Rogue }, [
                Improve.hitPointsPerLevel 8
                Improve.skill Skills.perception Expert
                Improve.skill Skills.fortitudeSave Trained
                Improve.skill Skills.reflexSave Expert
                Improve.skill Skills.willSave Expert
                Improve.skill Skills.stealth Trained
                Improve.skill (Skills.weaponSkill (SimpleWeapon, Melee)) Trained
                Improve.skill (Skills.weaponSkill (SimpleWeapon, Ranged)) Trained
                // TODO rapier, sap, shortbow, shortsword
                Improve.skill (Skills.weaponSkill (Unarmed, Melee)) Trained
                Improve.skill (Skills.armorSkill LightArmor) Trained
                Improve.skill (Skills.armorSkill Unarmored) Trained
                Improve.addFeats rogueRackets 1
                Improve.skills Skills.regularSkills Trained ((modValue Intelligence c) + 7)
                Feats.forceAdd surpriseAttack
                Improve.addFeats rogueFeats 1
            ]
        | _ -> failwithf "Bad level: %d" c.Level

    // -- SORCERER --

    // -- WIZARD --

    // -- EVERYTHING --
    let hasNoClass c = Option.isNone c.Class
    let classes = {
        Prompt = "Class"
        Choices = [
            "Alchemist", hasNoClass, alchemist
            "Bard", hasNoClass, bard
            "Druid", hasNoClass, druid
            "Ranger", hasNoClass, ranger
            "Rogue", hasNoClass, rogue
        ]
        Count = 1
    }

