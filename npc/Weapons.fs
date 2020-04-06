namespace Npc

open Npc.Attributes

module groups =
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

module Weapons =
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

    let club = simpleMelee ("Club", D6, Bludgeoning, Heavy 1, OneHanded, groups.club, [thrown 10<Feet>])
    let dagger = simpleMelee ("Dagger", D4, Piercing, Light, OneHanded, groups.knife, [agile; finesse; thrown 10<Feet>; Versatile Slashing])
    let staff = simpleMelee ("Staff", D4, Bludgeoning, Heavy 1, OneHanded, groups.club, [twoHand D8])

    let simpleMeleeWeapons = [
        club
        dagger
        simpleMelee ("Light mace", D4, Bludgeoning, Light, OneHanded, groups.club, [agile; finesse; shove])
        simpleMelee ("Longspear", D8, Piercing, Heavy 2, TwoHanded, groups.spear, [reach])
        simpleMelee ("Mace", D6, Bludgeoning, Heavy 1, OneHanded, groups.club, [shove])
        simpleMelee ("Morningstar", D6, Bludgeoning, Heavy 1, OneHanded, groups.club, [Versatile Piercing])
        simpleMelee ("Sickle", D4, Slashing, Light, OneHanded, groups.knife, [agile; finesse; trip])
        simpleMelee ("Spear", D6, Piercing, Heavy 1, OneHanded, groups.spear, [thrown 20<Feet>])
        simpleMelee ("Spiked gauntlet", D4, Piercing, Light, OneHanded, groups.brawling, [agile; freeHand])
        staff
    ]

    let uncommonSimpleMelee x = { (simpleMelee x) with Rarity = Uncommon }

    let uncommonSimpleMeleeWeapons = [
        uncommonSimpleMelee ("Clan dagger", D4, Piercing, Light, OneHanded, groups.knife, [agile; dwarf; parry; Versatile Bludgeoning])
        uncommonSimpleMelee ("Katar", D4, Piercing, Light, OneHanded, groups.knife, [agile; deadly D6])
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

    let crossbow = simpleRanged ("Crossbow", D8, Piercing, 120, 1, Heavy 1, TwoHanded, groups.bow, [])
    let heavyCrossbow = simpleRanged ("Heavy crossbow", D10, Piercing, 120, 2, Heavy 2, TwoHanded, groups.bow, [])
    let sling = simpleRanged ("Sling", D6, Bludgeoning, 50, 1, Light, OneHanded, groups.sling, [propulsive])

    let simpleRangedWeapons = [
        simpleRanged ("Blowgun", D1, Piercing, 20, 1, Light, OneHanded, groups.dart, [agile; nonlethal])
        crossbow
        simpleRanged ("Dart", D4, Piercing, 20, 0, Light, OneHanded, groups.dart, [agile; thrown 20<Feet>])
        simpleRanged ("Hand crossbow", D6, Piercing, 60, 1, Light, OneHanded, groups.bow, [])
        heavyCrossbow
        simpleRanged ("Javelin", D6, Piercing, 30, 0, Light, OneHanded, groups.dart, [thrown 30<Feet>])
        sling
    ]

    // -- MARTIAL WEAPONS --
    let martialMelee x = { (simpleMelee x) with Category = MartialWeapon }

    let battleAxe = martialMelee ("Battle axe", D8, Slashing, Heavy 1, OneHanded, groups.axe, [sweep])
    let glaive = martialMelee ("Glaive", D8, Slashing, Heavy 2, TwoHanded, groups.polearm, [deadly D8; forceful; reach])
    let longsword = martialMelee ("Longsword", D8, Slashing, Heavy 1, OneHanded, groups.sword, [Versatile Piercing])
    let pick = martialMelee ("Pick", D6, Piercing, Heavy 1, OneHanded, groups.pick, [fatal D10])
    let rapier = martialMelee ("Rapier", D6, Piercing, Heavy 1, OneHanded, groups.sword, [deadly D8; disarm; finesse])
    let sap = martialMelee ("Sap", D6, Bludgeoning, Light, OneHanded, groups.club, [agile; nonlethal])
    let shortsword = martialMelee ("Shortsword", D6, Piercing, Light, OneHanded, groups.sword, [agile; finesse; Versatile Slashing])
    let warhammer = martialMelee ("Warhammer", D8, Bludgeoning, Heavy 1, OneHanded, groups.hammer, [shove])
    let whip = martialMelee ("Whip", D4, Slashing, Heavy 1, OneHanded, groups.flail, [disarm; finesse; nonlethal; reach; trip])

    let martialMeleeWeapons = [
        martialMelee ("Bastard sword", D8, Slashing, Heavy 1, OneHanded, groups.sword, [twoHand D12])
        battleAxe
        martialMelee ("Bo staff", D8, Bludgeoning, Heavy 2, TwoHanded, groups.club, [monk; parry; reach; trip])
        martialMelee ("Falchion", D10, Slashing, Heavy 2, TwoHanded, groups.sword, [forceful; sweep])
        martialMelee ("Flail", D6, Bludgeoning, Heavy 1, OneHanded, groups.flail, [disarm; sweep; trip])
        glaive
        martialMelee ("Greataxe", D12, Slashing, Heavy 2, TwoHanded, groups.axe, [sweep])
        martialMelee ("Greatclub", D10, Bludgeoning, Heavy 2, TwoHanded, groups.club, [backswing; shove])
        martialMelee ("Greatpick", D10, Piercing, Heavy 2, TwoHanded, groups.pick, [fatal D12])
        martialMelee ("Greatsword", D12, Slashing, Heavy 2, TwoHanded, groups.sword, [Versatile Piercing])
        martialMelee ("Guisarme", D10, Slashing, Heavy 2, TwoHanded, groups.polearm, [reach; trip])
        martialMelee ("Halberd", D10, Slashing, Heavy 2, TwoHanded, groups.polearm, [reach; Versatile Slashing])
        martialMelee ("Hatchet", D6, Slashing, Light, OneHanded, groups.axe, [agile; sweep; thrown 10<Feet>])
        martialMelee ("Lance", D8, Piercing, Heavy 2, TwoHanded, groups.spear, [deadly D8; jousting D6; reach])
        martialMelee ("Light hammer", D6, Bludgeoning, Light, OneHanded, groups.hammer, [agile; thrown 20<Feet>])
        martialMelee ("Light pick", D4, Piercing, Light, OneHanded, groups.pick, [agile; fatal D8])
        longsword
        martialMelee ("Main-gauche", D4, Piercing, Light, OneHanded, groups.knife, [agile; disarm; finesse; parry; Versatile Slashing])
        martialMelee ("Maul", D12, Bludgeoning, Heavy 2, TwoHanded, groups.hammer, [shove])
        pick
        martialMelee ("Ranseur", D10, Piercing, Heavy 2, TwoHanded, groups.polearm, [disarm; reach])
        rapier
        sap
        martialMelee ("Scimitar", D6, Slashing, Heavy 1, OneHanded, groups.sword, [forceful; sweep])
        martialMelee ("Scythe", D10, Slashing, Heavy 2, TwoHanded, groups.polearm, [deadly D10; trip])
        // TODO Shield abilities only shown when a shield is equipped
        martialMelee ("Shield bash", D4, Bludgeoning, Insignificant, OneHanded, groups.shield, [])
        martialMelee ("Shield boss", D6, Bludgeoning, Insignificant, OneHanded, groups.shield, [attached])
        martialMelee ("Shield spikes", D6, Slashing, Insignificant, OneHanded, groups.shield, [attached])
        shortsword
        martialMelee ("Starknife", D4, Piercing, Light, OneHanded, groups.knife, [agile; deadly D6; finesse; thrown 20<Feet>; Versatile Slashing])
        martialMelee ("Trident", D8, Piercing, Heavy 1, OneHanded, groups.spear, [thrown 20<Feet>])
        martialMelee ("War flail", D10, Bludgeoning, Heavy 2, TwoHanded, groups.flail, [disarm; sweep; trip])
        warhammer
        whip
    ]

    let uncommonMartialMelee x = { (martialMelee x) with Rarity = Uncommon }

    let dogslicer = uncommonMartialMelee ("Dogslicer", D6, Slashing, Light, OneHanded, groups.sword, [agile; backstabber; finesse; goblin])
    let horsechopper = uncommonMartialMelee ("Horsechopper", D8, Slashing, Heavy 2, TwoHanded, groups.polearm, [goblin; reach; trip; Versatile Piercing])
    let kukri = uncommonMartialMelee ("Kukri", D6, Slashing, Light, OneHanded, groups.knife, [agile; finesse; trip])

    let uncommonMartialMeleeWeapons = [
        dogslicer
        uncommonMartialMelee ("Elven curve blade", D8, Slashing, Heavy 2, TwoHanded, groups.sword, [elf; finesse; forceful])
        uncommonMartialMelee ("Filcher's fork", D4, Piercing, Light, OneHanded, groups.spear, [agile; backstabber; deadly D6; finesse; halfling; thrown 20<Feet>])
        uncommonMartialMelee ("Gnome hooked hammer", D6, Bludgeoning, Heavy 1, OneHanded, groups.hammer, [gnome; trip; twoHand D10; Versatile Piercing])
        horsechopper
        uncommonMartialMelee ("Kama", D6, Slashing, Light, OneHanded, groups.knife, [agile; monk; trip])
        uncommonMartialMelee ("Katana", D6, Slashing, Heavy 1, OneHanded, groups.sword, [deadly D8; twoHand D10; Versatile Piercing])
        kukri
        uncommonMartialMelee ("Nunchaku", D6, Bludgeoning, Light, OneHanded, groups.club, [backswing; disarm; finesse; monk])
        uncommonMartialMelee ("Orc knuckle dagger", D6, Piercing, Light, OneHanded, groups.knife, [agile; disarm; orc])
        uncommonMartialMelee ("Sai", D4, Piercing, Light, OneHanded, groups.knife, [agile; disarm; finesse; monk; Versatile Bludgeoning])
        uncommonMartialMelee ("Spiked chain", D8, Slashing, Heavy 1, TwoHanded, groups.flail, [disarm; finesse; trip])
        uncommonMartialMelee ("Temple sword", D8, Slashing, Heavy 1, OneHanded, groups.sword, [monk; trip])
    ]

    let martialRanged x = { (simpleRanged x) with Category = MartialWeapon }

    let alchemicalBomb = 
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
            Group = groups.bomb
            Traits = []
        }
    let compositeLongbow = martialRanged ("Composite longbow", D8, Piercing, 100, 0, Heavy 2, TwoHanded, groups.bow, [deadly D10; propulsive; volley 30<Feet>])
    let compositeShortbow = martialRanged ("Composite shortbow", D6, Piercing, 60, 0, Heavy 1, TwoHanded, groups.bow, [deadly D10; propulsive])
    let longbow = martialRanged ("Longbow", D8, Piercing, 100, 0, Heavy 2, TwoHanded, groups.bow, [deadly D10; volley 30<Feet>])
    let shortbow = martialRanged ("Shortbow", D6, Piercing, 60, 0, Heavy 1, TwoHanded, groups.bow, [deadly D10])

    let martialRangedWeapons = [
        alchemicalBomb
        compositeLongbow
        compositeShortbow
        longbow
        shortbow
    ]

    let uncommonMartialRanged x = { (martialRanged x) with Rarity = Uncommon }

    let halflingSlingStaff = uncommonMartialRanged ("Halfling sling staff", D10, Bludgeoning, 80, 1, Heavy 1, TwoHanded, groups.sling, [halfling; propulsive])

    let uncommonMartialRangedWeapons = [
        halflingSlingStaff
        uncommonMartialRanged ("Shuriken", D4, Piercing, 20, 0, Insignificant, OneHanded, groups.dart, [agile; monk; thrown 20<Feet>])
    ]

    // -- ADVANCED WEAPONS --
    let advancedMelee x = { (martialMelee x) with Category = AdvancedWeapon }

    let advancedMeleeWeapons = [
        advancedMelee ("Dwarven waraxe", D8, Slashing, Heavy 2, OneHanded, groups.axe, [dwarf; sweep; twoHand D12])
        advancedMelee ("Gnome flickmace", D8, Bludgeoning, Heavy 2, OneHanded, groups.flail, [gnome; reach])
        advancedMelee ("Orc necksplitter", D8, Slashing, Heavy 1, OneHanded, groups.axe, [forceful; orc; sweep])
        advancedMelee ("Sawtooth saber", D6, Slashing, Light, OneHanded, groups.sword, [agile; finesse; twin])
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
            Group = groups.brawling
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

    // Improves the skill of a whole category of weapons (as perceived by this character).
    let improveSkill (cat, ty) prof = {
        Prompt = sprintf "%A in %A %A" prof cat ty
        Choices = [AddWeaponSkills (cat, ty, prof)]
        Count = None
    }

    // Adds a melee weapon to the character.  (Excluding shields; we'll
    // do those in their own way.)
    // We'll do this with a proxy improvement to analyse the character and
    // pick out the most skilful weapons they have access to.
    // For now, we'll assume Trained is good enough and not try to filter for
    // Expert or higher in particular weapons; reasonable if we choose a weapon
    // at character level 1.
    let addMeleeWeapon = AddWeaponOfType Melee

    // Similarly :)
    let addRangedWeapon = AddWeaponOfType Ranged
