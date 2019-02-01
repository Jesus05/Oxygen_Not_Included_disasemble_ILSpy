using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class EggIncubator : SingleEntityReceptacle, ISaveLoadable, ISim1000ms
{
	[MyCmpAdd]
	private EggIncubatorWorkable workable;

	private Chore chore;

	private EggIncubatorStates.Instance smi;

	private KBatchedAnimTracker tracker;

	private MeterController meter;

	private static readonly EventSystem.IntraObjectHandler<EggIncubator> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<EggIncubator>(delegate(EggIncubator component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<EggIncubator> OnOccupantChangedDelegate = new EventSystem.IntraObjectHandler<EggIncubator>(delegate(EggIncubator component, object data)
	{
		component.OnOccupantChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<EggIncubator> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<EggIncubator>(delegate(EggIncubator component, object data)
	{
		component.OnStorageChange(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		autoReplaceEntity = true;
		statusItemNeed = Db.Get().BuildingStatusItems.NeedEgg;
		statusItemNoneAvailable = Db.Get().BuildingStatusItems.NoAvailableEgg;
		statusItemAwaitingDelivery = Db.Get().BuildingStatusItems.AwaitingEggDelivery;
		requiredRolePerk = RoleManager.rolePerks.CanWrangleCreatures.id;
		occupyingObjectRelativePosition = new Vector3(0.5f, 1f, -1f);
		synchronizeAnims = false;
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		component.SetSymbolVisiblity("egg_target", false);
		meter = new MeterController(this, Meter.Offset.Infront, Grid.SceneLayer.NoLayer);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if ((bool)base.occupyingObject)
		{
			if (base.occupyingObject.HasTag(GameTags.Creature))
			{
				storage.allowItemRemoval = true;
			}
			storage.RenotifyAll();
			PositionOccupyingObject();
		}
		Subscribe(-592767678, OnOperationalChangedDelegate);
		Subscribe(-731304873, OnOccupantChangedDelegate);
		Subscribe(-1697596308, OnStorageChangeDelegate);
		smi = new EggIncubatorStates.Instance(this);
		smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		smi.StopSM("cleanup");
		base.OnCleanUp();
	}

	protected override void SubscribeToOccupant()
	{
		base.SubscribeToOccupant();
		if ((Object)base.occupyingObject != (Object)null)
		{
			tracker = base.occupyingObject.AddComponent<KBatchedAnimTracker>();
			tracker.symbol = "egg_target";
			tracker.forceAlwaysVisible = true;
		}
		UpdateProgress();
	}

	protected override void UnsubscribeFromOccupant()
	{
		base.UnsubscribeFromOccupant();
		Object.Destroy(tracker);
		tracker = null;
		UpdateProgress();
	}

	private void OnOperationalChanged(object data = null)
	{
		if (!(bool)base.occupyingObject)
		{
			storage.DropAll(false);
		}
	}

	private void OnOccupantChanged(object data = null)
	{
		if (!(bool)base.occupyingObject)
		{
			storage.allowItemRemoval = false;
		}
	}

	private void OnStorageChange(object data = null)
	{
		if ((bool)base.occupyingObject && !storage.items.Contains(base.occupyingObject))
		{
			UnsubscribeFromOccupant();
			ClearOccupant();
		}
	}

	protected override void ClearOccupant()
	{
		bool flag = false;
		if ((Object)base.occupyingObject != (Object)null)
		{
			flag = !base.occupyingObject.HasTag(GameTags.Egg);
		}
		base.ClearOccupant();
		if (autoReplaceEntity && flag && requestedEntityTag.IsValid)
		{
			CreateOrder(requestedEntityTag);
		}
	}

	protected override void PositionOccupyingObject()
	{
		base.PositionOccupyingObject();
		KBatchedAnimController component = base.occupyingObject.GetComponent<KBatchedAnimController>();
		component.SetSceneLayer(Grid.SceneLayer.BuildingUse);
		KSelectable component2 = base.occupyingObject.GetComponent<KSelectable>();
		if ((Object)component2 != (Object)null)
		{
			component2.IsSelectable = true;
		}
	}

	public override void OrderRemoveOccupant()
	{
		Object.Destroy(tracker);
		tracker = null;
		storage.DropAll(false);
		base.occupyingObject = null;
		ClearOccupant();
	}

	public float GetProgress()
	{
		float result = 0f;
		if ((bool)base.occupyingObject)
		{
			Amounts amounts = base.occupyingObject.GetAmounts();
			AmountInstance amountInstance = amounts.Get(Db.Get().Amounts.Incubation);
			result = ((amountInstance == null) ? 1f : (amountInstance.value / amountInstance.GetMax()));
		}
		return result;
	}

	private void UpdateProgress()
	{
		meter.SetPositionPercent(GetProgress());
	}

	public void Sim1000ms(float dt)
	{
		UpdateProgress();
		UpdateChore();
	}

	public void StoreBaby(GameObject baby)
	{
		UnsubscribeFromOccupant();
		storage.DropAll(false);
		storage.allowItemRemoval = true;
		storage.Store(baby, false, false, true, false);
		base.occupyingObject = baby;
		SubscribeToOccupant();
		Trigger(-731304873, base.occupyingObject);
	}

	private void UpdateChore()
	{
		if (operational.IsOperational && EggNeedsAttention())
		{
			if (chore == null)
			{
				chore = new WorkChore<EggIncubatorWorkable>(Db.Get().ChoreTypes.EggSing, workable, null, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
			}
		}
		else if (chore != null)
		{
			chore.Cancel("now is not the time for song");
			chore = null;
		}
	}

	private bool EggNeedsAttention()
	{
		if ((bool)base.Occupant)
		{
			IncubationMonitor.Instance sMI = base.Occupant.GetSMI<IncubationMonitor.Instance>();
			if (sMI != null)
			{
				return !sMI.HasSongBuff();
			}
			return false;
		}
		return false;
	}
}
