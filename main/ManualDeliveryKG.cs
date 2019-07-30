using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ManualDeliveryKG : KMonoBehaviour, ISim1000ms
{
	[MyCmpGet]
	private Operational operational;

	[SerializeField]
	private Storage storage;

	[SerializeField]
	public Tag requestedItemTag;

	[SerializeField]
	public float capacity = 100f;

	[SerializeField]
	public float refillMass = 10f;

	[SerializeField]
	public float minimumMass = 10f;

	[SerializeField]
	public FetchOrder2.OperationalRequirement operationalRequirement;

	[SerializeField]
	public bool allowPause;

	[SerializeField]
	private bool paused;

	[SerializeField]
	public HashedString choreTypeIDHash;

	[Serialize]
	private bool userPaused;

	public bool ShowStatusItem = true;

	private FetchList2 fetchList;

	private int onStorageChangeSubscription = -1;

	private static readonly EventSystem.IntraObjectHandler<ManualDeliveryKG> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<ManualDeliveryKG>(delegate(ManualDeliveryKG component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<ManualDeliveryKG> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<ManualDeliveryKG>(delegate(ManualDeliveryKG component, object data)
	{
		component.OnOperationalChanged(data);
	});

	public float Capacity => capacity;

	public Tag RequestedItemTag
	{
		get
		{
			return requestedItemTag;
		}
		set
		{
			requestedItemTag = value;
			AbortDelivery("Requested Item Tag Changed");
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		DebugUtil.Assert(choreTypeIDHash.IsValid, "ManualDeliveryKG Must have a valid chore type specified!", base.name);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Subscribe(-111137758, OnRefreshUserMenuDelegate);
		Subscribe(-592767678, OnOperationalChangedDelegate);
		if ((UnityEngine.Object)storage != (UnityEngine.Object)null)
		{
			SetStorage(storage);
		}
		Prioritizable.AddRef(base.gameObject);
		if (userPaused && allowPause)
		{
			OnPause();
		}
	}

	protected override void OnCleanUp()
	{
		AbortDelivery("ManualDeliverKG destroyed");
		Prioritizable.RemoveRef(base.gameObject);
		base.OnCleanUp();
	}

	public void SetStorage(Storage storage)
	{
		if ((UnityEngine.Object)this.storage != (UnityEngine.Object)null)
		{
			this.storage.Unsubscribe(onStorageChangeSubscription);
			onStorageChangeSubscription = -1;
		}
		AbortDelivery("storage pointer changed");
		this.storage = storage;
		if ((UnityEngine.Object)this.storage != (UnityEngine.Object)null && base.isSpawned)
		{
			Debug.Assert(onStorageChangeSubscription == -1);
			onStorageChangeSubscription = this.storage.Subscribe(-1697596308, delegate
			{
				OnStorageChanged(this.storage);
			});
		}
	}

	public void Pause(bool pause, string reason)
	{
		if (paused != pause)
		{
			paused = pause;
			if (pause)
			{
				AbortDelivery(reason);
			}
		}
	}

	public void Sim1000ms(float dt)
	{
		UpdateDeliveryState();
	}

	[ContextMenu("UpdateDeliveryState")]
	public void UpdateDeliveryState()
	{
		if (requestedItemTag.IsValid && !((UnityEngine.Object)storage == (UnityEngine.Object)null))
		{
			UpdateFetchList();
		}
	}

	private void UpdateFetchList()
	{
		if (!paused)
		{
			if (this.fetchList != null && this.fetchList.IsComplete)
			{
				this.fetchList = null;
			}
			if (!OperationalRequirementsMet())
			{
				if (this.fetchList != null)
				{
					this.fetchList.Cancel("Operational requirements");
					this.fetchList = null;
				}
			}
			else if (this.fetchList == null)
			{
				float massAvailable = storage.GetMassAvailable(requestedItemTag);
				if (massAvailable < refillMass)
				{
					float b = capacity - massAvailable;
					b = Mathf.Max(PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT, b);
					ChoreType byHash = Db.Get().ChoreTypes.GetByHash(choreTypeIDHash);
					this.fetchList = new FetchList2(storage, byHash);
					this.fetchList.ShowStatusItem = ShowStatusItem;
					this.fetchList.MinimumAmount[requestedItemTag] = Mathf.Max(PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT, minimumMass);
					FetchList2 fetchList = this.fetchList;
					Tag[] tags = new Tag[1]
					{
						requestedItemTag
					};
					float amount = b;
					fetchList.Add(tags, null, null, amount, FetchOrder2.OperationalRequirement.None);
					this.fetchList.Submit(null, false);
				}
			}
		}
	}

	private bool OperationalRequirementsMet()
	{
		if ((bool)operational)
		{
			if (operationalRequirement == FetchOrder2.OperationalRequirement.Operational)
			{
				return operational.IsOperational;
			}
			if (operationalRequirement == FetchOrder2.OperationalRequirement.Functional)
			{
				return operational.IsFunctional;
			}
		}
		return true;
	}

	public void AbortDelivery(string reason)
	{
		if (this.fetchList != null)
		{
			FetchList2 fetchList = this.fetchList;
			this.fetchList = null;
			fetchList.Cancel(reason);
		}
	}

	private void OnStorageChanged(Storage storage)
	{
		if ((UnityEngine.Object)storage == (UnityEngine.Object)this.storage)
		{
			UpdateDeliveryState();
		}
	}

	private void OnPause()
	{
		userPaused = true;
		Pause(true, "Forbid manual delivery");
	}

	private void OnResume()
	{
		userPaused = false;
		Pause(false, "Allow manual delivery");
	}

	private void OnRefreshUserMenu(object data)
	{
		if (allowPause)
		{
			object buttonInfo;
			if (!paused)
			{
				string iconName = "action_move_to_storage";
				string text = UI.USERMENUACTIONS.MANUAL_DELIVERY.NAME;
				System.Action on_click = OnPause;
				string tooltipText = UI.USERMENUACTIONS.MANUAL_DELIVERY.TOOLTIP;
				buttonInfo = new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true);
			}
			else
			{
				string tooltipText = "action_move_to_storage";
				string text = UI.USERMENUACTIONS.MANUAL_DELIVERY.NAME_OFF;
				System.Action on_click = OnResume;
				string iconName = UI.USERMENUACTIONS.MANUAL_DELIVERY.TOOLTIP_OFF;
				buttonInfo = new KIconButtonMenu.ButtonInfo(tooltipText, text, on_click, Action.NumActions, null, null, null, iconName, true);
			}
			KIconButtonMenu.ButtonInfo button = (KIconButtonMenu.ButtonInfo)buttonInfo;
			Game.Instance.userMenu.AddButton(base.gameObject, button, 1f);
		}
	}

	private void OnOperationalChanged(object data)
	{
		UpdateDeliveryState();
	}
}
