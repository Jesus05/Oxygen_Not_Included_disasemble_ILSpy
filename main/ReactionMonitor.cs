using System.Collections.Generic;

public class ReactionMonitor : GameStateMachine<ReactionMonitor, ReactionMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		private bool justReacted;

		private Reactable lastReactable;

		private Dictionary<HashedString, float> lastReactTimes;

		private List<Reactable> oneshotReactables;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			lastReactTimes = new Dictionary<HashedString, float>();
			oneshotReactables = new List<Reactable>();
		}

		public void PollForReactables(Navigator.ActiveTransition transition)
		{
			if (!IsReacting())
			{
				if (justReacted)
				{
					justReacted = false;
				}
				else
				{
					lastReactable = null;
				}
				for (int num = oneshotReactables.Count - 1; num >= 0; num--)
				{
					Reactable reactable = oneshotReactables[num];
					if (reactable.IsExpired())
					{
						reactable.Cleanup();
					}
				}
				int cell = Grid.PosToCell(base.smi.gameObject);
				ListPool<ScenePartitionerEntry, ReactionMonitor>.PooledList pooledList = ListPool<ScenePartitionerEntry, ReactionMonitor>.Allocate();
				GameScenePartitioner instance = GameScenePartitioner.Instance;
				Vector2I vector2I = Grid.CellToXY(cell);
				int x = vector2I.x;
				Vector2I vector2I2 = Grid.CellToXY(cell);
				instance.GatherEntries(x, vector2I2.y, 1, 1, GameScenePartitioner.Instance.objectLayers[0], pooledList);
				for (int i = 0; i < pooledList.Count; i++)
				{
					Reactable reactable2 = pooledList[i].obj as Reactable;
					if (reactable2 != null && reactable2 != lastReactable && (!lastReactTimes.ContainsKey(reactable2.id) || !(GameClock.Instance.GetTime() - lastReactTimes[reactable2.id] < reactable2.minReactorTime)) && reactable2.CanBegin(base.gameObject, transition))
					{
						justReacted = true;
						lastReactable = reactable2;
						lastReactTimes[reactable2.id] = GameClock.Instance.GetTime();
						base.sm.reactable.Set(reactable2, base.smi);
						base.smi.GoTo(base.sm.reacting);
						break;
					}
				}
				pooledList.Recycle();
			}
		}

		public void StopReaction()
		{
			for (int num = oneshotReactables.Count - 1; num >= 0; num--)
			{
				if (base.sm.reactable.Get(base.smi) == oneshotReactables[num])
				{
					oneshotReactables[num].Cleanup();
					oneshotReactables.RemoveAt(num);
				}
			}
			base.smi.GoTo(base.sm.idle);
		}

		public bool IsReacting()
		{
			return base.smi.IsInsideState(base.sm.reacting);
		}

		public void AddOneshotReactable(SelfEmoteReactable reactable)
		{
			oneshotReactables.Add(reactable);
		}

		public void CancelOneShotReactable(SelfEmoteReactable cancel_target)
		{
			int num = oneshotReactables.Count - 1;
			Reactable reactable;
			while (true)
			{
				if (num < 0)
				{
					return;
				}
				reactable = oneshotReactables[num];
				if (cancel_target == reactable)
				{
					break;
				}
				num--;
			}
			reactable.Cleanup();
		}
	}

	public State idle;

	public State reacting;

	public State dead;

	public ObjectParameter<Reactable> reactable;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = idle;
		base.serializable = false;
		idle.Enter("ClearReactable", delegate(Instance smi)
		{
			reactable.Set(null, smi);
		}).TagTransition(GameTags.Dead, dead, false);
		reacting.Enter("Reactable.Begin", delegate(Instance smi)
		{
			reactable.Get(smi).Begin(smi.gameObject);
		}).Update("Reactable.Update", delegate(Instance smi, float dt)
		{
			reactable.Get(smi).Update(dt);
		}, UpdateRate.SIM_200ms, false).Exit("Reactable.End", delegate(Instance smi)
		{
			reactable.Get(smi).End();
		})
			.EventTransition(GameHashes.NavigationFailed, idle, null)
			.Enter("Reactable.AddChorePreventionTag", delegate(Instance smi)
			{
				if (reactable.Get(smi).preventChoreInterruption)
				{
					smi.GetComponent<KPrefabID>().AddTag(GameTags.PreventChoreInterruption);
				}
			})
			.Exit("Reactable.RemoveChorePreventionTag", delegate(Instance smi)
			{
				if (reactable.Get(smi).preventChoreInterruption)
				{
					smi.GetComponent<KPrefabID>().RemoveTag(GameTags.PreventChoreInterruption);
				}
			})
			.TagTransition(GameTags.Dying, dead, false)
			.TagTransition(GameTags.Dead, dead, false);
		dead.DoNothing();
	}
}
