using Klei.AI;
using TUNING;
using UnityEngine;

[SkipSaveFileSerialization]
public class Stinky : StateMachineComponent<Stinky.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Stinky, object>.GameInstance
	{
		public StatesInstance(Stinky master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Stinky>
	{
		public State idle;

		public State emit;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			root.Enter(delegate(StatesInstance smi)
			{
				KBatchedAnimController kBatchedAnimController = FXHelpers.CreateEffect("odor_fx_kanim", smi.master.gameObject.transform.GetPosition(), smi.master.gameObject.transform, true, Grid.SceneLayer.Front, false);
				kBatchedAnimController.Play(WorkLoopAnims, KAnim.PlayMode.Once);
				smi.master.stinkyController = kBatchedAnimController;
			}).Update("StinkyFX", delegate(StatesInstance smi, float dt)
			{
				if ((Object)smi.master.stinkyController != (Object)null)
				{
					smi.master.stinkyController.Play(WorkLoopAnims, KAnim.PlayMode.Once);
				}
			}, UpdateRate.SIM_4000ms, false);
			idle.Enter("ScheduleNextFart", delegate(StatesInstance smi)
			{
				smi.ScheduleGoTo(GetNewInterval(), emit);
			});
			emit.Enter("Fart", delegate(StatesInstance smi)
			{
				smi.master.Emit(smi.master.gameObject);
			}).ToggleExpression(Db.Get().Expressions.Relief, null).ScheduleGoTo(3f, idle);
		}

		private float GetNewInterval()
		{
			float mu = TRAITS.STINKY_EMIT_INTERVAL_MAX - TRAITS.STINKY_EMIT_INTERVAL_MIN;
			float a = Util.GaussianRandom(mu, 1f);
			a = Mathf.Max(a, TRAITS.STINKY_EMIT_INTERVAL_MIN);
			return Mathf.Min(a, TRAITS.STINKY_EMIT_INTERVAL_MAX);
		}
	}

	private const float EmitMass = 0.00250000018f;

	private const SimHashes EmitElement = SimHashes.ContaminatedOxygen;

	private const float EmissionRadius = 1.5f;

	private const float MaxDistanceSq = 2.25f;

	private KBatchedAnimController stinkyController;

	private static readonly EventSystem.IntraObjectHandler<Stinky> OnDeathDelegate = new EventSystem.IntraObjectHandler<Stinky>(delegate(Stinky component, object data)
	{
		component.OnDeath(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Stinky> OnRevivedDelegate = new EventSystem.IntraObjectHandler<Stinky>(delegate(Stinky component, object data)
	{
		component.OnRevived(data);
	});

	private static readonly HashedString[] WorkLoopAnims = new HashedString[3]
	{
		"working_pre",
		"working_loop",
		"working_pst"
	};

	protected override void OnPrefabInit()
	{
		Subscribe(1623392196, OnDeathDelegate);
		Subscribe(-1117766961, OnRevivedDelegate);
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	private void Emit(object data)
	{
		GameObject gameObject = (GameObject)data;
		Components.Cmps<MinionIdentity> liveMinionIdentities = Components.LiveMinionIdentities;
		Vector2 a = gameObject.transform.GetPosition();
		for (int i = 0; i < liveMinionIdentities.Count; i++)
		{
			MinionIdentity minionIdentity = liveMinionIdentities[i];
			if ((Object)minionIdentity.gameObject != (Object)gameObject.gameObject)
			{
				Vector2 b = minionIdentity.transform.GetPosition();
				float num = Vector2.SqrMagnitude(a - b);
				if (num <= 2.25f)
				{
					minionIdentity.Trigger(508119890, Strings.Get("STRINGS.DUPLICANTS.DISEASES.PUTRIDODOUR.CRINGE_EFFECT").String);
					minionIdentity.GetComponent<Effects>().Add("SmelledStinky", true);
					minionIdentity.gameObject.GetSMI<ThoughtGraph.Instance>().AddThought(Db.Get().Thoughts.PutridOdour);
				}
			}
		}
		int gameCell = Grid.PosToCell(gameObject.transform.GetPosition());
		float value = Db.Get().Amounts.Temperature.Lookup(this).value;
		SimMessages.AddRemoveSubstance(gameCell, SimHashes.ContaminatedOxygen, CellEventLogger.Instance.ElementConsumerSimUpdate, 0.00250000018f, value, byte.MaxValue, 0, true, -1);
		KFMOD.PlayOneShot(GlobalAssets.GetSound("Dupe_Flatulence", false), base.transform.GetPosition());
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
