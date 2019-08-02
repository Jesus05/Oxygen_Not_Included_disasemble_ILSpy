using UnityEngine;

public class ClimbableTreeMonitor : GameStateMachine<ClimbableTreeMonitor, ClimbableTreeMonitor.Instance, IStateMachineTarget, ClimbableTreeMonitor.Def>
{
	public class Def : BaseDef
	{
		public float searchMinInterval = 60f;

		public float searchMaxInterval = 120f;
	}

	public new class Instance : GameInstance
	{
		public GameObject climbTarget;

		public float nextSearchTime;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			RefreshSearchTime();
		}

		private void RefreshSearchTime()
		{
			nextSearchTime = Time.time + Mathf.Lerp(base.def.searchMinInterval, base.def.searchMaxInterval, Random.value);
		}

		public bool UpdateHasClimbable()
		{
			if ((Object)climbTarget == (Object)null)
			{
				if (Time.time < nextSearchTime)
				{
					return false;
				}
				FindClimbableTree();
				RefreshSearchTime();
			}
			return (Object)climbTarget != (Object)null;
		}

		private void FindClimbableTree()
		{
			climbTarget = null;
			ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList pooledList = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
			ListPool<KMonoBehaviour, ClimbableTreeMonitor>.PooledList pooledList2 = ListPool<KMonoBehaviour, ClimbableTreeMonitor>.Allocate();
			Vector3 position = base.master.transform.GetPosition();
			Extents extents = new Extents(Grid.PosToCell(position), 10);
			GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.plants, pooledList);
			GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.completeBuildings, pooledList);
			Navigator component = GetComponent<Navigator>();
			foreach (ScenePartitionerEntry item in pooledList)
			{
				KMonoBehaviour kMonoBehaviour = item.obj as KMonoBehaviour;
				if (!kMonoBehaviour.HasTag(GameTags.Creatures.ReservedByCreature))
				{
					int cell = Grid.PosToCell(kMonoBehaviour);
					if (component.CanReach(cell))
					{
						BuddingTrunk component2 = kMonoBehaviour.GetComponent<BuddingTrunk>();
						StorageLocker component3 = kMonoBehaviour.GetComponent<StorageLocker>();
						if ((Object)component2 != (Object)null)
						{
							if (!component2.ExtraSeedAvailable)
							{
								continue;
							}
						}
						else
						{
							if (!((Object)component3 != (Object)null))
							{
								continue;
							}
							Storage component4 = component3.GetComponent<Storage>();
							if (!component4.allowItemRemoval || component4.IsEmpty())
							{
								continue;
							}
						}
						pooledList2.Add(kMonoBehaviour);
					}
				}
			}
			if (pooledList2.Count > 0)
			{
				int index = Random.Range(0, pooledList2.Count);
				KMonoBehaviour kMonoBehaviour2 = pooledList2[index];
				climbTarget = kMonoBehaviour2.gameObject;
			}
			pooledList.Recycle();
			pooledList2.Recycle();
		}

		public void OnClimbComplete()
		{
			climbTarget = null;
		}
	}

	private const int MAX_NAV_COST = int.MaxValue;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.ToggleBehaviour(GameTags.Creatures.WantsToClimbTree, (Instance smi) => smi.UpdateHasClimbable(), delegate(Instance smi)
		{
			smi.OnClimbComplete();
		});
	}
}
