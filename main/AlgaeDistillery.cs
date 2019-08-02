using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class AlgaeDistillery : StateMachineComponent<AlgaeDistillery.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, AlgaeDistillery, object>.GameInstance
	{
		public StatesInstance(AlgaeDistillery smi)
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
					GameObject gameObject2 = storage.Drop(gameObject, true);
					gameObject2.transform.SetPosition(base.transform.GetPosition() + base.master.emitOffset);
				}
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, AlgaeDistillery>
	{
		public State disabled;

		public State waiting;

		public State converting;

		public State overpressure;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = disabled;
			root.EventTransition(GameHashes.OperationalChanged, disabled, (StatesInstance smi) => !smi.master.operational.IsOperational);
			disabled.EventTransition(GameHashes.OperationalChanged, waiting, (StatesInstance smi) => smi.master.operational.IsOperational);
			waiting.Enter("Waiting", delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			}).EventTransition(GameHashes.OnStorageChange, converting, (StatesInstance smi) => smi.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting());
			converting.Enter("Ready", delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).Transition(waiting, (StatesInstance smi) => !smi.master.GetComponent<ElementConverter>().CanConvertAtAll(), UpdateRate.SIM_200ms).EventHandler(GameHashes.OnStorageChange, delegate(StatesInstance smi)
			{
				smi.TryEmit();
			});
		}
	}

	[SerializeField]
	public Tag emitTag;

	[SerializeField]
	public float emitMass;

	[SerializeField]
	public Vector3 emitOffset;

	[MyCmpAdd]
	private Storage storage;

	[MyCmpGet]
	private ElementConverter emitter;

	[MyCmpReq]
	private Operational operational;

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}
}
