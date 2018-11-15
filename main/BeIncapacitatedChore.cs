using System;
using UnityEngine;

public class BeIncapacitatedChore : Chore<BeIncapacitatedChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, BeIncapacitatedChore, object>.GameInstance
	{
		public StatesInstance(BeIncapacitatedChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, BeIncapacitatedChore>
	{
		public class IncapacitatedStates : State
		{
			public State lookingForBed;

			public BeingRescued rescue;

			public State death;

			public State recovering;
		}

		public class BeingRescued : State
		{
			public State waitingForPickup;

			public State carried;
		}

		public IncapacitatedStates incapacitation_root;

		public TargetParameter clinic;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = root;
			root.ToggleAnims("anim_incapacitated_kanim", 0f).ToggleStatusItem(Db.Get().DuplicantStatusItems.Incapacitated, (StatesInstance smi) => smi.master.gameObject.GetSMI<IncapacitationMonitor.Instance>()).Enter(delegate(StatesInstance smi)
			{
				smi.SetStatus(Status.Failed);
				smi.GoTo(incapacitation_root.lookingForBed);
			});
			incapacitation_root.EventHandler(GameHashes.Died, delegate(StatesInstance smi)
			{
				smi.SetStatus(Status.Failed);
				smi.StopSM("died");
			});
			incapacitation_root.lookingForBed.Update("LookForAvailableClinic", delegate(StatesInstance smi, float dt)
			{
				smi.master.FindAvailableMedicalBed(smi.master.GetComponent<Navigator>());
			}, UpdateRate.SIM_1000ms, false).Enter("PlayAnim", delegate(StatesInstance smi)
			{
				smi.sm.clinic.Set(null, smi);
				smi.Play(IncapacitatedDuplicantAnim_pre, KAnim.PlayMode.Once);
				smi.Queue(IncapacitatedDuplicantAnim_loop, KAnim.PlayMode.Loop);
			});
			incapacitation_root.rescue.ToggleChore((StatesInstance smi) => new RescueIncapacitatedChore(smi.master, masterTarget.Get(smi)), incapacitation_root.recovering, incapacitation_root.lookingForBed);
			incapacitation_root.rescue.waitingForPickup.EventTransition(GameHashes.OnStore, incapacitation_root.rescue.carried, null).Update("LookForAvailableClinic", delegate(StatesInstance smi, float dt)
			{
				bool flag2 = false;
				if ((UnityEngine.Object)smi.sm.clinic.Get(smi) == (UnityEngine.Object)null)
				{
					flag2 = true;
				}
				else if (!smi.master.gameObject.GetComponent<Navigator>().CanReach(clinic.Get(smi).GetComponent<Clinic>()))
				{
					flag2 = true;
				}
				else if (clinic.Get(smi).GetComponent<Assignable>().assignee == null)
				{
					flag2 = true;
				}
				else if ((UnityEngine.Object)clinic.Get(smi).GetComponent<Assignable>().assignee.GetSoleOwner().gameObject != (UnityEngine.Object)smi.master.gameObject)
				{
					flag2 = true;
				}
				if (flag2)
				{
					smi.GoTo(incapacitation_root.lookingForBed);
				}
			}, UpdateRate.SIM_1000ms, false);
			incapacitation_root.rescue.carried.Update("LookForAvailableClinic", delegate(StatesInstance smi, float dt)
			{
				bool flag = false;
				if ((UnityEngine.Object)smi.sm.clinic.Get(smi) == (UnityEngine.Object)null)
				{
					flag = true;
				}
				else if (clinic.Get(smi).GetComponent<Assignable>().assignee == null)
				{
					flag = true;
				}
				else if ((UnityEngine.Object)clinic.Get(smi).GetComponent<Assignable>().assignee.GetSoleOwner().gameObject != (UnityEngine.Object)smi.master.gameObject)
				{
					flag = true;
				}
				if (flag)
				{
					smi.GoTo(incapacitation_root.lookingForBed);
				}
			}, UpdateRate.SIM_1000ms, false).Enter(delegate(StatesInstance smi)
			{
				smi.Queue(IncapacitatedDuplicantAnim_carry, KAnim.PlayMode.Loop);
			}).Exit(delegate(StatesInstance smi)
			{
				smi.Play(IncapacitatedDuplicantAnim_place, KAnim.PlayMode.Once);
			});
			incapacitation_root.death.PlayAnim(IncapacitatedDuplicantAnim_death).Enter(delegate(StatesInstance smi)
			{
				smi.SetStatus(Status.Failed);
				smi.StopSM("died");
			});
			incapacitation_root.recovering.ToggleUrge(Db.Get().Urges.HealCritical).Enter(delegate(StatesInstance smi)
			{
				smi.Trigger(-1256572400, null);
				smi.SetStatus(Status.Success);
				smi.StopSM("recovering");
			});
		}
	}

	private static string IncapacitatedDuplicantAnim_pre = "incapacitate_pre";

	private static string IncapacitatedDuplicantAnim_loop = "incapacitate_loop";

	private static string IncapacitatedDuplicantAnim_death = "incapacitate_death";

	private static string IncapacitatedDuplicantAnim_carry = "carry_loop";

	private static string IncapacitatedDuplicantAnim_place = "place";

	public BeIncapacitatedChore(IStateMachineTarget master)
		: base(Db.Get().ChoreTypes.BeIncapacitated, master, master.GetComponent<ChoreProvider>(), true, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.emergency, 0, false, true, 0, (Tag[])null)
	{
		smi = new StatesInstance(this);
	}

	public void FindAvailableMedicalBed(Navigator navigator)
	{
		Clinic clinic = null;
		AssignableSlot clinic2 = Db.Get().AssignableSlots.Clinic;
		Ownables component = gameObject.GetComponent<Ownables>();
		AssignableSlotInstance slot = component.GetSlot(clinic2);
		if ((UnityEngine.Object)slot.assignable == (UnityEngine.Object)null)
		{
			Assignable assignable = component.AutoAssignSlot(clinic2);
			if ((UnityEngine.Object)assignable != (UnityEngine.Object)null)
			{
				clinic = assignable.GetComponent<Clinic>();
			}
		}
		else
		{
			clinic = slot.assignable.GetComponent<Clinic>();
		}
		if ((UnityEngine.Object)clinic != (UnityEngine.Object)null && navigator.CanReach(clinic))
		{
			smi.sm.clinic.Set(clinic.gameObject, smi);
			smi.GoTo(smi.sm.incapacitation_root.rescue.waitingForPickup);
		}
	}

	public GameObject GetChosenClinic()
	{
		return smi.sm.clinic.Get(smi);
	}
}
