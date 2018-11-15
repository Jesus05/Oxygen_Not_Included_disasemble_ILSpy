using TUNING;
using UnityEngine;

public class ApothecaryConfig : IBuildingConfig
{
	public const string ID = "Apothecary";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "Apothecary";
		int width = 2;
		int height = 3;
		string anim = "apothecary_kanim";
		int hitpoints = 30;
		float construction_time = 120f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] aLL_METALS = MATERIALS.ALL_METALS;
		float melting_point = 800f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, aLL_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.NONE, nONE, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ExhaustKilowattsWhenActive = 0.25f;
		buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.AudioCategory = "Glass";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		Prioritizable.AddRef(go);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		Fabricator fabricator = go.AddOrGet<Fabricator>();
		BuildingTemplates.CreateFabricatorStorage(go, fabricator);
		fabricator.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_apothecary_kanim")
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<PoweredActiveStoppableController.Def>();
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			Fabricator component = game_object.GetComponent<Fabricator>();
			component.SetAttributeConverter(Db.Get().AttributeConverters.CompoundingSpeed);
		};
	}
}
