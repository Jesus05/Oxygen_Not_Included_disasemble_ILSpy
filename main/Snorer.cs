using Klei.AI;
using UnityEngine;

[SkipSaveFileSerialization]
public class Snorer : StateMachineComponent<Snorer.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Snorer, object>.GameInstance
	{
		private SchedulerHandle snoreHandle;

		private KBatchedAnimController snoreEffect;

		private KBatchedAnimController snoreBGEffect;

		private const float BGEmissionRadius = 3f;

		public StatesInstance(Snorer master)
			: base(master)
		{
		}

		public bool IsSleeping()
		{
			return base.master.GetSMI<StaminaMonitor.Instance>()?.IsSleeping() ?? false;
		}

		public void StartSmallSnore()
		{
			snoreHandle = GameScheduler.Instance.Schedule("snorelines", 2f, StartSmallSnoreInternal, null, null);
		}

		private void StartSmallSnoreInternal(object data)
		{
			snoreHandle.ClearScheduler();
			KBatchedAnimController component = base.smi.master.GetComponent<KBatchedAnimController>();
			bool symbolVisible;
			Matrix4x4 symbolTransform = component.GetSymbolTransform(HeadHash, out symbolVisible);
			if (symbolVisible)
			{
				Vector4 column = symbolTransform.GetColumn(3);
				Vector3 position = column;
				position.z = Grid.GetLayerZ(Grid.SceneLayer.FXFront);
				snoreEffect = FXHelpers.CreateEffect("snore_fx_kanim", position, null, false, Grid.SceneLayer.Front, false);
				snoreEffect.destroyOnAnimComplete = true;
				snoreEffect.Play("snore", KAnim.PlayMode.Loop, 1f, 0f);
			}
		}

		public void StopSmallSnore()
		{
			snoreHandle.ClearScheduler();
			if ((Object)snoreEffect != (Object)null)
			{
				snoreEffect.PlayMode = KAnim.PlayMode.Once;
			}
			snoreEffect = null;
		}

		public void StartSnoreBGEffect()
		{
			AcousticDisturbance.Emit(base.smi.master.gameObject, 3);
		}

		public void StopSnoreBGEffect()
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Snorer>
	{
		public class SleepStates : State
		{
			public State quiet;

			public State snoring;
		}

		public State idle;

		public SleepStates sleeping;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			idle.Transition(sleeping, (StatesInstance smi) => smi.IsSleeping(), UpdateRate.SIM_200ms);
			sleeping.DefaultState(sleeping.quiet).Enter(delegate(StatesInstance smi)
			{
				smi.StartSmallSnore();
			}).Exit(delegate(StatesInstance smi)
			{
				smi.StopSmallSnore();
			})
				.Transition(idle, (StatesInstance smi) => !smi.master.GetSMI<StaminaMonitor.Instance>().IsSleeping(), UpdateRate.SIM_200ms);
			sleeping.quiet.Enter("ScheduleNextSnore", delegate(StatesInstance smi)
			{
				smi.ScheduleGoTo(GetNewInterval(), sleeping.snoring);
			});
			sleeping.snoring.Enter(delegate(StatesInstance smi)
			{
				smi.StartSnoreBGEffect();
			}).ToggleExpression(Db.Get().Expressions.Relief, null).ScheduleGoTo(3f, sleeping.quiet)
				.Exit(delegate(StatesInstance smi)
				{
					smi.StopSnoreBGEffect();
				});
		}

		private float GetNewInterval()
		{
			float a = Util.GaussianRandom(5f, 1f);
			a = Mathf.Max(a, 3f);
			return Mathf.Min(a, 10f);
		}
	}

	private struct CellInfo
	{
		public int cell;

		public int depth;

		public override int GetHashCode()
		{
			return cell;
		}

		public override bool Equals(object obj)
		{
			CellInfo cellInfo = (CellInfo)obj;
			return cell == cellInfo.cell;
		}
	}

	private static readonly HashedString HeadHash = "snapTo_mouth";

	private static readonly EventSystem.IntraObjectHandler<Snorer> OnDeathDelegate = new EventSystem.IntraObjectHandler<Snorer>(delegate(Snorer component, object data)
	{
		component.OnDeath(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Snorer> OnRevivedDelegate = new EventSystem.IntraObjectHandler<Snorer>(delegate(Snorer component, object data)
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

	public void ModifyTrait(Trait t)
	{
	}
}
