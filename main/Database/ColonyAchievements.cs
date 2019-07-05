using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using TUNING;

namespace Database
{
	public class ColonyAchievements : ResourceSet<ColonyAchievement>
	{
		public ColonyAchievement Thriving;

		public ColonyAchievement ReachedDistantPlanet;

		public ColonyAchievement SurvivedALongTime;

		public ColonyAchievement ReachedSpace;

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

		public ColonyAchievements(ResourceSet parent)
			: base("ColonyAchievements", parent)
		{
			Thriving = Add(new ColonyAchievement("Thriving", COLONY_ACHIEVEMENTS.THRIVING.NAME, COLONY_ACHIEVEMENTS.THRIVING.DESCRIPTION, true, new List<ColonyAchievementRequirement>
			{
				new CycleNumber(200),
				new MinimumMorale(16),
				new NumberOfDupes(12),
				new MonumentBuilt()
			}, COLONY_ACHIEVEMENTS.THRIVING.MESSAGE_TITLE, COLONY_ACHIEVEMENTS.THRIVING.MESSAGE_BODY, "Placeholder", "Placeholder_grey", delegate
			{
				IEnumerator enumerator2 = Components.MonumentParts.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						MonumentPart monumentPart = (MonumentPart)enumerator2.Current;
						if (monumentPart.IsMonumentCompleted())
						{
							return monumentPart.gameObject;
						}
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator2 as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
				return null;
			}, ""));
			ReachedDistantPlanet = Add(new ColonyAchievement("ReachedDistantPlanet", COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.NAME, COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.DESCRIPTION, true, new List<ColonyAchievementRequirement>
			{
				new ReachedSpace(Db.Get().SpaceDestinationTypes.Wormhole)
			}, COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.MESSAGE_TITLE, COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.MESSAGE_BODY, "Placeholder", "Placeholder_grey", delegate
			{
				foreach (Spacecraft item in SpacecraftManager.instance.GetSpacecraft())
				{
					if (item.state != 0 && SpacecraftManager.instance.GetSpacecraftDestination(item.id).GetDestinationType().Id == Db.Get().SpaceDestinationTypes.Wormhole.Id)
					{
						return item.launchConditions.rocketModules[item.launchConditions.rocketModules.Count / 2].gameObject;
					}
				}
				return null;
			}, AudioMixerSnapshots.Get().VictoryNISRocketSnapshot));
			SurvivedALongTime = Add(new ColonyAchievement("SurvivedALongTime", COLONY_ACHIEVEMENTS.SURVIVED.NAME, COLONY_ACHIEVEMENTS.SURVIVED.DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new CycleNumber(100)
			}, "", "", "", "", null, ""));
			ReachedSpace = Add(new ColonyAchievement("ReachedSpace", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACH_SPACE_ANY_DESTINATION, COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACH_SPACE_ANY_DESTINATION_DESCRIPTION, false, new List<ColonyAchievementRequirement>
			{
				new ReachedSpace(null)
			}, "", "", "", "", null, ""));
			Clothe8Dupes = Add(new ColonyAchievement("Clothe8Dupes", COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EQUIP_N_DUPES, string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.EQUIP_N_DUPES_DESCRIPTION, 8), false, new List<ColonyAchievementRequirement>
			{
				new EquipNDupes(Db.Get().AssignableSlots.Outfit, 8)
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
				}, GameTags.Creatures.Wild, false)
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
		}
	}
}
