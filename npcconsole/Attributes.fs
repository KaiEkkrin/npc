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
type ProficiencyRank = Untrained | Trained | Expert | Master | Legendary

// This defines a skill
// (TODO Add actions?)
type Skill = { Name: string; KeyAbility: Ability }

// This defines a feat -- which any given character may or may not
// qualify for, and which may result in further improvements to
// the character.
type Feat = {
    Name: string;
    MeetsPrerequisites: Character -> bool
    Improvements: Improvement list
}

// An ancestry is like this too, except a character can only have one,
// and they have no prerequisites:
and Ancestry = {
    Name: string;
    Improvements: Improvement list
}

// A background is similar too
and Background = {
    Name: string;
    Improvements: Improvement list
}

// A class offers improvements at each character level
and Class = {
    Name: string;
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
// The change function also accepts a list of the names of the previous
// choices the player has made while handling the same Improvement list.
and Improvement = {
    Choices: (string * (string list -> Character -> Character)) list
    Count: int
}

// Here's a whole character.  (Various things need to be optional,
// so that we can build them incrementally.)
and Character = {
    Name: string
    Ancestry: Ancestry option
    Background: Background option
    Class: Class option
    Level: int<Level>
    Abilities: Map<Ability, int<Score>>
    Skills: Map<Skill, ProficiencyRank>
    Feats: Feat list
}

// A helper for deriving stats:
module Derive =
    // Convert an ability score to a modifier:
    let modifier s = (s - 1<Score>) * 1<Modifier> / 2<Score>

    // Convert a modifier to a DC:
    let dc m = 10<DC> + m * 1<DC> / 1<Modifier>

    // Convert a character's level and their proficiency rank to a modifier
    let proficiency rank level =
        let lm = level * 1<Modifier> / 1<Level>
        match rank with
            | Untrained -> 0<Modifier>
            | Trained -> 2<Modifier> + lm
            | Expert -> 4<Modifier> + lm
            | Master -> 6<Modifier> + lm
            | Legendary -> 8<Modifier> + lm