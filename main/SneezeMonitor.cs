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

		private void OnSneezyChange()
		{
			AttributeInstance attributeInstance = Db.Get().Attributes.Sneezyness.Lookup(base.master.gameObject);
			if (attributeInstance.GetTotalValue() > 0f)
			{
				if (base.smi.GetCurrentState() != base.smi.sm.Sneezy.idle)
				{
					base.smi.GoTo(base.smi.sm.Sneezy.idle);
				}
			}
			else if (base.smi.GetCurrentState() != base.smi.sm.idle)
			{
				base.smi.GoTo(base.smi.sm.idle);
			}
		}
	}

	private static readonly HashedString[] SneezeAnims = new HashedString[2]
	{
		"sneeze",
		"sneeze_pst"
	};

	public State idle;

	public SneezyStates Sneezy;

	public const float SNEEZE_INTERVAL_MIN = 45f;

	public const float SNEEZE_INTERVAL_MAX = 90f;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = idle;
		Sneezy.idle.ScheduleGoTo(UnityEngine.Random.Range(45f, 90f), Sneezy.sneeze_pre);
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
