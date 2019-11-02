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

    // Increases the character's class skill, whatever it is (there should only be the one) --
    // only for Expert and higher
    let classSkill cl prof =
        let choices =
            [Strength; Dexterity; Constitution; Intelligence; Wisdom; Charisma]
            |> List.map (fun ab ->
                let boostName = classAbilityBoostName cl ab
                let sk = Skills.classSkill (cl, ab)
                (boostName, (fun c -> Map.containsKey sk c.Skills), fun c -> { c with Skills = Map.add sk prof c.Skills }, []))
        {
            Prompt = "Class skill increase"
            Choices = choices
            Count = 1
        }

    // -- ALCHEMIST --

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

    // -- BARBARIAN --
    
    // -- BARD --

    let bardicLoreSkill = { Name = "Bardic Lore"; KeyAbility = Intelligence }
    let bardicLore = classFeat Bard 1 [Improve.hasFeat "Enigma"] "Bardic Lore" 99 [
        Improve.skill bardicLoreSkill Trained // TODO make it expert upon Legendary proficiency in Occultism
    ]
    let expertSpellcaster = classFeat Bard 7 [] "Expert Spellcaster" 98 [
        classSkill Bard Expert
    ]
    let lingeringComposition = classFeat Bard 1 [Improve.hasFeat "Maestro"] "Lingering Composition" 99 []
    let signatureSpells = classFeat Bard 1 [] "Signature Spells" 98 []
    let versatilePerformance = classFeat Bard 1 [Improve.hasFeat "Polymath"] "Versatile Performance" 100 []

    let bardFeats = [
        bardicLore
        lingeringComposition
        classFeat Bard 1 [] "Reach Spell" 99 []
        versatilePerformance
        classFeat Bard 2 [] "Cantrip Expansion" 100 []
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
                Weapons.improveSkill (SimpleWeapon, Melee) Trained
                Weapons.improveSkill (SimpleWeapon, Ranged) Trained
                Weapons.improveSkill (Unarmed, Melee) Trained
                Improve.skill (Skills.armorSkill LightArmor) Trained
                Improve.skill (Skills.armorSkill MediumArmor) Trained
                Improve.skill (Skills.armorSkill Unarmored) Trained
                Improve.spellSkill (Skills.spellSkill (Primal, Wisdom))
                Improve.spell (0, 4)
                Improve.spell (1, 2)
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
                Weapons.improveSkill (SimpleWeapon, Melee) Trained
                Weapons.improveSkill (SimpleWeapon, Ranged) Trained
                Weapons.improveSkill (MartialWeapon, Melee) Trained
                Weapons.improveSkill (MartialWeapon, Ranged) Trained
                Weapons.improveSkill (Unarmed, Melee) Trained
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
                Weapons.improveSkill (SimpleWeapon, Melee) Trained
                Weapons.improveSkill (SimpleWeapon, Ranged) Trained
                Weapons.improveSkill (Unarmed, Melee) Trained
                Improve.skill (Skills.weaponSkill Weapons.rapier) Trained
                Improve.skill (Skills.weaponSkill Weapons.sap) Trained
                Improve.skill (Skills.weaponSkill Weapons.shortbow) Trained
                Improve.skill (Skills.weaponSkill Weapons.shortsword) Trained
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
    let hasClass cl c = match c.Class with | Some cl2 when cl2 = cl -> true | _ -> false
    
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

    let levelUp = {
        Prompt = "Level up"
        Choices = [
            "Alchemist", hasClass Alchemist, alchemist
            "Bard", hasClass Bard, bard
            "Druid", hasClass Druid, druid
            "Ranger", hasClass Ranger, ranger
            "Rogue", hasClass Rogue, rogue
        ]
        Count = 1
    }
