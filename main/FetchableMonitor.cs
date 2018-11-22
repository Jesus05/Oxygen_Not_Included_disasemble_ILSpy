using UnityEngine;

public class FetchableMonitor : GameStateMachine<FetchableMonitor, FetchableMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Pickupable pickupable;

		private Equippable equippable;

		public HandleVector<int>.Handle fetchable;

		public Instance(Pickupable pickupable)
			: base((IStateMachineTarget)pickupable)
		{
			this.pickupable = pickupable;
			equippable = GetComponent<Equippable>();
		}

		public void RegisterFetchable()
		{
			Game.Instance.Trigger(-1588644844, base.gameObject);
			fetchable = Game.Instance.fetchManager.Add(pickupable);
		}

		public void UnregisterFetchable()
		{
			Game.Instance.Trigger(-1491270284, base.gameObject);
			Game.Instance.fetchManager.Remove(base.smi.pickupable.PrefabID(), fetchable);
		}

		public void SetForceUnfetchable(bool is_unfetchable)
		{
			base.sm.forceUnfetchable.Set(is_unfetchable, base.smi);
		}

		public bool IsFetchable()
		{
			if (!base.sm.forceUnfetchable.Get(this))
			{
				if (!pickupable.IsEntombed)
				{
					if (pickupable.IsReachable())
					{
						if ((Object)equippable != (Object)null && equippable.isEquipped)
						{
							return false;
						}
						if (!pickupable.HasTag(GameTags.StoredPrivate))
						{
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
	}

	public State fetchable;

	public State unfetchable;

	public BoolParameter forceUnfetchable = new BoolParameter(false);

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = unfetchable;
		base.serializable = false;
		fetchable.Enter("RegisterFetchable", delegate(Instance smi)
		{
			smi.RegisterFetchable();
		}).Exit("UnregisterFetchable", delegate(Instance smi)
		{
			smi.UnregisterFetchable();
		}).EventTransition(GameHashes.ReachableChanged, unfetchable, GameStateMachine<FetchableMonitor, Instance, IStateMachineTarget, object>.Not(IsFetchable))
			.EventTransition(GameHashes.AssigneeChanged, unfetchable, GameStateMachine<FetchableMonitor, Instance, IStateMachineTarget, object>.Not(IsFetchable))
			.EventTransition(GameHashes.EntombedChanged, unfetchable, GameStateMachine<FetchableMonitor, Instance, IStateMachineTarget, object>.Not(IsFetchable))
			.EventTransition(GameHashes.TagsChanged, unfetchable, GameStateMachine<FetchableMonitor, Instance, IStateMachineTarget, object>.Not(IsFetchable))
			.EventHandler(GameHashes.OnStore, UpdateStorage)
			.EventHandler(GameHashes.StoragePriorityChanged, UpdateStorage)
			.EventHandler(GameHashes.TagsChanged, UpdateTags)
			.ParamTransition(forceUnfetchable, unfetchable, (Instance smi, bool p) => !smi.IsFetchable());
		unfetchable.EventTransition(GameHashes.ReachableChanged, fetchable, IsFetchable).EventTransition(GameHashes.AssigneeChanged, fetchable, IsFetchable).EventTransition(GameHashes.EntombedChanged, fetchable, IsFetchable)
			.EventTransition(GameHashes.TagsChanged, fetchable, IsFetchable)
			.ParamTransition(forceUnfetchable, fetchable, IsFetchable);
	}

	private bool IsFetchable(Instance smi, bool param)
	{
		return IsFetchable(smi);
	}

	private bool IsFetchable(Instance smi)
	{
		return smi.IsFetchable();
	}

	private void UpdateStorage(Instance smi, object data)
	{
		Game.Instance.fetchManager.UpdateStorage(smi.pickupable.PrefabID(), smi.fetchable, data as Storage);
	}

	private void UpdateTags(Instance smi, object data)
	{
		Game.Instance.fetchManager.UpdateTags(smi.pickupable.PrefabID(), smi.fetchable);
	}
}
