using STRINGS;
using System;
using TUNING;
using UnityEngine;

public class MingleChore : Chore<MingleChore.StatesInstance>, IWorkerPrioritizable
{
	public class States : GameStateMachine<States, StatesInstance, MingleChore>
	{
		public TargetParameter mingler;

		public State mingle;

		public State move;

		public State walk;

		public State onfloor;

		public State success;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = mingle;
			Target(mingler);
			root.EventTransition(GameHashes.ScheduleBlocksChanged, null, (StatesInstance smi) => !smi.IsRecTime());
			mingle.Transition(walk, (StatesInstance smi) => smi.IsSameRoom(), UpdateRate.SIM_200ms).Transition(move, (StatesInstance smi) => !smi.IsSameRoom(), UpdateRate.SIM_200ms);
			move.Transition(null, (StatesInstance smi) => !smi.HasMingleCell(), UpdateRate.SIM_200ms).MoveTo((StatesInstance smi) => smi.GetMingleCell(), onfloor, null, false);
			walk.Transition(null, (StatesInstance smi) => !smi.HasMingleCell(), UpdateRate.SIM_200ms).TriggerOnEnter(GameHashes.BeginWalk, null).TriggerOnExit(GameHashes.EndWalk)
				.ToggleAnims("anim_loco_walk_kanim", 0f)
				.MoveTo((StatesInstance smi) => smi.GetMingleCell(), onfloor, null, false);
			onfloor.ToggleAnims("anim_generic_convo_kanim", 0f).PlayAnim("idle", KAnim.PlayMode.Loop).ScheduleGoTo((StatesInstance smi) => (float)UnityEngine.Random.Range(5, 10), success)
				.ToggleTag(GameTags.AlwaysConverse);
			success.ReturnSuccess();
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, MingleChore, object>.GameInstance
	{
		private MingleCellSensor mingleCellSensor;

		private GameObject mingler;

		public StatesInstance(MingleChore master, GameObject mingler)
			: base(master)
		{
			this.mingler = mingler;
			base.sm.mingler.Set(mingler, base.smi);
			mingleCellSensor = GetComponent<Sensors>().GetSensor<MingleCellSensor>();
		}

		public bool IsRecTime()
		{
			Schedulable component = base.master.GetComponent<Schedulable>();
			return component.IsAllowed(Db.Get().ScheduleBlockTypes.Recreation);
		}

		public int GetMingleCell()
		{
			return mingleCellSensor.GetCell();
		}

		public bool HasMingleCell()
		{
			return mingleCellSensor.GetCell() != Grid.InvalidCell;
		}

		public bool IsSameRoom()
		{
			int cell = Grid.PosToCell(mingler);
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(cell);
			CavityInfo cavityForCell2 = Game.Instance.roomProber.GetCavityForCell(GetMingleCell());
			if (cavityForCell != null && cavityForCell2 != null)
			{
				return cavityForCell.handle == cavityForCell2.handle;
			}
			return false;
		}
	}

	private int basePriority = RELAXATION.PRIORITY.TIER1;

	private Precondition HasMingleCell = new Precondition
	{
		id = "HasMingleCell",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.HAS_MINGLE_CELL,
		fn = (PreconditionFn)delegate(ref Precondition.Context context, object data)
		{
			MingleChore mingleChore = (MingleChore)data;
			return mingleChore.smi.HasMingleCell();
		}
	};

	public MingleChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.Relax, target, target.GetComponent<ChoreProvider>(), false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.high, 5, false, true, 0, (Tag[])null, false, ReportManager.ReportType.PersonalTime)
	{
		showAvailabilityInHoverText = false;
		base.smi = new StatesInstance(this, target.gameObject);
		AddPrecondition(HasMingleCell, this);
		AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
		AddPrecondition(ChorePreconditions.instance.IsScheduledTime, Db.Get().ScheduleBlockTypes.Recreation);
		AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, this);
	}

	protected override StatusItem GetStatusItem()
	{
		return Db.Get().DuplicantStatusItems.Mingling;
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = basePriority;
		return true;
	}
}
