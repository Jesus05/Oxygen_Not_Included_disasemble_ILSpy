using Klei.AI;
using UnityEngine;

public class BladderMonitor : GameStateMachine<BladderMonitor, BladderMonitor.Instance>
{
	public class WantsToPeeStates : State
	{
		public State wanting;

		public State peeing;

		public State InitializeStates(State donePeeingState)
		{
			DefaultState(wanting).ToggleUrge(Db.Get().Urges.Pee).ToggleStateMachine((Instance smi) => new ToiletMonitor.Instance(smi.master));
			wanting.EventTransition(GameHashes.BeginChore, peeing, (Instance smi) => smi.IsPeeing());
			peeing.EventTransition(GameHashes.EndChore, donePeeingState, (Instance smi) => !smi.IsPeeing());
			return this;
		}
	}

	public new class Instance : GameInstance
	{
		private AmountInstance bladder;

		private ChoreDriver choreDriver;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			bladder = Db.Get().Amounts.Bladder.Lookup(master.gameObject);
			choreDriver = GetComponent<ChoreDriver>();
		}

		public bool NeedsToPee()
		{
			if (base.smi == null)
			{
				Debug.LogWarning("How can my state machine instance be null?", null);
				return false;
			}
			if ((Object)base.smi.master.gameObject == (Object)null)
			{
				Debug.LogWarning("How is my gameObject null?", null);
				return false;
			}
			KPrefabID component = base.master.GetComponent<KPrefabID>();
			if (component.HasTag(GameTags.Asleep))
			{
				return false;
			}
			return bladder.value >= 100f;
		}

		public bool WantsToPee()
		{
			return NeedsToPee() || (IsPeeTime() && bladder.value >= 40f);
		}

		public bool IsPeeing()
		{
			return choreDriver.HasChore() && choreDriver.GetCurrentChore().SatisfiesUrge(Db.Get().Urges.Pee);
		}

		public bool IsPeeTime()
		{
			Schedulable component = base.master.GetComponent<Schedulable>();
			return component.IsAllowed(Db.Get().ScheduleBlockTypes.Hygiene);
		}
	}

	public State satisfied;

	public WantsToPeeStates urgentwant;

	public WantsToPeeStates breakwant;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		satisfied.Transition(urgentwant, (Instance smi) => smi.NeedsToPee(), UpdateRate.SIM_200ms).Transition(breakwant, (Instance smi) => smi.WantsToPee(), UpdateRate.SIM_200ms);
		urgentwant.InitializeStates(satisfied).ToggleThought(Db.Get().Thoughts.FullBladder, null).ToggleExpression(Db.Get().Expressions.FullBladder, null)
			.ToggleStateMachine((Instance smi) => new PeeChoreMonitor.Instance(smi.master))
			.ToggleEffect("FullBladder");
		breakwant.InitializeStates(satisfied);
		breakwant.wanting.Transition(urgentwant, (Instance smi) => smi.NeedsToPee(), UpdateRate.SIM_200ms).EventTransition(GameHashes.ScheduleBlocksChanged, satisfied, (Instance smi) => !smi.WantsToPee());
		breakwant.peeing.ToggleThought(Db.Get().Thoughts.BreakBladder, null);
	}
}
