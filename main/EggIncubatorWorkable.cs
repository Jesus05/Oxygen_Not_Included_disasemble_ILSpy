using TUNING;

public class EggIncubatorWorkable : Workable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		synchronizeAnims = false;
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_incubator_kanim")
		};
		SetWorkTime(15f);
		showProgressBar = true;
		requiredRolePerk = RoleManager.rolePerks.CanWrangleCreatures.id;
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole("Rancher", work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_QUICK);
		resume.AddExperienceIfRole("SeniorRancher", work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_QUICK);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		EggIncubator component = GetComponent<EggIncubator>();
		if ((bool)component && (bool)component.Occupant)
		{
			component.Occupant.GetSMI<IncubationMonitor.Instance>()?.ApplySongBuff();
		}
	}
}
