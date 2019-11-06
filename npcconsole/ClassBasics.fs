namespace NpcConsole.Classes

open NpcConsole
open NpcConsole.Attributes

module ClassBasics =
    // -- CLASS-BUILDING THINGS --

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
