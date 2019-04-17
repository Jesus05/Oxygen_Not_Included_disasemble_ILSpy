using STRINGS;
using TUNING;

namespace Database
{
	public class SkillPerks : ResourceSet<SkillPerk>
	{
		public SkillPerk IncreaseDigSpeedSmall;

		public SkillPerk IncreaseDigSpeedMedium;

		public SkillPerk IncreaseDigSpeedLarge;

		public SkillPerk CanDigVeryFirm;

		public SkillPerk CanDigNearlyImpenetrable;

		public SkillPerk IncreaseConstructionSmall;

		public SkillPerk IncreaseConstructionMedium;

		public SkillPerk IncreaseConstructionLarge;

		public SkillPerk IncreaseConstructionMechatronics;

		public SkillPerk IncreaseLearningSmall;

		public SkillPerk IncreaseLearningMedium;

		public SkillPerk IncreaseLearningLarge;

		public SkillPerk IncreaseBotanySmall;

		public SkillPerk IncreaseBotanyMedium;

		public SkillPerk IncreaseBotanyLarge;

		public SkillPerk CanFarmTinker;

		public SkillPerk CanWrangleCreatures;

		public SkillPerk CanUseRanchStation;

		public SkillPerk IncreaseRanchingSmall;

		public SkillPerk IncreaseRanchingMedium;

		public SkillPerk IncreaseAthleticsSmall;

		public SkillPerk IncreaseAthleticsMedium;

		public SkillPerk IncreaseStrengthSmall;

		public SkillPerk IncreaseStrengthMedium;

		public SkillPerk IncreaseStrengthGofer;

		public SkillPerk IncreaseStrengthCourier;

		public SkillPerk IncreaseStrengthGroundskeeper;

		public SkillPerk IncreaseStrengthPlumber;

		public SkillPerk IncreaseCarryAmountSmall;

		public SkillPerk IncreaseCarryAmountMedium;

		public SkillPerk IncreaseArtSmall;

		public SkillPerk IncreaseArtMedium;

		public SkillPerk IncreaseArtLarge;

		public SkillPerk CanArt;

		public SkillPerk CanArtUgly;

		public SkillPerk CanArtOkay;

		public SkillPerk CanArtGreat;

		public SkillPerk IncreaseMachinerySmall;

		public SkillPerk IncreaseMachineryMedium;

		public SkillPerk IncreaseMachineryLarge;

		public SkillPerk ConveyorBuild;

		public SkillPerk CanPowerTinker;

		public SkillPerk CanElectricGrill;

		public SkillPerk IncreaseCookingSmall;

		public SkillPerk IncreaseCookingMedium;

		public SkillPerk IncreaseCaringSmall;

		public SkillPerk IncreaseCaringMedium;

		public SkillPerk IncreaseCaringLarge;

		public SkillPerk CanCompound;

		public SkillPerk CanDoctor;

		public SkillPerk CanAdvancedMedicine;

		public SkillPerk ExosuitExpertise;

		public SkillPerk AllowAdvancedResearch;

		public SkillPerk AllowInterstellarResearch;

		public SkillPerk CanStudyWorldObjects;

		public SkillPerk CanDoPlumbing;

		public SkillPerk CanUseRockets;

		public SkillPerk FasterSpaceFlight;

		public SkillPerk CanTrainToBeAstronaut;

		public SkillPerks(ResourceSet parent)
			: base("SkillPerks", parent)
		{
			IncreaseDigSpeedSmall = Add(new SkillAttributePerk("IncreaseDigSpeedSmall", Db.Get().Attributes.Digging.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_MINER.NAME));
			IncreaseDigSpeedMedium = Add(new SkillAttributePerk("IncreaseDigSpeedMedium", Db.Get().Attributes.Digging.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.MINER.NAME));
			IncreaseDigSpeedLarge = Add(new SkillAttributePerk("IncreaseDigSpeedLarge", Db.Get().Attributes.Digging.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.SENIOR_MINER.NAME));
			CanDigVeryFirm = Add(new SimpleSkillPerk("CanDigVeryFirm", UI.ROLES_SCREEN.PERKS.CAN_DIG_VERY_FIRM.DESCRIPTION));
			CanDigNearlyImpenetrable = Add(new SimpleSkillPerk("CanDigAbyssalite", UI.ROLES_SCREEN.PERKS.CAN_DIG_NEARLY_IMPENETRABLE.DESCRIPTION));
			IncreaseConstructionSmall = Add(new SkillAttributePerk("IncreaseConstructionSmall", Db.Get().Attributes.Construction.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_BUILDER.NAME));
			IncreaseConstructionMedium = Add(new SkillAttributePerk("IncreaseConstructionMedium", Db.Get().Attributes.Construction.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.BUILDER.NAME));
			IncreaseConstructionLarge = Add(new SkillAttributePerk("IncreaseConstructionLarge", Db.Get().Attributes.Construction.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.SENIOR_BUILDER.NAME));
			IncreaseConstructionMechatronics = Add(new SkillAttributePerk("IncreaseConstructionMechatronics", Db.Get().Attributes.Construction.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.NAME));
			IncreaseLearningSmall = Add(new SkillAttributePerk("IncreaseLearningSmall", Db.Get().Attributes.Learning.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_RESEARCHER.NAME));
			IncreaseLearningMedium = Add(new SkillAttributePerk("IncreaseLearningMedium", Db.Get().Attributes.Learning.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.RESEARCHER.NAME));
			IncreaseLearningLarge = Add(new SkillAttributePerk("IncreaseLearningLarge", Db.Get().Attributes.Learning.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.SENIOR_RESEARCHER.NAME));
			IncreaseBotanySmall = Add(new SkillAttributePerk("IncreaseBotanySmall", Db.Get().Attributes.Botanist.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_FARMER.NAME));
			IncreaseBotanyMedium = Add(new SkillAttributePerk("IncreaseBotanyMedium", Db.Get().Attributes.Botanist.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.FARMER.NAME));
			IncreaseBotanyLarge = Add(new SkillAttributePerk("IncreaseBotanyLarge", Db.Get().Attributes.Botanist.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.SENIOR_FARMER.NAME));
			CanFarmTinker = Add(new SimpleSkillPerk("CanFarmTinker", UI.ROLES_SCREEN.PERKS.CAN_FARM_TINKER.DESCRIPTION));
			IncreaseRanchingSmall = Add(new SkillAttributePerk("IncreaseRanchingSmall", Db.Get().Attributes.Ranching.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.RANCHER.NAME));
			IncreaseRanchingMedium = Add(new SkillAttributePerk("IncreaseRanchingMedium", Db.Get().Attributes.Ranching.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.SENIOR_RANCHER.NAME));
			CanWrangleCreatures = Add(new SimpleSkillPerk("CanWrangleCreatures", UI.ROLES_SCREEN.PERKS.CAN_WRANGLE_CREATURES.DESCRIPTION));
			CanUseRanchStation = Add(new SimpleSkillPerk("CanUseRanchStation", UI.ROLES_SCREEN.PERKS.CAN_USE_RANCH_STATION.DESCRIPTION));
			IncreaseAthleticsSmall = Add(new SkillAttributePerk("IncreaseAthleticsSmall", Db.Get().Attributes.Athletics.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.HAULER.NAME));
			IncreaseAthleticsMedium = Add(new SkillAttributePerk("IncreaseAthletics", Db.Get().Attributes.Athletics.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.SUIT_EXPERT.NAME));
			IncreaseStrengthGofer = Add(new SkillAttributePerk("IncreaseStrengthGofer", Db.Get().Attributes.Strength.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.HAULER.NAME));
			IncreaseStrengthCourier = Add(new SkillAttributePerk("IncreaseStrengthCourier", Db.Get().Attributes.Strength.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.MATERIALS_MANAGER.NAME));
			IncreaseStrengthGroundskeeper = Add(new SkillAttributePerk("IncreaseStrengthGroundskeeper", Db.Get().Attributes.Strength.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.HANDYMAN.NAME));
			IncreaseStrengthPlumber = Add(new SkillAttributePerk("IncreaseStrengthPlumber", Db.Get().Attributes.Strength.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.PLUMBER.NAME));
			IncreaseCarryAmountSmall = Add(new SkillAttributePerk("IncreaseCarryAmountSmall", Db.Get().Attributes.CarryAmount.Id, 400f, DUPLICANTS.ROLES.HAULER.NAME));
			IncreaseCarryAmountMedium = Add(new SkillAttributePerk("IncreaseCarryAmountMedium", Db.Get().Attributes.CarryAmount.Id, 800f, DUPLICANTS.ROLES.MATERIALS_MANAGER.NAME));
			IncreaseArtSmall = Add(new SkillAttributePerk("IncreaseArtSmall", Db.Get().Attributes.Art.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_ARTIST.NAME));
			IncreaseArtMedium = Add(new SkillAttributePerk("IncreaseArt", Db.Get().Attributes.Art.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.ARTIST.NAME));
			IncreaseArtLarge = Add(new SkillAttributePerk("IncreaseArtLarge", Db.Get().Attributes.Art.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.MASTER_ARTIST.NAME));
			CanArt = Add(new SimpleSkillPerk("CanArt", UI.ROLES_SCREEN.PERKS.CAN_ART.DESCRIPTION));
			CanArtUgly = Add(new SimpleSkillPerk("CanArtUgly", UI.ROLES_SCREEN.PERKS.CAN_ART_UGLY.DESCRIPTION));
			CanArtOkay = Add(new SimpleSkillPerk("CanArtOkay", UI.ROLES_SCREEN.PERKS.CAN_ART_OKAY.DESCRIPTION));
			CanArtGreat = Add(new SimpleSkillPerk("CanArtGreat", UI.ROLES_SCREEN.PERKS.CAN_ART_GREAT.DESCRIPTION));
			IncreaseMachinerySmall = Add(new SkillAttributePerk("IncreaseMachinerySmall", Db.Get().Attributes.Machinery.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.MACHINE_TECHNICIAN.NAME));
			IncreaseMachineryMedium = Add(new SkillAttributePerk("IncreaseMachineryMedium", Db.Get().Attributes.Machinery.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.POWER_TECHNICIAN.NAME));
			IncreaseMachineryLarge = Add(new SkillAttributePerk("IncreaseMachineryLarge", Db.Get().Attributes.Machinery.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.NAME));
			ConveyorBuild = Add(new SimpleSkillPerk("ConveyorBuild", UI.ROLES_SCREEN.PERKS.CONVEYOR_BUILD.DESCRIPTION));
			CanPowerTinker = Add(new SimpleSkillPerk("CanPowerTinker", UI.ROLES_SCREEN.PERKS.CAN_POWER_TINKER.DESCRIPTION));
			CanElectricGrill = Add(new SimpleSkillPerk("CanElectricGrill", UI.ROLES_SCREEN.PERKS.CAN_ELECTRIC_GRILL.DESCRIPTION));
			IncreaseCookingSmall = Add(new SkillAttributePerk("IncreaseCookingSmall", Db.Get().Attributes.Cooking.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_COOK.NAME));
			IncreaseCookingMedium = Add(new SkillAttributePerk("IncreaseCookingMedium", Db.Get().Attributes.Cooking.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.COOK.NAME));
			IncreaseCaringSmall = Add(new SkillAttributePerk("IncreaseCaringSmall", Db.Get().Attributes.Caring.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_MEDIC.NAME));
			IncreaseCaringMedium = Add(new SkillAttributePerk("IncreaseCaringMedium", Db.Get().Attributes.Caring.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.MEDIC.NAME));
			IncreaseCaringLarge = Add(new SkillAttributePerk("IncreaseCaringLarge", Db.Get().Attributes.Caring.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.SENIOR_MEDIC.NAME));
			CanCompound = Add(new SimpleSkillPerk("CanCompound", UI.ROLES_SCREEN.PERKS.CAN_COMPOUND.DESCRIPTION));
			CanDoctor = Add(new SimpleSkillPerk("CanDoctor", UI.ROLES_SCREEN.PERKS.CAN_DOCTOR.DESCRIPTION));
			CanAdvancedMedicine = Add(new SimpleSkillPerk("CanAdvancedMedicine", UI.ROLES_SCREEN.PERKS.CAN_ADVANCED_MEDICINE.DESCRIPTION));
			ExosuitExpertise = Add(new SimpleSkillPerk("ExosuitExpertise", UI.ROLES_SCREEN.PERKS.EXOSUIT_EXPERTISE.DESCRIPTION));
			AllowAdvancedResearch = Add(new SimpleSkillPerk("AllowAdvancedResearch", UI.ROLES_SCREEN.PERKS.ADVANCED_RESEARCH.DESCRIPTION));
			AllowInterstellarResearch = Add(new SimpleSkillPerk("AllowInterStellarResearch", UI.ROLES_SCREEN.PERKS.INTERSTELLAR_RESEARCH.DESCRIPTION));
			CanStudyWorldObjects = Add(new SimpleSkillPerk("CanStudyWorldObjects", UI.ROLES_SCREEN.PERKS.CAN_STUDY_WORLD_OBJECTS.DESCRIPTION));
			CanDoPlumbing = Add(new SimpleSkillPerk("CanDoPlumbing", UI.ROLES_SCREEN.PERKS.CAN_DO_PLUMBING.DESCRIPTION));
			CanUseRockets = Add(new SimpleSkillPerk("CanUseRockets", UI.ROLES_SCREEN.PERKS.CAN_USE_ROCKETS.DESCRIPTION));
			FasterSpaceFlight = Add(new SkillAttributePerk("FasterSpaceFlight", Db.Get().Attributes.SpaceNavigation.Id, 0.1f, DUPLICANTS.ROLES.ASTRONAUT.NAME));
			CanTrainToBeAstronaut = Add(new SimpleSkillPerk("CanTrainToBeAstronaut", UI.ROLES_SCREEN.PERKS.CAN_DO_ASTRONAUT_TRAINING.DESCRIPTION));
		}
	}
}
