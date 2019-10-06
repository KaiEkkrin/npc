namespace NpcConsole

open NpcConsole.Attributes

module Ancestry =
    let ancestries = [
        {
            Ancestry.Name = "Dwarf"
            Improvements = [
                Improve.hitPoints 10;
                Improve.size Medium;
                Improve.speed 20<Feet>;
                Improve.singleAbility Constitution;
                Improve.singleAbility Wisdom;
                Improve.anyAbility 1;
                Improve.abilityFlaw Charisma;
            ]
        };
        {
            Ancestry.Name = "Elf"
            Improvements = [
                Improve.hitPoints 6;
                Improve.size Medium;
                Improve.speed 30<Feet>;
                Improve.singleAbility Dexterity;
                Improve.singleAbility Wisdom;
                Improve.anyAbility 1;
                Improve.abilityFlaw Constitution;
            ]
        }
    ]
