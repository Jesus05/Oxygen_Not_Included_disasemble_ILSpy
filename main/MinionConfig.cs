using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class MinionConfig : IEntityConfig
{
	public struct LaserEffect
	{
		public string id;

		public string animFile;

		public string anim;

		public HashedString context;
	}

	public static string ID = "Minion";

	public static string MINION_BASE_TRAIT_ID = ID + "BaseTrait";

	public const int MINION_BASE_SYMBOL_LAYER = 0;

	public const int MINION_HAIR_ALWAYS_HACK_LAYER = 1;

	public const int MINION_EXPRESSION_SYMBOL_LAYER = 2;

	public const int MINION_MOUTH_FLAP_LAYER = 3;

	public const int MINION_CLOTHING_SYMBOL_LAYER = 4;

	public const int MINION_PICKUP_SYMBOL_LAYER = 5;

	public const int MINION_SUIT_SYMBOL_LAYER = 6;

	public GameObject CreatePrefab()
	{
		string text = DUPLICANTS.MODIFIERS.BASEDUPLICANT.NAME;
		GameObject gameObject = EntityTemplates.CreateEntity(ID, text, true);
		gameObject.AddOrGet<StateMachineController>();
		MinionModifiers modifiers = gameObject.AddOrGet<MinionModifiers>();
		AddMinionAmounts(modifiers);
		Trait trait = Db.Get().CreateTrait(MINION_BASE_TRAIT_ID, text, text, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Stamina.deltaAttribute.Id, -0.116666667f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -1666.66663f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, 4000000f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Toxicity.deltaAttribute.Id, 0f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.AirConsumptionRate.Id, 0.1f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Bladder.deltaAttribute.Id, 0.166666672f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 100f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.MaxUnderwaterTravelCost.Id, 8f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.DecorExpectation.Id, 0f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.FoodExpectation.Id, 0f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.ToiletEfficiency.Id, 1f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.RoomTemperaturePreference.Id, 0f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.CarryAmount.Id, 200f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.QualityOfLife.Id, 1f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Attributes.Sneezyness.Id, 0f, text, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.ImmuneLevel.deltaAttribute.Id, 0.025f, text, false, false, true));
		gameObject.AddOrGet<MinionBrain>();
		gameObject.AddOrGet<KPrefabID>().AddTag(GameTags.DupeBrain);
		gameObject.AddOrGet<Worker>();
		gameObject.AddOrGet<ChoreConsumer>();
		Storage storage = gameObject.AddOrGet<Storage>();
		storage.fxPrefix = Storage.FXPrefix.PickedUp;
		storage.dropOnLoad = true;
		storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Preserve,
			Storage.StoredItemModifier.Seal
		});
		gameObject.AddOrGet<Health>();
		OxygenBreather oxygenBreather = gameObject.AddOrGet<OxygenBreather>();
		oxygenBreather.O2toCO2conversion = 0.02f;
		oxygenBreather.lowOxygenThreshold = 0.52f;
		oxygenBreather.noOxygenThreshold = 0.05f;
		oxygenBreather.mouthOffset = new Vector2f(0.25f, 0.7f);
		oxygenBreather.minCO2ToEmit = 0.02f;
		oxygenBreather.breathableCells = new CellOffset[6]
		{
			new CellOffset(0, 0),
			new CellOffset(0, 1),
			new CellOffset(1, 1),
			new CellOffset(-1, 1),
			new CellOffset(1, 0),
			new CellOffset(-1, 0)
		};
		gameObject.AddOrGet<WarmBlooded>();
		gameObject.AddOrGet<MinionIdentity>();
		GridVisibility gridVisibility = gameObject.AddOrGet<GridVisibility>();
		gridVisibility.radius = 30f;
		gridVisibility.innerRadius = 20f;
		gameObject.AddOrGet<MiningSounds>();
		gameObject.AddOrGet<SaveLoadRoot>();
		gameObject.AddOrGet<AntiCluster>();
		Navigator navigator = gameObject.AddOrGet<Navigator>();
		navigator.NavGridName = "MinionNavGrid";
		navigator.CurrentNavType = NavType.Floor;
		KBatchedAnimController kBatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
		kBatchedAnimController.isMovable = true;
		kBatchedAnimController.sceneLayer = Grid.SceneLayer.Move;
		kBatchedAnimController.AnimFiles = new KAnimFile[9]
		{
			Assets.GetAnim("body_comp_default_kanim"),
			Assets.GetAnim("anim_construction_default_kanim"),
			Assets.GetAnim("anim_idles_default_kanim"),
			Assets.GetAnim("anim_loco_firepole_kanim"),
			Assets.GetAnim("anim_loco_new_kanim"),
			Assets.GetAnim("anim_loco_tube_kanim"),
			Assets.GetAnim("anim_loco_hover_kanim"),
			Assets.GetAnim("anim_construction_firepole_kanim"),
			Assets.GetAnim("anim_construction_jetsuit_kanim")
		};
		KBoxCollider2D kBoxCollider2D = gameObject.AddOrGet<KBoxCollider2D>();
		kBoxCollider2D.offset = new Vector2(0f, 0.8f);
		kBoxCollider2D.size = new Vector2(1f, 1.5f);
		SnapOn snapOn = gameObject.AddOrGet<SnapOn>();
		snapOn.snapPoints = new List<SnapOn.SnapPoint>(new SnapOn.SnapPoint[17]
		{
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = (HashedString)"dig",
				buildFile = Assets.GetAnim("excavator_kanim"),
				overrideSymbol = (HashedString)"snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = (HashedString)"build",
				buildFile = Assets.GetAnim("constructor_gun_kanim"),
				overrideSymbol = (HashedString)"snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = (HashedString)"fetchliquid",
				buildFile = Assets.GetAnim("water_gun_kanim"),
				overrideSymbol = (HashedString)"snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = (HashedString)"paint",
				buildFile = Assets.GetAnim("painting_gun_kanim"),
				overrideSymbol = (HashedString)"snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = (HashedString)"harvest",
				buildFile = Assets.GetAnim("plant_harvester_gun_kanim"),
				overrideSymbol = (HashedString)"snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = (HashedString)"capture",
				buildFile = Assets.GetAnim("net_gun_kanim"),
				overrideSymbol = (HashedString)"snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = (HashedString)"attack",
				buildFile = Assets.GetAnim("attack_gun_kanim"),
				overrideSymbol = (HashedString)"snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = (HashedString)"pickup",
				buildFile = Assets.GetAnim("pickupdrop_gun_kanim"),
				overrideSymbol = (HashedString)"snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = (HashedString)"store",
				buildFile = Assets.GetAnim("pickupdrop_gun_kanim"),
				overrideSymbol = (HashedString)"snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = (HashedString)"disinfect",
				buildFile = Assets.GetAnim("plant_spray_gun_kanim"),
				overrideSymbol = (HashedString)"snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = (HashedString)"tend",
				buildFile = Assets.GetAnim("plant_harvester_gun_kanim"),
				overrideSymbol = (HashedString)"snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "carry",
				automatic = false,
				context = (HashedString)"",
				buildFile = null,
				overrideSymbol = (HashedString)"snapTo_chest"
			},
			new SnapOn.SnapPoint
			{
				pointName = "build",
				automatic = false,
				context = (HashedString)"",
				buildFile = null,
				overrideSymbol = (HashedString)"snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "remote",
				automatic = false,
				context = (HashedString)"",
				buildFile = null,
				overrideSymbol = (HashedString)"snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "snapTo_neck",
				automatic = false,
				context = (HashedString)"",
				buildFile = Assets.GetAnim("helm_oxygen_kanim"),
				overrideSymbol = (HashedString)"snapTo_neck"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = (HashedString)"powertinker",
				buildFile = Assets.GetAnim("electrician_gun_kanim"),
				overrideSymbol = (HashedString)"snapTo_rgtHand"
			},
			new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = (HashedString)"specialistdig",
				buildFile = Assets.GetAnim("excavator_kanim"),
				overrideSymbol = (HashedString)"snapTo_rgtHand"
			}
		});
		gameObject.AddOrGet<Effects>();
		gameObject.AddOrGet<Traits>();
		gameObject.AddOrGet<AttributeLevels>();
		gameObject.AddOrGet<AttributeConverters>();
		PrimaryElement primaryElement = gameObject.AddOrGet<PrimaryElement>();
		primaryElement.InternalTemperature = 310.15f;
		primaryElement.MassPerUnit = 30f;
		primaryElement.ElementID = SimHashes.Creature;
		gameObject.AddOrGet<ChoreProvider>();
		gameObject.AddOrGetDef<DebugGoToMonitor.Def>();
		gameObject.AddOrGetDef<SpeechMonitor.Def>();
		gameObject.AddOrGetDef<BlinkMonitor.Def>();
		gameObject.AddOrGetDef<ConversationMonitor.Def>();
		gameObject.AddOrGet<Sensors>();
		gameObject.AddOrGet<Chattable>();
		gameObject.AddOrGet<FaceGraph>();
		gameObject.AddOrGet<Accessorizer>();
		gameObject.AddOrGet<Schedulable>();
		LoopingSounds loopingSounds = gameObject.AddOrGet<LoopingSounds>();
		loopingSounds.updatePosition = true;
		gameObject.AddOrGet<AnimEventHandler>();
		FactionAlignment factionAlignment = gameObject.AddOrGet<FactionAlignment>();
		factionAlignment.Alignment = FactionManager.FactionID.Duplicant;
		gameObject.AddOrGet<Weapon>();
		gameObject.AddOrGet<RangedAttackable>();
		gameObject.AddOrGet<CharacterOverlay>();
		OccupyArea occupyArea = gameObject.AddOrGet<OccupyArea>();
		occupyArea.objectLayers = new ObjectLayer[1];
		occupyArea.ApplyToCells = false;
		occupyArea.OccupiedCellsOffsets = new CellOffset[2]
		{
			new CellOffset(0, 0),
			new CellOffset(0, 1)
		};
		gameObject.AddOrGet<Pickupable>();
		CreatureSimTemperatureTransfer creatureSimTemperatureTransfer = gameObject.AddOrGet<CreatureSimTemperatureTransfer>();
		creatureSimTemperatureTransfer.SurfaceArea = 10f;
		creatureSimTemperatureTransfer.Thickness = 0.01f;
		gameObject.AddOrGet<DiseaseTrigger>();
		gameObject.AddOrGet<ClothingWearer>();
		gameObject.AddOrGet<SuitEquipper>();
		DecorProvider decorProvider = gameObject.AddOrGet<DecorProvider>();
		decorProvider.baseRadius = 3f;
		decorProvider.isMovable = true;
		gameObject.AddOrGet<ConsumableConsumer>();
		gameObject.AddOrGet<NoiseListener>();
		gameObject.AddOrGet<MinionResume>();
		DuplicantNoiseLevels.SetupNoiseLevels();
		SetupLaserEffects(gameObject);
		SymbolOverrideController symbolOverrideController = SymbolOverrideControllerUtil.AddToPrefab(gameObject);
		symbolOverrideController.applySymbolOverridesEveryFrame = true;
		ConfigureSymbols(gameObject);
		return gameObject;
	}

	private void SetupLaserEffects(GameObject prefab)
	{
		GameObject gameObject = new GameObject("LaserEffect");
		gameObject.transform.parent = prefab.transform;
		KBatchedAnimEventToggler kBatchedAnimEventToggler = gameObject.AddComponent<KBatchedAnimEventToggler>();
		kBatchedAnimEventToggler.eventSource = prefab;
		kBatchedAnimEventToggler.enableEvent = "LaserOn";
		kBatchedAnimEventToggler.disableEvent = "LaserOff";
		kBatchedAnimEventToggler.entries = new List<KBatchedAnimEventToggler.Entry>();
		LaserEffect[] array = new LaserEffect[13]
		{
			new LaserEffect
			{
				id = "DigEffect",
				animFile = "laser_kanim",
				anim = "idle",
				context = (HashedString)"dig"
			},
			new LaserEffect
			{
				id = "BuildEffect",
				animFile = "construct_beam_kanim",
				anim = "loop",
				context = (HashedString)"build"
			},
			new LaserEffect
			{
				id = "FetchLiquidEffect",
				animFile = "hose_fx_kanim",
				anim = "loop",
				context = (HashedString)"fetchliquid"
			},
			new LaserEffect
			{
				id = "PaintEffect",
				animFile = "paint_beam_kanim",
				anim = "loop",
				context = (HashedString)"paint"
			},
			new LaserEffect
			{
				id = "HarvestEffect",
				animFile = "plant_harvest_beam_kanim",
				anim = "loop",
				context = (HashedString)"harvest"
			},
			new LaserEffect
			{
				id = "CaptureEffect",
				animFile = "net_gun_fx_kanim",
				anim = "loop",
				context = (HashedString)"capture"
			},
			new LaserEffect
			{
				id = "AttackEffect",
				animFile = "attack_beam_fx_kanim",
				anim = "loop",
				context = (HashedString)"attack"
			},
			new LaserEffect
			{
				id = "PickupEffect",
				animFile = "vacuum_fx_kanim",
				anim = "loop",
				context = (HashedString)"pickup"
			},
			new LaserEffect
			{
				id = "StoreEffect",
				animFile = "vacuum_reverse_fx_kanim",
				anim = "loop",
				context = (HashedString)"store"
			},
			new LaserEffect
			{
				id = "DisinfectEffect",
				animFile = "plant_spray_beam_kanim",
				anim = "loop",
				context = (HashedString)"disinfect"
			},
			new LaserEffect
			{
				id = "TendEffect",
				animFile = "plant_tending_beam_fx_kanim",
				anim = "loop",
				context = (HashedString)"tend"
			},
			new LaserEffect
			{
				id = "PowerTinkerEffect",
				animFile = "electrician_beam_fx_kanim",
				anim = "idle",
				context = (HashedString)"powertinker"
			},
			new LaserEffect
			{
				id = "SpecialistDigEffect",
				animFile = "senior_miner_beam_fx_kanim",
				anim = "idle",
				context = (HashedString)"specialistdig"
			}
		};
		KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
		LaserEffect[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			LaserEffect laserEffect = array2[i];
			GameObject gameObject2 = new GameObject(laserEffect.id);
			gameObject2.transform.parent = gameObject.transform;
			KPrefabID kPrefabID = gameObject2.AddOrGet<KPrefabID>();
			kPrefabID.PrefabTag = new Tag(laserEffect.id);
			KBatchedAnimTracker kBatchedAnimTracker = gameObject2.AddOrGet<KBatchedAnimTracker>();
			kBatchedAnimTracker.controller = component;
			kBatchedAnimTracker.symbol = new HashedString("snapTo_rgtHand");
			kBatchedAnimTracker.offset = new Vector3(195f, -35f, 0f);
			kBatchedAnimTracker.useTargetPoint = true;
			KBatchedAnimController kBatchedAnimController = gameObject2.AddOrGet<KBatchedAnimController>();
			kBatchedAnimController.AnimFiles = new KAnimFile[1]
			{
				Assets.GetAnim(laserEffect.animFile)
			};
			KBatchedAnimEventToggler.Entry entry = default(KBatchedAnimEventToggler.Entry);
			entry.anim = laserEffect.anim;
			entry.context = laserEffect.context;
			entry.controller = kBatchedAnimController;
			KBatchedAnimEventToggler.Entry item = entry;
			kBatchedAnimEventToggler.entries.Add(item);
			gameObject2.AddOrGet<LoopingSounds>();
		}
	}

	public void OnPrefabInit(GameObject go)
	{
		AmountInstance amountInstance = Db.Get().Amounts.ImmuneLevel.Lookup(go);
		amountInstance.value = amountInstance.GetMax();
		AmountInstance amountInstance2 = Db.Get().Amounts.Bladder.Lookup(go);
		amountInstance2.value = Random.Range(0f, 10f);
		AmountInstance amountInstance3 = Db.Get().Amounts.Stress.Lookup(go);
		amountInstance3.value = 5f;
		AmountInstance amountInstance4 = Db.Get().Amounts.Temperature.Lookup(go);
		amountInstance4.value = 310.15f;
		AmountInstance amountInstance5 = Db.Get().Amounts.Stamina.Lookup(go);
		amountInstance5.value = amountInstance5.GetMax();
		AmountInstance amountInstance6 = Db.Get().Amounts.Breath.Lookup(go);
		amountInstance6.value = amountInstance6.GetMax();
		AmountInstance amountInstance7 = Db.Get().Amounts.Calories.Lookup(go);
		amountInstance7.value = 0.8875f * amountInstance7.GetMax();
	}

	public void OnSpawn(GameObject go)
	{
		Sensors component = go.GetComponent<Sensors>();
		component.Add(new PathProberSensor(component));
		component.Add(new SafeCellSensor(component));
		component.Add(new IdleCellSensor(component));
		component.Add(new PickupableSensor(component));
		component.Add(new ClosestEdibleSensor(component));
		component.Add(new BreathableAreaSensor(component));
		component.Add(new AssignableReachabilitySensor(component));
		component.Add(new ToiletSensor(component));
		component.Add(new MingleCellSensor(component));
		StateMachineController component2 = go.GetComponent<StateMachineController>();
		RationalAi.Instance instance = new RationalAi.Instance(component2);
		instance.StartSM();
		if (go.GetComponent<OxygenBreather>().GetGasProvider() == null)
		{
			go.GetComponent<OxygenBreather>().SetGasProvider(new GasBreatherFromWorldProvider());
		}
		Navigator component3 = go.GetComponent<Navigator>();
		component3.transitionDriver.overrideLayers.Add(new BipedTransitionLayer(component3, 3.325f, 2.5f));
		component3.transitionDriver.overrideLayers.Add(new DoorTransitionLayer(component3));
		component3.transitionDriver.overrideLayers.Add(new TubeTransitionLayer(component3));
		component3.transitionDriver.overrideLayers.Add(new LadderDiseaseTransitionLayer(component3));
		component3.transitionDriver.overrideLayers.Add(new ReactableTransitionLayer(component3));
		component3.transitionDriver.overrideLayers.Add(new SplashTransitionLayer(component3));
		ThreatMonitor.Instance sMI = go.GetSMI<ThreatMonitor.Instance>();
		if (sMI != null)
		{
			sMI.def.fleethresholdState = Health.HealthState.Critical;
		}
	}

	public static void AddMinionAmounts(Modifiers modifiers)
	{
		modifiers.initialAmounts.Add(Db.Get().Amounts.HitPoints.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.Stamina.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.Calories.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.ImmuneLevel.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.Breath.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.Stress.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.Toxicity.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.Bladder.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.Temperature.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.ExternalTemperature.Id);
		modifiers.initialAmounts.Add(Db.Get().Amounts.Decor.Id);
	}

	public static void ConfigureSymbols(GameObject go)
	{
		KBatchedAnimController component = go.GetComponent<KBatchedAnimController>();
		component.SetSymbolVisiblity("snapto_hat", false);
		component.SetSymbolVisiblity("snapTo_hat_hair", false);
		component.SetSymbolVisiblity("snapto_chest", false);
		component.SetSymbolVisiblity("snapto_neck", false);
		component.SetSymbolVisiblity("snapto_goggles", false);
		component.SetSymbolVisiblity("snapto_pivot", false);
		component.SetSymbolVisiblity("snapTo_rgtHand", false);
	}
}
