using TUNING;
using UnityEngine;

public class Apothecary : ComplexFabricator
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		choreType = Db.Get().ChoreTypes.Compound;
		fetchChoreTypeIdHash = Db.Get().ChoreTypes.DoctorFetch.IdHash;
		sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		workable.WorkerStatusItem = Db.Get().DuplicantStatusItems.Fabricating;
		workable.AttributeConverter = Db.Get().AttributeConverters.CompoundingSpeed;
		workable.SkillExperienceSkillGroup = Db.Get().SkillGroups.MedicalAid.Id;
		workable.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		workable.requiredSkillPerk = Db.Get().SkillPerks.CanCompound.Id;
		workable.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_apothecary_kanim")
		};
		workable.AnimOffset = new Vector3(-1f, 0f, 0f);
	}
}
