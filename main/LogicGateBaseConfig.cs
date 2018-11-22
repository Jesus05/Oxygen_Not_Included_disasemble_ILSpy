using TUNING;
using UnityEngine;

public abstract class LogicGateBaseConfig : IBuildingConfig
{
	protected BuildingDef CreateBuildingDef(string ID, string anim, int width = 2, int height = 2)
	{
		int hitpoints = 10;
		float construction_time = 3f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
		string[] rEFINED_METALS = MATERIALS.REFINED_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, width, height, anim, hitpoints, construction_time, tIER, rEFINED_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER0, nONE, 0.2f);
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.ObjectLayer = ObjectLayer.LogicGates;
		buildingDef.SceneLayer = Grid.SceneLayer.LogicWireBridges;
		buildingDef.ThermalConductivity = 0.05f;
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.Entombable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.DragBuild = true;
		LogicGateBase.uiSrcData = Assets.instance.logicModeUIData;
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
		return buildingDef;
	}

	protected abstract LogicGateBase.Op GetLogicOp();

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
		MoveableLogicGateVisualizer moveableLogicGateVisualizer = go.AddComponent<MoveableLogicGateVisualizer>();
		moveableLogicGateVisualizer.op = GetLogicOp();
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		LogicGateVisualizer logicGateVisualizer = go.AddComponent<LogicGateVisualizer>();
		logicGateVisualizer.op = GetLogicOp();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		LogicGate logicGate = go.AddComponent<LogicGate>();
		logicGate.op = GetLogicOp();
	}
}
