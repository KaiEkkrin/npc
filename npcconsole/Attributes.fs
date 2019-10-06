namespace NpcConsole.Attributes

open System

// Characters have levels from 1 up
[<Measure>]
type Level

// Ability scores default to 10 and are boosted in 2s
[<Measure>]
type Score

// A modifier is a value to add to a d20 roll
[<Measure>]
type Modifier

// A difficulty class is something you roll a d20 against
[<Measure>]
type DC

// Our typical measure of distance
[<Measure>]
type Feet

// These are the different abilities
type Ability = Strength | Dexterity | Constitution | Intelligence | Wisdom | Charisma

// Proficiency ranks are enumerated thus
type ProficiencyRank = Untrained = 0 | Trained = 2 | Expert = 4 | Master = 6 | Legendary = 8

// Characters have a size
type Size = Tiny | Small | Medium | Large | Huge | Gargantuan

// This defines a skill
// (TODO Add actions?)
type Skill = { Name: string; KeyAbility: Ability }

// This defines a feat -- which any given character may or may not
// qualify for, and which may result in further improvements to
// the character.
// (I'm going to classify special class features as feats too, they
// just won't be in easily acquirable lists.)
type Feat = {
    Name: string
    Category: string
    MeetsPrerequisites: Character -> bool
    Improvements: Improvement list
}

// An ancestry is like this too, except a character can only have one,
// and they have no prerequisites:
and Ancestry = {
    Name: string
    Improvements: Improvement list
}

// TODO include heritages (they don't really affect the character's core
// stats, so I'm leaving them out for now)

// A background is similar too
and Background = {
    Name: string
    Improvements: Improvement list
}

// A class offers improvements at each character level
and Class = {
    Name: string
    Improvements: int<Level> -> Improvement list
}

// When taking a feat, a background, a class, or w/e, the player may
// sometimes choose between a range of different possible improvements
// to their character.  We offer these via an Improvement list, where
// each Improvement includes a list of options (each with a
// description and a change it would make to the character should the
// player accept it) and a number of choices the player can make from them.
// (If the number of choices equals the number of improvements in the
// list, we need no interaction -- the character gets all of them.)
// The functions are:
//  - character -> true if the character qualifies for this improvement,
//    else false
//  - character -> improved character.
and Improvement = {
    Prompt: string
    Choices: (string * (Character -> bool) * (Character -> Character)) list
    Count: int
}

// Here's a whole character.  (Various things need to be optional,
// so that we can build them incrementally.)
and Character = {
    Name: string
    Ancestry: Ancestry option // TODO heritage as an ancestry choice
    Background: Background option
    Class: Class option
    Level: int<Level>
    HitPoints: int
    Size: Size option
    Speed: int<Feet>
    Abilities: Map<Ability, int<Score>>
    Skills: Map<Skill, ProficiencyRank>
    Feats: Feat list

    // This is a list of further improvement lists that have yet to be
    // applied to the character; when it's empty, the character is
    // compelete.
    // Improvements such as feats and ancestries that present further
    // options can add to this.
    // It's a double list because each individual list of improvements is
    // a collection of mutually exclusive things.
    FurtherImprovements: Improvement list list
}

// A helper for deriving stats:
module Derive =
    // Convert an ability score to a modifier:
    let modifier s = s * 1<Modifier> / 2<Score> - 5<Modifier>

    // Convert a modifier to a DC:
    let dc m = 10<DC> + m * 1<DC> / 1<Modifier>

    // Convert a character's level and their proficiency rank to a modifier
    let proficiency rank level =
        let lm = level * 1<Modifier> / 1<Level>
        if rank = 0 then 0<Modifier> else rank * 1<Modifier> + lm

// Describes the interactive UI with the player.
type IInteraction =
    interface

    // Offers the player, under a prompt, a selection of options so that
    // they can pick some number of them (returned).
    abstract member Prompt : string * string list * int -> string list

    // Shows the final character to the player.
    abstract member Show : Character -> unit

    end

// A module of canned improvements.
module Improve =
    // Any single improvement, with no prompt or interaction.
    let single name func = {
        Prompt = name
        Choices = [name, (fun _ -> true), func]
        Count = 1
    }

    // Adds hit points.
    let hitPoints n = single "Hit Points" (fun c -> { c with HitPoints = c.HitPoints + n })

    // Sets size.
    let size sz = single "Size" (fun c -> { c with Size = Some sz })

    // Adds speed.
    let speed n = single "Speed" (fun c -> { c with Speed = c.Speed + n })

    // Adds one or more ability boosts out of the list.
    let ability choices count =
        // An ability boost is +2 unless the ability is >= 18, then it's +1
        let boost score = if score >= 18<Score> then score + 1<Score> else score + 2<Score>
        let boostAbility ab c = { c with Abilities = Map.add ab (Map.find ab c.Abilities |> boost) c.Abilities }
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
    let abilityFlaw ab = single (sprintf "%A flaw" ab) (fun c -> { c with Abilities = Map.add ab ((Map.find ab c.Abilities) - 2<Score>) c.Abilities })

    // Adds ancestries, including queueing up their improvements.
    let ancestries (ass: Ancestry list) = {
        Prompt = "Ancestry"
        Choices = ass |> List.map (fun a -> a.Name, (fun _ -> true), fun c -> { c with Ancestry = Some a; FurtherImprovements = a.Improvements::c.FurtherImprovements })
        Count = 1
    }

    // Adds `count` out of the list of feats (with a prompt qualifier, e.g. "General"), including queueing up their improvements.
    // TODO Be able to filter the list for only the feats the character qualifies for ...
    let feats (fs: Feat list) count qual = {
        Prompt = sprintf "%s Feat" qual
        Choices = fs |> List.map (fun f -> f.Name, (fun c -> f.MeetsPrerequisites c), fun c -> { c with Feats = f::c.Feats; FurtherImprovements = f.Improvements::c.FurtherImprovements })
        Count = count
    }

    // Adds a single special feat automatically (this won't prompt.)
    let specialFeat f = feats [f] 1 "Special"

    // Adds a skill to a character at a particular proficiency.
    let addSkill sk prof c =
        match Map.tryFind sk c.Skills with
        | Some p when p < prof -> { c with Skills = Map.add sk prof c.Skills }
        | _ -> c

    // True if a character already has a skill at a particular proficiency,
    // else false.
    let hasSkill sk prof c =
        match Map.tryFind sk c.Skills with
        | Some p when p >= prof -> true
        | _ -> false

    // Adds a skill from the list at a particular proficiency.
    let skills (sks: Skill list) prof = {
        Prompt = sprintf "%A Skill" prof
        Choices = sks |> List.map (fun sk -> sk.Name, (hasSkill sk prof) >> not, addSkill sk prof)
        Count = 1
    }

    // Adds a skill at a particular proficiency or, if the character,
    // has it already, adds a different skill from the list instead.
    let skillOr sk sks prof =
        let addOr c =
            match Map.tryFind sk c.Skills with
            | Some p when p < prof -> { c with Skills = Map.add sk prof c.Skills }
            | _ -> { c with FurtherImprovements = [skills sks prof]::c.FurtherImprovements }
        {
            Prompt = sprintf "%A %s" prof sk.Name
            Choices = [sk.Name, (fun _ -> true), addOr]
            Count = 1
        }

module Interact =
    // The order to show abilities in.  (Most other things can be alphabetical)
    let abilityOrder = [ Strength; Dexterity; Constitution; Intelligence; Wisdom; Charisma ]

    // Prompts the player for a single improvement and applies it to
    // the character.  Accumulates a set of choices that have been
    // chosen already so they can't be chosen again.
    let promptOne (interact: IInteraction) (seen, c) imp =
        // Filter out our existing choices, and the ones the character
        // doesn't qualify for:
        let fImp = { imp with Choices = imp.Choices |> List.filter (fun (ch, qual, _) -> (qual c) && not (Set.contains ch seen)) }

        // Work out how many to choose.  If it's only as many as there are
        // in the list the choice is forced and we don't need to prompt.
        // (Show them instead?)
        let impCount = List.length fImp.Choices
        let chosen =
            if impCount < fImp.Count then failwithf "Found %A improvements, needed %A\n" impCount (fImp.Count)
            elif impCount = fImp.Count then fImp.Choices
            else
                let choices = fImp.Choices |> List.map (fun (ch, _, _) -> ch)
                let chosen = interact.Prompt (fImp.Prompt, choices, imp.Count)
                fImp.Choices |> List.filter (fun (ch, _, _) -> List.contains ch chosen)
        chosen |> List.fold (fun (seen, c) (ch, _, func) -> (Set.add ch seen, func c)) (seen, c)

    // How to prompt the player for a whole list of improvements.
    let prompt (interact: IInteraction) c imps =
        imps |> List.fold (promptOne interact) (Set.empty<string>, c) |> snd
