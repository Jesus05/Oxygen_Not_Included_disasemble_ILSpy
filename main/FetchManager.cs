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
			if (num != 0)
			{
				return num;
			}
			if (a.masterPriority == b.masterPriority)
			{
				if (a.PathCost == b.PathCost)
				{
					if (a.foodQuality == b.foodQuality)
					{
						return b.freshness - a.freshness;
					}
					return b.foodQuality - a.foodQuality;
				}
				return a.PathCost - b.PathCost;
			}
			return b.masterPriority - a.masterPriority;
		}
	}

	private class PickupComparerNoPriority : IComparer<Pickup>
	{
		public int Compare(Pickup a, Pickup b)
		{
			if (a.PathCost == b.PathCost)
			{
				if (a.foodQuality == b.foodQuality)
				{
					return b.freshness - a.freshness;
				}
				return b.foodQuality - a.foodQuality;
			}
			return a.PathCost - b.PathCost;
		}
	}

	public class FetchablesByPrefabId
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

		public FetchablesByPrefabId(Tag prefab_id)
		{
			prefabId = prefab_id;
			fetchables = new KCompactedVector<Fetchable>(0);
			rotUpdaters = new Dictionary<HandleVector<int>.Handle, Rottable.Instance>();
			finalPickups = new List<Pickup>();
		}

		public HandleVector<int>.Handle AddPickupable(Pickupable pickupable)
		{
			DebugUtil.DevAssert(true);
			byte b = 5;
			Edible component = pickupable.GetComponent<Edible>();
			if ((Object)component != (Object)null)
			{
				b = (byte)component.GetQuality();
				DebugUtil.DevAssert(b == b);
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
			TagBits rhs = new TagBits(ref disallowedTagMask);
			component2.AndTagBits(ref rhs);
			HandleVector<int>.Handle handle = fetchables.Allocate(new Fetchable
			{
				pickupable = pickupable,
				foodQuality = b,
				freshness = freshness,
				masterPriority = masterPriority,
				tagBitsHash = rhs.GetHashCode()
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
			GatherPickupablesWhichCanBePickedUp(worker_go);
			GatherReachablePickups(worker_navigator);
			finalPickups.Sort(ComparerIncludingPriority);
			if (finalPickups.Count > 0)
			{
				Pickup pickup = finalPickups[0];
				TagBits rhs = new TagBits(ref disallowedTagMask);
				pickup.pickupable.KPrefabID.AndTagBits(ref rhs);
				int num = pickup.tagBitsHash;
				int num2 = finalPickups.Count;
				int num3 = 0;
				for (int i = 1; i < finalPickups.Count; i++)
				{
					bool flag = false;
					Pickup pickup2 = finalPickups[i];
					TagBits rhs2 = default(TagBits);
					int tagBitsHash = pickup2.tagBitsHash;
					if (pickup.masterPriority == pickup2.masterPriority)
					{
						rhs2 = new TagBits(ref disallowedTagMask);
						pickup2.pickupable.KPrefabID.AndTagBits(ref rhs2);
						if (pickup2.tagBitsHash == num && rhs2.AreEqual(ref rhs))
						{
							flag = true;
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
						rhs = rhs2;
						num = tagBitsHash;
						if (i > num3)
						{
							finalPickups[num3] = pickup2;
						}
					}
				}
				finalPickups.RemoveRange(num2, finalPickups.Count - num2);
			}
		}

		private void GatherPickupablesWhichCanBePickedUp(GameObject worker_go)
		{
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
			TagBits rhs = new TagBits(ref disallowedTagMask);
			data.pickupable.KPrefabID.AndTagBits(ref rhs);
			data.tagBitsHash = rhs.GetHashCode();
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
	}

	private struct UpdatePickupWorkItem : IWorkItem<object>
	{
		public FetchablesByPrefabId fetchablesByPrefabId;

		public PathProber pathProber;

		public Navigator navigator;

		public GameObject worker;

		public void Run(object shared_data)
		{
			fetchablesByPrefabId.UpdatePickups(pathProber, navigator, worker);
		}
	}

	public static TagBits disallowedTagBits = new TagBits(GameTags.Preserved);

	public static TagBits disallowedTagMask = TagBits.MakeComplement(ref disallowedTagBits);

	private static readonly PickupComparerIncludingPriority ComparerIncludingPriority = new PickupComparerIncludingPriority();

	private static readonly PickupComparerNoPriority ComparerNoPriority = new PickupComparerNoPriority();

	private List<Pickup> pickups = new List<Pickup>();

	public Dictionary<Tag, FetchablesByPrefabId> prefabIdToFetchables = new Dictionary<Tag, FetchablesByPrefabId>();

	private WorkItemCollection<UpdatePickupWorkItem, object> updatePickupsWorkItems = new WorkItemCollection<UpdatePickupWorkItem, object>();

	[Conditional("ENABLE_FETCH_PROFILING")]
	private static void BeginDetailedSample(string region_name)
	{
	}

	[Conditional("ENABLE_FETCH_PROFILING")]
	private static void BeginDetailedSample(string region_name, int count)
	{
	}

	[Conditional("ENABLE_FETCH_PROFILING")]
	private static void EndDetailedSample()
	{
	}

	public HandleVector<int>.Handle Add(Pickupable pickupable)
	{
		Tag tag = pickupable.PrefabID();
		FetchablesByPrefabId value = null;
		if (!prefabIdToFetchables.TryGetValue(tag, out value))
		{
			value = new FetchablesByPrefabId(tag);
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
		foreach (KeyValuePair<Tag, FetchablesByPrefabId> prefabIdToFetchable in prefabIdToFetchables)
		{
			prefabIdToFetchable.Value.Sim1000ms(dt);
		}
	}

	public void UpdatePickups(PathProber path_prober, Worker worker)
	{
		updatePickupsWorkItems.Reset(null);
		foreach (KeyValuePair<Tag, FetchablesByPrefabId> prefabIdToFetchable in prefabIdToFetchables)
		{
			FetchablesByPrefabId value = prefabIdToFetchable.Value;
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
		foreach (KeyValuePair<Tag, FetchablesByPrefabId> prefabIdToFetchable2 in prefabIdToFetchables)
		{
			pickups.AddRange(prefabIdToFetchable2.Value.finalPickups);
		}
		pickups.Sort(ComparerNoPriority);
	}

	public static bool IsFetchablePickup(KPrefabID pickup_id, Storage source, float pickup_unreserved_amount, ref TagBits tag_bits, ref TagBits required_tags, ref TagBits forbid_tags, Storage destination)
	{
		if (!(pickup_unreserved_amount <= 0f))
		{
			if (!((Object)pickup_id == (Object)null))
			{
				pickup_id.UpdateTagBits();
				if (pickup_id.HasAnyTags_AssumeLaundered(ref tag_bits))
				{
					if (pickup_id.HasAllTags_AssumeLaundered(ref required_tags))
					{
						if (!pickup_id.HasAnyTags_AssumeLaundered(ref forbid_tags))
						{
							if ((Object)source != (Object)null)
							{
								if (destination.ShouldOnlyTransferFromLowerPriority && destination.masterPriority <= source.masterPriority)
								{
									return false;
								}
								if (destination.storageNetworkID != -1 && destination.storageNetworkID == source.storageNetworkID)
								{
									return false;
								}
							}
							return true;
						}
						return false;
					}
					return false;
				}
				return false;
			}
			return false;
		}
		return false;
	}

	public static bool IsFetchablePickup(Pickupable pickupable, ref TagBits tag_bits, ref TagBits required_tags, ref TagBits forbid_tags, Storage destination)
	{
		return IsFetchablePickup(pickupable.KPrefabID, pickupable.storage, pickupable.UnreservedAmount, ref tag_bits, ref required_tags, ref forbid_tags, destination);
	}

	public static Pickupable FindFetchTarget(List<Pickupable> pickupables, Storage destination, ref TagBits tag_bits, ref TagBits required_tags, ref TagBits forbid_tags, float required_amount)
	{
		foreach (Pickupable pickupable in pickupables)
		{
			if (IsFetchablePickup(pickupable, ref tag_bits, ref required_tags, ref forbid_tags, destination))
			{
				return pickupable;
			}
		}
		return null;
	}

	public Pickupable FindFetchTarget(Storage destination, ref TagBits tag_bits, ref TagBits required_tags, ref TagBits forbid_tags, float required_amount)
	{
		foreach (Pickup pickup in pickups)
		{
			Pickup current = pickup;
			if (IsFetchablePickup(current.pickupable, ref tag_bits, ref required_tags, ref forbid_tags, destination))
			{
				return current.pickupable;
			}
		}
		return null;
	}

	public Pickupable FindEdibleFetchTarget(Storage destination, ref TagBits tag_bits, ref TagBits required_tags, ref TagBits forbid_tags, float required_amount)
	{
		Pickup pickup = default(Pickup);
		pickup.PathCost = ushort.MaxValue;
		pickup.foodQuality = 0;
		Pickup pickup2 = pickup;
		int num = 2147483647;
		foreach (Pickup pickup3 in pickups)
		{
			Pickup current = pickup3;
			if (IsFetchablePickup(current.pickupable, ref tag_bits, ref required_tags, ref forbid_tags, destination))
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
