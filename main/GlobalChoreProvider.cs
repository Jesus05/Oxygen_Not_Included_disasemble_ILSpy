using System.Collections.Generic;
using UnityEngine;

public class GlobalChoreProvider : ChoreProvider
{
	public struct Fetch
	{
		public FetchChore chore;

		public int tagBitsHash;

		public int cost;

		public PrioritySetting priority;

		public Storage.FetchCategory category;

		public bool IsBetterThan(Fetch fetch)
		{
			if (category == fetch.category)
			{
				if (tagBitsHash == fetch.tagBitsHash)
				{
					if (chore.tagBits.AreEqual(ref fetch.chore.tagBits))
					{
						if (priority.priority_class <= fetch.priority.priority_class)
						{
							if (priority.priority_class == fetch.priority.priority_class)
							{
								if (priority.priority_value > fetch.priority.priority_value)
								{
									return true;
								}
								if (priority.priority_value == fetch.priority.priority_value)
								{
									return cost <= fetch.cost;
								}
							}
							return false;
						}
						return true;
					}
					return false;
				}
				return false;
			}
			return false;
		}
	}

	private class FetchComparer : IComparer<Fetch>
	{
		public int Compare(Fetch a, Fetch b)
		{
			int num = b.priority.priority_class - a.priority.priority_class;
			if (num == 0)
			{
				int num2 = b.priority.priority_value - a.priority.priority_value;
				if (num2 == 0)
				{
					return a.cost - b.cost;
				}
				return num2;
			}
			return num;
		}
	}

	public static GlobalChoreProvider Instance;

	public List<FetchChore> fetchChores = new List<FetchChore>();

	public List<Fetch> fetches = new List<Fetch>();

	private static readonly FetchComparer Comparer = new FetchComparer();

	private ClearableManager clearableManager;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		clearableManager = new ClearableManager();
	}

	public override Chore AddChore(Chore chore)
	{
		FetchChore fetchChore = chore as FetchChore;
		if (fetchChore != null)
		{
			fetchChores.Add(fetchChore);
		}
		return base.AddChore(chore);
	}

	public override Chore RemoveChore(Chore chore)
	{
		FetchChore fetchChore = chore as FetchChore;
		if (fetchChore != null)
		{
			fetchChores.Remove(fetchChore);
		}
		return base.RemoveChore(chore);
	}

	public void UpdateFetches(PathProber path_prober)
	{
		fetches.Clear();
		Navigator component = path_prober.GetComponent<Navigator>();
		foreach (FetchChore fetchChore in fetchChores)
		{
			int num = -1;
			if ((Object)fetchChore.destination != (Object)null)
			{
				if ((Object)fetchChore.automatable != (Object)null && fetchChore.automatable.GetAutomationOnly())
				{
					continue;
				}
				num = component.GetNavigationCost(fetchChore.destination);
			}
			if (num != -1 && !((Object)fetchChore.driver != (Object)null))
			{
				fetches.Add(new Fetch
				{
					chore = fetchChore,
					tagBitsHash = fetchChore.tagBitsHash,
					cost = num,
					priority = fetchChore.masterPriority,
					category = fetchChore.destination.fetchCategory
				});
			}
		}
		if (fetches.Count > 0)
		{
			fetches.Sort(Comparer);
			int i = 1;
			int num2 = 0;
			for (; i < fetches.Count; i++)
			{
				if (!fetches[num2].IsBetterThan(fetches[i]))
				{
					num2++;
					fetches[num2] = fetches[i];
				}
			}
			fetches.RemoveRange(num2 + 1, fetches.Count - num2 - 1);
		}
	}

	public override void CollectChores(ChoreConsumerState consumer_state, List<Chore.Precondition.Context> succeeded, List<Chore.Precondition.Context> failed_contexts)
	{
		base.CollectChores(consumer_state, succeeded, failed_contexts);
		clearableManager.CollectChores(consumer_state, succeeded, failed_contexts);
		foreach (Fetch fetch in fetches)
		{
			Fetch current = fetch;
			current.chore.CollectChoresFromGlobalChoreProvider(consumer_state, succeeded, failed_contexts, false);
		}
	}

	public HandleVector<int>.Handle RegisterClearable(Clearable clearable)
	{
		return clearableManager.RegisterClearable(clearable);
	}

	public void UnregisterClearable(HandleVector<int>.Handle handle)
	{
		clearableManager.UnregisterClearable(handle);
	}

	protected override void OnLoadLevel()
	{
		base.OnLoadLevel();
		Instance = null;
	}
}
