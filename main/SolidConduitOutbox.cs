using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SolidConduitOutbox : StateMachineComponent<SolidConduitOutbox.SMInstance>
{
	public class SMInstance : GameStateMachine<States, SMInstance, SolidConduitOutbox, object>.GameInstance
	{
		public SMInstance(SolidConduitOutbox master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, SMInstance, SolidConduitOutbox>
	{
		public BoolParameter consuming;

		public State idle;

		public State working;

		public State post;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			root.Update("RefreshConsuming", delegate(SMInstance smi, float dt)
			{
				smi.master.UpdateConsuming();
			}, UpdateRate.SIM_1000ms, false);
			idle.PlayAnim("on").ParamTransition(consuming, working, GameStateMachine<States, SMInstance, SolidConduitOutbox, object>.IsTrue);
			working.PlayAnim("working_pre").QueueAnim("working_loop", true, null).ParamTransition(consuming, post, GameStateMachine<States, SMInstance, SolidConduitOutbox, object>.IsFalse);
			post.PlayAnim("working_pst").OnAnimQueueComplete(idle);
		}
	}

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private SolidConduitConsumer consumer;

	[MyCmpAdd]
	private Storage storage;

	private MeterController meter;

	private static readonly EventSystem.IntraObjectHandler<SolidConduitOutbox> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<SolidConduitOutbox>(delegate(SolidConduitOutbox component, object data)
	{
		component.OnStorageChanged(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		meter = new MeterController(this, Meter.Offset.Infront, Grid.SceneLayer.NoLayer);
		Subscribe(-1697596308, OnStorageChangedDelegate);
		UpdateMeter();
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	private void OnStorageChanged(object data)
	{
		UpdateMeter();
	}

	private void UpdateMeter()
	{
		float positionPercent = Mathf.Clamp01(storage.MassStored() / storage.capacityKg);
		meter.SetPositionPercent(positionPercent);
	}

	private void UpdateConsuming()
	{
		base.smi.sm.consuming.Set(consumer.IsConsuming, base.smi);
	}
}
