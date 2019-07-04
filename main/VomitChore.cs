using Klei;
using Klei.AI;
using System;
using TUNING;
using UnityEngine;

public class VomitChore : Chore<VomitChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, VomitChore, object>.GameInstance
	{
		public StatusItem statusItem;

		private AmountInstance bodyTemperature;

		public Notification notification;

		private SafetyQuery vomitCellQuery;

		public StatesInstance(VomitChore master, GameObject vomiter, StatusItem status_item, Notification notification)
			: base(master)
		{
			base.sm.vomiter.Set(vomiter, base.smi);
			bodyTemperature = Db.Get().Amounts.Temperature.Lookup(vomiter);
			statusItem = status_item;
			this.notification = notification;
			vomitCellQuery = new SafetyQuery(Game.Instance.safetyConditions.VomitCellChecker, GetComponent<KMonoBehaviour>(), 10);
		}

		private static bool CanEmitLiquid(int cell)
		{
			bool result = true;
			if (Grid.Solid[cell] || (Grid.Properties[cell] & 2) != 0)
			{
				result = false;
			}
			return result;
		}

		public void SpawnDirtyWater(float dt)
		{
			if (dt > 0f)
			{
				float totalTime = GetComponent<KBatchedAnimController>().CurrentAnim.totalTime;
				float num = dt / totalTime;
				Sicknesses sicknesses = base.master.GetComponent<MinionModifiers>().sicknesses;
				SimUtil.DiseaseInfo invalid = SimUtil.DiseaseInfo.Invalid;
				for (int i = 0; i < sicknesses.Count; i++)
				{
					SicknessInstance sicknessInstance = sicknesses[i];
					if (sicknessInstance.modifier.sicknessType == Sickness.SicknessType.Pathogen)
					{
						break;
					}
				}
				Facing component = base.sm.vomiter.Get(base.smi).GetComponent<Facing>();
				int num2 = Grid.PosToCell(component.transform.GetPosition());
				int frontCell = component.GetFrontCell();
				int num3 = frontCell;
				if (!CanEmitLiquid(num3))
				{
					num3 = num2;
				}
				Equippable equippable = GetComponent<SuitEquipper>().IsWearingAirtightSuit();
				if ((UnityEngine.Object)equippable != (UnityEngine.Object)null)
				{
					equippable.GetComponent<Storage>().AddLiquid(SimHashes.DirtyWater, STRESS.VOMIT_AMOUNT * num, bodyTemperature.value, invalid.idx, invalid.count, false, true);
				}
				else
				{
					SimMessages.AddRemoveSubstance(num3, SimHashes.DirtyWater, CellEventLogger.Instance.Vomit, STRESS.VOMIT_AMOUNT * num, bodyTemperature.value, invalid.idx, invalid.count, true, -1);
				}
			}
		}

		public int GetVomitCell()
		{
			vomitCellQuery.Reset();
			Navigator component = GetComponent<Navigator>();
			component.RunQuery(vomitCellQuery);
			int num = vomitCellQuery.GetResultCell();
			if (Grid.InvalidCell == num)
			{
				num = Grid.PosToCell(component);
			}
			return num;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, VomitChore>
	{
		public class VomitState : State
		{
			public State buildup;

			public State release;

			public State release_pst;
		}

		public TargetParameter vomiter;

		public State moveto;

		public VomitState vomit;

		public State recover;

		public State recover_pst;

		public State complete;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = moveto;
			Target(vomiter);
			root.ToggleAnims("anim_emotes_default_kanim", 0f);
			moveto.TriggerOnEnter(GameHashes.BeginWalk, null).TriggerOnExit(GameHashes.EndWalk).ToggleAnims("anim_loco_vomiter_kanim", 0f)
				.MoveTo((StatesInstance smi) => smi.GetVomitCell(), vomit, vomit, false);
			vomit.DefaultState(vomit.buildup).ToggleAnims("anim_vomit_kanim", 0f).ToggleStatusItem((StatesInstance smi) => smi.statusItem, null)
				.DoNotification((StatesInstance smi) => smi.notification)
				.DoTutorial(Tutorial.TutorialMessages.TM_Mopping);
			vomit.buildup.PlayAnim("vomit_pre", KAnim.PlayMode.Once).OnAnimQueueComplete(vomit.release);
			vomit.release.ToggleEffect("Vomiting").PlayAnim("vomit_loop", KAnim.PlayMode.Once).Update("SpawnDirtyWater", delegate(StatesInstance smi, float dt)
			{
				smi.SpawnDirtyWater(dt);
			}, UpdateRate.SIM_200ms, false)
				.OnAnimQueueComplete(vomit.release_pst);
			vomit.release_pst.PlayAnim("vomit_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(recover);
			recover.PlayAnim("breathe_pre").QueueAnim("breathe_loop", true, null).ScheduleGoTo(8f, recover_pst);
			recover_pst.QueueAnim("breathe_pst", false, null).OnAnimQueueComplete(complete);
			complete.ReturnSuccess();
		}
	}

	public VomitChore(ChoreType chore_type, IStateMachineTarget target, StatusItem status_item, Notification notification, Action<Chore> on_complete = null)
		: base(Db.Get().ChoreTypes.Vomit, target, target.GetComponent<ChoreProvider>(), true, on_complete, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this, target.gameObject, status_item, notification);
	}
}
