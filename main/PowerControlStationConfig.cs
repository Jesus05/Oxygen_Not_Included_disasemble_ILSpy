using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class PowerControlStationConfig : IBuildingConfig
{
	public const string ID = "PowerControlStation";

	public static Tag MATERIAL_FOR_TINKER = GameTags.RefinedMetal;

	public static Tag TINKER_TOOLS = PowerStationToolsConfig.tag;

	public const float MASS_PER_TINKER = 5f;

	public static string ROLE_PERK = "CanPowerTinker";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "PowerControlStation";
		int width = 2;
		int height = 4;
		string anim = "electricianworkdesk_kanim";
		int hitpoints = 30;
		float construction_time = 30f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] rEFINED_METALS = MATERIALS.REFINED_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, rEFINED_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.NONE, tIER2, 0.2f);
		buildingDef.ViewMode = OverlayModes.Rooms.ID;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.PowerStation);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_0);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_0);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_0);
		go.AddOrGet<LogicOperationalController>();
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 50f;
		storage.showInUI = true;
		storage.storageFilters = new List<Tag>
		{
			MATERIAL_FOR_TINKER
		};
		TinkerStation tinkerStation = go.AddOrGet<TinkerStation>();
		tinkerStation.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_electricianworkdesk_kanim")
		};
		tinkerStation.inputMaterial = MATERIAL_FOR_TINKER;
		tinkerStation.massPerTinker = 5f;
		tinkerStation.outputPrefab = TINKER_TOOLS;
		tinkerStation.requiredSkillPerk = ROLE_PERK;
		tinkerStation.choreType = Db.Get().ChoreTypes.PowerFabricate.IdHash;
		tinkerStation.useFilteredStorage = true;
		tinkerStation.fetchChoreType = Db.Get().ChoreTypes.PowerFetch.IdHash;
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.PowerPlant.Id;
		roomTracker.requirement = RoomTracker.Requirement.Required;
		Prioritizable.AddRef(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			TinkerStation component = game_object.GetComponent<TinkerStation>();
			component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
			component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
			component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
			component.SkillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		};
	}
}
