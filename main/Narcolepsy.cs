using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[SkipSaveFileSerialization]
public class Narcolepsy : StateMachineComponent<Narcolepsy.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Narcolepsy, object>.GameInstance
	{
		public StatesInstance(Narcolepsy master)
			: base(master)
		{
		}

		public bool IsSleeping()
		{
			return base.master.GetSMI<StaminaMonitor.Instance>()?.IsSleeping() ?? false;
		}

		public bool IsNarcolepsing()
		{
			return GetCurrentState() == base.sm.sleepy;
		}

		public GameObject CreateFloorLocator()
		{
			Sleepable safeFloorLocator = SleepChore.GetSafeFloorLocator(base.master.gameObject);
			safeFloorLocator.effectName = "NarcolepticSleep";
			safeFloorLocator.stretchOnWake = false;
			return safeFloorLocator.gameObject;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Narcolepsy>
	{
		public State idle;

		public State sleepy;

		public State dead;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			root.TagTransition(GameTags.Dead, dead, false);
			idle.Enter("ScheduleNextSleep", delegate(StatesInstance smi)
			{
				smi.ScheduleGoTo(GetNewInterval(TRAITS.NARCOLEPSY_INTERVAL_MIN, TRAITS.NARCOLEPSY_INTERVAL_MAX), sleepy);
			});
			sleepy.Enter("Is Already Sleeping Check", delegate(StatesInstance smi)
			{
				if (smi.master.GetSMI<StaminaMonitor.Instance>().IsSleeping())
				{
					smi.GoTo(idle);
				}
				else
				{
					smi.ScheduleGoTo(GetNewInterval(TRAITS.NARCOLEPSY_SLEEPDURATION_MIN, TRAITS.NARCOLEPSY_SLEEPDURATION_MAX), idle);
				}
			}).ToggleUrge(Db.Get().Urges.Narcolepsy).ToggleChore(CreateNarcolepsyChore, idle);
			dead.DoNothing();
		}

		private Chore CreateNarcolepsyChore(StatesInstance smi)
		{
			GameObject bed = smi.CreateFloorLocator();
			SleepChore sleepChore = new SleepChore(Db.Get().ChoreTypes.Narcolepsy, smi.master, bed, true, false);
			sleepChore.AddPrecondition(IsNarcolepsingPrecondition, null);
			return sleepChore;
		}

		private float GetNewInterval(float min, float max)
		{
			float mu = max - min;
			float a = Util.GaussianRandom(mu, 1f);
			a = Mathf.Max(a, min);
			a = Mathf.Min(a, max);
			return Random.Range(min, max);
		}
	}

	public static readonly Chore.Precondition IsNarcolepsingPrecondition = new Chore.Precondition
	{
		id = "IsNarcolepsingPrecondition",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_NARCOLEPSING,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			Narcolepsy component2 = context.consumerState.consumer.GetComponent<Narcolepsy>();
			return (Object)component2 != (Object)null && component2.IsNarcolepsing();
		}
	};

	private static readonly EventSystem.IntraObjectHandler<Narcolepsy> OnDeathDelegate = new EventSystem.IntraObjectHandler<Narcolepsy>(delegate(Narcolepsy component, object data)
	{
		component.OnDeath(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Narcolepsy> OnRevivedDelegate = new EventSystem.IntraObjectHandler<Narcolepsy>(delegate(Narcolepsy component, object data)
	{
		component.OnRevived(data);
	});

	protected override void OnPrefabInit()
	{
		Subscribe(1623392196, OnDeathDelegate);
		Subscribe(-1117766961, OnRevivedDelegate);
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	private void OnDeath(object data)
	{
		base.enabled = false;
	}

	private void OnRevived(object data)
	{
		base.enabled = true;
	}

	public bool IsNarcolepsing()
	{
		return base.smi.IsNarcolepsing();
	}

	public void ModifyTrait(Trait t)
	{
	}
}
