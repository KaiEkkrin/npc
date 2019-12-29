namespace Npc

open Npc.Attributes
open Npc.FeatReq

module Feats =

    // -- HELPERS --

    // Defines a feat in terms of its (requirements, further improvements.)
    let featWith level reqs name page imps =
        // A feat also requires that it isn't taken already (TODO apart from take-multiple-times feats; deal with separately e.g. name differently each time)
        // and that the character meets the level:
        let allReqs =
            NotReq (FeatReq name)
            >&& LevelReq (level * 1<Level>)
            >&& reqs
        AddFeat (allReqs, { Name = name; Page = page }, imps)

    // Define a feat in terms of its (requirements, consequent changes).
    // Each consequent change is added individually if it applies.
    let feat level reqs name page changes =
        let imps = changes |> List.map (fun ch -> {
            Prompt = sprintf "%A" ch
            Choices = [ch]
            Count = None
        })
        featWith level reqs name page imps

    // Defines a skill feat in terms of the skill and proficiency it requires,
    // and its level prereq.
    let skillFeat level sk prof = feat level (SkillReq (sk, prof))

    // Adds a feat to a character even if they don't meet the prereqs.
    //let forceAdd feat = AddFeat (NoReq, feat, [])
    let forceAdd ch =
        match ch with
        | AddFeat (_, feat, imps) -> {
            Prompt = feat.Name
            Choices = [AddFeat (NoReq, feat, imps)]
            Count = None }
        | _ -> failwithf "Can't force add %A" ch

    // -- SPECIAL FEATS -- Usually applied straight up

    let alertness = feat 9 NoReq "Alertness" 0 [
        AddSkill (Skills.perception, Expert)
    ]
    let darkvision = feat 1 NoReq "Darkvision" 0 []
    let evasion = feat 15 NoReq "Evasion" 0 [
        AddSkill (Skills.reflexSave, Master)
    ]
    let greatFortitude = feat 9 NoReq "Great Fortitude" 0 [
        AddSkill (Skills.fortitudeSave, Expert)
    ]
    let ironWill = feat 7 NoReq "Iron Will" 0 [
        AddSkill (Skills.willSave, Expert)
    ]
    let juggernaut = feat 11 NoReq "Juggernaut" 0 [
        AddSkill (Skills.fortitudeSave, Master)
    ]
    let keenEyes = feat 1 NoReq "Keen Eyes" 0 []
    let lightArmorExpertise = feat 13 NoReq "Light Armor Expertise" 0 [
        AddSkill ((Char2.armorSkill Unarmored), Expert)
        AddSkill ((Char2.armorSkill LightArmor), Expert)
    ]
    let lightArmorMastery = feat 19 NoReq "Light Armor Mastery" 0 [
        AddSkill ((Char2.armorSkill Unarmored), Master)
        AddSkill ((Char2.armorSkill LightArmor), Master)
    ]
    let lightningReflexes = feat 3 NoReq "Lighting Reflexes" 0 [
        AddSkill (Skills.reflexSave, Expert)
    ]
    let lowLightVision = feat 1 NoReq "Low-light vision" 0 []
    let mediumArmorExpertise = feat 13 NoReq "Medium Armor Expertise" 0 [
        AddSkill ((Char2.armorSkill Unarmored), Expert)
        AddSkill ((Char2.armorSkill LightArmor), Expert)
        AddSkill ((Char2.armorSkill MediumArmor), Expert)
    ]
    let resolve = feat 9 NoReq "Resolve" 0 [
        AddSkill (Skills.willSave, Master)
    ]
    let weaponSpecialization = feat 13 NoReq "Weapon Specialization" 0 [] // The bonus is implemented in the character sheet

    // -- BIG LIST OF OTHER FEATS -- (The feats chapter in the book)

    let additionalLore l = feat 1 NoReq (sprintf "Additional Lore (%s)" l) 258 [
        AddSkill ((Skills.lore l), Trained)
    ]
    let adoptedAncestry a = feat 1 NoReq (sprintf "Adopted Ancestry (%s)" a) 258 // TODO actually implement this (qualifies you for different ancestry feats)
    let alchemicalCrafting = skillFeat 1 Skills.crafting Trained "Alchemical Crafting" 258 [] // TODO count items and formulas
    // TODO ancestral paragon (circular dependency)
    let arcaneSense = skillFeat 1 Skills.arcana Trained "Arcane Sense" 258 []
    let armorProficiencyLight = feat 1 (doesNotHaveSkill (Char2.armorSkill LightArmor) Trained) "Armor Proficiency (Light)" 258 [
        AddSkill ((Char2.armorSkill LightArmor), Trained)
    ]
    let armorProficiencyMedium = feat 1 (hasSkill (Char2.armorSkill LightArmor) Trained >&& doesNotHaveSkill (Char2.armorSkill MediumArmor) Trained) "Armor Proficiency (Medium)" 258 [
        AddSkill ((Char2.armorSkill MediumArmor), Trained)
    ]
    let armorProficiencyHeavy = feat 1 (hasSkill (Char2.armorSkill MediumArmor) Trained >&& doesNotHaveSkill (Char2.armorSkill HeavyArmor) Trained) "Armor Proficiency (Heavy)" 258 [
        AddSkill ((Char2.armorSkill HeavyArmor), Trained)
    ]
    let assurance (sk: Skill) = skillFeat 1 sk Trained (sprintf "Assurance (%s)" sk.Name) 258 []
    let automaticKnowledge (sk: Skill) = skillFeat 2 sk Expert (sprintf "Automatic Knowledge (%s)" sk.Name) 258 []
    let bargainHunter = skillFeat 1 Skills.diplomacy Trained "Bargain Hunter" 258 []
    let battleCry = skillFeat 7 Skills.intimidation Master "Battle Cry" 258 []
    let battleMedic = skillFeat 1 Skills.medicine Trained "Battle Medic" 258 []
    let bizarreMagic = skillFeat 7 Skills.occultism Master "Bizarre Magic" 258 []
    let bondedAnimal = skillFeat 2 Skills.nature Expert "Bonded Animal" 259 []
    let breathControl = feat 1 NoReq "Breath Control" 259 []
    let cannyAcumen (sk: Skill) = feat 1 (hasSkill sk Trained >&& doesNotHaveSkill sk Expert) (sprintf "Canny Acumen (%s)" sk.Name) 259 [ // Saves only.  TODO master at 17th level
        AddSkill (sk, Expert)
    ]
    let catFall = skillFeat 1 Skills.acrobatics Trained "Cat Fall" 259 []
    let charmingLiar = skillFeat 1 Skills.deception Trained "Charming Liar" 259 []
    let cloudJump = skillFeat 15 Skills.athletics Legendary "Cloud Jump" 260 []
    let combatClimber = skillFeat 1 Skills.athletics Trained "Combat Climber" 260 []
    let confabulator = skillFeat 2 Skills.deception Expert "Confabulator" 260 []
    let connections = feat 2 (hasSkill Skills.society Expert >&& FeatReq "Courtly Graces") "Connections" 260 []
    let continualRecovery = skillFeat 2 Skills.medicine Expert "Continual Recovery" 260 []
    let courtlyGraces = skillFeat 1 Skills.society Trained "Courtly Graces" 260 []
    let craftAnything = skillFeat 15 Skills.crafting Legendary "Craft Anything" 260 []
    let diehard = feat 1 NoReq "Diehard" 260 []
    let divineGuidance = skillFeat 15 Skills.religion Legendary "Divine Guidance" 260 []
    let dubiousKnowledge = feat 1 NoReq "Dubious Knowledge" 260 []
    let expeditiousSearch = skillFeat 7 Skills.perception Master "Expeditious Search" 260 []
    let experiencedProfessional = feat 1 NoReq "Experienced Professional" 261 [] // all characters get a Lore skill
    let experiencedSmuggler = skillFeat 1 Skills.stealth Trained "Experienced Smuggler" 261 []
    let experiencedTracker = skillFeat 1 Skills.survival Trained "Experienced Tracker" 261 []
    let fascinatingPerformance = skillFeat 1 Skills.performance Trained "Fascinating Performance" 261 []
    let fastRecovery = feat 1 (ability Constitution 14<Score>) "Fast Recovery" 261 []
    let featherStep = feat 1 (ability Dexterity 14<Score>) "Feather Step" 261 []
    let fleet = feat 1 NoReq "Fleet" 261 [
        IncreaseSpeed 5<Feet>
    ]
    let foilSenses = skillFeat 7 Skills.stealth Master "Foil Senses" 261 []
    let forager = skillFeat 1 Skills.survival Trained "Forager" 261 []
    let gladHand = skillFeat 2 Skills.diplomacy Expert "Glad-hand" 261 []
    let groupCoercion = skillFeat 1 Skills.intimidation Trained "Group Coercion" 262 []
    let groupImpression = skillFeat 1 Skills.diplomacy Trained "Group Impression" 262 []
    let heftyHauler = skillFeat 1 Skills.athletics Trained "Hefty Hauler" 262 []
    let hobnobber = skillFeat 1 Skills.diplomacy Trained "Hobnobber" 262 []
    let impeccableCrafting = feat 7 (hasSkill Skills.crafting Master >&& FeatReq "Specialty Crafting") "Impeccable Crafting" 262 []
    let impressivePerformance = skillFeat 1 Skills.performance Trained "Impressive Performance" 262 []
    let incredibleInitiative = feat 1 NoReq "Incredible Initiative" 262 []
    let incredibleInvestiture = feat 11 (ability Charisma 16<Score>) "Incredible Investiture" 262 [] // TODO encode number of magic items that can be invested
    let intimidatingGlare = skillFeat 1 Skills.intimidation Trained "Intimidating Glare" 262 []
    let intimidatingProwess = feat 2 (hasSkill Skills.intimidation Expert >&& ability Strength 16<Score>) "Intimidating Prowess" 262 []
    let inventor = skillFeat 7 Skills.crafting Master "Inventor" 262 []
    let kipUp = skillFeat 7 Skills.acrobatics Master "Kip Up" 262 []
    let lastingCoercion = skillFeat 2 Skills.intimidation Expert "Lasting Coercion" 262 []
    let legendaryCodebreaker = skillFeat 15 Skills.society Legendary "Legendary Codebreaker" 262 []
    let legendaryLinguist = feat 15 (hasSkill Skills.society Legendary >&& FeatReq "Multilingual") "Legendary Linguist" 263 []
    let legendaryMedic = skillFeat 15 Skills.medicine Legendary "Legendary Medic" 263 []
    let legendaryNegotiation = skillFeat 15 Skills.diplomacy Legendary "Legendary Negotiation" 263 []
    let legendaryPerformer = feat 15 (hasSkill Skills.performance Legendary >&& FeatReq "Virtuosic Performer") "Legendary Performer" 263 []
    let legendaryProfessional = feat 15 (LoreReq Legendary) "Legendary Professional" 263 []
    let legendarySneak = feat 15 (hasSkill Skills.stealth Legendary >&& FeatReq "Swift Sneak") "Legendary Sneak" 263 []
    let legendarySurvivalist = skillFeat 15 Skills.survival Legendary "Legendary Survivalist" 263 []
    let legendaryThief = feat 15 (hasSkill Skills.thievery Legendary >&& FeatReq "Pickpocket") "Legendary Thief" 263 []
    let lengthyDiversion = skillFeat 1 Skills.deception Trained "Lengthy Diversion" 263 []
    let lieToMe = skillFeat 1 Skills.deception Trained "Lie to Me" 263 []
    let magicalCrafting = skillFeat 2 Skills.crafting Expert "Magical Crafting" 263 []
    let magicalShorthand = feat 2 (hasAnySkill [Skills.arcana; Skills.nature; Skills.occultism; Skills.religion] Expert) "Magical Shorthand" 264 []
    let multilingual = skillFeat 1 Skills.society Trained "Multilingual" 264 [] // TODO take multiple times (requires me to implement languages)
    let naturalMedicine = skillFeat 1 Skills.nature Trained "Natural Medicine" 264 []
    let nimbleCrawl = skillFeat 2 Skills.acrobatics Expert "Nimble Crawl" 264 []
    let oddityIdentification = skillFeat 1 Skills.occultism Trained "Oddity Identification" 264 []
    let pickpocket = skillFeat 1 Skills.thievery Trained "Pickpocket" 264 []
    let planarSurvival = skillFeat 7 Skills.survival Master "Planar Survival" 264 []
    let powerfulLeap = skillFeat 2 Skills.athletics Expert "Powerful Leap" 264 []
    let quickClimb = skillFeat 7 Skills.athletics Master "Quick Climb" 264 []
    let quickCoercion = skillFeat 1 Skills.intimidation Trained "Quick Coercion" 264 []
    let quickDisguise = skillFeat 2 Skills.deception Expert "Quick Disguise" 264 []
    let quickIdentification = feat 1 (hasAnySkill [Skills.arcana; Skills.nature; Skills.occultism; Skills.religion] Trained) "Quick Identification" 264 []
    let quickJump = skillFeat 1 Skills.athletics Trained "Quick Jump" 264 []
    let quickRecognition = feat 7 (hasAnySkill [Skills.arcana; Skills.nature; Skills.occultism; Skills.religion] Master >&& FeatReq "Recognize Spell") "Quick Recognition" 265 []
    let quickRepair = skillFeat 1 Skills.crafting Trained "Quick Repair" 265 []
    let quickSqueeze = skillFeat 1 Skills.acrobatics Trained "Quick Squeeze" 265 []
    let quickSwim = skillFeat 7 Skills.athletics Master "Quick Swim" 265 []
    let quickUnlock = skillFeat 7 Skills.thievery Master "Quick Unlock" 265 []
    let quietAllies = skillFeat 2 Skills.stealth Expert "Quiet Allies" 265 []
    let rapidMantel = skillFeat 2 Skills.athletics Expert "Rapid Mantel" 265 []
    let readLips = skillFeat 1 Skills.society Trained "Read Lips" 265 []
    let recognizeSpell = feat 1 (hasAnySkill [Skills.arcana; Skills.nature; Skills.occultism; Skills.religion] Trained) "Recognize Spell" 265 []
    let ride = feat 1 NoReq "Ride" 266 []
    let robustRecovery = skillFeat 2 Skills.medicine Expert "Robust Recovery" 266 []
    let scareToDeath = skillFeat 15 Skills.intimidation Legendary "Scare to Death" 266 []
    let shamelessRequest = skillFeat 7 Skills.diplomacy Master "Shameless Request" 266 []
    let shieldBlock = feat 1 NoReq "Shield Block" 266 []
    let signLanguage = skillFeat 1 Skills.society Trained "Sign Language" 266 []
    let skillTraining sk = feat 1 (ability Intelligence 12<Score> >&& doesNotHaveSkill sk Trained) (sprintf "Skill Training (%s)" sk.Name) 266 []
    let slipperySecrets = skillFeat 7 Skills.deception Master "Slippery Secrets" 266 []
    let snareCrafting = skillFeat 1 Skills.crafting Trained "Snare Crafting" 266 []
    let specialtyCrafting specialty = skillFeat 1 Skills.crafting Trained (sprintf "Specialty Crafting (%s)" specialty) 266 []
    let steadyBalance = skillFeat 1 Skills.acrobatics Trained "Steady Balance" 267 []
    let streetwise = skillFeat 1 Skills.society Trained "Streetwise" 267 []
    let studentOfTheCanon = skillFeat 1 Skills.religion Trained "Student of the Canon" 267 []
    let subtleTheft = skillFeat 1 Skills.thievery Trained "Subtle Theft" 267 []
    let surveyWildlife = skillFeat 1 Skills.survival Trained "Survey Wildlife" 267 []
    let swiftSneak = skillFeat 7 Skills.stealth Master "Swift Sneak" 267 []
    let terrainExpertise terrain = skillFeat 1 Skills.survival Trained (sprintf "Terrain Expertise (%s)" terrain) 267 [] // TODO terrain types
    let terrainStalker terrain = skillFeat 1 Skills.stealth Trained (sprintf "Terrain Stalker (%s)" terrain) 267 []
    let terrifiedRetreat = skillFeat 7 Skills.intimidation Master "Terrified Retreat" 268 []
    let titanWrestler = skillFeat 1 Skills.athletics Trained "Titan Wrestler" 268 []
    let toughness = feat 1 NoReq "Toughness" 268 [
        IncreaseHitPointsPerLevel 1
    ]
    let trainAnimal = skillFeat 1 Skills.nature Trained "Train Animal" 268 []
    let trickMagicItem = feat 1 (hasAnySkill [Skills.arcana; Skills.nature; Skills.occultism; Skills.religion] Trained) "Trick Magic Item" 268 []
    let underwaterMarauder = skillFeat 1 Skills.athletics Trained "Underwater Marauder" 268 []
    let unifiedTheory = skillFeat 15 Skills.arcana Legendary "Unified Theory" 268 []
    let unmistakableLore = feat 2 (LoreReq Expert) "Unmistakable Lore" 268 []
    let untrainedImprovisation = feat 3 NoReq "Untrained Improvisation" 268 [] // TODO implement this bonus
    let virtuoisticPerformer specialty = skillFeat 1 Skills.performance Trained (sprintf "Virtuoistic Performer (%s)" specialty) 268 []
    let wallJump = skillFeat 7 Skills.athletics Master "Wall Jump" 269 []
    let wardMedic = skillFeat 2 Skills.medicine Expert "Ward Medic" 269 []
    let waryDisarmament = skillFeat 2 Skills.thievery Expert "Wary Disarmament" 269 []
    // TODO weapon proficiency on page 269 (difficult, since it optionally requires a parameter)

    // We list skill feats here.
    // TODO include choice feats e.g. Assurance
    let skillFeats = [
        alchemicalCrafting
        arcaneSense
        bargainHunter
        battleCry
        battleMedic
        bizarreMagic
        bondedAnimal
        catFall
        charmingLiar
        cloudJump
        combatClimber
        confabulator
        connections
        continualRecovery
        courtlyGraces
        craftAnything
        divineGuidance
        dubiousKnowledge
        experiencedProfessional
        experiencedSmuggler
        experiencedTracker
        fascinatingPerformance
        foilSenses
        forager
        gladHand
        groupCoercion
        groupImpression
        heftyHauler
        hobnobber
        impeccableCrafting
        impressivePerformance
        intimidatingGlare
        intimidatingProwess
        inventor
        kipUp
        lastingCoercion
        legendaryCodebreaker
        legendaryLinguist
        legendaryMedic
        legendaryNegotiation
        legendaryPerformer
        legendarySneak
        legendarySurvivalist
        legendaryThief
        lengthyDiversion
        lieToMe
        magicalCrafting
        magicalShorthand
        multilingual
        naturalMedicine
        nimbleCrawl
        oddityIdentification
        pickpocket
        planarSurvival
        powerfulLeap
        quickClimb
        quickCoercion
        quickDisguise
        quickIdentification
        quickJump
        quickRecognition
        quickRepair
        quickSqueeze
        quickSwim
        quickUnlock
        quietAllies
        rapidMantel
        readLips
        recognizeSpell
        robustRecovery
        scareToDeath
        shamelessRequest
        signLanguage
        slipperySecrets
        snareCrafting
        steadyBalance
        streetwise
        studentOfTheCanon
        subtleTheft
        surveyWildlife
        swiftSneak
        terrifiedRetreat
        titanWrestler
        trainAnimal
        trickMagicItem
        underwaterMarauder
        unifiedTheory
        unmistakableLore
        wallJump
        wardMedic
        waryDisarmament
    ]

    // We list general feats here (that aren't skill feats; we pull in that list too.)
    // TODO Include the ones that have parameters.  (I haven't yet decided how to implement that.)
    let generalFeats = List.concat [skillFeats; [
        armorProficiencyLight
        armorProficiencyMedium
        armorProficiencyHeavy
        breathControl
        diehard
        expeditiousSearch
        fastRecovery
        featherStep
        fleet
        incredibleInitiative
        incredibleInvestiture
        ride
        shieldBlock
        toughness
        untrainedImprovisation
    ]]