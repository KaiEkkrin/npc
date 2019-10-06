namespace NpcConsole

open NpcConsole.Attributes

module Feats =
    // Feat categories.
    let special = "Special"

    // Makes a simple feat, without any prerequisites or
    // further improvements.
    let simpleFeat category name = {
        Name = name
        Category = category
        MeetsPrerequisites = fun _ -> true
        Improvements = []
    }

    let darkvision = Improve.specialFeat (simpleFeat special "Darkvision")
    let lowLightVision = Improve.specialFeat (simpleFeat special "Low-light vision")
