using UnityEngine;

public class ElementSpout : StateMachineComponent<ElementSpout.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, ElementSpout, object>.GameInstance
	{
		public StatesInstance(ElementSpout smi)
			: base(smi)
		{
		}

		private bool CanEmitOnCell(int cell, float max_pressure, Element.State expected_state)
		{
			return Grid.Mass[cell] < max_pressure && (Grid.Element[cell].IsState(expected_state) || Grid.Element[cell].IsVacuum);
		}

		public bool CanEmitAnywhere()
		{
			int cell = Grid.PosToCell(base.smi.transform.GetPosition());
			int cell2 = Grid.CellLeft(cell);
			int cell3 = Grid.CellRight(cell);
			int cell4 = Grid.CellAbove(cell);
			Element.State state = base.smi.master.emitter.outputElement.element.state;
			return false || CanEmitOnCell(cell, base.smi.master.maxPressure, state) || CanEmitOnCell(cell2, base.smi.master.maxPressure, state) || CanEmitOnCell(cell3, base.smi.master.maxPressure, state) || CanEmitOnCell(cell4, base.smi.master.maxPressure, state);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, ElementSpout>
	{
		public class Idle : State
		{
			public State unblocked;

			public State blocked;
		}

		public class Emitting : State
		{
			public State unblocked;

			public State blocked;
		}

		public Idle idle;

		public Emitting emit;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			idle.DefaultState(idle.unblocked).Enter(delegate(StatesInstance smi)
			{
				smi.Play("idle", KAnim.PlayMode.Once);
			}).ScheduleGoTo((StatesInstance smi) => smi.master.emissionPollFrequency, emit);
			idle.unblocked.ToggleStatusItem(Db.Get().MiscStatusItems.SpoutPressureBuilding, (object)null).Transition(idle.blocked, (StatesInstance smi) => !smi.CanEmitAnywhere(), UpdateRate.SIM_200ms);
			idle.blocked.ToggleStatusItem(Db.Get().MiscStatusItems.SpoutOverPressure, (object)null).Transition(idle.blocked, (StatesInstance smi) => smi.CanEmitAnywhere(), UpdateRate.SIM_200ms);
			emit.DefaultState(emit.unblocked).Enter(delegate(StatesInstance smi)
			{
				float num = 1f + Random.Range(0f, smi.master.emissionIrregularity);
				float massGenerationRate = smi.master.perEmitAmount / num;
				smi.master.emitter.SetEmitting(true);
				smi.master.emitter.emissionFrequency = 1f;
				smi.master.emitter.outputElement.massGenerationRate = massGenerationRate;
				smi.ScheduleGoTo(num, idle);
			});
			emit.unblocked.ToggleStatusItem(Db.Get().MiscStatusItems.SpoutEmitting, (object)null).Enter(delegate(StatesInstance smi)
			{
				smi.Play("emit", KAnim.PlayMode.Once);
				smi.master.emitter.SetEmitting(true);
			}).Transition(emit.blocked, (StatesInstance smi) => !smi.CanEmitAnywhere(), UpdateRate.SIM_200ms);
			emit.blocked.ToggleStatusItem(Db.Get().MiscStatusItems.SpoutOverPressure, (object)null).Enter(delegate(StatesInstance smi)
			{
				smi.Play("idle", KAnim.PlayMode.Once);
				smi.master.emitter.SetEmitting(false);
			}).Transition(emit.unblocked, (StatesInstance smi) => smi.CanEmitAnywhere(), UpdateRate.SIM_200ms);
		}
	}

	[SerializeField]
	private ElementEmitter emitter;

	[MyCmpAdd]
	private KBatchedAnimController anim;

	public float maxPressure = 1.5f;

	public float emissionPollFrequency = 3f;

	public float emissionIrregularity = 1.5f;

	public float perEmitAmount = 0.5f;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		int cell = Grid.PosToCell(base.transform.GetPosition());
		Grid.Objects[cell, 2] = base.gameObject;
		base.smi.StartSM();
	}

	public void SetEmitter(ElementEmitter emitter)
	{
		this.emitter = emitter;
	}

	public void ConfigureEmissionSettings(float emissionPollFrequency = 3f, float emissionIrregularity = 1.5f, float maxPressure = 1.5f, float perEmitAmount = 0.5f)
	{
		this.maxPressure = maxPressure;
		this.emissionPollFrequency = emissionPollFrequency;
		this.emissionIrregularity = emissionIrregularity;
		this.perEmitAmount = perEmitAmount;
	}
}
