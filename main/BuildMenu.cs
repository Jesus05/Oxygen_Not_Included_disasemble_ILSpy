using FMOD.Studio;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenu : KScreen
{
	[Serializable]
	private struct PadInfo
	{
		public int left;

		public int right;

		public int top;

		public int bottom;
	}

	public struct BuildingInfo
	{
		public string id;

		public Action hotkey;

		public BuildingInfo(string id, Action hotkey)
		{
			this.id = id;
			this.hotkey = hotkey;
		}
	}

	public struct DisplayInfo
	{
		public HashedString category;

		public string iconName;

		public Action hotkey;

		public KKeyCode keyCode;

		public object data;

		public DisplayInfo(HashedString category, string icon_name, Action hotkey, KKeyCode key_code, object data)
		{
			this.category = category;
			iconName = icon_name;
			this.hotkey = hotkey;
			keyCode = key_code;
			this.data = data;
		}

		public DisplayInfo GetInfo(HashedString category)
		{
			DisplayInfo result = default(DisplayInfo);
			if (data != null && typeof(IList<DisplayInfo>).IsAssignableFrom(data.GetType()))
			{
				IList<DisplayInfo> list = (IList<DisplayInfo>)data;
				{
					foreach (DisplayInfo item in list)
					{
						DisplayInfo current = item;
						result = current.GetInfo(category);
						if (result.category == category)
						{
							return result;
						}
						if (current.category == category)
						{
							return current;
						}
					}
					return result;
				}
			}
			return result;
		}
	}

	public const string ENABLE_HOTKEY_BUILD_MENU_KEY = "ENABLE_HOTKEY_BUILD_MENU";

	[SerializeField]
	private BuildMenuCategoriesScreen categoriesMenuPrefab;

	[SerializeField]
	private BuildMenuBuildingsScreen buildingsMenuPrefab;

	[SerializeField]
	private GameObject productInfoScreenPrefab;

	private ProductInfoScreen productInfoScreen;

	private BuildMenuBuildingsScreen buildingsScreen;

	private BuildingDef selectedBuilding;

	private HashedString selectedCategory;

	private static readonly HashedString ROOT_HASHSTR = new HashedString("ROOT");

	private Dictionary<HashedString, BuildMenuCategoriesScreen> submenus = new Dictionary<HashedString, BuildMenuCategoriesScreen>();

	private Stack<KIconToggleMenu> submenuStack = new Stack<KIconToggleMenu>();

	private bool selecting;

	private bool updating;

	private bool deactivateToolQueued;

	[SerializeField]
	private Vector2 rootMenuOffset = Vector2.zero;

	[SerializeField]
	private PadInfo rootMenuPadding = default(PadInfo);

	[SerializeField]
	private Vector2 nestedMenuOffset = Vector2.zero;

	[SerializeField]
	private PadInfo nestedMenuPadding = default(PadInfo);

	[SerializeField]
	private Vector2 buildingsMenuOffset = Vector2.zero;

	public static DisplayInfo OrderedBuildings = new DisplayInfo(CacheHashString("ROOT"), "icon_category_base", Action.NumActions, KKeyCode.None, new List<DisplayInfo>
	{
		new DisplayInfo(CacheHashString("Base"), "icon_category_base", Action.Plan1, KKeyCode.None, new List<DisplayInfo>
		{
			new DisplayInfo(CacheHashString("Tiles"), "icon_category_base", Action.BuildCategoryTiles, KKeyCode.T, new List<BuildingInfo>
			{
				new BuildingInfo("Tile", Action.BuildMenuKeyT),
				new BuildingInfo("GasPermeableMembrane", Action.BuildMenuKeyA),
				new BuildingInfo("MeshTile", Action.BuildMenuKeyE),
				new BuildingInfo("InsulationTile", Action.BuildMenuKeyD),
				new BuildingInfo("PlasticTile", Action.BuildMenuKeyC),
				new BuildingInfo("MetalTile", Action.BuildMenuKeyX),
				new BuildingInfo("GlassTile", Action.BuildMenuKeyW),
				new BuildingInfo("BunkerTile", Action.BuildMenuKeyB),
				new BuildingInfo("CarpetTile", Action.BuildMenuKeyL)
			}),
			new DisplayInfo(CacheHashString("Ladders"), "icon_category_base", Action.BuildCategoryLadders, KKeyCode.A, new List<BuildingInfo>
			{
				new BuildingInfo("Ladder", Action.BuildMenuKeyA),
				new BuildingInfo("LadderFast", Action.BuildMenuKeyC),
				new BuildingInfo("FirePole", Action.BuildMenuKeyF)
			}),
			new DisplayInfo(CacheHashString("Doors"), "icon_category_base", Action.BuildCategoryDoors, KKeyCode.D, new List<BuildingInfo>
			{
				new BuildingInfo("Door", Action.BuildMenuKeyD),
				new BuildingInfo("ManualPressureDoor", Action.BuildMenuKeyA),
				new BuildingInfo("PressureDoor", Action.BuildMenuKeyE),
				new BuildingInfo("BunkerDoor", Action.BuildMenuKeyB)
			}),
			new DisplayInfo(CacheHashString("Storage"), "icon_category_base", Action.BuildCategoryStorage, KKeyCode.S, new List<BuildingInfo>
			{
				new BuildingInfo("StorageLocker", Action.BuildMenuKeyS),
				new BuildingInfo("RationBox", Action.BuildMenuKeyR),
				new BuildingInfo("Refrigerator", Action.BuildMenuKeyF),
				new BuildingInfo("StorageLockerSmart", Action.BuildMenuKeyA),
				new BuildingInfo("LiquidReservoir", Action.BuildMenuKeyQ),
				new BuildingInfo("GasReservoir", Action.BuildMenuKeyG),
				new BuildingInfo("ObjectDispenser", Action.BuildMenuKeyO)
			}),
			new DisplayInfo(CacheHashString("Research"), "icon_category_misc", Action.BuildCategoryResearch, KKeyCode.R, new List<BuildingInfo>
			{
				new BuildingInfo("ResearchCenter", Action.BuildMenuKeyR),
				new BuildingInfo("AdvancedResearchCenter", Action.BuildMenuKeyS),
				new BuildingInfo("CosmicResearchCenter", Action.BuildMenuKeyC),
				new BuildingInfo("Telescope", Action.BuildMenuKeyT)
			})
		}),
		new DisplayInfo(CacheHashString("Food And Agriculture"), "icon_category_food", Action.Plan2, KKeyCode.None, new List<DisplayInfo>
		{
			new DisplayInfo(CacheHashString("Farming"), "icon_category_food", Action.BuildCategoryFarming, KKeyCode.F, new List<BuildingInfo>
			{
				new BuildingInfo("PlanterBox", Action.BuildMenuKeyB),
				new BuildingInfo("FarmTile", Action.BuildMenuKeyF),
				new BuildingInfo("HydroponicFarm", Action.BuildMenuKeyD),
				new BuildingInfo("Compost", Action.BuildMenuKeyC),
				new BuildingInfo("FertilizerMaker", Action.BuildMenuKeyR)
			}),
			new DisplayInfo(CacheHashString("Cooking"), "icon_category_food", Action.BuildCategoryCooking, KKeyCode.C, new List<BuildingInfo>
			{
				new BuildingInfo("MicrobeMusher", Action.BuildMenuKeyC),
				new BuildingInfo("CookingStation", Action.BuildMenuKeyG),
				new BuildingInfo("GourmetCookingStation", Action.BuildMenuKeyS),
				new BuildingInfo("EggCracker", Action.BuildMenuKeyE)
			}),
			new DisplayInfo(CacheHashString("Ranching"), "icon_category_food", Action.BuildCategoryRanching, KKeyCode.R, new List<BuildingInfo>
			{
				new BuildingInfo("CreatureDeliveryPoint", Action.BuildMenuKeyD),
				new BuildingInfo("FishDeliveryPoint", Action.BuildMenuKeyG),
				new BuildingInfo("CreatureFeeder", Action.BuildMenuKeyF),
				new BuildingInfo("FishFeeder", Action.BuildMenuKeyE),
				new BuildingInfo("RanchStation", Action.BuildMenuKeyR),
				new BuildingInfo("ShearingStation", Action.BuildMenuKeyS),
				new BuildingInfo("EggIncubator", Action.BuildMenuKeyI),
				new BuildingInfo("CreatureTrap", Action.BuildMenuKeyT),
				new BuildingInfo("FishTrap", Action.BuildMenuKeyA),
				new BuildingInfo("AirborneCreatureLure", Action.BuildMenuKeyL),
				new BuildingInfo("FlyingCreatureBait", Action.BuildMenuKeyB)
			})
		}),
		new DisplayInfo(CacheHashString("Health And Happiness"), "icon_category_medical", Action.Plan3, KKeyCode.None, new List<DisplayInfo>
		{
			new DisplayInfo(CacheHashString("Medical"), "icon_category_medical", Action.BuildCategoryMedical, KKeyCode.C, new List<BuildingInfo>
			{
				new BuildingInfo("Apothecary", Action.BuildMenuKeyA),
				new BuildingInfo("DoctorStation", Action.BuildMenuKeyD),
				new BuildingInfo("AdvancedDoctorStation", Action.BuildMenuKeyO),
				new BuildingInfo("MedicalCot", Action.BuildMenuKeyB),
				new BuildingInfo("MassageTable", Action.BuildMenuKeyT),
				new BuildingInfo("Grave", Action.BuildMenuKeyR)
			}),
			new DisplayInfo(CacheHashString("Hygiene"), "icon_category_medical", Action.BuildCategoryHygiene, KKeyCode.E, new List<BuildingInfo>
			{
				new BuildingInfo("Outhouse", Action.BuildMenuKeyT),
				new BuildingInfo("FlushToilet", Action.BuildMenuKeyV),
				new BuildingInfo(ShowerConfig.ID, Action.BuildMenuKeyS),
				new BuildingInfo("WashBasin", Action.BuildMenuKeyB),
				new BuildingInfo("WashSink", Action.BuildMenuKeyW),
				new BuildingInfo("HandSanitizer", Action.BuildMenuKeyA)
			}),
			new DisplayInfo(CacheHashString("Furniture"), "icon_category_furniture", Action.BuildCategoryFurniture, KKeyCode.F, new List<BuildingInfo>
			{
				new BuildingInfo(BedConfig.ID, Action.BuildMenuKeyC),
				new BuildingInfo(LuxuryBedConfig.ID, Action.BuildMenuKeyX),
				new BuildingInfo("DiningTable", Action.BuildMenuKeyD),
				new BuildingInfo("FloorLamp", Action.BuildMenuKeyF),
				new BuildingInfo("CeilingLight", Action.BuildMenuKeyT)
			}),
			new DisplayInfo(CacheHashString("Decor"), "icon_category_furniture", Action.BuildCategoryDecor, KKeyCode.D, new List<BuildingInfo>
			{
				new BuildingInfo("FlowerVase", Action.BuildMenuKeyF),
				new BuildingInfo("Canvas", Action.BuildMenuKeyC),
				new BuildingInfo("CanvasWide", Action.BuildMenuKeyW),
				new BuildingInfo("CanvasTall", Action.BuildMenuKeyT),
				new BuildingInfo("Sculpture", Action.BuildMenuKeyS),
				new BuildingInfo("IceSculpture", Action.BuildMenuKeyE),
				new BuildingInfo("ItemPedestal", Action.BuildMenuKeyD),
				new BuildingInfo("CrownMoulding", Action.BuildMenuKeyM),
				new BuildingInfo("CornerMoulding", Action.BuildMenuKeyN)
			}),
			new DisplayInfo(CacheHashString("Recreation"), "icon_category_medical", Action.BuildCategoryRecreation, KKeyCode.R, new List<BuildingInfo>
			{
				new BuildingInfo("WaterCooler", Action.BuildMenuKeyC),
				new BuildingInfo("ArcadeMachine", Action.BuildMenuKeyA),
				new BuildingInfo("Phonobox", Action.BuildMenuKeyP),
				new BuildingInfo("EspressoMachine", Action.BuildMenuKeyE),
				new BuildingInfo("ParkSign", Action.BuildMenuKeyR)
			})
		}),
		new DisplayInfo(CacheHashString("Infrastructure"), "icon_category_utilities", Action.Plan4, KKeyCode.None, new List<DisplayInfo>
		{
			new DisplayInfo(CacheHashString("Wires"), "icon_category_electrical", Action.BuildCategoryWires, KKeyCode.W, new List<BuildingInfo>
			{
				new BuildingInfo("Wire", Action.BuildMenuKeyW),
				new BuildingInfo("WireBridge", Action.BuildMenuKeyB),
				new BuildingInfo("HighWattageWire", Action.BuildMenuKeyT),
				new BuildingInfo("WireBridgeHighWattage", Action.BuildMenuKeyG),
				new BuildingInfo("WireRefined", Action.BuildMenuKeyR),
				new BuildingInfo("WireRefinedBridge", Action.BuildMenuKeyQ),
				new BuildingInfo("WireRefinedHighWattage", Action.BuildMenuKeyE),
				new BuildingInfo("WireRefinedBridgeHighWattage", Action.BuildMenuKeyA)
			}),
			new DisplayInfo(CacheHashString("Generators"), "icon_category_electrical", Action.BuildCategoryGenerators, KKeyCode.G, new List<BuildingInfo>
			{
				new BuildingInfo("ManualGenerator", Action.BuildMenuKeyG),
				new BuildingInfo("Generator", Action.BuildMenuKeyC),
				new BuildingInfo("WoodGasGenerator", Action.BuildMenuKeyW),
				new BuildingInfo("HydrogenGenerator", Action.BuildMenuKeyD),
				new BuildingInfo("MethaneGenerator", Action.BuildMenuKeyA),
				new BuildingInfo("PetroleumGenerator", Action.BuildMenuKeyR),
				new BuildingInfo("SteamTurbine", Action.BuildMenuKeyT),
				new BuildingInfo("SteamTurbine2", Action.BuildMenuKeyT),
				new BuildingInfo("SolarPanel", Action.BuildMenuKeyS)
			}),
			new DisplayInfo(CacheHashString("PowerControl"), "icon_category_electrical", Action.BuildCategoryPowerControl, KKeyCode.R, new List<BuildingInfo>
			{
				new BuildingInfo("Battery", Action.BuildMenuKeyB),
				new BuildingInfo("BatteryMedium", Action.BuildMenuKeyE),
				new BuildingInfo("BatterySmart", Action.BuildMenuKeyS),
				new BuildingInfo("PowerTransformerSmall", Action.BuildMenuKeyT),
				new BuildingInfo("PowerTransformer", Action.BuildMenuKeyR),
				new BuildingInfo(SwitchConfig.ID, Action.BuildMenuKeyC),
				new BuildingInfo(TemperatureControlledSwitchConfig.ID, Action.BuildMenuKeyA),
				new BuildingInfo(PressureSwitchLiquidConfig.ID, Action.BuildMenuKeyQ),
				new BuildingInfo(PressureSwitchGasConfig.ID, Action.BuildMenuKeyG),
				new BuildingInfo(LogicPowerRelayConfig.ID, Action.BuildMenuKeyX)
			}),
			new DisplayInfo(CacheHashString("Pipes"), "icon_category_plumbing", Action.BuildCategoryPipes, KKeyCode.E, new List<BuildingInfo>
			{
				new BuildingInfo("LiquidConduit", Action.BuildMenuKeyQ),
				new BuildingInfo("LiquidConduitBridge", Action.BuildMenuKeyB),
				new BuildingInfo("InsulatedLiquidConduit", Action.BuildMenuKeyW),
				new BuildingInfo("LiquidConduitRadiant", Action.BuildMenuKeyE),
				new BuildingInfo("GasConduit", Action.BuildMenuKeyG),
				new BuildingInfo("GasConduitBridge", Action.BuildMenuKeyF),
				new BuildingInfo("InsulatedGasConduit", Action.BuildMenuKeyD),
				new BuildingInfo("GasConduitRadiant", Action.BuildMenuKeyR)
			}),
			new DisplayInfo(CacheHashString("Plumbing Structures"), "icon_category_plumbing", Action.BuildCategoryPlumbingStructures, KKeyCode.B, new List<BuildingInfo>
			{
				new BuildingInfo("LiquidPumpingStation", Action.BuildMenuKeyD),
				new BuildingInfo("BottleEmptier", Action.BuildMenuKeyB),
				new BuildingInfo("LiquidPump", Action.BuildMenuKeyQ),
				new BuildingInfo("LiquidMiniPump", Action.BuildMenuKeyX),
				new BuildingInfo("LiquidValve", Action.BuildMenuKeyA),
				new BuildingInfo("LiquidLogicValve", Action.BuildMenuKeyL),
				new BuildingInfo("LiquidVent", Action.BuildMenuKeyV),
				new BuildingInfo("LiquidFilter", Action.BuildMenuKeyF),
				new BuildingInfo("LiquidConduitPreferentialFlow", Action.BuildMenuKeyW),
				new BuildingInfo("LiquidConduitOverflow", Action.BuildMenuKeyR)
			}),
			new DisplayInfo(CacheHashString("Ventilation Structures"), "icon_category_ventilation", Action.BuildCategoryVentilationStructures, KKeyCode.V, new List<BuildingInfo>
			{
				new BuildingInfo("GasPump", Action.BuildMenuKeyQ),
				new BuildingInfo("GasMiniPump", Action.BuildMenuKeyX),
				new BuildingInfo("GasValve", Action.BuildMenuKeyA),
				new BuildingInfo("GasLogicValve", Action.BuildMenuKeyC),
				new BuildingInfo("GasVent", Action.BuildMenuKeyV),
				new BuildingInfo("GasVentHighPressure", Action.BuildMenuKeyE),
				new BuildingInfo("GasFilter", Action.BuildMenuKeyF),
				new BuildingInfo("GasBottler", Action.BuildMenuKeyB),
				new BuildingInfo("BottleEmptierGas", Action.BuildMenuKeyB),
				new BuildingInfo("GasConduitPreferentialFlow", Action.BuildMenuKeyW),
				new BuildingInfo("GasConduitOverflow", Action.BuildMenuKeyR)
			})
		}),
		new DisplayInfo(CacheHashString("Industrial"), "icon_category_refinery", Action.Plan5, KKeyCode.None, new List<DisplayInfo>
		{
			new DisplayInfo(CacheHashString("Oxygen"), "icon_category_oxygen", Action.BuildCategoryOxygen, KKeyCode.X, new List<BuildingInfo>
			{
				new BuildingInfo("MineralDeoxidizer", Action.BuildMenuKeyX),
				new BuildingInfo("AlgaeHabitat", Action.BuildMenuKeyA),
				new BuildingInfo("AirFilter", Action.BuildMenuKeyD),
				new BuildingInfo("CO2Scrubber", Action.BuildMenuKeyC),
				new BuildingInfo("Electrolyzer", Action.BuildMenuKeyE),
				new BuildingInfo("RustDeoxidizer", Action.BuildMenuKeyF)
			}),
			new DisplayInfo(CacheHashString("Utilities"), "icon_category_utilities", Action.BuildCategoryUtilities, KKeyCode.T, new List<BuildingInfo>
			{
				new BuildingInfo("SpaceHeater", Action.BuildMenuKeyS),
				new BuildingInfo("LiquidHeater", Action.BuildMenuKeyT),
				new BuildingInfo("IceCooledFan", Action.BuildMenuKeyQ),
				new BuildingInfo("IceMachine", Action.BuildMenuKeyI),
				new BuildingInfo("AirConditioner", Action.BuildMenuKeyR),
				new BuildingInfo("LiquidConditioner", Action.BuildMenuKeyA),
				new BuildingInfo("OreScrubber", Action.BuildMenuKeyC),
				new BuildingInfo("ThermalBlock", Action.BuildMenuKeyF),
				new BuildingInfo("ExteriorWall", Action.BuildMenuKeyD)
			}),
			new DisplayInfo(CacheHashString("Refining"), "icon_category_refinery", Action.BuildCategoryRefining, KKeyCode.R, new List<BuildingInfo>
			{
				new BuildingInfo("WaterPurifier", Action.BuildMenuKeyW),
				new BuildingInfo("AlgaeDistillery", Action.BuildMenuKeyA),
				new BuildingInfo("EthanolDistillery", Action.BuildMenuKeyX),
				new BuildingInfo("RockCrusher", Action.BuildMenuKeyG),
				new BuildingInfo("Kiln", Action.BuildMenuKeyZ),
				new BuildingInfo("OilWellCap", Action.BuildMenuKeyC),
				new BuildingInfo("OilRefinery", Action.BuildMenuKeyR),
				new BuildingInfo("Polymerizer", Action.BuildMenuKeyE),
				new BuildingInfo("MetalRefinery", Action.BuildMenuKeyT),
				new BuildingInfo("GlassForge", Action.BuildMenuKeyF),
				new BuildingInfo("OxyliteRefinery", Action.BuildMenuKeyO),
				new BuildingInfo("SupermaterialRefinery", Action.BuildMenuKeyS)
			}),
			new DisplayInfo(CacheHashString("Equipment"), "icon_category_misc", Action.BuildCategoryEquipment, KKeyCode.S, new List<BuildingInfo>
			{
				new BuildingInfo("RoleStation", Action.BuildMenuKeyB),
				new BuildingInfo("FarmStation", Action.BuildMenuKeyF),
				new BuildingInfo("PowerControlStation", Action.BuildMenuKeyC),
				new BuildingInfo("AstronautTrainingCenter", Action.BuildMenuKeyA),
				new BuildingInfo("ResetSkillsStation", Action.BuildMenuKeyR),
				new BuildingInfo("ClothingFabricator", Action.BuildMenuKeyT),
				new BuildingInfo("SuitFabricator", Action.BuildMenuKeyX),
				new BuildingInfo("SuitMarker", Action.BuildMenuKeyE),
				new BuildingInfo("SuitLocker", Action.BuildMenuKeyD),
				new BuildingInfo("JetSuitMarker", Action.BuildMenuKeyJ),
				new BuildingInfo("JetSuitLocker", Action.BuildMenuKeyO)
			}),
			new DisplayInfo(CacheHashString("Rocketry"), "icon_category_rocketry", Action.BuildCategoryRocketry, KKeyCode.C, new List<BuildingInfo>
			{
				new BuildingInfo("Gantry", Action.BuildMenuKeyT),
				new BuildingInfo("KeroseneEngine", Action.BuildMenuKeyE),
				new BuildingInfo("SolidBooster", Action.BuildMenuKeyB),
				new BuildingInfo("SteamEngine", Action.BuildMenuKeyS),
				new BuildingInfo("LiquidFuelTank", Action.BuildMenuKeyQ),
				new BuildingInfo("CargoBay", Action.BuildMenuKeyB),
				new BuildingInfo("GasCargoBay", Action.BuildMenuKeyG),
				new BuildingInfo("LiquidCargoBay", Action.BuildMenuKeyQ),
				new BuildingInfo("SpecialCargoBay", Action.BuildMenuKeyA),
				new BuildingInfo("CommandModule", Action.BuildMenuKeyC),
				new BuildingInfo("TouristModule", Action.BuildMenuKeyY),
				new BuildingInfo("ResearchModule", Action.BuildMenuKeyR),
				new BuildingInfo("HydrogenEngine", Action.BuildMenuKeyH)
			})
		}),
		new DisplayInfo(CacheHashString("Logistics"), "icon_category_ventilation", Action.Plan6, KKeyCode.None, new List<DisplayInfo>
		{
			new DisplayInfo(CacheHashString("TravelTubes"), "icon_category_ventilation", Action.BuildCategoryTravelTubes, KKeyCode.T, new List<BuildingInfo>
			{
				new BuildingInfo("TravelTube", Action.BuildMenuKeyT),
				new BuildingInfo("TravelTubeEntrance", Action.BuildMenuKeyE),
				new BuildingInfo("TravelTubeWallBridge", Action.BuildMenuKeyB)
			}),
			new DisplayInfo(CacheHashString("Conveyance"), "icon_category_ventilation", Action.BuildCategoryConveyance, KKeyCode.C, new List<BuildingInfo>
			{
				new BuildingInfo("SolidTransferArm", Action.BuildMenuKeyA),
				new BuildingInfo("SolidConduit", Action.BuildMenuKeyC),
				new BuildingInfo("SolidConduitInbox", Action.BuildMenuKeyI),
				new BuildingInfo("SolidConduitOutbox", Action.BuildMenuKeyO),
				new BuildingInfo("SolidVent", Action.BuildMenuKeyV),
				new BuildingInfo("SolidLogicValve", Action.BuildMenuKeyL),
				new BuildingInfo("SolidConduitBridge", Action.BuildMenuKeyB),
				new BuildingInfo("AutoMiner", Action.BuildMenuKeyM)
			}),
			new DisplayInfo(CacheHashString("LogicWiring"), "icon_category_automation", Action.BuildCategoryLogicWiring, KKeyCode.W, new List<BuildingInfo>
			{
				new BuildingInfo("LogicWire", Action.BuildMenuKeyW),
				new BuildingInfo("LogicWireBridge", Action.BuildMenuKeyB)
			}),
			new DisplayInfo(CacheHashString("LogicGates"), "icon_category_automation", Action.BuildCategoryLogicGates, KKeyCode.G, new List<BuildingInfo>
			{
				new BuildingInfo("LogicGateAND", Action.BuildMenuKeyA),
				new BuildingInfo("LogicGateOR", Action.BuildMenuKeyR),
				new BuildingInfo("LogicGateXOR", Action.BuildMenuKeyX),
				new BuildingInfo("LogicGateNOT", Action.BuildMenuKeyT),
				new BuildingInfo("LogicGateBUFFER", Action.BuildMenuKeyB),
				new BuildingInfo("LogicGateFILTER", Action.BuildMenuKeyF),
				new BuildingInfo(LogicMemoryConfig.ID, Action.BuildMenuKeyV)
			}),
			new DisplayInfo(CacheHashString("LogicSwitches"), "icon_category_automation", Action.BuildCategoryLogicSwitches, KKeyCode.S, new List<BuildingInfo>
			{
				new BuildingInfo(LogicSwitchConfig.ID, Action.BuildMenuKeyS),
				new BuildingInfo(LogicPressureSensorGasConfig.ID, Action.BuildMenuKeyA),
				new BuildingInfo(LogicPressureSensorLiquidConfig.ID, Action.BuildMenuKeyQ),
				new BuildingInfo(LogicTemperatureSensorConfig.ID, Action.BuildMenuKeyT),
				new BuildingInfo(LogicTimeOfDaySensorConfig.ID, Action.BuildMenuKeyD),
				new BuildingInfo(LogicCritterCountSensorConfig.ID, Action.BuildMenuKeyV),
				new BuildingInfo(LogicDiseaseSensorConfig.ID, Action.BuildMenuKeyG),
				new BuildingInfo(LogicElementSensorGasConfig.ID, Action.BuildMenuKeyE),
				new BuildingInfo("FloorSwitch", Action.BuildMenuKeyW),
				new BuildingInfo("Checkpoint", Action.BuildMenuKeyC),
				new BuildingInfo(CometDetectorConfig.ID, Action.BuildMenuKeyR),
				new BuildingInfo("LogicDuplicantSensor", Action.BuildMenuKeyF)
			}),
			new DisplayInfo(CacheHashString("ConduitSensors"), "icon_category_automation", Action.BuildCategoryLogicConduits, KKeyCode.X, new List<BuildingInfo>
			{
				new BuildingInfo(LiquidConduitTemperatureSensorConfig.ID, Action.BuildMenuKeyT),
				new BuildingInfo(LiquidConduitDiseaseSensorConfig.ID, Action.BuildMenuKeyG),
				new BuildingInfo(LiquidConduitElementSensorConfig.ID, Action.BuildMenuKeyE),
				new BuildingInfo(GasConduitTemperatureSensorConfig.ID, Action.BuildMenuKeyR),
				new BuildingInfo(GasConduitDiseaseSensorConfig.ID, Action.BuildMenuKeyF),
				new BuildingInfo(GasConduitElementSensorConfig.ID, Action.BuildMenuKeyS)
			})
		})
	});

	private Dictionary<HashedString, List<BuildingDef>> categorizedBuildingMap;

	private Dictionary<HashedString, List<HashedString>> categorizedCategoryMap;

	private Dictionary<Tag, HashedString> tagCategoryMap;

	private Dictionary<Tag, int> tagOrderMap;

	private const float NotificationPingExpire = 0.5f;

	private const float SpecialNotificationEmbellishDelay = 8f;

	private float timeSinceNotificationPing;

	private int notificationPingCount;

	private float initTime;

	private float updateInterval = 1f;

	private float elapsedTime;

	public static BuildMenu Instance
	{
		get;
		private set;
	}

	public BuildingDef SelectedBuildingDef => selectedBuilding;

	public override float GetSortKey()
	{
		return 6f;
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	private static HashedString CacheHashString(string str)
	{
		return HashCache.Get().Add(str);
	}

	public static bool UseHotkeyBuildMenu()
	{
		int @int = KPlayerPrefs.GetInt("ENABLE_HOTKEY_BUILD_MENU");
		return @int != 0;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ConsumeMouseScroll = true;
		initTime = KTime.Instance.UnscaledGameTime;
		bool flag = UseHotkeyBuildMenu();
		if (flag)
		{
			Instance = this;
			productInfoScreen = Util.KInstantiateUI<ProductInfoScreen>(productInfoScreenPrefab, base.gameObject, true);
			productInfoScreen.rectTransform().pivot = new Vector2(0f, 0f);
			productInfoScreen.onElementsFullySelected = OnRecipeElementsFullySelected;
			productInfoScreen.Show(false);
			buildingsScreen = Util.KInstantiateUI<BuildMenuBuildingsScreen>(buildingsMenuPrefab.gameObject, base.gameObject, true);
			BuildMenuBuildingsScreen buildMenuBuildingsScreen = buildingsScreen;
			buildMenuBuildingsScreen.onBuildingSelected = (Action<BuildingDef>)Delegate.Combine(buildMenuBuildingsScreen.onBuildingSelected, new Action<BuildingDef>(OnBuildingSelected));
			buildingsScreen.Show(false);
			Game.Instance.Subscribe(288942073, OnUIClear);
			Game.Instance.Subscribe(-1190690038, OnBuildToolDeactivated);
			Initialize();
			this.rectTransform().anchoredPosition = Vector2.zero;
		}
		else
		{
			base.gameObject.SetActive(flag);
		}
	}

	private void Initialize()
	{
		foreach (KeyValuePair<HashedString, BuildMenuCategoriesScreen> submenu in submenus)
		{
			BuildMenuCategoriesScreen value = submenu.Value;
			value.Close();
			UnityEngine.Object.DestroyImmediate(value.gameObject);
		}
		submenuStack.Clear();
		tagCategoryMap = new Dictionary<Tag, HashedString>();
		tagOrderMap = new Dictionary<Tag, int>();
		categorizedBuildingMap = new Dictionary<HashedString, List<BuildingDef>>();
		categorizedCategoryMap = new Dictionary<HashedString, List<HashedString>>();
		int building_index = 0;
		DisplayInfo orderedBuildings = OrderedBuildings;
		PopulateCategorizedMaps(orderedBuildings.category, 0, orderedBuildings.data, tagCategoryMap, tagOrderMap, ref building_index, categorizedBuildingMap, categorizedCategoryMap);
		BuildMenuCategoriesScreen buildMenuCategoriesScreen = submenus[ROOT_HASHSTR];
		buildMenuCategoriesScreen.Show(true);
		buildMenuCategoriesScreen.modalKeyInputBehaviour = false;
		foreach (KeyValuePair<HashedString, BuildMenuCategoriesScreen> submenu2 in submenus)
		{
			HashedString key = submenu2.Key;
			if (!(key == ROOT_HASHSTR) && categorizedCategoryMap.TryGetValue(key, out List<HashedString> value2))
			{
				BuildMenuCategoriesScreen value3 = submenu2.Value;
				Image component = value3.GetComponent<Image>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					component.enabled = (value2.Count > 0);
				}
			}
		}
		PositionMenus();
	}

	[ContextMenu("PositionMenus")]
	private void PositionMenus()
	{
		foreach (KeyValuePair<HashedString, BuildMenuCategoriesScreen> submenu in submenus)
		{
			HashedString key = submenu.Key;
			BuildMenuCategoriesScreen value = submenu.Value;
			LayoutGroup component = value.GetComponent<LayoutGroup>();
			Vector2 anchoredPosition;
			PadInfo padInfo;
			if (key == ROOT_HASHSTR)
			{
				anchoredPosition = rootMenuOffset;
				padInfo = rootMenuPadding;
				Image component2 = value.GetComponent<Image>();
				component2.enabled = false;
			}
			else
			{
				anchoredPosition = nestedMenuOffset;
				padInfo = nestedMenuPadding;
			}
			value.rectTransform().anchoredPosition = anchoredPosition;
			component.padding.left = padInfo.left;
			component.padding.right = padInfo.right;
			component.padding.top = padInfo.top;
			component.padding.bottom = padInfo.bottom;
		}
		buildingsScreen.rectTransform().anchoredPosition = buildingsMenuOffset;
	}

	public void Refresh()
	{
		foreach (KeyValuePair<HashedString, BuildMenuCategoriesScreen> submenu in submenus)
		{
			BuildMenuCategoriesScreen value = submenu.Value;
			value.UpdateBuildableStates(true);
		}
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		Game.Instance.Subscribe(-107300940, OnResearchComplete);
	}

	protected override void OnCmpDisable()
	{
		Game.Instance.Unsubscribe(-107300940, OnResearchComplete);
		base.OnCmpDisable();
	}

	private BuildMenuCategoriesScreen CreateCategorySubMenu(HashedString category, int depth, object data, Dictionary<HashedString, List<BuildingDef>> categorized_building_map, Dictionary<HashedString, List<HashedString>> categorized_category_map, Dictionary<Tag, HashedString> tag_category_map, BuildMenuBuildingsScreen buildings_screen)
	{
		BuildMenuCategoriesScreen buildMenuCategoriesScreen = Util.KInstantiateUI<BuildMenuCategoriesScreen>(categoriesMenuPrefab.gameObject, base.gameObject, true);
		buildMenuCategoriesScreen.Show(false);
		buildMenuCategoriesScreen.Configure(category, depth, data, categorizedBuildingMap, categorizedCategoryMap, buildingsScreen);
		BuildMenuCategoriesScreen buildMenuCategoriesScreen2 = buildMenuCategoriesScreen;
		buildMenuCategoriesScreen2.onCategoryClicked = (Action<HashedString, int>)Delegate.Combine(buildMenuCategoriesScreen2.onCategoryClicked, new Action<HashedString, int>(OnCategoryClicked));
		buildMenuCategoriesScreen.name = "BuildMenu_" + category.ToString();
		return buildMenuCategoriesScreen;
	}

	private void PopulateCategorizedMaps(HashedString category, int depth, object data, Dictionary<Tag, HashedString> category_map, Dictionary<Tag, int> order_map, ref int building_index, Dictionary<HashedString, List<BuildingDef>> categorized_building_map, Dictionary<HashedString, List<HashedString>> categorized_category_map)
	{
		Type type = data.GetType();
		if (type == typeof(DisplayInfo))
		{
			DisplayInfo displayInfo = (DisplayInfo)data;
			if (!categorized_category_map.TryGetValue(category, out List<HashedString> value))
			{
				value = (categorized_category_map[category] = new List<HashedString>());
			}
			value.Add(displayInfo.category);
			PopulateCategorizedMaps(displayInfo.category, depth + 1, displayInfo.data, category_map, order_map, ref building_index, categorized_building_map, categorized_category_map);
		}
		else if (typeof(IList<DisplayInfo>).IsAssignableFrom(type))
		{
			IList<DisplayInfo> list2 = (IList<DisplayInfo>)data;
			if (!categorized_category_map.TryGetValue(category, out List<HashedString> value2))
			{
				value2 = (categorized_category_map[category] = new List<HashedString>());
			}
			foreach (DisplayInfo item in list2)
			{
				DisplayInfo current = item;
				value2.Add(current.category);
				PopulateCategorizedMaps(current.category, depth + 1, current.data, category_map, order_map, ref building_index, categorized_building_map, categorized_category_map);
			}
		}
		else
		{
			IList<BuildingInfo> list4 = (IList<BuildingInfo>)data;
			foreach (BuildingInfo item2 in list4)
			{
				BuildingInfo current2 = item2;
				Tag key = new Tag(current2.id);
				category_map[key] = category;
				order_map[key] = building_index;
				building_index++;
				if (!categorized_building_map.TryGetValue(category, out List<BuildingDef> value3))
				{
					value3 = (categorized_building_map[category] = new List<BuildingDef>());
				}
				BuildingDef buildingDef = Assets.GetBuildingDef(current2.id);
				buildingDef.HotKey = current2.hotkey;
				value3.Add(buildingDef);
			}
		}
		submenus[category] = CreateCategorySubMenu(category, depth, data, categorizedBuildingMap, categorizedCategoryMap, tagCategoryMap, buildingsScreen);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.Consumed)
		{
			return;
		}
		if (mouseOver && ConsumeMouseScroll && !e.TryConsume(Action.ZoomIn) && !e.TryConsume(Action.ZoomOut))
		{
			goto IL_003a;
		}
		goto IL_003a;
		IL_003a:
		if (!e.Consumed && selectedCategory.IsValid && e.TryConsume(Action.Escape))
		{
			OnUIClear(null);
		}
		else if (!e.Consumed)
		{
			base.OnKeyDown(e);
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (selectedCategory.IsValid && PlayerController.Instance.ConsumeIfNotDragging(e, Action.MouseRight))
		{
			OnUIClear(null);
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect", false));
		}
		if (!e.Consumed)
		{
			base.OnKeyUp(e);
		}
	}

	private void OnUIClear(object data)
	{
		SelectTool.Instance.Activate();
		PlayerController.Instance.ActivateTool(SelectTool.Instance);
		SelectTool.Instance.Select(null, true);
		productInfoScreen.materialSelectionPanel.PriorityScreen.ResetPriority();
		CloseMenus();
	}

	private void OnBuildToolDeactivated(object data)
	{
		if (updating)
		{
			deactivateToolQueued = true;
		}
		else
		{
			CloseMenus();
			productInfoScreen.materialSelectionPanel.PriorityScreen.ResetPriority();
		}
	}

	private void CloseMenus()
	{
		productInfoScreen.Close();
		while (submenuStack.Count > 0)
		{
			KIconToggleMenu kIconToggleMenu = submenuStack.Pop();
			kIconToggleMenu.Close();
			productInfoScreen.Close();
		}
		selectedCategory = HashedString.Invalid;
		submenus[ROOT_HASHSTR].ClearSelection();
	}

	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
		if (timeSinceNotificationPing < 8f)
		{
			timeSinceNotificationPing += Time.unscaledDeltaTime;
		}
		if (timeSinceNotificationPing >= 0.5f)
		{
			notificationPingCount = 0;
		}
	}

	public void PlayNewBuildingSounds()
	{
		if (KTime.Instance.UnscaledGameTime - initTime > 1.5f)
		{
			if (Instance.timeSinceNotificationPing >= 8f)
			{
				string sound = GlobalAssets.GetSound("NewBuildable_Embellishment", false);
				if (sound != null)
				{
					EventInstance instance = SoundEvent.BeginOneShot(sound, SoundListenerController.Instance.transform.GetPosition());
					SoundEvent.EndOneShot(instance);
				}
			}
			string sound2 = GlobalAssets.GetSound("NewBuildable", false);
			if (sound2 != null)
			{
				EventInstance instance2 = SoundEvent.BeginOneShot(sound2, SoundListenerController.Instance.transform.GetPosition());
				instance2.setParameterValue("playCount", (float)Instance.notificationPingCount);
				SoundEvent.EndOneShot(instance2);
			}
		}
		timeSinceNotificationPing = 0f;
		notificationPingCount++;
	}

	public PlanScreen.RequirementsState BuildableState(BuildingDef def)
	{
		PlanScreen.RequirementsState result = PlanScreen.RequirementsState.Complete;
		if (!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive)
		{
			if (!Db.Get().TechItems.IsTechItemComplete(def.PrefabID))
			{
				result = PlanScreen.RequirementsState.Tech;
			}
			else if (!ProductInfoScreen.MaterialsMet(def.CraftRecipe))
			{
				result = PlanScreen.RequirementsState.Materials;
			}
		}
		return result;
	}

	private void CloseProductInfoScreen()
	{
		productInfoScreen.ClearProduct(true);
		productInfoScreen.Show(false);
	}

	private void Update()
	{
		if (deactivateToolQueued)
		{
			deactivateToolQueued = false;
			OnBuildToolDeactivated(null);
		}
		elapsedTime += Time.unscaledDeltaTime;
		if (!(elapsedTime <= updateInterval))
		{
			elapsedTime = 0f;
			updating = true;
			if (productInfoScreen.gameObject.activeSelf)
			{
				productInfoScreen.materialSelectionPanel.UpdateResourceToggleValues();
			}
			foreach (KIconToggleMenu item in submenuStack)
			{
				if (item is BuildMenuCategoriesScreen)
				{
					(item as BuildMenuCategoriesScreen).UpdateBuildableStates(false);
				}
			}
			submenus[ROOT_HASHSTR].UpdateBuildableStates(false);
			updating = false;
		}
	}

	private void OnRecipeElementsFullySelected()
	{
		if ((UnityEngine.Object)selectedBuilding == (UnityEngine.Object)null)
		{
			Debug.Log("No def!");
		}
		if (selectedBuilding.isKAnimTile && selectedBuilding.isUtility)
		{
			IList<Tag> getSelectedElementAsList = productInfoScreen.materialSelectionPanel.GetSelectedElementAsList;
			BaseUtilityBuildTool baseUtilityBuildTool = (!((UnityEngine.Object)selectedBuilding.BuildingComplete.GetComponent<Wire>() != (UnityEngine.Object)null)) ? ((BaseUtilityBuildTool)UtilityBuildTool.Instance) : ((BaseUtilityBuildTool)WireBuildTool.Instance);
			baseUtilityBuildTool.Activate(selectedBuilding, getSelectedElementAsList);
		}
		else
		{
			BuildTool.Instance.Activate(selectedBuilding, productInfoScreen.materialSelectionPanel.GetSelectedElementAsList, null);
		}
	}

	private void OnBuildingSelected(BuildingDef def)
	{
		if (!selecting)
		{
			selecting = true;
			selectedBuilding = def;
			buildingsScreen.SetHasFocus(false);
			foreach (KeyValuePair<HashedString, BuildMenuCategoriesScreen> submenu in submenus)
			{
				BuildMenuCategoriesScreen value = submenu.Value;
				value.SetHasFocus(false);
			}
			ToolMenu.Instance.ClearSelection();
			if ((UnityEngine.Object)def != (UnityEngine.Object)null)
			{
				Vector2 anchoredPosition = productInfoScreen.rectTransform().anchoredPosition;
				RectTransform rectTransform = buildingsScreen.rectTransform();
				Vector2 anchoredPosition2 = rectTransform.anchoredPosition;
				anchoredPosition.y = anchoredPosition2.y;
				Vector2 anchoredPosition3 = rectTransform.anchoredPosition;
				float x = anchoredPosition3.x;
				Vector2 sizeDelta = rectTransform.sizeDelta;
				anchoredPosition.x = x + sizeDelta.x + 10f;
				productInfoScreen.rectTransform().anchoredPosition = anchoredPosition;
				productInfoScreen.ClearProduct(false);
				productInfoScreen.Show(true);
				productInfoScreen.ConfigureScreen(def);
			}
			else
			{
				productInfoScreen.Close();
			}
			selecting = false;
		}
	}

	private void OnCategoryClicked(HashedString new_category, int depth)
	{
		while (submenuStack.Count > depth)
		{
			KIconToggleMenu kIconToggleMenu = submenuStack.Pop();
			kIconToggleMenu.ClearSelection();
			kIconToggleMenu.Close();
		}
		productInfoScreen.Close();
		if (new_category != selectedCategory && new_category.IsValid)
		{
			foreach (KIconToggleMenu item in submenuStack)
			{
				if (item is BuildMenuCategoriesScreen)
				{
					BuildMenuCategoriesScreen buildMenuCategoriesScreen = item as BuildMenuCategoriesScreen;
					buildMenuCategoriesScreen.SetHasFocus(false);
				}
			}
			selectedCategory = new_category;
			submenus.TryGetValue(new_category, out BuildMenuCategoriesScreen value);
			if ((UnityEngine.Object)value != (UnityEngine.Object)null)
			{
				value.Show(true);
				value.SetHasFocus(true);
				submenuStack.Push(value);
			}
		}
		else
		{
			selectedCategory = HashedString.Invalid;
		}
		foreach (KIconToggleMenu item2 in submenuStack)
		{
			if (item2 is BuildMenuCategoriesScreen)
			{
				(item2 as BuildMenuCategoriesScreen).UpdateBuildableStates(true);
			}
		}
		submenus[ROOT_HASHSTR].UpdateBuildableStates(true);
	}

	public void RefreshProductInfoScreen(BuildingDef def)
	{
		if ((UnityEngine.Object)productInfoScreen.currentDef == (UnityEngine.Object)def)
		{
			productInfoScreen.ClearProduct(false);
			productInfoScreen.Show(true);
			productInfoScreen.ConfigureScreen(def);
		}
	}

	private HashedString GetParentCategory(HashedString desired_category)
	{
		foreach (KeyValuePair<HashedString, List<HashedString>> item in categorizedCategoryMap)
		{
			foreach (HashedString item2 in item.Value)
			{
				if (item2 == desired_category)
				{
					return item.Key;
				}
			}
		}
		return HashedString.Invalid;
	}

	private void AddParentCategories(HashedString child_category, ICollection<HashedString> categories)
	{
		while (true)
		{
			HashedString parentCategory = GetParentCategory(child_category);
			if (parentCategory == HashedString.Invalid)
			{
				break;
			}
			categories.Add(parentCategory);
			child_category = parentCategory;
		}
	}

	private void OnResearchComplete(object data)
	{
		HashSet<HashedString> hashSet = new HashSet<HashedString>();
		Tech tech = (Tech)data;
		foreach (TechItem unlockedItem in tech.unlockedItems)
		{
			BuildingDef buildingDef = Assets.GetBuildingDef(unlockedItem.Id);
			if ((UnityEngine.Object)buildingDef == (UnityEngine.Object)null)
			{
				DebugUtil.LogWarningArgs($"Tech '{tech.Name}' unlocked building '{unlockedItem.Id}' but no such building exists");
			}
			else
			{
				HashedString hashedString = tagCategoryMap[buildingDef.Tag];
				hashSet.Add(hashedString);
				AddParentCategories(hashedString, hashSet);
			}
		}
		UpdateNotifications(hashSet, OrderedBuildings);
	}

	private void UpdateNotifications(ICollection<HashedString> updated_categories, object data)
	{
		foreach (KeyValuePair<HashedString, BuildMenuCategoriesScreen> submenu in submenus)
		{
			BuildMenuCategoriesScreen value = submenu.Value;
			value.UpdateNotifications(updated_categories);
		}
	}

	public PrioritySetting GetBuildingPriority()
	{
		return productInfoScreen.materialSelectionPanel.PriorityScreen.GetLastSelectedPriority();
	}
}
