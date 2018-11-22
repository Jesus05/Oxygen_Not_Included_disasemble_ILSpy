using System;

public class DeliverFoodChore : Chore<DeliverFoodChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, DeliverFoodChore, object>.GameInstance
	{
		public StatesInstance(DeliverFoodChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, DeliverFoodChore>
	{
		public TargetParameter deliverer;

		public TargetParameter ediblesource;

		public TargetParameter ediblechunk;

		public TargetParameter deliverypoint;

		public FloatParameter requestedrationcount;

		public FloatParameter actualrationcount;

		public FetchSubState fetch;

		public ApproachSubState<Chattable> movetodeliverypoint;

		public DropSubState drop;

		public State success;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = fetch;
			fetch.InitializeStates(deliverer, ediblesource, ediblechunk, requestedrationcount, actualrationcount, movetodeliverypoint, null);
			movetodeliverypoint.InitializeStates(deliverer, deliverypoint, drop, null, null, null);
			drop.InitializeStates(deliverer, ediblechunk, deliverypoint, success, null);
			success.ReturnSuccess();
		}
	}

	public DeliverFoodChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.DeliverFood, target, target.GetComponent<ChoreProvider>(), false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.basic, 5, false, true, 0, (Tag[])null)
	{
		smi = new StatesInstance(this);
		AddPrecondition(ChorePreconditions.instance.IsChattable, target);
	}

	public override void Begin(Precondition.Context context)
	{
		smi.sm.requestedrationcount.Set(smi.GetComponent<StateMachineController>().GetSMI<RationMonitor.Instance>().GetRationsRemaining(), smi);
		smi.sm.ediblesource.Set(context.consumerState.gameObject.GetComponent<Sensors>().GetSensor<ClosestEdibleSensor>().GetEdible(), smi);
		smi.sm.deliverypoint.Set(gameObject, smi);
		smi.sm.deliverer.Set(context.consumerState.gameObject, smi);
		base.Begin(context);
	}
}
