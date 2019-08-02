using System;
using System.Runtime.InteropServices;

public class ConduitTemperatureManager
{
	private struct ConduitInfo
	{
		public ConduitType type;

		public int idx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct ConduitTemperatureUpdateData
	{
		public int numEntries;

		public unsafe float* temperatures;

		public int numFrozenHandles;

		public unsafe int* frozenHandles;

		public int numMeltedHandles;

		public unsafe int* meltedHandles;
	}

	private float[] temperatures = new float[0];

	private ConduitInfo[] conduitInfo = new ConduitInfo[0];

	public ConduitTemperatureManager()
	{
		ConduitTemperatureManager_Initialize();
	}

	public void Shutdown()
	{
		ConduitTemperatureManager_Shutdown();
	}

	public HandleVector<int>.Handle Allocate(ConduitType conduit_type, int conduit_idx, HandleVector<int>.Handle conduit_structure_temperature_handle, ref ConduitFlow.ConduitContents contents)
	{
		StructureTemperaturePayload payload = GameComps.StructureTemperatures.GetPayload(conduit_structure_temperature_handle);
		Element element = payload.primaryElement.Element;
		BuildingDef def = payload.building.Def;
		float conduit_heat_capacity = def.MassForTemperatureModification * element.specificHeatCapacity;
		float conduit_thermal_conductivity = element.thermalConductivity * def.ThermalConductivity;
		int num = ConduitTemperatureManager_Add(contents.temperature, contents.mass, (int)contents.element, payload.simHandleCopy, conduit_heat_capacity, conduit_thermal_conductivity, def.ThermalConductivity < 1f);
		HandleVector<int>.Handle result = default(HandleVector<int>.Handle);
		result.index = num;
		int handleIndex = Sim.GetHandleIndex(num);
		if (handleIndex + 1 > temperatures.Length)
		{
			Array.Resize(ref temperatures, (handleIndex + 1) * 2);
			Array.Resize(ref conduitInfo, (handleIndex + 1) * 2);
		}
		temperatures[handleIndex] = contents.temperature;
		conduitInfo[handleIndex] = new ConduitInfo
		{
			type = conduit_type,
			idx = conduit_idx
		};
		return result;
	}

	public void SetData(HandleVector<int>.Handle handle, ref ConduitFlow.ConduitContents contents)
	{
		if (handle.IsValid())
		{
			temperatures[Sim.GetHandleIndex(handle.index)] = contents.temperature;
			ConduitTemperatureManager_Set(handle.index, contents.temperature, contents.mass, (int)contents.element);
		}
	}

	public void Free(HandleVector<int>.Handle handle)
	{
		if (handle.IsValid())
		{
			int handleIndex = Sim.GetHandleIndex(handle.index);
			temperatures[handleIndex] = -1f;
			conduitInfo[handleIndex] = new ConduitInfo
			{
				type = ConduitType.None,
				idx = -1
			};
			ConduitTemperatureManager_Remove(handle.index);
		}
	}

	public void Clear()
	{
		ConduitTemperatureManager_Clear();
	}

	public unsafe void Sim200ms(float dt)
	{
		IntPtr value = ConduitTemperatureManager_Update(dt, (IntPtr)(void*)Game.Instance.simData.buildingTemperatures);
		ConduitTemperatureUpdateData* ptr = (ConduitTemperatureUpdateData*)(void*)value;
		int numEntries = ptr->numEntries;
		if (numEntries > 0)
		{
			Marshal.Copy((IntPtr)(void*)ptr->temperatures, temperatures, 0, numEntries);
		}
		for (int i = 0; i < ptr->numFrozenHandles; i++)
		{
			int h = ptr->frozenHandles[i];
			int handleIndex = Sim.GetHandleIndex(h);
			ConduitInfo conduitInfo = this.conduitInfo[handleIndex];
			ConduitFlow flowManager = Conduit.GetFlowManager(conduitInfo.type);
			flowManager.FreezeConduitContents(conduitInfo.idx);
		}
		for (int j = 0; j < ptr->numMeltedHandles; j++)
		{
			int h2 = ptr->meltedHandles[j];
			int handleIndex2 = Sim.GetHandleIndex(h2);
			ConduitInfo conduitInfo2 = this.conduitInfo[handleIndex2];
			ConduitFlow flowManager2 = Conduit.GetFlowManager(conduitInfo2.type);
			flowManager2.MeltConduitContents(conduitInfo2.idx);
		}
	}

	public float GetTemperature(HandleVector<int>.Handle handle)
	{
		return temperatures[Sim.GetHandleIndex(handle.index)];
	}

	[DllImport("SimDLL")]
	private static extern void ConduitTemperatureManager_Initialize();

	[DllImport("SimDLL")]
	private static extern void ConduitTemperatureManager_Shutdown();

	[DllImport("SimDLL")]
	private static extern int ConduitTemperatureManager_Add(float contents_temperature, float contents_mass, int contents_element_hash, int conduit_structure_temperature_handle, float conduit_heat_capacity, float conduit_thermal_conductivity, bool conduit_insulated);

	[DllImport("SimDLL")]
	private static extern int ConduitTemperatureManager_Set(int handle, float contents_temperature, float contents_mass, int contents_element_hash);

	[DllImport("SimDLL")]
	private static extern void ConduitTemperatureManager_Remove(int handle);

	[DllImport("SimDLL")]
	private static extern IntPtr ConduitTemperatureManager_Update(float dt, IntPtr building_conductivity_data);

	[DllImport("SimDLL")]
	private static extern void ConduitTemperatureManager_Clear();
}
