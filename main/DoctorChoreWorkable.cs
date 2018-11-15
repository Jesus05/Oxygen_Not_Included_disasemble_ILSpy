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
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
	}
}
