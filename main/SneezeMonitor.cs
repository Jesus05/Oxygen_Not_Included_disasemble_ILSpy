using Klei.AI;
using System;
using UnityEngine;

public class SneezeMonitor : GameStateMachine<SneezeMonitor, SneezeMonitor.Instance>
{
	public class SneezyStates : State
	{
		public State idle;

		public State sneeze_pre;

		public State sneeze_pst;
	}

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

		public float NextSneezeTime()
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
	}

	private static readonly HashedString[] SneezeAnims = new HashedString[2]
	{
		"sneeze",
		"sneeze_pst"
	};

	public BoolParameter isSneezy = new BoolParameter(false);

	public State idle;

	public State taking_medicine;

	public SneezyStates Sneezy;

	public const float SINGLE_SNEEZE_TIME = 70f;

	public const float SNEEZE_TIME_VARIANCE = 0.3f;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = idle;
		idle.ParamTransition(isSneezy, Sneezy.idle, (Instance smi, bool p) => p);
		taking_medicine.TagTransition(GameTags.TakingMedicine, Sneezy.idle, true);
		Sneezy.idle.ScheduleGoTo((Instance smi) => smi.NextSneezeTime(), Sneezy.sneeze_pre).ParamTransition(isSneezy, idle, (Instance smi, bool p) => !p).TagTransition(GameTags.TakingMedicine, taking_medicine, false);
		Sneezy.sneeze_pre.ToggleScheduleCallback("Sneeze", (Instance smi) => 2f, delegate(Instance instanceObject)
		{
			AcousticDisturbance.Emit(instanceObject.master.gameObject, 3);
		}).ToggleChore((Instance smi) => new EmoteChore(smi.master, Db.Get().ChoreTypes.EmoteHighPriority, "anim_sneeze_kanim", SneezeAnims, null), Sneezy.sneeze_pst).ScheduleGoTo(5f, Sneezy.sneeze_pst);
		Sneezy.sneeze_pst.Enter(delegate(Instance smi)
		{
			smi.GoTo(Sneezy.idle);
		});
	}
}
