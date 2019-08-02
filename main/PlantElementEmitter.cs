using System.Collections.Generic;
using UnityEngine;

public class PlantElementEmitter : StateMachineComponent<PlantElementEmitter.StatesInstance>, IGameObjectEffectDescriptor
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, PlantElementEmitter, object>.GameInstance
	{
		public StatesInstance(PlantElementEmitter master)
			: base(master)
		{
		}

		public bool IsWilting()
		{
			if ((Object)base.master.wiltCondition == (Object)null)
			{
				return false;
			}
			return (Object)base.master.wiltCondition != (Object)null && base.master.wiltCondition.IsWilting();
		}
	}

	public class States : GameStateMachine<States, StatesInstance, PlantElementEmitter>
	{
		public State wilted;

		public State healthy;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = healthy;
			base.serializable = true;
			healthy.EventTransition(GameHashes.Wilt, wilted, (StatesInstance smi) => smi.IsWilting()).Update("PlantEmit", delegate(StatesInstance smi, float dt)
			{
				SimMessages.EmitMass(Grid.PosToCell(smi.master.gameObject), ElementLoader.FindElementByHash(smi.master.emittedElement).idx, smi.master.emitRate * dt, ElementLoader.FindElementByHash(smi.master.emittedElement).defaultValues.temperature, byte.MaxValue, 0, -1);
			}, UpdateRate.SIM_4000ms, false);
			wilted.EventTransition(GameHashes.WiltRecover, healthy, null);
		}
	}

	[MyCmpGet]
	private WiltCondition wiltCondition;

	[MyCmpReq]
	private KSelectable selectable;

	public SimHashes emittedElement;

	public float emitRate;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>();
	}
}
