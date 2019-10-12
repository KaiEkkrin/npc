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

    // Define a feat in terms of its (record, requirements, consequent improvements).
    let feat category (req: Character -> bool) name (imps: Improvement list) =
        { Name = name; Category = category },
        req,
        imps

    // Adds a feat to a character even if they don't meet the prereqs.
    let forceAdd (feat, _, imps) = Improve.addFeat (feat, imps)

    // -- SPECIAL FEATS -- Usually applied straight up

    let darkvision = feat SpecialFeat (fun _ -> true) "Darkvision" []
    let keenEyes = feat SpecialFeat (fun _ -> true) "Keen Eyes" []
    let lowLightVision = feat SpecialFeat (fun _ -> true) "Low-light vision" []

    // -- GENERAL FEATS --

    let ride = feat GeneralFeat (fun _ -> true) "Ride" []

    // TODO list general feats here
    let generalFeats = [
        ride
    ]

    // -- SKILL FEATS --
    // TODO fill these in with the necessaries

    let assurance (sk: Skill) = feat SkillFeat (fun _ -> true) (sprintf "Assurance (%s)" sk.Name) []
    let bargainHunter = feat SkillFeat (fun _ -> true) "Bargain Hunter" []
    let battleMedic = feat SkillFeat (fun _ -> true) "Battle Medic" []
    let catFall = feat SkillFeat (fun _ -> true) "Cat Fall" []
    let charmingLiar = feat SkillFeat (fun _ -> true) "Charming Liar" []
    let courtlyGraces = feat SkillFeat (fun _ -> true) "Courtly Graces" []
    let dubiousKnowledge = feat SkillFeat (fun _ -> true) "Dubious Knowledge" []
    let experiencedSmuggler = feat SkillFeat (fun _ -> true) "Experienced Smuggler" []
    let experiencedTracker = feat SkillFeat (fun _ -> true) "Experienced Tracker" []
    let fascinatingPerformance = feat SkillFeat (fun _ -> true) "Fascinating Performance" []
    let forager = feat SkillFeat (fun _ -> true) "Forager" []
    let groupImpression = feat SkillFeat (fun _ -> true) "Group Impression" []
    let heftyHauler (sk: Skill) = feat SkillFeat (fun _ -> true) (sprintf "Hefty Hauler (%s)" sk.Name) []
    let hobnobber = feat SkillFeat (fun _ -> true) "Hobnobber" []
    let impressivePerformance = feat SkillFeat (fun _ -> true) "Impressive Performance" []
    let intimidatingGlare = feat SkillFeat (fun _ -> true) "Intimidating Glare" []
    let lieToMe = feat SkillFeat (fun _ -> true) "Lie to Me" []
    let multilingual = feat SkillFeat (fun _ -> true) "Multilingual" []
    let naturalMedicine = feat SkillFeat (fun _ -> true) "Natural Medicine" []
    let oddityIdentification = feat SkillFeat (fun _ -> true) "Oddity Identification" []
    let pickpocket = feat SkillFeat (fun _ -> true) "Pickpocket" []
    let quickCoercion = feat SkillFeat (fun _ -> true) "Quick Coercion" []
    let specialtyCrafting = feat SkillFeat (fun _ -> true) "Specialty Crafting" []
    let steadyBalance = feat SkillFeat (fun _ -> true) "Steady Balance" []
    let streetwise = feat SkillFeat (fun _ -> true) "Streetwise" []
    let studentOfTheCanon = feat SkillFeat (fun _ -> true) "Student of the Canon" []
    let surveyWildlife = feat SkillFeat (fun _ -> true) "Survey Wildlife" []
    let terrainExpertise terrain = feat SkillFeat (fun _ -> true) (sprintf "Terrain Expertise (%s)" terrain) [] // TODO terrain types
    let trainAnimal = feat SkillFeat (fun _ -> true) "Train Animal" []
    let underwaterMarauder = feat SkillFeat (fun _ -> true) "Underwater Marauder" []

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
