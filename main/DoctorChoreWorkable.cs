using TUNING;

public class DoctorChoreWorkable : Workable
{
	private DoctorChoreWorkable()
	{
		synchronizeAnims = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		attributeConverter = Db.Get().AttributeConverters.DoctorSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.BARELY_EVER_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.MedicalAid.Id;
		skillExperienceMultiplier = SKILLS.BARELY_EVER_EXPERIENCE;
	}
}
