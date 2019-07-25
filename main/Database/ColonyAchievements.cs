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

		[CompilerGenerated]
		private static Action<KMonoBehaviour> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Action<KMonoBehaviour> _003C_003Ef__mg_0024cache1;

		public ColonyAchievements(ResourceSet parent)
			: base("ColonyAchievements", parent)
		{
			Thriving = Add(new ColonyAchievement("Thriving", COLONY_ACHIEVEMENTS.THRIVING.NAME, COLONY_ACHIEVEMENTS.THRIVING.DESCRIPTION, true, new List<ColonyAchievementRequirement>
			{
				new CycleNumber(200),
				new MinimumMorale(16),
				new NumberOfDupes(12),
				new MonumentBuilt()
			}, COLONY_ACHIEVEMENTS.THRIVING.MESSAGE_TITLE, COLONY_ACHIEVEMENTS.THRIVING.MESSAGE_BODY, "Placeholder", "Placeholder_grey", ThrivingSequence.Start, ""));
			ReachedDistantPlanet = Add(new ColonyAchievement("ReachedDistantPlanet", COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.NAME, COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.DESCRIPTION, true, new List<ColonyAchievementRequirement>
			{
				new ReachedSpace(Db.Get().SpaceDestinationTypes.Wormhole)
			}, COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.MESSAGE_TITLE, COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.MESSAGE_BODY, "Placeholder", "Placeholder_grey", ReachedDistantPlanetSequence.Start, AudioMixerSnapshots.Get().VictoryNISRocketSnapshot));
			Survived100Cycles = Add(new ColonyAchievement("Survived100Cycles", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SURVIVE_HUNDRED_CYCLES, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SURVIVE_HUNDRED_CYCLES_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new CycleNumber(100)
			}, "", "", "", "", null, ""));
			ReachedSpace = Add(new ColonyAchievement("ReachedSpace", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACH_SPACE_ANY_DESTINATION, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACH_SPACE_ANY_DESTINATION_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new ReachedSpace(null)
			}, "", "", "", "", null, ""));
			CompleteSkillBranch = Add(new ColonyAchievement("CompleteSkillBranch", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COMPLETED_SKILL_BRANCH, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COMPLETED_SKILL_BRANCH_DESCRIPTION, false, new List<ColonyAchievementRequirement>
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
			}, "", "", "", "", null, ""));
			CompleteResearchTree = Add(new ColonyAchievement("CompleteResearchTree", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COMPLETED_RESEARCH, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.COMPLETED_RESEARCH_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new ResearchComplete()
			}, "", "", "", "", null, ""));
			Clothe8Dupes = Add(new ColonyAchievement("Clothe8Dupes", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EQUIP_N_DUPES, string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EQUIP_N_DUPES_DESCRIPTION, 8), false, new List<ColonyAchievementRequirement>
			{
				new EquipNDupes(Db.Get().AssignableSlots.Outfit, 8)
			}, "", "", "", "", null, ""));
			TameAllBasicCritters = Add(new ColonyAchievement("TameAllBasicCritters", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TAME_BASIC_CRITTERS, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TAME_BASIC_CRITTERS_DESCRIPTION, false, new List<ColonyAchievementRequirement>
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
			}, "", "", "", "", null, ""));
			Build4NatureReserves = Add(new ColonyAchievement("Build4NatureReserves", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUILD_N_ROOM_TYPE, string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUILD_N_ROOM_TYPE_DESCRIPTION, Db.Get().RoomTypes.NatureReserve.Name, 4), false, new List<ColonyAchievementRequirement>
			{
				new BuildNRoomTypes(Db.Get().RoomTypes.NatureReserve, 4)
			}, "", "", "", "", null, ""));
			Minimum20LivingDupes = Add(new ColonyAchievement("Minimum20LivingDupes", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TWENTY_DUPES, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TWENTY_DUPES_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new NumberOfDupes(20)
			}, "", "", "", "", null, ""));
			TameAGassyMoo = Add(new ColonyAchievement("TameAGassyMoo", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TAME_GASSYMOO, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TAME_GASSYMOO_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new CritterTypesWithTraits(new List<Tag>
				{
					"Moo"
				}, false)
			}, "", "", "", "", null, ""));
			CoolBuildingTo6K = Add(new ColonyAchievement("CoolBuildingTo6K", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SIXKELVIN_BUILDING, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.SIXKELVIN_BUILDING_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new CoolBuildingToXKelvin(6)
			}, "", "", "", "", null, ""));
			EatkCalFromMeatByCycle100 = Add(new ColonyAchievement("EatkCalFromMeatByCycle100", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EAT_MEAT, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EAT_MEAT_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new BeforeCycleNumber(100),
				new EatXCaloriesFromY(400000, new List<string>
				{
					FOOD.FOOD_TYPES.MEAT.Id
				})
			}, "", "", "", "", null, ""));
			NoFarmTilesAndKCal = Add(new ColonyAchievement("NoFarmTilesAndKCal", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.NO_PLANTERBOX, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.NO_PLANTERBOX_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new NoFarmables(),
				new EatXCalories(400000)
			}, "", "", "", "", null, ""));
			Generate240000kJClean = Add(new ColonyAchievement("Generate240000kJClean", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.CLEAN_ENERGY, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.CLEAN_ENERGY_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new ProduceXEngeryWithoutUsingYList(240000f, new List<Tag>
				{
					"MethaneGenerator",
					"PetroleumGenerator",
					"WoodGasGenerator",
					"Generator"
				})
			}, "", "", "", "", null, ""));
			BuildOutsideStartBiome = Add(new ColonyAchievement("BuildOutsideStartBiome", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUILD_OUTSIDE_BIOME, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.BUILD_OUTSIDE_BIOME_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new BuildOutsideStartBiome()
			}, "", "", "", "", null, ""));
			Travel10000InTubes = Add(new ColonyAchievement("Travel10000InTubes", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TUBE_TRAVEL_DISTANCE, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.TUBE_TRAVEL_DISTANCE_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new TravelXUsingTransitTubes(NavType.Tube, 10000)
			}, "", "", "", "", null, ""));
			VarietyOfRooms = Add(new ColonyAchievement("VarietyOfRooms", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.VARIETY_OF_ROOMS, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.VARIETY_OF_ROOMS_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new BuildRoomType(Db.Get().RoomTypes.NatureReserve),
				new BuildRoomType(Db.Get().RoomTypes.Hospital),
				new BuildRoomType(Db.Get().RoomTypes.GreatHall),
				new BuildRoomType(Db.Get().RoomTypes.Bedroom),
				new BuildRoomType(Db.Get().RoomTypes.PlumbedBathroom),
				new BuildRoomType(Db.Get().RoomTypes.Farm),
				new BuildRoomType(Db.Get().RoomTypes.CreaturePen)
			}, "", "", "", "", null, ""));
		}
	}
}
