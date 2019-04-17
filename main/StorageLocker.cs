using KSerialization;
using UnityEngine;

public class StorageLocker : KMonoBehaviour, IUserControlledCapacity
{
	private LoggerFS log;

	[Serialize]
	private float userMaxCapacity = float.PositiveInfinity;

	[Serialize]
	public string lockerName = string.Empty;

	protected FilteredStorage filteredStorage;

	private static readonly EventSystem.IntraObjectHandler<StorageLocker> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<StorageLocker>(delegate(StorageLocker component, object data)
	{
		component.OnCopySettings(data);
	});

	public virtual float UserMaxCapacity
	{
		get
		{
			return Mathf.Min(userMaxCapacity, GetComponent<Storage>().capacityKg);
		}
		set
		{
			userMaxCapacity = value;
			filteredStorage.FilterChanged();
		}
	}

	public float AmountStored => GetComponent<Storage>().MassStored();

	public float MinCapacity => 0f;

	public float MaxCapacity => GetComponent<Storage>().capacityKg;

	public bool WholeValues => false;

	public LocString CapacityUnits => GameUtil.GetCurrentMassUnit(false);

	protected override void OnPrefabInit()
	{
		Initialize(false);
	}

	protected void Initialize(bool use_logic_meter)
	{
		base.OnPrefabInit();
		log = new LoggerFS("StorageLocker", 35);
		filteredStorage = new FilteredStorage(this, null, null, this, use_logic_meter, Db.Get().ChoreTypes.StorageFetch);
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	protected override void OnSpawn()
	{
		filteredStorage.FilterChanged();
		if (!lockerName.IsNullOrWhiteSpace())
		{
			SetName(lockerName);
		}
	}

	protected override void OnCleanUp()
	{
		filteredStorage.CleanUp();
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (!((Object)gameObject == (Object)null))
		{
			StorageLocker component = gameObject.GetComponent<StorageLocker>();
			if (!((Object)component == (Object)null))
			{
				UserMaxCapacity = component.UserMaxCapacity;
			}
		}
	}

	public void SetName(string name)
	{
		KSelectable component = GetComponent<KSelectable>();
		base.name = name;
		lockerName = name;
		if ((Object)component != (Object)null)
		{
			component.SetName(name);
		}
		base.gameObject.name = name;
		NameDisplayScreen.Instance.UpdateName(base.gameObject);
	}
}
