using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class MonumentBottomConfig : IBuildingConfig
{
	public const string ID = "MonumentBottom";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "MonumentBottom";
		int width = 5;
		int height = 5;
		string anim = "victory_monument_base_kanim";
		int hitpoints = 1000;
		float construction_time = 60f;
		float[] construction_mass = new float[2]
		{
			7500f,
			2500f
		};
		string[] construction_materials = new string[2]
		{
			(-899253461).ToString(),
			(-474151749).ToString()
		};
		float melting_point = 9999f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues tIER = NOISE_POLLUTION.NOISY.TIER2;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, construction_mass, construction_materials, melting_point, build_location_rule, BUILDINGS.DECOR.BONUS.MONUMENT.INCOMPLETE, tIER, 0.2f);
		BuildingTemplates.CreateMonumentBuildingDef(buildingDef);
		buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.Floodable = false;
		buildingDef.AttachmentSlotTag = "MonumentBottom";
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.attachablePosition = new CellOffset(0, 0);
		buildingDef.RequiresPowerInput = false;
		buildingDef.CanMove = false;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LoopingSounds>();
		BuildingAttachPoint buildingAttachPoint = go.AddOrGet<BuildingAttachPoint>();
		buildingAttachPoint.points = new BuildingAttachPoint.HardPoint[1]
		{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), "MonumentMiddle", null)
		};
		MonumentPart monumentPart = go.AddOrGet<MonumentPart>();
		monumentPart.part = MonumentPart.Part.Bottom;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<KBatchedAnimController>().initialAnim = "option_a";
		go.GetComponent<KPrefabID>().prefabSpawnFn += delegate(GameObject game_object)
		{
			MonumentPart monumentPart = game_object.AddOrGet<MonumentPart>();
			monumentPart.part = MonumentPart.Part.Bottom;
			monumentPart.selectableStatesAndSymbols = new List<Tuple<string, string>>();
			monumentPart.stateUISymbol = "base";
			monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_a", "straight_legs"));
			monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_b", "wide_stance"));
			monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_c", "hmmm_legs"));
			monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_d", "sitting_stool"));
			monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_e", "wide_stance2"));
			monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_f", "posing1"));
			monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_g", "knee_kick"));
			monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_h", "step_on_hatches"));
			monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_i", "sit_on_tools"));
			monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_j", "water_pacu"));
			monumentPart.selectableStatesAndSymbols.Add(new Tuple<string, string>("option_k", "sit_on_eggs"));
		};
	}
}
