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

// Our measure of time
[<Measure>]
type Actions

// These are the different abilities
type Ability = Strength | Dexterity | Constitution | Intelligence | Wisdom | Charisma

// We build character hit points out of a flat bonus and a per-level bonus
type HitPointValue = { Flat: int; PerLevel: int }

// Proficiency ranks are enumerated thus
type ProficiencyRank = Untrained | Trained | Expert | Master | Legendary

// Characters have a size
type Size = Tiny | Small | Medium | Large | Huge | Gargantuan

// Various things have rarity
type Rarity = Common | Uncommon

// And bulk
type Bulk = Insignificant | Light | Heavy of int

// Weapon damage dice are thus
type DieSize = D1 | D4 | D6 | D8 | D10 | D12 | D20

[<StructuredFormatDisplay("AsString")>]
type DamageDice = Dice of Map<DieSize, int> | Varies
with
    member this.AsString = this.ToString ()
    override this.ToString () =
        match this with
        | Dice dice ->
            let separate = dice |> Map.toSeq |> Seq.map (fun (sz, count) -> (sprintf "%d%A" count sz).ToLower ())
            String.Join ("+", separate)
        | Varies -> "Varies"

    static member Of (sz, count) = Dice (Map.empty |> Map.add sz count)
    static member One sz = DamageDice.Of (sz, 1)
    static member Add (sz, count) d =
        match d with
        | Dice dice ->
            match Map.tryFind sz dice with
            | Some c2 -> Dice (dice |> Map.add sz (count + c2))
            | None -> Dice (dice |> Map.add sz count)
        | Varies -> Varies

// This defines a skill.  As well as things officially called "skills", we also
// include some other things that work the same way here:
type Skill = { Name: string; KeyAbility: Ability }

// These are weapons
// "Group" and "Traits" should have matching strings, but I won't enforce this
type WeaponCategory = Unarmed | SimpleWeapon | MartialWeapon | AdvancedWeapon
type WeaponType = Melee | Ranged
type DamageType = Bludgeoning | Piercing | Slashing // TODO augment with electrical, use for resistances too?
type Handedness = OneHanded | TwoHanded
type WeaponTrait = Trait of string | DamageTrait of string * DamageDice | RangeTrait of string * int<Feet> | Versatile of DamageType
type Weapon = {
    Name: string
    Type: WeaponType
    Category: WeaponCategory
    Rarity: Rarity
    Damage: DamageDice
    DamageType: DamageType
    Range: int<Feet> option
    Reload: int<Actions>
    Bulk: Bulk
    Handedness: Handedness
    Group: string
    Traits: WeaponTrait list
}

// These are armors:
type ArmorCategory = LightArmor | MediumArmor | HeavyArmor | Unarmored
type Armor = {
    Name: string
    Skill: Skill
    Bonus: int<Modifier>
    DexCap: int<Modifier> option
    SpeedPenalty: int<Feet>
    Strength: int<Score>
    Group: string option
    Traits: string list
}

// Feats include a page number to help look them up because otherwise it would
// be very annoying!
// The feat list also includes things like class features, that aren't strictly
// feats but do go in the list of Named Things a Character Has.
// TODO Also include the source of feats?  (Makes it easier to understand the character build...)
type Feat = { Name: string; Page: int }

// We enumerate the classes, they're a fixed set.
type Class = Alchemist | Barbarian | Bard | Champion | Cleric | Druid | Fighter | Monk | Ranger | Rogue | Sorcerer | Wizard

// Here's a whole character.  (Various things need to be optional,
// so that we can build them incrementally.)
type Character = {
    Name: string
    Ancestry: string option // TODO heritage as an ancestry choice
    Heritage: string option
    Background: string option
    Class: Class option
    Level: int<Level>
    HitPoints: HitPointValue
    Size: Size option
    Speed: int<Feet>
    Abilities: Map<Ability, int<Score>>
    Skills: Map<Skill, ProficiencyRank>
    Feats: Feat list

    // Different characters can "see" different weapons in different categories!
    // This weapon list is edited to have categories corrected for this character.
    Weapons: Weapon list
    
    // TODO Gear and encumbrance.
    Armor: Armor option
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

    // Calculates a character's hit points
    let hitPoints c =
        let conModifier = Map.find Constitution c.Abilities |> modifier
        c.HitPoints.Flat + (c.HitPoints.PerLevel + conModifier / 1<Modifier>) * c.Level / 1<Level>

    // Calculates a character's armor class
    let armorClass c =
        match c.Armor with
        | None -> 10<Modifier> // expect an armor to be chosen, even if it's "No armor" :)
        | Some a ->
            // Calculate the character's AC bonus, capping Dexterity
            // if required
            let r = rank a.Skill c
            let rawModifier = Map.find a.Skill.KeyAbility c.Abilities |> modifier
            let modifier = match a.DexCap with | Some dc -> min dc rawModifier | None -> rawModifier
            10<Modifier> + a.Bonus + (proficiency r c.Level) + modifier

    // Calculates a character's speed, taking into account any armor penalty
    let speed c =
        match c.Armor with
        | Some a -> c.Speed - a.SpeedPenalty
        | None -> c.Speed
