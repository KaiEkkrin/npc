namespace Npc.Classes

open Npc
open Npc.Attributes
open Npc.FeatReq

module ClassBasics =
    // -- CLASS-BUILDING THINGS --

    let modValue ab c =
        Map.find ab c.Abilities
        |> Derive.modifier
        |> (fun m -> m / 1<Modifier>)

    let classFeatWith cl level reqs name page imps =
        let allReqs = ClassReq cl >&& reqs
        Feats.featWith level allReqs name page imps

    let classFeat cl level reqs name page chs =
        let allReqs = ClassReq cl >&& reqs
        Feats.feat level allReqs name page chs

    // I create the class ability boost as a feat like this so that I can
    // look it up later, for those classes that let you choose and change
    // based on what you choose.
    // It also always adds the class DC skill at Trained.
    let classAbilityBoostName cl ability = sprintf "%A (%A)" cl ability
    let classAbilityBoostFeats cl abilities =
        abilities |> List.map (fun a -> classFeatWith cl 1 NoReq (classAbilityBoostName cl a) 0 [
            Improve2.abilityBoost [a] 1
            Improve2.skill (Skills.classSkill (cl, a)) Trained
        ])

    // Increases the character's class skill, whatever it is (there should only be the one) --
    // only for Expert and higher
    let classSkill cl prof =
        let choices =
            [Strength; Dexterity; Constitution; Intelligence; Wisdom; Charisma]
            |> List.map (fun ab -> AddSkill ((Skills.classSkill (cl, ab), prof)))
        {
            Prompt = "Class skill increase"
            Choices = choices
            Count = Some 1
        }
