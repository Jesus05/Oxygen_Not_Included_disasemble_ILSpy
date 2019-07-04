using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SerializationConfig(MemberSerialization.OptIn)]
public class BuildingHP : Workable
{
	public struct DamageSourceInfo
	{
		public int damage;

		public string source;

		public string popString;

		public SpawnFXHashes takeDamageEffect;

		public string fullDamageEffectName;

		public string statusItemID;

		public override string ToString()
		{
			return source;
		}
	}

	public class SMInstance : GameStateMachine<States, SMInstance, BuildingHP, object>.GameInstance
	{
		private ProgressBar progressBar = null;

		public SMInstance(BuildingHP master)
			: base(master)
		{
		}

		public Notification CreateBrokenMachineNotification()
		{
			return new Notification(MISC.NOTIFICATIONS.BROKENMACHINE.NAME, NotificationType.BadMinor, HashedString.Invalid, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.BROKENMACHINE.TOOLTIP + notificationList.ReduceMessages(false), "/t• " + base.master.damageSourceInfo.source, false, 0f, null, null, null);
		}

		public void ShowProgressBar(bool show)
		{
			if (show && Grid.IsValidCell(Grid.PosToCell(base.gameObject)) && Grid.IsVisible(Grid.PosToCell(base.gameObject)))
			{
				CreateProgressBar();
			}
			else if ((UnityEngine.Object)progressBar != (UnityEngine.Object)null)
			{
				progressBar.gameObject.DeleteObject();
				progressBar = null;
			}
		}

		public void UpdateMeter()
		{
			if ((UnityEngine.Object)progressBar == (UnityEngine.Object)null)
			{
				ShowProgressBar(true);
			}
			if ((bool)progressBar)
			{
				progressBar.Update();
			}
		}

		private float HealthPercent()
		{
			return (float)base.smi.master.HitPoints / (float)base.smi.master.building.Def.HitPoints;
		}

		private void CreateProgressBar()
		{
			if (!((UnityEngine.Object)progressBar != (UnityEngine.Object)null))
			{
				progressBar = Util.KInstantiateUI<ProgressBar>(ProgressBarsConfig.Instance.progressBarPrefab, null, false);
				progressBar.transform.SetParent(GameScreenManager.Instance.worldSpaceCanvas.transform);
				progressBar.name = base.smi.master.name + "." + base.smi.master.GetType().Name + " ProgressBar";
				progressBar.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("ProgressBar");
				progressBar.SetUpdateFunc(HealthPercent);
				progressBar.barColor = ProgressBarsConfig.Instance.GetBarColor("HealthBar");
				CanvasGroup component = progressBar.GetComponent<CanvasGroup>();
				component.interactable = false;
				component.blocksRaycasts = false;
				progressBar.Update();
				float d = 0.15f;
				Vector3 vector = base.gameObject.transform.GetPosition() + Vector3.down * d;
				vector.z += 0.05f;
				Rotatable component2 = GetComponent<Rotatable>();
				vector = ((!((UnityEngine.Object)component2 == (UnityEngine.Object)null) && component2.GetOrientation() != 0 && base.smi.master.building.Def.WidthInCells >= 2 && base.smi.master.building.Def.HeightInCells >= 2) ? (vector + Vector3.left * (1f + 0.5f * (float)(base.smi.master.building.Def.WidthInCells % 2))) : (vector - Vector3.right * 0.5f * (float)(base.smi.master.building.Def.WidthInCells % 2)));
				progressBar.transform.SetPosition(vector);
				progressBar.gameObject.SetActive(true);
			}
		}

		private static string ToolTipResolver(List<Notification> notificationList, object data)
		{
			string text = "";
			for (int i = 0; i < notificationList.Count; i++)
			{
				Notification notification = notificationList[i];
				text += string.Format(BUILDINGS.DAMAGESOURCES.NOTIFICATION_TOOLTIP, notification.NotifierName, (string)notification.tooltipData);
				if (i < notificationList.Count - 1)
				{
					text += "\n";
				}
			}
			return text;
		}

		public void ShowDamagedEffect()
		{
			if (base.master.damageSourceInfo.takeDamageEffect != 0)
			{
				BuildingDef def = base.master.GetComponent<BuildingComplete>().Def;
				int cell = Grid.PosToCell(base.master);
				int cell2 = Grid.OffsetCell(cell, 0, def.HeightInCells - 1);
				Game.Instance.SpawnFX(base.master.damageSourceInfo.takeDamageEffect, cell2, 0f);
			}
		}

		public FXAnim.Instance InstantiateDamageFX()
		{
			if (base.master.damageSourceInfo.fullDamageEffectName != null)
			{
				BuildingDef def = base.master.GetComponent<BuildingComplete>().Def;
				Vector3 zero = Vector3.zero;
				return new FXAnim.Instance(offset: (def.HeightInCells > 1) ? new Vector3(0f, (float)(def.HeightInCells - 1), 0f) : new Vector3(0f, 0.5f, 0f), master: base.smi.master, kanim_file: base.master.damageSourceInfo.fullDamageEffectName, anim: "idle", mode: KAnim.PlayMode.Loop, tint_colour: Color.white);
			}
			return null;
		}

		public void SetCrackOverlayValue(float value)
		{
			KBatchedAnimController component = base.master.GetComponent<KBatchedAnimController>();
			if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
			{
				component.SetBlendValue(value);
				kbacQueryList.Clear();
				base.master.GetComponentsInChildren(kbacQueryList);
				for (int i = 0; i < kbacQueryList.Count; i++)
				{
					Meter meter = kbacQueryList[i];
					KBatchedAnimController component2 = meter.GetComponent<KBatchedAnimController>();
					component2.SetBlendValue(value);
				}
			}
		}
	}

	public class States : GameStateMachine<States, SMInstance, BuildingHP>
	{
		public class Healthy : State
		{
			public ImperfectStates imperfect;

			public State perfect;
		}

		public class ImperfectStates : State
		{
			public State playEffect;

			public State waiting;
		}

		private static readonly Operational.Flag healthyFlag = new Operational.Flag("healthy", Operational.Flag.Type.Functional);

		public State damaged;

		public Healthy healthy;

		public override void InitializeStates(out BaseState default_state)
		{
			base.serializable = true;
			default_state = healthy;
			healthy.DefaultState(healthy.imperfect).EventTransition(GameHashes.BuildingReceivedDamage, damaged, (SMInstance smi) => smi.master.HitPoints <= 0);
			healthy.imperfect.Enter(delegate(SMInstance smi)
			{
				smi.ShowProgressBar(true);
			}).DefaultState(healthy.imperfect.playEffect).EventTransition(GameHashes.BuildingPartiallyRepaired, healthy.perfect, (SMInstance smi) => smi.master.HitPoints == smi.master.building.Def.HitPoints)
				.EventHandler(GameHashes.BuildingPartiallyRepaired, delegate(SMInstance smi)
				{
					smi.UpdateMeter();
				})
				.ToggleStatusItem((SMInstance smi) => (smi.master.damageSourceInfo.statusItemID == null) ? null : Db.Get().BuildingStatusItems.Get(smi.master.damageSourceInfo.statusItemID), null)
				.Exit(delegate(SMInstance smi)
				{
					smi.ShowProgressBar(false);
				});
			healthy.imperfect.playEffect.Transition(healthy.imperfect.waiting, (SMInstance smi) => true, UpdateRate.SIM_200ms);
			healthy.imperfect.waiting.ScheduleGoTo((SMInstance smi) => UnityEngine.Random.Range(15f, 30f), healthy.imperfect.playEffect);
			healthy.perfect.EventTransition(GameHashes.BuildingReceivedDamage, healthy.imperfect, (SMInstance smi) => smi.master.HitPoints < smi.master.building.Def.HitPoints);
			damaged.Enter(delegate(SMInstance smi)
			{
				Operational component2 = smi.GetComponent<Operational>();
				if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
				{
					component2.SetFlag(healthyFlag, false);
				}
				smi.ShowProgressBar(true);
				smi.master.Trigger(774203113, smi.master);
				smi.SetCrackOverlayValue(1f);
			}).ToggleNotification((SMInstance smi) => smi.CreateBrokenMachineNotification()).ToggleStatusItem(Db.Get().BuildingStatusItems.Broken, (object)null)
				.ToggleFX((SMInstance smi) => smi.InstantiateDamageFX())
				.EventTransition(GameHashes.BuildingPartiallyRepaired, healthy.perfect, (SMInstance smi) => smi.master.HitPoints == smi.master.building.Def.HitPoints)
				.EventHandler(GameHashes.BuildingPartiallyRepaired, delegate(SMInstance smi)
				{
					smi.UpdateMeter();
				})
				.Exit(delegate(SMInstance smi)
				{
					Operational component = smi.GetComponent<Operational>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null)
					{
						component.SetFlag(healthyFlag, true);
					}
					smi.ShowProgressBar(false);
					smi.SetCrackOverlayValue(0f);
				});
		}

		private Chore CreateRepairChore(SMInstance smi)
		{
			return new WorkChore<BuildingHP>(Db.Get().ChoreTypes.Repair, smi.master, null, true, null, null, null, true, null, false, false, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
		}
	}

	[Serialize]
	[SerializeField]
	private int hitpoints;

	[Serialize]
	private DamageSourceInfo damageSourceInfo;

	private static readonly EventSystem.IntraObjectHandler<BuildingHP> OnDoBuildingDamageDelegate = new EventSystem.IntraObjectHandler<BuildingHP>(delegate(BuildingHP component, object data)
	{
		component.OnDoBuildingDamage(data);
	});

	private static readonly EventSystem.IntraObjectHandler<BuildingHP> DestroyOnDamagedDelegate = new EventSystem.IntraObjectHandler<BuildingHP>(delegate(BuildingHP component, object data)
	{
		component.DestroyOnDamaged(data);
	});

	public static List<Meter> kbacQueryList = new List<Meter>();

	public bool destroyOnDamaged = false;

	public bool invincible = false;

	[MyCmpGet]
	private Building building;

	private SMInstance smi;

	private float minDamagePopInterval = 4f;

	private float lastPopTime = 0f;

	public int HitPoints => hitpoints;

	public int MaxHitPoints => building.Def.HitPoints;

	public bool IsBroken => hitpoints == 0;

	public bool NeedsRepairs => HitPoints < building.Def.HitPoints;

	public void SetHitPoints(int hp)
	{
		hitpoints = hp;
	}

	public DamageSourceInfo GetDamageSourceInfo()
	{
		return damageSourceInfo;
	}

	protected override void OnLoadLevel()
	{
		smi = null;
		base.OnLoadLevel();
	}

	public void DoDamage(int damage)
	{
		if (!invincible)
		{
			damage = Math.Max(0, damage);
			hitpoints = Math.Max(0, hitpoints - damage);
			Trigger(-1964935036, this);
		}
	}

	public void Repair(int repair_amount)
	{
		if (hitpoints + repair_amount < hitpoints)
		{
			hitpoints = building.Def.HitPoints;
		}
		else
		{
			hitpoints = Math.Min(hitpoints + repair_amount, building.Def.HitPoints);
		}
		Trigger(-1699355994, this);
		if (hitpoints >= building.Def.HitPoints)
		{
			Trigger(-1735440190, this);
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		SetWorkTime(10f);
		multitoolContext = "build";
		multitoolHitEffectTag = EffectConfigs.BuildSplashId;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		smi = new SMInstance(this);
		smi.StartSM();
		Subscribe(-794517298, OnDoBuildingDamageDelegate);
		if (destroyOnDamaged)
		{
			Subscribe(774203113, DestroyOnDamagedDelegate);
		}
		if (hitpoints <= 0)
		{
			Trigger(774203113, this);
		}
	}

	private void DestroyOnDamaged(object data)
	{
		Util.KDestroyGameObject(base.gameObject);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		AttributeInstance attributeInstance = Db.Get().Attributes.Machinery.Lookup(worker);
		int num = (int)attributeInstance.GetTotalValue();
		int repair_amount = 10 + Math.Max(0, num * 10);
		Repair(repair_amount);
	}

	private void OnDoBuildingDamage(object data)
	{
		if (!invincible)
		{
			damageSourceInfo = (DamageSourceInfo)data;
			DoDamage(damageSourceInfo.damage);
			DoDamagePopFX(damageSourceInfo);
			DoTakeDamageFX(damageSourceInfo);
		}
	}

	private void DoTakeDamageFX(DamageSourceInfo info)
	{
		if (info.takeDamageEffect != 0)
		{
			BuildingDef def = GetComponent<BuildingComplete>().Def;
			int cell = Grid.PosToCell(this);
			int cell2 = Grid.OffsetCell(cell, 0, def.HeightInCells - 1);
			Game.Instance.SpawnFX(info.takeDamageEffect, cell2, 0f);
		}
	}

	private void DoDamagePopFX(DamageSourceInfo info)
	{
		if (info.popString != null && Time.time > lastPopTime + minDamagePopInterval)
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Building, info.popString, base.gameObject.transform, 1.5f, false);
			lastPopTime = Time.time;
		}
	}
}
