namespace NpcConsole

open NpcConsole.Attributes

module Weapons =
    // -- WEAPON GROUPS --
    let axe = "Axe"
    let bomb = "Bomb"
    let bow = "Bow"
    let brawling = "Brawling"
    let club = "Club"
    let dart = "Dart"
    let flail = "Flail"
    let hammer = "Hammer"
    let knife = "Knife"
    let pick = "Pick"
    let polearm = "Polearm"
    let shield = "Shield"
    let sling = "Sling"
    let spear = "Spear"
    let sword = "Sword"

    // -- WEAPON TRAITS --
    let agile = Trait "Agile"
    let attached = Trait "Attached to shield"
    let backstabber = Trait "Backstabber"
    let backswing = Trait "Backswing"
    let deadly die = DamageTrait ("Deadly", DamageDice.One die)
    let disarm = Trait "Disarm"
    let dwarf = Trait "Dwarf"
    let elf = Trait "Elf"
    let fatal die = DamageTrait ("Fatal", DamageDice.One die)
    let finesse = Trait "Finesse"
    let forceful = Trait "Forceful"
    let freeHand = Trait "Free-hand"
    let gnome = Trait "Gnome"
    let goblin = Trait "Goblin"
    let halfling = Trait "Halfling"
    let jousting die = DamageTrait ("Jousting", DamageDice.One die)
    let monk = Trait "Monk"
    let nonlethal = Trait "Nonlethal"
    let orc = Trait "Orc"
    let parry = Trait "Parry"
    let propulsive = Trait "Propulsive"
    let reach = Trait "Reach"
    let shove = Trait "Shove"
    let sweep = Trait "Sweep"
    let thrown distance = RangeTrait ("Thrown", distance)
    let trip = Trait "Trip"
    let twin = Trait "Twin"
    let twoHand die = DamageTrait ("Two-hand", DamageDice.One die)
    let unarmed = Trait "Unarmed"
    let volley range = RangeTrait ("Volley", range)

    // -- SIMPLE MELEE WEAPONS --
    let simpleMelee (name, die, damTy, bulk, hands, group, traits) = {
        Name = name
        Type = Melee
        Category = SimpleWeapon
        Rarity = Common
        Damage = DamageDice.One die
        DamageType = damTy
        Range = None
        Reload = 0<Actions>
        Bulk = bulk
        Handedness = hands
        Group = group
        Traits = traits
    }

    let simpleMeleeWeapons = [
        simpleMelee ("Club", D6, Bludgeoning, Heavy 1, OneHanded, club, [thrown 10<Feet>])
        simpleMelee ("Dagger", D4, Piercing, Light, OneHanded, knife, [agile; finesse; thrown 10<Feet>; Versatile Slashing])
        simpleMelee ("Gauntlet", D4, Bludgeoning, Light, OneHanded, brawling, [agile; freeHand])
        simpleMelee ("Light mace", D4, Bludgeoning, Light, OneHanded, club, [agile; finesse; shove])
        simpleMelee ("Longspear", D8, Piercing, Heavy 2, TwoHanded, spear, [reach])
        simpleMelee ("Mace", D6, Bludgeoning, Heavy 1, OneHanded, club, [shove])
        simpleMelee ("Morningstar", D6, Bludgeoning, Heavy 1, OneHanded, club, [Versatile Piercing])
        simpleMelee ("Sickle", D4, Slashing, Light, OneHanded, knife, [agile; finesse; trip])
        simpleMelee ("Spear", D6, Piercing, Heavy 1, OneHanded, spear, [thrown 20<Feet>])
        simpleMelee ("Spiked gauntlet", D4, Piercing, Light, OneHanded, brawling, [agile; freeHand])
        simpleMelee ("Staff", D4, Bludgeoning, Heavy 1, OneHanded, club, [twoHand D8])
    ]

    let uncommonSimpleMelee x = { (simpleMelee x) with Rarity = Uncommon }

    let uncommonSimpleMeleeWeapons = [
        uncommonSimpleMelee ("Clan dagger", D4, Piercing, Light, OneHanded, knife, [agile; dwarf; parry; Versatile Bludgeoning])
        uncommonSimpleMelee ("Katar", D4, Piercing, Light, OneHanded, knife, [agile; deadly D6])
    ]

    let simpleRanged (name, die, damTy, range, reload, bulk, hands, group, traits) = {
        Name = name
        Type = Ranged
        Category = SimpleWeapon
        Rarity = Common
        Damage = DamageDice.One die
        DamageType = damTy
        Range = Some (range * 1<Feet>)
        Reload = reload * 1<Actions>
        Bulk = bulk
        Handedness = hands
        Group = group
        Traits = traits
    }

    let simpleRangedWeapons = [
        simpleRanged ("Blowgun", D1, Piercing, 20, 1, Light, OneHanded, dart, [agile; nonlethal])
        simpleRanged ("Crossbow", D8, Piercing, 120, 1, Heavy 1, TwoHanded, bow, [])
        simpleRanged ("Dart", D4, Piercing, 20, 0, Light, OneHanded, dart, [agile; thrown 20<Feet>])
        simpleRanged ("Hand crossbow", D6, Piercing, 60, 1, Light, OneHanded, bow, [])
        simpleRanged ("Heavy crossbow", D10, Piercing, 120, 2, Heavy 2, TwoHanded, bow, [])
        simpleRanged ("Javelin", D6, Piercing, 30, 0, Light, OneHanded, dart, [thrown 30<Feet>])
        simpleRanged ("Sling", D6, Bludgeoning, 50, 1, Light, OneHanded, sling, [propulsive])
    ]

    // -- MARTIAL WEAPONS --
    let martialMelee x = { (simpleMelee x) with Category = MartialWeapon }

    let martialMeleeWeapons = [
        martialMelee ("Bastard sword", D8, Slashing, Heavy 1, OneHanded, sword, [twoHand D12])
        martialMelee ("Battle axe", D8, Slashing, Heavy 1, OneHanded, axe, [sweep])
        martialMelee ("Bo staff", D8, Bludgeoning, Heavy 2, TwoHanded, club, [monk; parry; reach; trip])
        martialMelee ("Falchion", D10, Slashing, Heavy 2, TwoHanded, sword, [forceful; sweep])
        martialMelee ("Flail", D6, Bludgeoning, Heavy 1, OneHanded, flail, [disarm; sweep; trip])
        martialMelee ("Glaive", D8, Slashing, Heavy 2, TwoHanded, polearm, [deadly D8; forceful; reach])
        martialMelee ("Greataxe", D12, Slashing, Heavy 2, TwoHanded, axe, [sweep])
        martialMelee ("Greatclub", D10, Bludgeoning, Heavy 2, TwoHanded, club, [backswing; shove])
        martialMelee ("Greatpick", D10, Piercing, Heavy 2, TwoHanded, pick, [fatal D12])
        martialMelee ("Greatsword", D12, Slashing, Heavy 2, TwoHanded, sword, [Versatile Piercing])
        martialMelee ("Guisarme", D10, Slashing, Heavy 2, TwoHanded, polearm, [reach; trip])
        martialMelee ("Halberd", D10, Slashing, Heavy 2, TwoHanded, polearm, [reach; Versatile Slashing])
        martialMelee ("Hatchet", D6, Slashing, Light, OneHanded, axe, [agile; sweep; thrown 10<Feet>])
        martialMelee ("Lance", D8, Piercing, Heavy 2, TwoHanded, spear, [deadly D8; jousting D6; reach])
        martialMelee ("Light hammer", D6, Bludgeoning, Light, OneHanded, hammer, [agile; thrown 20<Feet>])
        martialMelee ("Light pick", D4, Piercing, Light, OneHanded, pick, [agile; fatal D8])
        martialMelee ("Longsword", D8, Slashing, Heavy 1, OneHanded, sword, [Versatile Piercing])
        martialMelee ("Main-gauche", D4, Piercing, Light, OneHanded, knife, [agile; disarm; finesse; parry; Versatile Slashing])
        martialMelee ("Maul", D12, Bludgeoning, Heavy 2, TwoHanded, hammer, [shove])
        martialMelee ("Pick", D6, Piercing, Heavy 1, OneHanded, pick, [fatal D10])
        martialMelee ("Ranseur", D10, Piercing, Heavy 2, TwoHanded, polearm, [disarm; reach])
        martialMelee ("Rapier", D6, Piercing, Heavy 1, OneHanded, sword, [deadly D8; disarm; finesse])
        martialMelee ("Sap", D6, Bludgeoning, Light, OneHanded, club, [agile; nonlethal])
        martialMelee ("Scimitar", D6, Slashing, Heavy 1, OneHanded, sword, [forceful; sweep])
        martialMelee ("Scythe", D10, Slashing, Heavy 2, TwoHanded, polearm, [deadly D10; trip])
        // TODO Shield abilities only shown when a shield is equipped
        martialMelee ("Shield bash", D4, Bludgeoning, Insignificant, OneHanded, shield, [])
        martialMelee ("Shield boss", D6, Bludgeoning, Insignificant, OneHanded, shield, [attached])
        martialMelee ("Shield spikes", D6, Slashing, Insignificant, OneHanded, shield, [attached])
        martialMelee ("Shortsword", D6, Piercing, Light, OneHanded, sword, [agile; finesse; Versatile Slashing])
        martialMelee ("Starknife", D4, Piercing, Light, OneHanded, knife, [agile; deadly D6; finesse; thrown 20<Feet>; Versatile Slashing])
        martialMelee ("Trident", D8, Piercing, Heavy 1, OneHanded, spear, [thrown 20<Feet>])
        martialMelee ("War flail", D10, Bludgeoning, Heavy 2, TwoHanded, flail, [disarm; sweep; trip])
        martialMelee ("Warhammer", D8, Bludgeoning, Heavy 1, OneHanded, hammer, [shove])
        martialMelee ("Whip", D4, Slashing, Heavy 1, OneHanded, flail, [disarm; finesse; nonlethal; reach; trip])
    ]

    let uncommonMartialMelee x = { (martialMelee x) with Rarity = Uncommon }

    let uncommonMartialMeleeWeapons = [
        uncommonMartialMelee ("Dogslicer", D6, Slashing, Light, OneHanded, sword, [agile; backstabber; finesse; goblin])
        uncommonMartialMelee ("Elven curve blade", D8, Slashing, Heavy 2, TwoHanded, sword, [elf; finesse; forceful])
        uncommonMartialMelee ("Filcher's fork", D4, Piercing, Light, OneHanded, spear, [agile; backstabber; deadly D6; finesse; halfling; thrown 20<Feet>])
        uncommonMartialMelee ("Gnome hooked hammer", D6, Bludgeoning, Heavy 1, OneHanded, hammer, [gnome; trip; twoHand D10; Versatile Piercing])
        uncommonMartialMelee ("Horsechopper", D8, Slashing, Heavy 2, TwoHanded, polearm, [goblin; reach; trip; Versatile Piercing])
        uncommonMartialMelee ("Kama", D6, Slashing, Light, OneHanded, knife, [agile; monk; trip])
        uncommonMartialMelee ("Katana", D6, Slashing, Heavy 1, OneHanded, sword, [deadly D8; twoHand D10; Versatile Piercing])
        uncommonMartialMelee ("Kukri", D6, Slashing, Light, OneHanded, knife, [agile; finesse; trip])
        uncommonMartialMelee ("Nunchaku", D6, Bludgeoning, Light, OneHanded, club, [backswing; disarm; finesse; monk])
        uncommonMartialMelee ("Orc knuckle dagger", D6, Piercing, Light, OneHanded, knife, [agile; disarm; orc])
        uncommonMartialMelee ("Sai", D4, Piercing, Light, OneHanded, knife, [agile; disarm; finesse; monk; Versatile Bludgeoning])
        uncommonMartialMelee ("Spiked chain", D8, Slashing, Heavy 1, TwoHanded, flail, [disarm; finesse; trip])
        uncommonMartialMelee ("Temple sword", D8, Slashing, Heavy 1, OneHanded, sword, [monk; trip])
    ]

    let martialRanged x = { (simpleRanged x) with Category = MartialWeapon }

    let martialRangedWeapons = [
        {
            Name = "Alchemical bomb"
            Type = Ranged
            Category = MartialWeapon
            Rarity = Common
            Damage = Varies
            DamageType = Piercing // TODO fix; cba right now :)
            Range = Some 20<Feet>
            Reload = 0<Actions>
            Bulk = Light
            Handedness = OneHanded
            Group = bomb
            Traits = []
        }
        martialRanged ("Composite longbow", D8, Piercing, 100, 0, Heavy 2, TwoHanded, bow, [deadly D10; propulsive; volley 30<Feet>])
        martialRanged ("Composite shortbow", D6, Piercing, 60, 0, Heavy 1, TwoHanded, bow, [deadly D10; propulsive])
        martialRanged ("Longbow", D8, Piercing, 100, 0, Heavy 2, TwoHanded, bow, [deadly D10; volley 30<Feet>])
        martialRanged ("Shortbow", D6, Piercing, 60, 0, Heavy 1, TwoHanded, bow, [deadly D10])
    ]

    let uncommonMartialRanged x = { (martialRanged x) with Rarity = Uncommon }

    let uncommonMartialRangedWeapons = [
        uncommonMartialRanged ("Halfling sling staff", D10, Bludgeoning, 80, 1, Heavy 1, TwoHanded, sling, [halfling; propulsive])
        uncommonMartialRanged ("Shuriken", D4, Piercing, 20, 0, Insignificant, OneHanded, dart, [agile; monk; thrown 20<Feet>])
    ]

    // -- ADVANCED WEAPONS --
    let advancedMelee x = { (martialMelee x) with Category = AdvancedWeapon }

    let advancedMeleeWeapons = [
        advancedMelee ("Dwarven waraxe", D8, Slashing, Heavy 2, OneHanded, axe, [dwarf; sweep; twoHand D12])
        advancedMelee ("Gnome flickmace", D8, Bludgeoning, Heavy 2, OneHanded, flail, [gnome; reach])
        advancedMelee ("Orc necksplitter", D8, Slashing, Heavy 1, OneHanded, axe, [forceful; orc; sweep])
        advancedMelee ("Sawtooth saber", D6, Slashing, Light, OneHanded, sword, [agile; finesse; twin])
    ]

    // -- LISTS --
    let all = [
        // -- UNARMED --
        yield {
            Name = "Fist"
            Type = Melee
            Category = Unarmed
            Rarity = Common
            Damage = DamageDice.One D4
            DamageType = Bludgeoning
            Range = None
            Reload = 0<Actions>
            Bulk = Insignificant
            Handedness = OneHanded
            Group = brawling
            Traits = [agile; finesse; nonlethal; unarmed]
        }

        yield! simpleMeleeWeapons
        yield! uncommonSimpleMeleeWeapons
        yield! martialMeleeWeapons
        yield! uncommonMartialMeleeWeapons
        yield! advancedMeleeWeapons
        yield! simpleRangedWeapons
        yield! martialRangedWeapons
        yield! uncommonMartialRangedWeapons
    ]
