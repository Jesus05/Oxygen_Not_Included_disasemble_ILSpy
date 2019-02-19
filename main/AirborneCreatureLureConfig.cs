using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class AirborneCreatureLureConfig : IBuildingConfig
{
	public const string ID = "AirborneCreatureLure";

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[1]
	{
		LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false)
	};

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("AirborneCreatureLure", 1, 4, "airbornecreaturetrap_kanim", 10, 10f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.PLASTICS, 1600f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject prefab, Tag prefab_tag)
	{
		CreatureLure creatureLure = prefab.AddOrGet<CreatureLure>();
		creatureLure.baitStorage = prefab.AddOrGet<Storage>();
		creatureLure.baitTypes = new List<Tag>
		{
			GameTags.SlimeMold,
			GameTags.Phosphorite
		};
		creatureLure.baitStorage.storageFilters = creatureLure.baitTypes;
		creatureLure.baitStorage.allowItemRemoval = false;
		creatureLure.baitStorage.SetDefaultStoredItemModifiers(Storage.StandardFabricatorStorage);
		prefab.AddOrGet<Operational>();
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject prefab)
	{
		BuildingTemplates.DoPostConfigure(prefab);
		SymbolOverrideControllerUtil.AddToPrefab(prefab);
		GeneratedBuildings.RegisterLogicPorts(prefab, INPUT_PORTS);
		prefab.AddOrGet<LogicOperationalController>();
		Lure.Def def = prefab.AddOrGetDef<Lure.Def>();
		def.lurePoints = new CellOffset[13]
		{
			new CellOffset(0, 0),
			new CellOffset(-1, 4),
			new CellOffset(0, 4),
			new CellOffset(1, 4),
			new CellOffset(-2, 3),
			new CellOffset(-1, 3),
			new CellOffset(0, 3),
			new CellOffset(1, 3),
			new CellOffset(2, 3),
			new CellOffset(-1, 2),
			new CellOffset(0, 2),
			new CellOffset(1, 2),
			new CellOffset(0, 1)
		};
		def.radius = 32;
		Prioritizable.AddRef(prefab);
	}
}
