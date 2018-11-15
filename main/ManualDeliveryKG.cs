using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ManualDeliveryKG : KMonoBehaviour, ISim200ms
{
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

	[SerializeField]
	public Tag[] choreTags;

	[Serialize]
	private bool userPaused;

	[NonSerialized]
	public bool ShowStatusItem = true;

	private FetchList2 fetchList;

	private List<PrimaryElement> filteredStoredItems = new List<PrimaryElement>();

	private int onStorageChangeSubscription = -1;

	private static readonly EventSystem.IntraObjectHandler<ManualDeliveryKG> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<ManualDeliveryKG>(delegate(ManualDeliveryKG component, object data)
	{
		component.OnRefreshUserMenu(data);
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
		if (!choreTypeIDHash.IsValid)
		{
			choreTypeIDHash = Db.Get().ChoreTypes.Fetch.IdHash;
		}
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Subscribe(-111137758, OnRefreshUserMenuDelegate);
		if ((UnityEngine.Object)storage != (UnityEngine.Object)null)
		{
			SetStorage(storage);
		}
		UpdateFilteredItems();
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
		filteredStoredItems.Clear();
		this.storage = storage;
		if ((UnityEngine.Object)this.storage != (UnityEngine.Object)null && base.isSpawned)
		{
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

	public void Sim200ms(float dt)
	{
		UpdateDeliveryState();
	}

	[ContextMenu("UpdateDeliveryState")]
	public void UpdateDeliveryState()
	{
		if (requestedItemTag.IsValid && !((UnityEngine.Object)storage == (UnityEngine.Object)null) && !paused)
		{
			RequestDelivery();
		}
	}

	private void RequestDelivery()
	{
		float fetchAmount = GetFetchAmount();
		if (fetchAmount > 0f && (this.fetchList == null || this.fetchList.IsComplete))
		{
			if (this.fetchList != null)
			{
				this.fetchList.Cancel("Request Delivery");
			}
			ChoreType byHash = Db.Get().ChoreTypes.GetByHash(choreTypeIDHash);
			this.fetchList = new FetchList2(storage, byHash, choreTags);
			this.fetchList.ShowStatusItem = ShowStatusItem;
			this.fetchList.MinimumAmount[requestedItemTag] = minimumMass;
			FetchList2 fetchList = this.fetchList;
			Tag[] tags = new Tag[1]
			{
				requestedItemTag
			};
			float amount = fetchAmount;
			FetchOrder2.OperationalRequirement operationalRequirement = this.operationalRequirement;
			fetchList.Add(tags, null, null, amount, operationalRequirement);
			this.fetchList.Submit(null, false);
		}
	}

	private float GetFetchAmount()
	{
		float result = 0f;
		float num = 0f;
		for (int i = 0; i < filteredStoredItems.Count; i++)
		{
			num += filteredStoredItems[i].Mass;
		}
		if (num < refillMass)
		{
			result = Mathf.Max(0f, capacity - num);
		}
		return result;
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
			UpdateFilteredItems();
		}
	}

	private void UpdateFilteredItems()
	{
		filteredStoredItems.Clear();
		int num = 0;
		while ((UnityEngine.Object)storage != (UnityEngine.Object)null && num < storage.items.Count)
		{
			GameObject gameObject = storage.items[num];
			if (gameObject.HasTag(requestedItemTag))
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				filteredStoredItems.Add(component);
			}
			num++;
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
}
