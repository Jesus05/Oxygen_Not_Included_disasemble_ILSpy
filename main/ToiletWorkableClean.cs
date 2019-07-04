using KSerialization;
using TUNING;

public class ToiletWorkableClean : Workable
{
	[Serialize]
	public int timesCleaned = 0;

	private static readonly HashedString[] CLEAN_ANIMS = new HashedString[2]
	{
		"unclog_pre",
		"unclog_loop"
	};

	private static readonly HashedString PST_ANIM = new HashedString("unclog_pst");

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workerStatusItem = Db.Get().DuplicantStatusItems.Cleaning;
		workingStatusItem = Db.Get().MiscStatusItems.Cleaning;
		attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Basekeeping.Id;
		skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		workAnims = CLEAN_ANIMS;
		workingPstComplete = PST_ANIM;
		workingPstFailed = PST_ANIM;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		timesCleaned++;
		base.OnCompleteWork(worker);
	}
}
