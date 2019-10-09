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

    // Define a feat in terms of its (record, requirements, consequent improvements).
    let feat category req name imps =
        { Name = name; Category = category },
        req,
        imps

    // -- SPECIAL FEATS -- Usually applied straight up

    let darkvision = Improve.addFeat ({ Name = "Darkvision"; Category = SpecialFeat }, [])
    let lowLightVision = Improve.addFeat ({ Name = "Low-light vision"; Category = SpecialFeat }, [])
