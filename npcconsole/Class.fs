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

    // -- ALCHEMIST --

    let alchemistFeats = [
        classFeat Alchemist 1 [] "Alchemical Familiar" 76 []
        classFeat Alchemist 1 [] "Alchemical Savant" 76 []
        classFeat Alchemist 1 [] "Far Lobber" 76 []
        classFeat Alchemist 1 [] "Quick Bomber" 76 []
    ]

    let alchemist c =
        match c.Level with 
        | 1<Level> ->
            { c with Class = Some Alchemist }, [
                Improve.hitPointsPerLevel 8
                Improve.skill Skills.perception Trained
                Improve.skill Skills.fortitudeSave Expert
                Improve.skill Skills.reflexSave Expert
                Improve.skill Skills.willSave Trained
                Improve.skill Skills.crafting Trained
                Improve.skills Skills.regularSkills Trained (modValue Intelligence c)
                Improve.skill (Skills.weaponSkill (SimpleWeapon, Melee)) Trained
                Improve.skill (Skills.weaponSkill (SimpleWeapon, Ranged)) Trained
                Improve.skill (Skills.weaponSkill (Unarmed, Melee)) Trained
                Improve.skill Skills.alchemicalBombs Trained
                Improve.skill (Skills.armorSkill LightArmor) Trained
                Improve.skill (Skills.armorSkill Unarmored) Trained
                Improve.skill (Skills.classSkill (Alchemist, Intelligence)) Trained
                Feats.forceAdd Feats.alchemicalCrafting
                Improve.addFeats alchemistFeats 1
                // TODO add a research field -- I guess that would be a "class feature"
                // and go into a list of named features :)
            ]
        | _ -> failwithf "Bad level: %d" c.Level

    // -- BARBARIAN --

    // -- EVERYTHING --
    let classes = {
        Prompt = "Class"
        Choices = [
            "Alchemist", (fun c -> Option.isNone c.Class), alchemist
        ]
        Count = 1
    }

