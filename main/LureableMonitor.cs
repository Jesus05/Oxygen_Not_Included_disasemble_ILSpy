using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class LureableMonitor : GameStateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>
{
	public class Def : BaseDef, IGameObjectEffectDescriptor
	{
		public float cooldown = 20f;

		public Tag[] lures;

		public List<Descriptor> GetDescriptors(GameObject go)
		{
			List<Descriptor> list = new List<Descriptor>();
			list.Add(new Descriptor(UI.BUILDINGEFFECTS.CAPTURE_METHOD_LURE, UI.BUILDINGEFFECTS.TOOLTIPS.CAPTURE_METHOD_LURE, Descriptor.DescriptorType.Effect, false));
			return list;
		}
	}

	public new class Instance : GameInstance
	{
		private struct LureIterator : GameScenePartitioner.Iterator
		{
			private Navigator navigator;

			private Tag[] lures;

			public int cost
			{
				get;
				private set;
			}

			public GameObject result
			{
				get;
				private set;
			}

			public LureIterator(Navigator navigator, Tag[] lures)
			{
				this.navigator = navigator;
				this.lures = lures;
				cost = -1;
				result = null;
			}

			public void Iterate(object target_obj)
			{
				Lure.Instance instance = target_obj as Lure.Instance;
				if (instance != null && instance.IsActive() && instance.HasAnyLure(lures))
				{
					int navigationCost = navigator.GetNavigationCost(Grid.PosToCell(instance.transform.GetPosition()), instance.def.lurePoints);
					if (navigationCost != -1 && (cost == -1 || navigationCost < cost))
					{
						cost = navigationCost;
						result = instance.gameObject;
					}
				}
			}

			public void Cleanup()
			{
			}
		}

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public void FindLure()
		{
			LureIterator iterator = new LureIterator(GetComponent<Navigator>(), base.def.lures);
			GameScenePartitioner.Instance.Iterate(Grid.PosToCell(base.smi.transform.GetPosition()), 1, GameScenePartitioner.Instance.lure, ref iterator);
			iterator.Cleanup();
			base.sm.targetLure.Set(iterator.result, this);
		}

		public bool HasLure()
		{
			return (Object)base.sm.targetLure.Get(this) != (Object)null;
		}

		public GameObject GetTargetLure()
		{
			return base.sm.targetLure.Get(this);
		}
	}

	public TargetParameter targetLure;

	public State nolure;

	public State haslure;

	public State cooldown;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = cooldown;
		cooldown.ScheduleGoTo((Instance smi) => smi.def.cooldown, nolure);
		nolure.Update("FindLure", delegate(Instance smi, float dt)
		{
			smi.FindLure();
		}, UpdateRate.SIM_1000ms, false).ParamTransition(targetLure, haslure, (Instance smi, GameObject p) => (Object)p != (Object)null);
		haslure.ParamTransition(targetLure, haslure, (Instance smi, GameObject p) => (Object)p == (Object)null).Update("FindLure", delegate(Instance smi, float dt)
		{
			smi.FindLure();
		}, UpdateRate.SIM_1000ms, false).ToggleBehaviour(GameTags.Creatures.MoveToLure, (Instance smi) => smi.HasLure(), delegate(Instance smi)
		{
			smi.GoTo(cooldown);
		});
	}
}
