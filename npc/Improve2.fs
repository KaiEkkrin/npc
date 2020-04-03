namespace Npc

open Npc.Attributes

// How to build characters -- serializable version.
module Char2 =
    let canLevelUpTo cl lv c =
        match c.Class, c.Level with
        | Some cl2, lv2 when cl2 = cl && lv2 = lv - 1<Level> -> true
        | _ -> false

    // Adds an ability boost.
    let abilityBoost ab c =
        let boosted =
            let score = Map.find ab c.Abilities
            if score >= 18<Score> then score + 1<Score> else score + 2<Score>
        { c with Abilities = Map.add ab boosted c.Abilities }

    // Adds an ability flaw.
    let abilityFlaw ab c =
        let flawed = (Map.find ab c.Abilities) - 2<Score>
        { c with Abilities = Map.add ab flawed c.Abilities }

    // Adds a skill to a character at a particular proficiency.
    let addSkill sk prof c =
        match Map.tryFind sk c.Skills with
        | Some p when p >= prof -> c
        | _ -> { c with Skills = Map.add sk prof c.Skills }

    // True if a character already has a skill at a particular proficiency,
    // else false.
    let hasSkill sk prof c =
        if prof = Untrained then true
        else match Map.tryFind sk c.Skills with | Some p when p >= prof -> true | _ -> false

    let doesNotHaveSkill sk prof c = not (hasSkill sk prof c)

    let hasAnySkill sks prof c =
        sks |> List.fold (fun any sk -> any || hasSkill sk prof c) false

    let hasAllSkills sks prof c =
        sks |> List.fold (fun all sk -> all && hasSkill sk prof c) true

    // Increases a skill.  How high depends on level:
    let canIncreaseSkill sk prof c =
        match prof, Map.tryFind sk c.Skills with
        | Trained, None -> true // can always gain a new Trained skill
        | Trained, Some Untrained -> true // likewise
        | Expert, Some Trained -> c.Level >= 3<Level>
        | Master, Some Expert -> c.Level >= 7<Level>
        | Legendary, Some Master -> c.Level >= 15<Level>
        | _, _ -> false // can't gain more than one rank at once or anything like that

    let weaponSkill (w: Weapon) = {
        Name = sprintf "%s (%A)" w.Name w.Category
        KeyAbility = match w.Type with | Melee -> Strength | Ranged -> Dexterity // TODO admit finesse weapons
    }

    let armorSkill category = {
        Name = match category with | Unarmored -> "Unarmored" | c -> sprintf "%A armor" c
        KeyAbility = Dexterity // TODO cap (although that is based on the armor itself)
    }

    let classSkill (cl, ab) = {
        Name = sprintf "%A Class" cl
        KeyAbility = ab
    }

    let spellSkill (tradition, ab) = {
        Name = sprintf "%A Spell" tradition
        KeyAbility = ab
    }

    // Filters trained weapon skills from a character.
    let filterWeapons ty c =
        c.Weapons
        |> List.filter (fun w -> w.Type = ty && hasSkill (weaponSkill w) Trained c)

    // Gets the best weapons for a character.
    let bestWeapons ty c =
        let trained = filterWeapons ty c
        let bestCategory = trained |> List.map (fun w -> w.Category) |> List.max
        trained |> List.filter (fun w -> w.Category = bestCategory)

    let addWeapon (w: Weapon) c =
        match w.Type with
        | Melee -> { c with MeleeWeapon = Some w }
        | Ranged -> { c with RangedWeapon = Some w }

    // Re-categorises this character's weapons; weapons that match the
    // predicate are re-categorised as `cat`.
    let recategorise pred cat c =
        let rcw = c.Weapons |> List.map (fun w -> if (pred w) then { w with Category = cat } else w)
        { c with Weapons = rcw }

    let canAddArmor a c =
        let strengthScore = Map.find Strength c.Abilities
        Option.isNone c.Armor && strengthScore >= a.Strength && hasSkill a.Skill Trained c

    let addSpells (lv, ct) c =
        match Map.tryFind lv c.Spells with
        | Some already -> { c with Spells = Map.add lv (ct + already) c.Spells }
        | None -> { c with Spells = Map.add lv ct c.Spells }

    let hasPool p n c =
        match Map.tryFind p c.Pools with
        | Some already when already >= n -> true
        | _ -> false

    let increasePool p n c =
        match Map.tryFind p c.Pools with
        | Some already -> { c with Pools = Map.add p (already + n) c.Pools }
        | None -> { c with Pools = Map.add p n c.Pools }

// Serializable feat requirements are a bit exciting
// TODO This little bit really wants a unit test :P
type FeatRequirement =
    NoReq
    | AncestryReq of string
    | HeritageReq of string
    | ClassReq of Class
    | LevelReq of int<Level>
    | AbilityReq of Ability * int<Score>
    | SkillReq of Skill * ProficiencyRank
    | LoreReq of ProficiencyRank // any lore skill at least this high
    | FeatReq of string
    | PoolReq of string * int // must have at least this many points in the pool
    | NotReq of FeatRequirement
    | OneOfReq of FeatRequirement list // at least one of these
    | AllOfReq of FeatRequirement list

module FeatReq =
    let rec fulfils c r =
        match r with
        | NoReq -> true
        | AncestryReq a -> match c.Ancestry with | Some a2 when a2 = a -> true | _ -> false
        | HeritageReq h -> match c.Heritage with | Some h2 when h2 = h -> true | _ -> false
        | ClassReq cl -> match c.Class with | Some c2 when c2 = cl -> true | _ -> false
        | LevelReq lv -> c.Level >= lv
        | AbilityReq (ab, score) ->
            match Map.tryFind ab c.Abilities with
            | Some sc2 when sc2 >= score -> true
            | _ -> false
        | SkillReq (sk, prof) ->
            match prof, Map.tryFind sk c.Skills with
            | Untrained, _ -> true 
            | _, Some prof2 when prof2 >= prof -> true
            | _ -> false
        | LoreReq prof ->
            match c.Skills |> Map.tryPick (fun sk p -> if (sk.Name.Contains "Lore (") && p >= prof then Some p else None) with
            | Some _ -> true
            | None -> false
        | FeatReq feat ->
            match c.Feats |> List.tryFind (fun f -> f.Name = feat) with
            | Some _ -> true
            | None -> false
        | PoolReq (pool, count) -> Char2.hasPool pool count c
        | NotReq req -> not (fulfils c req)
        | OneOfReq reqs -> reqs |> List.fold (fun ok req -> ok || (fulfils c req)) false
        | AllOfReq reqs -> reqs |> List.fold (fun ok req -> ok && (fulfils c req)) true

    // Some helpers to reduce the amount of parentheses
    let ability ab score = AbilityReq (ab, score)
    let hasSkill sk prof = SkillReq (sk, prof)
    let doesNotHaveSkill sk prof = hasSkill sk prof |> NotReq
    let hasAnySkill sks prof = sks |> List.map (fun sk -> hasSkill sk prof) |> OneOfReq

    // Composes requirements together by OR:
    let (>||) r r2 =
        match r, r2 with
        | OneOfReq reqs, OneOfReq reqs2 -> OneOfReq (reqs @ reqs2)
        | OneOfReq reqs, _ -> OneOfReq (r2::reqs)
        | _, OneOfReq reqs -> OneOfReq (r::reqs)
        | _, _ -> OneOfReq [r; r2]

    // Composes requirements together by AND:
    let (>&&) r r2 =
        match r, r2 with
        | AllOfReq reqs, AllOfReq reqs2 -> AllOfReq (reqs @ reqs2)
        | AllOfReq reqs, _ -> AllOfReq (r2::reqs)
        | _, AllOfReq reqs -> AllOfReq (r::reqs)
        | _, _ -> AllOfReq [r; r2]

// This is an enumeration of the possible changes along with their
// parameterisation.  Thus, we can serialize changes whilst keeping
// the functions that do the changes static
[<StructuredFormatDisplay("{AsString}")>]
type Change2 =
    NoChange of string // string description
    | AddAncestry of string * Improvement2 list
    | AddHeritage of string * Improvement2 list
    | AddBackground of string * Improvement2 list
    | AddClass of Class * Improvement2 list // sets a class at level 1
    | LevelUp of Class * int<Level> * Improvement2 list // levels up a class by 1 level
    | IncreaseHitPointsFlat of int
    | IncreaseHitPointsPerLevel of int
    | AddSize of Size
    | IncreaseSpeed of int<Feet>
    | AbilityBoost of Ability
    | AbilityFlaw of Ability
    | AddSkill of Skill * ProficiencyRank // requires the rank below
    | AddSkillOr of Skill * Skill list * ProficiencyRank // adds the first skill or one of the others if the character has it already
    | AddSkillsBasedOnInt of int * Skill list // adds n + Int Trained skills
    | IncreaseSkill of Skill * ProficiencyRank // requires the rank below and the qualifying level
    | AddWeaponSkills of WeaponCategory * WeaponType * ProficiencyRank
    | AddWeapon of Weapon
    | AddWeaponOfType of WeaponType
    | Recategorise of WeaponCategory * WeaponTrait * WeaponCategory
    | AddArmor of Armor
    | AddFeat of FeatRequirement * Feat * Improvement2 list
    | AddSpellSkill of Skill // adds a spell skill and adds Trained in that skill
    | AddSpell of int<Level> * int // (lv, ct) -> adds ct spell slots at level lv
    | IncreasePool of string * int
with
    member this.AsString =
        match this with
        | NoChange s -> s
        | AddAncestry (n, _) -> n
        | AddHeritage (n, _) -> n
        | AddBackground (n, _) -> n
        | AddClass (n, _) -> sprintf "%A level 1" n
        | LevelUp (n, lv, _) -> sprintf "%A level %d" n lv
        | IncreaseHitPointsFlat p -> sprintf "Increase hit points by %d" p
        | IncreaseHitPointsPerLevel p -> sprintf "Increase hit points by %d per level" p
        | AddSize sz -> sprintf "%A size" sz
        | IncreaseSpeed s -> sprintf "Increase speed by %d feet" s
        | AbilityBoost ab -> sprintf "%A boost" ab
        | AbilityFlaw ab -> sprintf "%A flaw" ab
        | AddSkill (sk, prof) -> sprintf "%A in %s" prof sk.Name
        | AddSkillOr (sk, _, prof) -> sprintf "%A in %s (or another)" prof sk.Name
        | AddSkillsBasedOnInt (n, _) -> sprintf "Add (%d + Int modifier) trained skills" n
        | IncreaseSkill (sk, prof) -> sprintf "%A in %s" prof sk.Name
        | AddWeaponSkills (cat, ty, prof) -> sprintf "%A %A to %A" cat ty prof
        | AddWeapon w -> w.Name
        | AddWeaponOfType ty -> sprintf "%A weapon" ty
        | Recategorise (oldCat, tr, newCat) -> sprintf "Change %A %A weapons to %A" oldCat tr newCat
        | AddArmor a -> a.Name
        | AddFeat (_, f, __) -> f.Name
        | AddSpellSkill sk -> sprintf "Add spell skill %s" sk.Name
        | AddSpell (lv, ct) -> sprintf "Add %d spell slots of level %d" ct lv
        | IncreasePool (p, n) -> sprintf "Increase %s pool by %d" p n

    override this.ToString () = this.AsString

    member this.CanApply (c: Character) =
        match this with
        | AddAncestry _ -> Option.isNone c.Ancestry
        | AddHeritage _ -> Option.isNone c.Heritage
        | AddBackground _ -> Option.isNone c.Background
        | AddClass _ -> Option.isNone c.Class && c.Level = 1<Level>
        | LevelUp (cl, lv, _) -> Char2.canLevelUpTo cl lv c
        | AddSize _ -> Option.isNone c.Size
        | AddSkill (sk, prof) -> Char2.doesNotHaveSkill sk prof c
        | AddSkillOr (sk, sks, prof) -> Char2.hasAllSkills (sk::sks) prof c |> not
        | IncreaseSkill (sk, prof) -> Char2.canIncreaseSkill sk prof c
        | AddWeapon w -> Option.isNone (match w.Type with | Melee -> c.MeleeWeapon | Ranged -> c.RangedWeapon)
        | AddWeaponOfType ty -> Option.isNone (match ty with | Melee -> c.MeleeWeapon | Ranged -> c.RangedWeapon)
        | AddArmor a -> Char2.canAddArmor a c
        | AddFeat (r, _, __) -> FeatReq.fulfils c r
        | AddSpellSkill _ -> Option.isNone c.SpellSkill
        | _ -> true

    member this.Apply (c: Character) =
        match this with
        | NoChange _ -> c, []
        | AddAncestry (a, imps) -> { c with Ancestry = Some a }, imps
        | AddHeritage (h, imps) -> { c with Heritage = Some h }, imps
        | AddBackground (b, imps) -> { c with Background = Some b }, imps
        | AddClass (cl, imps) -> { c with Class = Some cl }, imps
        | LevelUp (_, lv, imps) -> { c with Level = lv }, imps
        | IncreaseHitPointsFlat p ->
            let hp = { c.HitPoints with Flat = c.HitPoints.Flat + p }
            { c with HitPoints = hp }, []
        | IncreaseHitPointsPerLevel p ->
            let hp = { c.HitPoints with PerLevel = c.HitPoints.PerLevel + p }
            { c with HitPoints = hp }, []
        | AddSize sz -> { c with Size = Some sz }, []
        | IncreaseSpeed s -> { c with Speed = c.Speed + s }, []
        | AbilityBoost ab -> Char2.abilityBoost ab c, []
        | AbilityFlaw ab -> Char2.abilityFlaw ab c, []
        | AddSkill (sk, prof) -> Char2.addSkill sk prof c, []
        | AddSkillOr (sk, sks, prof) ->
            if Char2.doesNotHaveSkill sk prof c
            then Char2.addSkill sk prof c, []
            else c, [{
                Prompt = sprintf "%A skill" prof
                Choices = sks |> List.map (fun sk2 -> AddSkill (sk2, prof))
                Count = Some 1
            }]
        | AddSkillsBasedOnInt (n, sks) ->
            let intScore = Map.find Intelligence c.Abilities
            let count = n + (Derive.modifier intScore) / 1<Modifier>
            c, [{
                Prompt = "Trained skill"
                Choices = sks |> List.map (fun sk -> AddSkill (sk, Trained))
                Count = Some count
            }]
        | IncreaseSkill (sk, prof) -> Char2.addSkill sk prof c, []
        | AddWeaponSkills (cat, ty, prof) ->
            c, c.Weapons
            |> List.filter (fun w -> w.Category = cat && w.Type = ty)
            |> List.map (fun w -> {
                Prompt = sprintf "Improve %s skill" w.Name
                Choices = [AddSkill (Char2.weaponSkill w, prof)]
                Count = None
            })
        | AddWeapon w -> Char2.addWeapon w c, []
        | AddWeaponOfType ty ->
            c, [{
                Prompt = this.AsString
                Choices = Char2.bestWeapons ty c |> List.map AddWeapon
                Count = Some 1
            }]
        | Recategorise (oldCat, tr, newCat) ->
            Char2.recategorise (fun w -> w.Category = oldCat && List.contains tr w.Traits) newCat c, []
        | AddArmor a -> { c with Armor = Some a }, []
        | AddFeat (_, f, more) -> { c with Feats = f::c.Feats }, more
        | AddSpellSkill sk -> { c with SpellSkill = Some sk }, [{
            Prompt = sprintf "Trained in %s" sk.Name
            Choices = [AddSkill (sk, Trained)]
            Count = None
        }]
        | AddSpell (lv, ct) -> Char2.addSpells (lv, ct) c, []
        | IncreasePool (p, n) -> Char2.increasePool p n c, []

// An improvement is a collection of changes, one or more of which
// may be applied.
and Improvement2 = {
    // What to prompt the user for (along with the names of the possible changes.)
    Prompt: string

    // A list of the choices that are valid here.
    Choices: Change2 list

    // The number of choices to make, or None to take all that apply (if any)
    Count: int option
}

// Improvement helpers.
module Improve2 =
    // A single change, added if possible.
    let single ch = {
        Prompt = sprintf "%A" ch
        Choices = [ch]
        Count = None
    }

    let hitPointsFlat n = single (IncreaseHitPointsFlat n)
    let hitPointsPerLevel n = single (IncreaseHitPointsPerLevel n)
    let size sz = single (AddSize sz)
    let speed n = single (IncreaseSpeed n)

    let heritage hs = {
        Prompt = "Heritage"
        Choices = hs
        Count = Some 1
    }

    let background bs = {
        Prompt = "Background"
        Choices = bs
        Count = Some 1
    }

    let abilityBoost abilities count = {
        Prompt = "Ability boost"
        Choices = abilities |> List.map AbilityBoost
        Count = Some count
    }

    let anyAbilityBoost = abilityBoost [Strength; Dexterity; Constitution; Intelligence; Wisdom; Charisma]

    let abilityFlaw abilities count = {
        Prompt = "Ability flaw"
        Choices = abilities |> List.map AbilityFlaw
        Count = Some count
    }

    let skill (sk: Skill) prof = {
        Prompt = sprintf "%A in %s" prof sk.Name
        Choices = [AddSkill (sk, prof)]
        Count = None
    }

    let skills (sks: Skill list) prof count = {
        Prompt = sprintf "%A in" prof
        Choices = sks |> List.map (fun sk -> AddSkill (sk, prof))
        Count = Some count
    }

    let skillsBasedOnInt n (sks: Skill list) = {
        Prompt = sprintf "(%d + Int modifier) skills" n
        Choices = [AddSkillsBasedOnInt (n, sks)]
        Count = None
    }

    let feat prompt feats count = {
        Prompt = prompt
        Choices = feats
        Count = Some count
    }
    
    let spellSkill sk = AddSpellSkill sk |> single
    let spell (lv, ct) = AddSpell (lv * 1<Level>, ct) |> single

    let pool (p, n) = {
        Prompt = sprintf "Increase %s pool by %d" p n
        Choices = [IncreasePool (p, n)]
        Count = None
    }