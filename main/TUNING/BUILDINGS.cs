using System;
using System.Collections.Generic;

namespace TUNING
{
	public class BUILDINGS
	{
		public class OVERPRESSURE
		{
			public const float TIER0 = 1.8f;
		}

		public class OVERHEAT_TEMPERATURES
		{
			public const float LOW_3 = 10f;

			public const float LOW_2 = 328.15f;

			public const float LOW_1 = 338.15f;

			public const float NORMAL = 348.15f;

			public const float HIGH_1 = 363.15f;

			public const float HIGH_2 = 398.15f;

			public const float HIGH_3 = 1273.15f;

			public const float HIGH_4 = 2273.15f;
		}

		public class OVERHEAT_MATERIAL_MOD
		{
			public const float LOW_3 = -200f;

			public const float LOW_2 = -20f;

			public const float LOW_1 = -10f;

			public const float NORMAL = 0f;

			public const float HIGH_1 = 15f;

			public const float HIGH_2 = 50f;

			public const float HIGH_3 = 200f;

			public const float HIGH_4 = 500f;

			public const float HIGH_5 = 900f;
		}

		public class DECOR_MATERIAL_MOD
		{
			public const float NORMAL = 0f;

			public const float HIGH_1 = 0.1f;

			public const float HIGH_2 = 0.2f;

			public const float HIGH_3 = 0.5f;

			public const float HIGH_4 = 1f;
		}

		public class CONSTRUCTION_MASS_KG
		{
			public static readonly float[] TIER_TINY = new float[1]
			{
				5f
			};

			public static readonly float[] TIER0 = new float[1]
			{
				25f
			};

			public static readonly float[] TIER1 = new float[1]
			{
				50f
			};

			public static readonly float[] TIER2 = new float[1]
			{
				100f
			};

			public static readonly float[] TIER3 = new float[1]
			{
				200f
			};

			public static readonly float[] TIER4 = new float[1]
			{
				400f
			};

			public static readonly float[] TIER5 = new float[1]
			{
				800f
			};

			public static readonly float[] TIER6 = new float[1]
			{
				1200f
			};

			public static readonly float[] TIER7 = new float[1]
			{
				2000f
			};
		}

		public class ROCKETRY_MASS_KG
		{
			public static float[] COMMAND_MODULE_MASS = new float[1]
			{
				200f
			};

			public static float[] CARGO_MASS = new float[2]
			{
				1000f,
				1000f
			};

			public static float[] FUEL_TANK_DRY_MASS = new float[1]
			{
				100f
			};

			public static float[] FUEL_TANK_WET_MASS = new float[1]
			{
				900f
			};

			public static float[] OXIDIZER_TANK_OXIDIZER_MASS = new float[1]
			{
				900f
			};

			public static float[] ENGINE_MASS_SMALL = new float[1]
			{
				200f
			};

			public static float[] ENGINE_MASS_LARGE = new float[1]
			{
				500f
			};
		}

		public class ENERGY_CONSUMPTION_WHEN_ACTIVE
		{
			public const float TIER0 = 0f;

			public const float TIER1 = 5f;

			public const float TIER2 = 60f;

			public const float TIER3 = 120f;

			public const float TIER4 = 240f;

			public const float TIER5 = 480f;

			public const float TIER6 = 960f;

			public const float TIER7 = 1200f;

			public const float TIER8 = 1600f;
		}

		public class EXHAUST_ENERGY_ACTIVE
		{
			public const float TIER0 = 0f;

			public const float TIER1 = 0.125f;

			public const float TIER2 = 0.25f;

			public const float TIER3 = 0.5f;

			public const float TIER4 = 1f;

			public const float TIER5 = 2f;

			public const float TIER6 = 4f;

			public const float TIER7 = 8f;

			public const float TIER8 = 16f;
		}

		public class SELF_HEAT_KILOWATTS
		{
			public const float TIER0 = 0f;

			public const float TIER1 = 0.5f;

			public const float TIER2 = 1f;

			public const float TIER3 = 2f;

			public const float TIER4 = 4f;

			public const float TIER5 = 8f;

			public const float TIER6 = 16f;

			public const float TIER7 = 32f;

			public const float TIER8 = 64f;
		}

		public class MELTING_POINT_KELVIN
		{
			public const float TIER0 = 800f;

			public const float TIER1 = 1600f;

			public const float TIER2 = 2400f;

			public const float TIER3 = 3200f;

			public const float TIER4 = 9999f;
		}

		public class CONSTRUCTION_TIME_SECONDS
		{
			public const float TIER0 = 3f;

			public const float TIER1 = 10f;

			public const float TIER2 = 30f;

			public const float TIER3 = 60f;

			public const float TIER4 = 120f;

			public const float TIER5 = 240f;

			public const float TIER6 = 480f;
		}

		public class HITPOINTS
		{
			public const int TIER0 = 10;

			public const int TIER1 = 30;

			public const int TIER2 = 100;

			public const int TIER3 = 250;

			public const int TIER4 = 1000;
		}

		public class DAMAGE_SOURCES
		{
			public const int CONDUIT_CONTENTS_BOILED = 1;

			public const int CONDUIT_CONTENTS_FROZE = 1;

			public const int BAD_INPUT_ELEMENT = 1;

			public const int BUILDING_OVERHEATED = 1;

			public const int HIGH_LIQUID_PRESSURE = 10;

			public const int MICROMETEORITE = 1;
		}

		public class RELOCATION_TIME_SECONDS
		{
			public const float DECONSTRUCT = 4f;

			public const float CONSTRUCT = 4f;
		}

		public class WORK_TIME_SECONDS
		{
			public const float VERYSHORT_WORK_TIME = 5f;

			public const float SHORT_WORK_TIME = 15f;

			public const float MEDIUM_WORK_TIME = 30f;

			public const float LONG_WORK_TIME = 90f;

			public const float EXTENSIVE_WORK_TIME = 180f;
		}

		public class FABRICATION_TIME_SECONDS
		{
			public const float SHORT = 40f;

			public const float MODERATE = 80f;

			public const float LONG = 250f;
		}

		public class DECOR
		{
			public class BONUS
			{
				public static readonly EffectorValues TIER0 = new EffectorValues
				{
					amount = 5,
					radius = 1
				};

				public static readonly EffectorValues TIER1 = new EffectorValues
				{
					amount = 10,
					radius = 2
				};

				public static readonly EffectorValues TIER2 = new EffectorValues
				{
					amount = 15,
					radius = 3
				};

				public static readonly EffectorValues TIER3 = new EffectorValues
				{
					amount = 20,
					radius = 4
				};

				public static readonly EffectorValues TIER4 = new EffectorValues
				{
					amount = 25,
					radius = 5
				};

				public static readonly EffectorValues TIER5 = new EffectorValues
				{
					amount = 30,
					radius = 6
				};
			}

			public class PENALTY
			{
				public static readonly EffectorValues TIER0 = new EffectorValues
				{
					amount = -5,
					radius = 1
				};

				public static readonly EffectorValues TIER1 = new EffectorValues
				{
					amount = -10,
					radius = 2
				};

				public static readonly EffectorValues TIER2 = new EffectorValues
				{
					amount = -15,
					radius = 3
				};

				public static readonly EffectorValues TIER3 = new EffectorValues
				{
					amount = -20,
					radius = 4
				};

				public static readonly EffectorValues TIER4 = new EffectorValues
				{
					amount = -20,
					radius = 5
				};

				public static readonly EffectorValues TIER5 = new EffectorValues
				{
					amount = -25,
					radius = 6
				};
			}

			public static readonly EffectorValues NONE = new EffectorValues
			{
				amount = 0,
				radius = 1
			};
		}

		public class MASS_KG
		{
			public const float TIER0 = 25f;

			public const float TIER1 = 50f;

			public const float TIER2 = 100f;

			public const float TIER3 = 200f;

			public const float TIER4 = 400f;

			public const float TIER5 = 800f;

			public const float TIER6 = 1200f;

			public const float TIER7 = 2000f;
		}

		public class UPGRADES
		{
			public class MATERIALTAGS
			{
				public const string METAL = "Metal";

				public const string REFINEDMETAL = "RefinedMetal";

				public const string CARBON = "Carbon";
			}

			public class MATERIALMASS
			{
				public const int TIER0 = 100;

				public const int TIER1 = 200;

				public const int TIER2 = 400;

				public const int TIER3 = 500;
			}

			public class MODIFIERAMOUNTS
			{
				public const float MANUALGENERATOR_ENERGYGENERATION = 1.2f;

				public const float MANUALGENERATOR_CAPACITY = 2f;

				public const float PROPANEGENERATOR_ENERGYGENERATION = 1.6f;

				public const float PROPANEGENERATOR_HEATGENERATION = 1.6f;

				public const float GENERATOR_HEATGENERATION = 0.8f;

				public const float GENERATOR_ENERGYGENERATION = 1.3f;

				public const float TURBINE_ENERGYGENERATION = 1.2f;

				public const float TURBINE_CAPACITY = 1.2f;

				public const float SUITRECHARGER_EXECUTIONTIME = 1.2f;

				public const float SUITRECHARGER_HEATGENERATION = 1.2f;

				public const float STORAGELOCKER_CAPACITY = 2f;

				public const float SOLARPANEL_ENERGYGENERATION = 1.2f;

				public const float SMELTER_HEATGENERATION = 0.7f;
			}

			public const float BUILDTIME_TIER0 = 120f;
		}

		public const float DEFAULT_STORAGE_CAPACITY = 2000f;

		public const float STANDARD_MANUAL_REFILL_LEVEL = 0.2f;

		public const float MASS_TEMPERATURE_SCALE = 0.2f;

		public const float AIRCONDITIONER_TEMPDELTA = -14f;

		public const float MAX_ENVIRONMENT_DELTA = -50f;

		public const float COMPOST_FLIP_TIME = 20f;

		public const int TUBE_LAUNCHER_MAX_CHARGES = 3;

		public const float TUBE_LAUNCHER_RECHARGE_TIME = 10f;

		public const float TUBE_LAUNCHER_WORK_TIME = 1f;

		public const float SMELTER_INGOT_INPUTKG = 500f;

		public const float SMELTER_INGOT_OUTPUTKG = 100f;

		public const float SMELTER_FABRICATIONTIME = 120f;

		public const float GEOREFINERY_SLAB_INPUTKG = 1000f;

		public const float GEOREFINERY_SLAB_OUTPUTKG = 200f;

		public const float GEOREFINERY_FABRICATIONTIME = 120f;

		public const float PHARMACY_FABRICATIONTIME = 40f;

		public const float PHARMACY_GENERIC_INPUTKG = 100f;

		public const float PHARMACY_GENERIC_SINGLE = 1f;

		public const float MASS_BURN_RATE_HYDROGENGENERATOR = 0.1f;

		public const float COOKER_FOOD_TEMPERATURE = 368.15f;

		public const float OVERHEAT_DAMAGE_INTERVAL = 7.5f;

		public const float MIN_BUILD_TEMPERATURE = 288.15f;

		public const float MAX_BUILD_TEMPERATURE = 318.15f;

		public const float MELTDOWN_TEMPERATURE = 533.15f;

		public const float REPAIR_FORCE_TEMPERATURE = 293.15f;

		public const int REPAIR_EFFECTIVENESS_BASE = 10;

		public static List<PlanScreen.PlanInfo> PLANORDER = new List<PlanScreen.PlanInfo>
		{
			new PlanScreen.PlanInfo(new HashedString("Base"), false, new List<string>
			{
				"Ladder",
				"FirePole",
				"LadderFast",
				"Tile",
				"GasPermeableMembrane",
				"MeshTile",
				"InsulationTile",
				"PlasticTile",
				"MetalTile",
				"GlassTile",
				"BunkerTile",
				"CarpetTile",
				"Door",
				"ManualPressureDoor",
				"PressureDoor",
				"BunkerDoor",
				"StorageLocker",
				"StorageLockerSmart",
				"LiquidReservoir",
				"GasReservoir",
				"TravelTube",
				"TravelTubeEntrance",
				"TravelTubeWallBridge"
			}),
			new PlanScreen.PlanInfo(new HashedString("Oxygen"), false, new List<string>
			{
				"MineralDeoxidizer",
				"AlgaeHabitat",
				"AirFilter",
				"CO2Scrubber",
				"Electrolyzer"
			}),
			new PlanScreen.PlanInfo(new HashedString("Power"), false, new List<string>
			{
				"ManualGenerator",
				"Generator",
				"HydrogenGenerator",
				"MethaneGenerator",
				"PetroleumGenerator",
				"SteamTurbine",
				"SolarPanel",
				"Wire",
				"WireBridge",
				"HighWattageWire",
				"WireBridgeHighWattage",
				"WireRefined",
				"WireRefinedBridge",
				"WireRefinedHighWattage",
				"WireRefinedBridgeHighWattage",
				"Battery",
				"BatteryMedium",
				"BatterySmart",
				"PowerTransformerSmall",
				"PowerTransformer",
				SwitchConfig.ID,
				LogicPowerRelayConfig.ID,
				TemperatureControlledSwitchConfig.ID,
				PressureSwitchLiquidConfig.ID,
				PressureSwitchGasConfig.ID
			}),
			new PlanScreen.PlanInfo(new HashedString("Food"), false, new List<string>
			{
				"MicrobeMusher",
				"CookingStation",
				"PlanterBox",
				"FarmTile",
				"HydroponicFarm",
				"RationBox",
				"Refrigerator",
				"CreatureDeliveryPoint",
				"FishDeliveryPoint",
				"CreatureFeeder",
				"FishFeeder",
				"EggIncubator",
				"EggCracker",
				"CreatureTrap",
				"FishTrap",
				"AirborneCreatureLure"
			}),
			new PlanScreen.PlanInfo(new HashedString("Plumbing"), false, new List<string>
			{
				"Outhouse",
				"FlushToilet",
				ShowerConfig.ID,
				"LiquidPumpingStation",
				"BottleEmptier",
				"LiquidConduit",
				"InsulatedLiquidConduit",
				"LiquidConduitRadiant",
				"LiquidConduitBridge",
				"LiquidConduitPreferentialFlow",
				"LiquidConduitOverflow",
				"LiquidPump",
				"LiquidMiniPump",
				"LiquidVent",
				"LiquidFilter",
				"LiquidValve",
				"LiquidLogicValve",
				LiquidConduitElementSensorConfig.ID,
				LiquidConduitDiseaseSensorConfig.ID,
				LiquidConduitTemperatureSensorConfig.ID
			}),
			new PlanScreen.PlanInfo(new HashedString("HVAC"), false, new List<string>
			{
				"GasConduit",
				"InsulatedGasConduit",
				"GasConduitRadiant",
				"GasConduitBridge",
				"GasConduitPreferentialFlow",
				"GasConduitOverflow",
				"GasPump",
				"GasMiniPump",
				"GasVent",
				"GasVentHighPressure",
				"GasFilter",
				"GasValve",
				"GasLogicValve",
				"GasBottler",
				"BottleEmptierGas",
				GasConduitElementSensorConfig.ID,
				GasConduitDiseaseSensorConfig.ID,
				GasConduitTemperatureSensorConfig.ID
			}),
			new PlanScreen.PlanInfo(new HashedString("Refining"), false, new List<string>
			{
				"Compost",
				"WaterPurifier",
				"FertilizerMaker",
				"AlgaeDistillery",
				"RockCrusher",
				"Kiln",
				"MetalRefinery",
				"GlassForge",
				"OilRefinery",
				"Polymerizer",
				"OxyliteRefinery",
				"SupermaterialRefinery"
			}),
			new PlanScreen.PlanInfo(new HashedString("Medical"), false, new List<string>
			{
				"WashBasin",
				"WashSink",
				"HandSanitizer",
				"Apothecary",
				"MedicalCot",
				"MedicalBed",
				"MassageTable",
				"Grave"
			}),
			new PlanScreen.PlanInfo(new HashedString("Furniture"), false, new List<string>
			{
				BedConfig.ID,
				LuxuryBedConfig.ID,
				"FloorLamp",
				"CeilingLight",
				"DiningTable",
				"WaterCooler",
				"Phonobox",
				"ArcadeMachine",
				"EspressoMachine",
				"FlowerVase",
				"FlowerVaseWall",
				"FlowerVaseHanging",
				"FlowerVaseHangingFancy",
				"SmallSculpture",
				"Sculpture",
				"IceSculpture",
				"MarbleSculpture",
				"MetalSculpture",
				"CrownMoulding",
				"Canvas",
				"CanvasWide",
				"CanvasTall",
				"ItemPedestal"
			}),
			new PlanScreen.PlanInfo(new HashedString("Equipment"), false, new List<string>
			{
				"ResearchCenter",
				"AdvancedResearchCenter",
				"CosmicResearchCenter",
				"Telescope",
				"PowerControlStation",
				"FarmStation",
				"RanchStation",
				"ShearingStation",
				"RoleStation",
				"ClothingFabricator",
				"SuitFabricator",
				"SuitMarker",
				"SuitLocker",
				"JetSuitMarker",
				"JetSuitLocker",
				"AstronautTrainingCenter"
			}),
			new PlanScreen.PlanInfo(new HashedString("Utilities"), true, new List<string>
			{
				"SpaceHeater",
				"LiquidHeater",
				"LiquidCooledFan",
				"AirConditioner",
				"LiquidConditioner",
				"OreScrubber",
				"OilWellCap",
				"ThermalBlock",
				"ExteriorWall"
			}),
			new PlanScreen.PlanInfo(new HashedString("Automation"), true, new List<string>
			{
				"LogicWire",
				"LogicWireBridge",
				"LogicGateAND",
				"LogicGateOR",
				"LogicGateXOR",
				"LogicGateNOT",
				"LogicGateBUFFER",
				"LogicGateFILTER",
				LogicMemoryConfig.ID,
				LogicSwitchConfig.ID,
				LogicPressureSensorGasConfig.ID,
				LogicPressureSensorLiquidConfig.ID,
				LogicTemperatureSensorConfig.ID,
				LogicTimeOfDaySensorConfig.ID,
				LogicDiseaseSensorConfig.ID,
				LogicElementSensorGasConfig.ID,
				LogicCritterCountSensorConfig.ID,
				"FloorSwitch",
				"Checkpoint",
				CometDetectorConfig.ID
			}),
			new PlanScreen.PlanInfo(new HashedString("Conveyance"), true, new List<string>
			{
				"SolidTransferArm",
				"SolidConduit",
				"SolidConduitInbox",
				"SolidConduitOutbox",
				"SolidConduitBridge",
				"AutoMiner"
			}),
			new PlanScreen.PlanInfo(new HashedString("Rocketry"), true, new List<string>
			{
				"Gantry",
				"SteamEngine",
				"KeroseneEngine",
				"SolidBooster",
				"LiquidFuelTank",
				"OxidizerTank",
				"OxidizerTankLiquid",
				"CargoBay",
				"GasCargoBay",
				"LiquidCargoBay",
				"CommandModule",
				"TouristModule",
				"ResearchModule",
				"SpecialCargoBay",
				"HydrogenEngine"
			})
		};

		public static List<Type> COMPONENT_DESCRIPTION_ORDER = new List<Type>
		{
			typeof(BottleEmptier),
			typeof(Fabricator),
			typeof(MicrobeMusher),
			typeof(CookingStation),
			typeof(RoleStation),
			typeof(ResearchCenter),
			typeof(LiquidCooledFan),
			typeof(HandSanitizer),
			typeof(HandSanitizer.Work),
			typeof(PlantAirConditioner),
			typeof(Clinic),
			typeof(BuildingElementEmitter),
			typeof(ElementConverter),
			typeof(ElementConsumer),
			typeof(PassiveElementConsumer),
			typeof(TinkerStation),
			typeof(EnergyConsumer),
			typeof(AirConditioner),
			typeof(Storage),
			typeof(Battery),
			typeof(AirFilter),
			typeof(FlushToilet),
			typeof(Toilet),
			typeof(EnergyGenerator),
			typeof(MassageTable),
			typeof(Shower),
			typeof(Ownable),
			typeof(PlantablePlot),
			typeof(RelaxationPoint),
			typeof(BuildingComplete),
			typeof(Building),
			typeof(BuildingPreview),
			typeof(BuildingUnderConstruction),
			typeof(Crop),
			typeof(Growing),
			typeof(Equippable),
			typeof(ColdBreather),
			typeof(ResearchPointObject),
			typeof(SuitTank),
			typeof(IlluminationVulnerable),
			typeof(TemperatureVulnerable),
			typeof(PressureVulnerable),
			typeof(SubmersionMonitor),
			typeof(BatterySmart),
			typeof(Compost),
			typeof(Refrigerator),
			typeof(Bed),
			typeof(OreScrubber),
			typeof(OreScrubber.Work),
			typeof(Refinery),
			typeof(LiquidCooledRefinery),
			typeof(MinimumOperatingTemperature),
			typeof(RoomTracker),
			typeof(EnergyConsumerSelfSustaining),
			typeof(ArcadeMachine),
			typeof(GlassForge),
			typeof(Telescope),
			typeof(EspressoMachine),
			typeof(EspressoMachineWorkable),
			typeof(JetSuitTank),
			typeof(Phonobox),
			typeof(ArcadeMachine),
			typeof(BottleEmptier),
			typeof(CommandModule),
			typeof(FuelTank),
			typeof(LaunchableRocket),
			typeof(OxidizerTank),
			typeof(RocketEngine),
			typeof(SolidBooster),
			typeof(TouristModule),
			typeof(WaterCooler),
			typeof(Edible),
			typeof(PlantableSeed),
			typeof(DiseaseTrigger),
			typeof(MedicinalPill),
			typeof(SeedProducer),
			typeof(Geyser),
			typeof(Overheatable),
			typeof(CreatureCalorieMonitor.Def),
			typeof(LureableMonitor.Def),
			typeof(CropSleepingMonitor.Def),
			typeof(FertilizationMonitor.Def),
			typeof(IrrigationMonitor.Def),
			typeof(ScaleGrowthMonitor.Def),
			typeof(TravelTubeEntrance.Work),
			typeof(ToiletWorkableUse),
			typeof(ReceptacleMonitor),
			typeof(Light2D),
			typeof(Ladder),
			typeof(SimCellOccupier),
			typeof(Vent),
			typeof(LogicPorts),
			typeof(Capturable),
			typeof(Trappable),
			typeof(DecorProvider)
		};
	}
}
