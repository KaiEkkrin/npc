namespace NpcConsole

open NpcConsole.Attributes

module Build =
    // How to build a character, by applying all its improvements.
    let rec build interact c =
        match c.FurtherImprovements with
        | [] -> c // done!
        | imp::imps -> Interact.prompt interact { c with FurtherImprovements = imps } imp |> (build interact)

    // A level 1 character, with all their improvements yet to be chosen.
    let start name = {
        Name = name
        Ancestry = None
        Background = None
        Class = None
        Level = 1<Level>
        HitPoints = 0
        Size = None
        Speed = 0<Feet>
        Abilities = Interact.abilityOrder |> List.map (fun a -> a, 10<Score>) |> Map.ofList
        Skills = Map.empty
        Feats = []

        // TODO Add to this list...
        FurtherImprovements = [
            [ Improve.ancestries Ancestry.ancestries ]
        ]
    }
