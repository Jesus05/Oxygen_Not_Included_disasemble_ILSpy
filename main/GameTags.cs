using System.Collections.Generic;

public class GameTags
{
	public abstract class ChoreTypes
	{
		public static readonly Tag Farming = TagManager.Create("Farming");

		public static readonly Tag Ranching = TagManager.Create("Ranching");

		public static readonly Tag Research = TagManager.Create("Research");

		public static readonly Tag Power = TagManager.Create("Power");

		public static readonly Tag Building = TagManager.Create("Building");

		public static readonly Tag Cooking = TagManager.Create("Cooking");

		public static readonly Tag Fabricating = TagManager.Create("Fabricating");

		public static readonly Tag Wiring = TagManager.Create("Wiring");

		public static readonly Tag Art = TagManager.Create("Art");

		public static readonly Tag Digging = TagManager.Create("Digging");

		public static readonly Tag Doctoring = TagManager.Create("Doctoring");

		public static readonly Tag Conveyor = TagManager.Create("Conveyor");
	}

	public static class Creatures
	{
		public static class Species
		{
			public static readonly Tag HatchSpecies = TagManager.Create("HatchSpecies");

			public static readonly Tag LightBugSpecies = TagManager.Create("LightBugSpecies");

			public static readonly Tag OilFloaterSpecies = TagManager.Create("OilFloaterSpecies");

			public static readonly Tag DreckoSpecies = TagManager.Create("DreckoSpecies");

			public static readonly Tag GlomSpecies = TagManager.Create("GlomSpecies");

			public static readonly Tag PuftSpecies = TagManager.Create("PuftSpecies");

			public static readonly Tag PacuSpecies = TagManager.Create("PacuSpecies");

			public static readonly Tag MooSpecies = TagManager.Create("MooSpecies");

			public static readonly Tag MoleSpecies = TagManager.Create("MoleSpecies");

			public static readonly Tag SquirrelSpecies = TagManager.Create("SquirrelSpecies");

			public static readonly Tag CrabSpecies = TagManager.Create("CrabSpecies");
		}

		public static class Behaviours
		{
			public static readonly Tag GrowUpBehaviour = TagManager.Create("GrowUpBehaviour");

			public static readonly Tag SleepBehaviour = TagManager.Create("SleepBehaviour");

			public static readonly Tag CallAdultBehaviour = TagManager.Create("CallAdultBehaviour");

			public static readonly Tag PlayInterruptAnim = TagManager.Create("PlayInterruptAnim");
		}

		public static readonly Tag ReservedByCreature = TagManager.Create("ReservedByCreature");

		public static readonly Tag Bagged = TagManager.Create("Bagged");

		public static readonly Tag InIncubator = TagManager.Create("InIncubator");

		public static readonly Tag Deliverable = TagManager.Create("Deliverable");

		public static readonly Tag Stunned = TagManager.Create("Stunned");

		public static readonly Tag Falling = TagManager.Create("Falling");

		public static readonly Tag Flopping = TagManager.Create("Flopping");

		public static readonly Tag WantsToEnterBurrow = TagManager.Create("WantsToBurrow");

		public static readonly Tag Burrowed = TagManager.Create("Burrowed");

		public static readonly Tag WantsToExitBurrow = TagManager.Create("WantsToExitBurrow");

		public static readonly Tag WantsToEat = TagManager.Create("WantsToEat");

		public static readonly Tag WantsToGetRanched = TagManager.Create("WantsToGetRanched");

		public static readonly Tag WantsToGetCaptured = TagManager.Create("WantsToGetCaptured");

		public static readonly Tag WantsToClimbTree = TagManager.Create("WantsToClimbTree");

		public static readonly Tag WantsToPlantSeed = TagManager.Create("WantsToPlantSeed");

		public static readonly Tag Flee = TagManager.Create("Flee");

		public static readonly Tag Attack = TagManager.Create("Attack");

		public static readonly Tag Defend = TagManager.Create("Defend");

		public static readonly Tag ReturnToEgg = TagManager.Create("ReturnToEgg");

		public static readonly Tag CrabFriend = TagManager.Create("CrabFriend");

		public static readonly Tag Die = TagManager.Create("Die");

		public static readonly Tag Poop = TagManager.Create("Poop");

		public static readonly Tag MoveToLure = TagManager.Create("MoveToLure");

		public static readonly Tag Drowning = TagManager.Create("Drowning");

		public static readonly Tag Hungry = TagManager.Create("Hungry");

		public static readonly Tag Flyer = TagManager.Create("Flyer");

		public static readonly Tag FishTrapLure = TagManager.Create("FishTrapLure");

		public static readonly Tag Walker = TagManager.Create("Walker");

		public static readonly Tag Hoverer = TagManager.Create("Hoverer");

		public static readonly Tag Swimmer = TagManager.Create("Swimmer");

		public static readonly Tag Fertile = TagManager.Create("Fertile");

		public static readonly Tag Submerged = TagManager.Create("Submerged");

		public static readonly Tag ExitSubmerged = TagManager.Create("ExitSubmerged");

		public static readonly Tag WantsToDropElements = TagManager.Create("WantsToDropElements");

		public static readonly Tag OriginallyWild = TagManager.Create("Wild");

		public static readonly Tag Wild = TagManager.Create("Wild");

		public static readonly Tag Overcrowded = TagManager.Create("Overcrowded");

		public static readonly Tag Expecting = TagManager.Create("Expecting");

		public static readonly Tag Confined = TagManager.Create("Confined");

		public static readonly Tag Digger = TagManager.Create("Digger");

		public static readonly Tag Tunnel = TagManager.Create("Tunnel");

		public static readonly Tag ScalesGrown = TagManager.Create("ScalesGrown");

		public static readonly Tag CanMolt = TagManager.Create("CanMolt");
	}

	public static readonly Tag DeprecatedContent = TagManager.Create("DeprecatedContent");

	public static readonly Tag Any = TagManager.Create("Any");

	public static readonly Tag SpawnsInWorld = TagManager.Create("SpawnsInWorld");

	public static readonly Tag Experimental = TagManager.Create("Experimental");

	public static readonly Tag Miscellaneous = TagManager.Create("Miscellaneous");

	public static readonly Tag Specimen = TagManager.Create("Specimen");

	public static readonly Tag Seed = TagManager.Create("Seed");

	public static readonly Tag Edible = TagManager.Create("Edible");

	public static readonly Tag CookingIngredient = TagManager.Create("CookingIngredient");

	public static readonly Tag Medicine = TagManager.Create("Medicine");

	public static readonly Tag MedicalSupplies = TagManager.Create("MedicalSupplies");

	public static readonly Tag Plant = TagManager.Create("Plant");

	public static readonly Tag GrowingPlant = TagManager.Create("GrowingPlant");

	public static readonly Tag Pickupable = TagManager.Create("Pickupable");

	public static readonly Tag Liquifiable = TagManager.Create("Liquifiable");

	public static readonly Tag IceOre = TagManager.Create("IceOre");

	public static readonly Tag OxyRock = TagManager.Create("OxyRock");

	public static readonly Tag Life = TagManager.Create("Life");

	public static readonly Tag Fertilizer = TagManager.Create("Fertilizer");

	public static readonly Tag Farmable = TagManager.Create("Farmable");

	public static readonly Tag Agriculture = TagManager.Create("Agriculture");

	public static readonly Tag Organics = TagManager.Create("Organics");

	public static readonly Tag IndustrialProduct = TagManager.Create("IndustrialProduct");

	public static readonly Tag IndustrialIngredient = TagManager.Create("IndustrialIngredient");

	public static readonly Tag Other = TagManager.Create("Other");

	public static readonly Tag ManufacturedMaterial = TagManager.Create("ManufacturedMaterial");

	public static readonly Tag Plastic = TagManager.Create("Plastic");

	public static readonly Tag Steel = TagManager.Create("Steel");

	public static readonly Tag BuildableAny = TagManager.Create("BuildableAny");

	public static readonly Tag Decoration = TagManager.Create("Decoration");

	public static readonly Tag Window = TagManager.Create("Window");

	public static readonly Tag Bunker = TagManager.Create("Bunker");

	public static readonly Tag Transition = TagManager.Create("Transition");

	public static readonly Tag Detecting = TagManager.Create("Detecting");

	public static readonly Tag RareMaterials = TagManager.Create("RareMaterials");

	public static readonly Tag BuildingFiber = TagManager.Create("BuildingFiber");

	public static readonly Tag PreciousRock = TagManager.Create("PreciousRock");

	public static readonly Tag Artifact = TagManager.Create("Artifact");

	public static readonly Tag MiscPickupable = TagManager.Create("MiscPickupable");

	public static readonly Tag CombustibleGas = TagManager.Create("CombustibleGas");

	public static readonly Tag CombustibleLiquid = TagManager.Create("CombustibleLiquid");

	public static readonly Tag CombustibleSolid = TagManager.Create("CombustibleSolid");

	public static readonly Tag FlyingCritterEdible = TagManager.Create("FlyingCritterEdible");

	public static readonly Tag Incapacitated = TagManager.Create("Incapacitated");

	public static readonly Tag CaloriesDepleted = TagManager.Create("CaloriesDepleted");

	public static readonly Tag HitPointsDepleted = TagManager.Create("HitPointsDepleted");

	public static readonly Tag Wilting = TagManager.Create("Wilting");

	public static readonly Tag PreventEmittingDisease = TagManager.Create("EmittingDisease");

	public static readonly Tag Creature = TagManager.Create("Creature");

	public static readonly Tag Hexaped = TagManager.Create("Hexaped");

	public static readonly Tag HeatBulb = TagManager.Create("HeatBulb");

	public static readonly Tag Egg = TagManager.Create("Egg");

	public static readonly Tag IncubatableEgg = TagManager.Create("IncubatableEgg");

	public static readonly Tag Trapped = TagManager.Create("Trapped");

	public static readonly Tag BagableCreature = TagManager.Create("BagableCreature");

	public static readonly Tag SwimmingCreature = TagManager.Create("SwimmingCreature");

	public static readonly Tag Spawner = TagManager.Create("Spawner");

	public static readonly Tag FullyIncubated = TagManager.Create("FullyIncubated");

	public static readonly Tag Amphibious = TagManager.Create("Amphibious");

	public static readonly Tag Alloy = TagManager.Create("Alloy");

	public static readonly Tag Metal = TagManager.Create("Metal");

	public static readonly Tag RefinedMetal = TagManager.Create("RefinedMetal");

	public static readonly Tag PreciousMetal = TagManager.Create("PreciousMetal");

	public static readonly Tag StoredMetal = TagManager.Create("StoredMetal");

	public static readonly Tag Solid = TagManager.Create("Solid");

	public static readonly Tag Liquid = TagManager.Create("Liquid");

	public static readonly Tag LiquidSource = TagManager.Create("LiquidSource");

	public static readonly Tag Water = TagManager.Create("Water");

	public static readonly Tag DirtyWater = TagManager.Create("DirtyWater");

	public static readonly Tag AnyWater = TagManager.Create("AnyWater");

	public static readonly Tag Algae = TagManager.Create("Algae");

	public static readonly Tag Void = TagManager.Create("Void");

	public static readonly Tag Chlorine = TagManager.Create("Chlorine");

	public static readonly Tag Oxygen = TagManager.Create("Oxygen");

	public static readonly Tag Hydrogen = TagManager.Create("Hydrogen");

	public static readonly Tag Methane = TagManager.Create("Methane");

	public static readonly Tag CarbonDioxide = TagManager.Create("CarbonDioxide");

	public static readonly Tag Carbon = TagManager.Create("Carbon");

	public static readonly Tag BuildableRaw = TagManager.Create("BuildableRaw");

	public static readonly Tag BuildableProcessed = TagManager.Create("BuildableProcessed");

	public static readonly Tag Phosphorus = TagManager.Create("Phosphorus");

	public static readonly Tag Phosphorite = TagManager.Create("Phosphorite");

	public static readonly Tag SlimeMold = TagManager.Create("SlimeMold");

	public static readonly Tag Filler = TagManager.Create("Filler");

	public static readonly Tag Item = TagManager.Create("Item");

	public static readonly Tag Ore = TagManager.Create("Ore");

	public static readonly Tag GenericOre = TagManager.Create("GenericOre");

	public static readonly Tag Ingot = TagManager.Create("Ingot");

	public static readonly Tag Dirt = TagManager.Create("Dirt");

	public static readonly Tag Filter = TagManager.Create("Filter");

	public static readonly Tag ConsumableOre = TagManager.Create("ConsumableOre");

	public static readonly Tag Unstable = TagManager.Create("Unstable");

	public static readonly Tag EmitsLight = TagManager.Create("EmitsLight");

	public static readonly Tag Toxic = TagManager.Create("Toxic");

	public static readonly Tag Special = TagManager.Create("Special");

	public static readonly Tag Breathable = TagManager.Create("Breathable");

	public static readonly Tag Unbreathable = TagManager.Create("Unbreathable");

	public static readonly Tag Gas = TagManager.Create("Gas");

	public static readonly Tag Crushable = TagManager.Create("Crushable");

	public static readonly Tag IronOre = TagManager.Create("IronOre");

	public static readonly Tag Minion = TagManager.Create("Minion");

	public static readonly Tag Corpse = TagManager.Create("Corpse");

	public static readonly Tag RiverSource = TagManager.Create("RiverSource");

	public static readonly Tag RiverSink = TagManager.Create("RiverSink");

	public static readonly Tag Garbage = TagManager.Create("Garbage");

	public static readonly Tag OilWell = TagManager.Create("OilWell");

	public static readonly Tag Glass = TagManager.Create("Glass");

	public static readonly Tag Door = TagManager.Create("Door");

	public static readonly Tag Farm = TagManager.Create("Farm");

	public static readonly Tag StorageLocker = TagManager.Create("StorageLocker");

	public static readonly Tag FloorTiles = TagManager.Create("FloorTiles");

	public static readonly Tag FarmTiles = TagManager.Create("FarmTiles");

	public static readonly Tag Ladders = TagManager.Create("Ladders");

	public static readonly Tag Wires = TagManager.Create("Wires");

	public static readonly Tag Vents = TagManager.Create("Vents");

	public static readonly Tag Pipes = TagManager.Create("Pipes");

	public static readonly Tag WireBridges = TagManager.Create("WireBridges");

	public static readonly Tag TravelTubeBridges = TagManager.Create("TravelTubeBridges");

	public static readonly Tag MISSING_TAG = TagManager.Create("MISSING_TAG");

	public static readonly Tag PlantRenderer = TagManager.Create("PlantRenderer");

	public static readonly Tag Usable = TagManager.Create("Usable");

	public static readonly Tag PedestalDisplayable = TagManager.Create("PedestalDisplayable");

	public static readonly Tag HasChores = TagManager.Create("HasChores");

	public static readonly Tag Suit = TagManager.Create("Suit");

	public static readonly Tag AirtightSuit = TagManager.Create("AirtightSuit");

	public static readonly Tag AtmoSuit = TagManager.Create("Atmo_Suit");

	public static readonly Tag AquaSuit = TagManager.Create("Aqua_Suit");

	public static readonly Tag JetSuit = TagManager.Create("Jet_Suit");

	public static readonly Tag JetSuitOutOfFuel = TagManager.Create("JetSuitOutOfFuel");

	public static readonly Tag TemperatureSuit = TagManager.Create("Temperature_Suit");

	public static readonly List<Tag> AllSuitTags = new List<Tag>
	{
		Suit,
		AquaSuit,
		AtmoSuit,
		JetSuit,
		TemperatureSuit
	};

	public static readonly List<Tag> OxygenSuitTags = new List<Tag>
	{
		AtmoSuit,
		AquaSuit,
		JetSuit
	};

	public static readonly Tag Clothes = TagManager.Create("Clothes");

	public static readonly Tag WarmVest = TagManager.Create("Warm_Vest");

	public static readonly Tag CoolVest = TagManager.Create("Cool_Vest");

	public static readonly Tag FunkyVest = TagManager.Create("Funky_Vest");

	public static readonly List<Tag> AllClothesTags = new List<Tag>
	{
		Clothes,
		WarmVest,
		CoolVest,
		FunkyVest
	};

	public static readonly Tag Assigned = TagManager.Create("Assigned");

	public static readonly Tag Helmet = TagManager.Create("Helmet");

	public static readonly Tag Equipped = TagManager.Create("Equipped");

	public static readonly Tag Entombed = TagManager.Create("Entombed");

	public static readonly Tag Preserved = TagManager.Create("Preserved");

	public static readonly Tag Compostable = TagManager.Create("Compostable");

	public static readonly Tag MarkedForCompost = TagManager.Create("MarkedForCompost");

	public static readonly Tag Pickled = TagManager.Create("Pickled");

	public static readonly Tag Dying = TagManager.Create("Dying");

	public static readonly Tag Dead = TagManager.Create("Dead");

	public static readonly Tag Reachable = TagManager.Create("Reachable");

	public static readonly Tag PreventChoreInterruption = TagManager.Create("PreventChoreInterruption");

	public static readonly Tag PerformingWorkRequest = TagManager.Create("PerformingWorkRequest");

	public static readonly Tag RecoveringBreath = TagManager.Create("RecoveringBreath");

	public static readonly Tag NoOxygen = TagManager.Create("NoOxygen");

	public static readonly Tag Idle = TagManager.Create("Idle");

	public static readonly Tag AlwaysConverse = TagManager.Create("AlwaysConverse");

	public static readonly Tag HasDebugDestination = TagManager.Create("HasDebugDestination");

	public static readonly Tag Shaded = TagManager.Create("Shaded");

	public static readonly Tag TakingMedicine = TagManager.Create("TakingMedicine");

	public static readonly Tag DupeBrain = TagManager.Create("DupeBrain");

	public static readonly Tag CreatureBrain = TagManager.Create("CreatureBrain");

	public static readonly Tag Asleep = TagManager.Create("Asleep");

	public static readonly Tag HoldingBreath = TagManager.Create("HoldingBreath");

	public static readonly Tag Operational = TagManager.Create("Operational");

	public static readonly Tag JetSuitBlocker = TagManager.Create("JetSuitBlocker");

	public static readonly Tag HasInvalidPorts = TagManager.Create("HasInvalidPorts");

	public static readonly Tag NotRoomAssignable = TagManager.Create("NotRoomAssignable");

	public static readonly Tag OneTimeUseLure = TagManager.Create("OneTimeUseLure");

	public static readonly Tag TemplateBuilding = TagManager.Create("TemplateBuilding");

	public static readonly Tag Rocket = TagManager.Create("Rocket");

	public static readonly Tag RocketNotOnGround = TagManager.Create("RocketNotOnGround");

	public static readonly Tag Monument = TagManager.Create("Monument");

	public static readonly Tag Stored = TagManager.Create("Stored");

	public static readonly Tag StoredPrivate = TagManager.Create("StoredPrivate");

	public static readonly Tag Sealed = TagManager.Create("Sealed");

	public static readonly Tag CropSeed = TagManager.Create("CropSeed");

	public static readonly Tag DecorSeed = TagManager.Create("DecorSeed");

	public static readonly Tag WaterSeed = TagManager.Create("WaterSeed");

	public static readonly Tag Harvestable = TagManager.Create("Harvestable");

	public static readonly Tag Hanging = TagManager.Create("Hanging");

	public static readonly Tag FarmingMaterial = TagManager.Create("FarmingMaterial");

	public static readonly Tag MassChunk = TagManager.Create("MassChunk");

	public static readonly Tag UnitChunk = TagManager.Create("UnitChunk");

	public static readonly Tag NotConversationTopic = TagManager.Create("NotConversationTopic");

	public static readonly Tag MinionSelectPreview = TagManager.Create("MinionSelectPreview");

	public static readonly Tag Empty = TagManager.Create("Empty");

	public static TagSet SolidElements = new TagSet();

	public static TagSet LiquidElements = new TagSet();

	public static TagSet GasElements = new TagSet();

	public static TagSet CalorieCategories = new TagSet
	{
		Edible
	};

	public static TagSet UnitCategories = new TagSet
	{
		Medicine,
		MedicalSupplies,
		Seed,
		Egg,
		Clothes,
		IndustrialIngredient,
		MarkedForCompost
	};

	public static TagSet IgnoredMaterialCategories = new TagSet
	{
		Special
	};

	public static TagSet MaterialCategories = new TagSet
	{
		Alloy,
		Metal,
		RefinedMetal,
		BuildableRaw,
		BuildableProcessed,
		Filter,
		Liquifiable,
		Liquid,
		Breathable,
		Unbreathable,
		ConsumableOre,
		Organics,
		Farmable,
		Agriculture,
		Other,
		ManufacturedMaterial,
		CookingIngredient,
		RareMaterials
	};

	public static TagSet MaterialBuildingElements = new TagSet
	{
		BuildingFiber
	};

	public static TagSet OtherEntityTags = new TagSet
	{
		BagableCreature,
		SwimmingCreature,
		MiscPickupable
	};

	public static TagSet AllCategories = new TagSet(CalorieCategories, UnitCategories, MaterialCategories, OtherEntityTags);

	public static TagSet DisplayAsCalories = new TagSet(CalorieCategories);

	public static TagSet DisplayAsUnits = new TagSet(UnitCategories);

	public static TagSet DisplayAsInformation = new TagSet();
}
