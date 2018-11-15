using System.Collections.Generic;
using UnityEngine;

public class SleepChoreMonitor : GameStateMachine<SleepChoreMonitor, SleepChoreMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		private int locatorCell;

		public GameObject locator;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void UpdateBed()
		{
			Ownables component = base.sm.masterTarget.Get(base.smi).GetComponent<Ownables>();
			Assignable assignable = null;
			Assignable assignable2 = component.GetAssignable(Db.Get().AssignableSlots.MedicalBed);
			if ((Object)assignable2 != (Object)null && assignable2.CanAutoAssignTo(base.gameObject.GetComponent<MinionIdentity>()))
			{
				assignable = assignable2;
			}
			else
			{
				assignable = component.GetAssignable(Db.Get().AssignableSlots.Bed);
				if ((Object)assignable == (Object)null)
				{
					assignable = component.AutoAssignSlot(Db.Get().AssignableSlots.Bed);
					if ((Object)assignable != (Object)null)
					{
						AssignableReachabilitySensor sensor = GetComponent<Sensors>().GetSensor<AssignableReachabilitySensor>();
						sensor.Update();
					}
				}
			}
			base.smi.sm.bed.Set(assignable, base.smi);
		}

		public bool HasSleepUrge()
		{
			return GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.Sleep);
		}

		public bool IsBedReachable()
		{
			AssignableReachabilitySensor sensor = GetComponent<Sensors>().GetSensor<AssignableReachabilitySensor>();
			return sensor.IsReachable(Db.Get().AssignableSlots.Bed) || sensor.IsReachable(Db.Get().AssignableSlots.MedicalBed);
		}

		public GameObject CreatePassedOutLocator()
		{
			Sleepable safeFloorLocator = SleepChore.GetSafeFloorLocator(base.master.gameObject);
			safeFloorLocator.effectName = "PassedOutSleep";
			safeFloorLocator.wakeEffects = new List<string>
			{
				"SoreBack"
			};
			safeFloorLocator.stretchOnWake = false;
			return safeFloorLocator.gameObject;
		}

		public GameObject CreateFloorLocator()
		{
			Sleepable safeFloorLocator = SleepChore.GetSafeFloorLocator(base.master.gameObject);
			safeFloorLocator.effectName = "FloorSleep";
			safeFloorLocator.wakeEffects = new List<string>
			{
				"SoreBack"
			};
			safeFloorLocator.stretchOnWake = false;
			return safeFloorLocator.gameObject;
		}
	}

	public State satisfied;

	public State checkforbed;

	public State passingout;

	public State sleeponfloor;

	public State bedassigned;

	public TargetParameter bed;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		base.serializable = false;
		root.EventHandler(GameHashes.AssignablesChanged, delegate(Instance smi)
		{
			smi.UpdateBed();
		});
		satisfied.EventTransition(GameHashes.AddUrge, checkforbed, (Instance smi) => smi.HasSleepUrge());
		checkforbed.Enter("SetBed", delegate(Instance smi)
		{
			smi.UpdateBed();
			StaminaMonitor.Instance sMI = smi.GetSMI<StaminaMonitor.Instance>();
			if (sMI.NeedsToSleep())
			{
				smi.GoTo(passingout);
			}
			else if ((Object)bed.Get(smi) == (Object)null || !smi.IsBedReachable())
			{
				smi.GoTo(sleeponfloor);
			}
			else
			{
				smi.GoTo(bedassigned);
			}
		});
		passingout.ToggleChore(CreatePassingOutChore, satisfied, satisfied);
		sleeponfloor.ToggleChore(CreateSleepOnFloorChore, satisfied, satisfied);
		bedassigned.ParamTransition(bed, checkforbed, (Instance smi, GameObject p) => (Object)p == (Object)null).EventTransition(GameHashes.AssignablesChanged, checkforbed, null).EventTransition(GameHashes.AssignableReachabilityChanged, checkforbed, (Instance smi) => !smi.IsBedReachable())
			.ToggleChore(CreateSleepChore, satisfied, satisfied);
	}

	private Chore CreatePassingOutChore(Instance smi)
	{
		GameObject gameObject = smi.CreatePassedOutLocator();
		return new SleepChore(Db.Get().ChoreTypes.Sleep, smi.master, gameObject, true, false);
	}

	private Chore CreateSleepOnFloorChore(Instance smi)
	{
		GameObject gameObject = smi.CreateFloorLocator();
		return new SleepChore(Db.Get().ChoreTypes.Sleep, smi.master, gameObject, true, true);
	}

	private Chore CreateSleepChore(Instance smi)
	{
		return new SleepChore(Db.Get().ChoreTypes.Sleep, smi.master, bed.Get(smi), false, true);
	}
}
