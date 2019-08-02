using Klei.AI;
using KSerialization;
using System;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Health : KMonoBehaviour, ISaveLoadable
{
	public enum HealthState
	{
		Perfect,
		Alright,
		Scuffed,
		Injured,
		Critical,
		Incapacitated,
		Dead,
		Invincible
	}

	[Serialize]
	public bool CanBeIncapacitated;

	[Serialize]
	public HealthState State;

	public HealthBar healthBar;

	private Effects effects;

	private AmountInstance amountInstance;

	public AmountInstance GetAmountInstance => amountInstance;

	public float hitPoints
	{
		get
		{
			return amountInstance.value;
		}
		set
		{
			amountInstance.value = value;
		}
	}

	public float maxHitPoints => amountInstance.GetMax();

	public float percent()
	{
		return hitPoints / maxHitPoints;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.Health.Add(this);
		amountInstance = Db.Get().Amounts.HitPoints.Lookup(base.gameObject);
		amountInstance.value = amountInstance.GetMax();
		AmountInstance obj = amountInstance;
		obj.OnDelta = (Action<float>)Delegate.Combine(obj.OnDelta, new Action<float>(OnHealthChanged));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (State == HealthState.Incapacitated || hitPoints == 0f)
		{
			if (CanBeIncapacitated)
			{
				Incapacitate(Db.Get().Deaths.Slain);
			}
			else
			{
				Kill();
			}
		}
		if (State != HealthState.Incapacitated && State != HealthState.Dead)
		{
			UpdateStatus();
		}
		effects = GetComponent<Effects>();
		UpdateHealthBar();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Health.Remove(this);
	}

	public void UpdateHealthBar()
	{
		if (!((UnityEngine.Object)NameDisplayScreen.Instance == (UnityEngine.Object)null))
		{
			bool flag = State == HealthState.Dead || State == HealthState.Incapacitated || hitPoints >= maxHitPoints;
			NameDisplayScreen.Instance.SetHealthDisplay(base.gameObject, percent, !flag);
		}
	}

	private void Recover()
	{
		GetComponent<KPrefabID>().RemoveTag(GameTags.HitPointsDepleted);
	}

	public void OnHealthChanged(float delta)
	{
		Trigger(-1664904872, delta);
		if (State != HealthState.Invincible)
		{
			if (hitPoints == 0f && !IsDefeated())
			{
				if (CanBeIncapacitated)
				{
					Incapacitate(Db.Get().Deaths.Slain);
				}
				else
				{
					Kill();
				}
			}
			else
			{
				GetComponent<KPrefabID>().RemoveTag(GameTags.HitPointsDepleted);
			}
		}
		UpdateStatus();
		UpdateWoundEffects();
		UpdateHealthBar();
	}

	public void RegisterHitReaction()
	{
		ReactionMonitor.Instance sMI = base.gameObject.GetSMI<ReactionMonitor.Instance>();
		if (sMI != null)
		{
			SelfEmoteReactable selfEmoteReactable = new SelfEmoteReactable(base.gameObject, "Hit", Db.Get().ChoreTypes.Cough, "anim_hits_kanim", 0f, 1f, 1f);
			selfEmoteReactable.AddStep(new EmoteReactable.EmoteStep
			{
				anim = (HashedString)"hit"
			});
			if (!base.gameObject.GetComponent<Navigator>().IsMoving())
			{
				EmoteChore emoteChore = new EmoteChore(base.gameObject.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteIdle, "anim_hits_kanim", new HashedString[1]
				{
					"hit"
				}, null);
				emoteChore.PairReactable(selfEmoteReactable);
				selfEmoteReactable.PairEmote(emoteChore);
			}
			sMI.AddOneshotReactable(selfEmoteReactable);
		}
	}

	[ContextMenu("DoDamage")]
	public void DoDamage()
	{
		Damage(1f);
	}

	public void Damage(float amount)
	{
		if (State != HealthState.Invincible)
		{
			hitPoints = Mathf.Max(0f, hitPoints - amount);
		}
		OnHealthChanged(0f - amount);
	}

	private void UpdateWoundEffects()
	{
		if ((bool)effects)
		{
			switch (State)
			{
			case HealthState.Perfect:
				effects.Remove("LightWounds");
				effects.Remove("ModerateWounds");
				effects.Remove("SevereWounds");
				break;
			case HealthState.Alright:
				effects.Remove("LightWounds");
				effects.Remove("ModerateWounds");
				effects.Remove("SevereWounds");
				break;
			case HealthState.Scuffed:
				effects.Remove("ModerateWounds");
				effects.Remove("SevereWounds");
				if (!effects.HasEffect("LightWounds"))
				{
					effects.Add("LightWounds", true);
				}
				break;
			case HealthState.Injured:
				effects.Remove("LightWounds");
				effects.Remove("SevereWounds");
				if (!effects.HasEffect("ModerateWounds"))
				{
					effects.Add("ModerateWounds", true);
				}
				break;
			case HealthState.Critical:
				effects.Remove("LightWounds");
				effects.Remove("ModerateWounds");
				if (!effects.HasEffect("SevereWounds"))
				{
					effects.Add("SevereWounds", true);
				}
				break;
			case HealthState.Incapacitated:
				effects.Remove("LightWounds");
				effects.Remove("ModerateWounds");
				effects.Remove("SevereWounds");
				break;
			case HealthState.Dead:
				effects.Remove("LightWounds");
				effects.Remove("ModerateWounds");
				effects.Remove("SevereWounds");
				break;
			}
		}
	}

	private void UpdateStatus()
	{
		float num = hitPoints / maxHitPoints;
		HealthState healthState = (State == HealthState.Invincible) ? HealthState.Invincible : ((!(num >= 1f)) ? ((num >= 0.85f) ? HealthState.Alright : ((num >= 0.66f) ? HealthState.Scuffed : (((double)num >= 0.33) ? HealthState.Injured : ((num > 0f) ? HealthState.Critical : ((num != 0f) ? HealthState.Dead : HealthState.Incapacitated))))) : HealthState.Perfect);
		if (State != healthState)
		{
			if (State == HealthState.Incapacitated && healthState != HealthState.Dead)
			{
				Recover();
			}
			if (healthState == HealthState.Perfect)
			{
				Trigger(-1491582671, this);
			}
			State = healthState;
			KSelectable component = GetComponent<KSelectable>();
			if (State != HealthState.Dead && State != 0 && State != HealthState.Alright)
			{
				component.SetStatusItem(Db.Get().StatusItemCategories.Hitpoints, Db.Get().CreatureStatusItems.HealthStatus, State);
			}
			else
			{
				component.SetStatusItem(Db.Get().StatusItemCategories.Hitpoints, null, null);
			}
		}
	}

	public bool IsIncapacitated()
	{
		return State == HealthState.Incapacitated;
	}

	public bool IsDefeated()
	{
		return State == HealthState.Incapacitated || State == HealthState.Dead;
	}

	public void Incapacitate(Death source_of_death)
	{
		State = HealthState.Incapacitated;
		GetComponent<KPrefabID>().AddTag(GameTags.HitPointsDepleted, false);
	}

	private void Kill()
	{
		DeathMonitor.Instance sMI = base.gameObject.GetSMI<DeathMonitor.Instance>();
		if (sMI != null)
		{
			base.gameObject.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Slain);
		}
	}
}
