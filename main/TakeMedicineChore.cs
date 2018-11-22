using STRINGS;
using System;
using UnityEngine;

public class TakeMedicineChore : Chore<TakeMedicineChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, TakeMedicineChore, object>.GameInstance
	{
		public StatesInstance(TakeMedicineChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, TakeMedicineChore>
	{
		public TargetParameter eater;

		public TargetParameter source;

		public TargetParameter chunk;

		public FloatParameter requestedpillcount;

		public FloatParameter actualpillcount;

		public FetchSubState fetch;

		public State takemedicine;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = fetch;
			Target(eater);
			fetch.InitializeStates(eater, source, chunk, requestedpillcount, actualpillcount, takemedicine, null);
			takemedicine.ToggleAnims("anim_eat_floor_kanim", 0f).ToggleWork("TakeMedicine", delegate(StatesInstance smi)
			{
				MedicinalPill workable = chunk.Get<MedicinalPill>(smi);
				Worker worker = eater.Get<Worker>(smi);
				worker.StartWork(new Worker.StartWorkInfo(workable));
			}, (StatesInstance smi) => (UnityEngine.Object)chunk.Get<MedicinalPill>(smi) != (UnityEngine.Object)null, null, null);
		}
	}

	private Pickupable pickupable;

	private MedicinalPill medicine;

	public static readonly Precondition CanCure = new Precondition
	{
		id = "CanCure",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.CAN_CURE,
		fn = (PreconditionFn)delegate(ref Precondition.Context context, object data)
		{
			TakeMedicineChore takeMedicineChore2 = (TakeMedicineChore)data;
			return takeMedicineChore2.medicine.CanBeTakenBy(context.consumerState.gameObject);
		}
	};

	public static readonly Precondition IsConsumptionPermitted = new Precondition
	{
		id = "IsConsumptionPermitted",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_CONSUMPTION_PERMITTED,
		fn = (PreconditionFn)delegate(ref Precondition.Context context, object data)
		{
			TakeMedicineChore takeMedicineChore = (TakeMedicineChore)data;
			ConsumableConsumer consumableConsumer = context.consumerState.consumableConsumer;
			return (UnityEngine.Object)consumableConsumer == (UnityEngine.Object)null || consumableConsumer.IsPermitted(takeMedicineChore.medicine.PrefabID().Name);
		}
	};

	public TakeMedicineChore(MedicinalPill master)
		: base(Db.Get().ChoreTypes.TakeMedicine, (IStateMachineTarget)master, (ChoreProvider)null, false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.emergency, 5, false, true, 0, (Tag[])null)
	{
		medicine = master;
		pickupable = medicine.GetComponent<Pickupable>();
		smi = new StatesInstance(this);
		AddPrecondition(ChorePreconditions.instance.CanPickup, pickupable);
		AddPrecondition(CanCure, this);
		AddPrecondition(IsConsumptionPermitted, this);
	}

	public override void Begin(Precondition.Context context)
	{
		smi.sm.source.Set(pickupable.gameObject, smi);
		smi.sm.requestedpillcount.Set(1f, smi);
		smi.sm.eater.Set(context.consumerState.gameObject, smi);
		base.Begin(context);
		new TakeMedicineChore(medicine);
	}
}
