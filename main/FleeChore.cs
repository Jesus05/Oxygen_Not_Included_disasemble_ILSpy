using System;
using System.Collections.Generic;
using UnityEngine;

public class FleeChore : Chore<FleeChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, FleeChore, object>.GameInstance
	{
		public StatesInstance(FleeChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, FleeChore>
	{
		public TargetParameter fleeFromTarget;

		public TargetParameter fleeToTarget;

		public TargetParameter self;

		public State planFleeRoute;

		public ApproachSubState<IApproachable> flee;

		public State cower;

		public State end;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = planFleeRoute;
			root.ToggleStatusItem(Db.Get().DuplicantStatusItems.Fleeing, null);
			planFleeRoute.Enter(delegate(StatesInstance smi)
			{
				int num = -1;
				int num2 = Grid.PosToCell(fleeFromTarget.Get(smi));
				HashSet<int> hashSet = GameUtil.FloodCollectCells(Grid.PosToCell(smi.master.gameObject), smi.master.CanFleeTo, 300, null, true);
				int num3 = -1;
				int num4 = -1;
				foreach (int item in hashSet)
				{
					if (smi.master.nav.CanReach(item))
					{
						int num5 = -1;
						num5 += Grid.GetCellDistance(item, num2);
						if (smi.master.isInFavoredDirection(item, num2))
						{
							num5 += 8;
						}
						if (num5 > num4)
						{
							num4 = num5;
							num3 = item;
						}
					}
				}
				num = num3;
				if (num != -1)
				{
					smi.sm.fleeToTarget.Set(smi.master.CreateLocator(Grid.CellToPos(num)), smi);
					smi.sm.fleeToTarget.Get(smi).name = "FleeLocator";
					if (num == num2)
					{
						smi.GoTo(cower);
					}
					else
					{
						smi.GoTo(flee);
					}
				}
				else
				{
					smi.GoTo(cower);
				}
			});
			flee.InitializeStates(self, fleeToTarget, cower, cower, null, NavigationTactics.ReduceTravelDistance).ToggleAnims("anim_loco_run_insane_kanim", 2f);
			cower.ToggleAnims("anim_cringe_kanim", 4f).PlayAnim("cringe_pre").QueueAnim("cringe_loop", false, null)
				.QueueAnim("cringe_pst", false, null)
				.OnAnimQueueComplete(end);
			end.Enter(delegate(StatesInstance smi)
			{
				smi.StopSM("stopped");
			});
		}
	}

	private Navigator nav;

	public FleeChore(IStateMachineTarget target, GameObject enemy)
		: base(Db.Get().ChoreTypes.Flee, target, target.GetComponent<ChoreProvider>(), false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, false, true, 0, (Tag[])null, false, ReportManager.ReportType.WorkTime)
	{
		smi = new StatesInstance(this);
		smi.sm.self.Set(gameObject, smi);
		nav = gameObject.GetComponent<Navigator>();
		smi.sm.fleeFromTarget.Set(enemy, smi);
	}

	private bool isInFavoredDirection(int cell, int fleeFromCell)
	{
		Vector3 vector = Grid.CellToPos(fleeFromCell);
		float x = vector.x;
		Vector3 position = gameObject.transform.GetPosition();
		bool flag = (x < position.x) ? true : false;
		Vector3 vector2 = Grid.CellToPos(fleeFromCell);
		float x2 = vector2.x;
		Vector3 vector3 = Grid.CellToPos(cell);
		bool flag2 = (x2 < vector3.x) ? true : false;
		return flag == flag2;
	}

	private bool CanFleeTo(int cell)
	{
		return nav.CanReach(cell) || nav.CanReach(Grid.OffsetCell(cell, -1, -1)) || nav.CanReach(Grid.OffsetCell(cell, 1, -1)) || nav.CanReach(Grid.OffsetCell(cell, -1, 1)) || nav.CanReach(Grid.OffsetCell(cell, 1, 1));
	}

	public GameObject CreateLocator(Vector3 pos)
	{
		return ChoreHelpers.CreateLocator("GoToLocator", pos);
	}

	protected override void OnStateMachineStop(string reason, StateMachine.Status status)
	{
		if ((UnityEngine.Object)smi.sm.fleeToTarget.Get(smi) != (UnityEngine.Object)null)
		{
			ChoreHelpers.DestroyLocator(smi.sm.fleeToTarget.Get(smi));
		}
		base.OnStateMachineStop(reason, status);
	}
}
