using Klei.AI;
using STRINGS;
using System;
using UnityEngine;

public class SleepChore : Chore<SleepChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, SleepChore, object>.GameInstance
	{
		public bool hadPeacefulSleep = false;

		public bool hadNormalSleep = false;

		public bool hadBadSleep = false;

		public bool hadTerribleSleep = false;

		public int lastEvaluatedDay = -1;

		public float wakeUpBuffer = 2f;

		public string stateChangeNoiseSource;

		private GameObject locator = null;

		public StatesInstance(SleepChore master, GameObject sleeper, GameObject bed, bool bedIsLocator, bool isInterruptable)
			: base(master)
		{
			base.sm.sleeper.Set(sleeper, base.smi);
			base.sm.isInterruptable.Set(isInterruptable, base.smi);
			if (bedIsLocator)
			{
				AddLocator(bed);
			}
			else
			{
				base.sm.bed.Set(bed, base.smi);
			}
		}

		public void CheckLightLevel()
		{
			GameObject go = base.sm.sleeper.Get(base.smi);
			int cell = Grid.PosToCell(go);
			if (Grid.IsValidCell(cell) && !IsLightLevelOk(cell) && !IsLoudSleeper())
			{
				go.Trigger(-1063113160, null);
			}
		}

		public bool IsLoudSleeper()
		{
			GameObject gameObject = base.sm.sleeper.Get(base.smi);
			if (!((UnityEngine.Object)gameObject.GetComponent<Snorer>() != (UnityEngine.Object)null))
			{
				return false;
			}
			return true;
		}

		public void EvaluateSleepQuality()
		{
		}

		public void AddLocator(GameObject sleepable)
		{
			locator = sleepable;
			int i = Grid.PosToCell(locator);
			Grid.Reserved[i] = true;
			base.sm.bed.Set(locator, this);
		}

		public void DestroyLocator()
		{
			if ((UnityEngine.Object)locator != (UnityEngine.Object)null)
			{
				Grid.Reserved[Grid.PosToCell(locator)] = false;
				ChoreHelpers.DestroyLocator(locator);
				base.sm.bed.Set(null, this);
				locator = null;
			}
		}

		public void SetAnim()
		{
			Sleepable sleepable = base.sm.bed.Get<Sleepable>(base.smi);
			if ((UnityEngine.Object)sleepable.GetComponent<Building>() == (UnityEngine.Object)null)
			{
				string s;
				switch (base.sm.sleeper.Get<Navigator>(base.smi).CurrentNavType)
				{
				case NavType.Ladder:
					s = "anim_sleep_ladder_kanim";
					break;
				case NavType.Pole:
					s = "anim_sleep_pole_kanim";
					break;
				default:
					s = "anim_sleep_floor_kanim";
					break;
				}
				sleepable.overrideAnims = new KAnimFile[1]
				{
					Assets.GetAnim(s)
				};
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, SleepChore>
	{
		public class SleepStates : State
		{
			public State condition_transition;

			public State condition_transition_pre;

			public State uninterruptable;

			public State normal;

			public State interrupt_noise;

			public State interrupt_noise_transition;

			public State interrupt_light;

			public State interrupt_light_transition;
		}

		public TargetParameter sleeper;

		public TargetParameter bed;

		public BoolParameter isInterruptable;

		public BoolParameter isDisturbedByNoise;

		public BoolParameter isDisturbedByLight;

		public ApproachSubState<IApproachable> approach;

		public SleepStates sleep;

		public State success;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = approach;
			Target(sleeper);
			root.Exit("DestroyLocator", delegate(StatesInstance smi)
			{
				smi.DestroyLocator();
			});
			approach.InitializeStates(sleeper, bed, sleep, null, null, null);
			sleep.Enter("SetAnims", delegate(StatesInstance smi)
			{
				smi.SetAnim();
			}).DefaultState(sleep.normal).ToggleTag(GameTags.Asleep)
				.DoSleep(sleeper, bed, success, null)
				.TriggerOnExit(GameHashes.SleepFinished)
				.EventHandler(GameHashes.SleepDisturbedByLight, delegate(StatesInstance smi)
				{
					isDisturbedByLight.Set(true, smi);
				})
				.EventHandler(GameHashes.SleepDisturbedByNoise, delegate(StatesInstance smi)
				{
					isDisturbedByNoise.Set(true, smi);
				});
			sleep.uninterruptable.DoNothing();
			sleep.normal.ParamTransition(isInterruptable, sleep.uninterruptable, GameStateMachine<States, StatesInstance, SleepChore, object>.IsFalse).ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.Sleeping, null).QueueAnim("working_loop", true, null)
				.ParamTransition(isDisturbedByNoise, sleep.interrupt_noise, GameStateMachine<States, StatesInstance, SleepChore, object>.IsTrue)
				.ParamTransition(isDisturbedByLight, sleep.interrupt_light, GameStateMachine<States, StatesInstance, SleepChore, object>.IsTrue)
				.Update(delegate(StatesInstance smi, float dt)
				{
					smi.CheckLightLevel();
				}, UpdateRate.SIM_200ms, false);
			sleep.interrupt_noise.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.SleepingInterruptedByNoise, null).QueueAnim("interrupt_light", false, null).OnAnimQueueComplete(sleep.interrupt_noise_transition);
			sleep.interrupt_noise_transition.Enter(delegate(StatesInstance smi)
			{
				Effects component2 = smi.master.GetComponent<Effects>();
				component2.Add(Db.Get().effects.Get("TerribleSleep"), true);
				if (component2.HasEffect(Db.Get().effects.Get("BadSleep")))
				{
					component2.Remove(Db.Get().effects.Get("BadSleep"));
				}
				isDisturbedByNoise.Set(false, smi);
				State state2 = (!smi.master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Sleep)) ? success : sleep.normal;
				smi.GoTo(state2);
			});
			sleep.interrupt_light.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.SleepingInterruptedByLight, null).QueueAnim("interrupt", false, null).OnAnimQueueComplete(sleep.interrupt_light_transition);
			sleep.interrupt_light_transition.Enter(delegate(StatesInstance smi)
			{
				Effects component = smi.master.GetComponent<Effects>();
				if (!component.HasEffect(Db.Get().effects.Get("TerribleSleep")))
				{
					smi.master.GetComponent<Effects>().Add(Db.Get().effects.Get("BadSleep"), true);
				}
				State state = (!smi.master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Sleep)) ? success : sleep.normal;
				isDisturbedByLight.Set(false, smi);
				smi.GoTo(state);
			});
			success.Enter(delegate(StatesInstance smi)
			{
				smi.EvaluateSleepQuality();
			}).ReturnSuccess();
		}
	}

	public static readonly Precondition IsOkayTimeToSleep = new Precondition
	{
		id = "IsOkayTimeToSleep",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_OKAY_TIME_TO_SLEEP,
		fn = (PreconditionFn)delegate(ref Precondition.Context context, object data)
		{
			Narcolepsy component = context.consumerState.consumer.GetComponent<Narcolepsy>();
			bool flag = (UnityEngine.Object)component != (UnityEngine.Object)null && component.IsNarcolepsing();
			bool flag2 = context.consumerState.consumer.GetSMI<StaminaMonitor.Instance>()?.NeedsToSleep() ?? false;
			bool flag3 = ChorePreconditions.instance.IsScheduledTime.fn(ref context, Db.Get().ScheduleBlockTypes.Sleep);
			return flag || flag3 || flag2;
		}
	};

	public SleepChore(ChoreType choreType, IStateMachineTarget target, GameObject bed, bool bedIsLocator, bool isInterruptable)
		: base(choreType, target, target.GetComponent<ChoreProvider>(), false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.personalNeeds, 5, false, true, 0, false, ReportManager.ReportType.PersonalTime)
	{
		base.smi = new StatesInstance(this, target.gameObject, bed, bedIsLocator, isInterruptable);
		if (isInterruptable)
		{
			AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
		}
		AddPrecondition(IsOkayTimeToSleep, null);
		Operational component = bed.GetComponent<Operational>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			AddPrecondition(ChorePreconditions.instance.IsOperational, component);
		}
	}

	public static Sleepable GetSafeFloorLocator(GameObject sleeper)
	{
		int num = sleeper.GetComponent<Sensors>().GetSensor<SafeCellSensor>().GetSleepCellQuery();
		if (num == Grid.InvalidCell)
		{
			num = Grid.PosToCell(sleeper.transform.GetPosition());
		}
		Vector3 pos = Grid.CellToPosCBC(num, Grid.SceneLayer.Move);
		GameObject gameObject = ChoreHelpers.CreateSleepLocator(pos);
		return gameObject.GetComponent<Sleepable>();
	}

	public static bool IsLightLevelOk(int cell)
	{
		int num = Grid.LightIntensity[cell];
		return num <= 0;
	}
}
