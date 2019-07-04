using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Toilet : StateMachineComponent<Toilet.StatesInstance>, ISaveLoadable, IUsable, IEffectDescriptor, IGameObjectEffectDescriptor
{
	[Serializable]
	public struct SpawnInfo
	{
		[HashedEnum]
		public SimHashes elementID;

		public float mass;

		public float interval;

		public SpawnInfo(SimHashes element_id, float mass, float interval)
		{
			elementID = element_id;
			this.mass = mass;
			this.interval = interval;
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, Toilet, object>.GameInstance
	{
		public Chore cleanChore;

		public List<Chore> activeUseChores;

		public float monsterSpawnTime = 1200f;

		public bool IsSoiled => base.master.FlushesUsed > 0;

		public StatesInstance(Toilet master)
			: base(master)
		{
			activeUseChores = new List<Chore>();
		}

		public int GetFlushesRemaining()
		{
			return base.master.maxFlushes - base.master.FlushesUsed;
		}

		public bool HasDirt()
		{
			if (!base.master.storage.IsEmpty())
			{
				return base.master.storage.Has(ElementLoader.FindElementByHash(SimHashes.Dirt).tag);
			}
			return false;
		}

		public float MassPerFlush()
		{
			return base.master.solidWastePerUse.mass;
		}

		public bool IsToxicSandRemoved()
		{
			Tag tag = GameTagExtensions.Create(base.master.solidWastePerUse.elementID);
			return (UnityEngine.Object)base.master.storage.FindFirst(tag) == (UnityEngine.Object)null;
		}

		public void CreateCleanChore()
		{
			if (cleanChore != null)
			{
				cleanChore.Cancel("dupe");
			}
			ToiletWorkableClean component = base.master.GetComponent<ToiletWorkableClean>();
			cleanChore = new WorkChore<ToiletWorkableClean>(Db.Get().ChoreTypes.CleanToilet, component, null, true, OnCleanComplete, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, true, true);
		}

		public void CancelCleanChore()
		{
			if (cleanChore != null)
			{
				cleanChore.Cancel("Cancelled");
				cleanChore = null;
			}
		}

		private void OnCleanComplete(Chore chore)
		{
			cleanChore = null;
			Tag tag = GameTagExtensions.Create(base.master.solidWastePerUse.elementID);
			ListPool<GameObject, Toilet>.PooledList pooledList = ListPool<GameObject, Toilet>.Allocate();
			base.master.storage.Find(tag, pooledList);
			foreach (GameObject item in pooledList)
			{
				base.master.storage.Drop(item, true);
			}
			pooledList.Recycle();
			base.master.meter.SetPositionPercent((float)base.master.FlushesUsed / (float)base.master.maxFlushes);
		}

		public void Flush()
		{
			Worker worker = base.master.GetComponent<ToiletWorkableUse>().worker;
			base.master.Flush(worker);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Toilet>
	{
		public class ReadyStates : State
		{
			public State idle;

			public State inuse;

			public State flush;
		}

		public State needsdirt;

		public State empty;

		public State notoperational;

		public State ready;

		public State earlyclean;

		public State earlyWaitingForClean;

		public State full;

		public State fullWaitingForClean;

		private static readonly HashedString[] FULL_ANIMS = new HashedString[2]
		{
			"full_pre",
			"full"
		};

		public IntParameter flushes = new IntParameter(0);

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = needsdirt;
			root.PlayAnim("off").EventTransition(GameHashes.OnStorageChange, needsdirt, (StatesInstance smi) => !smi.HasDirt()).EventTransition(GameHashes.OperationalChanged, notoperational, (StatesInstance smi) => !smi.Get<Operational>().IsOperational);
			needsdirt.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Unusable).EventTransition(GameHashes.OnStorageChange, ready, (StatesInstance smi) => smi.HasDirt());
			ready.ParamTransition(flushes, full, (StatesInstance smi, int p) => smi.GetFlushesRemaining() <= 0).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Toilet).ToggleRecurringChore(CreateUrgentUseChore, null)
				.ToggleRecurringChore(CreateBreakUseChore, null)
				.ToggleTag(GameTags.Usable)
				.EventHandler(GameHashes.Flush, delegate(StatesInstance smi, object data)
				{
					smi.Flush();
				});
			earlyclean.PlayAnims((StatesInstance smi) => FULL_ANIMS, KAnim.PlayMode.Once).OnAnimQueueComplete(earlyWaitingForClean);
			earlyWaitingForClean.Enter(delegate(StatesInstance smi)
			{
				smi.CreateCleanChore();
			}).Exit(delegate(StatesInstance smi)
			{
				smi.CancelCleanChore();
			}).ToggleStatusItem(Db.Get().BuildingStatusItems.ToiletNeedsEmptying, (object)null)
				.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Unusable)
				.EventTransition(GameHashes.OnStorageChange, empty, (StatesInstance smi) => smi.IsToxicSandRemoved());
			full.PlayAnims((StatesInstance smi) => FULL_ANIMS, KAnim.PlayMode.Once).OnAnimQueueComplete(fullWaitingForClean);
			fullWaitingForClean.Enter(delegate(StatesInstance smi)
			{
				smi.CreateCleanChore();
			}).Exit(delegate(StatesInstance smi)
			{
				smi.CancelCleanChore();
			}).ToggleStatusItem(Db.Get().BuildingStatusItems.ToiletNeedsEmptying, (object)null)
				.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Unusable)
				.EventTransition(GameHashes.OnStorageChange, empty, (StatesInstance smi) => smi.IsToxicSandRemoved())
				.Enter(delegate(StatesInstance smi)
				{
					smi.Schedule(smi.monsterSpawnTime, delegate
					{
						smi.master.SpawnMonster();
					}, null);
				});
			empty.PlayAnim("off").Enter("ClearFlushes", delegate(StatesInstance smi)
			{
				smi.master.FlushesUsed = 0;
			}).Enter("ClearDirt", delegate(StatesInstance smi)
			{
				smi.master.storage.ConsumeAllIgnoringDisease();
			})
				.GoTo(needsdirt);
			notoperational.EventTransition(GameHashes.OperationalChanged, needsdirt, (StatesInstance smi) => smi.Get<Operational>().IsOperational).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Unusable);
		}

		private Chore CreateUrgentUseChore(StatesInstance smi)
		{
			Chore chore = CreateUseChore(smi, Db.Get().ChoreTypes.Pee);
			chore.AddPrecondition(ChorePreconditions.instance.IsBladderFull, null);
			chore.AddPrecondition(ChorePreconditions.instance.NotCurrentlyPeeing, null);
			return chore;
		}

		private Chore CreateBreakUseChore(StatesInstance smi)
		{
			Chore chore = CreateUseChore(smi, Db.Get().ChoreTypes.BreakPee);
			chore.AddPrecondition(ChorePreconditions.instance.IsBladderNotFull, null);
			chore.AddPrecondition(ChorePreconditions.instance.IsScheduledTime, Db.Get().ScheduleBlockTypes.Hygiene);
			return chore;
		}

		private Chore CreateUseChore(StatesInstance smi, ChoreType choreType)
		{
			WorkChore<ToiletWorkableUse> workChore = new WorkChore<ToiletWorkableUse>(choreType, smi.master, null, true, null, null, null, false, null, true, true, null, false, true, false, PriorityScreen.PriorityClass.personalNeeds, 5, false, false);
			smi.activeUseChores.Add(workChore);
			WorkChore<ToiletWorkableUse> workChore2 = workChore;
			workChore2.onExit = (Action<Chore>)Delegate.Combine(workChore2.onExit, (Action<Chore>)delegate(Chore exiting_chore)
			{
				smi.activeUseChores.Remove(exiting_chore);
			});
			workChore.AddPrecondition(ChorePreconditions.instance.IsPreferredAssignableOrUrgentBladder, smi.master.GetComponent<Assignable>());
			workChore.AddPrecondition(ChorePreconditions.instance.IsExclusivelyAvailableWithOtherChores, smi.activeUseChores);
			return workChore;
		}
	}

	[SerializeField]
	public SpawnInfo solidWastePerUse;

	[SerializeField]
	public SpawnInfo gasWasteWhenFull;

	[SerializeField]
	public int maxFlushes = 15;

	[SerializeField]
	public string diseaseId;

	[SerializeField]
	public int diseasePerFlush;

	[SerializeField]
	public int diseaseOnDupePerFlush;

	[Serialize]
	public int _flushesUsed = 0;

	private MeterController meter;

	[MyCmpReq]
	private Storage storage;

	private static readonly EventSystem.IntraObjectHandler<Toilet> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Toilet>(delegate(Toilet component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	public int FlushesUsed
	{
		get
		{
			return _flushesUsed;
		}
		set
		{
			_flushesUsed = value;
			base.smi.sm.flushes.Set(value, base.smi);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.Toilets.Add(this);
		base.smi.StartSM();
		ToiletWorkableUse component = GetComponent<ToiletWorkableUse>();
		component.trackUses = true;
		meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, "meter_target", "meter_arrow", "meter_scale");
		meter.SetPositionPercent((float)FlushesUsed / (float)maxFlushes);
		FlushesUsed = _flushesUsed;
		Subscribe(493375141, OnRefreshUserMenuDelegate);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Toilets.Remove(this);
	}

	public bool IsUsable()
	{
		return base.smi.HasTag(GameTags.Usable);
	}

	public void Flush(Worker worker)
	{
		float temperature = GetComponent<PrimaryElement>().Temperature;
		Element element = ElementLoader.FindElementByHash(solidWastePerUse.elementID);
		byte index = Db.Get().Diseases.GetIndex(diseaseId);
		GameObject go = element.substance.SpawnResource(base.transform.GetPosition(), base.smi.MassPerFlush(), temperature, index, diseasePerFlush, true, false, false);
		storage.Store(go, false, false, true, false);
		PrimaryElement component = worker.GetComponent<PrimaryElement>();
		component.AddDisease(index, diseaseOnDupePerFlush, "Toilet.Flush");
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, string.Format(DUPLICANTS.DISEASES.ADDED_POPFX, Db.Get().Diseases[index].Name, diseasePerFlush + diseaseOnDupePerFlush), base.transform, Vector3.up, 1.5f, false, false);
		FlushesUsed++;
		meter.SetPositionPercent((float)FlushesUsed / (float)maxFlushes);
		Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_LotsOfGerms);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (base.smi.GetCurrentState() != base.smi.sm.full && base.smi.IsSoiled && base.smi.cleanChore == null)
		{
			UserMenu userMenu = Game.Instance.userMenu;
			GameObject gameObject = base.gameObject;
			string iconName = "status_item_toilet_needs_emptying";
			string text = UI.USERMENUACTIONS.CLEANTOILET.NAME;
			System.Action on_click = delegate
			{
				base.smi.GoTo(base.smi.sm.earlyclean);
			};
			string tooltipText = UI.USERMENUACTIONS.CLEANTOILET.TOOLTIP;
			userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true), 1f);
		}
	}

	private void SpawnMonster()
	{
		GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(new Tag("Glom")), base.smi.transform.GetPosition(), Grid.SceneLayer.Creatures, null, 0);
		gameObject.SetActive(true);
	}

	public List<Descriptor> RequirementDescriptors()
	{
		List<Descriptor> list = new List<Descriptor>();
		ManualDeliveryKG component = GetComponent<ManualDeliveryKG>();
		string arg = component.requestedItemTag.ProperName();
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, arg, GameUtil.GetFormattedMass(base.smi.MassPerFlush(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, arg, GameUtil.GetFormattedMass(base.smi.MassPerFlush(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Requirement);
		list.Add(item);
		return list;
	}

	public List<Descriptor> EffectDescriptors()
	{
		List<Descriptor> list = new List<Descriptor>();
		Element element = ElementLoader.FindElementByHash(solidWastePerUse.elementID);
		string arg = element.tag.ProperName();
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTEDPERUSE, arg, GameUtil.GetFormattedMass(base.smi.MassPerFlush(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTEDPERUSE, arg, GameUtil.GetFormattedMass(base.smi.MassPerFlush(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Effect, false));
		Disease disease = Db.Get().Diseases.Get(diseaseId);
		int units = diseasePerFlush + diseaseOnDupePerFlush;
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.DISEASEEMITTEDPERUSE, disease.Name, GameUtil.GetFormattedDiseaseAmount(units)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.DISEASEEMITTEDPERUSE, disease.Name, GameUtil.GetFormattedDiseaseAmount(units)), Descriptor.DescriptorType.DiseaseSource, false));
		return list;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.AddRange(RequirementDescriptors());
		list.AddRange(EffectDescriptors());
		return list;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor item in RequirementDescriptors())
		{
			list.Add(item);
		}
		foreach (Descriptor item2 in EffectDescriptors())
		{
			list.Add(item2);
		}
		return list;
	}

	Transform get_transform()
	{
		return base.transform;
	}

	Transform IUsable.get_transform()
	{
		//ILSpy generated this explicit interface implementation from .override directive in get_transform
		return this.get_transform();
	}
}
