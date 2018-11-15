using UnityEngine;

internal class DrowningStates : GameStateMachine<DrowningStates, DrowningStates.Instance, IStateMachineTarget, DrowningStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.HasTag, GameTags.Creatures.Drowning);
		}
	}

	public class EscapeCellQuery : PathFinderQuery
	{
		private DrowningMonitor monitor;

		public EscapeCellQuery(DrowningMonitor monitor)
		{
			this.monitor = monitor;
		}

		public override bool IsMatch(int cell, int parent_cell, int cost)
		{
			return monitor.IsCellSafe(cell);
		}
	}

	public State drowning;

	public State escape;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = drowning;
		root.ToggleStatusItem(Db.Get().CreatureStatusItems.Drowning, (object)null).TagTransition(GameTags.Creatures.Drowning, null, true);
		drowning.PlayAnim("harvest", KAnim.PlayMode.Loop).ToggleScheduleCallback("IdleMove", (Instance smi) => (float)Random.Range(1, 3), delegate(Instance smi)
		{
			smi.GoTo(escape);
		});
		escape.Enter(MoveToSafeCell).EventTransition(GameHashes.DestinationReached, drowning, null).EventTransition(GameHashes.NavigationFailed, drowning, null);
	}

	public void MoveToSafeCell(Instance smi)
	{
		Navigator component = smi.GetComponent<Navigator>();
		DrowningMonitor component2 = smi.GetComponent<DrowningMonitor>();
		EscapeCellQuery escapeCellQuery = new EscapeCellQuery(component2);
		component.RunQuery(escapeCellQuery);
		component.GoTo(escapeCellQuery.GetResultCell(), null);
	}
}
