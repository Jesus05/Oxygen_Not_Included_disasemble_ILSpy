using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class EnergyGenerator : Generator, IEffectDescriptor, ISingleSliderControl, ISliderControl
{
	[Serializable]
	[DebuggerDisplay("{tag} -{consumptionRate} kg/s")]
	public struct InputItem
	{
		public Tag tag;

		public float consumptionRate;

		public float maxStoredMass;

		public InputItem(Tag tag, float consumption_rate, float max_stored_mass)
		{
			this.tag = tag;
			consumptionRate = consumption_rate;
			maxStoredMass = max_stored_mass;
		}
	}

	[Serializable]
	[DebuggerDisplay("{element} {creationRate} kg/s")]
	public struct OutputItem
	{
		public SimHashes element;

		public float creationRate;

		public bool store;

		public CellOffset emitOffset;

		public float minTemperature;

		public OutputItem(SimHashes element, float creation_rate, bool store, float min_temperature = 0f)
		{
			this = new OutputItem(element, creation_rate, store, CellOffset.none, min_temperature);
		}

		public OutputItem(SimHashes element, float creation_rate, bool store, CellOffset emit_offset, float min_temperature = 0f)
		{
			this.element = element;
			creationRate = creation_rate;
			this.store = store;
			emitOffset = emit_offset;
			minTemperature = min_temperature;
		}
	}

	[Serializable]
	public struct Formula
	{
		public InputItem[] inputs;

		public OutputItem[] outputs;

		public Tag meterTag;
	}

	[MyCmpAdd]
	private Storage storage;

	[MyCmpGet]
	private ManualDeliveryKG delivery;

	[SerializeField]
	[Serialize]
	private float batteryRefillPercent = 0.5f;

	public bool ignoreBatteryRefillPercent = false;

	public bool hasMeter = true;

	private static StatusItem batteriesSufficientlyFull;

	public Meter.Offset meterOffset = Meter.Offset.Infront;

	[SerializeField]
	public Formula formula;

	private MeterController meter;

	private static readonly EventSystem.IntraObjectHandler<EnergyGenerator> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<EnergyGenerator>(delegate(EnergyGenerator component, object data)
	{
		component.OnActiveChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<EnergyGenerator> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<EnergyGenerator>(delegate(EnergyGenerator component, object data)
	{
		component.OnCopySettings(data);
	});

	public string SliderTitleKey => "STRINGS.UI.UISIDESCREENS.MANUALGENERATORSIDESCREEN.TITLE";

	public string SliderUnits => UI.UNITSUFFIXES.PERCENT;

	public static StatusItem BatteriesSufficientlyFull => batteriesSufficientlyFull;

	public int SliderDecimalPlaces(int index)
	{
		return 0;
	}

	public float GetSliderMin(int index)
	{
		return 0f;
	}

	public float GetSliderMax(int index)
	{
		return 100f;
	}

	public float GetSliderValue(int index)
	{
		return batteryRefillPercent * 100f;
	}

	public void SetSliderValue(float value, int index)
	{
		batteryRefillPercent = value / 100f;
	}

	public string GetSliderTooltipKey(int index)
	{
		return "STRINGS.UI.UISIDESCREENS.MANUALGENERATORSIDESCREEN.TOOLTIP";
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		EnsureStatusItemAvailable();
		Subscribe(824508782, OnActiveChangedDelegate);
		if (!ignoreBatteryRefillPercent)
		{
			base.gameObject.AddOrGet<CopyBuildingSettings>();
			Subscribe(-905833192, OnCopySettingsDelegate);
		}
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		EnergyGenerator component = gameObject.GetComponent<EnergyGenerator>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			batteryRefillPercent = component.batteryRefillPercent;
		}
	}

	protected void OnActiveChanged(object data)
	{
		StatusItem status_item = (!((Operational)data).IsActive) ? Db.Get().BuildingStatusItems.GeneratorOffline : Db.Get().BuildingStatusItems.Wattage;
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item, this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (hasMeter)
		{
			meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", meterOffset, Grid.SceneLayer.NoLayer, "meter_target", "meter_fill", "meter_frame", "meter_OL");
		}
	}

	private bool IsConvertible(float dt)
	{
		bool flag = true;
		InputItem[] inputs = formula.inputs;
		for (int i = 0; i < inputs.Length; i++)
		{
			InputItem inputItem = inputs[i];
			GameObject gameObject = storage.FindFirst(inputItem.tag);
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				float num = inputItem.consumptionRate * dt;
				flag = (flag && component.Mass >= num);
			}
			else
			{
				flag = false;
			}
			if (!flag)
			{
				break;
			}
		}
		return flag;
	}

	public override void EnergySim200ms(float dt)
	{
		base.EnergySim200ms(dt);
		if (hasMeter)
		{
			InputItem inputItem = formula.inputs[0];
			float positionPercent = 0f;
			GameObject gameObject = storage.FindFirst(inputItem.tag);
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				positionPercent = component.Mass / inputItem.maxStoredMass;
			}
			meter.SetPositionPercent(positionPercent);
		}
		ushort circuitID = base.CircuitID;
		operational.SetFlag(Generator.wireConnectedFlag, circuitID != 65535);
		bool value = false;
		if (operational.IsOperational)
		{
			bool flag = false;
			List<Battery> batteriesOnCircuit = Game.Instance.circuitManager.GetBatteriesOnCircuit(circuitID);
			if (!ignoreBatteryRefillPercent && batteriesOnCircuit.Count > 0)
			{
				foreach (Battery item in batteriesOnCircuit)
				{
					if (batteryRefillPercent <= 0f && item.PercentFull <= 0f)
					{
						flag = true;
						break;
					}
					if (item.PercentFull < batteryRefillPercent)
					{
						flag = true;
						break;
					}
				}
			}
			else
			{
				flag = true;
			}
			if (!ignoreBatteryRefillPercent)
			{
				selectable.ToggleStatusItem(batteriesSufficientlyFull, !flag, null);
			}
			if ((UnityEngine.Object)delivery != (UnityEngine.Object)null)
			{
				delivery.Pause(!flag, "Circuit has sufficient energy");
			}
			if (formula.inputs != null)
			{
				bool flag2 = IsConvertible(dt);
				selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NeedResourceMass, !flag2, formula);
				if (flag2)
				{
					InputItem[] inputs = formula.inputs;
					for (int i = 0; i < inputs.Length; i++)
					{
						InputItem inputItem2 = inputs[i];
						float amount = inputItem2.consumptionRate * dt;
						storage.ConsumeIgnoringDisease(inputItem2.tag, amount);
					}
					PrimaryElement component2 = GetComponent<PrimaryElement>();
					OutputItem[] outputs = formula.outputs;
					foreach (OutputItem output in outputs)
					{
						Emit(output, dt, component2);
					}
					GenerateJoules(base.WattageRating * dt, false);
					selectable.SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.Wattage, this);
					value = true;
				}
			}
		}
		operational.SetActive(value, false);
	}

	public List<Descriptor> RequirementDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (formula.inputs != null && formula.inputs.Length != 0)
		{
			for (int i = 0; i < formula.inputs.Length; i++)
			{
				InputItem inputItem = formula.inputs[i];
				string arg = inputItem.tag.ProperName();
				Descriptor item = default(Descriptor);
				item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMED, arg, GameUtil.GetFormattedMass(inputItem.consumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMED, arg, GameUtil.GetFormattedMass(inputItem.consumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Requirement);
				list.Add(item);
			}
			return list;
		}
		return list;
	}

	public List<Descriptor> EffectDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (formula.outputs != null && formula.outputs.Length != 0)
		{
			for (int i = 0; i < formula.outputs.Length; i++)
			{
				OutputItem outputItem = formula.outputs[i];
				Element element = ElementLoader.FindElementByHash(outputItem.element);
				string arg = element.tag.ProperName();
				Descriptor item = default(Descriptor);
				item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTED, arg, GameUtil.GetFormattedMass(outputItem.creationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED, arg, GameUtil.GetFormattedMass(outputItem.creationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Effect);
				list.Add(item);
			}
			return list;
		}
		return list;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor item in RequirementDescriptors(def))
		{
			list.Add(item);
		}
		foreach (Descriptor item2 in EffectDescriptors(def))
		{
			list.Add(item2);
		}
		return list;
	}

	public static void EnsureStatusItemAvailable()
	{
		if (batteriesSufficientlyFull == null)
		{
			batteriesSufficientlyFull = new StatusItem("BatteriesSufficientlyFull", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
		}
	}

	public static Formula CreateSimpleFormula(SimHashes input_element, float input_mass_rate, float max_stored_input_mass, SimHashes output_element = SimHashes.Void, float output_mass_rate = 0f, bool store_output_mass = true)
	{
		Formula result = default(Formula);
		result.inputs = new InputItem[1]
		{
			new InputItem(GameTagExtensions.Create(input_element), input_mass_rate, max_stored_input_mass)
		};
		if (output_element != SimHashes.Void)
		{
			result.outputs = new OutputItem[1]
			{
				new OutputItem(output_element, output_mass_rate, store_output_mass, 0f)
			};
		}
		else
		{
			result.outputs = null;
		}
		return result;
	}

	private void Emit(OutputItem output, float dt, PrimaryElement root_pe)
	{
		Element element = ElementLoader.FindElementByHash(output.element);
		float num = output.creationRate * dt;
		if (output.store)
		{
			if (element.IsGas)
			{
				storage.AddGasChunk(output.element, num, root_pe.Temperature, byte.MaxValue, 0, true, true);
			}
			else if (element.IsLiquid)
			{
				storage.AddLiquid(output.element, num, root_pe.Temperature, byte.MaxValue, 0, true, true);
			}
			else
			{
				GameObject go = element.substance.SpawnResource(base.transform.GetPosition(), num, root_pe.Temperature, byte.MaxValue, 0, false, false, false);
				storage.Store(go, true, false, true, false);
			}
		}
		else
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			int num2 = Grid.OffsetCell(cell, output.emitOffset);
			float temperature = Mathf.Max(root_pe.Temperature, output.minTemperature);
			if (element.IsGas)
			{
				SimMessages.ModifyMass(num2, num, byte.MaxValue, 0, CellEventLogger.Instance.EnergyGeneratorModifyMass, temperature, output.element);
			}
			else if (element.IsLiquid)
			{
				int elementIndex = ElementLoader.GetElementIndex(output.element);
				FallingWater.instance.AddParticle(num2, (byte)elementIndex, num, temperature, byte.MaxValue, 0, true, false, false, false);
			}
			else
			{
				element.substance.SpawnResource(Grid.CellToPosCCC(num2, Grid.SceneLayer.Front), num, temperature, byte.MaxValue, 0, true, false, false);
			}
		}
	}
}
