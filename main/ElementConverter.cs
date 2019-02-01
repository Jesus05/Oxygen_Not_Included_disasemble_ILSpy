using Klei;
using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ElementConverter : StateMachineComponent<ElementConverter.StatesInstance>, IEffectDescriptor
{
	[Serializable]
	[DebuggerDisplay("{tag} {massConsumptionRate}")]
	public struct ConsumedElement
	{
		public Tag tag;

		public float massConsumptionRate;

		public HandleVector<int>.Handle accumulator;

		public string Name => tag.ProperName();

		public float Rate => Game.Instance.accumulators.GetAverageRate(accumulator);

		public ConsumedElement(Tag tag, float kgPerSecond)
		{
			this.tag = tag;
			massConsumptionRate = kgPerSecond;
			accumulator = HandleVector<int>.InvalidHandle;
		}
	}

	[Serializable]
	public struct OutputElement
	{
		public SimHashes elementHash;

		public float outputTemperature;

		public float massGenerationRate;

		public bool storeOutput;

		public bool applyInputTemperature;

		public Vector2 outputElementOffset;

		public HandleVector<int>.Handle accumulator;

		public float diseaseWeight;

		public byte addedDiseaseIdx;

		public int addedDiseaseCount;

		public string Name => ElementLoader.FindElementByHash(elementHash).tag.ProperName();

		public float Rate => Game.Instance.accumulators.GetAverageRate(accumulator);

		public OutputElement(float kgPerSecond, SimHashes element, float outputTemperature = 0f, bool storeOutput = false, float outputElementOffsetx = 0f, float outputElementOffsety = 0.5f, bool apply_input_temperature = false, float diseaseWeight = 1f, byte addedDiseaseIdx = byte.MaxValue, int addedDiseaseCount = 0)
		{
			elementHash = element;
			this.outputTemperature = ((!(outputTemperature > 0f)) ? ElementLoader.FindElementByHash(element).defaultValues.temperature : outputTemperature);
			this.storeOutput = storeOutput;
			massGenerationRate = kgPerSecond;
			outputElementOffset = new Vector2(outputElementOffsetx, outputElementOffsety);
			accumulator = HandleVector<int>.InvalidHandle;
			applyInputTemperature = apply_input_temperature;
			this.diseaseWeight = diseaseWeight;
			this.addedDiseaseIdx = addedDiseaseIdx;
			this.addedDiseaseCount = addedDiseaseCount;
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, ElementConverter, object>.GameInstance
	{
		private List<Guid> statusItemEntries = new List<Guid>();

		public StatesInstance(ElementConverter smi)
			: base(smi)
		{
		}

		public void AddStatusItems()
		{
			ConsumedElement[] consumedElements = base.master.consumedElements;
			foreach (ConsumedElement consumedElement in consumedElements)
			{
				Guid item = base.master.GetComponent<KSelectable>().AddStatusItem(ElementConverterInput, consumedElement);
				statusItemEntries.Add(item);
			}
			OutputElement[] outputElements = base.master.outputElements;
			foreach (OutputElement outputElement in outputElements)
			{
				Guid item2 = base.master.GetComponent<KSelectable>().AddStatusItem(ElementConverterOutput, outputElement);
				statusItemEntries.Add(item2);
			}
		}

		public void RemoveStatusItems()
		{
			foreach (Guid statusItemEntry in statusItemEntries)
			{
				base.master.GetComponent<KSelectable>().RemoveStatusItem(statusItemEntry, false);
			}
			statusItemEntries.Clear();
		}
	}

	public class States : GameStateMachine<States, StatesInstance, ElementConverter>
	{
		public State disabled;

		public State converting;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = disabled;
			disabled.EventTransition(GameHashes.ActiveChanged, converting, (StatesInstance smi) => (UnityEngine.Object)smi.master.operational == (UnityEngine.Object)null || smi.master.operational.IsActive);
			converting.Enter("AddStatusItems", delegate(StatesInstance smi)
			{
				smi.AddStatusItems();
			}).Exit("RemoveStatusItems", delegate(StatesInstance smi)
			{
				smi.RemoveStatusItems();
			}).EventTransition(GameHashes.ActiveChanged, disabled, (StatesInstance smi) => (UnityEngine.Object)smi.master.operational != (UnityEngine.Object)null && !smi.master.operational.IsActive)
				.Update("ConvertMass", delegate(StatesInstance smi, float dt)
				{
					smi.master.ConvertMass();
				}, UpdateRate.SIM_1000ms, true);
		}
	}

	[MyCmpGet]
	private Operational operational;

	[MyCmpReq]
	private Storage storage;

	public Action<float> onConvertMass;

	private float totalDiseaseWeight = 3.40282347E+38f;

	private AttributeInstance machinerySpeedAttribute;

	private float workSpeedMultiplier = 1f;

	public bool showDescriptors = true;

	private const float BASE_INTERVAL = 1f;

	[SerializeField]
	public ConsumedElement[] consumedElements;

	[SerializeField]
	public OutputElement[] outputElements;

	private float outputMultiplier = 1f;

	private static StatusItem ElementConverterInput = null;

	private static StatusItem ElementConverterOutput = null;

	public float OutputMultiplier
	{
		get
		{
			return outputMultiplier;
		}
		set
		{
			outputMultiplier = value;
		}
	}

	public float AverageConvertRate => Game.Instance.accumulators.GetAverageRate(outputElements[0].accumulator);

	public void SetWorkSpeedMultiplier(float speed)
	{
		workSpeedMultiplier = speed;
	}

	public void SetStorage(Storage storage)
	{
		this.storage = storage;
	}

	public bool HasEnoughMass(Tag tag)
	{
		bool result = false;
		List<GameObject> items = storage.items;
		ConsumedElement[] array = consumedElements;
		for (int i = 0; i < array.Length; i++)
		{
			ConsumedElement consumedElement = array[i];
			if (tag == consumedElement.tag)
			{
				float num = 0f;
				for (int j = 0; j < items.Count; j++)
				{
					GameObject gameObject = items[j];
					if (gameObject.HasTag(tag))
					{
						num += gameObject.GetComponent<PrimaryElement>().Mass;
					}
				}
				result = (num >= consumedElement.massConsumptionRate);
				break;
			}
		}
		return result;
	}

	public bool HasEnoughMassToStartConverting()
	{
		return HasEnoughMass();
	}

	public bool CanConvertAtAll()
	{
		bool result = true;
		List<GameObject> items = storage.items;
		for (int i = 0; i < consumedElements.Length; i++)
		{
			ConsumedElement consumedElement = consumedElements[i];
			bool flag = false;
			for (int j = 0; j < items.Count; j++)
			{
				GameObject gameObject = items[j];
				if (gameObject.HasTag(consumedElement.tag) && gameObject.GetComponent<PrimaryElement>().Mass > 0f)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	private float GetSpeedMultiplier()
	{
		return machinerySpeedAttribute.GetTotalValue() * workSpeedMultiplier;
	}

	private bool HasEnoughMass()
	{
		float speedMultiplier = GetSpeedMultiplier();
		float num = 1f * speedMultiplier;
		bool result = true;
		List<GameObject> items = storage.items;
		for (int i = 0; i < consumedElements.Length; i++)
		{
			ConsumedElement consumedElement = consumedElements[i];
			float num2 = 0f;
			for (int j = 0; j < items.Count; j++)
			{
				GameObject gameObject = items[j];
				if (gameObject.HasTag(consumedElement.tag))
				{
					num2 += gameObject.GetComponent<PrimaryElement>().Mass;
				}
			}
			if (num2 < consumedElement.massConsumptionRate * num)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	private void ConvertMass()
	{
		float speedMultiplier = GetSpeedMultiplier();
		float num = 1f * speedMultiplier;
		float num2 = 0f;
		float num3 = 1f;
		for (int i = 0; i < consumedElements.Length; i++)
		{
			ConsumedElement consumedElement = consumedElements[i];
			float num4 = consumedElement.massConsumptionRate * num * num3;
			if (num4 <= 0f)
			{
				num3 = 0f;
				break;
			}
			float num5 = 0f;
			for (int j = 0; j < storage.items.Count; j++)
			{
				GameObject gameObject = storage.items[j];
				if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null) && gameObject.HasTag(consumedElement.tag))
				{
					PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
					float num6 = Mathf.Min(num4, component.Mass);
					num5 += num6 / num4;
				}
			}
			num3 = Mathf.Min(num3, num5);
		}
		if (!(num3 <= 0f))
		{
			SimUtil.DiseaseInfo diseaseInfo = SimUtil.DiseaseInfo.Invalid;
			diseaseInfo.idx = byte.MaxValue;
			diseaseInfo.count = 0;
			float num7 = 0f;
			for (int k = 0; k < consumedElements.Length; k++)
			{
				ConsumedElement consumedElement2 = consumedElements[k];
				float num8 = consumedElement2.massConsumptionRate * num * num3;
				Game.Instance.accumulators.Accumulate(consumedElement2.accumulator, num8);
				for (int l = 0; l < storage.items.Count; l++)
				{
					GameObject gameObject2 = storage.items[l];
					if (gameObject2.HasTag(consumedElement2.tag))
					{
						PrimaryElement component2 = gameObject2.GetComponent<PrimaryElement>();
						component2.KeepZeroMassObject = true;
						float num9 = Mathf.Min(num8, component2.Mass);
						float num10 = num9 / component2.Mass;
						int num11 = (int)(num10 * (float)component2.DiseaseCount);
						component2.Mass -= num9;
						component2.ModifyDiseaseCount(-num11, "ElementConverter.ConvertMass");
						num7 += num9;
						diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(diseaseInfo.idx, diseaseInfo.count, component2.DiseaseIdx, num11);
						num2 = component2.Temperature;
						num8 -= num9;
						if (num8 <= 0f)
						{
							break;
						}
					}
					if (!(num8 <= 0f))
					{
						continue;
					}
				}
			}
			if (onConvertMass != null && num7 > 0f)
			{
				onConvertMass(num7);
			}
			if (outputElements != null && outputElements.Length > 0)
			{
				for (int m = 0; m < outputElements.Length; m++)
				{
					OutputElement outputElement = outputElements[m];
					SimUtil.DiseaseInfo a = diseaseInfo;
					if (totalDiseaseWeight <= 0f)
					{
						a.idx = byte.MaxValue;
						a.count = 0;
					}
					else
					{
						float num12 = outputElement.diseaseWeight / totalDiseaseWeight;
						a.count = (int)((float)a.count * num12);
					}
					if (outputElement.addedDiseaseIdx != 255)
					{
						a = SimUtil.CalculateFinalDiseaseInfo(a, new SimUtil.DiseaseInfo
						{
							idx = outputElement.addedDiseaseIdx,
							count = outputElement.addedDiseaseCount
						});
					}
					float num13 = outputElement.massGenerationRate * OutputMultiplier * num * num3;
					Game.Instance.accumulators.Accumulate(outputElement.accumulator, num13);
					float temperature = (outputElement.outputTemperature != 0f) ? outputElement.outputTemperature : GetComponent<PrimaryElement>().Temperature;
					if (outputElement.applyInputTemperature)
					{
						temperature = num2;
					}
					Element element = ElementLoader.FindElementByHash(outputElement.elementHash);
					if (outputElement.storeOutput)
					{
						PrimaryElement primaryElement = storage.AddToPrimaryElement(outputElement.elementHash, num13, temperature);
						if ((UnityEngine.Object)primaryElement == (UnityEngine.Object)null)
						{
							if (element.IsGas)
							{
								storage.AddGasChunk(outputElement.elementHash, num13, temperature, a.idx, a.count, true, true);
							}
							else if (element.IsLiquid)
							{
								storage.AddLiquid(outputElement.elementHash, num13, temperature, a.idx, a.count, true, true);
							}
							else
							{
								GameObject go = element.substance.SpawnResource(base.transform.GetPosition(), num13, temperature, a.idx, a.count, true, false);
								storage.Store(go, true, false, true, false);
							}
						}
						else
						{
							primaryElement.AddDisease(a.idx, a.count, "ElementConverter.ConvertMass");
						}
					}
					else
					{
						Vector3 position = base.transform.GetPosition();
						float x = position.x + outputElement.outputElementOffset.x;
						Vector3 position2 = base.transform.GetPosition();
						Vector3 vector = new Vector3(x, position2.y + outputElement.outputElementOffset.y, 0f);
						int num14 = Grid.PosToCell(vector);
						if (element.IsLiquid)
						{
							int idx = element.idx;
							FallingWater.instance.AddParticle(num14, (byte)idx, num13, temperature, a.idx, a.count, true, false, false, false);
						}
						else if (element.IsSolid)
						{
							element.substance.SpawnResource(vector, num13, temperature, a.idx, a.count, false, false);
						}
						else
						{
							SimMessages.AddRemoveSubstance(num14, outputElement.elementHash, CellEventLogger.Instance.OxygenModifierSimUpdate, num13, temperature, a.idx, a.count, true, -1);
						}
					}
					if (outputElement.elementHash == SimHashes.Oxygen)
					{
						ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, num13, base.gameObject.GetProperName(), null);
					}
				}
			}
			storage.Trigger(-1697596308, base.gameObject);
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Attributes attributes = base.gameObject.GetAttributes();
		machinerySpeedAttribute = attributes.Add(Db.Get().Attributes.MachinerySpeed);
		if (ElementConverterInput == null)
		{
			ElementConverterInput = new StatusItem("ElementConverterInput", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.None.ID, true, 63486).SetResolveStringCallback(delegate(string str, object data)
			{
				ConsumedElement consumedElement = (ConsumedElement)data;
				str = str.Replace("{ElementTypes}", consumedElement.Name);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedByTag(consumedElement.tag, consumedElement.Rate, GameUtil.TimeSlice.PerSecond));
				return str;
			});
		}
		if (ElementConverterOutput == null)
		{
			ElementConverterOutput = new StatusItem("ElementConverterOutput", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.None.ID, true, 63486).SetResolveStringCallback(delegate(string str, object data)
			{
				OutputElement outputElement = (OutputElement)data;
				str = str.Replace("{ElementTypes}", outputElement.Name);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(outputElement.Rate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			});
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		for (int i = 0; i < consumedElements.Length; i++)
		{
			consumedElements[i].accumulator = Game.Instance.accumulators.Add("ElementsConsumed", this);
		}
		totalDiseaseWeight = 0f;
		for (int j = 0; j < outputElements.Length; j++)
		{
			outputElements[j].accumulator = Game.Instance.accumulators.Add("OutputElements", this);
			totalDiseaseWeight += outputElements[j].diseaseWeight;
		}
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		for (int i = 0; i < consumedElements.Length; i++)
		{
			Game.Instance.accumulators.Remove(consumedElements[i].accumulator);
		}
		for (int j = 0; j < outputElements.Length; j++)
		{
			Game.Instance.accumulators.Remove(outputElements[j].accumulator);
		}
		base.OnCleanUp();
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (showDescriptors)
		{
			if (consumedElements != null)
			{
				ConsumedElement[] array = consumedElements;
				for (int i = 0; i < array.Length; i++)
				{
					ConsumedElement consumedElement = array[i];
					Descriptor item = default(Descriptor);
					item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMED, consumedElement.Name, GameUtil.GetFormattedMass(consumedElement.massConsumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMED, consumedElement.Name, GameUtil.GetFormattedMass(consumedElement.massConsumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Requirement);
					list.Add(item);
				}
			}
			if (outputElements != null)
			{
				OutputElement[] array2 = outputElements;
				for (int j = 0; j < array2.Length; j++)
				{
					OutputElement outputElement = array2[j];
					Descriptor item2 = default(Descriptor);
					item2.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTED, outputElement.Name, GameUtil.GetFormattedMass(outputElement.massGenerationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED, outputElement.Name, GameUtil.GetFormattedMass(outputElement.massGenerationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Effect);
					list.Add(item2);
				}
			}
			return list;
		}
		return list;
	}
}
