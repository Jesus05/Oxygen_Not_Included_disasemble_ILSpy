using KSerialization;
using STRINGS;
using UnityEngine;

public class StorageLocker : KMonoBehaviour, IUserControlledCapacity
{
	private LoggerFS log;

	[Serialize]
	private float userMaxCapacity = float.PositiveInfinity;

	protected FilteredStorage filteredStorage;

	private static readonly EventSystem.IntraObjectHandler<StorageLocker> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<StorageLocker>(delegate(StorageLocker component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly EventSystem.IntraObjectHandler<StorageLocker> OnToggleClosedDelegate = new EventSystem.IntraObjectHandler<StorageLocker>(delegate(StorageLocker component, object data)
	{
		component.OnToggleClosed(data);
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

	public LocString CapacityUnits
	{
		get
		{
			LocString locString = null;
			switch (GameUtil.massUnit)
			{
			case GameUtil.MassUnit.Pounds:
				return UI.UNITSUFFIXES.MASS.POUND;
			default:
				return UI.UNITSUFFIXES.MASS.KILOGRAM;
			}
		}
	}

	protected override void OnPrefabInit()
	{
		Initialize(false);
	}

	protected void Initialize(bool use_logic_meter)
	{
		base.OnPrefabInit();
		log = new LoggerFS("StorageLocker", 35);
		filteredStorage = new FilteredStorage(this, null, null, this, use_logic_meter, Db.Get().ChoreTypes.Fetch);
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	protected override void OnSpawn()
	{
		Subscribe(1088293757, OnToggleClosedDelegate);
		filteredStorage.FilterChanged();
	}

	protected override void OnCleanUp()
	{
		filteredStorage.CleanUp();
	}

	private void OnToggleClosed(object data)
	{
		BuildingEnabledButton component = GetComponent<BuildingEnabledButton>();
		bool flag = (Object)component != (Object)null && !component.IsEnabled;
		filteredStorage.SetEnabled(!flag);
		Game.Instance.userMenu.Refresh(base.gameObject);
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
}
