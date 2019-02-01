using UnityEngine;

public class IncapacitationMonitor : GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			Health component = master.GetComponent<Health>();
			if ((bool)component)
			{
				component.CanBeIncapacitated = true;
			}
		}

		public void Bleed(float dt, Instance smi)
		{
			smi.sm.bleedOutStamina.Delta(dt * (0f - smi.sm.baseBleedOutSpeed.Get(smi)), smi);
		}

		public void RecoverStamina(float dt, Instance smi)
		{
			smi.sm.bleedOutStamina.Delta(Mathf.Min(dt * smi.sm.baseStaminaRecoverSpeed.Get(smi), smi.sm.maxBleedOutStamina.Get(smi) - smi.sm.bleedOutStamina.Get(smi)), smi);
		}

		public float GetBleedLifeTime(Instance smi)
		{
			return Mathf.Floor(smi.sm.bleedOutStamina.Get(smi) / smi.sm.baseBleedOutSpeed.Get(smi));
		}

		public Death GetCauseOfIncapacitation()
		{
			KPrefabID component = GetComponent<KPrefabID>();
			if (!component.HasTag(GameTags.CaloriesDepleted))
			{
				if (!component.HasTag(GameTags.HitPointsDepleted))
				{
					return Db.Get().Deaths.Generic;
				}
				return Db.Get().Deaths.Slain;
			}
			return Db.Get().Deaths.Starvation;
		}
	}

	public State healthy;

	public State start_recovery;

	public State Incapacitated;

	public State die;

	private FloatParameter bleedOutStamina = new FloatParameter(120f);

	private FloatParameter baseBleedOutSpeed = new FloatParameter(1f);

	private FloatParameter baseStaminaRecoverSpeed = new FloatParameter(1f);

	private FloatParameter maxBleedOutStamina = new FloatParameter(120f);

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = healthy;
		base.serializable = true;
		healthy.TagTransition(GameTags.CaloriesDepleted, Incapacitated, false).TagTransition(GameTags.HitPointsDepleted, Incapacitated, false).Update(delegate(Instance smi, float dt)
		{
			smi.RecoverStamina(dt, smi);
		}, UpdateRate.SIM_200ms, false);
		start_recovery.TagTransition(new Tag[2]
		{
			GameTags.CaloriesDepleted,
			GameTags.HitPointsDepleted
		}, healthy, true);
		Incapacitated.EventTransition(GameHashes.IncapacitationRecovery, start_recovery, null).ToggleTag(GameTags.Incapacitated).ToggleRecurringChore((Instance smi) => new BeIncapacitatedChore(smi.master), null)
			.ParamTransition(bleedOutStamina, die, GameStateMachine<IncapacitationMonitor, Instance, IStateMachineTarget, object>.IsLTEZero)
			.ToggleUrge(Db.Get().Urges.BeIncapacitated)
			.Enter(delegate(Instance smi)
			{
				smi.master.Trigger(-1506500077, null);
			})
			.Update(delegate(Instance smi, float dt)
			{
				smi.Bleed(dt, smi);
			}, UpdateRate.SIM_200ms, false);
		die.Enter(delegate(Instance smi)
		{
			smi.master.gameObject.GetSMI<DeathMonitor.Instance>().Kill(smi.GetCauseOfIncapacitation());
		});
	}
}
