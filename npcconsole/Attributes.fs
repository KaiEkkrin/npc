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

// Characters have a size
type Size = Tiny | Small | Medium | Large | Huge | Gargantuan

// Various things have rarity
type Rarity = Common | Uncommon

// These are weapons
// "Group" and "Traits" should have matching strings, but I won't enforce this
type WeaponCategory = Unarmed | SimpleWeapon | MartialWeapon | AdvancedWeapon
type WeaponType = Melee | Ranged
type Weapon = {
    Name: string
    Type: WeaponType
    Category: WeaponCategory
    Rarity: Rarity
    Damage: string // XdY
    Group: string
    Traits: string list
}

// These are armors:
type ArmorCategory = LightArmor | MediumArmor | HeavyArmor | Unarmored

// This defines a skill.  As well as things officially called "skills", we also
// include some other things that work the same way here:
type SkillType = RegularSkill | ArmorProficiency | WeaponProficiency | Perception | SavingThrow | ClassSkill
type Skill = { Name: string; Type: SkillType; KeyAbility: Ability }

// This defines a feat -- which any given character may or may not
// qualify for, and which may result in further improvements to
// the character.
// (I'm going to classify special class features as feats too, they
// just won't be in easily acquirable lists.)
type Class = Alchemist | Barbarian | Bard | Champion | Cleric | Druid | Fighter | Monk | Ranger | Rogue | Sorcerer | Wizard
type FeatCategory = AncestryFeat | ClassFeat of Class | GeneralFeat | SkillFeat | SpecialFeat
type Feat = { Name: string; Category: FeatCategory }

// Here's a whole character.  (Various things need to be optional,
// so that we can build them incrementally.)
type Character = {
    Name: string
    Ancestry: string option // TODO heritage as an ancestry choice
    Heritage: string option
    Background: string option
    Class: Class option
    Level: int<Level>
    HitPoints: int
    Size: Size option
    Speed: int<Feet>
    Abilities: Map<Ability, int<Score>>
    Skills: Map<Skill, ProficiencyRank>
    Feats: Feat list

    // TODO Include the weapons as they apply to this character.  Different
    // characters can "see" different weapons in different categories!
    
    // TODO Gear.
}

// A helper for deriving stats:
module Derive =
    // Convert an ability score to a modifier:
    let modifier s = s * 1<Modifier> / 2<Score> - 5<Modifier>

    // Convert a modifier to a DC:
    let dc m = 10<DC> + m * 1<DC> / 1<Modifier>

    // Gets a character's skill rank
    let rank sk c = match Map.tryFind sk c.Skills with | Some p -> p | None -> ProficiencyRank.Untrained

    // Convert a proficiency rank to the bonus it applies
    let proficiencyBonus (rank: ProficiencyRank) =
        match rank with
        | Untrained -> 0<Modifier>
        | Trained -> 2<Modifier>
        | Expert -> 4<Modifier>
        | Master -> 6<Modifier>
        | Legendary -> 8<Modifier>

    // Convert a character's level and their proficiency rank to a modifier
    let proficiency (rank: ProficiencyRank) level =
        let lm = level * 1<Modifier> / 1<Level>
        let pb = proficiencyBonus rank
        match rank with
        | Untrained -> pb // no level modifier for untrained
        | _ -> lm + pb

    // Calculates the total bonus for a character's skill
    let bonus sk c =
        let r = rank sk c
        let m = Map.find sk.KeyAbility c.Abilities |> modifier
        (proficiency r c.Level) + m
