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
        feat GeneralFeat (fun _ -> true) "Ride" []
    ]
