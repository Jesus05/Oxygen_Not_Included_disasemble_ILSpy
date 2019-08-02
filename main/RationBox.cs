using KSerialization;
using UnityEngine;

public class RationBox : KMonoBehaviour, IUserControlledCapacity, IRender1000ms
{
	[MyCmpReq]
	private Storage storage;

	[Serialize]
	private float userMaxCapacity = float.PositiveInfinity;

	private FilteredStorage filteredStorage;

	private static readonly EventSystem.IntraObjectHandler<RationBox> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<RationBox>(delegate(RationBox component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<RationBox> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<RationBox>(delegate(RationBox component, object data)
	{
		component.OnCopySettings(data);
	});

	public float UserMaxCapacity
	{
		get
		{
			return Mathf.Min(userMaxCapacity, storage.capacityKg);
		}
		set
		{
			userMaxCapacity = value;
			filteredStorage.FilterChanged();
		}
	}

	public float AmountStored => storage.MassStored();

	public float MinCapacity => 0f;

	public float MaxCapacity => storage.capacityKg;

	public bool WholeValues => false;

	public LocString CapacityUnits => GameUtil.GetCurrentMassUnit(false);

	protected override void OnPrefabInit()
	{
		filteredStorage = new FilteredStorage(this, null, new Tag[1]
		{
			GameTags.Compostable
		}, this, false, Db.Get().ChoreTypes.FoodFetch);
		Subscribe(-592767678, OnOperationalChangedDelegate);
		Subscribe(-905833192, OnCopySettingsDelegate);
		WorldInventory.Instance.Discover("FieldRation".ToTag(), GameTags.Edible);
	}

	protected override void OnSpawn()
	{
		Operational component = GetComponent<Operational>();
		component.SetActive(component.IsOperational, false);
		filteredStorage.FilterChanged();
	}

	protected override void OnCleanUp()
	{
		filteredStorage.CleanUp();
	}

	private void OnOperationalChanged(object data)
	{
		Operational component = GetComponent<Operational>();
		component.SetActive(component.IsOperational, false);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (!((Object)gameObject == (Object)null))
		{
			RationBox component = gameObject.GetComponent<RationBox>();
			if (!((Object)component == (Object)null))
			{
				UserMaxCapacity = component.UserMaxCapacity;
			}
		}
	}

	public void Render1000ms(float dt)
	{
		Rottable.SetStatusItems(GetComponent<KSelectable>(), Rottable.IsRefrigerated(base.gameObject), Rottable.AtmosphereQuality(base.gameObject));
	}
}
