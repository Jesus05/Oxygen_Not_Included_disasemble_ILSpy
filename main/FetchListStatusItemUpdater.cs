using System.Collections.Generic;
using UnityEngine;

public class FetchListStatusItemUpdater : KMonoBehaviour, IRender1000ms
{
	public static FetchListStatusItemUpdater instance;

	private List<FetchList2> fetchLists = new List<FetchList2>();

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		instance = this;
	}

	public void AddFetchList(FetchList2 fetch_list)
	{
		fetchLists.Add(fetch_list);
	}

	public void RemoveFetchList(FetchList2 fetch_list)
	{
		fetchLists.Remove(fetch_list);
	}

	public void Render1000ms(float dt)
	{
		DictionaryPool<int, ListPool<FetchList2, FetchListStatusItemUpdater>.PooledList, FetchListStatusItemUpdater>.PooledDictionary pooledDictionary = DictionaryPool<int, ListPool<FetchList2, FetchListStatusItemUpdater>.PooledList, FetchListStatusItemUpdater>.Allocate();
		foreach (FetchList2 fetchList in fetchLists)
		{
			if (!((Object)fetchList.Destination == (Object)null))
			{
				ListPool<FetchList2, FetchListStatusItemUpdater>.PooledList value = null;
				int instanceID = fetchList.Destination.GetInstanceID();
				if (!pooledDictionary.TryGetValue(instanceID, out value))
				{
					value = (pooledDictionary[instanceID] = ListPool<FetchList2, FetchListStatusItemUpdater>.Allocate());
				}
				value.Add(fetchList);
			}
		}
		DictionaryPool<Tag, float, FetchListStatusItemUpdater>.PooledDictionary pooledDictionary2 = DictionaryPool<Tag, float, FetchListStatusItemUpdater>.Allocate();
		DictionaryPool<Tag, float, FetchListStatusItemUpdater>.PooledDictionary pooledDictionary3 = DictionaryPool<Tag, float, FetchListStatusItemUpdater>.Allocate();
		foreach (KeyValuePair<int, ListPool<FetchList2, FetchListStatusItemUpdater>.PooledList> item in pooledDictionary)
		{
			ListPool<Tag, FetchListStatusItemUpdater>.PooledList pooledList2 = ListPool<Tag, FetchListStatusItemUpdater>.Allocate();
			Storage destination = item.Value[0].Destination;
			foreach (FetchList2 item2 in item.Value)
			{
				item2.UpdateRemaining();
				Dictionary<Tag, float> remaining = item2.GetRemaining();
				foreach (KeyValuePair<Tag, float> item3 in remaining)
				{
					if (!pooledList2.Contains(item3.Key))
					{
						pooledList2.Add(item3.Key);
					}
				}
			}
			ListPool<Pickupable, FetchListStatusItemUpdater>.PooledList pooledList3 = ListPool<Pickupable, FetchListStatusItemUpdater>.Allocate();
			foreach (GameObject item4 in destination.items)
			{
				if (!((Object)item4 == (Object)null))
				{
					Pickupable component = item4.GetComponent<Pickupable>();
					if (!((Object)component == (Object)null))
					{
						pooledList3.Add(component);
					}
				}
			}
			DictionaryPool<Tag, float, FetchListStatusItemUpdater>.PooledDictionary pooledDictionary4 = DictionaryPool<Tag, float, FetchListStatusItemUpdater>.Allocate();
			foreach (Tag item5 in pooledList2)
			{
				float num = 0f;
				foreach (Pickupable item6 in pooledList3)
				{
					if (item6.KPrefabID.HasTag(item5))
					{
						num += item6.TotalAmount;
					}
				}
				pooledDictionary4[item5] = num;
			}
			foreach (Tag item7 in pooledList2)
			{
				if (!pooledDictionary2.ContainsKey(item7))
				{
					pooledDictionary2[item7] = WorldInventory.Instance.GetTotalAmount(item7);
				}
				if (!pooledDictionary3.ContainsKey(item7))
				{
					pooledDictionary3[item7] = WorldInventory.Instance.GetAmount(item7);
				}
			}
			foreach (FetchList2 item8 in item.Value)
			{
				bool should_add = false;
				bool should_add2 = true;
				bool should_add3 = false;
				Dictionary<Tag, float> remaining2 = item8.GetRemaining();
				foreach (KeyValuePair<Tag, float> item9 in remaining2)
				{
					Tag key = item9.Key;
					float value2 = item9.Value;
					float num2 = pooledDictionary4[key];
					float b = pooledDictionary2[key];
					float num3 = pooledDictionary3[key];
					float num4 = Mathf.Min(value2, b);
					float num5 = num3 + num4;
					float minimumAmount = item8.GetMinimumAmount(key);
					if (num2 + num5 < minimumAmount)
					{
						should_add = true;
					}
					if (num5 < value2)
					{
						should_add2 = false;
					}
					if (num2 + num5 > value2 && value2 > num5)
					{
						should_add3 = true;
					}
				}
				item8.UpdateStatusItem(Db.Get().BuildingStatusItems.WaitingForMaterials, ref item8.waitingForMaterialsHandle, should_add2);
				item8.UpdateStatusItem(Db.Get().BuildingStatusItems.MaterialsUnavailable, ref item8.materialsUnavailableHandle, should_add);
				item8.UpdateStatusItem(Db.Get().BuildingStatusItems.MaterialsUnavailableForRefill, ref item8.materialsUnavailableForRefillHandle, should_add3);
			}
			pooledDictionary4.Recycle();
			pooledList3.Recycle();
			pooledList2.Recycle();
			item.Value.Recycle();
		}
		pooledDictionary3.Recycle();
		pooledDictionary2.Recycle();
		pooledDictionary.Recycle();
	}
}
