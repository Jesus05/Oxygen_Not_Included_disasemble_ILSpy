using System;
using UnityEngine;

public class EquipChore : Chore<EquipChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, EquipChore, object>.GameInstance
	{
		public StatesInstance(EquipChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, EquipChore>
	{
		public class Equip : State
		{
			public State pre;

			public State pst;
		}

		public TargetParameter equipper;

		public TargetParameter equippable_source;

		public TargetParameter equippable_result;

		public FloatParameter requested_units;

		public FloatParameter actual_units;

		public FetchSubState fetch;

		public Equip equip;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = fetch;
			Target(equipper);
			root.DoNothing();
			fetch.InitializeStates(equipper, equippable_source, equippable_result, requested_units, actual_units, equip, null);
			equip.ToggleWork<EquippableWorkable>(equippable_result, null, null, null);
		}
	}

	public EquipChore(IStateMachineTarget equippable)
		: base(Db.Get().ChoreTypes.Equip, equippable, (ChoreProvider)null, false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.personalNeeds, 5, false, true, 0, (Tag[])null, false, ReportManager.ReportType.WorkTime)
	{
		smi = new StatesInstance(this);
		smi.sm.equippable_source.Set(equippable.gameObject, smi);
		smi.sm.requested_units.Set(1f, smi);
		showAvailabilityInHoverText = false;
		AddPrecondition(ChorePreconditions.instance.IsAssignedtoMe, equippable.GetComponent<Assignable>());
		AddPrecondition(ChorePreconditions.instance.CanPickup, equippable.GetComponent<Pickupable>());
	}

	public override void Begin(Precondition.Context context)
	{
		if ((UnityEngine.Object)context.consumerState.consumer == (UnityEngine.Object)null)
		{
			Debug.LogError("EquipChore null context.consumer", null);
		}
		else if (smi == null)
		{
			Debug.LogError("EquipChore null smi", null);
		}
		else if (smi.sm == null)
		{
			Debug.LogError("EquipChore null smi.sm", null);
		}
		else if (smi.sm.equippable_source == null)
		{
			Debug.LogError("EquipChore null smi.sm.equippable_source", null);
		}
		else
		{
			smi.sm.equipper.Set(context.consumerState.gameObject, smi);
			base.Begin(context);
		}
	}
}
