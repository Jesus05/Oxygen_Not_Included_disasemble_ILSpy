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
		synchronizeAnims = false;
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
		Debug.Assert(chore == null, "chore should be null");
		chore = new EquipChore(this);
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
		if (target != null)
		{
			Ownables soleOwner = target.GetSoleOwner();
			Equipment component = soleOwner.GetComponent<Equipment>();
			if (!component.IsEquipped(equippable))
			{
				CreateChore();
			}
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		if (equippable.assignee != null)
		{
			Ownables soleOwner = equippable.assignee.GetSoleOwner();
			if ((bool)soleOwner)
			{
				soleOwner.GetComponent<Equipment>().Equip(equippable);
			}
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		workTimeRemaining = GetWorkTime();
		base.OnStopWork(worker);
	}
}
