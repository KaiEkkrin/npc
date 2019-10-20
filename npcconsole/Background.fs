namespace NpcConsole

open NpcConsole.Attributes

module Background =
    let hasNone c = Option.isNone c.Background

    // All the backgrounds work in a similar way.
    // TODO Support multiple choice of lore skills here
    let background (name, abilities, skill, lore, feat) = name, hasNone, (fun c -> { c with Background = Some name }, [
        Improve.ability abilities 1
        Improve.anyAbility 1
        Improve.skill skill Trained
        Improve.skill (Skills.lore lore) Trained
        Feats.forceAdd feat
    ])

    let backgrounds = {
        Prompt = "Background"
        Choices = [
            // TODO Fix the choices that rely on a parameterised feat.
            background ("Acolyte", [Intelligence; Wisdom], Skills.religion, "Scribing", Feats.studentOfTheCanon)
            background ("Acrobat", [Strength; Dexterity], Skills.acrobatics, "Circus", Feats.steadyBalance)
            background ("Animal Whisperer", [Wisdom; Charisma], Skills.nature, "Plains", Feats.trainAnimal)
            //background ("Artist", [Strength; Intelligence], Skills.crafting, "Guild", Feats.specialtyCrafting)
            background ("Barkeep", [Constitution; Charisma], Skills.diplomacy, "Alcohol", Feats.hobnobber)
            background ("Barrister", [Intelligence; Charisma], Skills.diplomacy, "Legal", Feats.groupImpression)
            background ("Bounty Hunter", [Strength; Wisdom], Skills.survival, "Legal", Feats.experiencedTracker)
            background ("Charlatan", [Intelligence; Charisma], Skills.deception, "Underworld", Feats.charmingLiar)
            background ("Criminal", [Dexterity; Intelligence], Skills.stealth, "Underworld", Feats.experiencedSmuggler)
            background ("Detective", [Intelligence; Wisdom], Skills.society, "Underworld", Feats.streetwise)
            background ("Emissary", [Intelligence; Charisma], Skills.society, "City", Feats.multilingual)
            background ("Entertainer", [Dexterity; Charisma], Skills.performance, "Theater", Feats.fascinatingPerformance)
            background ("Farmhand", [Constitution; Wisdom], Skills.athletics, "Farming", Feats.assurance Skills.athletics)
            background ("Field Medic", [Constitution; Wisdom], Skills.medicine, "Warfare", Feats.battleMedic)
            background ("Fortune Teller", [Intelligence; Charisma], Skills.occultism, "Fortune-Telling", Feats.oddityIdentification)
            background ("Gambler", [Dexterity; Charisma], Skills.deception, "Games", Feats.lieToMe)
            background ("Gladiator", [Strength; Charisma], Skills.performance, "Gladiatorial", Feats.impressivePerformance)
            background ("Guard", [Strength; Charisma], Skills.intimidation, "Legal", Feats.quickCoercion)
            background ("Herbalist", [Constitution; Wisdom], Skills.nature, "Herbalism", Feats.naturalMedicine)
            background ("Hermit", [Constitution; Intelligence], Skills.nature, "Cave", Feats.dubiousKnowledge) // TODO or occultism not nature
            background ("Hunter", [Dexterity; Wisdom], Skills.survival, "Tanning", Feats.surveyWildlife)
            background ("Laborer", [Strength; Constitution], Skills.athletics, "Labor", Feats.heftyHauler)
            background ("Martial Disciple", [Strength; Dexterity], Skills.acrobatics, "Warfare", Feats.catFall) // TODO or athetics and Quick Jump feat
            background ("Merchant", [Intelligence; Charisma], Skills.diplomacy, "Mercantile", Feats.bargainHunter)
            background ("Miner", [Strength; Wisdom], Skills.survival, "Mining", Feats.terrainExpertise "Underground")
            background ("Noble", [Intelligence; Charisma], Skills.society, "Genealogy", Feats.courtlyGraces) // TODO or Heraldry Lore
            background ("Nomad", [Constitution; Wisdom], Skills.survival, "Desert", Feats.assurance Skills.survival) // TODO choose lore
            background ("Prisoner", [Strength; Constitution], Skills.stealth, "Underworld", Feats.experiencedSmuggler)
            background ("Sailor", [Strength; Dexterity], Skills.athletics, "Sailing", Feats.underwaterMarauder)
            background ("Scholar", [Intelligence; Wisdom], Skills.arcana, "Academia", Feats.assurance Skills.arcana) // TODO or religion, nature or occultism and the matching Assurance
            background ("Scout", [Dexterity; Wisdom], Skills.survival, "Forest", Feats.forager)
            background ("Street Urchin", [Dexterity; Constitution], Skills.thievery, "City", Feats.pickpocket)
            //background ("Tinker", [Dexterity; Intelligence], Skills.crafting, "Engineering", Feats.specialtyCrafting)
            background ("Warrior", [Strength; Constitution], Skills.intimidation, "Warfare", Feats.intimidatingGlare)
        ]
        Count = 1
    }