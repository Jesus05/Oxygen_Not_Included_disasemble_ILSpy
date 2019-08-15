using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TUNING;

namespace Database
{
	public class ColonyAchievements : ResourceSet<ColonyAchievement>
	{
		public ColonyAchievement Thriving;

		public ColonyAchievement ReachedDistantPlanet;

		public ColonyAchievement Survived100Cycles;

		public ColonyAchievement ReachedSpace;

		public ColonyAchievement CompleteSkillBranch;

		public ColonyAchievement CompleteResearchTree;

		public ColonyAchievement Clothe8Dupes;

		public ColonyAchievement Build4NatureReserves;

		public ColonyAchievement Minimum20LivingDupes;

		public ColonyAchievement TameAGassyMoo;

		public ColonyAchievement CoolBuildingTo6K;

		public ColonyAchievement EatkCalFromMeatByCycle100;

		public ColonyAchievement NoFarmTilesAndKCal;

		public ColonyAchievement Generate240000kJClean;

		public ColonyAchievement BuildOutsideStartBiome;

		public ColonyAchievement Travel10000InTubes;

		public ColonyAchievement VarietyOfRooms;

		public ColonyAchievement TameAllBasicCritters;

		public ColonyAchievement SurviveOneYear;

		public ColonyAchievement ExploreOilBiome;

		public ColonyAchievement EatCookedFood;

		public ColonyAchievement BasicPumping;

		public ColonyAchievement BasicComforts;

		public ColonyAchievement PlumbedWashrooms;

		public ColonyAchievement AutomateABuilding;

		public ColonyAchievement MasterpiecePainting;

		public ColonyAchievement InspectPOI;

		public ColonyAchievement HatchACritter;

		public ColonyAchievement CuredDisease;

		public ColonyAchievement GeneratorTuneup;

		public ColonyAchievement ClearFOW;

		public ColonyAchievement HatchRefinement;

		public ColonyAchievement BunkerDoorDefense;

		public ColonyAchievement IdleDuplicants;

		[CompilerGenerated]
		private static Action<KMonoBehaviour> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Action<KMonoBehaviour> _003C_003Ef__mg_0024cache1;

		public ColonyAchievements(ResourceSet parent)
			: base("ColonyAchievements", parent)
		{
			Thriving = Add(new ColonyAchievement("Thriving", "WINCONDITION_STAY", COLONY_ACHIEVEMENTS.THRIVING.NAME, COLONY_ACHIEVEMENTS.THRIVING.DESCRIPTION, true, new List<ColonyAchievementRequirement>
			{
				new CycleNumber(200),
				new MinimumMorale(16),
				new NumberOfDupes(12),
				new MonumentBuilt()
			}, COLONY_ACHIEVEMENTS.THRIVING.MESSAGE_TITLE, COLONY_ACHIEVEMENTS.THRIVING.MESSAGE_BODY, "victoryShorts/Stay", "victoryLoops/Stay_loop", ThrivingSequence.Start, AudioMixerSnapshots.Get().VictoryNISGenericSnapshot, "home_sweet_home"));
			ReachedDistantPlanet = Add(new ColonyAchievement("ReachedDistantPlanet", "WINCONDITION_LEAVE", COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.NAME, COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.DESCRIPTION, true, new List<ColonyAchievementRequirement>
			{
				new ReachedSpace(Db.Get().SpaceDestinationTypes.Wormhole)
			}, COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.MESSAGE_TITLE, COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.MESSAGE_BODY, "victoryShorts/Leave", "victoryLoops/Leave_loop", ReachedDistantPlanetSequence.Start, AudioMixerSnapshots.Get().VictoryNISRocketSnapshot, "rocket"));
			Survived100Cycles = Add(new ColonyAchievement("Survived100Cycles", "SURVIVE_HUNDRED_CYCLES", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SURVIVE_HUNDRED_CYCLES, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SURVIVE_HUNDRED_CYCLES_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new CycleNumber(100)
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, string.Empty));
			ReachedSpace = Add(new ColonyAchievement("ReachedSpace", "REACH_SPACE_ANY_DESTINATION", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACH_SPACE_ANY_DESTINATION, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACH_SPACE_ANY_DESTINATION_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new ReachedSpace(null)
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, string.Empty));
			CompleteSkillBranch = Add(new ColonyAchievement("CompleteSkillBranch", "COMPLETED_SKILL_BRANCH", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COMPLETED_SKILL_BRANCH, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COMPLETED_SKILL_BRANCH_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new SkillBranchComplete(new List<Skill>
				{
					Db.Get().Skills.Mining3,
					Db.Get().Skills.Building3,
					Db.Get().Skills.Farming3,
					Db.Get().Skills.Ranching2,
					Db.Get().Skills.Researching3,
					Db.Get().Skills.Cooking2,
					Db.Get().Skills.Arting3,
					Db.Get().Skills.Hauling2,
					Db.Get().Skills.Technicals2,
					Db.Get().Skills.Engineering1,
					Db.Get().Skills.Basekeeping2,
					Db.Get().Skills.Astronauting2,
					Db.Get().Skills.Medicine3
				})
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, string.Empty));
			CompleteResearchTree = Add(new ColonyAchievement("CompleteResearchTree", "COMPLETED_RESEARCH", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COMPLETED_RESEARCH, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COMPLETED_RESEARCH_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new ResearchComplete()
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, "honorary_doctorate"));
			Clothe8Dupes = Add(new ColonyAchievement("Clothe8Dupes", "EQUIP_EIGHT_DUPES", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EQUIP_N_DUPES, string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EQUIP_N_DUPES_DESCRIPTION, 8), false, new List<ColonyAchievementRequirement>
			{
				new EquipNDupes(Db.Get().AssignableSlots.Outfit, 8)
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, string.Empty));
			TameAllBasicCritters = Add(new ColonyAchievement("TameAllBasicCritters", "TAME_BASIC_CRITTERS", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TAME_BASIC_CRITTERS, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TAME_BASIC_CRITTERS_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new CritterTypesWithTraits(new List<Tag>
				{
					"Drecko",
					"Hatch",
					"LightBug",
					"Mole",
					"Oilfloater",
					"Pacu",
					"Puft",
					"Moo",
					"Crab",
					"Squirrel"
				}, false)
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, "Animal_friends"));
			Build4NatureReserves = Add(new ColonyAchievement("Build4NatureReserves", "BUILD_NATURE_RESERVES", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUILD_NATURE_RESERVES, string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUILD_NATURE_RESERVES_DESCRIPTION, Db.Get().RoomTypes.NatureReserve.Name, 4), false, new List<ColonyAchievementRequirement>
			{
				new BuildNRoomTypes(Db.Get().RoomTypes.NatureReserve, 4)
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, "Some_Reservations"));
			Minimum20LivingDupes = Add(new ColonyAchievement("Minimum20LivingDupes", "TWENTY_DUPES", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TWENTY_DUPES, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TWENTY_DUPES_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new NumberOfDupes(20)
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, string.Empty));
			TameAGassyMoo = Add(new ColonyAchievement("TameAGassyMoo", "TAME_GASSYMOO", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TAME_GASSYMOO, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TAME_GASSYMOO_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new CritterTypesWithTraits(new List<Tag>
				{
					"Moo"
				}, false)
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, "moovin_on_up"));
			CoolBuildingTo6K = Add(new ColonyAchievement("CoolBuildingTo6K", "SIXKELVIN_BUILDING", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SIXKELVIN_BUILDING, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SIXKELVIN_BUILDING_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new CoolBuildingToXKelvin(6)
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, "not_0k"));
			EatkCalFromMeatByCycle100 = Add(new ColonyAchievement("EatkCalFromMeatByCycle100", "EAT_MEAT", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EAT_MEAT, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EAT_MEAT_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new BeforeCycleNumber(100),
				new EatXCaloriesFromY(400000, new List<string>
				{
					FOOD.FOOD_TYPES.MEAT.Id,
					FOOD.FOOD_TYPES.FISH_MEAT.Id,
					FOOD.FOOD_TYPES.COOKED_MEAT.Id,
					FOOD.FOOD_TYPES.COOKED_FISH.Id,
					FOOD.FOOD_TYPES.SURF_AND_TURF.Id,
					FOOD.FOOD_TYPES.BURGER.Id
				})
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, "Carnivore"));
			NoFarmTilesAndKCal = Add(new ColonyAchievement("NoFarmTilesAndKCal", "NO_PLANTERBOX", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.NO_PLANTERBOX, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.NO_PLANTERBOX_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new NoFarmables(),
				new EatXCalories(400000)
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, "Locavore"));
			Generate240000kJClean = Add(new ColonyAchievement("Generate240000kJClean", "CLEAN_ENERGY", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.CLEAN_ENERGY, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.CLEAN_ENERGY_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new ProduceXEngeryWithoutUsingYList(240000f, new List<Tag>
				{
					"MethaneGenerator",
					"PetroleumGenerator",
					"WoodGasGenerator",
					"Generator"
				})
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, "sustainably_sustaining"));
			BuildOutsideStartBiome = Add(new ColonyAchievement("BuildOutsideStartBiome", "BUILD_OUTSIDE_BIOME", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUILD_OUTSIDE_BIOME, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUILD_OUTSIDE_BIOME_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new BuildOutsideStartBiome()
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, string.Empty));
			Travel10000InTubes = Add(new ColonyAchievement("Travel10000InTubes", "TUBE_TRAVEL_DISTANCE", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TUBE_TRAVEL_DISTANCE, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TUBE_TRAVEL_DISTANCE_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new TravelXUsingTransitTubes(NavType.Tube, 10000)
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, "Totally-Tubular"));
			VarietyOfRooms = Add(new ColonyAchievement("VarietyOfRooms", "VARIETY_OF_ROOMS", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.VARIETY_OF_ROOMS, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.VARIETY_OF_ROOMS_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new BuildRoomType(Db.Get().RoomTypes.NatureReserve),
				new BuildRoomType(Db.Get().RoomTypes.Hospital),
				new BuildRoomType(Db.Get().RoomTypes.RecRoom),
				new BuildRoomType(Db.Get().RoomTypes.GreatHall),
				new BuildRoomType(Db.Get().RoomTypes.Bedroom),
				new BuildRoomType(Db.Get().RoomTypes.PlumbedBathroom),
				new BuildRoomType(Db.Get().RoomTypes.Farm),
				new BuildRoomType(Db.Get().RoomTypes.CreaturePen)
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, "Get-a-Room"));
			SurviveOneYear = Add(new ColonyAchievement("SurviveOneYear", "SURVIVE_ONE_YEAR", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SURVIVE_ONE_YEAR, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SURVIVE_ONE_YEAR_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new FractionalCycleNumber(365.25f)
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, "One_year"));
			ExploreOilBiome = Add(new ColonyAchievement("ExploreOilBiome", "EXPLORE_OIL_BIOME", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EXPLORE_OIL_BIOME, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EXPLORE_OIL_BIOME_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new ExploreOilFieldSubZone()
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, "enter_oil_biome"));
			EatCookedFood = Add(new ColonyAchievement("EatCookedFood", "COOKED_FOOD", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COOKED_FOOD, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COOKED_FOOD_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new EatXKCalProducedByY(1, new List<Tag>
				{
					"GourmetCookingStation",
					"CookingStation"
				})
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, "its_not_raw"));
			BasicPumping = Add(new ColonyAchievement("BasicPumping", "BASIC_PUMPING", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BASIC_PUMPING, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BASIC_PUMPING_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new VentXKG(SimHashes.Oxygen, 1000f)
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, string.Empty));
			BasicComforts = Add(new ColonyAchievement("BasicComforts", "BASIC_COMFORTS", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BASIC_COMFORTS, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BASIC_COMFORTS_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new AtLeastOneBuildingForEachDupe(new List<Tag>
				{
					"FlushToilet",
					"Outhouse"
				}),
				new AtLeastOneBuildingForEachDupe(new List<Tag>
				{
					BedConfig.ID,
					LuxuryBedConfig.ID
				})
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, string.Empty));
			PlumbedWashrooms = Add(new ColonyAchievement("PlumbedWashrooms", "PLUMBED_WASHROOMS", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.PLUMBED_WASHROOMS, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.PLUMBED_WASHROOMS_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new UpgradeAllBasicBuildings("Outhouse", "FlushToilet"),
				new UpgradeAllBasicBuildings("WashBasin", "WashSink")
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, "royal_flush"));
			AutomateABuilding = Add(new ColonyAchievement("AutomateABuilding", "AUTOMATE_A_BUILDING", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.AUTOMATE_A_BUILDING, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.AUTOMATE_A_BUILDING_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new AutomateABuilding()
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, string.Empty));
			MasterpiecePainting = Add(new ColonyAchievement("MasterpiecePainting", "MASTERPIECE_PAINTING", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.MASTERPIECE_PAINTING, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.MASTERPIECE_PAINTING_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new CreateMasterPainting()
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, string.Empty));
			InspectPOI = Add(new ColonyAchievement("InspectPOI", "INSPECT_POI", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.INSPECT_POI, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.INSPECT_POI_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new ActivateLorePOI()
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, string.Empty));
			HatchACritter = Add(new ColonyAchievement("HatchACritter", "HATCH_A_CRITTER", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.HATCH_A_CRITTER, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.HATCH_A_CRITTER_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new CritterTypeExists(new List<Tag>
				{
					"DreckoPlasticBaby",
					"HatchHardBaby",
					"HatchMetalBaby",
					"HatchVeggieBaby",
					"LightBugBlackBaby",
					"LightBugBlueBaby",
					"LightBugCrystalBaby",
					"LightBugOrangeBaby",
					"LightBugPinkBaby",
					"LightBugPurpleBaby",
					"OilfloaterDecorBaby",
					"OilfloaterHighTempBaby",
					"PacuCleanerBaby",
					"PacuTropicalBaby",
					"PuftBleachstoneBaby",
					"PuftOxyliteBaby"
				})
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, string.Empty));
			CuredDisease = Add(new ColonyAchievement("CuredDisease", "CURED_DISEASE", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.CURED_DISEASE, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.CURED_DISEASE_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new CureDisease()
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, string.Empty));
			GeneratorTuneup = Add(new ColonyAchievement("GeneratorTuneup", "GENERATOR_TUNEUP", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.GENERATOR_TUNEUP, string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.GENERATOR_TUNEUP_DESCRIPTION, 100), false, new List<ColonyAchievementRequirement>
			{
				new TuneUpGenerator(100f)
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, string.Empty));
			ClearFOW = Add(new ColonyAchievement("ClearFOW", "CLEAR_FOW", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.CLEAR_FOW, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.CLEAR_FOW_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new RevealAsteriod(0.8f)
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, string.Empty));
			HatchRefinement = Add(new ColonyAchievement("HatchRefinement", "HATCH_REFINEMENT", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.HATCH_REFINEMENT, string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.HATCH_REFINEMENT_DESCRIPTION, GameUtil.GetFormattedMass(10000f, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}")), false, new List<ColonyAchievementRequirement>
			{
				new CreaturePoopKGProduction("HatchMetal", 10000f)
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, string.Empty));
			BunkerDoorDefense = Add(new ColonyAchievement("BunkerDoorDefense", "BUNKER_DOOR_DEFENSE", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUNKER_DOOR_DEFENSE, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUNKER_DOOR_DEFENSE_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new BlockedCometWithBunkerDoor()
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, string.Empty));
			IdleDuplicants = Add(new ColonyAchievement("IdleDuplicants", "IDLE_DUPLICANTS", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.IDLE_DUPLICANTS, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.IDLE_DUPLICANTS_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new DupesVsSolidTransferArmFetch(0.51f, 5)
			}, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, string.Empty));
		}
	}
}
