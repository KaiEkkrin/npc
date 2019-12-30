namespace Npc.Classes

open Npc

module All =
    let classes = {
        Prompt = "Class"
        Choices = [
            Alchemist.alchemist
            Bard.bard
            Druid.druid
            Ranger.ranger
            Rogue.rogue
        ]
        Count = Some 1
    }
