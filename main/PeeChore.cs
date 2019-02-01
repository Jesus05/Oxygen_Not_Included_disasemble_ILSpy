using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PeeChore : Chore<PeeChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, PeeChore, object>.GameInstance
	{
		public Notification stressfullyEmptyingBladder = new Notification(DUPLICANTS.STATUSITEMS.STRESSFULLYEMPTYINGBLADDER.NOTIFICATION_NAME, NotificationType.Bad, HashedString.Invalid, (List<Notification> notificationList, object data) => DUPLICANTS.STATUSITEMS.STRESSFULLYEMPTYINGBLADDER.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false), null, true, 0f, null, null);

		public AmountInstance bladder;

		private AmountInstance bodyTemperature;

		public StatesInstance(PeeChore master, GameObject worker)
			: base(master)
		{
			bladder = Db.Get().Amounts.Bladder.Lookup(worker);
			bodyTemperature = Db.Get().Amounts.Temperature.Lookup(worker);
			base.sm.worker.Set(worker, base.smi);
		}

		public bool IsDonePeeing()
		{
			return bladder.value <= 0f;
		}

		public void SpawnDirtyWater(float dt)
		{
			int gameCell = Grid.PosToCell(base.sm.worker.Get<KMonoBehaviour>(base.smi));
			byte index = Db.Get().Diseases.GetIndex("FoodPoisoning");
			float num = dt * (0f - bladder.GetDelta()) / bladder.GetMax();
			if (num > 0f)
			{
				SimMessages.AddRemoveSubstance(gameCell, SimHashes.DirtyWater, CellEventLogger.Instance.Vomit, 2f * num, bodyTemperature.value, index, Mathf.CeilToInt(100000f * num), true, -1);
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, PeeChore>
	{
		public TargetParameter worker;

		public State running;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = running;
			Target(worker);
			running.ToggleAnims("anim_expel_kanim", 0f).ToggleEffect("StressfulyEmptyingBladder").DoNotification((StatesInstance smi) => smi.stressfullyEmptyingBladder)
				.DoReport(ReportManager.ReportType.ToiletIncident, (StatesInstance smi) => 1f, (StatesInstance smi) => masterTarget.Get(smi).GetProperName())
				.DoTutorial(Tutorial.TutorialMessages.TM_Mopping)
				.Transition(null, (StatesInstance smi) => smi.IsDonePeeing(), UpdateRate.SIM_200ms)
				.Update("SpawnDirtyWater", delegate(StatesInstance smi, float dt)
				{
					smi.SpawnDirtyWater(dt);
				}, UpdateRate.SIM_200ms, false)
				.PlayAnim("working_loop", KAnim.PlayMode.Loop);
		}
	}

	public PeeChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.Pee, target, target.GetComponent<ChoreProvider>(), false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, false, true, 0, (Tag[])null, false)
	{
		smi = new StatesInstance(this, target.gameObject);
	}
}
