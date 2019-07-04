using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EntityElementExchanger : StateMachineComponent<EntityElementExchanger.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, EntityElementExchanger, object>.GameInstance
	{
		public StatesInstance(EntityElementExchanger master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, EntityElementExchanger>
	{
		public State exchanging;

		public State paused;

		[CompilerGenerated]
		private static Action<Sim.MassConsumedCallback, object> _003C_003Ef__mg_0024cache0;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = exchanging;
			base.serializable = true;
			exchanging.Enter(delegate(StatesInstance smi)
			{
				WiltCondition component = smi.master.gameObject.GetComponent<WiltCondition>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.IsWilting())
				{
					smi.GoTo(smi.sm.paused);
				}
			}).EventTransition(GameHashes.Wilt, paused, null).ToggleStatusItem(Db.Get().CreatureStatusItems.ExchangingElementConsume, (object)null)
				.ToggleStatusItem(Db.Get().CreatureStatusItems.ExchangingElementOutput, (object)null)
				.Update("EntityElementExchanger", delegate(StatesInstance smi, float dt)
				{
					HandleVector<Game.ComplexCallbackInfo<Sim.MassConsumedCallback>>.Handle handle = Game.Instance.massConsumedCallbackManager.Add(OnSimConsumeCallback, smi.master, "EntityElementExchanger");
					SimMessages.ConsumeMass(Grid.PosToCell(smi.master.gameObject), smi.master.consumedElement, smi.master.consumeRate * dt, 3, handle.index);
				}, UpdateRate.SIM_1000ms, false);
			paused.EventTransition(GameHashes.WiltRecover, exchanging, null);
		}
	}

	public Vector3 outputOffset = Vector3.zero;

	public bool reportExchange = false;

	[MyCmpReq]
	private KSelectable selectable;

	public SimHashes consumedElement;

	public SimHashes emittedElement;

	public float consumeRate;

	public float exchangeRatio;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public void SetConsumptionRate(float consumptionRate)
	{
		consumeRate = consumptionRate;
	}

	private static void OnSimConsumeCallback(Sim.MassConsumedCallback mass_cb_info, object data)
	{
		EntityElementExchanger entityElementExchanger = (EntityElementExchanger)data;
		if ((UnityEngine.Object)entityElementExchanger != (UnityEngine.Object)null)
		{
			entityElementExchanger.OnSimConsume(mass_cb_info);
		}
	}

	private void OnSimConsume(Sim.MassConsumedCallback mass_cb_info)
	{
		float num = mass_cb_info.mass * base.smi.master.exchangeRatio;
		if (reportExchange && base.smi.master.emittedElement == SimHashes.Oxygen)
		{
			string text = base.gameObject.GetProperName();
			ReceptacleMonitor component = GetComponent<ReceptacleMonitor>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null && (UnityEngine.Object)component.GetReceptacle() != (UnityEngine.Object)null)
			{
				text = text + " (" + component.GetReceptacle().gameObject.GetProperName() + ")";
			}
			ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, num, text, null);
		}
		SimMessages.EmitMass(Grid.PosToCell(base.smi.master.transform.GetPosition() + outputOffset), ElementLoader.FindElementByHash(base.smi.master.emittedElement).idx, num, ElementLoader.FindElementByHash(base.smi.master.emittedElement).defaultValues.temperature, byte.MaxValue, 0, -1);
	}
}
