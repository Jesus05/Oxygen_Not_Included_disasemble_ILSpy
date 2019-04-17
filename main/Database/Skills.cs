using STRINGS;
using System.Collections.Generic;

namespace Database
{
	public class Skills : ResourceSet<Skill>
	{
		public Skill Mining1;

		public Skill Mining2;

		public Skill Mining3;

		public Skill Building1;

		public Skill Building2;

		public Skill Building3;

		public Skill Farming1;

		public Skill Farming2;

		public Skill Farming3;

		public Skill Ranching1;

		public Skill Ranching2;

		public Skill Researching1;

		public Skill Researching2;

		public Skill Researching3;

		public Skill Cooking1;

		public Skill Cooking2;

		public Skill Arting1;

		public Skill Arting2;

		public Skill Arting3;

		public Skill Hauling1;

		public Skill Hauling2;

		public Skill Suits1;

		public Skill Technicals1;

		public Skill Technicals2;

		public Skill Engineering1;

		public Skill Basekeeping1;

		public Skill Basekeeping2;

		public Skill Astronauting1;

		public Skill Astronauting2;

		public Skill Medicine1;

		public Skill Medicine2;

		public Skill Medicine3;

		public Skills(ResourceSet parent)
			: base("Skills", parent)
		{
			Mining1 = Add(new Skill("Mining1", DUPLICANTS.ROLES.JUNIOR_MINER.NAME, DUPLICANTS.ROLES.JUNIOR_MINER.DESCRIPTION, 0, "hat_role_mining1", Db.Get().SkillGroups.Mining.Id));
			Mining1.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseDigSpeedSmall,
				Db.Get().SkillPerks.CanDigVeryFirm
			};
			Mining2 = Add(new Skill("Mining2", DUPLICANTS.ROLES.MINER.NAME, DUPLICANTS.ROLES.MINER.DESCRIPTION, 1, "hat_role_mining2", Db.Get().SkillGroups.Mining.Id));
			Mining2.priorSkills = new List<string>
			{
				Mining1.Id
			};
			Mining2.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseDigSpeedMedium,
				Db.Get().SkillPerks.CanDigNearlyImpenetrable
			};
			Mining3 = Add(new Skill("Mining3", DUPLICANTS.ROLES.SENIOR_MINER.NAME, DUPLICANTS.ROLES.SENIOR_MINER.DESCRIPTION, 2, "hat_role_mining3", Db.Get().SkillGroups.Mining.Id));
			Mining3.priorSkills = new List<string>
			{
				Mining2.Id
			};
			Mining3.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseDigSpeedLarge
			};
			Building1 = Add(new Skill("Building1", DUPLICANTS.ROLES.JUNIOR_BUILDER.NAME, DUPLICANTS.ROLES.JUNIOR_BUILDER.DESCRIPTION, 0, "hat_role_building1", Db.Get().SkillGroups.Building.Id));
			Building1.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseConstructionSmall
			};
			Building2 = Add(new Skill("Building2", DUPLICANTS.ROLES.BUILDER.NAME, DUPLICANTS.ROLES.BUILDER.DESCRIPTION, 1, "hat_role_building2", Db.Get().SkillGroups.Building.Id));
			Building2.priorSkills = new List<string>
			{
				Building1.Id
			};
			Building2.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseConstructionMedium
			};
			Building3 = Add(new Skill("Building3", DUPLICANTS.ROLES.SENIOR_BUILDER.NAME, DUPLICANTS.ROLES.SENIOR_BUILDER.DESCRIPTION, 2, "hat_role_building3", Db.Get().SkillGroups.Building.Id));
			Building3.priorSkills = new List<string>
			{
				Building2.Id
			};
			Building3.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseConstructionLarge
			};
			Farming1 = Add(new Skill("Farming1", DUPLICANTS.ROLES.JUNIOR_FARMER.NAME, DUPLICANTS.ROLES.JUNIOR_FARMER.DESCRIPTION, 0, "hat_role_farming1", Db.Get().SkillGroups.Farming.Id));
			Farming1.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseBotanySmall
			};
			Farming2 = Add(new Skill("Farming2", DUPLICANTS.ROLES.FARMER.NAME, DUPLICANTS.ROLES.FARMER.DESCRIPTION, 1, "hat_role_farming2", Db.Get().SkillGroups.Farming.Id));
			Farming2.priorSkills = new List<string>
			{
				Farming1.Id
			};
			Farming2.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseBotanyMedium,
				Db.Get().SkillPerks.CanFarmTinker
			};
			Farming3 = Add(new Skill("Farming3", DUPLICANTS.ROLES.SENIOR_FARMER.NAME, DUPLICANTS.ROLES.SENIOR_FARMER.DESCRIPTION, 2, "hat_role_farming3", Db.Get().SkillGroups.Farming.Id));
			Farming3.priorSkills = new List<string>
			{
				Farming2.Id
			};
			Farming3.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseBotanyLarge
			};
			Ranching1 = Add(new Skill("Ranching1", DUPLICANTS.ROLES.RANCHER.NAME, DUPLICANTS.ROLES.RANCHER.DESCRIPTION, 1, "hat_role_rancher1", Db.Get().SkillGroups.Ranching.Id));
			Ranching1.priorSkills = new List<string>
			{
				Farming1.Id
			};
			Ranching1.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.CanWrangleCreatures,
				Db.Get().SkillPerks.CanUseRanchStation,
				Db.Get().SkillPerks.IncreaseRanchingSmall
			};
			Ranching2 = Add(new Skill("Ranching2", DUPLICANTS.ROLES.SENIOR_RANCHER.NAME, DUPLICANTS.ROLES.SENIOR_RANCHER.DESCRIPTION, 2, "hat_role_rancher2", Db.Get().SkillGroups.Ranching.Id));
			Ranching2.priorSkills = new List<string>
			{
				Ranching1.Id
			};
			Ranching2.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseRanchingMedium
			};
			Researching1 = Add(new Skill("Researching1", DUPLICANTS.ROLES.JUNIOR_RESEARCHER.NAME, DUPLICANTS.ROLES.JUNIOR_RESEARCHER.DESCRIPTION, 0, "hat_role_research1", Db.Get().SkillGroups.Research.Id));
			Researching1.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseLearningSmall,
				Db.Get().SkillPerks.AllowAdvancedResearch
			};
			Researching2 = Add(new Skill("Researching2", DUPLICANTS.ROLES.RESEARCHER.NAME, DUPLICANTS.ROLES.RESEARCHER.DESCRIPTION, 1, "hat_role_research2", Db.Get().SkillGroups.Research.Id));
			Researching2.priorSkills = new List<string>
			{
				Researching1.Id
			};
			Researching2.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseLearningMedium,
				Db.Get().SkillPerks.CanStudyWorldObjects
			};
			Researching3 = Add(new Skill("Researching3", DUPLICANTS.ROLES.SENIOR_RESEARCHER.NAME, DUPLICANTS.ROLES.SENIOR_RESEARCHER.DESCRIPTION, 2, "hat_role_research3", Db.Get().SkillGroups.Research.Id));
			Researching3.priorSkills = new List<string>
			{
				Researching2.Id
			};
			Researching3.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseLearningLarge,
				Db.Get().SkillPerks.AllowInterstellarResearch
			};
			Cooking1 = Add(new Skill("Cooking1", DUPLICANTS.ROLES.JUNIOR_COOK.NAME, DUPLICANTS.ROLES.JUNIOR_COOK.DESCRIPTION, 0, "hat_role_cooking1", Db.Get().SkillGroups.Cooking.Id));
			Cooking1.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseCookingSmall,
				Db.Get().SkillPerks.CanElectricGrill
			};
			Cooking2 = Add(new Skill("Cooking2", DUPLICANTS.ROLES.COOK.NAME, DUPLICANTS.ROLES.COOK.DESCRIPTION, 1, "hat_role_cooking2", Db.Get().SkillGroups.Cooking.Id));
			Cooking2.priorSkills = new List<string>
			{
				Cooking1.Id
			};
			Cooking2.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseCookingMedium
			};
			Arting1 = Add(new Skill("Arting1", DUPLICANTS.ROLES.JUNIOR_ARTIST.NAME, DUPLICANTS.ROLES.JUNIOR_ARTIST.DESCRIPTION, 0, "hat_role_art1", Db.Get().SkillGroups.Art.Id));
			Arting1.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.CanArt,
				Db.Get().SkillPerks.CanArtUgly,
				Db.Get().SkillPerks.IncreaseArtSmall
			};
			Arting2 = Add(new Skill("Arting2", DUPLICANTS.ROLES.ARTIST.NAME, DUPLICANTS.ROLES.ARTIST.DESCRIPTION, 1, "hat_role_art2", Db.Get().SkillGroups.Art.Id));
			Arting2.priorSkills = new List<string>
			{
				Arting1.Id
			};
			Arting2.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.CanArtOkay,
				Db.Get().SkillPerks.IncreaseArtMedium
			};
			Arting3 = Add(new Skill("Arting3", DUPLICANTS.ROLES.MASTER_ARTIST.NAME, DUPLICANTS.ROLES.MASTER_ARTIST.DESCRIPTION, 2, "hat_role_art3", Db.Get().SkillGroups.Art.Id));
			Arting3.priorSkills = new List<string>
			{
				Arting2.Id
			};
			Arting3.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.CanArtGreat,
				Db.Get().SkillPerks.IncreaseArtLarge
			};
			Hauling1 = Add(new Skill("Hauling1", DUPLICANTS.ROLES.HAULER.NAME, DUPLICANTS.ROLES.HAULER.DESCRIPTION, 0, "hat_role_hauling1", Db.Get().SkillGroups.Hauling.Id));
			Hauling1.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseStrengthGofer,
				Db.Get().SkillPerks.IncreaseCarryAmountSmall
			};
			Hauling2 = Add(new Skill("Hauling2", DUPLICANTS.ROLES.MATERIALS_MANAGER.NAME, DUPLICANTS.ROLES.MATERIALS_MANAGER.DESCRIPTION, 1, "hat_role_hauling2", Db.Get().SkillGroups.Hauling.Id));
			Hauling2.priorSkills = new List<string>
			{
				Hauling1.Id
			};
			Hauling2.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseStrengthCourier,
				Db.Get().SkillPerks.IncreaseCarryAmountMedium
			};
			Suits1 = Add(new Skill("Suits1", DUPLICANTS.ROLES.SUIT_EXPERT.NAME, DUPLICANTS.ROLES.SUIT_EXPERT.DESCRIPTION, 2, "hat_role_suits1", Db.Get().SkillGroups.Suits.Id));
			Suits1.priorSkills = new List<string>
			{
				Hauling2.Id
			};
			Suits1.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.ExosuitExpertise,
				Db.Get().SkillPerks.IncreaseAthleticsMedium
			};
			Technicals1 = Add(new Skill("Technicals1", DUPLICANTS.ROLES.MACHINE_TECHNICIAN.NAME, DUPLICANTS.ROLES.MACHINE_TECHNICIAN.DESCRIPTION, 0, "hat_role_technicals1", Db.Get().SkillGroups.Technicals.Id));
			Technicals1.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseMachinerySmall
			};
			Technicals2 = Add(new Skill("Technicals2", DUPLICANTS.ROLES.POWER_TECHNICIAN.NAME, DUPLICANTS.ROLES.POWER_TECHNICIAN.DESCRIPTION, 1, "hat_role_technicals2", Db.Get().SkillGroups.Technicals.Id));
			Technicals2.priorSkills = new List<string>
			{
				Technicals1.Id
			};
			Technicals2.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseMachineryMedium,
				Db.Get().SkillPerks.CanPowerTinker
			};
			Engineering1 = Add(new Skill("Engineering1", DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.NAME, DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.DESCRIPTION, 2, "hat_role_engineering1", Db.Get().SkillGroups.Technicals.Id));
			Engineering1.priorSkills = new List<string>
			{
				Technicals2.Id,
				Hauling2.Id
			};
			Engineering1.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseMachineryLarge,
				Db.Get().SkillPerks.IncreaseConstructionMechatronics,
				Db.Get().SkillPerks.ConveyorBuild
			};
			Basekeeping1 = Add(new Skill("Basekeeping1", DUPLICANTS.ROLES.HANDYMAN.NAME, DUPLICANTS.ROLES.HANDYMAN.DESCRIPTION, 0, "hat_role_basekeeping1", Db.Get().SkillGroups.Basekeeping.Id));
			Basekeeping1.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseStrengthGroundskeeper
			};
			Basekeeping2 = Add(new Skill("Basekeeping2", DUPLICANTS.ROLES.PLUMBER.NAME, DUPLICANTS.ROLES.PLUMBER.DESCRIPTION, 1, "hat_role_basekeeping2", Db.Get().SkillGroups.Basekeeping.Id));
			Basekeeping2.priorSkills = new List<string>
			{
				Basekeeping1.Id
			};
			Basekeeping2.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseStrengthPlumber,
				Db.Get().SkillPerks.CanDoPlumbing
			};
			Astronauting1 = Add(new Skill("Astronauting1", DUPLICANTS.ROLES.ASTRONAUTTRAINEE.NAME, DUPLICANTS.ROLES.ASTRONAUTTRAINEE.DESCRIPTION, 3, "hat_role_astronaut1", Db.Get().SkillGroups.Suits.Id));
			Astronauting1.priorSkills = new List<string>
			{
				Researching3.Id,
				Suits1.Id
			};
			Astronauting1.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.CanUseRockets
			};
			Astronauting2 = Add(new Skill("Astronauting2", DUPLICANTS.ROLES.ASTRONAUT.NAME, DUPLICANTS.ROLES.ASTRONAUT.DESCRIPTION, 4, "hat_role_astronaut2", Db.Get().SkillGroups.Suits.Id));
			Astronauting2.priorSkills = new List<string>
			{
				Astronauting1.Id
			};
			Astronauting2.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.FasterSpaceFlight
			};
			Medicine1 = Add(new Skill("Medicine1", DUPLICANTS.ROLES.JUNIOR_MEDIC.NAME, DUPLICANTS.ROLES.JUNIOR_MEDIC.DESCRIPTION, 0, "hat_role_medicalaid1", Db.Get().SkillGroups.MedicalAid.Id));
			Medicine1.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.CanCompound,
				Db.Get().SkillPerks.IncreaseCaringSmall
			};
			Medicine2 = Add(new Skill("Medicine2", DUPLICANTS.ROLES.MEDIC.NAME, DUPLICANTS.ROLES.MEDIC.DESCRIPTION, 1, "hat_role_medicalaid1", Db.Get().SkillGroups.MedicalAid.Id));
			Medicine2.priorSkills = new List<string>
			{
				Medicine1.Id
			};
			Medicine2.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.CanDoctor,
				Db.Get().SkillPerks.IncreaseCaringMedium
			};
			Medicine3 = Add(new Skill("Medicine3", DUPLICANTS.ROLES.SENIOR_MEDIC.NAME, DUPLICANTS.ROLES.SENIOR_MEDIC.DESCRIPTION, 2, "hat_role_medicalaid1", Db.Get().SkillGroups.MedicalAid.Id));
			Medicine3.priorSkills = new List<string>
			{
				Medicine2.Id
			};
			Medicine3.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.CanAdvancedMedicine,
				Db.Get().SkillPerks.IncreaseCaringLarge
			};
		}

		public List<Skill> GetSkillsWithPerk(string perk)
		{
			List<Skill> list = new List<Skill>();
			foreach (Skill resource in resources)
			{
				if (resource.GivesPerk(perk))
				{
					list.Add(resource);
				}
			}
			return list;
		}

		public List<Skill> GetSkillsWithPerk(SkillPerk perk)
		{
			List<Skill> list = new List<Skill>();
			foreach (Skill resource in resources)
			{
				if (resource.GivesPerk(perk))
				{
					list.Add(resource);
				}
			}
			return list;
		}
	}
}
