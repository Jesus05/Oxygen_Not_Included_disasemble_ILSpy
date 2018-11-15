using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class WaterPurifier : StateMachineComponent<WaterPurifier.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, WaterPurifier, object>.GameInstance
	{
		public StatesInstance(WaterPurifier smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, WaterPurifier>
	{
		public class OnStates : State
		{
			public State waiting;

			public State working_pre;

			public State working;

			public State working_pst;
		}

		public State off;

		public OnStates on;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = off;
			off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, on, (StatesInstance smi) => smi.master.operational.IsOperational);
			on.PlayAnim("on").EventTransition(GameHashes.OperationalChanged, off, (StatesInstance smi) => !smi.master.operational.IsOperational).DefaultState(on.waiting);
			on.waiting.EventTransition(GameHashes.OnStorageChange, on.working_pre, (StatesInstance smi) => smi.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting());
			on.working_pre.PlayAnim("working_pre").OnAnimQueueComplete(on.working);
			on.working.Enter(delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).QueueAnim("working_loop", true, null).EventTransition(GameHashes.OnStorageChange, on.working_pst, (StatesInstance smi) => !smi.master.GetComponent<ElementConverter>().CanConvertAtAll())
				.Exit(delegate(StatesInstance smi)
				{
					smi.master.operational.SetActive(false, false);
				});
			on.working_pst.PlayAnim("working_pst").OnAnimQueueComplete(on.waiting);
		}
	}

	[MyCmpGet]
	private Operational operational;

	private ManualDeliveryKG[] deliveryComponents;

	private static readonly EventSystem.IntraObjectHandler<WaterPurifier> OnConduitConnectionChangedDelegate = new EventSystem.IntraObjectHandler<WaterPurifier>(delegate(WaterPurifier component, object data)
	{
		component.OnConduitConnectionChanged(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		deliveryComponents = GetComponents<ManualDeliveryKG>();
		OnConduitConnectionChanged(GetComponent<ConduitConsumer>().IsConnected);
		Subscribe(-2094018600, OnConduitConnectionChangedDelegate);
		base.smi.StartSM();
	}

	private void OnConduitConnectionChanged(object data)
	{
		bool pause = (bool)data;
		ManualDeliveryKG[] array = deliveryComponents;
		foreach (ManualDeliveryKG manualDeliveryKG in array)
		{
			Element element = ElementLoader.GetElement(manualDeliveryKG.requestedItemTag);
			if (element != null && element.IsLiquid)
			{
				manualDeliveryKG.Pause(pause, "pipe connected");
			}
		}
	}
}
