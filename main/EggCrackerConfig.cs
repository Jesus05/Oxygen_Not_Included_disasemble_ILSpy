using TUNING;
using UnityEngine;

[EntityConfigOrder(2)]
public class EggCrackerConfig : IBuildingConfig
{
	public const string ID = "EggCracker";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "EggCracker";
		int width = 2;
		int height = 2;
		string anim = "egg_cracker_kanim";
		int hitpoints = 30;
		float construction_time = 10f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
		string[] rAW_METALS = MATERIALS.RAW_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, rAW_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.BONUS.TIER0, nONE, 0.2f);
		buildingDef.AudioCategory = "Metal";
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingFront;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGet<KBatchedAnimController>().SetSymbolVisiblity("snapto_egg", false);
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		complexFabricator.labelByResult = false;
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		complexFabricator.duplicantOperated = true;
		ComplexFabricatorWorkable complexFabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
		complexFabricatorWorkable.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_egg_cracker_kanim")
		};
		complexFabricator.outputOffset = new Vector3(1f, 1f, 0f);
		Prioritizable.AddRef(go);
		go.AddOrGet<EggCracker>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
