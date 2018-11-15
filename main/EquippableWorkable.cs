using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class EquippableWorkable : Workable, ISaveLoadable
{
	[MyCmpReq]
	private Equippable equippable;

	private Chore chore;

	private QualityLevel quality;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workerStatusItem = Db.Get().DuplicantStatusItems.Equipping;
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_equip_clothing_kanim")
		};
	}

	public QualityLevel GetQuality()
	{
		return quality;
	}

	public void SetQuality(QualityLevel level)
	{
		quality = level;
	}

	protected override void OnSpawn()
	{
		SetWorkTime(1.5f);
		equippable.OnAssign += RefreshChore;
	}

	private void CreateChore()
	{
		chore = new WorkChore<EquippableWorkable>(Db.Get().ChoreTypes.Equip, this, equippable.assignee.GetSoleOwner().GetComponent<ChoreProvider>(), null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
	}

	public void CancelChore()
	{
		if (chore != null)
		{
			chore.Cancel("Manual equip");
			chore = null;
		}
	}

	private void RefreshChore(IAssignableIdentity target)
	{
		if (chore != null)
		{
			chore.Cancel("Equipment Reassigned");
			chore = null;
		}
		if (target != null && !target.GetSoleOwner().GetComponent<Equipment>().IsEquipped(equippable))
		{
			CreateChore();
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		if (equippable.assignee != null)
		{
			equippable.assignee.GetSoleOwner().GetComponent<Equipment>().Equip(equippable);
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		workTimeRemaining = GetWorkTime();
		base.OnStopWork(worker);
	}
}
