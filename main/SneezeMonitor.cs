using Klei.AI;
using System;
using UnityEngine;

public class SneezeMonitor : GameStateMachine<SneezeMonitor, SneezeMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		private StatusItem statusItem;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			AttributeInstance attributeInstance = Db.Get().Attributes.Sneezyness.Lookup(master.gameObject);
			OnSneezyChange();
			AttributeInstance attributeInstance2 = attributeInstance;
			attributeInstance2.OnDirty = (System.Action)Delegate.Combine(attributeInstance2.OnDirty, new System.Action(OnSneezyChange));
		}

		public override void StopSM(string reason)
		{
			AttributeInstance attributeInstance = Db.Get().Attributes.Sneezyness.Lookup(base.master.gameObject);
			AttributeInstance attributeInstance2 = attributeInstance;
			attributeInstance2.OnDirty = (System.Action)Delegate.Remove(attributeInstance2.OnDirty, new System.Action(OnSneezyChange));
			base.StopSM(reason);
		}

		public float NextSneezeInterval()
		{
			AttributeInstance attributeInstance = Db.Get().Attributes.Sneezyness.Lookup(base.master.gameObject);
			if (attributeInstance.GetTotalValue() <= 0f)
			{
				return 70f;
			}
			float num = 70f / attributeInstance.GetTotalValue();
			return UnityEngine.Random.Range(num * 0.7f, num * 1.3f);
		}

		private void OnSneezyChange()
		{
			AttributeInstance attributeInstance = Db.Get().Attributes.Sneezyness.Lookup(base.master.gameObject);
			base.smi.sm.isSneezy.Set(attributeInstance.GetTotalValue() > 0f, base.smi);
		}

		public Reactable GetReactable()
		{
			float num = NextSneezeInterval();
			GameObject gameObject = base.master.gameObject;
			HashedString id = "Sneeze";
			ChoreType cough = Db.Get().ChoreTypes.Cough;
			HashedString animset = "anim_sneeze_kanim";
			float min_reactor_time = num;
			return new SelfEmoteReactable(gameObject, id, cough, animset, 0f, min_reactor_time, float.PositiveInfinity).AddStep(new EmoteReactable.EmoteStep
			{
				anim = (HashedString)"sneeze",
				startcb = new Action<GameObject>(TriggerDisurbance)
			}).AddStep(new EmoteReactable.EmoteStep
			{
				anim = (HashedString)"sneeze_pst",
				finishcb = new Action<GameObject>(ResetSneeze)
			});
		}

		private void TriggerDisurbance(GameObject go)
		{
			AcousticDisturbance.Emit(go, 3);
		}

		private void ResetSneeze(GameObject go)
		{
			base.smi.GoTo(base.sm.idle);
		}
	}

	public BoolParameter isSneezy = new BoolParameter(false);

	public State idle;

	public State taking_medicine;

	public State sneezy;

	public const float SINGLE_SNEEZE_TIME = 70f;

	public const float SNEEZE_TIME_VARIANCE = 0.3f;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = idle;
		idle.ParamTransition(isSneezy, sneezy, (Instance smi, bool p) => p);
		sneezy.ParamTransition(isSneezy, idle, (Instance smi, bool p) => !p).ToggleReactable((Instance smi) => smi.GetReactable());
	}
}
