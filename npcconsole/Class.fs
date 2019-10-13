namespace NpcConsole

open NpcConsole.Attributes

module Classes =
    let modValue ab c =
        Map.find ab c.Abilities
        |> Derive.modifier
        |> (fun m -> m / 1<Modifier>)

    let hitPoints n c = Improve.hitPoints (n + modValue Constitution c)
    let req level cl c = (Improve.hasLevel level c && c.Class = Some cl)

    // -- ALCHEMIST --

    let alchemicalFamiliar = Feats.feat (ClassFeat Alchemist) (req 1 Alchemist) "Alchemical Familiar" []
    let alchemicalSavant = Feats.feat (ClassFeat Alchemist) (req 1 Alchemist) "Alchemical Savant" []
    let farLobber = Feats.feat (ClassFeat Alchemist) (req 1 Alchemist) "Far Lobber" []
    let quickBomber = Feats.feat (ClassFeat Alchemist) (req 1 Alchemist) "Quick Bomber" []

    let alchemistFeats = [
        alchemicalFamiliar
        alchemicalSavant
        farLobber
        quickBomber
    ]

    let alchemist c =
        match c.Level with 
        | 1<Level> ->
            { c with Class = Some Alchemist }, [
                hitPoints 8 c
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

