using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FetchManager : KMonoBehaviour, ISim1000ms
{
	public struct Fetchable
	{
		public Pickupable pickupable;

		public int tagBitsHash;

		public byte masterPriority;

		public byte freshness;

		public byte foodQuality;
	}

	[DebuggerDisplay("{pickupable.name}")]
	public struct Pickup
	{
		public Pickupable pickupable;

		public int tagBitsHash;

		public ushort PathCost;

		public byte masterPriority;

		public byte freshness;

		public byte foodQuality;
	}

	private class PickupComparerIncludingPriority : IComparer<Pickup>
	{
		public int Compare(Pickup a, Pickup b)
		{
			int num = a.tagBitsHash - b.tagBitsHash;
			if (num == 0)
			{
				if (a.masterPriority != b.masterPriority)
				{
					return b.masterPriority - a.masterPriority;
				}
				if (a.PathCost != b.PathCost)
				{
					return a.PathCost - b.PathCost;
				}
				if (a.foodQuality != b.foodQuality)
				{
					return b.foodQuality - a.foodQuality;
				}
				return b.freshness - a.freshness;
			}
			return num;
		}
	}

	private class PickupComparerNoPriority : IComparer<Pickup>
	{
		public int Compare(Pickup a, Pickup b)
		{
			if (a.PathCost != b.PathCost)
			{
				return a.PathCost - b.PathCost;
			}
			if (a.foodQuality != b.foodQuality)
			{
				return b.foodQuality - a.foodQuality;
			}
			return b.freshness - a.freshness;
		}
	}

	public class FecthablesByPrefabId
	{
		public KCompactedVector<Fetchable> fetchables;

		public List<Pickup> finalPickups = new List<Pickup>();

		private Dictionary<HandleVector<int>.Handle, Rottable.Instance> rotUpdaters;

		private List<Pickup> pickupsWhichCanBePickedUp = new List<Pickup>();

		private Dictionary<int, int> cellCosts = new Dictionary<int, int>();

		public Tag prefabId
		{
			get;
			private set;
		}

		public FecthablesByPrefabId(Tag prefab_id)
		{
			prefabId = prefab_id;
			fetchables = new KCompactedVector<Fetchable>(0);
			rotUpdaters = new Dictionary<HandleVector<int>.Handle, Rottable.Instance>();
			finalPickups = new List<Pickup>();
		}

		public HandleVector<int>.Handle AddPickupable(Pickupable pickupable)
		{
			DebugUtil.DevAssert(true, "Assert!", string.Empty, string.Empty);
			byte b = 5;
			Edible component = pickupable.GetComponent<Edible>();
			if ((Object)component != (Object)null)
			{
				b = (byte)component.GetQuality();
				DebugUtil.DevAssert(b == b, "Assert!", string.Empty, string.Empty);
			}
			byte masterPriority = 0;
			Prioritizable prioritizable = null;
			if ((Object)pickupable.storage != (Object)null)
			{
				prioritizable = pickupable.storage.prioritizable;
				if ((Object)prioritizable != (Object)null)
				{
					PrioritySetting masterPriority2 = prioritizable.GetMasterPriority();
					masterPriority = (byte)masterPriority2.priority_value;
				}
			}
			Rottable.Instance sMI = pickupable.GetSMI<Rottable.Instance>();
			byte freshness = 0;
			if (!sMI.IsNullOrStopped())
			{
				freshness = QuantizeRotValue(sMI.RotValue);
			}
			KPrefabID component2 = pickupable.GetComponent<KPrefabID>();
			TagBits tagBits = component2.GetTagBits() & disallowedTagMask;
			HandleVector<int>.Handle handle = fetchables.Allocate(new Fetchable
			{
				pickupable = pickupable,
				foodQuality = b,
				freshness = freshness,
				masterPriority = masterPriority,
				tagBitsHash = tagBits.GetHashCode()
			});
			if (!sMI.IsNullOrStopped())
			{
				rotUpdaters[handle] = sMI;
			}
			return handle;
		}

		public void RemovePickupable(HandleVector<int>.Handle fetchable_handle)
		{
			fetchables.Free(fetchable_handle);
			rotUpdaters.Remove(fetchable_handle);
		}

		public void UpdatePickups(PathProber path_prober, Navigator worker_navigator, GameObject worker_go)
		{
			BeginSample("FetchManagerUpdater.UpdatePickups");
			GatherPickupablesWhichCanBePickedUp(worker_go);
			GatherReachablePickups(worker_navigator);
			BeginSample("SortPickups", finalPickups.Count);
			finalPickups.Sort(ComparerIncludingPriority);
			EndSample();
			if (finalPickups.Count > 0)
			{
				Pickup pickup = finalPickups[0];
				TagBits tag_bits = pickup.pickupable.KPrefabID.GetTagBits() & disallowedTagMask;
				int num = pickup.tagBitsHash;
				BeginSample("CleanupPickups");
				int num2 = finalPickups.Count;
				int num3 = 0;
				for (int i = 1; i < finalPickups.Count; i++)
				{
					bool flag = false;
					Pickup pickup2 = finalPickups[i];
					TagBits tagBits = default(TagBits);
					int tagBitsHash = pickup2.tagBitsHash;
					if (pickup.masterPriority == pickup2.masterPriority)
					{
						if (pickup2.tagBitsHash == num)
						{
							tagBits = (pickup2.pickupable.KPrefabID.GetTagBits() & disallowedTagMask);
							if (tagBits.AreEqual(tag_bits))
							{
								flag = true;
							}
						}
						else
						{
							tagBits = (pickup2.pickupable.KPrefabID.GetTagBits() & disallowedTagMask);
						}
					}
					if (flag)
					{
						num2--;
					}
					else
					{
						num3++;
						pickup = pickup2;
						tag_bits = tagBits;
						num = tagBitsHash;
						if (i > num3)
						{
							finalPickups[num3] = pickup2;
						}
					}
				}
				finalPickups.RemoveRange(num2, finalPickups.Count - num2);
			}
			EndSample();
			EndSample();
		}

		private void GatherPickupablesWhichCanBePickedUp(GameObject worker_go)
		{
			BeginSample("GatherPickupablesWhichCanBePickedUp");
			pickupsWhichCanBePickedUp.Clear();
			foreach (Fetchable data in fetchables.GetDataList())
			{
				Fetchable current = data;
				Pickupable pickupable = current.pickupable;
				if (pickupable.CouldBePickedUpByMinion(worker_go))
				{
					pickupsWhichCanBePickedUp.Add(new Pickup
					{
						pickupable = pickupable,
						tagBitsHash = current.tagBitsHash,
						PathCost = 65535,
						masterPriority = current.masterPriority,
						freshness = current.freshness,
						foodQuality = current.foodQuality
					});
				}
			}
			EndSample();
		}

		public void UpdateOffsetTables()
		{
			foreach (Fetchable data in fetchables.GetDataList())
			{
				Fetchable current = data;
				current.pickupable.GetOffsets(current.pickupable.cachedCell);
			}
		}

		private void GatherReachablePickups(Navigator navigator)
		{
			BeginSample("GatherReachablePickups");
			cellCosts.Clear();
			finalPickups.Clear();
			foreach (Pickup item in pickupsWhichCanBePickedUp)
			{
				Pickup current = item;
				Pickupable pickupable = current.pickupable;
				int value = -1;
				if (!cellCosts.TryGetValue(pickupable.cachedCell, out value))
				{
					value = pickupable.GetNavigationCost(navigator, pickupable.cachedCell);
					cellCosts[pickupable.cachedCell] = value;
				}
				if (value != -1)
				{
					finalPickups.Add(new Pickup
					{
						pickupable = pickupable,
						tagBitsHash = current.tagBitsHash,
						PathCost = (ushort)value,
						masterPriority = current.masterPriority,
						freshness = current.freshness,
						foodQuality = current.foodQuality
					});
				}
			}
			EndSample();
		}

		public void UpdateStorage(HandleVector<int>.Handle fetchable_handle, Storage storage)
		{
			Fetchable data = fetchables.GetData(fetchable_handle);
			byte masterPriority = 0;
			Prioritizable prioritizable = null;
			Pickupable pickupable = data.pickupable;
			if ((Object)pickupable.storage != (Object)null)
			{
				prioritizable = pickupable.storage.prioritizable;
				if ((Object)prioritizable != (Object)null)
				{
					PrioritySetting masterPriority2 = prioritizable.GetMasterPriority();
					masterPriority = (byte)masterPriority2.priority_value;
				}
			}
			data.masterPriority = masterPriority;
			fetchables.SetData(fetchable_handle, data);
		}

		public void UpdateTags(HandleVector<int>.Handle fetchable_handle)
		{
			Fetchable data = fetchables.GetData(fetchable_handle);
			data.tagBitsHash = (data.pickupable.KPrefabID.GetTagBits() & disallowedTagMask).GetHashCode();
			fetchables.SetData(fetchable_handle, data);
		}

		public void Sim1000ms(float dt)
		{
			foreach (KeyValuePair<HandleVector<int>.Handle, Rottable.Instance> rotUpdater in rotUpdaters)
			{
				HandleVector<int>.Handle key = rotUpdater.Key;
				Rottable.Instance value = rotUpdater.Value;
				Fetchable data = fetchables.GetData(key);
				data.freshness = QuantizeRotValue(value.RotValue);
				fetchables.SetData(key, data);
			}
		}

		private static byte QuantizeRotValue(float rot_value)
		{
			return (byte)(4f * rot_value);
		}

		private static void BeginSample(string name)
		{
		}

		private static void BeginSample(string name, int count)
		{
		}

		private static void EndSample()
		{
		}
	}

	private struct UpdatePickupWorkItem : IWorkItem<object>
	{
		public FecthablesByPrefabId fetchablesByPrefabId;

		public PathProber pathProber;

		public Navigator navigator;

		public GameObject worker;

		public void Run(object shared_data)
		{
			fetchablesByPrefabId.UpdatePickups(pathProber, navigator, worker);
		}
	}

	public static readonly TagBits disallowedTagMask = ~new TagBits(new Tag[1]
	{
		GameTags.Preserved
	});

	private static readonly PickupComparerIncludingPriority ComparerIncludingPriority = new PickupComparerIncludingPriority();

	private static readonly PickupComparerNoPriority ComparerNoPriority = new PickupComparerNoPriority();

	private List<Pickup> pickups = new List<Pickup>();

	public Dictionary<Tag, FecthablesByPrefabId> prefabIdToFetchables = new Dictionary<Tag, FecthablesByPrefabId>();

	private WorkItemCollection<UpdatePickupWorkItem, object> updatePickupsWorkItems = new WorkItemCollection<UpdatePickupWorkItem, object>();

	public HandleVector<int>.Handle Add(Pickupable pickupable)
	{
		Tag tag = pickupable.PrefabID();
		FecthablesByPrefabId value = null;
		if (!prefabIdToFetchables.TryGetValue(tag, out value))
		{
			value = new FecthablesByPrefabId(tag);
			prefabIdToFetchables[tag] = value;
		}
		return value.AddPickupable(pickupable);
	}

	public void Remove(Tag prefab_tag, HandleVector<int>.Handle fetchable_handle)
	{
		prefabIdToFetchables[prefab_tag].RemovePickupable(fetchable_handle);
	}

	public void UpdateStorage(Tag prefab_tag, HandleVector<int>.Handle fetchable_handle, Storage storage)
	{
		prefabIdToFetchables[prefab_tag].UpdateStorage(fetchable_handle, storage);
	}

	public void UpdateTags(Tag prefab_tag, HandleVector<int>.Handle fetchable_handle)
	{
		prefabIdToFetchables[prefab_tag].UpdateTags(fetchable_handle);
	}

	public void Sim1000ms(float dt)
	{
		foreach (KeyValuePair<Tag, FecthablesByPrefabId> prefabIdToFetchable in prefabIdToFetchables)
		{
			prefabIdToFetchable.Value.Sim1000ms(dt);
		}
	}

	public void UpdatePickups(PathProber path_prober, Worker worker)
	{
		updatePickupsWorkItems.Reset(null);
		foreach (KeyValuePair<Tag, FecthablesByPrefabId> prefabIdToFetchable in prefabIdToFetchables)
		{
			FecthablesByPrefabId value = prefabIdToFetchable.Value;
			value.UpdateOffsetTables();
			updatePickupsWorkItems.Add(new UpdatePickupWorkItem
			{
				fetchablesByPrefabId = value,
				pathProber = path_prober,
				navigator = worker.GetComponent<Navigator>(),
				worker = worker.gameObject
			});
		}
		OffsetTracker.isExecutingWithinJob = true;
		GlobalJobManager.Run(updatePickupsWorkItems);
		OffsetTracker.isExecutingWithinJob = false;
		pickups.Clear();
		foreach (KeyValuePair<Tag, FecthablesByPrefabId> prefabIdToFetchable2 in prefabIdToFetchables)
		{
			pickups.AddRange(prefabIdToFetchable2.Value.finalPickups);
		}
		pickups.Sort(ComparerNoPriority);
	}

	public static bool IsFetchablePickup(KPrefabID pickup_id, Storage source, float pickup_unreserved_amount, TagBits tag_bits, TagBits required_tags, TagBits forbid_tags, Storage destination)
	{
		if ((Object)pickup_id == (Object)null)
		{
			return false;
		}
		TagBits tagBits = pickup_id.GetTagBits();
		if (!tagBits.HasAny(tag_bits))
		{
			return false;
		}
		if (pickup_unreserved_amount <= 0f)
		{
			return false;
		}
		if (!tagBits.HasAll(required_tags))
		{
			return false;
		}
		if (tagBits.HasAny(forbid_tags))
		{
			return false;
		}
		if ((Object)source != (Object)null)
		{
			if (destination.ShouldOnlyTransferFromLowerPriority)
			{
				int num = 10;
				if ((Object)destination.prioritizable != (Object)null)
				{
					PrioritySetting masterPriority = destination.prioritizable.GetMasterPriority();
					num = masterPriority.priority_value;
				}
				int num2 = 10;
				if ((Object)source.prioritizable != (Object)null)
				{
					PrioritySetting masterPriority2 = source.prioritizable.GetMasterPriority();
					num2 = masterPriority2.priority_value;
				}
				if (num <= num2)
				{
					return false;
				}
			}
			if (destination.storageNetworkID != -1 && destination.storageNetworkID == source.storageNetworkID)
			{
				return false;
			}
		}
		return true;
	}

	public Pickupable FindFetchTarget(Worker worker, Storage destination, TagBits tag_bits, TagBits required_tags, TagBits forbid_tags, float required_amount)
	{
		foreach (Pickup pickup in pickups)
		{
			Pickup current = pickup;
			if (IsFetchablePickup(current.pickupable.KPrefabID, current.pickupable.storage, current.pickupable.UnreservedAmount, tag_bits, required_tags, forbid_tags, destination))
			{
				return current.pickupable;
			}
		}
		return null;
	}

	public Pickupable FindEdibleFetchTarget(Worker worker, Storage destination, TagBits tag_bits, TagBits required_tags, TagBits forbid_tags, float required_amount)
	{
		Pickup pickup = default(Pickup);
		pickup.PathCost = ushort.MaxValue;
		pickup.foodQuality = 0;
		Pickup pickup2 = pickup;
		int num = 2147483647;
		foreach (Pickup pickup3 in pickups)
		{
			Pickup current = pickup3;
			if (IsFetchablePickup(current.pickupable.KPrefabID, current.pickupable.storage, current.pickupable.UnreservedAmount, tag_bits, required_tags, forbid_tags, destination))
			{
				int num2 = current.PathCost + (5 - current.foodQuality) * 50;
				if (num2 < num)
				{
					pickup2 = current;
					num = num2;
				}
			}
		}
		return pickup2.pickupable;
	}
}
