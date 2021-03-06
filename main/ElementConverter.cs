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

		public float minOutputTemperature;

		public bool useEntityTemperature;

		public float massGenerationRate;

		public bool storeOutput;

		public Vector2 outputElementOffset;

		public HandleVector<int>.Handle accumulator;

		public float diseaseWeight;

		public byte addedDiseaseIdx;

		public int addedDiseaseCount;

		public string Name => ElementLoader.FindElementByHash(elementHash).tag.ProperName();

		public float Rate => Game.Instance.accumulators.GetAverageRate(accumulator);

		public OutputElement(float kgPerSecond, SimHashes element, float minOutputTemperature, bool useEntityTemperature = false, bool storeOutput = false, float outputElementOffsetx = 0f, float outputElementOffsety = 0.5f, float diseaseWeight = 1f, byte addedDiseaseIdx = byte.MaxValue, int addedDiseaseCount = 0)
		{
			elementHash = element;
			this.minOutputTemperature = minOutputTemperature;
			this.useEntityTemperature = useEntityTemperature;
			this.storeOutput = storeOutput;
			massGenerationRate = kgPerSecond;
			outputElementOffset = new Vector2(outputElementOffsetx, outputElementOffsety);
			accumulator = HandleVector<int>.InvalidHandle;
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

	private static StatusItem ElementConverterInput;

	private static StatusItem ElementConverterOutput;

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
					if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null) && gameObject.HasTag(tag))
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
				if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null) && gameObject.HasTag(consumedElement.tag) && gameObject.GetComponent<PrimaryElement>().Mass > 0f)
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
				if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null) && gameObject.HasTag(consumedElement.tag))
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
		float num2 = 1f;
		for (int i = 0; i < consumedElements.Length; i++)
		{
			ConsumedElement consumedElement = consumedElements[i];
			float num3 = consumedElement.massConsumptionRate * num * num2;
			if (num3 <= 0f)
			{
				num2 = 0f;
				break;
			}
			float num4 = 0f;
			for (int j = 0; j < storage.items.Count; j++)
			{
				GameObject gameObject = storage.items[j];
				if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null) && gameObject.HasTag(consumedElement.tag))
				{
					PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
					float num5 = Mathf.Min(num3, component.Mass);
					num4 += num5 / num3;
				}
			}
			num2 = Mathf.Min(num2, num4);
		}
		if (!(num2 <= 0f))
		{
			SimUtil.DiseaseInfo diseaseInfo = SimUtil.DiseaseInfo.Invalid;
			diseaseInfo.idx = byte.MaxValue;
			diseaseInfo.count = 0;
			float num6 = 0f;
			float num7 = 0f;
			float num8 = 0f;
			for (int k = 0; k < consumedElements.Length; k++)
			{
				ConsumedElement consumedElement2 = consumedElements[k];
				float num9 = consumedElement2.massConsumptionRate * num * num2;
				Game.Instance.accumulators.Accumulate(consumedElement2.accumulator, num9);
				for (int l = 0; l < storage.items.Count; l++)
				{
					GameObject gameObject2 = storage.items[l];
					if (!((UnityEngine.Object)gameObject2 == (UnityEngine.Object)null))
					{
						if (gameObject2.HasTag(consumedElement2.tag))
						{
							PrimaryElement component2 = gameObject2.GetComponent<PrimaryElement>();
							component2.KeepZeroMassObject = true;
							float num10 = Mathf.Min(num9, component2.Mass);
							float num11 = num10 / component2.Mass;
							int num12 = (int)(num11 * (float)component2.DiseaseCount);
							float num13 = num10 * component2.Element.specificHeatCapacity;
							num8 += num13;
							num7 += num13 * component2.Temperature;
							component2.Mass -= num10;
							component2.ModifyDiseaseCount(-num12, "ElementConverter.ConvertMass");
							num6 += num10;
							diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(diseaseInfo.idx, diseaseInfo.count, component2.DiseaseIdx, num12);
							num9 -= num10;
							if (num9 <= 0f)
							{
								break;
							}
						}
						if (num9 <= 0f)
						{
							Debug.Assert(num9 <= 0f);
						}
					}
				}
			}
			float num14 = (!(num8 > 0f)) ? 0f : (num7 / num8);
			if (onConvertMass != null && num6 > 0f)
			{
				onConvertMass(num6);
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
						float num15 = outputElement.diseaseWeight / totalDiseaseWeight;
						a.count = (int)((float)a.count * num15);
					}
					if (outputElement.addedDiseaseIdx != 255)
					{
						a = SimUtil.CalculateFinalDiseaseInfo(a, new SimUtil.DiseaseInfo
						{
							idx = outputElement.addedDiseaseIdx,
							count = outputElement.addedDiseaseCount
						});
					}
					float num16 = outputElement.massGenerationRate * OutputMultiplier * num * num2;
					Game.Instance.accumulators.Accumulate(outputElement.accumulator, num16);
					float num17 = 0f;
					num17 = ((!outputElement.useEntityTemperature && (num14 != 0f || outputElement.minOutputTemperature != 0f)) ? Mathf.Max(outputElement.minOutputTemperature, num14) : GetComponent<PrimaryElement>().Temperature);
					Element element = ElementLoader.FindElementByHash(outputElement.elementHash);
					if (outputElement.storeOutput)
					{
						PrimaryElement primaryElement = storage.AddToPrimaryElement(outputElement.elementHash, num16, num17);
						if ((UnityEngine.Object)primaryElement == (UnityEngine.Object)null)
						{
							if (element.IsGas)
							{
								storage.AddGasChunk(outputElement.elementHash, num16, num17, a.idx, a.count, true, true);
							}
							else if (element.IsLiquid)
							{
								storage.AddLiquid(outputElement.elementHash, num16, num17, a.idx, a.count, true, true);
							}
							else
							{
								GameObject go = element.substance.SpawnResource(base.transform.GetPosition(), num16, num17, a.idx, a.count, true, false, false);
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
						int num18 = Grid.PosToCell(vector);
						if (element.IsLiquid)
						{
							int idx = element.idx;
							FallingWater.instance.AddParticle(num18, (byte)idx, num16, num17, a.idx, a.count, true, false, false, false);
						}
						else if (element.IsSolid)
						{
							element.substance.SpawnResource(vector, num16, num17, a.idx, a.count, false, false, false);
						}
						else
						{
							SimMessages.AddRemoveSubstance(num18, outputElement.elementHash, CellEventLogger.Instance.OxygenModifierSimUpdate, num16, num17, a.idx, a.count, true, -1);
						}
					}
					if (outputElement.elementHash == SimHashes.Oxygen)
					{
						ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, num16, base.gameObject.GetProperName(), null);
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
			ElementConverterInput = new StatusItem("ElementConverterInput", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.None.ID, true, 129022).SetResolveStringCallback(delegate(string str, object data)
			{
				ConsumedElement consumedElement = (ConsumedElement)data;
				str = str.Replace("{ElementTypes}", consumedElement.Name);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedByTag(consumedElement.tag, consumedElement.Rate, GameUtil.TimeSlice.PerSecond));
				return str;
			});
		}
		if (ElementConverterOutput == null)
		{
			ElementConverterOutput = new StatusItem("ElementConverterOutput", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.None.ID, true, 129022).SetResolveStringCallback(delegate(string str, object data)
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
		if (!showDescriptors)
		{
			return list;
		}
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
				LocString loc_string;
				LocString loc_string2;
				if (outputElement.useEntityTemperature)
				{
					loc_string = UI.BUILDINGEFFECTS.ELEMENTEMITTED_ENTITYTEMP;
					loc_string2 = UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_ENTITYTEMP;
				}
				else if (outputElement.minOutputTemperature > 0f)
				{
					loc_string = UI.BUILDINGEFFECTS.ELEMENTEMITTED_MINTEMP;
					loc_string2 = UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_MINTEMP;
				}
				else
				{
					loc_string = UI.BUILDINGEFFECTS.ELEMENTEMITTED_INPUTTEMP;
					loc_string2 = UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_INPUTTEMP;
				}
				item2.SetupDescriptor(string.Format(loc_string, outputElement.Name, GameUtil.GetFormattedMass(outputElement.massGenerationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}"), GameUtil.GetFormattedTemperature(outputElement.minOutputTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), string.Format(loc_string2, outputElement.Name, GameUtil.GetFormattedMass(outputElement.massGenerationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}"), GameUtil.GetFormattedTemperature(outputElement.minOutputTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), Descriptor.DescriptorType.Effect);
				list.Add(item2);
			}
		}
		return list;
	}
}
