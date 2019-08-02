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
		requiredSkillPerk = Db.Get().SkillPerks.CanWrangleCreatures.Id;
		attributeConverter = Db.Get().AttributeConverters.RanchingEffectDuration;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.BARELY_EVER_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Ranching.Id;
		skillExperienceMultiplier = SKILLS.BARELY_EVER_EXPERIENCE;
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
