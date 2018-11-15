using STRINGS;
using TUNING;

public class RolePerks
{
	public RoleAttributePerk IncreaseDigSpeedSmall;

	public RoleAttributePerk IncreaseDigSpeedMedium;

	public RoleAttributePerk IncreaseDigSpeedLarge;

	public SimpleRolePerk CanDigVeryFirm;

	public SimpleRolePerk CanDigNearlyImpenetrable;

	public RoleAttributePerk IncreaseConstructionSmall;

	public RoleAttributePerk IncreaseConstructionMedium;

	public RoleAttributePerk IncreaseConstructionLarge;

	public RoleAttributePerk IncreaseConstructionMechatronics;

	public RoleAttributePerk IncreaseLearningSmall;

	public RoleAttributePerk IncreaseLearningMedium;

	public RoleAttributePerk IncreaseLearningLarge;

	public RoleAttributePerk IncreaseBotanySmall;

	public RoleAttributePerk IncreaseBotanyMedium;

	public RoleAttributePerk IncreaseBotanyLarge;

	public SimpleRolePerk CanFarmTinker;

	public SimpleRolePerk CanWrangleCreatures;

	public SimpleRolePerk CanUseRanchStation;

	public RoleAttributePerk IncreaseRanchingSmall;

	public RoleAttributePerk IncreaseRanchingMedium;

	public RoleAttributePerk IncreaseAthleticsSmall;

	public RoleAttributePerk IncreaseAthleticsMedium;

	public RoleAttributePerk IncreaseStrengthSmall;

	public RoleAttributePerk IncreaseStrengthMedium;

	public RoleAttributePerk IncreaseStrengthGofer;

	public RoleAttributePerk IncreaseStrengthCourier;

	public RoleAttributePerk IncreaseStrengthGroundskeeper;

	public RoleAttributePerk IncreaseStrengthPlumber;

	public RoleAttributePerk IncreaseCarryAmountSmall;

	public RoleAttributePerk IncreaseCarryAmountMedium;

	public RoleAttributePerk IncreaseArtSmall;

	public RoleAttributePerk IncreaseArtMedium;

	public SimpleRolePerk CanArt;

	public RoleAttributePerk IncreaseMachinerySmall;

	public RoleAttributePerk IncreaseMachineryMedium;

	public RoleAttributePerk IncreaseMachineryLarge;

	public SimpleRolePerk ConveyorBuild;

	public SimpleRolePerk CanPowerTinker;

	public SimpleRolePerk CanElectricGrill;

	public RoleAttributePerk IncreaseCookingSmall;

	public RoleAttributePerk IncreaseCookingMedium;

	public RoleAttributePerk IncreaseCaringMedium;

	public SimpleRolePerk ExosuitExpertise;

	public SimpleRolePerk AllowAdvancedResearch;

	public SimpleRolePerk AllowInterstellarResearch;

	public SimpleRolePerk CanStudyWorldObjects;

	public SimpleRolePerk CanDoPlumbing;

	public SimpleRolePerk CanUseRockets;

	public SimpleRolePerk CanTrainToBeAstronaut;

	public RolePerks()
	{
		IncreaseDigSpeedSmall = new RoleAttributePerk("IncreaseDigSpeedSmall", UI.ROLES_SCREEN.PERKS.INCREASED_DIG_SPEED.DESCRIPTION, Db.Get().Attributes.Digging.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_MINER.NAME);
		IncreaseDigSpeedMedium = new RoleAttributePerk("IncreaseDigSpeedMedium", UI.ROLES_SCREEN.PERKS.INCREASED_DIG_SPEED.DESCRIPTION, Db.Get().Attributes.Digging.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.MINER.NAME);
		IncreaseDigSpeedLarge = new RoleAttributePerk("IncreaseDigSpeedLarge", UI.ROLES_SCREEN.PERKS.INCREASED_DIG_SPEED.DESCRIPTION, Db.Get().Attributes.Digging.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.SENIOR_MINER.NAME);
		CanDigVeryFirm = new SimpleRolePerk("CanDigVeryFirm", UI.ROLES_SCREEN.PERKS.CAN_DIG_VERY_FIRM.DESCRIPTION);
		CanDigNearlyImpenetrable = new SimpleRolePerk("CanDigAbyssalite", UI.ROLES_SCREEN.PERKS.CAN_DIG_NEARLY_IMPENETRABLE.DESCRIPTION);
		IncreaseConstructionSmall = new RoleAttributePerk("IncreaseConstructionSmall", UI.ROLES_SCREEN.PERKS.INCREASED_CONSTRUCTION.DESCRIPTION, Db.Get().Attributes.Construction.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_BUILDER.NAME);
		IncreaseConstructionMedium = new RoleAttributePerk("IncreaseConstructionMedium", UI.ROLES_SCREEN.PERKS.INCREASED_CONSTRUCTION.DESCRIPTION, Db.Get().Attributes.Construction.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.BUILDER.NAME);
		IncreaseConstructionLarge = new RoleAttributePerk("IncreaseConstructionLarge", UI.ROLES_SCREEN.PERKS.INCREASED_CONSTRUCTION.DESCRIPTION, Db.Get().Attributes.Construction.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.SENIOR_BUILDER.NAME);
		IncreaseConstructionMechatronics = new RoleAttributePerk("IncreaseConstructionMechatronics", UI.ROLES_SCREEN.PERKS.INCREASED_CONSTRUCTION.DESCRIPTION, Db.Get().Attributes.Construction.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.NAME);
		IncreaseLearningSmall = new RoleAttributePerk("IncreaseLearningSmall", UI.ROLES_SCREEN.PERKS.INCREASED_LEARNING.DESCRIPTION, Db.Get().Attributes.Learning.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_RESEARCHER.NAME);
		IncreaseLearningMedium = new RoleAttributePerk("IncreaseLearningMedium", UI.ROLES_SCREEN.PERKS.INCREASED_LEARNING.DESCRIPTION, Db.Get().Attributes.Learning.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.RESEARCHER.NAME);
		IncreaseLearningLarge = new RoleAttributePerk("IncreaseLearningLarge", UI.ROLES_SCREEN.PERKS.INCREASED_LEARNING.DESCRIPTION, Db.Get().Attributes.Learning.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.SENIOR_RESEARCHER.NAME);
		IncreaseBotanySmall = new RoleAttributePerk("IncreaseBotanySmall", UI.ROLES_SCREEN.PERKS.INCREASE_BOTANIST.DESCRIPTION, Db.Get().Attributes.Botanist.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_FARMER.NAME);
		IncreaseBotanyMedium = new RoleAttributePerk("IncreaseBotanyMedium", UI.ROLES_SCREEN.PERKS.INCREASE_BOTANIST.DESCRIPTION, Db.Get().Attributes.Botanist.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.FARMER.NAME);
		IncreaseBotanyLarge = new RoleAttributePerk("IncreaseBotanyLarge", UI.ROLES_SCREEN.PERKS.INCREASE_BOTANIST.DESCRIPTION, Db.Get().Attributes.Botanist.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.SENIOR_FARMER.NAME);
		CanFarmTinker = new SimpleRolePerk("CanFarmTinker", UI.ROLES_SCREEN.PERKS.CAN_FARM_TINKER.DESCRIPTION);
		IncreaseRanchingSmall = new RoleAttributePerk("IncreaseRanchingSmall", UI.ROLES_SCREEN.PERKS.INCREASE_RANCHING.DESCRIPTION, Db.Get().Attributes.Ranching.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.RANCHER.NAME);
		IncreaseRanchingMedium = new RoleAttributePerk("IncreaseRanchingMedium", UI.ROLES_SCREEN.PERKS.INCREASE_RANCHING.DESCRIPTION, Db.Get().Attributes.Ranching.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.SENIOR_RANCHER.NAME);
		CanWrangleCreatures = new SimpleRolePerk("CanWrangleCreatures", UI.ROLES_SCREEN.PERKS.CAN_WRANGLE_CREATURES.DESCRIPTION);
		CanUseRanchStation = new SimpleRolePerk("CanUseRanchStation", UI.ROLES_SCREEN.PERKS.CAN_USE_RANCH_STATION.DESCRIPTION);
		IncreaseAthleticsSmall = new RoleAttributePerk("IncreaseAthleticsSmall", UI.ROLES_SCREEN.PERKS.INCREASED_ATHLETICS.DESCRIPTION, Db.Get().Attributes.Athletics.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.HAULER.NAME);
		IncreaseAthleticsMedium = new RoleAttributePerk("IncreaseAthletics", UI.ROLES_SCREEN.PERKS.INCREASED_ATHLETICS.DESCRIPTION, Db.Get().Attributes.Athletics.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.SUIT_EXPERT.NAME);
		IncreaseStrengthGofer = new RoleAttributePerk("IncreaseStrengthGofer", UI.ROLES_SCREEN.PERKS.INCREASED_STRENGTH.DESCRIPTION, Db.Get().Attributes.Strength.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.HAULER.NAME);
		IncreaseStrengthCourier = new RoleAttributePerk("IncreaseStrengthCourier", UI.ROLES_SCREEN.PERKS.INCREASED_STRENGTH.DESCRIPTION, Db.Get().Attributes.Strength.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.MATERIALS_MANAGER.NAME);
		IncreaseStrengthGroundskeeper = new RoleAttributePerk("IncreaseStrengthGroundskeeper", UI.ROLES_SCREEN.PERKS.INCREASED_STRENGTH.DESCRIPTION, Db.Get().Attributes.Strength.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.HANDYMAN.NAME);
		IncreaseStrengthPlumber = new RoleAttributePerk("IncreaseStrengthPlumber", UI.ROLES_SCREEN.PERKS.INCREASED_STRENGTH.DESCRIPTION, Db.Get().Attributes.Strength.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.PLUMBER.NAME);
		IncreaseCarryAmountSmall = new RoleAttributePerk("IncreaseCarryAmountSmall", UI.ROLES_SCREEN.PERKS.INCREASED_CARRY_AMOUNT.DESCRIPTION, Db.Get().Attributes.CarryAmount.Id, 400f, DUPLICANTS.ROLES.HAULER.NAME);
		IncreaseCarryAmountMedium = new RoleAttributePerk("IncreaseCarryAmountMedium", UI.ROLES_SCREEN.PERKS.INCREASED_CARRY_AMOUNT.DESCRIPTION, Db.Get().Attributes.CarryAmount.Id, 800f, DUPLICANTS.ROLES.MATERIALS_MANAGER.NAME);
		IncreaseArtSmall = new RoleAttributePerk("IncreaseArtSmall", UI.ROLES_SCREEN.PERKS.INCREASED_ART.DESCRIPTION, Db.Get().Attributes.Art.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_ARTIST.NAME);
		IncreaseArtMedium = new RoleAttributePerk("IncreaseArt", UI.ROLES_SCREEN.PERKS.INCREASED_ART.DESCRIPTION, Db.Get().Attributes.Art.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.ARTIST.NAME);
		CanArt = new SimpleRolePerk("CanArt", UI.ROLES_SCREEN.PERKS.CAN_ART.DESCRIPTION);
		IncreaseMachinerySmall = new RoleAttributePerk("IncreaseMachinerySmall", UI.ROLES_SCREEN.PERKS.INCREASED_MACHINERY.DESCRIPTION, Db.Get().Attributes.Machinery.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.MACHINE_TECHNICIAN.NAME);
		IncreaseMachineryMedium = new RoleAttributePerk("IncreaseMachineryMedium", UI.ROLES_SCREEN.PERKS.INCREASED_MACHINERY.DESCRIPTION, Db.Get().Attributes.Machinery.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.POWER_TECHNICIAN.NAME);
		IncreaseMachineryLarge = new RoleAttributePerk("IncreaseMachineryLarge", UI.ROLES_SCREEN.PERKS.INCREASED_MACHINERY.DESCRIPTION, Db.Get().Attributes.Machinery.Id, (float)ROLES.ATTRIBUTE_BONUS_THIRD, DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.NAME);
		ConveyorBuild = new SimpleRolePerk("ConveyorBuild", UI.ROLES_SCREEN.PERKS.CONVEYOR_BUILD.DESCRIPTION);
		CanPowerTinker = new SimpleRolePerk("CanPowerTinker", UI.ROLES_SCREEN.PERKS.CAN_POWER_TINKER.DESCRIPTION);
		CanElectricGrill = new SimpleRolePerk("CanElectricGrill", UI.ROLES_SCREEN.PERKS.CAN_ELECTRIC_GRILL.DESCRIPTION);
		IncreaseCookingSmall = new RoleAttributePerk("IncreaseCookingSmall", UI.ROLES_SCREEN.PERKS.INCREASED_COOKING.DESCRIPTION, Db.Get().Attributes.Cooking.Id, (float)ROLES.ATTRIBUTE_BONUS_FIRST, DUPLICANTS.ROLES.JUNIOR_COOK.NAME);
		IncreaseCookingMedium = new RoleAttributePerk("IncreaseCookingMedium", UI.ROLES_SCREEN.PERKS.INCREASED_COOKING.DESCRIPTION, Db.Get().Attributes.Cooking.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.COOK.NAME);
		IncreaseCaringMedium = new RoleAttributePerk("IncreaseCaringMedium", UI.ROLES_SCREEN.PERKS.INCREASED_CARING.DESCRIPTION, Db.Get().Attributes.Caring.Id, (float)ROLES.ATTRIBUTE_BONUS_SECOND, DUPLICANTS.ROLES.MEDIC.NAME);
		ExosuitExpertise = new SimpleRolePerk("ExosuitExpertise", UI.ROLES_SCREEN.PERKS.EXOSUIT_EXPERTISE.DESCRIPTION);
		AllowAdvancedResearch = new SimpleRolePerk("AllowAdvancedResearch", UI.ROLES_SCREEN.PERKS.ADVANCED_RESEARCH.DESCRIPTION);
		AllowInterstellarResearch = new SimpleRolePerk("AllowInterStellarResearch", UI.ROLES_SCREEN.PERKS.INTERSTELLAR_RESEARCH.DESCRIPTION);
		CanStudyWorldObjects = new SimpleRolePerk("CanStudyWorldObjects", UI.ROLES_SCREEN.PERKS.CAN_STUDY_WORLD_OBJECTS.DESCRIPTION);
		CanDoPlumbing = new SimpleRolePerk("CanDoPlumbing", UI.ROLES_SCREEN.PERKS.CAN_DO_PLUMBING.DESCRIPTION);
		CanUseRockets = new SimpleRolePerk("CanUseRockets", UI.ROLES_SCREEN.PERKS.CAN_USE_ROCKETS.DESCRIPTION);
		CanTrainToBeAstronaut = new SimpleRolePerk("CanTrainToBeAstronaut", UI.ROLES_SCREEN.PERKS.CAN_DO_ASTRONAUT_TRAINING.DESCRIPTION);
	}
}
