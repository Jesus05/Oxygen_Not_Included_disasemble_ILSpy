using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FlushToilet : StateMachineComponent<FlushToilet.SMInstance>, IUsable, IEffectDescriptor
{
	public class SMInstance : GameStateMachine<States, SMInstance, FlushToilet, object>.GameInstance
	{
		public List<Chore> activeUseChores;

		public SMInstance(FlushToilet master)
			: base(master)
		{
			activeUseChores = new List<Chore>();
			UpdateFullnessState();
			UpdateDirtyState();
		}

		public bool HasValidConnections()
		{
			return Game.Instance.liquidConduitFlow.HasConduit(base.master.inputCell) && Game.Instance.liquidConduitFlow.HasConduit(base.master.outputCell);
		}

		public bool UpdateFullnessState()
		{
			float num = 0f;
			ListPool<GameObject, FlushToilet>.PooledList pooledList = ListPool<GameObject, FlushToilet>.Allocate();
			base.master.storage.Find(WaterTag, pooledList);
			foreach (GameObject item in pooledList)
			{
				PrimaryElement component = item.GetComponent<PrimaryElement>();
				num += component.Mass;
			}
			pooledList.Recycle();
			bool flag = num >= base.master.massConsumedPerUse;
			base.master.conduitConsumer.enabled = !flag;
			float positionPercent = Mathf.Clamp01(num / base.master.massConsumedPerUse);
			base.master.fillMeter.SetPositionPercent(positionPercent);
			return flag;
		}

		public void UpdateDirtyState()
		{
			ToiletWorkableUse component = GetComponent<ToiletWorkableUse>();
			float percentComplete = component.GetPercentComplete();
			base.master.contaminationMeter.SetPositionPercent(percentComplete);
		}

		public void Flush()
		{
			base.master.fillMeter.SetPositionPercent(0f);
			base.master.contaminationMeter.SetPositionPercent(1f);
			base.smi.ShowFillMeter();
			Worker worker = base.master.GetComponent<ToiletWorkableUse>().worker;
			base.master.Flush(worker);
		}

		public void ShowFillMeter()
		{
			base.master.fillMeter.gameObject.SetActive(true);
			base.master.contaminationMeter.gameObject.SetActive(false);
		}

		public bool HasContaminatedMass()
		{
			foreach (GameObject item in GetComponent<Storage>().items)
			{
				PrimaryElement component = item.GetComponent<PrimaryElement>();
				if (!((UnityEngine.Object)component == (UnityEngine.Object)null) && component.ElementID == SimHashes.DirtyWater && component.Mass > 0f)
				{
					return true;
				}
			}
			return false;
		}

		public void ShowContaminatedMeter()
		{
			base.master.fillMeter.gameObject.SetActive(false);
			base.master.contaminationMeter.gameObject.SetActive(true);
		}
	}

	public class States : GameStateMachine<States, SMInstance, FlushToilet>
	{
		public class ReadyStates : State
		{
			public State idle;

			public State inuse;
		}

		public State disconnected;

		public State backedup;

		public ReadyStates ready;

		public State fillingInactive;

		public State filling;

		public State flushing;

		public State flushed;

		public BoolParameter outputBlocked;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = disconnected;
			disconnected.PlayAnim("off").EventTransition(GameHashes.ConduitConnectionChanged, backedup, (SMInstance smi) => smi.HasValidConnections()).Enter(delegate(SMInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(false, false);
			});
			backedup.PlayAnim("off").ToggleStatusItem(Db.Get().BuildingStatusItems.OutputPipeFull, (object)null).EventTransition(GameHashes.ConduitConnectionChanged, disconnected, (SMInstance smi) => !smi.HasValidConnections())
				.ParamTransition(outputBlocked, fillingInactive, GameStateMachine<States, SMInstance, FlushToilet, object>.IsFalse)
				.Enter(delegate(SMInstance smi)
				{
					smi.GetComponent<Operational>().SetActive(false, false);
				});
			filling.PlayAnim("off").Enter(delegate(SMInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(true, false);
			}).EventTransition(GameHashes.ConduitConnectionChanged, disconnected, (SMInstance smi) => !smi.HasValidConnections())
				.ParamTransition(outputBlocked, backedup, GameStateMachine<States, SMInstance, FlushToilet, object>.IsTrue)
				.EventTransition(GameHashes.OnStorageChange, ready, (SMInstance smi) => smi.UpdateFullnessState())
				.EventTransition(GameHashes.OperationalChanged, fillingInactive, (SMInstance smi) => !smi.GetComponent<Operational>().IsOperational);
			fillingInactive.PlayAnim("off").Enter(delegate(SMInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(false, false);
			}).EventTransition(GameHashes.OperationalChanged, filling, (SMInstance smi) => smi.GetComponent<Operational>().IsOperational)
				.ParamTransition(outputBlocked, backedup, GameStateMachine<States, SMInstance, FlushToilet, object>.IsTrue);
			ready.DefaultState(ready.idle).Enter(delegate(SMInstance smi)
			{
				smi.master.fillMeter.SetPositionPercent(1f);
				smi.master.contaminationMeter.SetPositionPercent(0f);
			}).PlayAnim("off")
				.EventTransition(GameHashes.ConduitConnectionChanged, disconnected, (SMInstance smi) => !smi.HasValidConnections())
				.ParamTransition(outputBlocked, backedup, GameStateMachine<States, SMInstance, FlushToilet, object>.IsTrue)
				.ToggleChore(CreateUrgentUseChore, flushing)
				.ToggleChore(CreateBreakUseChore, flushing);
			ready.idle.Enter(delegate(SMInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(false, false);
			}).ToggleMainStatusItem(Db.Get().BuildingStatusItems.FlushToilet).WorkableStartTransition((SMInstance smi) => smi.master.GetComponent<ToiletWorkableUse>(), ready.inuse);
			ready.inuse.Enter(delegate(SMInstance smi)
			{
				smi.ShowContaminatedMeter();
			}).ToggleMainStatusItem(Db.Get().BuildingStatusItems.FlushToiletInUse).Update(delegate(SMInstance smi, float dt)
			{
				smi.UpdateDirtyState();
			}, UpdateRate.SIM_200ms, false)
				.WorkableCompleteTransition((SMInstance smi) => smi.master.GetComponent<ToiletWorkableUse>(), flushing)
				.WorkableStopTransition((SMInstance smi) => smi.master.GetComponent<ToiletWorkableUse>(), flushed);
			flushing.Enter(delegate(SMInstance smi)
			{
				smi.Flush();
			}).GoTo(flushed);
			flushed.EventTransition(GameHashes.OnStorageChange, fillingInactive, (SMInstance smi) => !smi.HasContaminatedMass()).ParamTransition(outputBlocked, backedup, GameStateMachine<States, SMInstance, FlushToilet, object>.IsTrue);
		}

		private Chore CreateUrgentUseChore(SMInstance smi)
		{
			Chore chore = CreateUseChore(smi, Db.Get().ChoreTypes.Pee);
			chore.AddPrecondition(ChorePreconditions.instance.IsBladderFull, null);
			chore.AddPrecondition(ChorePreconditions.instance.NotCurrentlyPeeing, null);
			return chore;
		}

		private Chore CreateBreakUseChore(SMInstance smi)
		{
			Chore chore = CreateUseChore(smi, Db.Get().ChoreTypes.BreakPee);
			chore.AddPrecondition(ChorePreconditions.instance.IsBladderNotFull, null);
			return chore;
		}

		private Chore CreateUseChore(SMInstance smi, ChoreType choreType)
		{
			WorkChore<ToiletWorkableUse> workChore = new WorkChore<ToiletWorkableUse>(choreType, smi.master, null, null, true, null, null, null, false, null, true, true, null, false, true, false, PriorityScreen.PriorityClass.personalNeeds, 5, false, false);
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

	private MeterController fillMeter;

	private MeterController contaminationMeter;

	[SerializeField]
	public float massConsumedPerUse = 5f;

	[SerializeField]
	public float massEmittedPerUse = 5f;

	[SerializeField]
	public string diseaseId;

	[SerializeField]
	public int diseasePerFlush;

	[SerializeField]
	public int diseaseOnDupePerFlush;

	[MyCmpGet]
	private ConduitConsumer conduitConsumer;

	[MyCmpGet]
	private Storage storage;

	public static readonly Tag WaterTag = GameTagExtensions.Create(SimHashes.Water);

	private int inputCell;

	private int outputCell;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Building component = GetComponent<Building>();
		inputCell = component.GetUtilityInputCell();
		outputCell = component.GetUtilityOutputCell();
		ConduitFlow liquidConduitFlow = Game.Instance.liquidConduitFlow;
		liquidConduitFlow.onConduitsRebuilt += OnConduitsRebuilt;
		liquidConduitFlow.AddConduitUpdater(OnConduitUpdate, ConduitFlowPriority.Default);
		KBatchedAnimController component2 = GetComponent<KBatchedAnimController>();
		fillMeter = new MeterController(component2, "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, new Vector3(0.4f, 3.2f, 0.1f));
		contaminationMeter = new MeterController(component2, "meter_target", "meter_dirty", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, new Vector3(0.4f, 3.2f, 0.1f));
		Components.Toilets.Add(this);
		base.smi.StartSM();
		base.smi.ShowFillMeter();
	}

	protected override void OnCleanUp()
	{
		ConduitFlow liquidConduitFlow = Game.Instance.liquidConduitFlow;
		liquidConduitFlow.onConduitsRebuilt -= OnConduitsRebuilt;
		Components.Toilets.Remove(this);
		base.OnCleanUp();
	}

	private void OnConduitsRebuilt()
	{
		Trigger(-2094018600, null);
	}

	public bool IsUsable()
	{
		return base.smi.HasTag(GameTags.Usable);
	}

	private void Flush(Worker worker)
	{
		ListPool<GameObject, Storage>.PooledList pooledList = ListPool<GameObject, Storage>.Allocate();
		storage.Find(WaterTag, pooledList);
		float num = 0f;
		float num2 = massConsumedPerUse;
		foreach (GameObject item in pooledList)
		{
			PrimaryElement component = item.GetComponent<PrimaryElement>();
			float num3 = Mathf.Min(component.Mass, num2);
			component.Mass -= num3;
			num2 -= num3;
			num += num3 * component.Temperature;
		}
		pooledList.Recycle();
		float temperature = num / massConsumedPerUse;
		byte index = Db.Get().Diseases.GetIndex(diseaseId);
		storage.AddLiquid(SimHashes.DirtyWater, massEmittedPerUse, temperature, index, diseasePerFlush, false, true);
		if ((UnityEngine.Object)worker != (UnityEngine.Object)null)
		{
			PrimaryElement component2 = worker.GetComponent<PrimaryElement>();
			component2.AddDisease(index, diseaseOnDupePerFlush, "FlushToilet.Flush");
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, string.Format(DUPLICANTS.DISEASES.ADDED_POPFX, Db.Get().Diseases[index].Name, diseasePerFlush + diseaseOnDupePerFlush), base.transform, Vector3.up, 1.5f, false, false);
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_LotsOfGerms);
		}
		else
		{
			DebugUtil.LogWarningArgs("Tried to add disease on toilet use but worker was null");
		}
	}

	public List<Descriptor> RequirementDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Element element = ElementLoader.FindElementByHash(SimHashes.Water);
		string arg = element.tag.ProperName();
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, arg, GameUtil.GetFormattedMass(massConsumedPerUse, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, arg, GameUtil.GetFormattedMass(massConsumedPerUse, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Requirement, false));
		return list;
	}

	public List<Descriptor> EffectDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Element element = ElementLoader.FindElementByHash(SimHashes.DirtyWater);
		string arg = element.tag.ProperName();
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTEDPERUSE, arg, GameUtil.GetFormattedMass(massEmittedPerUse, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTEDPERUSE, arg, GameUtil.GetFormattedMass(massEmittedPerUse, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Effect, false));
		Disease disease = Db.Get().Diseases.Get(diseaseId);
		int units = diseasePerFlush + diseaseOnDupePerFlush;
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.DISEASEEMITTEDPERUSE, disease.Name, GameUtil.GetFormattedDiseaseAmount(units)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.DISEASEEMITTEDPERUSE, disease.Name, GameUtil.GetFormattedDiseaseAmount(units)), Descriptor.DescriptorType.DiseaseSource, false));
		return list;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.AddRange(RequirementDescriptors(def));
		list.AddRange(EffectDescriptors(def));
		return list;
	}

	private void OnConduitUpdate(float dt)
	{
		if (GetSMI() != null)
		{
			ConduitFlow liquidConduitFlow = Game.Instance.liquidConduitFlow;
			ConduitFlow.ConduitContents contents = liquidConduitFlow.GetContents(outputCell);
			bool value = contents.mass > 0f && base.smi.HasContaminatedMass();
			base.smi.sm.outputBlocked.Set(value, base.smi);
		}
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
