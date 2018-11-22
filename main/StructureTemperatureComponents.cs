#define UNITY_ASSERTIONS
using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class StructureTemperatureComponents : KGameObjectSplitComponentManager<StructureTemperatureHeader, StructureTemperaturePayload>
{
	private const float MAX_PRESSURE = 1.5f;

	private static Dictionary<int, HandleVector<int>.Handle> handleInstanceMap = new Dictionary<int, HandleVector<int>.Handle>();

	private StatusItem operatingEnergyStatusItem;

	[CompilerGenerated]
	private static PrimaryElement.GetTemperatureCallback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static PrimaryElement.SetTemperatureCallback _003C_003Ef__mg_0024cache1;

	public HandleVector<int>.Handle Add(GameObject go)
	{
		StructureTemperaturePayload payload = new StructureTemperaturePayload(go);
		return Add(go, new StructureTemperatureHeader
		{
			dirty = false,
			simHandle = -1,
			isActiveBuilding = false
		}, ref payload);
	}

	public static void ClearInstanceMap()
	{
		handleInstanceMap.Clear();
	}

	protected override void OnPrefabInit(HandleVector<int>.Handle handle)
	{
		InitializeStatusItem();
		base.OnPrefabInit(handle);
		GetData(handle, out StructureTemperatureHeader header, out StructureTemperaturePayload payload);
		payload.primaryElement.getTemperatureCallback = OnGetTemperature;
		payload.primaryElement.setTemperatureCallback = OnSetTemperature;
		header.isActiveBuilding = (payload.building.Def.SelfHeatKilowattsWhenActive != 0f || payload.ExhaustKilowatts != 0f);
		SetData(handle, header, ref payload);
	}

	private void InitializeStatusItem()
	{
		if (operatingEnergyStatusItem == null)
		{
			operatingEnergyStatusItem = new StatusItem("OperatingEnergy", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			operatingEnergyStatusItem.resolveStringCallback = delegate(string str, object ev_data)
			{
				int key = (int)ev_data;
				HandleVector<int>.Handle handle = handleInstanceMap[key];
				StructureTemperaturePayload payload = GetPayload(handle);
				if (str != (string)BUILDING.STATUSITEMS.OPERATINGENERGY.TOOLTIP)
				{
					try
					{
						str = string.Format(str, GameUtil.GetFormattedHeatEnergy(payload.TotalEnergyProducedKW * 1000f, GameUtil.HeatEnergyFormatterUnit.Automatic));
					}
					catch (Exception obj)
					{
						Debug.LogWarning(obj, null);
						Debug.LogWarning(BUILDING.STATUSITEMS.OPERATINGENERGY.TOOLTIP, null);
						Debug.LogWarning(str, null);
					}
				}
				else
				{
					string text = "";
					foreach (StructureTemperaturePayload.EnergySource item in payload.energySourcesKW)
					{
						text += string.Format(BUILDING.STATUSITEMS.OPERATINGENERGY.LINEITEM, item.source, GameUtil.GetFormattedHeatEnergy(item.value * 1000f, GameUtil.HeatEnergyFormatterUnit.Automatic));
					}
					str = string.Format(str, GameUtil.GetFormattedHeatEnergy(payload.TotalEnergyProducedKW * 1000f, GameUtil.HeatEnergyFormatterUnit.None), text);
				}
				return str;
			};
		}
	}

	protected override void OnSpawn(HandleVector<int>.Handle handle)
	{
		GetData(handle, out StructureTemperatureHeader header, out StructureTemperaturePayload payload);
		if ((UnityEngine.Object)payload.operational != (UnityEngine.Object)null && header.isActiveBuilding)
		{
			payload.primaryElement.Subscribe(824508782, delegate
			{
				OnActiveChanged(handle);
			});
		}
		payload.maxTemperature = ((!((UnityEngine.Object)payload.overheatable != (UnityEngine.Object)null)) ? 10000f : payload.overheatable.OverheatTemperature);
		if (payload.maxTemperature <= 0f)
		{
			Output.LogError("invalid max temperature");
		}
		SetPayload(handle, ref payload);
		SimRegister(handle, ref header, ref payload);
	}

	private static void OnActiveChanged(HandleVector<int>.Handle handle)
	{
		StructureTemperaturePayload new_data = GameComps.StructureTemperatures.GetPayload(handle);
		new_data.primaryElement.InternalTemperature = new_data.Temperature;
		GameComps.StructureTemperatures.SetPayload(handle, ref new_data);
		StructureTemperatureHeader header = GameComps.StructureTemperatures.GetHeader(handle);
		header.dirty = true;
		GameComps.StructureTemperatures.SetHeader(handle, header);
	}

	protected override void OnCleanUp(HandleVector<int>.Handle handle)
	{
		SimUnregister(handle);
		base.OnCleanUp(handle);
	}

	public void ApplyPendingEnergyModifications(float pendingEnergyModifications, int simHandle)
	{
		if (pendingEnergyModifications != 0f)
		{
			SimMessages.ModifyBuildingEnergy(simHandle, pendingEnergyModifications, 0f, 10000f);
			pendingEnergyModifications = 0f;
		}
	}

	public override void Sim200ms(float dt)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		GetDataLists(out List<StructureTemperatureHeader> headers, out List<StructureTemperaturePayload> payloads);
		ListPool<int, StructureTemperatureComponents>.PooledList pooledList = ListPool<int, StructureTemperatureComponents>.Allocate();
		pooledList.Capacity = Math.Max(pooledList.Capacity, headers.Count);
		ListPool<int, StructureTemperatureComponents>.PooledList pooledList2 = ListPool<int, StructureTemperatureComponents>.Allocate();
		pooledList2.Capacity = Math.Max(pooledList2.Capacity, headers.Count);
		ListPool<int, StructureTemperatureComponents>.PooledList pooledList3 = ListPool<int, StructureTemperatureComponents>.Allocate();
		pooledList3.Capacity = Math.Max(pooledList3.Capacity, headers.Count);
		for (int i = 0; i != headers.Count; i++)
		{
			StructureTemperatureHeader value = headers[i];
			if (Sim.IsValidHandle(value.simHandle))
			{
				pooledList.Add(i);
				if (value.dirty)
				{
					pooledList2.Add(i);
					value.dirty = false;
					headers[i] = value;
				}
				if (value.isActiveBuilding)
				{
					pooledList3.Add(i);
				}
			}
		}
		foreach (int item in pooledList2)
		{
			StructureTemperaturePayload payload = payloads[item];
			UpdateSimState(ref payload);
		}
		foreach (int item2 in pooledList2)
		{
			StructureTemperaturePayload structureTemperaturePayload = payloads[item2];
			ApplyPendingEnergyModifications(structureTemperaturePayload.pendingEnergyModifications, structureTemperaturePayload.simHandleCopy);
		}
		foreach (int item3 in pooledList3)
		{
			StructureTemperaturePayload value2 = payloads[item3];
			if ((UnityEngine.Object)value2.operational == (UnityEngine.Object)null || value2.operational.IsActive)
			{
				num++;
				if (!value2.isActiveStatusItemSet)
				{
					num3++;
					value2.primaryElement.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.OperatingEnergy, operatingEnergyStatusItem, value2.simHandleCopy);
					value2.isActiveStatusItemSet = true;
				}
				value2.energySourcesKW = AccumulateProducedEnergyKW(value2.energySourcesKW, value2.OperatingKilowatts, BUILDING.STATUSITEMS.OPERATINGENERGY.OPERATING);
				if (value2.ExhaustKilowatts != 0f)
				{
					num2++;
					Extents extents = value2.GetExtents();
					int num4 = extents.width * extents.height;
					float num5 = value2.ExhaustKilowatts * dt / (float)num4;
					for (int j = 0; j < extents.height; j++)
					{
						int num6 = extents.y + j;
						for (int k = 0; k < extents.width; k++)
						{
							int num7 = extents.x + k;
							int num8 = num6 * Grid.WidthInCells + num7;
							float a = Grid.Mass[num8];
							float num9 = Mathf.Min(a, 1.5f) / 1.5f;
							float kilojoules = num5 * num9;
							SimMessages.ModifyEnergy(num8, kilojoules, value2.maxTemperature, SimMessages.EnergySourceID.StructureTemperature);
						}
					}
					value2.energySourcesKW = AccumulateProducedEnergyKW(value2.energySourcesKW, value2.ExhaustKilowatts, BUILDING.STATUSITEMS.OPERATINGENERGY.EXHAUSTING);
				}
			}
			else if (value2.isActiveStatusItemSet)
			{
				num3++;
				value2.primaryElement.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.OperatingEnergy, null, null);
				value2.isActiveStatusItemSet = false;
			}
			payloads[item3] = value2;
		}
		pooledList3.Recycle();
		pooledList2.Recycle();
		pooledList.Recycle();
	}

	private static void UpdateSimState(ref StructureTemperaturePayload payload)
	{
		DebugUtil.Assert(Sim.IsValidHandle(payload.simHandleCopy));
		float internalTemperature = payload.primaryElement.InternalTemperature;
		BuildingDef def = payload.building.Def;
		float num = def.MassForTemperatureModification;
		float operatingKilowatts = payload.OperatingKilowatts;
		float overheat_temperature = (!((UnityEngine.Object)payload.overheatable != (UnityEngine.Object)null)) ? 10000f : payload.overheatable.OverheatTemperature;
		UnityEngine.Debug.Assert(internalTemperature > 0f, "Invalid temperature");
		UnityEngine.Debug.Assert(num > 0f);
		if (!payload.enabled || payload.bypass)
		{
			num = 0f;
		}
		Extents extents = payload.GetExtents();
		byte idx = payload.primaryElement.Element.idx;
		SimMessages.ModifyBuildingHeatExchange(payload.simHandleCopy, extents, num, internalTemperature, def.ThermalConductivity, overheat_temperature, operatingKilowatts, idx);
	}

	private unsafe static float OnGetTemperature(PrimaryElement primary_element)
	{
		HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(primary_element.gameObject);
		StructureTemperaturePayload payload = GameComps.StructureTemperatures.GetPayload(handle);
		if (Sim.IsValidHandle(payload.simHandleCopy) && payload.enabled)
		{
			if (payload.bypass)
			{
				int i = Grid.PosToCell(payload.primaryElement.transform.GetPosition());
				return Grid.Temperature[i];
			}
			int handleIndex = Sim.GetHandleIndex(payload.simHandleCopy);
			return Game.Instance.simData.buildingTemperatures[handleIndex].temperature;
		}
		return payload.primaryElement.InternalTemperature;
	}

	private static void OnSetTemperature(PrimaryElement primary_element, float temperature)
	{
		HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(primary_element.gameObject);
		GameComps.StructureTemperatures.GetData(handle, out StructureTemperatureHeader header, out StructureTemperaturePayload payload);
		payload.primaryElement.InternalTemperature = temperature;
		GameComps.StructureTemperatures.SetPayload(handle, ref payload);
		if (!header.isActiveBuilding && Sim.IsValidHandle(payload.simHandleCopy))
		{
			UpdateSimState(ref payload);
			GameComps.StructureTemperatures.ApplyPendingEnergyModifications(payload.pendingEnergyModifications, payload.simHandleCopy);
		}
	}

	public void ProduceEnergy(HandleVector<int>.Handle handle, float delta_kilojoules, string source, float display_dt)
	{
		StructureTemperaturePayload new_data = GetPayload(handle);
		if (Sim.IsValidHandle(new_data.simHandleCopy))
		{
			SimMessages.ModifyBuildingEnergy(new_data.simHandleCopy, delta_kilojoules, 0f, 10000f);
		}
		else
		{
			new_data.pendingEnergyModifications += delta_kilojoules;
			StructureTemperatureHeader header = GetHeader(handle);
			header.dirty = true;
			SetHeader(handle, header);
		}
		new_data.energySourcesKW = AccumulateProducedEnergyKW(new_data.energySourcesKW, delta_kilojoules / display_dt, source);
		SetPayload(handle, ref new_data);
	}

	private List<StructureTemperaturePayload.EnergySource> AccumulateProducedEnergyKW(List<StructureTemperaturePayload.EnergySource> sources, float kw, string source)
	{
		if (sources == null)
		{
			sources = new List<StructureTemperaturePayload.EnergySource>();
		}
		bool flag = false;
		for (int i = 0; i < sources.Count; i++)
		{
			if (sources[i].source == source)
			{
				sources[i].Accumulate(kw);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			sources.Add(new StructureTemperaturePayload.EnergySource(kw, source));
		}
		return sources;
	}

	public static void DoStateTransition(int sim_handle)
	{
		HandleVector<int>.Handle value = HandleVector<int>.InvalidHandle;
		if (handleInstanceMap.TryGetValue(sim_handle, out value))
		{
			DoMelt(GameComps.StructureTemperatures.GetPayload(value).primaryElement);
		}
	}

	public static void DoMelt(PrimaryElement primary_element)
	{
		Element element = primary_element.Element;
		if (element.highTempTransitionTarget != SimHashes.Unobtanium)
		{
			int gameCell = Grid.PosToCell(primary_element.transform.GetPosition());
			SimMessages.AddRemoveSubstance(gameCell, element.highTempTransitionTarget, CellEventLogger.Instance.OreMelted, primary_element.Mass, primary_element.Element.highTemp, primary_element.DiseaseIdx, primary_element.DiseaseCount, true, -1);
			Util.KDestroyGameObject(primary_element.gameObject);
		}
	}

	public static void DoOverheat(int sim_handle)
	{
		HandleVector<int>.Handle value = HandleVector<int>.InvalidHandle;
		if (handleInstanceMap.TryGetValue(sim_handle, out value))
		{
			GameComps.StructureTemperatures.GetPayload(value).primaryElement.gameObject.Trigger(1832602615, null);
		}
	}

	public static void DoNoLongerOverheated(int sim_handle)
	{
		HandleVector<int>.Handle value = HandleVector<int>.InvalidHandle;
		if (handleInstanceMap.TryGetValue(sim_handle, out value))
		{
			GameComps.StructureTemperatures.GetPayload(value).primaryElement.gameObject.Trigger(171119937, null);
		}
	}

	private void MarkDirty(HandleVector<int>.Handle handle)
	{
		StructureTemperatureHeader header = GetHeader(handle);
		header.dirty = true;
		SetHeader(handle, header);
	}

	public bool IsEnabled(HandleVector<int>.Handle handle)
	{
		StructureTemperaturePayload payload = GetPayload(handle);
		return payload.enabled;
	}

	private void Enable(HandleVector<int>.Handle handle, bool isEnabled)
	{
		StructureTemperaturePayload new_data = GetPayload(handle);
		new_data.enabled = isEnabled;
		SetPayload(handle, ref new_data);
		MarkDirty(handle);
	}

	public void Enable(HandleVector<int>.Handle handle)
	{
		Enable(handle, true);
	}

	public void Disable(HandleVector<int>.Handle handle)
	{
		Enable(handle, false);
	}

	public bool IsBypassed(HandleVector<int>.Handle handle)
	{
		StructureTemperaturePayload payload = GetPayload(handle);
		return payload.bypass;
	}

	private void Bypass(HandleVector<int>.Handle handle, bool bypass)
	{
		StructureTemperaturePayload new_data = GetPayload(handle);
		new_data.bypass = bypass;
		SetPayload(handle, ref new_data);
		MarkDirty(handle);
	}

	public void Bypass(HandleVector<int>.Handle handle)
	{
		Bypass(handle, true);
	}

	public void UnBypass(HandleVector<int>.Handle handle)
	{
		Bypass(handle, false);
	}

	protected void SimRegister(HandleVector<int>.Handle handle, ref StructureTemperatureHeader header, ref StructureTemperaturePayload payload)
	{
		if (payload.simHandleCopy == -1)
		{
			PrimaryElement primaryElement = payload.primaryElement;
			if (!(primaryElement.Mass <= 0f))
			{
				Element element = primaryElement.Element;
				if (!element.IsTemperatureInsulated)
				{
					payload.simHandleCopy = -2;
					string dbg_name = primaryElement.name;
					HandleVector<Game.ComplexCallbackInfo<int>>.Handle handle2 = Game.Instance.simComponentCallbackManager.Add(delegate(int sim_handle, object callback_data)
					{
						OnSimRegistered(handle, sim_handle, dbg_name);
					}, null, "StructureTemperature.SimRegister");
					BuildingDef def = primaryElement.GetComponent<Building>().Def;
					float internalTemperature = primaryElement.InternalTemperature;
					float massForTemperatureModification = def.MassForTemperatureModification;
					float operatingKilowatts = payload.OperatingKilowatts;
					UnityEngine.Debug.Assert(0f < internalTemperature && internalTemperature < 10000f, "Invalid temperature");
					UnityEngine.Debug.Assert(primaryElement.Mass > 0f);
					UnityEngine.Debug.Assert(massForTemperatureModification > 0f);
					Extents extents = payload.GetExtents();
					byte elem_idx = (byte)ElementLoader.elements.IndexOf(primaryElement.Element);
					SimMessages.AddBuildingHeatExchange(extents, massForTemperatureModification, internalTemperature, def.ThermalConductivity, operatingKilowatts, elem_idx, handle2.index);
					SetPayload(handle, ref payload);
					header.simHandle = payload.simHandleCopy;
					SetHeader(handle, header);
				}
			}
		}
	}

	private static void OnSimRegistered(HandleVector<int>.Handle handle, int sim_handle, string dbg_name)
	{
		if (GameComps.StructureTemperatures.IsValid(handle) && GameComps.StructureTemperatures.IsVersionValid(handle))
		{
			StructureTemperaturePayload new_data = GameComps.StructureTemperatures.GetPayload(handle);
			if (new_data.simHandleCopy == -2)
			{
				new_data.simHandleCopy = sim_handle;
				handleInstanceMap[sim_handle] = handle;
				GameComps.StructureTemperatures.SetPayload(handle, ref new_data);
				StructureTemperatureHeader header = GameComps.StructureTemperatures.GetHeader(handle);
				header.simHandle = sim_handle;
				GameComps.StructureTemperatures.SetHeader(handle, header);
				new_data.primaryElement.Trigger(-1555603773, null);
			}
			else
			{
				SimMessages.RemoveBuildingHeatExchange(sim_handle, -1);
			}
		}
	}

	protected unsafe void SimUnregister(HandleVector<int>.Handle handle)
	{
		if (!GameComps.StructureTemperatures.IsVersionValid(handle))
		{
			KCrashReporter.Assert(false, "Handle version mismatch in StructureTemperature.SimUnregister");
		}
		else
		{
			StructureTemperaturePayload new_data = GetPayload(handle);
			if (new_data.simHandleCopy != -1 && !KMonoBehaviour.isLoadingScene)
			{
				if (Sim.IsValidHandle(new_data.simHandleCopy))
				{
					int handleIndex = Sim.GetHandleIndex(new_data.simHandleCopy);
					new_data.primaryElement.InternalTemperature = Game.Instance.simData.buildingTemperatures[handleIndex].temperature;
					SimMessages.RemoveBuildingHeatExchange(new_data.simHandleCopy, -1);
					handleInstanceMap.Remove(new_data.simHandleCopy);
				}
				new_data.simHandleCopy = -1;
				SetPayload(handle, ref new_data);
				StructureTemperatureHeader header = GetHeader(handle);
				header.simHandle = -1;
				SetHeader(handle, header);
			}
		}
	}
}
