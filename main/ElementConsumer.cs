using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[SkipSaveFileSerialization]
[SerializationConfig(MemberSerialization.OptIn)]
public class ElementConsumer : SimComponent, ISaveLoadable, IEffectDescriptor
{
	public enum Configuration
	{
		Element,
		AllLiquid,
		AllGas
	}

	[HashedEnum]
	[SerializeField]
	public SimHashes elementToConsume = SimHashes.Vacuum;

	[SerializeField]
	public float consumptionRate;

	[SerializeField]
	public byte consumptionRadius = 1;

	[SerializeField]
	public float minimumMass;

	[SerializeField]
	public bool showInStatusPanel = true;

	[SerializeField]
	public Vector3 sampleCellOffset;

	[SerializeField]
	public float capacityKG = float.PositiveInfinity;

	[SerializeField]
	public Configuration configuration;

	[NonSerialized]
	[Serialize]
	public float consumedMass;

	[NonSerialized]
	[Serialize]
	public float consumedTemperature;

	[SerializeField]
	public bool storeOnConsume;

	[MyCmpGet]
	public Storage storage;

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private KSelectable selectable;

	private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;

	private Guid statusHandle;

	public bool showDescriptor = true;

	public bool isRequired = true;

	private bool consumptionEnabled;

	private bool hasAvailableCapacity = true;

	private static Dictionary<int, ElementConsumer> handleInstanceMap = new Dictionary<int, ElementConsumer>();

	private static readonly EventSystem.IntraObjectHandler<ElementConsumer> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<ElementConsumer>(delegate(ElementConsumer component, object data)
	{
		component.OnActiveChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<ElementConsumer> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<ElementConsumer>(delegate(ElementConsumer component, object data)
	{
		component.OnStorageChange(data);
	});

	[CompilerGenerated]
	private static Action<int> _003C_003Ef__mg_0024cache0;

	public float AverageConsumeRate => Game.Instance.accumulators.GetAverageRate(accumulator);

	public bool IsElementAvailable
	{
		get
		{
			int sampleCell = GetSampleCell();
			SimHashes id = Grid.Element[sampleCell].id;
			return elementToConsume == id && Grid.Mass[sampleCell] >= minimumMass;
		}
	}

	public static void ClearInstanceMap()
	{
		handleInstanceMap.Clear();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		accumulator = Game.Instance.accumulators.Add("Element", this);
		if (elementToConsume == SimHashes.Void)
		{
			throw new ArgumentException("No consumable elements specified");
		}
		Subscribe(824508782, OnActiveChangedDelegate);
		if (capacityKG != float.PositiveInfinity)
		{
			hasAvailableCapacity = !IsStorageFull();
			Subscribe(-1697596308, OnStorageChangeDelegate);
		}
	}

	protected override void OnCleanUp()
	{
		Game.Instance.accumulators.Remove(accumulator);
		base.OnCleanUp();
	}

	protected virtual bool IsActive()
	{
		return (UnityEngine.Object)operational == (UnityEngine.Object)null || operational.IsActive;
	}

	public void EnableConsumption(bool enabled)
	{
		bool flag = consumptionEnabled;
		consumptionEnabled = enabled;
		if (Sim.IsValidHandle(simHandle) && enabled != flag)
		{
			UpdateSimData();
			UpdateStatusItem();
		}
	}

	private bool IsStorageFull()
	{
		PrimaryElement primaryElement = storage.FindPrimaryElement(elementToConsume);
		if ((UnityEngine.Object)primaryElement != (UnityEngine.Object)null)
		{
			return primaryElement.Mass >= capacityKG;
		}
		return false;
	}

	public void RefreshConsumptionRate()
	{
		if (Sim.IsValidHandle(simHandle))
		{
			UpdateSimData();
		}
	}

	private void UpdateSimData()
	{
		int sampleCell = GetSampleCell();
		float num = (!consumptionEnabled || !hasAvailableCapacity) ? 0f : consumptionRate;
		SimMessages.SetElementConsumerData(simHandle, sampleCell, num);
	}

	public static void AddMass(Sim.ConsumedMassInfo consumed_info)
	{
		if (Sim.IsValidHandle(consumed_info.simHandle) && handleInstanceMap.TryGetValue(consumed_info.simHandle, out ElementConsumer value))
		{
			value.AddMassInternal(consumed_info);
		}
	}

	private int GetSampleCell()
	{
		return Grid.PosToCell(base.transform.GetPosition() + sampleCellOffset);
	}

	private void AddMassInternal(Sim.ConsumedMassInfo consumed_info)
	{
		if (consumed_info.mass > 0f)
		{
			if (storeOnConsume)
			{
				Element element = ElementLoader.elements[consumed_info.removedElemIdx];
				if (elementToConsume == SimHashes.Vacuum || elementToConsume == element.id)
				{
					if (element.IsLiquid)
					{
						storage.AddLiquid(element.id, consumed_info.mass, consumed_info.temperature, consumed_info.diseaseIdx, consumed_info.diseaseCount, true, true);
					}
					else if (element.IsGas)
					{
						storage.AddGasChunk(element.id, consumed_info.mass, consumed_info.temperature, consumed_info.diseaseIdx, consumed_info.diseaseCount, true, true);
					}
				}
			}
			else
			{
				consumedTemperature = GameUtil.GetFinalTemperature(consumed_info.temperature, consumed_info.mass, consumedTemperature, consumedMass);
				consumedMass += consumed_info.mass;
			}
		}
		Game.Instance.accumulators.Accumulate(accumulator, consumed_info.mass);
	}

	private void UpdateStatusItem()
	{
		if (showInStatusPanel)
		{
			if (statusHandle == Guid.Empty && IsActive() && consumptionEnabled)
			{
				statusHandle = selectable.AddStatusItem(Db.Get().BuildingStatusItems.ElementConsumer, this);
			}
			else if (statusHandle != Guid.Empty)
			{
				GetComponent<KSelectable>().RemoveStatusItem(statusHandle, false);
			}
		}
	}

	private void OnStorageChange(object data)
	{
		bool flag = !IsStorageFull();
		if (flag != hasAvailableCapacity)
		{
			hasAvailableCapacity = flag;
			RefreshConsumptionRate();
		}
	}

	protected override void OnCmpEnable()
	{
		if (base.isSpawned && IsActive())
		{
			UpdateStatusItem();
		}
	}

	protected override void OnCmpDisable()
	{
		UpdateStatusItem();
	}

	public List<Descriptor> RequirementDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (isRequired && showDescriptor)
		{
			Element element = ElementLoader.FindElementByHash(elementToConsume);
			string arg = element.tag.ProperName();
			if (element.IsVacuum)
			{
				arg = ((configuration == Configuration.AllGas) ? ((string)ELEMENTS.STATE.GAS) : ((configuration != Configuration.AllLiquid) ? ((string)UI.BUILDINGEFFECTS.CONSUMESANYELEMENT) : ((string)ELEMENTS.STATE.LIQUID)));
			}
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.REQUIRESELEMENT, arg), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESELEMENT, arg), Descriptor.DescriptorType.Requirement);
			list.Add(item);
		}
		return list;
	}

	public List<Descriptor> EffectDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (showDescriptor)
		{
			Element element = ElementLoader.FindElementByHash(elementToConsume);
			string arg = element.tag.ProperName();
			if (element.IsVacuum)
			{
				arg = ((configuration == Configuration.AllGas) ? ((string)ELEMENTS.STATE.GAS) : ((configuration != Configuration.AllLiquid) ? ((string)UI.BUILDINGEFFECTS.CONSUMESANYELEMENT) : ((string)ELEMENTS.STATE.LIQUID)));
			}
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMED, arg, GameUtil.GetFormattedMass(consumptionRate / 100f * 100f, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMED, arg, GameUtil.GetFormattedMass(consumptionRate / 100f * 100f, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Effect);
			list.Add(item);
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

	private void OnActiveChanged(object data)
	{
		bool isActive = operational.IsActive;
		EnableConsumption(isActive);
	}

	protected override void OnSimUnregister()
	{
		handleInstanceMap.Remove(simHandle);
		StaticUnregister(simHandle);
	}

	protected override void OnSimRegister(HandleVector<Game.ComplexCallbackInfo<int>>.Handle cb_handle)
	{
		int sampleCell = GetSampleCell();
		SimMessages.AddElementConsumer(sampleCell, configuration, elementToConsume, consumptionRadius, cb_handle.index);
	}

	protected override Action<int> GetStaticUnregister()
	{
		return StaticUnregister;
	}

	private static void StaticUnregister(int sim_handle)
	{
		SimMessages.RemoveElementConsumer(-1, sim_handle);
	}

	protected override void OnSimRegistered()
	{
		if (consumptionEnabled)
		{
			UpdateSimData();
		}
		handleInstanceMap[simHandle] = this;
	}
}
