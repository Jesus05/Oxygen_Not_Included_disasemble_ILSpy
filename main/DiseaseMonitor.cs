using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class DiseaseMonitor : GameStateMachine<DiseaseMonitor, DiseaseMonitor.Instance>
{
	public class NotifyStates : State
	{
		public State notify;

		public State cooldown;
	}

	public class SickStates : State
	{
		public NotifyStates notify;
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

		public void AutoAssignClinic()
		{
			Ownables component = base.sm.masterTarget.Get(base.smi).GetComponent<Ownables>();
			AssignableSlot clinic = Db.Get().AssignableSlots.Clinic;
			AssignableSlotInstance slot = component.GetSlot(clinic);
			if (slot != null && !((Object)slot.assignable != (Object)null))
			{
				component.AutoAssignSlot(clinic);
			}
		}

		public void UnassignClinic()
		{
			Ownables component = base.sm.masterTarget.Get(base.smi).GetComponent<Ownables>();
			AssignableSlot clinic = Db.Get().AssignableSlots.Clinic;
			component.GetSlot(clinic)?.Unassign(true);
		}

		public bool IsNightTime()
		{
			return TimeOfDay.Instance.GetCurrentTimeRegion() == TimeOfDay.TimeRegion.Night;
		}
	}

	public State healthy;

	public SickStates sick;

	public State post;

	public State post_nocheer;

	private static readonly HashedString[] SickAnims = new HashedString[2]
	{
		"idle_pre",
		"idle_default"
	};

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
		sick.DefaultState(sick.notify).EventTransition(GameHashes.DiseaseCured, post_nocheer, (Instance smi) => !smi.IsSick()).ToggleAnims("anim_idle_sick_kanim", 0f)
			.ToggleExpression(Db.Get().Expressions.Sick, null)
			.ToggleUrge(Db.Get().Urges.RestDueToDisease)
			.Update("AutoAssignClinic", delegate(Instance smi, float dt)
			{
				smi.AutoAssignClinic();
			}, UpdateRate.SIM_4000ms, false)
			.Exit(delegate(Instance smi)
			{
				smi.UnassignClinic();
			});
		sick.notify.DefaultState(sick.notify.notify);
		sick.notify.notify.ToggleThought(Db.Get().Thoughts.GotInfected, null).ToggleChore((Instance smi) => new EmoteChore(smi.master, Db.Get().ChoreTypes.Emote, SickAnims, null), sick.notify.cooldown);
		sick.notify.cooldown.ScheduleGoTo(5f, sick.notify);
		post_nocheer.Enter(delegate(Instance smi)
		{
			if (smi.IsNightTime())
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
