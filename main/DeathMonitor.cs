using STRINGS;
using UnityEngine;

public class DeathMonitor : GameStateMachine<DeathMonitor, DeathMonitor.Instance, IStateMachineTarget, DeathMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public class Dead : State
	{
		public State ground;

		public State carried;
	}

	public new class Instance : GameInstance
	{
		private bool isDuplicant;

		public bool IsDuplicant => isDuplicant;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			isDuplicant = GetComponent<MinionIdentity>();
		}

		public void Kill(Death death)
		{
			base.sm.death.Set(death, base.smi);
		}

		public void PickedUp(object data = null)
		{
			if (data is Storage || (data != null && (bool)data))
			{
				base.smi.GoTo(base.sm.dead.carried);
			}
		}

		public bool IsDead()
		{
			return base.smi.IsInsideState(base.smi.sm.dead);
		}

		public void ApplyDeath()
		{
			if (isDuplicant)
			{
				GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.Dead, base.smi.sm.death.Get(base.smi));
				float value = 600f - GameClock.Instance.GetTimeSinceStartOfReport();
				ReportManager.Instance.ReportValue(ReportManager.ReportType.PersonalTime, value, string.Format(UI.ENDOFDAYREPORT.NOTES.PERSONAL_TIME, DUPLICANTS.CHORES.IS_DEAD_TASK), base.smi.master.gameObject.GetProperName());
				Pickupable component = GetComponent<Pickupable>();
				if ((Object)component != (Object)null)
				{
					component.RegisterListeners();
				}
			}
			GetComponent<KPrefabID>().AddTag(GameTags.Corpse, false);
		}
	}

	public State alive;

	public State dying_duplicant;

	public State dying_creature;

	public State die;

	public Dead dead;

	public ResourceParameter<Death> death;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = alive;
		base.serializable = true;
		alive.ParamTransition(death, dying_duplicant, (Instance smi, Death p) => p != null && smi.IsDuplicant).ParamTransition(death, dying_creature, (Instance smi, Death p) => p != null && !smi.IsDuplicant);
		dying_duplicant.ToggleAnims("anim_emotes_default_kanim", 0f).ToggleTag(GameTags.Dying).ToggleChore((Instance smi) => new DieChore(smi.master, death.Get(smi)), die);
		dying_creature.ToggleBehaviour(GameTags.Creatures.Die, (Instance smi) => true, delegate(Instance smi)
		{
			smi.GoTo(dead);
		});
		die.ToggleTag(GameTags.Dying).Enter("Die", delegate(Instance smi)
		{
			Death death2 = death.Get(smi);
			if (smi.IsDuplicant)
			{
				DeathMessage message = new DeathMessage(smi.gameObject, death2);
				KFMOD.PlayOneShot(GlobalAssets.GetSound("Death_Notification_localized", false), smi.master.transform.GetPosition());
				KFMOD.PlayOneShot(GlobalAssets.GetSound("Death_Notification_ST", false));
				Messenger.Instance.QueueMessage(message);
			}
		}).GoTo(dead);
		dead.ToggleAnims("anim_emotes_default_kanim", 0f).defaultState = dead.ground.TriggerOnEnter(GameHashes.Died, null).ToggleTag(GameTags.Dead).Enter(delegate(Instance smi)
		{
			smi.ApplyDeath();
			Game.Instance.Trigger(282337316, smi.gameObject);
		});
		dead.ground.Enter(delegate(Instance smi)
		{
			Death death = this.death.Get(smi);
			if (death == null)
			{
				death = Db.Get().Deaths.Generic;
			}
			if (smi.IsDuplicant)
			{
				smi.GetComponent<KAnimControllerBase>().Play(death.loopAnim, KAnim.PlayMode.Loop, 1f, 0f);
			}
		}).EventTransition(GameHashes.OnStore, dead.carried, (Instance smi) => smi.IsDuplicant && smi.HasTag(GameTags.Stored));
		dead.carried.ToggleAnims("anim_dead_carried_kanim", 0f).PlayAnim("idle_default", KAnim.PlayMode.Loop).EventTransition(GameHashes.OnStore, dead.ground, (Instance smi) => !smi.HasTag(GameTags.Stored));
	}
}
