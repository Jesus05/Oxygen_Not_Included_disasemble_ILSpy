using System.Collections.Generic;

internal class ClearableManager
{
	private struct MarkedClearable
	{
		public Clearable clearable;

		public Pickupable pickupable;

		public Prioritizable prioritizable;
	}

	private struct SortedClearable
	{
		public class Comparer : IComparer<SortedClearable>
		{
			public int Compare(SortedClearable a, SortedClearable b)
			{
				int num = b.masterPriority.priority_value - a.masterPriority.priority_value;
				if (num == 0)
				{
					return a.cost - b.cost;
				}
				return num;
			}
		}

		public Pickupable pickupable;

		public PrioritySetting masterPriority;

		public int cost;

		public static Comparer comparer = new Comparer();
	}

	private KCompactedVector<MarkedClearable> markedClearables = new KCompactedVector<MarkedClearable>(0);

	private List<SortedClearable> sortedClearables = new List<SortedClearable>();

	public HandleVector<int>.Handle RegisterClearable(Clearable clearable)
	{
		return markedClearables.Allocate(new MarkedClearable
		{
			clearable = clearable,
			pickupable = clearable.GetComponent<Pickupable>(),
			prioritizable = clearable.GetComponent<Prioritizable>()
		});
	}

	public void UnregisterClearable(HandleVector<int>.Handle handle)
	{
		markedClearables.Free(handle);
	}

	private static void CollectSortedClearables(Navigator navigator, KCompactedVector<MarkedClearable> clearables, List<SortedClearable> sorted_clearables)
	{
		sorted_clearables.Clear();
		foreach (MarkedClearable data in clearables.GetDataList())
		{
			MarkedClearable current = data;
			int navigationCost = current.pickupable.GetNavigationCost(navigator, current.pickupable.cachedCell);
			if (navigationCost != -1)
			{
				sorted_clearables.Add(new SortedClearable
				{
					pickupable = current.pickupable,
					masterPriority = current.prioritizable.GetMasterPriority(),
					cost = navigationCost
				});
			}
		}
		sorted_clearables.Sort(SortedClearable.comparer);
	}

	public void CollectChores(ChoreConsumerState consumer_state, List<Chore.Precondition.Context> succeeded, List<Chore.Precondition.Context> failed_contexts)
	{
		ChoreType transport = Db.Get().ChoreTypes.Transport;
		int personalPriority = consumer_state.consumer.GetPersonalPriority(transport);
		int priority = (!Game.Instance.advancedPersonalPriorities) ? transport.priority : transport.explicitPriority;
		CollectSortedClearables(consumer_state.navigator, markedClearables, sortedClearables);
		bool flag = false;
		foreach (SortedClearable sortedClearable in sortedClearables)
		{
			SortedClearable current = sortedClearable;
			Pickupable pickupable = current.pickupable;
			PrioritySetting masterPriority = current.masterPriority;
			Chore.Precondition.Context item = default(Chore.Precondition.Context);
			item.personalPriority = personalPriority;
			KPrefabID kPrefabID = pickupable.KPrefabID;
			kPrefabID.UpdateTagBits();
			foreach (GlobalChoreProvider.Fetch fetch in GlobalChoreProvider.Instance.fetches)
			{
				GlobalChoreProvider.Fetch current2 = fetch;
				if (kPrefabID.HasAnyTags_AssumeLaundered(ref current2.chore.tagBits))
				{
					item.Set(current2.chore, consumer_state, false, pickupable);
					item.choreTypeForPermission = transport;
					item.RunPreconditions();
					if (item.IsSuccess())
					{
						item.masterPriority = masterPriority;
						item.priority = priority;
						item.interruptPriority = transport.interruptPriority;
						succeeded.Add(item);
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				break;
			}
		}
	}
}
