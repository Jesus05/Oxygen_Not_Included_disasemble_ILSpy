using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class DiseaseMonitor : GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance>
{
	public class SickStates : State
	{
		public State minor;

		public State major;
	}

	public new class Instance : GameInstance
	{
		private Diseases activeDiseases;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			activeDiseases = master.GetComponent<MinionModifiers>().diseases;
		}

		private string OnGetToolTip(List<Notification> notifications, object data)
		{
			return DUPLICANTS.STATUSITEMS.HASDISEASE.TOOLTIP;
		}

		public bool IsSick()
		{
			return activeDiseases.Count > 0;
		}

		public bool HasMajorDisease()
		{
			foreach (DiseaseInstance activeDisease in activeDiseases)
			{
				if (activeDisease.modifier.severity >= Disease.Severity.Major)
				{
					return true;
				}
			}
			return false;
		}

		public void AutoAssignClinic()
		{
			Ownables soleOwner = base.sm.masterTarget.Get(base.smi).GetComponent<MinionIdentity>().GetSoleOwner();
			AssignableSlot clinic = Db.Get().AssignableSlots.Clinic;
			AssignableSlotInstance slot = soleOwner.GetSlot(clinic);
			if (slot != null && !((Object)slot.assignable != (Object)null))
			{
				soleOwner.AutoAssignSlot(clinic);
			}
		}

		public void UnassignClinic()
		{
			Ownables soleOwner = base.sm.masterTarget.Get(base.smi).GetComponent<MinionIdentity>().GetSoleOwner();
			AssignableSlot clinic = Db.Get().AssignableSlots.Clinic;
			soleOwner.GetSlot(clinic)?.Unassign(true);
		}

		public bool IsSleepingOrSleepSchedule()
		{
			Schedulable component = GetComponent<Schedulable>();
			if ((Object)component != (Object)null && component.IsAllowed(Db.Get().ScheduleBlockTypes.Sleep))
			{
				return true;
			}
			KPrefabID component2 = GetComponent<KPrefabID>();
			if ((Object)component2 != (Object)null && component2.HasTag(GameTags.Asleep))
			{
				return true;
			}
			return false;
		}
	}

	public State healthy;

	public SickStates sick;

	public State post;

	public State post_nocheer;

	private static readonly HashedString SickPostKAnim = "anim_cheer_kanim";

	private static readonly HashedString[] SickPostAnims = new HashedString[3]
	{
		"cheer_pre",
		"cheer_loop",
		"cheer_pst"
	};

	public override void InitializeStates(out BaseState default_state)
	{
		base.serializable = true;
		default_state = healthy;
		healthy.EventTransition(GameHashes.DiseaseAdded, sick, (Instance smi) => smi.IsSick());
		sick.DefaultState(sick.minor).EventTransition(GameHashes.DiseaseCured, post_nocheer, (Instance smi) => !smi.IsSick()).ToggleThought(Db.Get().Thoughts.GotInfected, null);
		sick.minor.EventTransition(GameHashes.DiseaseAdded, sick.major, (Instance smi) => smi.HasMajorDisease());
		sick.major.EventTransition(GameHashes.DiseaseCured, sick.minor, (Instance smi) => !smi.HasMajorDisease()).ToggleUrge(Db.Get().Urges.RestDueToDisease).Update("AutoAssignClinic", delegate(Instance smi, float dt)
		{
			smi.AutoAssignClinic();
		}, UpdateRate.SIM_4000ms, false)
			.Exit(delegate(Instance smi)
			{
				smi.UnassignClinic();
			});
		post_nocheer.Enter(delegate(Instance smi)
		{
			if (smi.IsSleepingOrSleepSchedule())
			{
				smi.GoTo(healthy);
			}
			else
			{
				smi.GoTo(post);
			}
		});
		post.ToggleChore((Instance smi) => new EmoteChore(smi.master, Db.Get().ChoreTypes.EmoteHighPriority, SickPostKAnim, SickPostAnims, KAnim.PlayMode.Once, false), healthy);
	}
}
