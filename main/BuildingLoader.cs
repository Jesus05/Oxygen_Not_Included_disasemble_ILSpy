using UnityEngine;

public class BuildingLoader : KMonoBehaviour
{
	private GameObject previewTemplate;

	private GameObject constructionTemplate;

	public static BuildingLoader Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		previewTemplate = CreatePreviewTemplate();
		constructionTemplate = CreateConstructionTemplate();
	}

	private GameObject CreateTemplate()
	{
		GameObject gameObject = new GameObject();
		gameObject.SetActive(false);
		gameObject.AddOrGet<KPrefabID>();
		gameObject.AddOrGet<KSelectable>();
		gameObject.AddOrGet<StateMachineController>();
		PrimaryElement primaryElement = gameObject.AddOrGet<PrimaryElement>();
		primaryElement.Mass = 1f;
		primaryElement.Temperature = 293f;
		return gameObject;
	}

	private GameObject CreatePreviewTemplate()
	{
		GameObject gameObject = CreateTemplate();
		gameObject.AddComponent<BuildingPreview>();
		return gameObject;
	}

	private GameObject CreateConstructionTemplate()
	{
		GameObject gameObject = CreateTemplate();
		gameObject.AddOrGet<BuildingUnderConstruction>();
		gameObject.AddOrGet<Constructable>();
		Storage storage = gameObject.AddComponent<Storage>();
		storage.doDiseaseTransfer = false;
		gameObject.AddOrGet<Cancellable>();
		gameObject.AddOrGet<Prioritizable>();
		gameObject.AddOrGet<Notifier>();
		gameObject.AddOrGet<SaveLoadRoot>();
		return gameObject;
	}

	public GameObject CreateBuilding(BuildingDef def, GameObject go, GameObject parent = null)
	{
		go = Object.Instantiate(go);
		go.name = def.PrefabID;
		if ((Object)parent != (Object)null)
		{
			go.transform.parent = parent.transform;
		}
		go.GetComponent<Building>().Def = def;
		return go;
	}

	private static bool Add2DComponents(BuildingDef def, GameObject go, string initialAnimState = null, bool no_collider = false, int layer = -1)
	{
		bool flag = def.AnimFiles != null && def.AnimFiles.Length > 0;
		if (layer == -1)
		{
			layer = LayerMask.NameToLayer("Default");
		}
		go.layer = layer;
		KBatchedAnimController[] components = go.GetComponents<KBatchedAnimController>();
		if (components.Length > 1)
		{
			for (int i = 2; i < components.Length; i++)
			{
				Object.DestroyImmediate(components[i]);
			}
		}
		if ((Object)def.BlockTileAtlas == (Object)null)
		{
			KBatchedAnimController kBatchedAnimController = UpdateComponentRequirement<KBatchedAnimController>(go, flag);
			if ((Object)kBatchedAnimController != (Object)null)
			{
				kBatchedAnimController.AnimFiles = def.AnimFiles;
				if (def.isKAnimTile)
				{
					kBatchedAnimController.initialAnim = null;
				}
				else
				{
					if (def.isUtility && initialAnimState == null)
					{
						initialAnimState = "idle";
					}
					else if ((Object)go.GetComponent<Door>() != (Object)null)
					{
						initialAnimState = "closed";
					}
					kBatchedAnimController.initialAnim = ((initialAnimState == null) ? def.DefaultAnimState : initialAnimState);
				}
				kBatchedAnimController.SetFGLayer(def.ForegroundLayer);
				kBatchedAnimController.materialType = KAnimBatchGroup.MaterialType.Default;
			}
		}
		KBoxCollider2D kBoxCollider2D = UpdateComponentRequirement<KBoxCollider2D>(go, flag && !no_collider);
		if ((Object)kBoxCollider2D != (Object)null)
		{
			kBoxCollider2D.offset = new Vector3(0f, 0.5f * (float)def.HeightInCells, 0f);
			kBoxCollider2D.size = new Vector3((float)def.WidthInCells, (float)def.HeightInCells, 0f);
		}
		if (def.AnimFiles == null)
		{
			Debug.LogError(def.Name + " Def missing anim files", null);
		}
		return flag;
	}

	private static T UpdateComponentRequirement<T>(GameObject go, bool required) where T : Component
	{
		T val = go.GetComponent(typeof(T)) as T;
		if (!required && (Object)val != (Object)null)
		{
			Object.DestroyImmediate(val, true);
			val = (T)null;
		}
		else if (required && (Object)val == (Object)null)
		{
			val = (go.AddComponent(typeof(T)) as T);
		}
		return val;
	}

	public static KPrefabID AddID(GameObject go, string str)
	{
		KPrefabID kPrefabID = go.GetComponent<KPrefabID>();
		if ((Object)kPrefabID == (Object)null)
		{
			kPrefabID = go.AddComponent<KPrefabID>();
		}
		kPrefabID.PrefabTag = new Tag(str);
		kPrefabID.SaveLoadTag = kPrefabID.PrefabTag;
		return kPrefabID;
	}

	public GameObject CreateBuildingUnderConstruction(BuildingDef def)
	{
		GameObject gameObject = CreateBuilding(def, constructionTemplate, null);
		Object.DontDestroyOnLoad(gameObject);
		KSelectable component = gameObject.GetComponent<KSelectable>();
		component.SetName(def.Name);
		for (int i = 0; i < def.Mass.Length; i++)
		{
			gameObject.GetComponent<PrimaryElement>().MassPerUnit += def.Mass[i];
		}
		KPrefabID kPrefabID = AddID(gameObject, def.PrefabID + "UnderConstruction");
		UpdateComponentRequirement<BuildingCellVisualizer>(gameObject, BuildingCellVisualizer.CheckRequiresComponent(def));
		Constructable component2 = gameObject.GetComponent<Constructable>();
		component2.SetWorkTime(def.ConstructionTime);
		Rotatable rotatable = UpdateComponentRequirement<Rotatable>(gameObject, def.PermittedRotations != PermittedRotations.Unrotatable);
		if ((bool)rotatable)
		{
			rotatable.permittedRotations = def.PermittedRotations;
		}
		Add2DComponents(def, gameObject, "place", false, kPrefabID.defaultLayer = LayerMask.NameToLayer("Construction"));
		UpdateComponentRequirement<Vent>(gameObject, false);
		bool required = (Object)def.BuildingComplete.GetComponent<AnimTileable>() != (Object)null;
		UpdateComponentRequirement<AnimTileable>(gameObject, required);
		if (def.RequiresPowerInput)
		{
			GeneratedBuildings.RegisterLogicPorts(gameObject, LogicOperationalController.INPUT_PORTS);
		}
		Assets.AddPrefab(kPrefabID);
		gameObject.PreInit();
		return gameObject;
	}

	public GameObject CreateBuildingComplete(GameObject go, BuildingDef def)
	{
		go.name = def.PrefabID + "Complete";
		go.transform.SetPosition(new Vector3(0f, 0f, Grid.GetLayerZ(def.SceneLayer)));
		KSelectable component = go.GetComponent<KSelectable>();
		component.SetName(def.Name);
		PrimaryElement component2 = go.GetComponent<PrimaryElement>();
		component2.MassPerUnit = 0f;
		for (int i = 0; i < def.Mass.Length; i++)
		{
			component2.MassPerUnit += def.Mass[i];
		}
		component2.Temperature = 273.15f;
		BuildingHP buildingHP = go.AddOrGet<BuildingHP>();
		if (def.Invincible)
		{
			buildingHP.invincible = true;
		}
		buildingHP.SetHitPoints(def.HitPoints);
		if (def.Repairable)
		{
			UpdateComponentRequirement<Repairable>(go, true);
		}
		int defaultLayer = go.layer = LayerMask.NameToLayer("Default");
		Building component3 = go.GetComponent<BuildingComplete>();
		component3.Def = def;
		if (def.InputConduitType != 0 || def.OutputConduitType != 0)
		{
			go.AddComponent<BuildingConduitEndpoints>();
		}
		if (!Add2DComponents(def, go, null, false, -1))
		{
			Debug.Log(def.Name + " is not yet a 2d building!", null);
		}
		UpdateComponentRequirement<EnergyConsumer>(go, def.RequiresPowerInput);
		Rotatable rotatable = UpdateComponentRequirement<Rotatable>(go, def.PermittedRotations != PermittedRotations.Unrotatable);
		if ((bool)rotatable)
		{
			rotatable.permittedRotations = def.PermittedRotations;
		}
		if (def.Breakable)
		{
			go.AddComponent<Breakable>();
		}
		ConduitConsumer conduitConsumer = UpdateComponentRequirement<ConduitConsumer>(go, def.InputConduitType == ConduitType.Gas || def.InputConduitType == ConduitType.Liquid);
		if ((Object)conduitConsumer != (Object)null)
		{
			conduitConsumer.SetConduitData(def.InputConduitType);
		}
		bool required = def.RequiresPowerInput || def.InputConduitType == ConduitType.Gas || def.InputConduitType == ConduitType.Liquid;
		RequireInputs requireInputs = UpdateComponentRequirement<RequireInputs>(go, required);
		if ((Object)requireInputs != (Object)null)
		{
			requireInputs.SetRequirements(def.RequiresPowerInput, def.InputConduitType == ConduitType.Gas || def.InputConduitType == ConduitType.Liquid);
		}
		UpdateComponentRequirement<RequireOutputs>(go, def.OutputConduitType != ConduitType.None);
		UpdateComponentRequirement<Operational>(go, !def.isUtility);
		if (def.Floodable)
		{
			go.AddComponent<Floodable>();
		}
		if (def.Disinfectable)
		{
			go.AddOrGet<AutoDisinfectable>();
			go.AddOrGet<Disinfectable>();
		}
		if (def.Overheatable)
		{
			Overheatable overheatable = go.AddComponent<Overheatable>();
			overheatable.baseOverheatTemp = def.OverheatTemperature;
			overheatable.baseFatalTemp = def.FatalHot;
		}
		if (def.Entombable)
		{
			go.AddComponent<Structure>();
		}
		if (def.RequiresPowerInput)
		{
			GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS);
			go.AddOrGet<LogicOperationalController>();
		}
		UpdateComponentRequirement<BuildingCellVisualizer>(go, BuildingCellVisualizer.CheckRequiresComponent(def));
		if (def.BaseDecor != 0f)
		{
			DecorProvider decorProvider = UpdateComponentRequirement<DecorProvider>(go, true);
			decorProvider.baseDecor = def.BaseDecor;
			decorProvider.baseRadius = def.BaseDecorRadius;
		}
		if (def.AttachmentSlotTag != Tag.Invalid)
		{
			AttachableBuilding attachableBuilding = UpdateComponentRequirement<AttachableBuilding>(go, true);
			attachableBuilding.attachableToTag = def.AttachmentSlotTag;
		}
		KPrefabID kPrefabID = AddID(go, def.PrefabID);
		kPrefabID.defaultLayer = defaultLayer;
		Assets.AddPrefab(kPrefabID);
		go.PreInit();
		return go;
	}

	public GameObject CreateBuildingPreview(BuildingDef def)
	{
		GameObject gameObject = CreateBuilding(def, previewTemplate, null);
		Object.DontDestroyOnLoad(gameObject);
		int num = LayerMask.NameToLayer("Place");
		gameObject.transform.SetPosition(new Vector3(0f, 0f, Grid.GetLayerZ(def.SceneLayer)));
		Add2DComponents(def, gameObject, "place", true, num);
		KAnimControllerBase component = gameObject.GetComponent<KAnimControllerBase>();
		if ((Object)component != (Object)null)
		{
			component.fgLayer = Grid.SceneLayer.NoLayer;
		}
		Rotatable rotatable = UpdateComponentRequirement<Rotatable>(gameObject, def.PermittedRotations != PermittedRotations.Unrotatable);
		if ((bool)rotatable)
		{
			rotatable.permittedRotations = def.PermittedRotations;
		}
		KPrefabID kPrefabID = AddID(gameObject, def.PrefabID + "Preview");
		kPrefabID.defaultLayer = num;
		KSelectable component2 = gameObject.GetComponent<KSelectable>();
		component2.SetName(def.Name);
		UpdateComponentRequirement<BuildingCellVisualizer>(gameObject, BuildingCellVisualizer.CheckRequiresComponent(def));
		KAnimGraphTileVisualizer component3 = gameObject.GetComponent<KAnimGraphTileVisualizer>();
		if ((Object)component3 != (Object)null)
		{
			Object.DestroyImmediate(component3);
		}
		if (def.RequiresPowerInput)
		{
			GeneratedBuildings.RegisterLogicPorts(gameObject, LogicOperationalController.INPUT_PORTS);
		}
		gameObject.PreInit();
		Assets.AddPrefab(gameObject.GetComponent<KPrefabID>());
		return gameObject;
	}
}
