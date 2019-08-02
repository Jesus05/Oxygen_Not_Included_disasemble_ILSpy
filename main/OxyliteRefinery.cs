using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class OxyliteRefinery : StateMachineComponent<OxyliteRefinery.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, OxyliteRefinery, object>.GameInstance
	{
		public StatesInstance(OxyliteRefinery smi)
			: base(smi)
		{
		}

		public void TryEmit()
		{
			Storage storage = base.smi.master.storage;
			GameObject gameObject = storage.FindFirst(base.smi.master.emitTag);
			if ((Object)gameObject != (Object)null)
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				if (component.Mass >= base.master.emitMass)
				{
					Vector3 position = base.transform.GetPosition() + base.master.dropOffset;
					position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
					gameObject.transform.SetPosition(position);
					storage.Drop(gameObject, true);
				}
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, OxyliteRefinery>
	{
		public State disabled;

		public State waiting;

		public State converting;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = disabled;
			root.EventTransition(GameHashes.OperationalChanged, disabled, (StatesInstance smi) => !smi.master.operational.IsOperational);
			disabled.EventTransition(GameHashes.OperationalChanged, waiting, (StatesInstance smi) => smi.master.operational.IsOperational);
			waiting.EventTransition(GameHashes.OnStorageChange, converting, (StatesInstance smi) => smi.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting());
			converting.Enter(delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).Exit(delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			}).Transition(waiting, (StatesInstance smi) => !smi.master.GetComponent<ElementConverter>().CanConvertAtAll(), UpdateRate.SIM_200ms)
				.EventHandler(GameHashes.OnStorageChange, delegate(StatesInstance smi)
				{
					smi.TryEmit();
				});
		}
	}

	[MyCmpAdd]
	private Storage storage;

	[MyCmpReq]
	private Operational operational;

	public Tag emitTag;

	public float emitMass;

	public Vector3 dropOffset;

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}
}
