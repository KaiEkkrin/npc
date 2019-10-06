namespace NpcConsole

open NpcConsole.Attributes

module Skills =
    // Things listed as "proper skills"
    let acrobatics = { Name = "Acrobatics"; KeyAbility = Dexterity }
    let arcana = { Name = "Arcana"; KeyAbility = Intelligence }
    let athletics = { Name = "Athletics"; KeyAbility = Strength }
    let crafting = { Name = "Crafting"; KeyAbility = Intelligence }
    let deception = { Name = "Deception"; KeyAbility = Charisma }
    let diplomacy = { Name = "Diplomacy"; KeyAbility = Charisma }
    let intimidation = { Name = "Intimidation"; KeyAbility = Charisma }
    let lore v = { Name = sprintf "Lore (%s)" v; KeyAbility = Intelligence }
