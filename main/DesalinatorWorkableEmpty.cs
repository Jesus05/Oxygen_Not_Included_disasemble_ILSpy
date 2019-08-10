using KSerialization;
using TUNING;

public class DesalinatorWorkableEmpty : Workable
{
	[Serialize]
	public int timesCleaned;

	private static readonly HashedString[] WORK_ANIMS = new HashedString[2]
	{
		"salt_pre",
		"salt_loop"
	};

	private static readonly HashedString PST_ANIM = new HashedString("salt_pst");

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workerStatusItem = Db.Get().DuplicantStatusItems.Cleaning;
		workingStatusItem = Db.Get().MiscStatusItems.Cleaning;
		attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_desalinator_kanim")
		};
		workAnims = WORK_ANIMS;
		workingPstComplete = PST_ANIM;
		workingPstFailed = PST_ANIM;
		synchronizeAnims = false;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		timesCleaned++;
		base.OnCompleteWork(worker);
	}
}
