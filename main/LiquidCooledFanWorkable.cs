using TUNING;

public class LiquidCooledFanWorkable : Workable
{
	[MyCmpGet]
	private Operational operational;

	private LiquidCooledFanWorkable()
	{
		showProgressBar = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
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

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole(MachineTechnician.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
		resume.AddExperienceIfRole("PowerTechnician", work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
		resume.AddExperienceIfRole("MechatronicEngineer", work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
	}
}
