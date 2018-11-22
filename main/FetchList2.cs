using System;
using System.Collections.Generic;
using UnityEngine;

public class FetchList2 : IFetchList
{
	private System.Action OnComplete;

	private ChoreType choreType;

	private Tag[] choreTags;

	public Guid waitingForMaterialsHandle = Guid.Empty;

	public Guid materialsUnavailableForRefillHandle = Guid.Empty;

	public Guid materialsUnavailableHandle = Guid.Empty;

	public Dictionary<Tag, float> MinimumAmount = new Dictionary<Tag, float>();

	public List<FetchOrder2> FetchOrders = new List<FetchOrder2>();

	private Dictionary<Tag, float> Remaining = new Dictionary<Tag, float>();

	private bool bShowStatusItem = true;

	public bool ShowStatusItem
	{
		get
		{
			return bShowStatusItem;
		}
		set
		{
			bShowStatusItem = value;
		}
	}

	public bool IsComplete => FetchOrders.Count == 0;

	public bool InProgress
	{
		get
		{
			if (FetchOrders.Count >= 0)
			{
				bool result = false;
				foreach (FetchOrder2 fetchOrder in FetchOrders)
				{
					if (fetchOrder.InProgress)
					{
						result = true;
						break;
					}
				}
				return result;
			}
			return false;
		}
	}

	public Storage Destination
	{
		get;
		private set;
	}

	public int PriorityMod
	{
		get;
		private set;
	}

	public FetchList2(Storage destination, ChoreType chore_type, Tag[] chore_tags)
	{
		Destination = destination;
		choreType = chore_type;
		choreTags = chore_tags;
	}

	public void SetPriorityMod(int priorityMod)
	{
		PriorityMod = priorityMod;
		for (int i = 0; i < FetchOrders.Count; i++)
		{
			FetchOrders[i].SetPriorityMod(PriorityMod);
		}
	}

	public void Add(Tag[] tags, Tag[] required_tags = null, Tag[] forbidden_tags = null, float amount = 1f, FetchOrder2.OperationalRequirement operationalRequirement = FetchOrder2.OperationalRequirement.None)
	{
		if (amount <= 0f)
		{
			Output.LogError("Requesting an invalid FetchList2 amount");
		}
		foreach (Tag key in tags)
		{
			if (!MinimumAmount.ContainsKey(key))
			{
				MinimumAmount[key] = amount;
			}
		}
		FetchOrder2 item = new FetchOrder2(choreType, tags, required_tags, forbidden_tags, Destination, amount, operationalRequirement, PriorityMod, choreTags);
		FetchOrders.Add(item);
	}

	public void Add(Tag tag, Tag[] required_tags = null, Tag[] forbidden_tags = null, float amount = 1f, FetchOrder2.OperationalRequirement operationalRequirement = FetchOrder2.OperationalRequirement.None)
	{
		Add(new Tag[1]
		{
			tag
		}, required_tags, forbidden_tags, amount, operationalRequirement);
	}

	public float GetMinimumAmount(Tag tag)
	{
		float value = 0f;
		MinimumAmount.TryGetValue(tag, out value);
		return value;
	}

	private void OnFetchOrderComplete(FetchOrder2 fetch_order, Pickupable fetched_item)
	{
		FetchOrders.Remove(fetch_order);
		if (FetchOrders.Count == 0)
		{
			if (OnComplete != null)
			{
				OnComplete();
			}
			FetchListStatusItemUpdater.instance.RemoveFetchList(this);
			ClearStatus();
		}
	}

	public void Cancel(string reason)
	{
		foreach (FetchOrder2 fetchOrder in FetchOrders)
		{
			fetchOrder.Cancel(reason);
		}
		ClearStatus();
		FetchListStatusItemUpdater.instance.RemoveFetchList(this);
	}

	public void UpdateRemaining()
	{
		Remaining.Clear();
		for (int i = 0; i < FetchOrders.Count; i++)
		{
			FetchOrder2 fetchOrder = FetchOrders[i];
			for (int j = 0; j < fetchOrder.Tags.Length; j++)
			{
				Tag key = fetchOrder.Tags[j];
				float value = 0f;
				Remaining.TryGetValue(key, out value);
				Remaining[key] = value + fetchOrder.AmountWaitingToFetch();
			}
		}
	}

	public Dictionary<Tag, float> GetRemaining()
	{
		return Remaining;
	}

	public Dictionary<Tag, float> GetRemainingMinimum()
	{
		Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
		foreach (FetchOrder2 fetchOrder in FetchOrders)
		{
			Tag[] tags = fetchOrder.Tags;
			foreach (Tag key in tags)
			{
				dictionary[key] = MinimumAmount[key];
			}
		}
		foreach (GameObject item in Destination.items)
		{
			if ((UnityEngine.Object)item != (UnityEngine.Object)null)
			{
				Pickupable component = item.GetComponent<Pickupable>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					KPrefabID component2 = component.GetComponent<KPrefabID>();
					foreach (Tag tag in component2.Tags)
					{
						if (dictionary.ContainsKey(tag))
						{
							dictionary[tag] = Math.Max(dictionary[tag] - component.TotalAmount, 0f);
						}
					}
				}
			}
		}
		return dictionary;
	}

	public void Suspend(string reason)
	{
		foreach (FetchOrder2 fetchOrder in FetchOrders)
		{
			fetchOrder.Suspend(reason);
		}
	}

	public void Resume(string reason)
	{
		foreach (FetchOrder2 fetchOrder in FetchOrders)
		{
			fetchOrder.Resume(reason);
		}
	}

	public void Submit(System.Action on_complete, bool check_storage_contents)
	{
		OnComplete = on_complete;
		List<FetchOrder2> range = FetchOrders.GetRange(0, FetchOrders.Count);
		foreach (FetchOrder2 item in range)
		{
			item.Submit(OnFetchOrderComplete, check_storage_contents, null);
		}
		if (!IsComplete && ShowStatusItem)
		{
			FetchListStatusItemUpdater.instance.AddFetchList(this);
		}
	}

	private void ClearStatus()
	{
		if ((UnityEngine.Object)Destination != (UnityEngine.Object)null)
		{
			KSelectable component = Destination.GetComponent<KSelectable>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				waitingForMaterialsHandle = component.RemoveStatusItem(waitingForMaterialsHandle, false);
				materialsUnavailableHandle = component.RemoveStatusItem(materialsUnavailableHandle, false);
				materialsUnavailableForRefillHandle = component.RemoveStatusItem(materialsUnavailableForRefillHandle, false);
			}
		}
	}

	public void UpdateStatusItem(MaterialsStatusItem status_item, ref Guid handle, bool should_add)
	{
		bool flag = handle != Guid.Empty;
		if (should_add != flag)
		{
			if (should_add)
			{
				KSelectable component = Destination.GetComponent<KSelectable>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					handle = component.AddStatusItem(status_item, this);
				}
			}
			else
			{
				KSelectable component2 = Destination.GetComponent<KSelectable>();
				if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
				{
					handle = component2.RemoveStatusItem(handle, false);
				}
			}
		}
	}
}
