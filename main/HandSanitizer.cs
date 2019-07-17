using Klei;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HandSanitizer : StateMachineComponent<HandSanitizer.SMInstance>, IEffectDescriptor
{
	private class WashHandsReactable : WorkableReactable
	{
		public WashHandsReactable(Workable workable, ChoreType chore_type, AllowedDirection allowed_direction = AllowedDirection.Any)
			: base(workable, "WashHands", chore_type, allowed_direction)
		{
		}

		public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
		{
			if (base.InternalCanBegin(new_reactor, transition))
			{
				PrimaryElement component = new_reactor.GetComponent<PrimaryElement>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					return component.DiseaseIdx != 255;
				}
			}
			return false;
		}
	}

	public class SMInstance : GameStateMachine<States, SMInstance, HandSanitizer, object>.GameInstance
	{
		public SMInstance(HandSanitizer master)
			: base(master)
		{
		}

		private bool HasSufficientMass()
		{
			bool result = false;
			PrimaryElement primaryElement = GetComponent<Storage>().FindPrimaryElement(base.master.consumedElement);
			if ((UnityEngine.Object)primaryElement != (UnityEngine.Object)null)
			{
				result = (primaryElement.Mass > 0f);
			}
			return result;
		}

		public bool OutputFull()
		{
			PrimaryElement primaryElement = GetComponent<Storage>().FindPrimaryElement(base.master.outputElement);
			if (!((UnityEngine.Object)primaryElement != (UnityEngine.Object)null))
			{
				return false;
			}
			return primaryElement.Mass >= (float)base.master.maxUses * base.master.massConsumedPerUse;
		}

		public bool IsReady()
		{
			if (HasSufficientMass())
			{
				if (!OutputFull())
				{
					return true;
				}
				return false;
			}
			return false;
		}

		public void OnCompleteWork(Worker worker)
		{
		}

		public void DumpOutput()
		{
			Storage component = base.master.GetComponent<Storage>();
			if (base.master.outputElement != SimHashes.Vacuum)
			{
				component.Drop(ElementLoader.FindElementByHash(base.master.outputElement).tag);
			}
		}
	}

	public class States : GameStateMachine<States, SMInstance, HandSanitizer>
	{
		public class ReadyStates : State
		{
			public State free;

			public State occupied;
		}

		public State notready;

		public ReadyStates ready;

		public State notoperational;

		public State full;

		public State empty;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = notready;
			root.Update(UpdateStatusItems, UpdateRate.SIM_200ms, false);
			notoperational.PlayAnim("off").TagTransition(GameTags.Operational, notready, false);
			notready.PlayAnim("off").EventTransition(GameHashes.OnStorageChange, ready, (SMInstance smi) => smi.IsReady()).TagTransition(GameTags.Operational, notoperational, true);
			ready.DefaultState(ready.free).ToggleReactable((SMInstance smi) => smi.master.reactable = new WashHandsReactable(smi.master.GetComponent<Work>(), Db.Get().ChoreTypes.WashHands, smi.master.GetComponent<DirectionControl>().allowedDirection)).TagTransition(GameTags.Operational, notoperational, true);
			ready.free.PlayAnim("on").WorkableStartTransition((SMInstance smi) => smi.GetComponent<Work>(), ready.occupied);
			ready.occupied.PlayAnim("working_pre").QueueAnim("working_loop", true, null).WorkableStopTransition((SMInstance smi) => smi.GetComponent<Work>(), notready);
		}

		private void UpdateStatusItems(SMInstance smi, float dt)
		{
			if (smi.OutputFull())
			{
				smi.master.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.OutputPipeFull, this);
			}
			else
			{
				smi.master.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.OutputPipeFull, false);
			}
		}
	}

	public class Work : Workable, IGameObjectEffectDescriptor
	{
		private int diseaseRemoved = 0;

		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			resetProgressOnStop = true;
			shouldTransferDiseaseWithWorker = false;
			GameScheduler.Instance.Schedule("WaterFetchingTutorial", 2f, delegate
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_FetchingWater, true);
			}, null, null);
		}

		protected override void OnStartWork(Worker worker)
		{
			base.OnStartWork(worker);
			diseaseRemoved = 0;
		}

		protected override bool OnWorkTick(Worker worker, float dt)
		{
			base.OnWorkTick(worker, dt);
			HandSanitizer component = GetComponent<HandSanitizer>();
			Storage component2 = GetComponent<Storage>();
			float massAvailable = component2.GetMassAvailable(component.consumedElement);
			if (massAvailable != 0f)
			{
				PrimaryElement component3 = worker.GetComponent<PrimaryElement>();
				float a = component.massConsumedPerUse * dt / workTime;
				float num = Mathf.Min(a, massAvailable);
				int num2 = Math.Min((int)(dt / workTime * (float)component.diseaseRemovalCount), component3.DiseaseCount);
				diseaseRemoved += num2;
				SimUtil.DiseaseInfo invalid = SimUtil.DiseaseInfo.Invalid;
				invalid.idx = component3.DiseaseIdx;
				invalid.count = num2;
				component3.ModifyDiseaseCount(-num2, "HandSanitizer.OnWorkTick");
				component.maxPossiblyRemoved += num2;
				SimUtil.DiseaseInfo disease_info = SimUtil.DiseaseInfo.Invalid;
				component2.ConsumeAndGetDisease(ElementLoader.FindElementByHash(component.consumedElement).tag, num, out disease_info, out float aggregate_temperature);
				if (component.outputElement != SimHashes.Vacuum)
				{
					disease_info = SimUtil.CalculateFinalDiseaseInfo(invalid, disease_info);
					component2.AddLiquid(component.outputElement, num, aggregate_temperature, disease_info.idx, disease_info.count, false, true);
				}
				return diseaseRemoved > component.diseaseRemovalCount;
			}
			return true;
		}

		protected override void OnCompleteWork(Worker worker)
		{
			base.OnCompleteWork(worker);
		}
	}

	public float massConsumedPerUse = 1f;

	public SimHashes consumedElement = SimHashes.BleachStone;

	public int diseaseRemovalCount = 10000;

	public int maxUses = 10;

	public SimHashes outputElement = SimHashes.Vacuum;

	public bool dumpWhenFull = false;

	private WorkableReactable reactable;

	private MeterController cleanMeter;

	private MeterController dirtyMeter;

	public Meter.Offset cleanMeterOffset = Meter.Offset.Infront;

	public Meter.Offset dirtyMeterOffset = Meter.Offset.Infront;

	[Serialize]
	public int maxPossiblyRemoved = 0;

	private static readonly EventSystem.IntraObjectHandler<HandSanitizer> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<HandSanitizer>(delegate(HandSanitizer component, object data)
	{
		component.OnStorageChange(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.gameObject.FindOrAddComponent<Workable>();
	}

	private void RefreshMeters()
	{
		float positionPercent = 0f;
		PrimaryElement primaryElement = GetComponent<Storage>().FindPrimaryElement(consumedElement);
		float num = (float)maxUses * massConsumedPerUse;
		ConduitConsumer component = GetComponent<ConduitConsumer>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			num = component.capacityKG;
		}
		if ((UnityEngine.Object)primaryElement != (UnityEngine.Object)null)
		{
			positionPercent = Mathf.Clamp01(primaryElement.Mass / num);
		}
		float positionPercent2 = 0f;
		PrimaryElement primaryElement2 = GetComponent<Storage>().FindPrimaryElement(outputElement);
		if ((UnityEngine.Object)primaryElement2 != (UnityEngine.Object)null)
		{
			positionPercent2 = Mathf.Clamp01(primaryElement2.Mass / ((float)maxUses * massConsumedPerUse));
		}
		cleanMeter.SetPositionPercent(positionPercent);
		dirtyMeter.SetPositionPercent(positionPercent2);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		cleanMeter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_clean_target", "meter_clean", cleanMeterOffset, Grid.SceneLayer.NoLayer, "meter_clean_target");
		dirtyMeter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_dirty_target", "meter_dirty", dirtyMeterOffset, Grid.SceneLayer.NoLayer, "meter_dirty_target");
		RefreshMeters();
		Components.HandSanitizers.Add(this);
		Subscribe(-1697596308, OnStorageChangeDelegate);
		DirectionControl component = GetComponent<DirectionControl>();
		component.onDirectionChanged = (Action<WorkableReactable.AllowedDirection>)Delegate.Combine(component.onDirectionChanged, new Action<WorkableReactable.AllowedDirection>(OnDirectionChanged));
		OnDirectionChanged(GetComponent<DirectionControl>().allowedDirection);
	}

	protected override void OnCleanUp()
	{
		Components.HandSanitizers.Remove(this);
		base.OnCleanUp();
	}

	private void OnDirectionChanged(WorkableReactable.AllowedDirection allowed_direction)
	{
		if (reactable != null)
		{
			reactable.allowedDirection = allowed_direction;
		}
	}

	public List<Descriptor> RequirementDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, ElementLoader.FindElementByHash(consumedElement).name, GameUtil.GetFormattedMass(massConsumedPerUse, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, ElementLoader.FindElementByHash(consumedElement).name, GameUtil.GetFormattedMass(massConsumedPerUse, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Requirement, false));
		return list;
	}

	public List<Descriptor> EffectDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (outputElement != SimHashes.Vacuum)
		{
			list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTEDPERUSE, ElementLoader.FindElementByHash(outputElement).name, GameUtil.GetFormattedMass(massConsumedPerUse, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTEDPERUSE, ElementLoader.FindElementByHash(outputElement).name, GameUtil.GetFormattedMass(massConsumedPerUse, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Effect, false));
		}
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.DISEASECONSUMEDPERUSE, GameUtil.GetFormattedDiseaseAmount(diseaseRemovalCount)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.DISEASECONSUMEDPERUSE, GameUtil.GetFormattedDiseaseAmount(diseaseRemovalCount)), Descriptor.DescriptorType.Effect, false));
		return list;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.AddRange(RequirementDescriptors(def));
		list.AddRange(EffectDescriptors(def));
		return list;
	}

	private void OnStorageChange(object data)
	{
		if (dumpWhenFull && base.smi.OutputFull())
		{
			base.smi.DumpOutput();
		}
		RefreshMeters();
	}
}
