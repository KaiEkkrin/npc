namespace Npc

open System

open Npc.Attributes

// Describes how to improve a character, and emit a new one along with
// any further pending improvements.
type Improvement = {
    // The context of this improvement -- what heading to include when asking the
    // user for their choice (if need be)
    Prompt: string

    // A list of the character improvements that could be chosen to apply
    // next, here.  The middle function returns true if the improvement is a
    // valid one to apply to this character, else false.
    Choices: (string * (Character -> bool) * (Character -> Character * Improvement list)) list

    // The number of (different!) improvements that could be chosen at this step.
    Count: int
}

// This module declares some general ways to improve a character.
module Improve =
    
    // An abstract single improvement that requires no interaction.
    // (The name is a bit irrelevant, the player will never see it.)
    let single name func = {
        Prompt = name
        Choices = [name, (fun _ -> true), func]
        Count = 1
    }

    let hitPointsFlat n = single "Hit Points" (fun c -> { c with HitPoints = { c.HitPoints with Flat = c.HitPoints.Flat + n } }, [])
    let hitPointsPerLevel n = single "Hit Points per Level" (fun c -> { c with HitPoints = { c.HitPoints with PerLevel = c.HitPoints.PerLevel + n } }, [])
    let size sz = single "Size" (fun c -> { c with Size = Some sz }, [])
    let speed n = single "Speed" (fun c -> { c with Speed = c.Speed + n }, [])

    // How to require a bunch of things
    let require reqs c = reqs |> List.fold (fun ok r -> ok && (r c)) true

    // How to require a particular character level
    let hasLevel n c = c.Level >= (n * 1<Level>)

    // Level up
    let levelUpTo n = {
        Prompt = sprintf "Level up to %d" n
        Choices = [sprintf "Level up to %d" n, (fun c -> c.Level = n - 1<Level>), (fun c -> { c with Level = n }, [])]
        Count = 1
    }

    // How to require an ability score value or higher
    let hasAbilityScore ab n c = match Map.tryFind ab c.Abilities with | Some score when score >= n -> true | _ -> false

    // Adds one or more ability boosts out of the list.
    let ability choices count =
        // An ability boost is +2 unless the ability is >= 18, then it's +1
        let boost score = if score >= 18<Score> then score + 1<Score> else score + 2<Score>
        let boostAbility ab c = { c with Abilities = Map.add ab (Map.find ab c.Abilities |> boost) c.Abilities }, []
        {
            Prompt = "Ability boost"
            Choices = choices |> List.map (fun ch -> (sprintf "%A boost" ch, (fun _ -> true), boostAbility ch))
            Count = count
        }

    // Helper -- adds a single, fixed ability boost.
    let singleAbility ab = ability [ab] 1

    // Helper -- adds some number of boosts to any (unique) ability.
    let anyAbility count = ability [Strength; Dexterity; Constitution; Intelligence; Wisdom; Charisma] count

    // Adds an ability flaw (always just the one)
    let abilityFlaw ab = single (sprintf "%A flaw" ab) (fun c -> { c with Abilities = Map.add ab ((Map.find ab c.Abilities) - 2<Score>) c.Abilities }, [])

    // True if a character already has a feat with the given name, else false.
    let hasFeat name c = match c.Feats |> List.tryFind (fun f -> f.Name = name) with | Some _ -> true | None -> false

    // True if a character has one of the feats with these names, else false.
    let hasOneFeatOf names c =
        let existingFeats = c.Feats |> List.map (fun f -> f.Name) |> Set.ofList
        let requiredFeats = names |> Set.ofList
        let intersection = Set.intersect existingFeats requiredFeats
        (Set.count intersection) > 0

    // The other way around :)
    let doesNotHaveFeat name c = match c.Feats |> List.tryFind (fun f -> f.Name = name) with | Some _ -> false | None -> true

    // Adds a feat.  If the character already has it, does nothing.
    // (Use for special feats e.g. darkvision granted by ancestries.)
    // Feats are supplied as a tuple (feat, further improvements) because
    // some of them apply further improvements!
    let addFeat (f: Feat, imps) = single f.Name (fun c ->
        if hasFeat f.Name c then c, imps
        else { c with Feats = f::c.Feats }, imps)

    // Adds one or more of the list of (feat, further improvements), assuming
    // the character doesn't have them already and they meet the requirements
    // (`req` returns true).
    let addFeats l count = {
        Prompt = "Feat"
        Choices = l |> List.map (fun (f: Feat, req, imps) ->
            f.Name,
            (fun c -> (doesNotHaveFeat f.Name c) && req c),
            fun c -> { c with Feats = f::c.Feats }, imps)
        Count = count
    }

    // Adds a skill to a character at a particular proficiency.
    let addSkill sk prof c =
        match Map.tryFind sk c.Skills with
        | Some p when p >= prof -> c, []
        | _ -> { c with Skills = Map.add sk prof c.Skills }, []

    // True if a character already has a skill at a particular proficiency,
    // else false.
    let hasSkill sk prof c =
        if prof = Untrained then true
        else match Map.tryFind sk c.Skills with | Some p when p >= prof -> true | _ -> false

    let doesNotHaveSkill sk prof c = not (hasSkill sk prof c)

    let hasAnySkill sks prof c =
        if prof = Untrained then true
        else c.Skills |> Map.tryPick (fun sk v -> if (List.contains sk sks) && v >= prof then Some sk else None) |> Option.isSome

    // True if a character already has *any* Lore skill at a particular proficiency, else false.
    let hasLore prof c =
        if prof = Untrained then true
        else c.Skills |> Map.tryPick (fun sk v -> if sk.Name.Contains "Lore" && v >= prof then Some sk else None) |> Option.isSome

    // Adds one or more skills from the list at a particular proficiency.
    // TODO How to allow input of a custom lore skill?  (Maybe that's just a UI twiddle
    // to how lore skills are rendered and I could do it entirely based on the string?)
    let skills (sks: Skill list) prof count = {
        Prompt = sprintf "%A Skill" prof
        Choices = sks |> List.map (fun sk -> sk.Name, (hasSkill sk prof) >> not, addSkill sk prof)
        Count = count
    }

    // Adds a specific skill at a particular proficiency.
    let skill (sk: Skill) prof = {
        Prompt = sprintf "%A %s" prof sk.Name
        Choices = [sk.Name, (fun _ -> true), addSkill sk prof]
        Count = 1
    }

    // Adds a skill at a particular proficiency or, if the character,
    // has it already, adds a different skill from the list instead.
    let skillOr sk sks prof =
        let addOr c =
            if (Derive.rank sk c) < prof
            then { c with Skills = Map.add sk prof c.Skills }, []
            else c, [skills sks prof 1]
        {
            Prompt = sprintf "%A %s" prof sk.Name
            Choices = [sk.Name, (fun _ -> true), addOr]
            Count = 1
        }

    // Adds a heritage.
    let heritage hs = {
        Prompt = "Heritage"
        Choices = hs
        Count = 1
    }

    // True if a character has an ancestry with a particular name, else false.
    let hasAncestry a c = c.Ancestry = Some a

    // True if a character is of the given class, else false.
    let hasClass cl c = c.Class = Some cl

    // Adds an armor to the character.
    let addArmor (armors: List<Armor>) = {
        Prompt = "Armor"
        Choices =
            armors |> List.map (fun a ->
                a.Name,
                (fun c -> hasAbilityScore Strength a.Strength c && hasSkill a.Skill Trained c),
                (fun c -> { c with Armor = Some a }, [])
            )
        Count = 1
    }

    // Re-categorises this character's weapons; weapons that match the
    // predicate are re-categorised as `cat`.
    let recategorise pred cat = single "Recategorise" (fun c ->
        let rcw = c.Weapons |> List.map (fun w -> if (pred w) then { w with Category = cat } else w)
        { c with Weapons = rcw }, [])

    // Adding a spell skill automatically makes the character Trained in that skill
    let spellSkill sk = single "Spell skill" (fun c -> { c with SpellSkill = Some sk }, [
        skill sk Trained
    ])

    let spell (level, count) = single "Spell level" (fun c ->
        let lv = level * 1<Level>
        let newCount =
            match Map.tryFind lv c.Spells with
            | Some already -> already + count
            | None -> count
        { c with Spells = Map.add lv newCount c.Spells }, [])

    let pool (name, count) = single "Pool" (fun c ->
        let newCount =
            match Map.tryFind name c.Pools with
            | Some already -> already + count
            | None -> count
        { c with Pools = Map.add name newCount c.Pools }, [])