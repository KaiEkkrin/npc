namespace Npc.Classes

open Npc
open Npc.Attributes
open Npc.Classes.ClassBasics
open Npc.FeatReq

module Champion =

    let retributiveStrike = classFeat Champion 1 NoReq "Retributive Strike" 106 []
    let glimpseOfRedemption = classFeat Champion 1 NoReq "Glimpse of Redemption" 106 []
    let liberatingStep = classFeat Champion 1 NoReq "Liberating Step" 106 []

    let paladin = classFeatWith Champion 1 NoReq "Paladin" 105 [
        Feats.forceAdd retributiveStrike
    ]
    let redeemer = classFeatWith Champion 1 NoReq "Redeemer" 106 [
        Feats.forceAdd glimpseOfRedemption
    ]
    let liberator = classFeatWith Champion 1 NoReq "Liberator" 106 [
        Feats.forceAdd liberatingStep
    ]

    let causes = [paladin; redeemer; liberator]
    let tenetsOfGoodReq = FeatReq "Paladin" >|| FeatReq "Redeemer" >|| FeatReq "Liberator"

    let bladeAlly = classFeat Champion 3 NoReq "Blade Ally" 108 [] // TODO weapon choice, specialization etc.
    let shieldAlly = classFeat Champion 3 NoReq "Shield Ally" 108 [] // TODO shield stats.
    let steedAlly = classFeat Champion 3 NoReq "Steed Ally" 108 []

    let divineAllies = [bladeAlly; shieldAlly; steedAlly]

    let weaponExpertise = classFeatWith Champion 5 NoReq "Weapon Expertise" 169 [
        Weapons.improveSkill (SimpleWeapon, Melee) Expert
        Weapons.improveSkill (SimpleWeapon, Ranged) Expert
        Weapons.improveSkill (MartialWeapon, Melee) Expert
        Weapons.improveSkill (MartialWeapon, Ranged) Expert
    ]

    let championExpertise = classFeatWith Champion 9 NoReq "Champion Expertise" 108 [
        increaseClassSkill Champion Expert
        increaseSpellSkill Divine Expert
    ]

    let divineSmite = classFeat Champion 9 NoReq "Divine Smite" 109 []
    let juggernaut = classFeatWith Champion 9 NoReq "Juggernaut" 109 [
        Improve2.skill Skills.fortitudeSave Master // TODO successes become criticals
    ]

    let championFeats = [
        classFeat Champion 1 NoReq "Deity's Domain" 109 [] // TODO implement deities etc
        classFeat Champion 1 NoReq "Ranged Reprisal" 109 []
        classFeat Champion 1 (FeatReq "Liberator") "Unimpeded Step" 109 []
        classFeat Champion 1 (FeatReq "Redeemer") "Weight of Guilt" 110 []
        classFeat Champion 2 NoReq "Divine Grace" 110 []
        classFeat Champion 2 tenetsOfGoodReq "Dragonslayer Oath" 110 []
        classFeat Champion 2 tenetsOfGoodReq "Fiendsbane Oath" 110 []
        classFeat Champion 2 tenetsOfGoodReq "Shining Oath" 110 []
        classFeat Champion 2 NoReq "Vengeful Oath" 110 []
        classFeat Champion 4 tenetsOfGoodReq "Aura of Courage" 111 []
        classFeat Champion 4 tenetsOfGoodReq "Divine Health" 111 []
        classFeat Champion 4 NoReq "Mercy" 111 []
        classFeat Champion 6 NoReq "Attack of Opportunity" 111 []
        classFeatWith Champion 6 tenetsOfGoodReq "Litany against Wrath" 112 [
            Improve2.pool ("Focus", 1)
        ]
        classFeat Champion 6 (FeatReq "Steed Ally") "Loyal Warhorse" 112 []
        classFeat Champion 6 (FeatReq "Shield Ally") "Shield Warden" 112 []
        classFeat Champion 6 (FeatReq "Blade Ally") "Smite Evil" 112 []
        classFeatWith Champion 8 (FeatReq "Deity's Domain") "Advanced Deity's Domain" 112 [
            Improve2.pool ("Focus", 1)
        ]
        classFeat Champion 8 (FeatReq "Mercy") "Greater Mercy" 112 []
        classFeat Champion 8 (FeatReq "Steed Ally") "Heal Mount" 113 []
        classFeat Champion 8 NoReq "Quick Block" 113 []
        classFeatWith Champion 8 NoReq "Second Ally" 113 [
            Improve2.feat "Divine Ally" divineAllies 1
        ]
        classFeat Champion 8 tenetsOfGoodReq "Sense Evil" 113 []
        classFeat Champion 10 NoReq "Devoted Focus" 113 []
        classFeat Champion 10 (FeatReq "Steed Ally" >&& FeatReq "Loyal Warhorse") "Imposing Destrier" 113 []
        classFeatWith Champion 10 tenetsOfGoodReq "Litany against Sloth" 113 [
            Improve2.pool ("Focus", 1)
        ]
        classFeat Champion 10 (FeatReq "Blade Ally") "Radiant Blade Spirit" 113 []
        classFeat Champion 10 (FeatReq "Shield Ally" >&& tenetsOfGoodReq >&& FeatReq "Shield Warden") "Shield of Reckoning" 113 []
    ]

    let addChampionFeat = Improve2.feat "Champion feat" championFeats 1

    let champion = [
        AddClass (Champion, [
            Improve2.feat "Ability boost" (classAbilityBoostFeats Champion [Strength; Dexterity]) 1
            Improve2.hitPointsPerLevel 10
            Improve2.skill Skills.perception Trained
            Improve2.skill Skills.fortitudeSave Expert
            Improve2.skill Skills.reflexSave Trained
            Improve2.skill Skills.willSave Expert
            Improve2.skill Skills.religion Trained
            Improve2.skillsBasedOnInt 3 Skills.regularSkills // I rolled the deity-based skill into this one
            Weapons.improveSkill (SimpleWeapon, Melee) Trained
            Weapons.improveSkill (SimpleWeapon, Ranged) Trained
            Weapons.improveSkill (MartialWeapon, Melee) Trained
            Weapons.improveSkill (MartialWeapon, Ranged) Trained
            Weapons.improveSkill (Unarmed, Melee) Trained
            Improve2.skill (Char2.armorSkill LightArmor) Trained
            Improve2.skill (Char2.armorSkill MediumArmor) Trained
            Improve2.skill (Char2.armorSkill HeavyArmor) Trained
            Improve2.skill (Char2.armorSkill Unarmored) Trained
            Improve2.feat "Cause" causes 1
            // TODO Choose deific weapon
            Improve2.pool ("Focus", 1)
            Improve2.spellSkill (Skills.spellSkill (Divine, Charisma)) // different from the class skill
            Feats.forceAdd Feats.shieldBlock
            addChampionFeat
        ])
        LevelUp (Champion, 2<Level>, [
            addChampionFeat
            Feats.addSkillFeat
        ])
        LevelUp (Champion, 3<Level>, [
            Improve2.feat "Divine Ally" divineAllies 1
            Feats.addGeneralFeat
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Champion, 4<Level>, [
            addChampionFeat
            Feats.addSkillFeat
        ])
        LevelUp (Champion, 5<Level>, [
            Improve2.anyAbilityBoost 4
            Ancestry.addAncestryFeat
            Skills.increase Skills.regularSkills
            Feats.forceAdd weaponExpertise
        ])
        LevelUp (Champion, 6<Level>, [
            addChampionFeat
            Feats.addSkillFeat
        ])
        LevelUp (Champion, 7<Level>, [
            Feats.forceAdd Feats.heavyArmorExpertise // TODO also specializations
            Feats.addGeneralFeat
            Skills.increase Skills.regularSkills
            Feats.forceAdd Feats.weaponSpecialization
        ])
        LevelUp (Champion, 8<Level>, [
            addChampionFeat
            Feats.addSkillFeat
        ])
        LevelUp (Champion, 9<Level>, [
            Ancestry.addAncestryFeat
            Feats.forceAdd championExpertise
            Feats.forceAdd divineSmite
            Feats.forceAdd juggernaut
            Feats.forceAdd Feats.lightningReflexes
            Skills.increase Skills.regularSkills
        ])
        LevelUp (Champion, 10<Level>, [
            Improve2.anyAbilityBoost 4
            addChampionFeat
            Feats.addSkillFeat
        ])
    ]