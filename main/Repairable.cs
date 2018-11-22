using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Repairable : Workable
{
	public class SMInstance : GameStateMachine<States, SMInstance, Repairable, object>.GameInstance
	{
		private const float REQUIRED_MASS_SCALE = 0.1f;

		public SMInstance(Repairable smi)
			: base(smi)
		{
		}

		public bool HasRequiredMass()
		{
			PrimaryElement component = GetComponent<PrimaryElement>();
			float num = component.Mass * 0.1f;
			Storage storageProxy = base.smi.master.storageProxy;
			PrimaryElement primaryElement = storageProxy.FindPrimaryElement(component.ElementID);
			return (UnityEngine.Object)primaryElement != (UnityEngine.Object)null && primaryElement.Mass >= num;
		}

		public KeyValuePair<Tag, float> GetRequiredMass()
		{
			PrimaryElement component = GetComponent<PrimaryElement>();
			float num = component.Mass * 0.1f;
			Storage storageProxy = base.smi.master.storageProxy;
			PrimaryElement primaryElement = storageProxy.FindPrimaryElement(component.ElementID);
			float value = (!((UnityEngine.Object)primaryElement != (UnityEngine.Object)null)) ? num : Math.Max(0f, num - primaryElement.Mass);
			return new KeyValuePair<Tag, float>(component.Element.tag, value);
		}

		public void ConsumeRepairMaterials()
		{
			base.smi.master.storageProxy.ConsumeAllIgnoringDisease();
		}

		public void DestroyStorageProxy()
		{
			if ((UnityEngine.Object)base.smi.master.storageProxy != (UnityEngine.Object)null)
			{
				base.smi.master.transform.GetComponent<Prioritizable>().RemoveRef();
				base.smi.master.storageProxy.DropAll(false);
				Util.KDestroyGameObject(base.smi.master.storageProxy.gameObject);
			}
		}

		public bool NeedsRepairs()
		{
			return base.smi.master.GetComponent<BuildingHP>().NeedsRepairs;
		}
	}

	public class States : GameStateMachine<States, SMInstance, Repairable>
	{
		public class AllowedState : State
		{
			public State needMass;

			public State repairable;
		}

		public Signal allow;

		public Signal forbid;

		public State forbidden;

		public AllowedState allowed;

		public State repaired;

		public static readonly Chore.Precondition IsNotBeingAttacked = new Chore.Precondition
		{
			id = "IsNotBeingAttacked",
			description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_NOT_BEING_ATTACKED,
			fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
			{
				bool result = true;
				if (data != null)
				{
					Breakable breakable = (Breakable)data;
					result = ((UnityEngine.Object)breakable.worker == (UnityEngine.Object)null);
				}
				return result;
			}
		};

		public static readonly Chore.Precondition IsNotAngry = new Chore.Precondition
		{
			id = "IsNotAngry",
			description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_NOT_ANGRY,
			fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
			{
				Traits traits = context.consumerState.traits;
				AmountInstance amountInstance = Db.Get().Amounts.Stress.Lookup(context.consumerState.gameObject);
				if ((UnityEngine.Object)traits != (UnityEngine.Object)null && amountInstance != null && amountInstance.value >= STRESS.ACTING_OUT_RESET && traits.HasTrait("Aggressive"))
				{
					return false;
				}
				return true;
			}
		};

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = repaired;
			base.serializable = true;
			forbidden.OnSignal(allow, repaired);
			allowed.Enter(delegate(SMInstance smi)
			{
				smi.master.CreateStorageProxy();
			}).DefaultState(allowed.needMass).EventHandler(GameHashes.BuildingFullyRepaired, delegate(SMInstance smi)
			{
				smi.ConsumeRepairMaterials();
			})
				.EventTransition(GameHashes.BuildingFullyRepaired, repaired, null)
				.OnSignal(forbid, forbidden)
				.Exit(delegate(SMInstance smi)
				{
					smi.DestroyStorageProxy();
				});
			allowed.needMass.Enter(delegate(SMInstance smi)
			{
				Prioritizable.AddRef(smi.master.storageProxy.transform.parent.gameObject);
			}).Exit(delegate(SMInstance smi)
			{
				if (!smi.isMasterNull && (UnityEngine.Object)smi.master.storageProxy != (UnityEngine.Object)null)
				{
					Prioritizable.RemoveRef(smi.master.storageProxy.transform.parent.gameObject);
				}
			}).EventTransition(GameHashes.OnStorageChange, allowed.repairable, (SMInstance smi) => smi.HasRequiredMass())
				.ToggleChore(CreateFetchChore, allowed.repairable, allowed.needMass)
				.ToggleStatusItem(Db.Get().BuildingStatusItems.WaitingForRepairMaterials, (SMInstance smi) => smi.GetRequiredMass());
			allowed.repairable.ToggleRecurringChore(CreateRepairChore, null).ToggleStatusItem(Db.Get().BuildingStatusItems.PendingRepair, (object)null);
			repaired.EventTransition(GameHashes.BuildingReceivedDamage, allowed, (SMInstance smi) => smi.NeedsRepairs()).OnSignal(allow, allowed).OnSignal(forbid, forbidden);
		}

		private Chore CreateFetchChore(SMInstance smi)
		{
			PrimaryElement component = smi.master.GetComponent<PrimaryElement>();
			Storage storageProxy = smi.master.storageProxy;
			PrimaryElement primaryElement = storageProxy.FindPrimaryElement(component.ElementID);
			float amount = component.Mass * 0.1f - ((!((UnityEngine.Object)primaryElement != (UnityEngine.Object)null)) ? 0f : primaryElement.Mass);
			Tag[] tags = new Tag[1]
			{
				GameTagExtensions.Create(component.ElementID)
			};
			return new FetchChore(Db.Get().ChoreTypes.Fetch, smi.master.storageProxy, amount, tags, null, null, null, true, null, null, null, FetchOrder2.OperationalRequirement.None, 0, null);
		}

		private Chore CreateRepairChore(SMInstance smi)
		{
			WorkChore<Repairable> workChore = new WorkChore<Repairable>(Db.Get().ChoreTypes.Repair, smi.master, null, null, true, null, null, null, true, null, false, false, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, true);
			Deconstructable component = smi.master.GetComponent<Deconstructable>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				workChore.AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDeconstruction, component);
			}
			Breakable component2 = smi.master.GetComponent<Breakable>();
			if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
			{
				workChore.AddPrecondition(IsNotBeingAttacked, component2);
			}
			workChore.AddPrecondition(IsNotAngry, null);
			return workChore;
		}
	}

	public float expectedRepairTime = -1f;

	[MyCmpGet]
	private BuildingHP hp;

	private SMInstance smi;

	private Storage storageProxy;

	[Serialize]
	private byte[] storedData;

	private float timeSpentRepairing = 0f;

	private static readonly Operational.Flag repairedFlag = new Operational.Flag("repaired", Operational.Flag.Type.Functional);

	private static readonly EventSystem.IntraObjectHandler<Repairable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Repairable>(delegate(Repairable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		showProgressBar = false;
		faceTargetWhenWorking = true;
		multitoolContext = "build";
		multitoolHitEffectTag = EffectConfigs.BuildSplashId;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		smi = new SMInstance(this);
		smi.StartSM();
		workTime = float.PositiveInfinity;
		workTimeRemaining = float.PositiveInfinity;
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole(Handyman.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
	}

	private void OnProxyStorageChanged(object data)
	{
		Trigger(-1697596308, data);
	}

	protected override void OnLoadLevel()
	{
		smi = null;
		base.OnLoadLevel();
	}

	protected override void OnCleanUp()
	{
		if (smi != null)
		{
			smi.StopSM("Destroy Repairable");
		}
		base.OnCleanUp();
	}

	private void OnRefreshUserMenu(object data)
	{
		if ((UnityEngine.Object)base.gameObject != (UnityEngine.Object)null && smi != null)
		{
			StateMachine.BaseState currentState = smi.GetCurrentState();
			if (currentState == smi.sm.forbidden)
			{
				UserMenu userMenu = Game.Instance.userMenu;
				GameObject gameObject = base.gameObject;
				string iconName = "action_repair";
				string text = STRINGS.BUILDINGS.REPAIRABLE.ENABLE_AUTOREPAIR.NAME;
				System.Action on_click = AllowRepair;
				string tooltipText = STRINGS.BUILDINGS.REPAIRABLE.ENABLE_AUTOREPAIR.TOOLTIP;
				userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true), 1f);
			}
			else
			{
				UserMenu userMenu2 = Game.Instance.userMenu;
				GameObject gameObject2 = base.gameObject;
				string tooltipText = "action_repair";
				string text = STRINGS.BUILDINGS.REPAIRABLE.DISABLE_AUTOREPAIR.NAME;
				System.Action on_click = CancelRepair;
				string iconName = STRINGS.BUILDINGS.REPAIRABLE.DISABLE_AUTOREPAIR.TOOLTIP;
				userMenu2.AddButton(gameObject2, new KIconButtonMenu.ButtonInfo(tooltipText, text, on_click, Action.NumActions, null, null, null, iconName, true), 1f);
			}
		}
	}

	private void AllowRepair()
	{
		if (DebugHandler.InstantBuildMode)
		{
			hp.Repair(hp.MaxHitPoints);
			OnCompleteWork(null);
		}
		smi.sm.allow.Trigger(smi);
		OnRefreshUserMenu(null);
	}

	public void CancelRepair()
	{
		if (smi != null)
		{
			smi.sm.forbid.Trigger(smi);
		}
		OnRefreshUserMenu(null);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		Operational component = GetComponent<Operational>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.SetFlag(repairedFlag, false);
		}
		timeSpentRepairing = 0f;
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		PrimaryElement component = GetComponent<PrimaryElement>();
		float num = Mathf.Sqrt(component.Mass);
		float num2 = (!(expectedRepairTime < 0f)) ? expectedRepairTime : num;
		float num3 = num2 * 0.1f;
		if (timeSpentRepairing >= num3)
		{
			timeSpentRepairing -= num3;
			int num4 = 0;
			if ((UnityEngine.Object)worker != (UnityEngine.Object)null)
			{
				AttributeInstance attributeInstance = Db.Get().Attributes.Machinery.Lookup(worker);
				num4 = (int)attributeInstance.GetTotalValue();
			}
			int num5 = 10 + Math.Max(0, num4 * 10);
			int repair_amount = Mathf.CeilToInt((float)num5 * 0.1f);
			hp.Repair(repair_amount);
			if (hp.HitPoints >= hp.MaxHitPoints)
			{
				return true;
			}
		}
		timeSpentRepairing += dt;
		return false;
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		Operational component = GetComponent<Operational>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.SetFlag(repairedFlag, true);
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Operational component = GetComponent<Operational>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.SetFlag(repairedFlag, true);
		}
	}

	public void CreateStorageProxy()
	{
		if ((UnityEngine.Object)storageProxy == (UnityEngine.Object)null)
		{
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(RepairableStorageProxy.ID), base.transform.gameObject, null);
			gameObject.transform.SetLocalPosition(Vector3.zero);
			storageProxy = gameObject.GetComponent<Storage>();
			storageProxy.prioritizable = base.transform.GetComponent<Prioritizable>();
			storageProxy.prioritizable.AddRef();
			gameObject.SetActive(true);
		}
	}

	[OnSerializing]
	private void OnSerializing()
	{
		storedData = null;
		if ((UnityEngine.Object)storageProxy != (UnityEngine.Object)null && !storageProxy.IsEmpty())
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter writer = new BinaryWriter(memoryStream))
				{
					storageProxy.Serialize(writer);
				}
				storedData = memoryStream.ToArray();
			}
		}
	}

	[OnSerialized]
	private void OnSerialized()
	{
		storedData = null;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (storedData != null)
		{
			FastReader reader = new FastReader(storedData);
			CreateStorageProxy();
			storageProxy.Deserialize(reader);
			storedData = null;
		}
	}
}
