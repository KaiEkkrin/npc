namespace NpcConsole.Classes

open NpcConsole
open NpcConsole.Attributes

module All =
    // -- BARBARIAN --
    
    // -- CHAMPION --

    // -- CLERIC --

    // -- DRUID --

    // -- FIGHTER --

    // -- MONK --

    // -- RANGER --

    // -- ROGUE --

    // -- SORCERER --

    // -- WIZARD --

    // -- EVERYTHING --
    let hasNoClass c = Option.isNone c.Class
    let hasClass cl c = match c.Class with | Some cl2 when cl2 = cl -> true | _ -> false
    
    let classes = {
        Prompt = "Class"
        Choices = [
            "Alchemist", hasNoClass, Alchemist.alchemist
            "Bard", hasNoClass, Bard.bard
            "Druid", hasNoClass, Druid.druid
            "Ranger", hasNoClass, Ranger.ranger
            "Rogue", hasNoClass, Rogue.rogue
        ]
        Count = 1
    }

    let levelUpTo (n: int<Level>) = {
        Prompt = sprintf "Level up to %d" n
        Choices = [
            "Alchemist", hasClass Alchemist, Alchemist.alchemist
            "Bard", hasClass Bard, Bard.bard
            "Druid", hasClass Druid, Druid.druid
            "Ranger", hasClass Ranger, Ranger.ranger
            "Rogue", hasClass Rogue, Rogue.rogue
        ]
        Count = 1
    }
