using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TUNING
{
	public class CREATURES
	{
		public class HITPOINTS
		{
			public const float TIER0 = 5f;

			public const float TIER1 = 25f;

			public const float TIER2 = 50f;

			public const float TIER3 = 100f;

			public const float TIER4 = 150f;

			public const float TIER5 = 200f;

			public const float TIER6 = 400f;
		}

		public class MASS_KG
		{
			public const float TIER0 = 5f;

			public const float TIER1 = 25f;

			public const float TIER2 = 50f;

			public const float TIER3 = 100f;

			public const float TIER4 = 200f;

			public const float TIER5 = 400f;
		}

		public class TEMPERATURE
		{
			public static float FREEZING_3 = 243f;

			public static float FREEZING_2 = 253f;

			public static float FREEZING_1 = 263f;

			public static float FREEZING = 273f;

			public static float COOL = 283f;

			public static float MODERATE = 293f;

			public static float HOT = 303f;

			public static float HOT_1 = 313f;

			public static float HOT_2 = 323f;

			public static float HOT_3 = 333f;
		}

		public class LIFESPAN
		{
			public const float TIER0 = 5f;

			public const float TIER1 = 25f;

			public const float TIER2 = 75f;

			public const float TIER3 = 100f;

			public const float TIER4 = 150f;

			public const float TIER5 = 200f;

			public const float TIER6 = 400f;
		}

		public class CONVERSION_EFFICIENCY
		{
			public static float BAD_2 = 0.1f;

			public static float BAD_1 = 0.25f;

			public static float NORMAL = 0.5f;

			public static float GOOD_1 = 0.75f;

			public static float GOOD_2 = 0.95f;

			public static float GOOD_3 = 1f;
		}

		public class SPACE_REQUIREMENTS
		{
			public static int TIER2 = 8;

			public static int TIER3 = 12;

			public static int TIER4 = 16;
		}

		public class EGG_CHANCE_MODIFIERS
		{
			public static List<System.Action> MODIFIER_CREATORS = new List<System.Action>
			{
				CreateDietaryModifier("HatchHard", "HatchHardEgg".ToTag(), SimHashes.SedimentaryRock.CreateTag(), 0.05f / HatchTuning.STANDARD_CALORIES_PER_CYCLE),
				CreateDietaryModifier("HatchVeggie", "HatchVeggieEgg".ToTag(), SimHashes.Dirt.CreateTag(), 0.05f / HatchTuning.STANDARD_CALORIES_PER_CYCLE),
				CreateDietaryModifier("HatchMetal", "HatchMetalEgg".ToTag(), HatchMetalConfig.METAL_ORE_TAGS, 0.05f / HatchTuning.STANDARD_CALORIES_PER_CYCLE),
				CreateNearbyCreatureModifier("PuftAlphaBalance", "PuftAlphaEgg".ToTag(), "PuftAlpha".ToTag(), -0.00025f, true),
				CreateNearbyCreatureModifier("PuftAlphaNearbyOxylite", "PuftOxyliteEgg".ToTag(), "PuftAlpha".ToTag(), 8.333333E-05f, false),
				CreateNearbyCreatureModifier("PuftAlphaNearbyBleachstone", "PuftBleachstoneEgg".ToTag(), "PuftAlpha".ToTag(), 8.333333E-05f, false),
				CreateTemperatureModifier("OilFloaterHighTemp", "OilfloaterHighTempEgg".ToTag(), 373.15f, 523.15f, 8.333333E-05f, false),
				CreateTemperatureModifier("OilFloaterDecor", "OilfloaterDecorEgg".ToTag(), 293.15f, 333.15f, 8.333333E-05f, false),
				CreateDietaryModifier("LightBugOrange", "LightBugOrangeEgg".ToTag(), "GrilledPrickleFruit".ToTag(), 0.00125f),
				CreateDietaryModifier("LightBugPurple", "LightBugPurpleEgg".ToTag(), "FriedMushroom".ToTag(), 0.00125f),
				CreateDietaryModifier("LightBugPink", "LightBugPinkEgg".ToTag(), "SpiceBread".ToTag(), 0.00125f),
				CreateDietaryModifier("LightBugBlue", "LightBugBlueEgg".ToTag(), "Salsa".ToTag(), 0.00125f),
				CreateDietaryModifier("LightBugBlack", "LightBugBlackEgg".ToTag(), SimHashes.Phosphorus.CreateTag(), 0.00125f),
				CreateDietaryModifier("LightBugCrystal", "LightBugCrystalEgg".ToTag(), "CookedMeat".ToTag(), 0.00125f),
				CreateTemperatureModifier("PacuTropical", "PacuTropicalEgg".ToTag(), 308.15f, 353.15f, 8.333333E-05f, false),
				CreateTemperatureModifier("PacuCleaner", "PacuCleanerEgg".ToTag(), 243.15f, 278.15f, 8.333333E-05f, false),
				CreateDietaryModifier("DreckoPlastic", "DreckoPlasticEgg".ToTag(), "BasicSingleHarvestPlant".ToTag(), 0.025f / DreckoTuning.STANDARD_CALORIES_PER_CYCLE)
			};

			[CompilerGenerated]
			private static Func<Tag, string> _003C_003Ef__mg_0024cache0;

			private static System.Action CreateDietaryModifier(string id, Tag eggTag, TagBits foodTags, float modifierPerCal)
			{
				return delegate
				{
					string name = STRINGS.CREATURES.FERTILITY_MODIFIERS.DIET.NAME;
					string description = STRINGS.CREATURES.FERTILITY_MODIFIERS.DIET.DESC;
					List<Tag> foodTagsActual = foodTags.GetTagsVerySlow();
					Db.Get().CreateFertilityModifier(id, eggTag, name, description, delegate(string descStr)
					{
						string arg = string.Join(", ", foodTagsActual.Select(GameTagExtensions.ProperName).ToArray());
						descStr = string.Format(descStr, arg);
						return descStr;
					}, delegate(FertilityMonitor.Instance inst, Tag eggType)
					{
						inst.gameObject.Subscribe(-2038961714, delegate(object data)
						{
							CreatureCalorieMonitor.CaloriesConsumedEvent caloriesConsumedEvent = (CreatureCalorieMonitor.CaloriesConsumedEvent)data;
							TagBits tag_bits = new TagBits(caloriesConsumedEvent.tag);
							if (foodTags.HasAny(ref tag_bits))
							{
								inst.AddBreedingChance(eggType, caloriesConsumedEvent.calories * modifierPerCal);
							}
						});
					});
				};
			}

			private static System.Action CreateDietaryModifier(string id, Tag eggTag, Tag foodTag, float modifierPerCal)
			{
				return CreateDietaryModifier(id, eggTag, new TagBits(foodTag), modifierPerCal);
			}

			private static System.Action CreateNearbyCreatureModifier(string id, Tag eggTag, Tag nearbyCreature, float modifierPerSecond, bool alsoInvert)
			{
				return delegate
				{
					string name = (!(modifierPerSecond < 0f)) ? STRINGS.CREATURES.FERTILITY_MODIFIERS.NEARBY_CREATURE.NAME : STRINGS.CREATURES.FERTILITY_MODIFIERS.NEARBY_CREATURE_NEG.NAME;
					string description = (!(modifierPerSecond < 0f)) ? STRINGS.CREATURES.FERTILITY_MODIFIERS.NEARBY_CREATURE.DESC : STRINGS.CREATURES.FERTILITY_MODIFIERS.NEARBY_CREATURE_NEG.DESC;
					Db.Get().CreateFertilityModifier(id, eggTag, name, description, (string descStr) => string.Format(descStr, nearbyCreature.ProperName()), delegate(FertilityMonitor.Instance inst, Tag eggType)
					{
						NearbyCreatureMonitor.Instance instance = inst.gameObject.GetSMI<NearbyCreatureMonitor.Instance>();
						if (instance == null)
						{
							instance = new NearbyCreatureMonitor.Instance(inst.master);
							instance.StartSM();
						}
						instance.OnUpdateNearbyCreatures += delegate(float dt, List<KPrefabID> creatures)
						{
							bool flag = false;
							foreach (KPrefabID creature in creatures)
							{
								if (creature.PrefabTag == nearbyCreature)
								{
									flag = true;
									break;
								}
							}
							if (flag)
							{
								inst.AddBreedingChance(eggType, dt * modifierPerSecond);
							}
							else if (alsoInvert)
							{
								inst.AddBreedingChance(eggType, dt * (0f - modifierPerSecond));
							}
						};
					});
				};
			}

			private static System.Action CreateTemperatureModifier(string id, Tag eggTag, float minTemp, float maxTemp, float modifierPerSecond, bool alsoInvert)
			{
				return delegate
				{
					string name = STRINGS.CREATURES.FERTILITY_MODIFIERS.TEMPERATURE.NAME;
					Db.Get().CreateFertilityModifier(id, eggTag, name, null, (string src) => string.Format(STRINGS.CREATURES.FERTILITY_MODIFIERS.TEMPERATURE.DESC, GameUtil.GetFormattedTemperature(minTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedTemperature(maxTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), delegate(FertilityMonitor.Instance inst, Tag eggType)
					{
						TemperatureVulnerable component = inst.master.GetComponent<TemperatureVulnerable>();
						if ((UnityEngine.Object)component != (UnityEngine.Object)null)
						{
							component.OnTemperature += delegate(float dt, float newTemp)
							{
								if (newTemp > minTemp && newTemp < maxTemp)
								{
									inst.AddBreedingChance(eggType, dt * modifierPerSecond);
								}
								else if (alsoInvert)
								{
									inst.AddBreedingChance(eggType, dt * (0f - modifierPerSecond));
								}
							};
						}
						else
						{
							DebugUtil.LogErrorArgs("Ack! Trying to add temperature modifier", id, "to", inst.master.name, "but it's not temperature vulnerable!");
						}
					});
				};
			}
		}

		public const int DEFAULT_PROBING_RADIUS = 32;

		public const float FERTILITY_TIME_BY_LIFESPAN = 0.6f;

		public const float INCUBATION_TIME_BY_LIFESPAN = 0.2f;

		public const float INCUBATOR_INCUBATION_MULTIPLIER = 4f;

		public const float WILD_CALORIE_BURN_RATIO = 0.25f;

		public const float VIABILITY_LOSS_RATE = -0.0166666675f;
	}
}
