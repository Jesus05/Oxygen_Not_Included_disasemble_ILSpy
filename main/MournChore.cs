using Klei.AI;
using STRINGS;
using System;
using System.Collections;
using UnityEngine;

public class MournChore : Chore<MournChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, MournChore, object>.GameInstance
	{
		private int locatorCell = -1;

		public StatesInstance(MournChore master)
			: base(master)
		{
		}

		public void CreateLocator()
		{
			Grave grave = FindGraveToMournAt();
			int cell = Grid.PosToCell(grave.transform.GetPosition());
			Navigator component = base.master.GetComponent<Navigator>();
			int standableCell = GetStandableCell(cell, component);
			if (standableCell < 0)
			{
				base.smi.GoTo((StateMachine.BaseState)null);
			}
			else
			{
				Grid.Reserved[standableCell] = true;
				Vector3 pos = Grid.CellToPosCBC(standableCell, Grid.SceneLayer.Move);
				GameObject value = ChoreHelpers.CreateLocator("MournLocator", pos);
				base.smi.sm.locator.Set(value, base.smi);
				locatorCell = standableCell;
				base.smi.GoTo(base.sm.moveto);
			}
		}

		public void DestroyLocator()
		{
			if (locatorCell >= 0)
			{
				Grid.Reserved[locatorCell] = false;
				ChoreHelpers.DestroyLocator(base.sm.locator.Get(this));
				base.sm.locator.Set(null, this);
				locatorCell = -1;
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, MournChore>
	{
		public TargetParameter mourner;

		public TargetParameter locator;

		public State findOffset;

		public ApproachSubState<IApproachable> moveto;

		public State mourn;

		public State completed;

		private static readonly HashedString[] WORK_ANIMS = new HashedString[2]
		{
			"working_pre",
			"working_loop"
		};

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = findOffset;
			Target(mourner);
			root.ToggleAnims("anim_react_mourning_kanim", 0f).Exit("DestroyLocator", delegate(StatesInstance smi)
			{
				smi.DestroyLocator();
			});
			findOffset.Enter("CreateLocator", delegate(StatesInstance smi)
			{
				smi.CreateLocator();
			});
			moveto.InitializeStates(mourner, locator, mourn, null, null, null);
			mourn.PlayAnims((StatesInstance smi) => WORK_ANIMS, KAnim.PlayMode.Loop).ScheduleGoTo(10f, completed);
			completed.PlayAnim("working_pst").OnAnimQueueComplete(null).Exit(delegate(StatesInstance smi)
			{
				mourner.Get<Effects>(smi).Remove(Db.Get().effects.Get("Mourning"));
			});
		}
	}

	private static readonly CellOffset[] ValidStandingOffsets = new CellOffset[3]
	{
		new CellOffset(0, 0),
		new CellOffset(-1, 0),
		new CellOffset(1, 0)
	};

	private static readonly Precondition HasValidMournLocation = new Precondition
	{
		id = "HasPlaceToStand",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.HAS_PLACE_TO_STAND,
		fn = (PreconditionFn)delegate(ref Precondition.Context context, object data)
		{
			IStateMachineTarget stateMachineTarget = (IStateMachineTarget)data;
			Navigator component = stateMachineTarget.GetComponent<Navigator>();
			bool result = false;
			Grave grave = FindGraveToMournAt();
			if ((UnityEngine.Object)grave != (UnityEngine.Object)null)
			{
				int cell = Grid.PosToCell(grave);
				int standableCell = GetStandableCell(cell, component);
				if (Grid.IsValidCell(standableCell))
				{
					result = true;
				}
			}
			return result;
		}
	};

	public MournChore(IStateMachineTarget master)
		: base(Db.Get().ChoreTypes.Mourn, master, master.GetComponent<ChoreProvider>(), false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.high, 5, false, true, 0, (Tag[])null, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this);
		AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
		AddPrecondition(ChorePreconditions.instance.NoDeadBodies, null);
		AddPrecondition(HasValidMournLocation, master);
	}

	private static int GetStandableCell(int cell, Navigator navigator)
	{
		CellOffset[] validStandingOffsets = ValidStandingOffsets;
		foreach (CellOffset offset in validStandingOffsets)
		{
			if (Grid.IsCellOffsetValid(cell, offset))
			{
				int num = Grid.OffsetCell(cell, offset);
				if (!Grid.Reserved[num] && navigator.NavGrid.NavTable.IsValid(num, NavType.Floor) && navigator.GetNavigationCost(num) != -1)
				{
					return num;
				}
			}
		}
		return -1;
	}

	public static Grave FindGraveToMournAt()
	{
		Grave result = null;
		float num = -1f;
		IEnumerator enumerator = Components.Graves.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Grave grave = (Grave)enumerator.Current;
				if (grave.burialTime > num)
				{
					num = grave.burialTime;
					result = grave;
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		return result;
	}

	public override void Begin(Precondition.Context context)
	{
		if ((UnityEngine.Object)context.consumerState.consumer == (UnityEngine.Object)null)
		{
			Debug.LogError("MournChore null context.consumer", null);
		}
		else if (base.smi == null)
		{
			Debug.LogError("MournChore null smi", null);
		}
		else if (base.smi.sm == null)
		{
			Debug.LogError("MournChore null smi.sm", null);
		}
		else
		{
			Grave x = FindGraveToMournAt();
			if ((UnityEngine.Object)x == (UnityEngine.Object)null)
			{
				Debug.LogError("MournChore no grave", null);
			}
			else
			{
				base.smi.sm.mourner.Set(context.consumerState.gameObject, base.smi);
				base.Begin(context);
			}
		}
	}
}
