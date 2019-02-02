using STRINGS;
using System;
using UnityEngine;

public class ChoreDriver : StateMachineComponent<ChoreDriver.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, ChoreDriver, object>.GameInstance
	{
		public StatesInstance(ChoreDriver master)
			: base(master)
		{
			ChoreConsumer component = GetComponent<ChoreConsumer>();
			component.choreRulesChanged = (System.Action)Delegate.Combine(component.choreRulesChanged, new System.Action(OnChoreRulesChanged));
		}

		public void BeginChore()
		{
			Chore nextChore = GetNextChore();
			Chore chore = base.smi.sm.currentChore.Set(nextChore, base.smi);
			if (chore != null && chore.IsPreemptable && (UnityEngine.Object)chore.driver != (UnityEngine.Object)null)
			{
				chore.Fail("Preemption!");
			}
			base.smi.sm.nextChore.Set(null, base.smi);
			Chore chore2 = chore;
			chore2.onExit = (Action<Chore>)Delegate.Combine(chore2.onExit, new Action<Chore>(OnChoreExit));
			chore.Begin(base.master.context);
			Trigger(-1988963660, chore);
		}

		public void EndChore(string reason)
		{
			if (GetCurrentChore() != null)
			{
				Chore currentChore = GetCurrentChore();
				base.smi.sm.currentChore.Set(null, base.smi);
				Chore chore = currentChore;
				chore.onExit = (Action<Chore>)Delegate.Remove(chore.onExit, new Action<Chore>(OnChoreExit));
				currentChore.Fail(reason);
				Trigger(1745615042, currentChore);
			}
		}

		private void OnChoreExit(Chore chore)
		{
			base.smi.sm.stop.Trigger(base.smi);
		}

		public Chore GetNextChore()
		{
			return base.smi.sm.nextChore.Get(base.smi);
		}

		public Chore GetCurrentChore()
		{
			return base.smi.sm.currentChore.Get(base.smi);
		}

		private void OnChoreRulesChanged()
		{
			Chore currentChore = GetCurrentChore();
			if (currentChore != null)
			{
				ChoreConsumer component = GetComponent<ChoreConsumer>();
				if (!component.IsPermittedOrEnabled(currentChore.choreType, currentChore))
				{
					EndChore("Permissions changed");
				}
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, ChoreDriver>
	{
		public ObjectParameter<Chore> currentChore;

		public ObjectParameter<Chore> nextChore;

		public Signal stop;

		public State nochore;

		public State haschore;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = nochore;
			saveHistory = true;
			nochore.Update(delegate(StatesInstance smi, float dt)
			{
				if (smi.master.HasTag(GameTags.Minion) && !smi.master.HasTag(GameTags.Dead))
				{
					ReportManager.Instance.ReportValue(ReportManager.ReportType.WorkTime, dt, string.Format(UI.ENDOFDAYREPORT.NOTES.TIME_SPENT, DUPLICANTS.CHORES.THINKING.NAME), smi.master.GetProperName());
				}
			}, UpdateRate.SIM_200ms, false).ParamTransition(nextChore, haschore, (StatesInstance smi, Chore next_chore) => next_chore != null);
			haschore.Enter("BeginChore", delegate(StatesInstance smi)
			{
				smi.BeginChore();
			}).Exit("EndChore", delegate(StatesInstance smi)
			{
				smi.EndChore("ChoreDriver.SignalStop");
			}).OnSignal(stop, nochore);
		}
	}

	[MyCmpAdd]
	private User user;

	private Chore.Precondition.Context context;

	public Chore GetCurrentChore()
	{
		return base.smi.GetCurrentChore();
	}

	public bool HasChore()
	{
		return base.smi.GetCurrentChore() != null;
	}

	public void StopChore()
	{
		base.smi.sm.stop.Trigger(base.smi);
	}

	public void SetChore(Chore.Precondition.Context context)
	{
		Chore currentChore = base.smi.GetCurrentChore();
		if (currentChore != context.chore)
		{
			StopChore();
			if (context.chore.IsValid())
			{
				this.context = context;
				base.smi.sm.nextChore.Set(context.chore, base.smi);
			}
			else
			{
				string text = "Null";
				string text2 = "Null";
				if (currentChore != null)
				{
					text = currentChore.GetType().Name;
				}
				if (context.chore != null)
				{
					text2 = context.chore.GetType().Name;
				}
				string obj = "Stopping chore " + text + " to start " + text2 + " but stopping the first chore cancelled the second one.";
				Debug.LogWarning(obj, null);
			}
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}
}
