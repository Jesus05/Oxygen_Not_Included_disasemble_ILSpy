using System;
using System.Collections.Generic;
using UnityEngine;

namespace Database
{
	public class Techs : ResourceSet<Tech>
	{
		public int tierCount;

		public static Dictionary<string, string[]> TECH_GROUPING = new Dictionary<string, string[]>
		{
			{
				"FarmingTech",
				new string[4]
				{
					"AlgaeHabitat",
					"PlanterBox",
					"RationBox",
					"Compost"
				}
			},
			{
				"FineDining",
				new string[4]
				{
					"DiningTable",
					"FarmTile",
					"CookingStation",
					"EggCracker"
				}
			},
			{
				"Agriculture",
				new string[4]
				{
					"FertilizerMaker",
					"HydroponicFarm",
					"Refrigerator",
					"FarmStation"
				}
			},
			{
				"Ranching",
				new string[6]
				{
					"CreatureDeliveryPoint",
					"FishDeliveryPoint",
					"CreatureFeeder",
					"FishFeeder",
					"RanchStation",
					"ShearingStation"
				}
			},
			{
				"AnimalControl",
				new string[5]
				{
					"CreatureTrap",
					"FishTrap",
					"AirborneCreatureLure",
					"EggIncubator",
					LogicCritterCountSensorConfig.ID
				}
			},
			{
				"ImprovedOxygen",
				new string[1]
				{
					"Electrolyzer"
				}
			},
			{
				"GasPiping",
				new string[4]
				{
					"GasConduit",
					"GasPump",
					"GasVent",
					"GasConduitBridge"
				}
			},
			{
				"ImprovedGasPiping",
				new string[6]
				{
					"InsulatedGasConduit",
					LogicPressureSensorGasConfig.ID,
					"GasVentHighPressure",
					"GasLogicValve",
					"GasConduitPreferentialFlow",
					"GasConduitOverflow"
				}
			},
			{
				"PressureManagement",
				new string[4]
				{
					"LiquidValve",
					"GasValve",
					"ManualPressureDoor",
					"GasPermeableMembrane"
				}
			},
			{
				"DirectedAirStreams",
				new string[4]
				{
					"PressureDoor",
					"OreScrubber",
					"AirFilter",
					"CO2Scrubber"
				}
			},
			{
				"MedicineI",
				new string[1]
				{
					"Apothecary"
				}
			},
			{
				"MedicineII",
				new string[2]
				{
					"DoctorStation",
					"HandSanitizer"
				}
			},
			{
				"MedicineIII",
				new string[3]
				{
					LogicDiseaseSensorConfig.ID,
					GasConduitDiseaseSensorConfig.ID,
					LiquidConduitDiseaseSensorConfig.ID
				}
			},
			{
				"MedicineIV",
				new string[1]
				{
					"AdvancedDoctorStation"
				}
			},
			{
				"LiquidPiping",
				new string[4]
				{
					"LiquidConduit",
					"LiquidPump",
					"LiquidVent",
					"LiquidConduitBridge"
				}
			},
			{
				"ImprovedLiquidPiping",
				new string[6]
				{
					"InsulatedLiquidConduit",
					LogicPressureSensorLiquidConfig.ID,
					"LiquidLogicValve",
					"LiquidConduitPreferentialFlow",
					"LiquidConduitOverflow",
					"LiquidReservoir"
				}
			},
			{
				"PrecisionPlumbing",
				new string[1]
				{
					"EspressoMachine"
				}
			},
			{
				"SanitationSciences",
				new string[4]
				{
					"WashSink",
					"FlushToilet",
					ShowerConfig.ID,
					"MeshTile"
				}
			},
			{
				"AdvancedFiltration",
				new string[2]
				{
					"GasFilter",
					"LiquidFilter"
				}
			},
			{
				"Distillation",
				new string[4]
				{
					"WaterPurifier",
					"AlgaeDistillery",
					"GasBottler",
					"BottleEmptierGas"
				}
			},
			{
				"Catalytics",
				new string[2]
				{
					"OxyliteRefinery",
					"SupermaterialRefinery"
				}
			},
			{
				"PowerRegulation",
				new string[3]
				{
					SwitchConfig.ID,
					"BatteryMedium",
					"WireBridge"
				}
			},
			{
				"AdvancedPowerRegulation",
				new string[5]
				{
					"HydrogenGenerator",
					"HighWattageWire",
					"WireBridgeHighWattage",
					"PowerTransformerSmall",
					"PowerControlStation"
				}
			},
			{
				"PrettyGoodConductors",
				new string[5]
				{
					"WireRefined",
					"WireRefinedBridge",
					"WireRefinedHighWattage",
					"WireRefinedBridgeHighWattage",
					"PowerTransformer"
				}
			},
			{
				"RenewableEnergy",
				new string[3]
				{
					"SteamTurbine",
					"SteamTurbine2",
					"SolarPanel"
				}
			},
			{
				"Combustion",
				new string[1]
				{
					"Generator"
				}
			},
			{
				"ImprovedCombustion",
				new string[3]
				{
					"MethaneGenerator",
					"OilRefinery",
					"PetroleumGenerator"
				}
			},
			{
				"InteriorDecor",
				new string[3]
				{
					"FlowerVase",
					"FloorLamp",
					"CeilingLight"
				}
			},
			{
				"Artistry",
				new string[7]
				{
					"CrownMoulding",
					"CornerMoulding",
					"SmallSculpture",
					"IceSculpture",
					"ItemPedestal",
					"FlowerVaseWall",
					"FlowerVaseHanging"
				}
			},
			{
				"Clothing",
				new string[3]
				{
					"Canvas",
					"ClothingFabricator",
					"CarpetTile"
				}
			},
			{
				"Acoustics",
				new string[1]
				{
					"Phonobox"
				}
			},
			{
				"FineArt",
				new string[3]
				{
					"CanvasWide",
					"CanvasTall",
					"Sculpture"
				}
			},
			{
				"Luxury",
				new string[4]
				{
					LuxuryBedConfig.ID,
					"LadderFast",
					"PlasticTile",
					"ExteriorWall"
				}
			},
			{
				"RefractiveDecor",
				new string[4]
				{
					"GlassTile",
					"FlowerVaseHangingFancy",
					"MarbleSculpture",
					"MetalSculpture"
				}
			},
			{
				"Plastics",
				new string[2]
				{
					"Polymerizer",
					"OilWellCap"
				}
			},
			{
				"ValveMiniaturization",
				new string[2]
				{
					"LiquidMiniPump",
					"GasMiniPump"
				}
			},
			{
				"Suits",
				new string[4]
				{
					"SuitMarker",
					"SuitLocker",
					"SuitFabricator",
					"SuitsOverlay"
				}
			},
			{
				"Jobs",
				new string[2]
				{
					"RoleStation",
					"WaterCooler"
				}
			},
			{
				"AdvancedResearch",
				new string[3]
				{
					"AdvancedResearchCenter",
					"BetaResearchPoint",
					"ResetSkillsStation"
				}
			},
			{
				"BasicRefinement",
				new string[2]
				{
					"RockCrusher",
					"Kiln"
				}
			},
			{
				"RefinedObjects",
				new string[2]
				{
					"ThermalBlock",
					"FirePole"
				}
			},
			{
				"Smelting",
				new string[2]
				{
					"MetalRefinery",
					"MetalTile"
				}
			},
			{
				"HighTempForging",
				new string[3]
				{
					"GlassForge",
					"BunkerTile",
					"BunkerDoor"
				}
			},
			{
				"TemperatureModulation",
				new string[5]
				{
					"LiquidCooledFan",
					"IceCooledFan",
					"IceMachine",
					"SpaceHeater",
					"InsulationTile"
				}
			},
			{
				"HVAC",
				new string[6]
				{
					"AirConditioner",
					LogicTemperatureSensorConfig.ID,
					"GasConduitRadiant",
					GasConduitTemperatureSensorConfig.ID,
					GasConduitElementSensorConfig.ID,
					"GasReservoir"
				}
			},
			{
				"LiquidTemperature",
				new string[5]
				{
					"LiquidHeater",
					"LiquidConditioner",
					"LiquidConduitRadiant",
					LiquidConduitTemperatureSensorConfig.ID,
					LiquidConduitElementSensorConfig.ID
				}
			},
			{
				"LogicControl",
				new string[5]
				{
					"AutomationOverlay",
					"LogicWire",
					"LogicWireBridge",
					LogicSwitchConfig.ID,
					LogicPowerRelayConfig.ID
				}
			},
			{
				"GenericSensors",
				new string[4]
				{
					LogicTimeOfDaySensorConfig.ID,
					"FloorSwitch",
					LogicElementSensorGasConfig.ID,
					"BatterySmart"
				}
			},
			{
				"LogicCircuits",
				new string[7]
				{
					"LogicGateAND",
					"LogicGateOR",
					"LogicGateXOR",
					"LogicGateNOT",
					"LogicGateBUFFER",
					"LogicGateFILTER",
					"BatterySmart"
				}
			},
			{
				"DupeTrafficControl",
				new string[4]
				{
					"Checkpoint",
					LogicMemoryConfig.ID,
					"ArcadeMachine",
					"CosmicResearchCenter"
				}
			},
			{
				"SkyDetectors",
				new string[3]
				{
					CometDetectorConfig.ID,
					"Telescope",
					"AstronautTrainingCenter"
				}
			},
			{
				"TravelTubes",
				new string[3]
				{
					"TravelTubeEntrance",
					"TravelTube",
					"TravelTubeWallBridge"
				}
			},
			{
				"SmartStorage",
				new string[4]
				{
					"StorageLockerSmart",
					"SolidTransferArm",
					"ObjectDispenser",
					"ConveyorOverlay"
				}
			},
			{
				"SolidTransport",
				new string[7]
				{
					"SolidConduit",
					"SolidConduitBridge",
					"SolidConduitInbox",
					"SolidConduitOutbox",
					"SolidVent",
					"SolidLogicValve",
					"AutoMiner"
				}
			},
			{
				"BasicRocketry",
				new string[4]
				{
					"CommandModule",
					"SteamEngine",
					"ResearchModule",
					"Gantry"
				}
			},
			{
				"CargoI",
				new string[1]
				{
					"CargoBay"
				}
			},
			{
				"CargoII",
				new string[2]
				{
					"LiquidCargoBay",
					"GasCargoBay"
				}
			},
			{
				"CargoIII",
				new string[2]
				{
					"TouristModule",
					"SpecialCargoBay"
				}
			},
			{
				"EnginesI",
				new string[1]
				{
					"SolidBooster"
				}
			},
			{
				"EnginesII",
				new string[3]
				{
					"KeroseneEngine",
					"LiquidFuelTank",
					"OxidizerTank"
				}
			},
			{
				"EnginesIII",
				new string[2]
				{
					"OxidizerTankLiquid",
					"HydrogenEngine"
				}
			},
			{
				"Jetpacks",
				new string[3]
				{
					"JetSuit",
					"JetSuitMarker",
					"JetSuitLocker"
				}
			}
		};

		private readonly List<List<Tuple<string, float>>> TECH_TIERS = new List<List<Tuple<string, float>>>
		{
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 15f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 20f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 30f),
				new Tuple<string, float>("beta", 20f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 35f),
				new Tuple<string, float>("beta", 30f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 40f),
				new Tuple<string, float>("beta", 50f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 50f),
				new Tuple<string, float>("beta", 70f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 70f),
				new Tuple<string, float>("beta", 100f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 70f),
				new Tuple<string, float>("beta", 100f),
				new Tuple<string, float>("gamma", 200f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 70f),
				new Tuple<string, float>("beta", 100f),
				new Tuple<string, float>("gamma", 400f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 70f),
				new Tuple<string, float>("beta", 100f),
				new Tuple<string, float>("gamma", 800f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 70f),
				new Tuple<string, float>("beta", 100f),
				new Tuple<string, float>("gamma", 1600f)
			}
		};

		public Techs(ResourceSet parent)
			: base("Techs", parent)
		{
		}

		public void Load(TextAsset tree_file)
		{
			ResourceTreeLoader<ResourceTreeNode> resourceTreeLoader = new ResourceTreeLoader<ResourceTreeNode>(tree_file);
			foreach (ResourceTreeNode item in resourceTreeLoader)
			{
				Tech tech = TryGet(item.Id);
				if (tech == null)
				{
					tech = new Tech(item.Id, this, Strings.Get("STRINGS.RESEARCH.TECHS." + item.Id.ToUpper() + ".NAME"), Strings.Get("STRINGS.RESEARCH.TECHS." + item.Id.ToUpper() + ".DESC"), item);
				}
				foreach (ResourceTreeNode reference in item.references)
				{
					Tech tech2 = TryGet(reference.Id);
					if (tech2 == null)
					{
						tech2 = new Tech(reference.Id, this, Strings.Get("STRINGS.RESEARCH.TECHS." + reference.Id.ToUpper() + ".NAME"), Strings.Get("STRINGS.RESEARCH.TECHS." + reference.Id.ToUpper() + ".DESC"), reference);
					}
					tech2.requiredTech.Add(tech);
					tech.unlockedTech.Add(tech2);
				}
			}
			tierCount = 0;
			foreach (Tech resource in resources)
			{
				resource.tier = GetTier(resource);
				List<Tuple<string, float>> list = TECH_TIERS[resource.tier];
				foreach (Tuple<string, float> item2 in list)
				{
					resource.costsByResearchTypeID.Add(item2.first, item2.second);
				}
				tierCount = Math.Max(resource.tier + 1, tierCount);
			}
		}

		private int GetTier(Tech tech)
		{
			if (tech.requiredTech.Count == 0)
			{
				return 0;
			}
			int num = 0;
			foreach (Tech item in tech.requiredTech)
			{
				num = Math.Max(num, GetTier(item));
			}
			return num + 1;
		}

		private void AddPrerequisite(Tech tech, string prerequisite_name)
		{
			Tech tech2 = TryGet(prerequisite_name);
			if (tech2 != null)
			{
				tech.requiredTech.Add(tech2);
				tech2.unlockedTech.Add(tech);
			}
		}

		public bool IsTechItemComplete(string id)
		{
			foreach (Tech resource in resources)
			{
				foreach (TechItem unlockedItem in resource.unlockedItems)
				{
					if (unlockedItem.Id == id)
					{
						return resource.IsComplete();
					}
				}
			}
			return true;
		}
	}
}
