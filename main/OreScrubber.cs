using Klei;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class OreScrubber : StateMachineComponent<OreScrubber.SMInstance>, IEffectDescriptor
{
	private class ScrubOreReactable : WorkableReactable
	{
		public ScrubOreReactable(Workable workable, ChoreType chore_type, AllowedDirection allowed_direction = AllowedDirection.Any)
			: base(workable, "ScrubOre", chore_type, allowed_direction)
		{
		}

		public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
		{
			if (base.InternalCanBegin(new_reactor, transition))
			{
				Storage component = new_reactor.GetComponent<Storage>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null && (UnityEngine.Object)GetFirstInfected(component) != (UnityEngine.Object)null)
				{
					return true;
				}
			}
			return false;
		}
	}

	public class SMInstance : GameStateMachine<States, SMInstance, OreScrubber, object>.GameInstance
	{
		public SMInstance(OreScrubber master)
			: base(master)
		{
		}

		public bool HasSufficientMass()
		{
			bool result = false;
			PrimaryElement primaryElement = GetComponent<Storage>().FindPrimaryElement(base.master.consumedElement);
			if ((UnityEngine.Object)primaryElement != (UnityEngine.Object)null)
			{
				result = (primaryElement.Mass > 0f);
			}
			return result;
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

	public class States : GameStateMachine<States, SMInstance, OreScrubber>
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
			base.serializable = true;
			notoperational.PlayAnim("off").TagTransition(GameTags.Operational, notready, false);
			notready.PlayAnim("off").EventTransition(GameHashes.OnStorageChange, ready, (SMInstance smi) => smi.HasSufficientMass()).TagTransition(GameTags.Operational, notoperational, true);
			ready.DefaultState(ready.free).ToggleReactable((SMInstance smi) => smi.master.reactable = new ScrubOreReactable(smi.master.GetComponent<Work>(), Db.Get().ChoreTypes.ScrubOre, smi.master.GetComponent<DirectionControl>().allowedDirection)).EventTransition(GameHashes.OnStorageChange, notready, (SMInstance smi) => !smi.HasSufficientMass())
				.TagTransition(GameTags.Operational, notoperational, true);
			ready.free.PlayAnim("on").WorkableStartTransition((SMInstance smi) => smi.GetComponent<Work>(), ready.occupied);
			ready.occupied.PlayAnim("working_pre").QueueAnim("working_loop", true, null).WorkableStopTransition((SMInstance smi) => smi.GetComponent<Work>(), ready);
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
		}

		protected override void OnStartWork(Worker worker)
		{
			base.OnStartWork(worker);
			diseaseRemoved = 0;
		}

		protected override bool OnWorkTick(Worker worker, float dt)
		{
			base.OnWorkTick(worker, dt);
			OreScrubber component = GetComponent<OreScrubber>();
			Storage component2 = GetComponent<Storage>();
			PrimaryElement firstInfected = GetFirstInfected(worker.GetComponent<Storage>());
			int num = 0;
			SimUtil.DiseaseInfo invalid = SimUtil.DiseaseInfo.Invalid;
			if ((UnityEngine.Object)firstInfected != (UnityEngine.Object)null)
			{
				num = Math.Min((int)(dt / workTime * (float)component.diseaseRemovalCount), firstInfected.DiseaseCount);
				diseaseRemoved += num;
				invalid.idx = firstInfected.DiseaseIdx;
				invalid.count = num;
				firstInfected.ModifyDiseaseCount(-num, "OreScrubber.OnWorkTick");
			}
			component.maxPossiblyRemoved += num;
			float num2 = component.massConsumedPerUse * dt / workTime;
			SimUtil.DiseaseInfo disease_info = SimUtil.DiseaseInfo.Invalid;
			component2.ConsumeAndGetDisease(ElementLoader.FindElementByHash(component.consumedElement).tag, num2, out disease_info, out float aggregate_temperature);
			if (component.outputElement != SimHashes.Vacuum)
			{
				disease_info = SimUtil.CalculateFinalDiseaseInfo(invalid, disease_info);
				component2.AddLiquid(component.outputElement, num2, aggregate_temperature, disease_info.idx, disease_info.count, false, true);
			}
			return diseaseRemoved > component.diseaseRemovalCount;
		}

		protected override void OnCompleteWork(Worker worker)
		{
			base.OnCompleteWork(worker);
		}
	}

	public float massConsumedPerUse = 1f;

	public SimHashes consumedElement = SimHashes.BleachStone;

	public int diseaseRemovalCount = 10000;

	public SimHashes outputElement = SimHashes.Vacuum;

	private WorkableReactable reactable;

	private MeterController cleanMeter;

	[Serialize]
	public int maxPossiblyRemoved = 0;

	private static readonly EventSystem.IntraObjectHandler<OreScrubber> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<OreScrubber>(delegate(OreScrubber component, object data)
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
		if ((UnityEngine.Object)primaryElement != (UnityEngine.Object)null)
		{
			positionPercent = Mathf.Clamp01(primaryElement.Mass / GetComponent<ConduitConsumer>().capacityKG);
		}
		cleanMeter.SetPositionPercent(positionPercent);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		cleanMeter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_clean_target", "meter_clean", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_clean_target");
		RefreshMeters();
		Subscribe(-1697596308, OnStorageChangeDelegate);
		DirectionControl component = GetComponent<DirectionControl>();
		component.onDirectionChanged = (Action<WorkableReactable.AllowedDirection>)Delegate.Combine(component.onDirectionChanged, new Action<WorkableReactable.AllowedDirection>(OnDirectionChanged));
		OnDirectionChanged(GetComponent<DirectionControl>().allowedDirection);
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
		string name = ElementLoader.FindElementByHash(consumedElement).name;
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, name, GameUtil.GetFormattedMass(massConsumedPerUse, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, name, GameUtil.GetFormattedMass(massConsumedPerUse, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Requirement, false));
		return list;
	}

	public List<Descriptor> EffectDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (outputElement != SimHashes.Vacuum)
		{
			list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTEDPERUSE, ElementLoader.FindElementByHash(outputElement).name, GameUtil.GetFormattedMass(massConsumedPerUse, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTEDPERUSE, ElementLoader.FindElementByHash(outputElement).name, GameUtil.GetFormattedMass(massConsumedPerUse, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Requirement, false));
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
		RefreshMeters();
	}

	private static PrimaryElement GetFirstInfected(Storage storage)
	{
		foreach (GameObject item in storage.items)
		{
			if (!((UnityEngine.Object)item == (UnityEngine.Object)null))
			{
				PrimaryElement component = item.GetComponent<PrimaryElement>();
				if (component.DiseaseIdx != 255 && !item.HasTag(GameTags.Edible))
				{
					return component;
				}
			}
		}
		return null;
	}
}
