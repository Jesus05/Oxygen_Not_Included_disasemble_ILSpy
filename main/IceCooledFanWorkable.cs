using TUNING;

public class IceCooledFanWorkable : Workable
{
	[MyCmpGet]
	private Operational operational;

	private IceCooledFanWorkable()
	{
		showProgressBar = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
		skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		workerStatusItem = null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	protected override void OnStartWork(Worker worker)
	{
		operational.SetActive(true, false);
	}

	protected override void OnStopWork(Worker worker)
	{
		operational.SetActive(false, false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		operational.SetActive(false, false);
	}
}
