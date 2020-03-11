namespace Npc.Classes

open Npc

module All =
    let classes = {
        Prompt = "Class"
        Choices = List.concat [
            Alchemist.alchemist
            Bard.bard
            Druid.druid
            Ranger.ranger
            Rogue.rogue
        ]
        Count = Some 1
    }
