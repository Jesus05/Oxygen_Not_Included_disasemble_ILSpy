using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class StructureTemperatureComponents : KGameObjectComponentManager<StructureTemperatureData>
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
		return Add(go, new StructureTemperatureData(go));
	}

	public static void ClearInstanceMap()
	{
		handleInstanceMap.Clear();
	}

	protected override void OnPrefabInit(HandleVector<int>.Handle handle)
	{
		InitializeStatusItem();
		base.OnPrefabInit(handle);
		StructureTemperatureData data = GetData(handle);
		data.primaryElement.getTemperatureCallback = OnGetTemperature;
		data.primaryElement.setTemperatureCallback = OnSetTemperature;
		data.isActiveBuilding = (data.building.Def.SelfHeatKilowattsWhenActive != 0f || data.ExhaustKilowatts != 0f);
		SetData(handle, data);
	}

	private void InitializeStatusItem()
	{
		if (operatingEnergyStatusItem == null)
		{
			operatingEnergyStatusItem = new StatusItem("OperatingEnergy", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 63486);
			operatingEnergyStatusItem.resolveStringCallback = delegate(string str, object ev_data)
			{
				int key = (int)ev_data;
				HandleVector<int>.Handle handle = handleInstanceMap[key];
				StructureTemperatureData data = GetData(handle);
				if (str != (string)BUILDING.STATUSITEMS.OPERATINGENERGY.TOOLTIP)
				{
					try
					{
						str = string.Format(str, GameUtil.GetFormattedHeatEnergy(data.TotalEnergyProducedKW * 1000f, GameUtil.HeatEnergyFormatterUnit.Automatic));
						return str;
					}
					catch (Exception obj)
					{
						Debug.LogWarning(obj, null);
						Debug.LogWarning(BUILDING.STATUSITEMS.OPERATINGENERGY.TOOLTIP, null);
						Debug.LogWarning(str, null);
						return str;
					}
				}
				string text = string.Empty;
				foreach (StructureTemperatureData.EnergySource item in data.energySourcesKW)
				{
					text += string.Format(BUILDING.STATUSITEMS.OPERATINGENERGY.LINEITEM, item.source, GameUtil.GetFormattedHeatEnergy(item.value * 1000f, GameUtil.HeatEnergyFormatterUnit.Automatic));
				}
				str = string.Format(str, GameUtil.GetFormattedHeatEnergy(data.TotalEnergyProducedKW * 1000f, GameUtil.HeatEnergyFormatterUnit.None), text);
				return str;
			};
		}
	}

	protected override void OnSpawn(HandleVector<int>.Handle handle)
	{
		StructureTemperatureData data = GetData(handle);
		if ((UnityEngine.Object)data.operational != (UnityEngine.Object)null && data.isActiveBuilding)
		{
			data.primaryElement.Subscribe(824508782, delegate
			{
				OnActiveChanged(handle);
			});
		}
		Overheatable component = data.primaryElement.GetComponent<Overheatable>();
		data.maxTemperature = ((!((UnityEngine.Object)component != (UnityEngine.Object)null)) ? 10000f : component.OverheatTemperature);
		if (data.maxTemperature <= 0f)
		{
			Output.LogError("invalid max temperature");
		}
		SetData(handle, data);
		SimRegister(handle, ref data);
	}

	private static void OnActiveChanged(HandleVector<int>.Handle handle)
	{
		StructureTemperatureData data = GameComps.StructureTemperatures.GetData(handle);
		float temperature = data.Temperature;
		data.primaryElement.InternalTemperature = temperature;
		data.dirty = true;
		GameComps.StructureTemperatures.SetData(handle, data);
	}

	protected override void OnCleanUp(HandleVector<int>.Handle handle)
	{
		SimUnregister(handle);
		base.OnCleanUp(handle);
	}

	public override void Sim200ms(float dt)
	{
		for (int i = 0; i < base.data.Count; i++)
		{
			StructureTemperatureData data = base.data[i];
			if (Sim.IsValidHandle(data.simHandle))
			{
				UpdateSimState(ref data);
				data.ApplyPendingEnergyModifications();
				if (!data.isActiveBuilding)
				{
					base.data[i] = data;
				}
				else
				{
					if ((UnityEngine.Object)data.operational == (UnityEngine.Object)null || data.operational.IsActive)
					{
						if (!data.isActiveStatusItemSet)
						{
							data.primaryElement.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.OperatingEnergy, operatingEnergyStatusItem, data.simHandle);
							data.isActiveStatusItemSet = true;
						}
						data.energySourcesKW = AccumulateProducedEnergyKW(data.energySourcesKW, data.OperatingKilowatts, BUILDING.STATUSITEMS.OPERATINGENERGY.OPERATING);
						if (data.ExhaustKilowatts != 0f)
						{
							Extents extents = data.GetExtents();
							int num = extents.width * extents.height;
							float num2 = data.ExhaustKilowatts * dt / (float)num;
							for (int j = 0; j < extents.height; j++)
							{
								int num3 = extents.y + j;
								for (int k = 0; k < extents.width; k++)
								{
									int num4 = extents.x + k;
									int num5 = num3 * Grid.WidthInCells + num4;
									float a = Grid.Mass[num5];
									float num6 = Mathf.Min(a, 1.5f) / 1.5f;
									float kilojoules = num2 * num6;
									SimMessages.ModifyEnergy(num5, kilojoules, data.maxTemperature, SimMessages.EnergySourceID.StructureTemperature);
								}
							}
							data.energySourcesKW = AccumulateProducedEnergyKW(data.energySourcesKW, data.ExhaustKilowatts, BUILDING.STATUSITEMS.OPERATINGENERGY.EXHAUSTING);
						}
					}
					else if (data.isActiveStatusItemSet)
					{
						data.primaryElement.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.OperatingEnergy, null, null);
						data.isActiveStatusItemSet = false;
					}
					base.data[i] = data;
				}
			}
		}
	}

	private static void UpdateSimState(ref StructureTemperatureData data)
	{
		if (data.dirty && Sim.IsValidHandle(data.simHandle))
		{
			data.dirty = false;
			float internalTemperature = data.primaryElement.InternalTemperature;
			BuildingDef def = data.building.Def;
			float mass = def.MassForTemperatureModification;
			float operatingKilowatts = data.OperatingKilowatts;
			Overheatable component = data.primaryElement.GetComponent<Overheatable>();
			float overheat_temperature = (!((UnityEngine.Object)component != (UnityEngine.Object)null)) ? 10000f : component.OverheatTemperature;
			if (!data.enabled)
			{
				mass = 0f;
			}
			Extents extents = data.GetExtents();
			byte idx = data.primaryElement.Element.idx;
			SimMessages.ModifyBuildingHeatExchange(data.simHandle, extents, mass, internalTemperature, def.ThermalConductivity, overheat_temperature, operatingKilowatts, idx);
		}
	}

	private unsafe static float OnGetTemperature(PrimaryElement primary_element)
	{
		HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(primary_element.gameObject);
		StructureTemperatureData data = GameComps.StructureTemperatures.GetData(handle);
		if (!Sim.IsValidHandle(data.simHandle))
		{
			return data.primaryElement.InternalTemperature;
		}
		if (!data.enabled)
		{
			int i = Grid.PosToCell(data.primaryElement.transform.GetPosition());
			return Grid.Temperature[i];
		}
		int handleIndex = Sim.GetHandleIndex(data.simHandle);
		return Game.Instance.simData.buildingTemperatures[handleIndex].temperature;
	}

	private static void OnSetTemperature(PrimaryElement primary_element, float temperature)
	{
		HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(primary_element.gameObject);
		StructureTemperatureData data = GameComps.StructureTemperatures.GetData(handle);
		data.primaryElement.InternalTemperature = temperature;
		data.dirty = true;
		if (!data.isActiveBuilding && Sim.IsValidHandle(data.simHandle))
		{
			UpdateSimState(ref data);
			data.ApplyPendingEnergyModifications();
		}
		GameComps.StructureTemperatures.SetData(handle, data);
	}

	public void ProduceEnergy(HandleVector<int>.Handle handle, float delta_kilojoules, string source, float display_dt)
	{
		StructureTemperatureData data = GetData(handle);
		data.ModifyEnergy(delta_kilojoules);
		data.energySourcesKW = AccumulateProducedEnergyKW(data.energySourcesKW, delta_kilojoules / display_dt, source);
	}

	private List<StructureTemperatureData.EnergySource> AccumulateProducedEnergyKW(List<StructureTemperatureData.EnergySource> sources, float kw, string source)
	{
		if (sources == null)
		{
			sources = new List<StructureTemperatureData.EnergySource>();
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
			sources.Add(new StructureTemperatureData.EnergySource(kw, source));
		}
		return sources;
	}

	public static void DoStateTransition(int sim_handle)
	{
		HandleVector<int>.Handle value = HandleVector<int>.InvalidHandle;
		if (handleInstanceMap.TryGetValue(sim_handle, out value))
		{
			StructureTemperatureData data = GameComps.StructureTemperatures.GetData(value);
			DoMelt(data.primaryElement);
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
			StructureTemperatureData data = GameComps.StructureTemperatures.GetData(value);
			data.primaryElement.gameObject.Trigger(1832602615, null);
		}
	}

	public static void DoNoLongerOverheated(int sim_handle)
	{
		HandleVector<int>.Handle value = HandleVector<int>.InvalidHandle;
		if (handleInstanceMap.TryGetValue(sim_handle, out value))
		{
			StructureTemperatureData data = GameComps.StructureTemperatures.GetData(value);
			data.primaryElement.gameObject.Trigger(171119937, null);
		}
	}

	public bool IsEnabled(HandleVector<int>.Handle handle)
	{
		StructureTemperatureData data = GetData(handle);
		return data.enabled;
	}

	public void Enable(HandleVector<int>.Handle handle)
	{
		StructureTemperatureData data = GetData(handle);
		data.enabled = true;
		data.dirty = true;
		SetData(handle, data);
	}

	public void Disable(HandleVector<int>.Handle handle)
	{
		StructureTemperatureData data = GetData(handle);
		data.enabled = false;
		data.dirty = true;
		SetData(handle, data);
	}

	protected void SimRegister(HandleVector<int>.Handle handle, ref StructureTemperatureData data)
	{
		if (data.simHandle == -1)
		{
			PrimaryElement primaryElement = data.primaryElement;
			if (primaryElement.Mass > 0f)
			{
				Element element = primaryElement.Element;
				if (!element.IsTemperatureInsulated)
				{
					data.simHandle = -2;
					string dbg_name = primaryElement.name;
					HandleVector<Game.ComplexCallbackInfo<int>>.Handle handle2 = Game.Instance.simComponentCallbackManager.Add(delegate(int sim_handle, object callback_data)
					{
						OnSimRegistered(handle, sim_handle, dbg_name);
					}, null, "StructureTemperature.SimRegister");
					BuildingDef def = primaryElement.GetComponent<Building>().Def;
					float internalTemperature = primaryElement.InternalTemperature;
					float massForTemperatureModification = def.MassForTemperatureModification;
					float operatingKilowatts = data.OperatingKilowatts;
					Extents extents = data.GetExtents();
					byte elem_idx = (byte)ElementLoader.elements.IndexOf(primaryElement.Element);
					SimMessages.AddBuildingHeatExchange(extents, massForTemperatureModification, internalTemperature, def.ThermalConductivity, operatingKilowatts, elem_idx, handle2.index);
					SetData(handle, data);
				}
			}
		}
	}

	private static void OnSimRegistered(HandleVector<int>.Handle handle, int sim_handle, string dbg_name)
	{
		if (GameComps.StructureTemperatures.IsValid(handle) && GameComps.StructureTemperatures.IsVersionValid(handle))
		{
			StructureTemperatureData data = GameComps.StructureTemperatures.GetData(handle);
			if (data.simHandle == -2)
			{
				data.simHandle = sim_handle;
				handleInstanceMap[sim_handle] = handle;
				GameComps.StructureTemperatures.SetData(handle, data);
				data.primaryElement.Trigger(-1555603773, null);
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
			StructureTemperatureData data = GetData(handle);
			if (data.simHandle != -1 && !KMonoBehaviour.isLoadingScene)
			{
				if (Sim.IsValidHandle(data.simHandle))
				{
					int handleIndex = Sim.GetHandleIndex(data.simHandle);
					data.primaryElement.InternalTemperature = Game.Instance.simData.buildingTemperatures[handleIndex].temperature;
					SimMessages.RemoveBuildingHeatExchange(data.simHandle, -1);
					handleInstanceMap.Remove(data.simHandle);
				}
				data.simHandle = -1;
				SetData(handle, data);
			}
		}
	}
}
