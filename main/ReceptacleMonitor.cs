using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class ReceptacleMonitor : StateMachineComponent<ReceptacleMonitor.StatesInstance>, IGameObjectEffectDescriptor, IWiltCause, ISim1000ms
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, ReceptacleMonitor, object>.GameInstance
	{
		public StatesInstance(ReceptacleMonitor master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, ReceptacleMonitor>
	{
		public ObjectParameter<SingleEntityReceptacle> receptacle;

		public State wild;

		public State inoperational;

		public State operational;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = wild;
			base.serializable = true;
			wild.TriggerOnEnter(GameHashes.ReceptacleOperational, null);
			inoperational.TriggerOnEnter(GameHashes.ReceptacleInoperational, null);
			operational.TriggerOnEnter(GameHashes.ReceptacleOperational, null);
		}
	}

	WiltCondition.Condition[] IWiltCause.Conditions
	{
		get
		{
			return new WiltCondition.Condition[1]
			{
				WiltCondition.Condition.Receptacle
			};
		}
	}

	public string WiltStateString
	{
		get
		{
			string text = string.Empty;
			if (base.smi.IsInsideState(base.smi.sm.inoperational))
			{
				text += CREATURES.STATUSITEMS.RECEPTACLEINOPERATIONAL.NAME;
			}
			return text;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public void Sim1000ms(float dt)
	{
		if ((Object)base.smi.sm.receptacle.Get(base.smi) == (Object)null)
		{
			base.smi.GoTo(base.smi.sm.wild);
		}
		else
		{
			Operational component = base.smi.sm.receptacle.Get(base.smi).GetComponent<Operational>();
			if ((Object)component == (Object)null)
			{
				base.smi.GoTo(base.smi.sm.operational);
			}
			else if (component.IsOperational)
			{
				base.smi.GoTo(base.smi.sm.operational);
			}
			else
			{
				base.smi.GoTo(base.smi.sm.inoperational);
			}
		}
	}

	public bool HasReceptacle()
	{
		return !base.smi.IsInsideState(base.smi.sm.wild);
	}

	public bool HasOperationalReceptacle()
	{
		return base.smi.IsInsideState(base.smi.sm.operational);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.Add(new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_RECEPTACLE, UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_RECEPTACLE, Descriptor.DescriptorType.Requirement, false));
		return list;
	}
}
