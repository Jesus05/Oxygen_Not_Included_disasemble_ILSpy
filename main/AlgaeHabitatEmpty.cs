using TUNING;

public class AlgaeHabitatEmpty : Workable
{
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
		workAnims = CLEAN_ANIMS;
		workingPstComplete = PST_ANIM;
		workingPstFailed = PST_ANIM;
		synchronizeAnims = false;
	}
}
