namespace NpcConsole

open NpcConsole.Attributes

module Feats =

    // -- HELPERS --

    // Defines a feat requirement in terms of (level, names of other feats.)
    let req level others c =
        let hasOthers = others |> List.fold (fun ok o ->
            match List.tryFind (fun (f: Feat) -> f.Name = o) c.Feats with
            | Some _ -> ok
            | None -> false) true
        hasOthers && Improve.hasLevel level c

    // Similarly, but defines a feat requirement in terms of level and a list of
    // feats that a character must have one of, not all of.
    let reqOne level others c =
        let hasOne = others |> List.fold (fun ok o ->
            match List.tryFind (fun (f: Feat) -> f.Name = o) c.Feats with
            | Some _ -> true
            | None -> ok) false
        hasOne && Improve.hasLevel level c

    // Define a feat in terms of its (requirements, consequent improvements).
    // TODO replace 0 with pages here
    let feat level reqs name page (imps: Improvement list) =
        // A feat also requires that it isn't taken already (TODO apart from take-multiple-times feats; deal with separately e.g. name differently each time)
        // and that the character meets the level:
        let allReqs = [
            yield Improve.doesNotHaveFeat name
            yield Improve.hasLevel level
            yield! reqs
        ]
        { Name = name; Page = page },
        Improve.require allReqs,
        imps

    // Defines a skill feat in terms of the skill and proficiency it requires,
    // and its level prereq.
    let skillFeat level sk prof = feat level [Improve.hasSkill sk prof]

    // Adds a feat to a character even if they don't meet the prereqs.
    let forceAdd (feat, _, imps) = Improve.addFeat (feat, imps)

    // -- SPECIAL FEATS -- Usually applied straight up

    let darkvision = feat 1 [] "Darkvision" 0 []
    let keenEyes = feat 1 [] "Keen Eyes" 0 []
    let lowLightVision = feat 1 [] "Low-light vision" 0 []

    // -- BIG LIST OF OTHER FEATS -- (The feats chapter in the book)

    let additionalLore l = feat 1 [] (sprintf "Additional Lore (%s)" l) 258 [
        Improve.skill (Skills.lore l) Trained
    ]
    let adoptedAncestry a = feat 1 [] (sprintf "Adopted Ancestry (%s)" a) 258 // TODO actually implement this (qualifies you for different ancestry feats)
    let alchemicalCrafting = skillFeat 1 Skills.crafting Trained "Alchemical Crafting" 258 [] // TODO count items and formulas
    // TODO ancestral paragon (circular dependency)
    let arcaneSense = skillFeat 1 Skills.arcana Trained "Arcane Sense" 258 []
    let armorProficiencyLight = feat 1 [Improve.doesNotHaveSkill (Skills.armorSkill LightArmor) Trained] "Armor Proficiency (Light)" 258 [
        Improve.skill (Skills.armorSkill LightArmor) Trained
    ]
    let armorProficiencyMedium = feat 1 [Improve.hasSkill (Skills.armorSkill LightArmor) Trained; Improve.doesNotHaveSkill (Skills.armorSkill MediumArmor) Trained] "Armor Proficiency (Medium)" 258 [
        Improve.skill (Skills.armorSkill MediumArmor) Trained
    ]
    let armorProficiencyHeavy = feat 1 [Improve.hasSkill (Skills.armorSkill MediumArmor) Trained; Improve.doesNotHaveSkill (Skills.armorSkill HeavyArmor) Trained] "Armor Proficiency (Heavy)" 258 [
        Improve.skill (Skills.armorSkill HeavyArmor) Trained
    ]
    let assurance (sk: Skill) = skillFeat 1 sk Trained (sprintf "Assurance (%s)" sk.Name) 258 []
    let automaticKnowledge (sk: Skill) = skillFeat 2 sk Expert (sprintf "Automatic Knowledge (%s)" sk.Name) 258 []
    let bargainHunter = skillFeat 1 Skills.diplomacy Trained "Bargain Hunter" 258 []
    let battleCry = skillFeat 7 Skills.intimidation Master "Battle Cry" 258 []
    let battleMedic = skillFeat 1 Skills.medicine Trained "Battle Medic" 258 []
    let bizarreMagic = skillFeat 7 Skills.occultism Master "Bizarre Magic" 258 []
    let bondedAnimal = skillFeat 2 Skills.nature Expert "Bonded Animal" 259 []
    let breathControl = feat 1 [] "Breath Control" 259 []
    let cannyAcumen (sk: Skill) = feat 1 [Improve.hasSkill sk Trained; Improve.doesNotHaveSkill sk Expert] (sprintf "Canny Acumen (%s)" sk.Name) 259 [ // Saves only.  TODO master at 17th level
        Improve.skill sk Expert
    ]
    let catFall = skillFeat 1 Skills.acrobatics Trained "Cat Fall" 259 []
    let charmingLiar = skillFeat 1 Skills.deception Trained "Charming Liar" 259 []
    let cloudJump = skillFeat 15 Skills.athletics Legendary "Cloud Jump" 260 []
    let combatClimber = skillFeat 1 Skills.athletics Trained "Combat Climber" 260 []
    let confabulator = skillFeat 2 Skills.deception Expert "Confabulator" 260 []
    let continualRecovery = skillFeat 2 Skills.medicine Expert "Continual Recovery" 260 []
    let courtlyGraces = skillFeat 1 Skills.society Trained "Courtly Graces" 260 []
    let craftAnything = skillFeat 15 Skills.crafting Legendary "Craft Anything" 260 []
    let diehard = feat 1 [] "Diehard" 260 []
    let divineGuidance = skillFeat 15 Skills.religion Legendary "Divine Guidance" 260 []
    let dubiousKnowledge = feat 1 [] "Dubious Knowledge" 260 []
    let expeditiousSearch = skillFeat 7 Skills.perception Master "Expeditious Search" 260 []
    let experiencedProfessional = feat 1 [] "Experienced Professional" 261 [] // all characters get a Lore skill
    let experiencedSmuggler = skillFeat 1 Skills.stealth Trained "Experienced Smuggler" 261 []
    let experiencedTracker = skillFeat 1 Skills.survival Trained "Experienced Tracker" 261 []
    let fascinatingPerformance = skillFeat 1 Skills.performance Trained "Fascinating Performance" 261 []
    let fastRecovery = feat 1 [Improve.hasAbilityScore Constitution 14<Score>] "Fast Recovery" 261 []
    let featherStep = feat 1 [Improve.hasAbilityScore Dexterity 14<Score>] "Feather Step" 261 []
    let fleet = feat 1 [] "Fleet" 261 [
        Improve.speed 5<Feet>
    ]
    let foilSenses = skillFeat 7 Skills.stealth Master "Foil Senses" 261 []
    let forager = skillFeat 1 Skills.survival Trained "Forager" 261 []
    let gladHand = skillFeat 2 Skills.diplomacy Expert "Glad-hand" 261 []
    let groupCoercion = skillFeat 1 Skills.intimidation Trained "Group Coercion" 262 []
    let groupImpression = skillFeat 1 Skills.diplomacy Trained "Group Impression" 262 []
    let heftyHauler = skillFeat 1 Skills.athletics Trained "Hefty Hauler" 262 []
    let hobnobber = skillFeat 1 Skills.diplomacy Trained "Hobnobber" 262 []
    let impressivePerformance = skillFeat 1 Skills.performance Trained "Impressive Performance" 262 []
    let incredibleInitiative = feat 1 [] "Incredible Initiative" 262 []
    let incredibleInvestiture = feat 11 [Improve.hasAbilityScore Charisma 16<Score>] "Incredible Investiture" 262 [] // TODO encode number of magic items that can be invested
    let intimidatingGlare = skillFeat 1 Skills.intimidation Trained "Intimidating Glare" 262 []
    let intimidatingProwess = feat 2 [Improve.hasSkill Skills.intimidation Expert; Improve.hasAbilityScore Strength 16<Score>] "Intimidating Prowess" 262 []
    let inventor = skillFeat 7 Skills.crafting Master "Inventor" 262 []
    let kipUp = skillFeat 7 Skills.acrobatics Master "Kip Up" 262 []
    let lastingCoercion = skillFeat 2 Skills.intimidation Expert "Lasting Coercion" 262 []
    let legendaryCodebreaker = skillFeat 15 Skills.society Legendary "Legendary Codebreaker" 262 []
    let legendaryMedic = skillFeat 15 Skills.medicine Legendary "Legendary Medic" 263 []
    let legendaryNegotiation = skillFeat 15 Skills.diplomacy Legendary "Legendary Negotiation" 263 []
    let legendaryProfessional = feat 15 [Improve.hasLore Legendary] "Legendary Professional" 263 []
    let legendarySurvivalist = skillFeat 15 Skills.survival Legendary "Legendary Survivalist" 263 []
    let lengthyDiversion = skillFeat 1 Skills.deception Trained "Lengthy Diversion" 263 []
    let lieToMe = skillFeat 1 Skills.deception Trained "Lie to Me" 263 []
    let magicalCrafting = skillFeat 2 Skills.crafting Expert "Magical Crafting" 263 []
    let magicalShorthand = feat 2 [Improve.hasAnySkill [Skills.arcana; Skills.nature; Skills.occultism; Skills.religion] Expert] "Magical Shorthand" 264 []
    let multilingual = skillFeat 1 Skills.society Trained "Multilingual" 264 [] // TODO take multiple times (requires me to implement languages)
    let naturalMedicine = feat SkillFeat (fun _ -> true) "Natural Medicine" []
    let oddityIdentification = feat SkillFeat (fun _ -> true) "Oddity Identification" []
    let pickpocket = feat SkillFeat (fun _ -> true) "Pickpocket" []
    let quickCoercion = feat SkillFeat (fun _ -> true) "Quick Coercion" []
    let ride = feat [] "Ride" 0 []
    let specialtyCrafting = feat SkillFeat (fun _ -> true) "Specialty Crafting" []
    let steadyBalance = feat SkillFeat (fun _ -> true) "Steady Balance" []
    let streetwise = feat SkillFeat (fun _ -> true) "Streetwise" []
    let studentOfTheCanon = feat SkillFeat (fun _ -> true) "Student of the Canon" []
    let surveyWildlife = feat SkillFeat (fun _ -> true) "Survey Wildlife" []
    let terrainExpertise terrain = feat SkillFeat (fun _ -> true) (sprintf "Terrain Expertise (%s)" terrain) [] // TODO terrain types
    let trainAnimal = feat SkillFeat (fun _ -> true) "Train Animal" []
    let underwaterMarauder = feat SkillFeat (fun _ -> true) "Underwater Marauder" []

    // Feats that depend on others
    let connections = feat 2 [Improve.hasSkill Skills.society Expert; Improve.hasFeat "Courtly Graces"] "Connections" 260 []
    let impeccableCrafting = feat 7 [Improve.hasSkill Skills.crafting Master; Improve.hasFeat "Specialty Crafting"] "Impeccable Crafting" 262 []
    let legendaryLinguist = feat 15 [Improve.hasSkill Skills.society Legendary; Improve.hasFeat "Multilingual"] "Legendary Linguist" 263 []
    let legendaryPerformer = feat 15 [Improve.hasSkill Skills.performance Legendary; Improve.hasFeat "Virtuosic Performer"] "Legendary Performer" 263 []
    let legendarySneak = feat 15 [Improve.hasSkill Skills.stealth Legendary; Improve.hasFeat "Swift Sneak"] "Legendary Sneak" 263 []
    let legendaryThief = feat 15 [Improve.hasSkill Skills.thievery Legendary; Improve.hasFeat "Pickpocket"] "Legendary Thief" 263 []

    // TODO list general feats here
    let generalFeats = [
        alchemicalCrafting
        ride
    ]

    // TODO list skill feats here
    // TODO *2 include choice feats e.g. Assurance
    let skillFeats = [
        bargainHunter
        battleMedic
        catFall
        charmingLiar
        courtlyGraces
        dubiousKnowledge
        experiencedSmuggler
        experiencedTracker
        fascinatingPerformance
        forager
        groupImpression
        hobnobber
        impressivePerformance
        lieToMe
        multilingual
        naturalMedicine
        oddityIdentification
        pickpocket
        quickCoercion
        specialtyCrafting
        steadyBalance
        streetwise
        studentOfTheCanon
        surveyWildlife
        trainAnimal
        underwaterMarauder
    ]
