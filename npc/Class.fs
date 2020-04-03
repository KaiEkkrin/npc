namespace Npc.Classes

open Npc

module All =
    let classes = {
        Prompt = "Class"
        Choices = List.concat [
            Alchemist.alchemist
            Bard.bard
            Champion.champion
            Cleric.cleric
            Druid.druid
            Fighter.fighter
            Monk.monk
            Ranger.ranger
            Rogue.rogue
            Sorcerer.sorcerer
        ]
        Count = Some 1
    }
