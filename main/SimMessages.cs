#define UNITY_ASSERTIONS
using Database;
using Klei.AI;
using Klei.AI.DiseaseGrowthRules;
using STRINGS;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public static class SimMessages
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct AddElementConsumerMessage
	{
		public int cellIdx;

		public int callbackIdx;

		public byte radius;

		public byte configuration;

		public byte elementIdx;

		private byte pad0;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct SetElementConsumerDataMessage
	{
		public int handle;

		public int cell;

		public float consumptionRate;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct RemoveElementConsumerMessage
	{
		public int handle;

		public int callbackIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct AddElementEmitterMessage
	{
		public float maxPressure;

		public int callbackIdx;

		public int onBlockedCB;

		public int onUnblockedCB;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct ModifyElementEmitterMessage
	{
		public int handle;

		public int cellIdx;

		public float emitInterval;

		public float emitMass;

		public float emitTemperature;

		public float maxPressure;

		public int diseaseCount;

		public byte elementIdx;

		public byte maxDepth;

		public byte diseaseIdx;

		private byte pad0;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct RemoveElementEmitterMessage
	{
		public int handle;

		public int callbackIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct AddElementChunkMessage
	{
		public int gameCell;

		public int callbackIdx;

		public float mass;

		public float temperature;

		public float surfaceArea;

		public float thickness;

		public float groundTransferScale;

		public byte elementIdx;

		public byte pad0;

		public byte pad1;

		public byte pad2;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct RemoveElementChunkMessage
	{
		public int handle;

		public int callbackIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct SetElementChunkDataMessage
	{
		public int handle;

		public float temperature;

		public float heatCapacity;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct MoveElementChunkMessage
	{
		public int handle;

		public int gameCell;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct ModifyElementChunkEnergyMessage
	{
		public int handle;

		public float deltaKJ;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct ModifyElementChunkAdjusterMessage
	{
		public int handle;

		public float temperature;

		public float heatCapacity;

		public float thermalConductivity;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct AddBuildingHeatExchangeMessage
	{
		public int callbackIdx;

		public byte elemIdx;

		public byte pad0;

		public byte pad1;

		public byte pad2;

		public float mass;

		public float temperature;

		public float thermalConductivity;

		public float overheatTemperature;

		public float operatingKilowatts;

		public int minX;

		public int minY;

		public int maxX;

		public int maxY;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ModifyBuildingHeatExchangeMessage
	{
		public int callbackIdx;

		public byte elemIdx;

		public byte pad0;

		public byte pad1;

		public byte pad2;

		public float mass;

		public float temperature;

		public float thermalConductivity;

		public float overheatTemperature;

		public float operatingKilowatts;

		public int minX;

		public int minY;

		public int maxX;

		public int maxY;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ModifyBuildingEnergyMessage
	{
		public int handle;

		public float deltaKJ;

		public float minTemperature;

		public float maxTemperature;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct RemoveBuildingHeatExchangeMessage
	{
		public int handle;

		public int callbackIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct AddDiseaseEmitterMessage
	{
		public int callbackIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ModifyDiseaseEmitterMessage
	{
		public int handle;

		public int gameCell;

		public byte diseaseIdx;

		public byte maxDepth;

		private byte pad0;

		private byte pad1;

		public float emitInterval;

		public int emitCount;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct RemoveDiseaseEmitterMessage
	{
		public int handle;

		public int callbackIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct SetSavedOptionsMessage
	{
		public byte clearBits;

		public byte setBits;
	}

	public enum SimSavedOptions : byte
	{
		ENABLE_DIAGONAL_FALLING_SAND = 1
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct DigMessage
	{
		public int cellIdx;

		public int callbackIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct SetCellFloatValueMessage
	{
		public int cellIdx;

		public float value;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct CellPropertiesMessage
	{
		public int cellIdx;

		public byte properties;

		public byte set;

		public byte pad0;

		public byte pad1;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct SetInsulationValueMessage
	{
		public int cellIdx;

		public float value;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct ModifyCellMessage
	{
		public int cellIdx;

		public int callbackIdx;

		public float temperature;

		public float mass;

		public int diseaseCount;

		public byte elementIdx;

		public byte replaceType;

		public byte diseaseIdx;

		public byte addSubType;
	}

	public enum ReplaceType
	{
		None,
		Replace,
		ReplaceAndDisplace
	}

	private enum AddSolidMassSubType
	{
		DoVerticalDisplacement,
		OnlyIfSameElement
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct CellDiseaseModification
	{
		public int cellIdx;

		public byte diseaseIdx;

		public byte pad0;

		public byte pad1;

		public byte pad2;

		public int diseaseCount;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct MassConsumptionMessage
	{
		public int cellIdx;

		public int callbackIdx;

		public float mass;

		public byte elementIdx;

		public byte radius;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct MassEmissionMessage
	{
		public int cellIdx;

		public int callbackIdx;

		public float mass;

		public float temperature;

		public int diseaseCount;

		public byte elementIdx;

		public byte diseaseIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct ConsumeDiseaseMessage
	{
		public int gameCell;

		public int callbackIdx;

		public float percentToConsume;

		public int maxToConsume;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct ModifyCellEnergyMessage
	{
		public int cellIdx;

		public float kilojoules;

		public float maxTemperature;

		public int id;
	}

	public enum EnergySourceID
	{
		DebugHeat = 1000,
		DebugCool,
		FierySkin,
		Overheatable,
		LiquidCooledFan,
		ConduitTemperatureManager,
		Excavator,
		HeatBulb,
		WarmBlooded,
		StructureTemperature,
		Burner
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct VisibleCells
	{
		public Vector2I min;

		public Vector2I max;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct WakeCellMessage
	{
		public int gameCell;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ElementInteraction
	{
		public uint interactionType;

		public byte elemIdx1;

		public byte elemIdx2;

		public byte elemResultIdx;

		public byte pad;

		public float minMass;

		public float interactionProbability;

		public float elem1MassDestructionPercent;

		public float elem2MassRequiredMultiplier;

		public float elemResultMassCreationMultiplier;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct CreateElementInteractionsMsg
	{
		public int numInteractions;

		public unsafe ElementInteraction* interactions;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct PipeChange
	{
		public int cell;

		public byte layer;

		public byte pad0;

		public byte pad1;

		public byte pad2;

		public float mass;

		public float temperature;

		public int elementHash;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct CellWorldZoneModification
	{
		public int cell;

		public byte zoneID;
	}

	public const int InvalidCallback = -1;

	public const float STATE_TRANSITION_TEMPERATURE_BUFER = 3f;

	public unsafe static void AddElementConsumer(int gameCell, ElementConsumer.Configuration configuration, SimHashes element, byte radius, int cb_handle)
	{
		Debug.Assert(Grid.IsValidCell(gameCell));
		if (Grid.IsValidCell(gameCell))
		{
			int elementIndex = ElementLoader.GetElementIndex(element);
			AddElementConsumerMessage* ptr = stackalloc AddElementConsumerMessage[1];
			ptr->cellIdx = gameCell;
			ptr->configuration = (byte)configuration;
			ptr->elementIdx = (byte)elementIndex;
			ptr->radius = radius;
			ptr->callbackIdx = cb_handle;
			Sim.SIM_HandleMessage(2024405073, sizeof(AddElementConsumerMessage), (byte*)ptr);
		}
	}

	public unsafe static void SetElementConsumerData(int sim_handle, int cell, float consumptionRate)
	{
		if (Sim.IsValidHandle(sim_handle))
		{
			SetElementConsumerDataMessage* ptr = stackalloc SetElementConsumerDataMessage[1];
			ptr->handle = sim_handle;
			ptr->cell = cell;
			ptr->consumptionRate = consumptionRate;
			Sim.SIM_HandleMessage(1575539738, sizeof(SetElementConsumerDataMessage), (byte*)ptr);
		}
	}

	public unsafe static void RemoveElementConsumer(int cb_handle, int sim_handle)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
			Debug.Assert(false, "Invalid handle");
		}
		else
		{
			RemoveElementConsumerMessage* ptr = stackalloc RemoveElementConsumerMessage[1];
			ptr->callbackIdx = cb_handle;
			ptr->handle = sim_handle;
			Sim.SIM_HandleMessage(894417742, sizeof(RemoveElementConsumerMessage), (byte*)ptr);
		}
	}

	public unsafe static void AddElementEmitter(float max_pressure, int on_registered, int on_blocked = -1, int on_unblocked = -1)
	{
		AddElementEmitterMessage* ptr = stackalloc AddElementEmitterMessage[1];
		ptr->maxPressure = max_pressure;
		ptr->callbackIdx = on_registered;
		ptr->onBlockedCB = on_blocked;
		ptr->onUnblockedCB = on_unblocked;
		Sim.SIM_HandleMessage(-505471181, sizeof(AddElementEmitterMessage), (byte*)ptr);
	}

	public unsafe static void ModifyElementEmitter(int sim_handle, int game_cell, int max_depth, SimHashes element, float emit_interval, float emit_mass, float emit_temperature, float max_pressure, byte disease_idx, int disease_count)
	{
		Debug.Assert(Grid.IsValidCell(game_cell));
		if (Grid.IsValidCell(game_cell))
		{
			int elementIndex = ElementLoader.GetElementIndex(element);
			ModifyElementEmitterMessage* ptr = stackalloc ModifyElementEmitterMessage[1];
			ptr->handle = sim_handle;
			ptr->cellIdx = game_cell;
			ptr->emitInterval = emit_interval;
			ptr->emitMass = emit_mass;
			ptr->emitTemperature = emit_temperature;
			ptr->maxPressure = max_pressure;
			ptr->elementIdx = (byte)elementIndex;
			ptr->maxDepth = (byte)max_depth;
			ptr->diseaseIdx = disease_idx;
			ptr->diseaseCount = disease_count;
			Sim.SIM_HandleMessage(403589164, sizeof(ModifyElementEmitterMessage), (byte*)ptr);
		}
	}

	public unsafe static void RemoveElementEmitter(int cb_handle, int sim_handle)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
			Debug.Assert(false, "Invalid handle");
		}
		else
		{
			RemoveElementEmitterMessage* ptr = stackalloc RemoveElementEmitterMessage[1];
			ptr->callbackIdx = cb_handle;
			ptr->handle = sim_handle;
			Sim.SIM_HandleMessage(-1524118282, sizeof(RemoveElementEmitterMessage), (byte*)ptr);
		}
	}

	public unsafe static void AddElementChunk(int gameCell, SimHashes element, float mass, float temperature, float surface_area, float thickness, float ground_transfer_scale, int cb_handle)
	{
		Debug.Assert(Grid.IsValidCell(gameCell));
		if (Grid.IsValidCell(gameCell))
		{
			UnityEngine.Debug.Assert(mass * temperature > 0f, "Tried to add an SimTemperatureTransfer component with 0 mass or temperature. Unsupported! Your game is now in a bad state.");
			if (mass * temperature > 0f)
			{
				int elementIndex = ElementLoader.GetElementIndex(element);
				AddElementChunkMessage* ptr = stackalloc AddElementChunkMessage[1];
				ptr->gameCell = gameCell;
				ptr->callbackIdx = cb_handle;
				ptr->mass = mass;
				ptr->temperature = temperature;
				ptr->surfaceArea = surface_area;
				ptr->thickness = thickness;
				ptr->groundTransferScale = ground_transfer_scale;
				ptr->elementIdx = (byte)elementIndex;
				Sim.SIM_HandleMessage(1445724082, sizeof(AddElementChunkMessage), (byte*)ptr);
			}
		}
	}

	public unsafe static void RemoveElementChunk(int sim_handle, int cb_handle)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
			Debug.Assert(false, "Invalid handle");
		}
		else
		{
			RemoveElementChunkMessage* ptr = stackalloc RemoveElementChunkMessage[1];
			ptr->callbackIdx = cb_handle;
			ptr->handle = sim_handle;
			Sim.SIM_HandleMessage(-912908555, sizeof(RemoveElementChunkMessage), (byte*)ptr);
		}
	}

	public unsafe static void SetElementChunkData(int sim_handle, float temperature, float heat_capacity)
	{
		if (Sim.IsValidHandle(sim_handle))
		{
			SetElementChunkDataMessage* ptr = stackalloc SetElementChunkDataMessage[1];
			ptr->handle = sim_handle;
			ptr->temperature = temperature;
			ptr->heatCapacity = heat_capacity;
			Sim.SIM_HandleMessage(-435115907, sizeof(SetElementChunkDataMessage), (byte*)ptr);
		}
	}

	public unsafe static void MoveElementChunk(int sim_handle, int cell)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
			Debug.Assert(false, "Invalid handle");
		}
		else
		{
			MoveElementChunkMessage* ptr = stackalloc MoveElementChunkMessage[1];
			ptr->handle = sim_handle;
			ptr->gameCell = cell;
			Sim.SIM_HandleMessage(-374911358, sizeof(MoveElementChunkMessage), (byte*)ptr);
		}
	}

	public unsafe static void ModifyElementChunkEnergy(int sim_handle, float delta_kj)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
			Debug.Assert(false, "Invalid handle");
		}
		else
		{
			ModifyElementChunkEnergyMessage* ptr = stackalloc ModifyElementChunkEnergyMessage[1];
			ptr->handle = sim_handle;
			ptr->deltaKJ = delta_kj;
			Sim.SIM_HandleMessage(1020555667, sizeof(ModifyElementChunkEnergyMessage), (byte*)ptr);
		}
	}

	public unsafe static void ModifyElementChunkTemperatureAdjuster(int sim_handle, float temperature, float heat_capacity, float thermal_conductivity)
	{
		if (!Sim.IsValidHandle(sim_handle))
		{
			Debug.Assert(false, "Invalid handle");
		}
		else
		{
			ModifyElementChunkAdjusterMessage* ptr = stackalloc ModifyElementChunkAdjusterMessage[1];
			ptr->handle = sim_handle;
			ptr->temperature = temperature;
			ptr->heatCapacity = heat_capacity;
			ptr->thermalConductivity = thermal_conductivity;
			Sim.SIM_HandleMessage(-1387601379, sizeof(ModifyElementChunkAdjusterMessage), (byte*)ptr);
		}
	}

	public unsafe static void AddBuildingHeatExchange(Extents extents, float mass, float temperature, float thermal_conductivity, float operating_kw, byte elem_idx, int callbackIdx = -1)
	{
		int cell = Grid.XYToCell(extents.x, extents.y);
		Debug.Assert(Grid.IsValidCell(cell));
		if (Grid.IsValidCell(cell))
		{
			int num = Grid.XYToCell(extents.x + extents.width, extents.y + extents.height);
			if (!Grid.IsValidCell(num))
			{
				Debug.LogErrorFormat("Invalid Cell [{0}] Extents [{1},{2}] [{3},{4}]", num, extents.x, extents.y, extents.width, extents.height);
			}
			if (Grid.IsValidCell(num))
			{
				AddBuildingHeatExchangeMessage* ptr = stackalloc AddBuildingHeatExchangeMessage[1];
				ptr->callbackIdx = callbackIdx;
				ptr->elemIdx = elem_idx;
				ptr->mass = mass;
				ptr->temperature = temperature;
				ptr->thermalConductivity = thermal_conductivity;
				ptr->overheatTemperature = 3.40282347E+38f;
				ptr->operatingKilowatts = operating_kw;
				ptr->minX = extents.x;
				ptr->minY = extents.y;
				ptr->maxX = extents.x + extents.width;
				ptr->maxY = extents.y + extents.height;
				Sim.SIM_HandleMessage(1739021608, sizeof(AddBuildingHeatExchangeMessage), (byte*)ptr);
			}
		}
	}

	public unsafe static void ModifyBuildingHeatExchange(int sim_handle, Extents extents, float mass, float temperature, float thermal_conductivity, float overheat_temperature, float operating_kw, byte element_idx)
	{
		int cell = Grid.XYToCell(extents.x, extents.y);
		Debug.Assert(Grid.IsValidCell(cell));
		if (Grid.IsValidCell(cell))
		{
			int cell2 = Grid.XYToCell(extents.x + extents.width, extents.y + extents.height);
			Debug.Assert(Grid.IsValidCell(cell2));
			if (Grid.IsValidCell(cell2))
			{
				ModifyBuildingHeatExchangeMessage* ptr = stackalloc ModifyBuildingHeatExchangeMessage[1];
				ptr->callbackIdx = sim_handle;
				ptr->elemIdx = element_idx;
				ptr->mass = mass;
				ptr->temperature = temperature;
				ptr->thermalConductivity = thermal_conductivity;
				ptr->overheatTemperature = overheat_temperature;
				ptr->operatingKilowatts = operating_kw;
				ptr->minX = extents.x;
				ptr->minY = extents.y;
				ptr->maxX = extents.x + extents.width;
				ptr->maxY = extents.y + extents.height;
				Sim.SIM_HandleMessage(1818001569, sizeof(ModifyBuildingHeatExchangeMessage), (byte*)ptr);
			}
		}
	}

	public unsafe static void RemoveBuildingHeatExchange(int sim_handle, int callbackIdx = -1)
	{
		RemoveBuildingHeatExchangeMessage* ptr = stackalloc RemoveBuildingHeatExchangeMessage[1];
		Debug.Assert(Sim.IsValidHandle(sim_handle));
		ptr->handle = sim_handle;
		ptr->callbackIdx = callbackIdx;
		Sim.SIM_HandleMessage(-456116629, sizeof(RemoveBuildingHeatExchangeMessage), (byte*)ptr);
	}

	public unsafe static void ModifyBuildingEnergy(int sim_handle, float delta_kj, float min_temperature, float max_temperature)
	{
		ModifyBuildingEnergyMessage* ptr = stackalloc ModifyBuildingEnergyMessage[1];
		Debug.Assert(Sim.IsValidHandle(sim_handle));
		ptr->handle = sim_handle;
		ptr->deltaKJ = delta_kj;
		ptr->minTemperature = min_temperature;
		ptr->maxTemperature = max_temperature;
		Sim.SIM_HandleMessage(-1348791658, sizeof(ModifyBuildingEnergyMessage), (byte*)ptr);
	}

	public unsafe static void AddDiseaseEmitter(int callbackIdx)
	{
		AddDiseaseEmitterMessage* ptr = stackalloc AddDiseaseEmitterMessage[1];
		ptr->callbackIdx = callbackIdx;
		Sim.SIM_HandleMessage(1486783027, sizeof(AddDiseaseEmitterMessage), (byte*)ptr);
	}

	public unsafe static void ModifyDiseaseEmitter(int sim_handle, int cell, byte range, byte disease_idx, float emit_interval, int emit_count)
	{
		Debug.Assert(Sim.IsValidHandle(sim_handle));
		ModifyDiseaseEmitterMessage* ptr = stackalloc ModifyDiseaseEmitterMessage[1];
		ptr->handle = sim_handle;
		ptr->gameCell = cell;
		ptr->maxDepth = range;
		ptr->diseaseIdx = disease_idx;
		ptr->emitInterval = emit_interval;
		ptr->emitCount = emit_count;
		Sim.SIM_HandleMessage(-1899123924, sizeof(ModifyDiseaseEmitterMessage), (byte*)ptr);
	}

	public unsafe static void RemoveDiseaseEmitter(int cb_handle, int sim_handle)
	{
		Debug.Assert(Sim.IsValidHandle(sim_handle));
		RemoveDiseaseEmitterMessage* ptr = stackalloc RemoveDiseaseEmitterMessage[1];
		ptr->handle = sim_handle;
		ptr->callbackIdx = cb_handle;
		Sim.SIM_HandleMessage(468135926, sizeof(RemoveDiseaseEmitterMessage), (byte*)ptr);
	}

	public unsafe static void SetSavedOptionValue(SimSavedOptions option, int zero_or_one)
	{
		SetSavedOptionsMessage* ptr = stackalloc SetSavedOptionsMessage[1];
		if (zero_or_one == 0)
		{
			ptr->clearBits |= (byte)option;
			ptr->setBits = 0;
		}
		else
		{
			ptr->clearBits = 0;
			ptr->setBits |= (byte)option;
		}
		Sim.SIM_HandleMessage(1154135737, sizeof(SetSavedOptionsMessage), (byte*)ptr);
	}

	private static void WriteKleiString(this BinaryWriter writer, string str)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(str);
		writer.Write(bytes.Length);
		if (bytes.Length > 0)
		{
			writer.Write(bytes);
		}
	}

	public unsafe static void CreateSimElementsTable(List<Element> elements)
	{
		//IL_00c7: Incompatible stack types: I vs Ref
		MemoryStream memoryStream = new MemoryStream(Marshal.SizeOf(typeof(int)) + Marshal.SizeOf(typeof(Sim.Element)) * elements.Count);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(elements.Count);
		for (int i = 0; i < elements.Count; i++)
		{
			new Sim.Element(elements[i], elements).Write(binaryWriter);
		}
		for (int j = 0; j < elements.Count; j++)
		{
			binaryWriter.WriteKleiString(UI.StripLinkFormatting(elements[j].name));
		}
		byte[] buffer = memoryStream.GetBuffer();
		fixed (byte* msg = &((buffer != null && buffer.Length != 0) ? ref buffer[0] : ref *(byte*)null))
		{
			Sim.SIM_HandleMessage(1108437482, buffer.Length, msg);
		}
	}

	public unsafe static void CreateWorldGenHACKDiseaseTable(List<string> diseaseIds)
	{
		//IL_0154: Incompatible stack types: I vs Ref
		MemoryStream memoryStream = new MemoryStream(1024);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(diseaseIds.Count);
		List<Element> elements = ElementLoader.elements;
		binaryWriter.Write(elements.Count);
		Disease.RangeInfo rangeInfo = default(Disease.RangeInfo);
		rangeInfo.maxGrowth = 350f;
		rangeInfo.minGrowth = 250f;
		rangeInfo.minViable = 200f;
		rangeInfo.maxViable = 400f;
		Disease.RangeInfo rangeInfo2 = default(Disease.RangeInfo);
		rangeInfo2.maxGrowth = float.PositiveInfinity;
		rangeInfo2.minGrowth = float.PositiveInfinity;
		rangeInfo2.minViable = float.PositiveInfinity;
		rangeInfo2.maxViable = float.PositiveInfinity;
		for (int i = 0; i < diseaseIds.Count; i++)
		{
			binaryWriter.WriteKleiString("");
			binaryWriter.Write(new HashedString(diseaseIds[i]).GetHashCode());
			binaryWriter.Write(0f);
			rangeInfo.Write(binaryWriter);
			rangeInfo2.Write(binaryWriter);
			rangeInfo.Write(binaryWriter);
			rangeInfo2.Write(binaryWriter);
			for (int j = 0; j < elements.Count; j++)
			{
				Disease.DEFAULT_GROWTH_INFO.Write(binaryWriter);
			}
		}
		byte[] buffer = memoryStream.GetBuffer();
		fixed (byte* msg = &((buffer != null && buffer.Length != 0) ? ref buffer[0] : ref *(byte*)null))
		{
			Sim.SIM_HandleMessage(825301935, (int)memoryStream.Length, msg);
		}
	}

	public unsafe static void CreateDiseaseTable()
	{
		//IL_012a: Incompatible stack types: I vs Ref
		Diseases diseases = Db.Get().Diseases;
		MemoryStream memoryStream = new MemoryStream(1024);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(diseases.Count);
		List<Element> elements = ElementLoader.elements;
		binaryWriter.Write(elements.Count);
		for (int i = 0; i < diseases.Count; i++)
		{
			Disease disease = diseases[i];
			binaryWriter.WriteKleiString(UI.StripLinkFormatting(disease.Name));
			binaryWriter.Write(disease.id.GetHashCode());
			binaryWriter.Write(disease.strength);
			disease.temperatureRange.Write(binaryWriter);
			disease.temperatureHalfLives.Write(binaryWriter);
			disease.pressureRange.Write(binaryWriter);
			disease.pressureHalfLives.Write(binaryWriter);
			for (int j = 0; j < elements.Count; j++)
			{
				ElemGrowthInfo elemGrowthInfo = disease.elemGrowthInfo[j];
				elemGrowthInfo.Write(binaryWriter);
			}
		}
		byte[] buffer = memoryStream.GetBuffer();
		fixed (byte* msg = &((buffer != null && buffer.Length != 0) ? ref buffer[0] : ref *(byte*)null))
		{
			Sim.SIM_HandleMessage(825301935, (int)memoryStream.Length, msg);
		}
	}

	public static void SimDataInitializeFromCells(int width, int height, Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dc)
	{
		MemoryStream memoryStream = new MemoryStream(Marshal.SizeOf(typeof(int)) + Marshal.SizeOf(typeof(int)) + Marshal.SizeOf(typeof(Sim.Cell)) * width * height + Marshal.SizeOf(typeof(float)) * width * height + Marshal.SizeOf(typeof(Sim.DiseaseCell)) * width * height);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(width);
		binaryWriter.Write(height);
		int num = width * height;
		for (int i = 0; i < num; i++)
		{
			cells[i].Write(binaryWriter);
		}
		for (int j = 0; j < num; j++)
		{
			binaryWriter.Write(bgTemp[j]);
		}
		for (int k = 0; k < num; k++)
		{
			dc[k].Write(binaryWriter);
		}
		byte[] buffer = memoryStream.GetBuffer();
		Sim.HandleMessage(SimMessageHashes.SimData_InitializeFromCells, buffer.Length, buffer);
	}

	public unsafe static void Dig(int gameCell, int callbackIdx = -1)
	{
		if (!Grid.IsValidCell(gameCell))
		{
			UnityEngine.Debug.AssertFormat(false, "Invalid cell: {0}", gameCell);
		}
		else
		{
			DigMessage* ptr = stackalloc DigMessage[1];
			ptr->cellIdx = gameCell;
			ptr->callbackIdx = callbackIdx;
			Sim.SIM_HandleMessage(833038498, sizeof(DigMessage), (byte*)ptr);
		}
	}

	public unsafe static void SetInsulation(int gameCell, float value)
	{
		if (!Grid.IsValidCell(gameCell))
		{
			UnityEngine.Debug.AssertFormat(false, "Invalid cell: {0}", gameCell);
		}
		else
		{
			SetCellFloatValueMessage* ptr = stackalloc SetCellFloatValueMessage[1];
			ptr->cellIdx = gameCell;
			ptr->value = value;
			Sim.SIM_HandleMessage(-898773121, sizeof(SetCellFloatValueMessage), (byte*)ptr);
		}
	}

	public unsafe static void SetStrength(int gameCell, int weight, float strengthMultiplier)
	{
		if (!Grid.IsValidCell(gameCell))
		{
			UnityEngine.Debug.AssertFormat(false, "Invalid cell: {0}", gameCell);
		}
		else
		{
			SetCellFloatValueMessage* ptr = stackalloc SetCellFloatValueMessage[1];
			ptr->cellIdx = gameCell;
			int num = (int)(strengthMultiplier * 4f) & 0x7F;
			int num2 = ((weight & 1) << 7) | num;
			ptr->value = (float)(int)(byte)num2;
			Sim.SIM_HandleMessage(1593243982, sizeof(SetCellFloatValueMessage), (byte*)ptr);
		}
	}

	public unsafe static void SetCellProperties(int gameCell, byte properties)
	{
		if (!Grid.IsValidCell(gameCell))
		{
			UnityEngine.Debug.AssertFormat(false, "Invalid cell: {0}", gameCell);
		}
		else
		{
			CellPropertiesMessage* ptr = stackalloc CellPropertiesMessage[1];
			ptr->cellIdx = gameCell;
			ptr->properties = properties;
			ptr->set = 1;
			Sim.SIM_HandleMessage(-469311643, sizeof(CellPropertiesMessage), (byte*)ptr);
		}
	}

	public unsafe static void ClearCellProperties(int gameCell, byte properties)
	{
		if (!Grid.IsValidCell(gameCell))
		{
			UnityEngine.Debug.AssertFormat(false, "Invalid cell: {0}", gameCell);
		}
		else
		{
			CellPropertiesMessage* ptr = stackalloc CellPropertiesMessage[1];
			ptr->cellIdx = gameCell;
			ptr->properties = properties;
			ptr->set = 0;
			Sim.SIM_HandleMessage(-469311643, sizeof(CellPropertiesMessage), (byte*)ptr);
		}
	}

	public unsafe static void ModifyCell(int gameCell, int elementIdx, float temperature, float mass, byte disease_idx, int disease_count, ReplaceType replace_type = ReplaceType.None, bool do_vertical_solid_displacement = false, int callbackIdx = -1)
	{
		if (!Grid.IsValidCell(gameCell))
		{
			UnityEngine.Debug.AssertFormat(false, "Invalid cell: {0}", gameCell);
		}
		else
		{
			Element element = ElementLoader.elements[elementIdx];
			if (mass > element.maxMass)
			{
				Debug.LogWarningFormat("Invalid cell modification (mass greater than element maximum): Cell={0}, EIdx={1}, T={2}, M={3}, {4} max mass = {5}", gameCell, elementIdx, temperature, mass, element.id, element.maxMass);
				mass = element.maxMass;
			}
			if (temperature < 0f || temperature > 10000f)
			{
				Debug.LogWarningFormat("Invalid cell modification (temp out of bounds): Cell={0}, EIdx={1}, T={2}, M={3}, {4} default temp = {5}", gameCell, elementIdx, temperature, mass, element.id, element.defaultValues.temperature);
				temperature = element.defaultValues.temperature;
			}
			if (temperature == 0f && mass > 0f)
			{
				Debug.LogWarningFormat("Invalid cell modification (zero temp with non-zero mass): Cell={0}, EIdx={1}, T={2}, M={3}, {4} default temp = {5}", gameCell, elementIdx, temperature, mass, element.id, element.defaultValues.temperature);
				temperature = element.defaultValues.temperature;
			}
			ModifyCellMessage* ptr = stackalloc ModifyCellMessage[1];
			ptr->cellIdx = gameCell;
			ptr->callbackIdx = callbackIdx;
			ptr->temperature = temperature;
			ptr->mass = mass;
			ptr->elementIdx = (byte)elementIdx;
			ptr->replaceType = (byte)replace_type;
			ptr->diseaseIdx = disease_idx;
			ptr->diseaseCount = disease_count;
			ptr->addSubType = (byte)((!do_vertical_solid_displacement) ? 1 : 0);
			Sim.SIM_HandleMessage(-1252920804, sizeof(ModifyCellMessage), (byte*)ptr);
		}
	}

	public unsafe static void ModifyDiseaseOnCell(int gameCell, byte disease_idx, int disease_delta)
	{
		CellDiseaseModification* ptr = stackalloc CellDiseaseModification[1];
		ptr->cellIdx = gameCell;
		ptr->diseaseIdx = disease_idx;
		ptr->diseaseCount = disease_delta;
		Sim.SIM_HandleMessage(-1853671274, sizeof(CellDiseaseModification), (byte*)ptr);
	}

	public static int GetElementIndex(SimHashes element)
	{
		int result = -1;
		List<Element> elements = ElementLoader.elements;
		for (int i = 0; i < elements.Count; i++)
		{
			if (elements[i].id == element)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	public unsafe static void ConsumeMass(int gameCell, SimHashes element, float mass, byte radius, int callbackIdx = -1)
	{
		if (!Grid.IsValidCell(gameCell))
		{
			UnityEngine.Debug.AssertFormat(false, "Invalid cell: {0}", gameCell);
		}
		else
		{
			int elementIndex = ElementLoader.GetElementIndex(element);
			MassConsumptionMessage* ptr = stackalloc MassConsumptionMessage[1];
			ptr->cellIdx = gameCell;
			ptr->callbackIdx = callbackIdx;
			ptr->mass = mass;
			ptr->elementIdx = (byte)elementIndex;
			ptr->radius = radius;
			Sim.SIM_HandleMessage(1727657959, sizeof(MassConsumptionMessage), (byte*)ptr);
		}
	}

	public unsafe static void EmitMass(int gameCell, byte element_idx, float mass, float temperature, byte disease_idx, int disease_count, int callbackIdx = -1)
	{
		if (!Grid.IsValidCell(gameCell))
		{
			UnityEngine.Debug.AssertFormat(false, "Invalid cell: {0}", gameCell);
		}
		else
		{
			MassEmissionMessage* ptr = stackalloc MassEmissionMessage[1];
			ptr->cellIdx = gameCell;
			ptr->callbackIdx = callbackIdx;
			ptr->mass = mass;
			ptr->temperature = temperature;
			ptr->elementIdx = element_idx;
			ptr->diseaseIdx = disease_idx;
			ptr->diseaseCount = disease_count;
			Sim.SIM_HandleMessage(797274363, sizeof(MassEmissionMessage), (byte*)ptr);
		}
	}

	public unsafe static void ConsumeDisease(int game_cell, float percent_to_consume, int max_to_consume, int callback_idx)
	{
		if (Grid.IsValidCell(game_cell))
		{
			ConsumeDiseaseMessage* ptr = stackalloc ConsumeDiseaseMessage[1];
			ptr->callbackIdx = callback_idx;
			ptr->gameCell = game_cell;
			ptr->percentToConsume = percent_to_consume;
			ptr->maxToConsume = max_to_consume;
			Sim.SIM_HandleMessage(-1019841536, sizeof(ConsumeDiseaseMessage), (byte*)ptr);
		}
	}

	public static void AddRemoveSubstance(int gameCell, SimHashes new_element, CellAddRemoveSubstanceEvent ev, float mass, float temperature, byte disease_idx, int disease_count, bool do_vertical_solid_displacement = true, int callbackIdx = -1)
	{
		int elementIndex = GetElementIndex(new_element);
		AddRemoveSubstance(gameCell, elementIndex, ev, mass, temperature, disease_idx, disease_count, do_vertical_solid_displacement, callbackIdx);
	}

	public static void AddRemoveSubstance(int gameCell, int elementIdx, CellAddRemoveSubstanceEvent ev, float mass, float temperature, byte disease_idx, int disease_count, bool do_vertical_solid_displacement = true, int callbackIdx = -1)
	{
		if (elementIdx != -1)
		{
			Element element = ElementLoader.elements[elementIdx];
			float temperature2 = (temperature == -1f) ? element.defaultValues.temperature : temperature;
			ModifyCell(gameCell, elementIdx, temperature2, mass, disease_idx, disease_count, ReplaceType.None, do_vertical_solid_displacement, callbackIdx);
			if (ev == null)
			{
				return;
			}
		}
	}

	public static void ReplaceElement(int gameCell, SimHashes new_element, CellElementEvent ev, float mass, float temperature = -1f, byte diseaseIdx = byte.MaxValue, int diseaseCount = 0, int callbackIdx = -1)
	{
		int elementIndex = GetElementIndex(new_element);
		if (elementIndex != -1)
		{
			Element element = ElementLoader.elements[elementIndex];
			float num = (temperature == -1f) ? element.defaultValues.temperature : temperature;
			int elementIdx = elementIndex;
			float temperature2 = num;
			byte disease_idx = diseaseIdx;
			ReplaceType replace_type = ReplaceType.Replace;
			ModifyCell(gameCell, elementIdx, temperature2, mass, disease_idx, diseaseCount, replace_type, false, callbackIdx);
		}
	}

	public static void ReplaceAndDisplaceElement(int gameCell, SimHashes new_element, CellElementEvent ev, float mass, float temperature = -1f, byte disease_idx = byte.MaxValue, int disease_count = 0, int callbackIdx = -1)
	{
		int elementIndex = GetElementIndex(new_element);
		if (elementIndex != -1)
		{
			Element element = ElementLoader.elements[elementIndex];
			float num = (temperature == -1f) ? element.defaultValues.temperature : temperature;
			int elementIdx = elementIndex;
			float temperature2 = num;
			byte disease_idx2 = disease_idx;
			ReplaceType replace_type = ReplaceType.ReplaceAndDisplace;
			ModifyCell(gameCell, elementIdx, temperature2, mass, disease_idx2, disease_count, replace_type, false, callbackIdx);
		}
	}

	public unsafe static void ModifyEnergy(int gameCell, float kilojoules, float max_temperature, EnergySourceID id)
	{
		if (!Grid.IsValidCell(gameCell))
		{
			UnityEngine.Debug.AssertFormat(false, "Invalid cell: {0}", gameCell);
		}
		else if (max_temperature <= 0f)
		{
			Debug.LogError("invalid max temperature for cell energy modification");
		}
		else
		{
			ModifyCellEnergyMessage* ptr = stackalloc ModifyCellEnergyMessage[1];
			ptr->cellIdx = gameCell;
			ptr->kilojoules = kilojoules;
			ptr->maxTemperature = max_temperature;
			ptr->id = (int)id;
			Sim.SIM_HandleMessage(818320644, sizeof(ModifyCellEnergyMessage), (byte*)ptr);
		}
	}

	public static void ModifyMass(int gameCell, float mass, byte disease_idx, int disease_count, CellModifyMassEvent ev, float temperature = -1f, SimHashes element = SimHashes.Vacuum)
	{
		if (element != SimHashes.Vacuum)
		{
			int elementIndex = GetElementIndex(element);
			if (elementIndex != -1)
			{
				if (temperature == -1f)
				{
					temperature = ElementLoader.elements[elementIndex].defaultValues.temperature;
				}
				ModifyCell(gameCell, elementIndex, temperature, mass, disease_idx, disease_count, ReplaceType.None, false, -1);
			}
		}
		else
		{
			UnityEngine.Debug.Assert(mass < 0f, "Mass less than zero");
			ModifyCell(gameCell, 0, temperature, mass, disease_idx, disease_count, ReplaceType.None, false, -1);
		}
	}

	public unsafe static void CreateElementInteractions(ElementInteraction[] interactions)
	{
		//IL_0018: Incompatible stack types: I vs Ref
		fixed (ElementInteraction* interactions2 = &((interactions != null && interactions.Length != 0) ? ref interactions[0] : ref *(ElementInteraction*)null))
		{
			CreateElementInteractionsMsg* ptr = stackalloc CreateElementInteractionsMsg[1];
			ptr->numInteractions = interactions.Length;
			ptr->interactions = interactions2;
			Sim.SIM_HandleMessage(-930289787, sizeof(CreateElementInteractionsMsg), (byte*)ptr);
		}
	}

	public unsafe static void NewGameFrame(float elapsed_seconds, Vector2I min, Vector2I max)
	{
		min = new Vector2I(MathUtil.Clamp(0, Grid.WidthInCells - 1, (min.x / 32 - 1) * 32), MathUtil.Clamp(0, Grid.HeightInCells - 1, (min.y / 32 - 1) * 32));
		max = new Vector2I(MathUtil.Clamp(0, Grid.WidthInCells - 1, ((max.x + 31) / 32 + 1) * 32), MathUtil.Clamp(0, Grid.HeightInCells - 1, ((max.y + 31) / 32 + 1) * 32));
		Sim.NewGameFrame* ptr = stackalloc Sim.NewGameFrame[1];
		ptr->elapsedSeconds = elapsed_seconds;
		ptr->minX = min.x;
		ptr->minY = min.y;
		ptr->maxX = max.x;
		ptr->maxY = max.y;
		Sim.SIM_HandleMessage(-775326397, sizeof(Sim.NewGameFrame), (byte*)ptr);
	}

	public unsafe static void SetDebugProperties(Sim.DebugProperties properties)
	{
		Sim.DebugProperties* ptr = stackalloc Sim.DebugProperties[1];
		*ptr = properties;
		ptr->buildingTemperatureScale = properties.buildingTemperatureScale;
		Sim.SIM_HandleMessage(-1683118492, sizeof(Sim.DebugProperties), (byte*)ptr);
	}

	public unsafe static void ModifyCellWorldZone(int cell, byte zone_id)
	{
		CellWorldZoneModification* ptr = stackalloc CellWorldZoneModification[1];
		ptr->cell = cell;
		ptr->zoneID = zone_id;
		Sim.SIM_HandleMessage(-449718014, sizeof(CellWorldZoneModification), (byte*)ptr);
	}
}
