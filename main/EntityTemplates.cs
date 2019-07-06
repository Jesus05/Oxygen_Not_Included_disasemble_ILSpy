using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class EntityTemplates
{
	public enum CollisionShape
	{
		CIRCLE,
		RECTANGLE,
		POLYGONAL
	}

	private static GameObject selectableEntityTemplate;

	private static GameObject unselectableEntityTemplate;

	private static GameObject baseEntityTemplate;

	private static GameObject placedEntityTemplate;

	private static GameObject baseOreTemplate;

	public static void CreateTemplates()
	{
		unselectableEntityTemplate = new GameObject("unselectableEntityTemplate");
		unselectableEntityTemplate.SetActive(false);
		unselectableEntityTemplate.AddComponent<KPrefabID>();
		Object.DontDestroyOnLoad(unselectableEntityTemplate);
		selectableEntityTemplate = Object.Instantiate(unselectableEntityTemplate);
		selectableEntityTemplate.name = "selectableEntityTemplate";
		selectableEntityTemplate.AddComponent<KSelectable>();
		Object.DontDestroyOnLoad(selectableEntityTemplate);
		baseEntityTemplate = Object.Instantiate(selectableEntityTemplate);
		baseEntityTemplate.name = "baseEntityTemplate";
		baseEntityTemplate.AddComponent<KBatchedAnimController>();
		baseEntityTemplate.AddComponent<SaveLoadRoot>();
		baseEntityTemplate.AddComponent<StateMachineController>();
		baseEntityTemplate.AddComponent<PrimaryElement>();
		baseEntityTemplate.AddComponent<SimTemperatureTransfer>();
		baseEntityTemplate.AddComponent<InfoDescription>();
		baseEntityTemplate.AddComponent<Notifier>();
		Object.DontDestroyOnLoad(baseEntityTemplate);
		placedEntityTemplate = Object.Instantiate(baseEntityTemplate);
		placedEntityTemplate.name = "placedEntityTemplate";
		placedEntityTemplate.AddComponent<KBoxCollider2D>();
		placedEntityTemplate.AddComponent<OccupyArea>();
		placedEntityTemplate.AddComponent<Modifiers>();
		placedEntityTemplate.AddComponent<DecorProvider>();
		Object.DontDestroyOnLoad(placedEntityTemplate);
	}

	private static void ConfigEntity(GameObject template, string id, string name, bool is_selectable = true)
	{
		template.name = id;
		KPrefabID kPrefabID = template.AddOrGet<KPrefabID>();
		kPrefabID.PrefabTag = TagManager.Create(id, name);
		if (is_selectable)
		{
			KSelectable kSelectable = template.AddOrGet<KSelectable>();
			kSelectable.SetName(name);
		}
	}

	public static GameObject CreateEntity(string id, string name, bool is_selectable = true)
	{
		GameObject gameObject = null;
		gameObject = ((!is_selectable) ? Object.Instantiate(unselectableEntityTemplate) : Object.Instantiate(selectableEntityTemplate));
		Object.DontDestroyOnLoad(gameObject);
		ConfigEntity(gameObject, id, name, is_selectable);
		return gameObject;
	}

	public static GameObject ConfigBasicEntity(GameObject template, string id, string name, string desc, float mass, bool unitMass, KAnimFile anim, string initialAnim, Grid.SceneLayer sceneLayer, SimHashes element = SimHashes.Creature, List<Tag> additionalTags = null, float defaultTemperature = 293f)
	{
		ConfigEntity(template, id, name, true);
		KPrefabID kPrefabID = template.AddOrGet<KPrefabID>();
		if (additionalTags != null)
		{
			foreach (Tag additionalTag in additionalTags)
			{
				kPrefabID.AddTag(additionalTag, false);
			}
		}
		KBatchedAnimController kBatchedAnimController = template.AddOrGet<KBatchedAnimController>();
		kBatchedAnimController.AnimFiles = new KAnimFile[1]
		{
			anim
		};
		kBatchedAnimController.sceneLayer = sceneLayer;
		kBatchedAnimController.initialAnim = initialAnim;
		template.AddOrGet<StateMachineController>();
		PrimaryElement primaryElement = template.AddOrGet<PrimaryElement>();
		primaryElement.ElementID = element;
		primaryElement.Temperature = defaultTemperature;
		if (unitMass)
		{
			primaryElement.MassPerUnit = mass;
			primaryElement.Units = 1f;
			GameTags.DisplayAsUnits.Add(kPrefabID.PrefabTag);
		}
		else
		{
			primaryElement.Mass = mass;
		}
		InfoDescription infoDescription = template.AddOrGet<InfoDescription>();
		infoDescription.description = desc;
		template.AddOrGet<Notifier>();
		return template;
	}

	public static GameObject CreateBasicEntity(string id, string name, string desc, float mass, bool unitMass, KAnimFile anim, string initialAnim, Grid.SceneLayer sceneLayer, SimHashes element = SimHashes.Creature, List<Tag> additionalTags = null, float defaultTemperature = 293f)
	{
		GameObject gameObject = Object.Instantiate(baseEntityTemplate);
		Object.DontDestroyOnLoad(gameObject);
		ConfigBasicEntity(gameObject, id, name, desc, mass, unitMass, anim, initialAnim, sceneLayer, element, additionalTags, defaultTemperature);
		return gameObject;
	}

	private static GameObject ConfigPlacedEntity(GameObject template, string id, string name, string desc, float mass, KAnimFile anim, string initialAnim, Grid.SceneLayer sceneLayer, int width, int height, EffectorValues decor, EffectorValues noise = default(EffectorValues), SimHashes element = SimHashes.Creature, List<Tag> additionalTags = null, float defaultTemperature = 293f)
	{
		if ((Object)anim == (Object)null)
		{
			Debug.LogErrorFormat("Cant create [{0}] entity without an anim", name);
		}
		ConfigBasicEntity(template, id, name, desc, mass, true, anim, initialAnim, sceneLayer, element, additionalTags, defaultTemperature);
		KBoxCollider2D kBoxCollider2D = template.AddOrGet<KBoxCollider2D>();
		kBoxCollider2D.size = new Vector2f(width, height);
		float num = 0.5f * (float)((width + 1) % 2);
		kBoxCollider2D.offset = new Vector2f(num, (float)height / 2f);
		KBatchedAnimController component = template.GetComponent<KBatchedAnimController>();
		component.Offset = new Vector3(num, 0f, 0f);
		OccupyArea occupyArea = template.AddOrGet<OccupyArea>();
		occupyArea.OccupiedCellsOffsets = GenerateOffsets(width, height);
		DecorProvider decorProvider = template.AddOrGet<DecorProvider>();
		decorProvider.SetValues(decor);
		decorProvider.overrideName = name;
		return template;
	}

	public static GameObject CreatePlacedEntity(string id, string name, string desc, float mass, KAnimFile anim, string initialAnim, Grid.SceneLayer sceneLayer, int width, int height, EffectorValues decor, EffectorValues noise = default(EffectorValues), SimHashes element = SimHashes.Creature, List<Tag> additionalTags = null, float defaultTemperature = 293f)
	{
		GameObject gameObject = Object.Instantiate(placedEntityTemplate);
		Object.DontDestroyOnLoad(gameObject);
		ConfigPlacedEntity(gameObject, id, name, desc, mass, anim, initialAnim, sceneLayer, width, height, decor, noise, element, additionalTags, defaultTemperature);
		return gameObject;
	}

	public static GameObject MakeHangingOffsets(GameObject template, int width, int height)
	{
		KBoxCollider2D component = template.GetComponent<KBoxCollider2D>();
		if ((bool)component)
		{
			component.size = new Vector2f(width, height);
			float a = 0.5f * (float)((width + 1) % 2);
			component.offset = new Vector2f(a, (float)(-height) / 2f + 1f);
		}
		OccupyArea component2 = template.GetComponent<OccupyArea>();
		if ((bool)component2)
		{
			component2.OccupiedCellsOffsets = GenerateHangingOffsets(width, height);
		}
		return template;
	}

	public static GameObject ExtendBuildingToRocketModule(GameObject template)
	{
		template.GetComponent<KBatchedAnimController>().isMovable = true;
		template.GetComponent<Building>().Def.ThermalConductivity = 0.1f;
		return template;
	}

	public static GameObject ExtendEntityToBasicPlant(GameObject template, float temperature_lethal_low = 218.15f, float temperature_warning_low = 283.15f, float temperature_warning_high = 303.15f, float temperature_lethal_high = 398.15f, SimHashes[] safe_elements = null, bool pressure_sensitive = true, float pressure_lethal_low = 0f, float pressure_warning_low = 0.15f, string crop_id = null, bool can_drown = true, bool can_tinker = true, bool require_solid_tile = true, bool should_grow_old = true, float max_age = 2400f)
	{
		template.AddOrGet<EntombVulnerable>();
		PressureVulnerable pressureVulnerable = template.AddOrGet<PressureVulnerable>();
		if (pressure_sensitive)
		{
			PressureVulnerable pressureVulnerable2 = pressureVulnerable;
			SimHashes[] safeAtmospheres = safe_elements;
			pressureVulnerable2.Configure(pressure_warning_low, pressure_lethal_low, 10f, 30f, safeAtmospheres);
		}
		else
		{
			pressureVulnerable.Configure(safe_elements);
		}
		template.AddOrGet<WiltCondition>();
		template.AddOrGet<Prioritizable>();
		template.AddOrGet<Uprootable>();
		if (require_solid_tile)
		{
			template.AddOrGet<UprootedMonitor>();
		}
		template.AddOrGet<ReceptacleMonitor>();
		template.AddOrGet<Notifier>();
		if (can_drown)
		{
			template.AddOrGet<DrowningMonitor>();
		}
		TemperatureVulnerable temperatureVulnerable = template.AddOrGet<TemperatureVulnerable>();
		temperatureVulnerable.Configure(temperature_warning_low, temperature_lethal_low, temperature_warning_high, temperature_lethal_high);
		template.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
		KPrefabID component = template.GetComponent<KPrefabID>();
		if (crop_id != null)
		{
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.HarvestableIDs, component.PrefabID().ToString());
			Crop.CropVal cropval = CROPS.CROP_TYPES.Find((Crop.CropVal m) => m.cropId == crop_id);
			Crop crop = template.AddOrGet<Crop>();
			crop.Configure(cropval);
			Growing growing = template.AddOrGet<Growing>();
			growing.growthTime = cropval.cropDuration;
			growing.shouldGrowOld = should_grow_old;
			growing.maxAge = max_age;
			template.AddOrGet<Harvestable>();
			template.AddOrGet<HarvestDesignatable>();
		}
		component.prefabInitFn += delegate(GameObject inst)
		{
			PressureVulnerable component2 = inst.GetComponent<PressureVulnerable>();
			if (safe_elements != null)
			{
				SimHashes[] array = safe_elements;
				foreach (SimHashes hash in array)
				{
					component2.safe_atmospheres.Add(ElementLoader.FindElementByHash(hash));
				}
			}
		};
		if (can_tinker)
		{
			Tinkerable.MakeFarmTinkerable(template);
		}
		return template;
	}

	public static GameObject ExtendEntityToWildCreature(GameObject prefab, int space_required_per_creature, float lifespan)
	{
		prefab.AddOrGetDef<AgeMonitor.Def>();
		prefab.AddOrGetDef<HappinessMonitor.Def>();
		Tag prefabTag = prefab.GetComponent<KPrefabID>().PrefabTag;
		WildnessMonitor.Def def = prefab.AddOrGetDef<WildnessMonitor.Def>();
		def.wildEffect = new Effect("Wild" + prefabTag.Name, STRINGS.CREATURES.MODIFIERS.WILD.NAME, STRINGS.CREATURES.MODIFIERS.WILD.TOOLTIP, 0f, true, true, false, null, 0f, null);
		def.wildEffect.Add(new AttributeModifier(Db.Get().Amounts.Wildness.deltaAttribute.Id, 0.008333334f, STRINGS.CREATURES.MODIFIERS.WILD.NAME, false, false, true));
		def.wildEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, 25f, STRINGS.CREATURES.MODIFIERS.WILD.NAME, false, false, true));
		def.wildEffect.Add(new AttributeModifier(Db.Get().Amounts.ScaleGrowth.deltaAttribute.Id, -0.75f, STRINGS.CREATURES.MODIFIERS.WILD.NAME, true, false, true));
		def.tameEffect = new Effect("Tame" + prefabTag.Name, STRINGS.CREATURES.MODIFIERS.TAME.NAME, STRINGS.CREATURES.MODIFIERS.TAME.TOOLTIP, 0f, true, true, false, null, 0f, null);
		def.tameEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -1f, STRINGS.CREATURES.MODIFIERS.TAME.NAME, false, false, true));
		def.tameEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, 100f, STRINGS.CREATURES.MODIFIERS.TAME.NAME, false, false, true));
		OvercrowdingMonitor.Def def2 = prefab.AddOrGetDef<OvercrowdingMonitor.Def>();
		def2.spaceRequiredPerCreature = space_required_per_creature;
		return prefab;
	}

	public static GameObject ExtendEntityToFertileCreature(GameObject prefab, string eggId, string eggName, string eggDesc, string egg_anim, float egg_mass, string baby_id, float fertility_cycles, float incubation_cycles, List<FertilityMonitor.BreedingChance> egg_chances, int eggSortOrder = -1, bool is_ranchable = true, bool add_fish_overcrowding_monitor = false, bool add_fixed_capturable_monitor = true, float egg_anim_scale = 1f)
	{
		FertilityMonitor.Def def = prefab.AddOrGetDef<FertilityMonitor.Def>();
		def.baseFertileCycles = fertility_cycles;
		DebugUtil.DevAssert(eggSortOrder > -1, "Added a fertile creature without an egg sort order!");
		float base_incubation_rate = 100f / (600f * incubation_cycles);
		GameObject gameObject = EggConfig.CreateEgg(eggId, eggName, eggDesc, baby_id, egg_anim, egg_mass, eggSortOrder, base_incubation_rate);
		def.eggPrefab = new Tag(eggId);
		def.initialBreedingWeights = egg_chances;
		if (egg_anim_scale != 1f)
		{
			KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
			component.animWidth = egg_anim_scale;
			component.animHeight = egg_anim_scale;
		}
		KPrefabID egg_prefab_id = gameObject.GetComponent<KPrefabID>();
		SymbolOverrideController symbol_override_controller = SymbolOverrideControllerUtil.AddToPrefab(gameObject);
		string symbolPrefix = prefab.GetComponent<CreatureBrain>().symbolPrefix;
		if (!string.IsNullOrEmpty(symbolPrefix))
		{
			symbol_override_controller.ApplySymbolOverridesByAffix(Assets.GetAnim(egg_anim), symbolPrefix, null, 0);
		}
		KPrefabID creature_prefab_id = prefab.GetComponent<KPrefabID>();
		creature_prefab_id.prefabSpawnFn += delegate
		{
			WorldInventory.Instance.Discover(eggId.ToTag(), WorldInventory.GetCategoryForTags(egg_prefab_id.Tags));
			WorldInventory.Instance.Discover(baby_id.ToTag(), WorldInventory.GetCategoryForTags(creature_prefab_id.Tags));
		};
		if (is_ranchable)
		{
			prefab.AddOrGetDef<RanchableMonitor.Def>();
		}
		if (add_fixed_capturable_monitor)
		{
			prefab.AddOrGetDef<FixedCapturableMonitor.Def>();
		}
		if (add_fish_overcrowding_monitor)
		{
			gameObject.AddOrGetDef<FishOvercrowdingMonitor.Def>();
		}
		return prefab;
	}

	public static GameObject ExtendEntityToBeingABaby(GameObject prefab, Tag adult_prefab_id, string on_grow_item_drop_id = null)
	{
		prefab.AddOrGetDef<BabyMonitor.Def>().adultPrefab = adult_prefab_id;
		prefab.AddOrGetDef<BabyMonitor.Def>().onGrowDropID = on_grow_item_drop_id;
		prefab.AddOrGetDef<IncubatorMonitor.Def>();
		prefab.AddOrGetDef<CreatureSleepMonitor.Def>();
		prefab.AddOrGetDef<CallAdultMonitor.Def>();
		prefab.AddOrGetDef<AgeMonitor.Def>().maxAgePercentOnSpawn = 0.01f;
		return prefab;
	}

	public static GameObject ExtendEntityToBasicCreature(GameObject template, FactionManager.FactionID faction = FactionManager.FactionID.Prey, string initialTraitID = null, string NavGridName = "WalkerNavGrid1x1", NavType navType = NavType.Floor, int max_probing_radius = 32, float moveSpeed = 2f, string onDeathDropID = "Meat", int onDeathDropCount = 1, bool drownVulnerable = true, bool entombVulnerable = true, float warningLowTemperature = 283.15f, float warningHighTemperature = 293.15f, float lethalLowTemperature = 243.15f, float lethalHighTemperature = 343.15f)
	{
		template.GetComponent<KBatchedAnimController>().isMovable = true;
		KPrefabID kPrefabID = template.AddOrGet<KPrefabID>();
		kPrefabID.AddTag(GameTags.Creature, false);
		Modifiers modifiers = template.AddOrGet<Modifiers>();
		if (initialTraitID != null)
		{
			modifiers.initialTraits = new string[1]
			{
				initialTraitID
			};
		}
		modifiers.initialAmounts.Add(Db.Get().Amounts.HitPoints.Id);
		template.AddOrGet<KBatchedAnimController>().SetSymbolVisiblity("snapto_pivot", false);
		template.AddOrGet<Pickupable>();
		template.AddOrGet<Clearable>().isClearable = false;
		template.AddOrGet<Traits>();
		template.AddOrGet<Health>();
		template.AddOrGet<CharacterOverlay>();
		template.AddOrGet<RangedAttackable>();
		template.AddOrGet<FactionAlignment>().Alignment = faction;
		template.AddOrGet<Prioritizable>();
		template.AddOrGet<Effects>();
		template.AddOrGetDef<CreatureDebugGoToMonitor.Def>();
		template.AddOrGetDef<DeathMonitor.Def>();
		template.AddOrGetDef<AnimInterruptMonitor.Def>();
		SymbolOverrideControllerUtil.AddToPrefab(template);
		template.AddOrGet<TemperatureVulnerable>().Configure(warningLowTemperature, lethalLowTemperature, warningHighTemperature, lethalHighTemperature);
		if (drownVulnerable)
		{
			template.AddOrGet<DrowningMonitor>();
		}
		if (entombVulnerable)
		{
			template.AddOrGet<EntombVulnerable>();
		}
		if (onDeathDropCount > 0 && onDeathDropID != "")
		{
			string[] array = new string[onDeathDropCount];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = onDeathDropID;
			}
			template.AddOrGet<Butcherable>().SetDrops(array);
		}
		Navigator navigator = template.AddOrGet<Navigator>();
		navigator.NavGridName = NavGridName;
		navigator.CurrentNavType = navType;
		navigator.defaultSpeed = moveSpeed;
		navigator.updateProber = true;
		navigator.maxProbingRadius = max_probing_radius;
		navigator.sceneLayer = Grid.SceneLayer.Creatures;
		return template;
	}

	public static void AddCreatureBrain(GameObject prefab, ChoreTable.Builder chore_table, Tag species, string symbol_prefix)
	{
		CreatureBrain creatureBrain = prefab.AddOrGet<CreatureBrain>();
		creatureBrain.species = species;
		creatureBrain.symbolPrefix = symbol_prefix;
		ChoreConsumer chore_consumer = prefab.AddOrGet<ChoreConsumer>();
		chore_consumer.choreTable = chore_table.CreateTable();
		KPrefabID kPrefabID = prefab.AddOrGet<KPrefabID>();
		kPrefabID.AddTag(GameTags.CreatureBrain, false);
		kPrefabID.instantiateFn += delegate(GameObject go)
		{
			go.GetComponent<ChoreConsumer>().choreTable = chore_consumer.choreTable;
		};
	}

	public static Tag GetBaggedCreatureTag(Tag tag)
	{
		return TagManager.Create("Bagged" + tag.Name);
	}

	public static Tag GetUnbaggedCreatureTag(Tag bagged_tag)
	{
		return TagManager.Create(bagged_tag.Name.Substring(6));
	}

	public static string GetBaggedCreatureID(string name)
	{
		return "Bagged" + name;
	}

	public static GameObject CreateAndRegisterBaggedCreature(GameObject creature, bool must_stand_on_top_for_pickup, bool allow_mark_for_capture, bool use_gun_for_pickup = false)
	{
		KPrefabID creature_prefab_id = creature.GetComponent<KPrefabID>();
		creature_prefab_id.AddTag(GameTags.BagableCreature, false);
		Baggable baggable = creature.AddOrGet<Baggable>();
		baggable.mustStandOntopOfTrapForPickup = must_stand_on_top_for_pickup;
		baggable.useGunForPickup = use_gun_for_pickup;
		Capturable capturable = creature.AddOrGet<Capturable>();
		capturable.allowCapture = allow_mark_for_capture;
		creature_prefab_id.prefabSpawnFn += delegate
		{
			WorldInventory.Instance.Discover(creature_prefab_id.PrefabTag, WorldInventory.GetCategoryForTags(creature_prefab_id.Tags));
		};
		return creature;
	}

	public static GameObject CreateLooseEntity(string id, string name, string desc, float mass, bool unitMass, KAnimFile anim, string initialAnim, Grid.SceneLayer sceneLayer, CollisionShape collisionShape, float width = 1f, float height = 1f, bool isPickupable = false, int sortOrder = 0, SimHashes element = SimHashes.Creature, List<Tag> additionalTags = null)
	{
		GameObject template = CreateBasicEntity(id, name, desc, mass, unitMass, anim, initialAnim, sceneLayer, element, additionalTags, 293f);
		template = AddCollision(template, collisionShape, width, height);
		KBatchedAnimController component = template.GetComponent<KBatchedAnimController>();
		component.isMovable = true;
		template.AddOrGet<Modifiers>();
		if (isPickupable)
		{
			Pickupable pickupable = template.AddOrGet<Pickupable>();
			pickupable.SetWorkTime(5f);
			pickupable.sortOrder = sortOrder;
		}
		return template;
	}

	public static void CreateBaseOreTemplates()
	{
		baseOreTemplate = new GameObject("OreTemplate");
		Object.DontDestroyOnLoad(baseOreTemplate);
		baseOreTemplate.SetActive(false);
		baseOreTemplate.AddComponent<KPrefabID>();
		baseOreTemplate.AddComponent<PrimaryElement>();
		baseOreTemplate.AddComponent<Pickupable>();
		baseOreTemplate.AddComponent<KSelectable>();
		baseOreTemplate.AddComponent<SaveLoadRoot>();
		baseOreTemplate.AddComponent<StateMachineController>();
		baseOreTemplate.AddComponent<Clearable>();
		baseOreTemplate.AddComponent<Prioritizable>();
		baseOreTemplate.AddComponent<KBatchedAnimController>();
		baseOreTemplate.AddComponent<SimTemperatureTransfer>();
		baseOreTemplate.AddComponent<Modifiers>();
		OccupyArea occupyArea = baseOreTemplate.AddOrGet<OccupyArea>();
		occupyArea.OccupiedCellsOffsets = new CellOffset[1]
		{
			default(CellOffset)
		};
		DecorProvider decorProvider = baseOreTemplate.AddOrGet<DecorProvider>();
		decorProvider.baseDecor = -10f;
		decorProvider.baseRadius = 1f;
		baseOreTemplate.AddOrGet<ElementChunk>();
	}

	public static void DestroyBaseOreTemplates()
	{
		Object.Destroy(baseOreTemplate);
		baseOreTemplate = null;
	}

	public static GameObject CreateOreEntity(SimHashes elementID, CollisionShape shape, float width, float height, List<Tag> additionalTags = null, float default_temperature = 293f)
	{
		Element element = ElementLoader.FindElementByHash(elementID);
		GameObject gameObject = Object.Instantiate(baseOreTemplate);
		gameObject.name = element.name;
		Object.DontDestroyOnLoad(gameObject);
		KPrefabID kPrefabID = gameObject.AddOrGet<KPrefabID>();
		kPrefabID.PrefabTag = element.tag;
		if (additionalTags != null)
		{
			foreach (Tag additionalTag in additionalTags)
			{
				kPrefabID.AddTag(additionalTag, false);
			}
		}
		if (element.lowTemp < 296.15f && element.highTemp > 296.15f)
		{
			kPrefabID.AddTag(GameTags.PedestalDisplayable, false);
		}
		PrimaryElement primaryElement = gameObject.AddOrGet<PrimaryElement>();
		primaryElement.SetElement(elementID);
		primaryElement.Mass = 1f;
		primaryElement.Temperature = default_temperature;
		Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
		pickupable.SetWorkTime(5f);
		pickupable.sortOrder = element.buildMenuSort;
		KSelectable kSelectable = gameObject.AddOrGet<KSelectable>();
		kSelectable.SetName(element.name);
		KBatchedAnimController kBatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
		kBatchedAnimController.AnimFiles = new KAnimFile[1]
		{
			element.substance.anim
		};
		kBatchedAnimController.sceneLayer = Grid.SceneLayer.Front;
		kBatchedAnimController.initialAnim = "idle1";
		kBatchedAnimController.isMovable = true;
		return AddCollision(gameObject, shape, width, height);
	}

	public static GameObject CreateSolidOreEntity(SimHashes elementId, List<Tag> additionalTags = null)
	{
		return CreateOreEntity(elementId, CollisionShape.CIRCLE, 0.5f, 0.5f, additionalTags, 293f);
	}

	public static GameObject CreateLiquidOreEntity(SimHashes elementId, List<Tag> additionalTags = null)
	{
		GameObject gameObject = CreateOreEntity(elementId, CollisionShape.RECTANGLE, 0.5f, 0.6f, additionalTags, 293f);
		Dumpable dumpable = gameObject.AddOrGet<Dumpable>();
		dumpable.SetWorkTime(5f);
		gameObject.AddOrGet<SubstanceChunk>();
		return gameObject;
	}

	public static GameObject CreateGasOreEntity(SimHashes elementId, List<Tag> additionalTags = null)
	{
		GameObject gameObject = CreateOreEntity(elementId, CollisionShape.RECTANGLE, 0.5f, 0.6f, additionalTags, 293f);
		Dumpable dumpable = gameObject.AddOrGet<Dumpable>();
		dumpable.SetWorkTime(5f);
		gameObject.AddOrGet<SubstanceChunk>();
		return gameObject;
	}

	public static GameObject ExtendEntityToFood(GameObject template, EdiblesManager.FoodInfo foodInfo)
	{
		template.AddOrGet<EntitySplitter>();
		if (foodInfo.CanRot)
		{
			Rottable.Def def = template.AddOrGetDef<Rottable.Def>();
			def.rotTemperature = foodInfo.RotTemperature;
			def.spoilTime = foodInfo.SpoilTime;
			def.staleTime = foodInfo.StaleTime;
			CreateAndRegisterCompostableFromPrefab(template);
		}
		KPrefabID component = template.GetComponent<KPrefabID>();
		component.AddTag(GameTags.PedestalDisplayable, false);
		if (foodInfo.CaloriesPerUnit > 0f)
		{
			Edible edible = template.AddOrGet<Edible>();
			edible.FoodInfo = foodInfo;
			component.instantiateFn += delegate(GameObject go)
			{
				go.GetComponent<Edible>().FoodInfo = foodInfo;
			};
			GameTags.DisplayAsCalories.Add(component.PrefabTag);
		}
		else
		{
			component.AddTag(GameTags.CookingIngredient, false);
			template.AddOrGet<HasSortOrder>();
		}
		return template;
	}

	public static GameObject ExtendEntityToMedicine(GameObject template, MedicineInfo medicineInfo)
	{
		template.AddOrGet<EntitySplitter>();
		KPrefabID component = template.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Medicine, false);
		MedicinalPill medicinalPill = template.AddOrGet<MedicinalPill>();
		medicinalPill.info = medicineInfo;
		return template;
	}

	public static GameObject ExtendPlantToFertilizable(GameObject template, PlantElementAbsorber.ConsumeInfo[] fertilizers)
	{
		HashedString idHash = Db.Get().ChoreTypes.FarmFetch.IdHash;
		for (int i = 0; i < fertilizers.Length; i++)
		{
			PlantElementAbsorber.ConsumeInfo consumeInfo = fertilizers[i];
			ManualDeliveryKG manualDeliveryKG = template.AddComponent<ManualDeliveryKG>();
			manualDeliveryKG.RequestedItemTag = consumeInfo.tag;
			manualDeliveryKG.capacity = consumeInfo.massConsumptionRate * 600f * 3f;
			manualDeliveryKG.refillMass = consumeInfo.massConsumptionRate * 600f * 0.5f;
			manualDeliveryKG.minimumMass = consumeInfo.massConsumptionRate * 600f * 0.5f;
			manualDeliveryKG.operationalRequirement = FetchOrder2.OperationalRequirement.Functional;
			manualDeliveryKG.choreTypeIDHash = idHash;
		}
		KPrefabID component = template.GetComponent<KPrefabID>();
		FertilizationMonitor.Def def = template.AddOrGetDef<FertilizationMonitor.Def>();
		def.wrongFertilizerTestTag = GameTags.Solid;
		def.consumedElements = fertilizers;
		component.prefabInitFn += delegate(GameObject inst)
		{
			ManualDeliveryKG[] components = inst.GetComponents<ManualDeliveryKG>();
			foreach (ManualDeliveryKG manualDeliveryKG2 in components)
			{
				manualDeliveryKG2.Pause(true, "init");
			}
		};
		return template;
	}

	public static GameObject ExtendPlantToIrrigated(GameObject template, PlantElementAbsorber.ConsumeInfo info)
	{
		return ExtendPlantToIrrigated(template, new PlantElementAbsorber.ConsumeInfo[1]
		{
			info
		});
	}

	public static GameObject ExtendPlantToIrrigated(GameObject template, PlantElementAbsorber.ConsumeInfo[] consume_info)
	{
		HashedString idHash = Db.Get().ChoreTypes.FarmFetch.IdHash;
		for (int i = 0; i < consume_info.Length; i++)
		{
			PlantElementAbsorber.ConsumeInfo consumeInfo = consume_info[i];
			ManualDeliveryKG manualDeliveryKG = template.AddComponent<ManualDeliveryKG>();
			manualDeliveryKG.RequestedItemTag = consumeInfo.tag;
			manualDeliveryKG.capacity = consumeInfo.massConsumptionRate * 600f * 3f;
			manualDeliveryKG.refillMass = consumeInfo.massConsumptionRate * 600f * 0.5f;
			manualDeliveryKG.minimumMass = consumeInfo.massConsumptionRate * 600f * 0.5f;
			manualDeliveryKG.operationalRequirement = FetchOrder2.OperationalRequirement.Functional;
			manualDeliveryKG.choreTypeIDHash = idHash;
		}
		IrrigationMonitor.Def def = template.AddOrGetDef<IrrigationMonitor.Def>();
		def.wrongIrrigationTestTag = GameTags.Liquid;
		def.consumedElements = consume_info;
		return template;
	}

	public static GameObject CreateAndRegisterCompostableFromPrefab(GameObject original)
	{
		if (!((Object)original.GetComponent<Compostable>() != (Object)null))
		{
			Compostable compostable = original.AddComponent<Compostable>();
			compostable.isMarkedForCompost = false;
			KPrefabID component = original.GetComponent<KPrefabID>();
			GameObject gameObject = Object.Instantiate(original);
			Object.DontDestroyOnLoad(gameObject);
			string tag_string = "Compost" + component.PrefabTag.Name;
			string text = MISC.TAGS.COMPOST_FORMAT.Replace("{Item}", component.PrefabTag.ProperName());
			gameObject.GetComponent<KPrefabID>().PrefabTag = TagManager.Create(tag_string, text);
			gameObject.name = text;
			gameObject.GetComponent<Compostable>().isMarkedForCompost = true;
			gameObject.GetComponent<KSelectable>().SetName(text);
			gameObject.GetComponent<Compostable>().originalPrefab = original;
			gameObject.GetComponent<Compostable>().compostPrefab = gameObject;
			original.GetComponent<Compostable>().originalPrefab = original;
			original.GetComponent<Compostable>().compostPrefab = gameObject;
			Assets.AddPrefab(gameObject.GetComponent<KPrefabID>());
			return gameObject;
		}
		return null;
	}

	public static GameObject CreateAndRegisterSeedForPlant(GameObject plant, SeedProducer.ProductionType productionType, string id, string name, string desc, KAnimFile anim, string initialAnim = "object", int numberOfSeeds = 1, List<Tag> additionalTags = null, SingleEntityReceptacle.ReceptacleDirection planterDirection = SingleEntityReceptacle.ReceptacleDirection.Top, Tag replantGroundTag = default(Tag), int sortOrder = 0, string domesticatedDescription = "", CollisionShape collisionShape = CollisionShape.CIRCLE, float width = 0.25f, float height = 0.25f, Recipe.Ingredient[] recipe_ingredients = null, string recipe_description = "", bool ignoreDefaultSeedTag = false)
	{
		GameObject gameObject = CreateLooseEntity(id, name, desc, 1f, true, anim, initialAnim, Grid.SceneLayer.Front, collisionShape, width, height, true, SORTORDER.SEEDS + sortOrder, SimHashes.Creature, null);
		gameObject.AddOrGet<EntitySplitter>();
		CreateAndRegisterCompostableFromPrefab(gameObject);
		PlantableSeed plantableSeed = gameObject.AddOrGet<PlantableSeed>();
		plantableSeed.PlantID = new Tag(plant.name);
		plantableSeed.replantGroundTag = replantGroundTag;
		plantableSeed.domesticatedDescription = domesticatedDescription;
		plantableSeed.direction = planterDirection;
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		foreach (Tag additionalTag in additionalTags)
		{
			component.AddTag(additionalTag, false);
		}
		if (!ignoreDefaultSeedTag)
		{
			component.AddTag(GameTags.Seed, false);
		}
		component.AddTag(GameTags.PedestalDisplayable, false);
		KPrefabID component2 = gameObject.GetComponent<KPrefabID>();
		Assets.AddPrefab(component2);
		SeedProducer seedProducer = plant.AddOrGet<SeedProducer>();
		seedProducer.Configure(gameObject.name, productionType, numberOfSeeds);
		return gameObject;
	}

	public static GameObject CreateAndRegisterPreview(string id, KAnimFile anim, string initial_anim, ObjectLayer object_layer, int width, int height)
	{
		GameObject gameObject = CreatePlacedEntity(id, id, id, 1f, anim, initial_anim, Grid.SceneLayer.Front, width, height, TUNING.BUILDINGS.DECOR.NONE, default(EffectorValues), SimHashes.Creature, null, 293f);
		gameObject.UpdateComponentRequirement<KSelectable>(false);
		gameObject.UpdateComponentRequirement<SaveLoadRoot>(false);
		EntityPreview entityPreview = gameObject.AddOrGet<EntityPreview>();
		entityPreview.objectLayer = object_layer;
		OccupyArea occupyArea = gameObject.AddOrGet<OccupyArea>();
		occupyArea.objectLayers = new ObjectLayer[1]
		{
			object_layer
		};
		occupyArea.ApplyToCells = false;
		gameObject.AddOrGet<Storage>();
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		Assets.AddPrefab(component);
		return gameObject;
	}

	public static GameObject CreateAndRegisterPreviewForPlant(GameObject seed, string id, KAnimFile anim, string initialAnim, int width, int height)
	{
		GameObject result = CreateAndRegisterPreview(id, anim, initialAnim, ObjectLayer.Building, width, height);
		PlantableSeed component = seed.GetComponent<PlantableSeed>();
		component.PreviewID = TagManager.Create(id);
		return result;
	}

	public static CellOffset[] GenerateOffsets(int width, int height)
	{
		int num = width / 2;
		int num2 = num;
		int startX = num2 - width + 1;
		int startY = 0;
		int endY = height - 1;
		return GenerateOffsets(startX, startY, num2, endY);
	}

	private static CellOffset[] GenerateOffsets(int startX, int startY, int endX, int endY)
	{
		List<CellOffset> list = new List<CellOffset>();
		for (int i = startY; i <= endY; i++)
		{
			for (int j = startX; j <= endX; j++)
			{
				list.Add(new CellOffset
				{
					x = j,
					y = i
				});
			}
		}
		return list.ToArray();
	}

	public static CellOffset[] GenerateHangingOffsets(int width, int height)
	{
		int num = width / 2;
		int num2 = num;
		int startX = num2 - width + 1;
		int startY = -height + 1;
		int endY = 0;
		return GenerateOffsets(startX, startY, num2, endY);
	}

	public static GameObject AddCollision(GameObject template, CollisionShape shape, float width, float height)
	{
		switch (shape)
		{
		case CollisionShape.RECTANGLE:
		{
			KBoxCollider2D kBoxCollider2D = template.AddOrGet<KBoxCollider2D>();
			kBoxCollider2D.size = new Vector2f(width, height);
			break;
		}
		case CollisionShape.POLYGONAL:
			template.AddOrGet<PolygonCollider2D>();
			break;
		default:
		{
			KCircleCollider2D kCircleCollider2D = template.AddOrGet<KCircleCollider2D>();
			kCircleCollider2D.radius = Mathf.Max(width, height);
			break;
		}
		}
		return template;
	}
}
