using TUNING;
using UnityEngine;

public class MedicalBedConfig : IBuildingConfig
{
	public const string ID = "MedicalBed";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "MedicalBed";
		int width = 2;
		int height = 3;
		string anim = "bed_medical_kanim";
		int hitpoints = 100;
		float construction_time = 10f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] rEFINED_METALS = MATERIALS.REFINED_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, rEFINED_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.BONUS.TIER2, tIER2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ExhaustKilowattsWhenActive = 0.25f;
		buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.Clinic);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KAnimControllerBase>().initialAnim = "off";
		go.GetComponent<KPrefabID>().AddTag(TagManager.Create("Bed"));
		Clinic clinic = go.AddOrGet<Clinic>();
		clinic.workerInjuredAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_medical_bed_kanim")
		};
		clinic.workerDiseasedAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_medical_bed_kanim")
		};
		clinic.workLayer = Grid.SceneLayer.BuildingFront;
		clinic.doctorVisitInterval = 450f;
		clinic.workLayer = Grid.SceneLayer.BuildingFront;
		string diseaseEffect = "Rejuvenator";
		string doctoredDiseaseEffect = "RejuvenatorDoctored";
		clinic.diseaseEffect = diseaseEffect;
		clinic.doctoredDiseaseEffect = doctoredDiseaseEffect;
		clinic.doctoredPlaceholderEffect = "DoctoredOffRejuvenatorEffect";
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.Hospital.Id;
		roomTracker.requirement = RoomTracker.Requirement.CustomRecommended;
		roomTracker.customStatusItemID = Db.Get().BuildingStatusItems.ClinicOutsideHospital.Id;
		Sleepable sleepable = go.AddOrGet<Sleepable>();
		sleepable.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_medical_bed_kanim")
		};
		DoctorChoreWorkable doctorChoreWorkable = go.AddOrGet<DoctorChoreWorkable>();
		doctorChoreWorkable.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_med_cot_doctor_kanim")
		};
		doctorChoreWorkable.workTime = 25f;
		Ownable ownable = go.AddOrGet<Ownable>();
		ownable.slotID = Db.Get().AssignableSlots.Clinic.Id;
	}
}
