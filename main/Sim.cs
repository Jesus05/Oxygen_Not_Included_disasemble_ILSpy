using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public static class Sim
{
	public delegate int GAME_MessageHandler(int message_id, IntPtr data);

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct DLLExceptionHandlerMessage
	{
		public IntPtr callstack;

		public IntPtr dmpFilename;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct DLLReportMessageMessage
	{
		public IntPtr callstack;

		public IntPtr message;

		public IntPtr file;

		public int line;
	}

	private enum GameHandledMessages
	{
		ExceptionHandler,
		ReportMessage
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct PhysicsData
	{
		public float temperature;

		public float mass;

		public float pressure;

		public void Write(BinaryWriter writer)
		{
			writer.Write(temperature);
			writer.Write(mass);
			writer.Write(pressure);
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Cell
	{
		public enum Properties
		{
			GasImpermeable = 1,
			LiquidImpermeable = 2,
			SolidImpermeable = 4,
			Unbreakable = 8,
			Transparent = 0x10,
			Opaque = 0x20,
			NotifyOnMelt = 0x40
		}

		public byte elementIdx;

		public byte properties;

		public byte insulation;

		public byte strengthInfo;

		public float temperature;

		public float mass;

		public void Write(BinaryWriter writer)
		{
			writer.Write(elementIdx);
			writer.Write((byte)0);
			writer.Write(insulation);
			writer.Write((byte)0);
			writer.Write(temperature);
			writer.Write(mass);
		}

		public void SetValues(global::Element elem, List<global::Element> elements)
		{
			SetValues(elem, elem.defaultValues, elements);
		}

		public void SetValues(global::Element elem, PhysicsData pd, List<global::Element> elements)
		{
			elementIdx = (byte)elements.IndexOf(elem);
			temperature = pd.temperature;
			mass = pd.mass;
			insulation = byte.MaxValue;
		}

		public void SetValues(byte new_elem_idx, float new_temperature, float new_mass)
		{
			elementIdx = new_elem_idx;
			temperature = new_temperature;
			mass = new_mass;
			insulation = byte.MaxValue;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct Element
	{
		public SimHashes id;

		public byte state;

		public byte lowTempTransitionIdx;

		public byte highTempTransitionIdx;

		public byte elementsTableIdx;

		public float specificHeatCapacity;

		public float thermalConductivity;

		public float molarMass;

		public float solidSurfaceAreaMultiplier;

		public float liquidSurfaceAreaMultiplier;

		public float gasSurfaceAreaMultiplier;

		public float flow;

		public float viscosity;

		public float minHorizontalFlow;

		public float minVerticalFlow;

		public float maxMass;

		public float lowTemp;

		public float highTemp;

		public float strength;

		public SimHashes lowTempTransitionOreID;

		public float lowTempTransitionOreMassConversion;

		public SimHashes highTempTransitionOreID;

		public float highTempTransitionOreMassConversion;

		public sbyte sublimateIndex;

		public sbyte convertIndex;

		public byte pack0;

		public byte pack1;

		public uint colour;

		public SpawnFXHashes sublimateFX;

		public float lightAbsorptionFactor;

		public PhysicsData defaultValues;

		public Element(global::Element e, List<global::Element> elements)
		{
			id = e.id;
			state = (byte)e.state;
			if (e.HasTag(GameTags.Unstable))
			{
				state |= 8;
			}
			int num = elements.FindIndex((global::Element ele) => ele.id == e.lowTempTransitionTarget);
			int num2 = elements.FindIndex((global::Element ele) => ele.id == e.highTempTransitionTarget);
			lowTempTransitionIdx = (byte)((num < 0) ? 255 : num);
			highTempTransitionIdx = (byte)((num2 < 0) ? 255 : num2);
			elementsTableIdx = (byte)elements.IndexOf(e);
			specificHeatCapacity = e.specificHeatCapacity;
			thermalConductivity = e.thermalConductivity;
			solidSurfaceAreaMultiplier = e.solidSurfaceAreaMultiplier;
			liquidSurfaceAreaMultiplier = e.liquidSurfaceAreaMultiplier;
			gasSurfaceAreaMultiplier = e.gasSurfaceAreaMultiplier;
			molarMass = e.molarMass;
			strength = e.strength;
			flow = e.flow;
			viscosity = e.viscosity;
			minHorizontalFlow = e.minHorizontalFlow;
			minVerticalFlow = e.minVerticalFlow;
			maxMass = e.maxMass;
			lowTemp = e.lowTemp;
			highTemp = e.highTemp;
			highTempTransitionOreID = e.highTempTransitionOreID;
			highTempTransitionOreMassConversion = e.highTempTransitionOreMassConversion;
			lowTempTransitionOreID = e.lowTempTransitionOreID;
			lowTempTransitionOreMassConversion = e.lowTempTransitionOreMassConversion;
			sublimateIndex = (sbyte)elements.FindIndex((global::Element ele) => ele.id == e.sublimateId);
			convertIndex = (sbyte)elements.FindIndex((global::Element ele) => ele.id == e.convertId);
			pack0 = 0;
			pack1 = 0;
			if (e.substance == null)
			{
				colour = 0u;
			}
			else
			{
				Color32 color = e.substance.colour;
				colour = (uint)((color.a << 24) | (color.b << 16) | (color.g << 8) | color.r);
			}
			sublimateFX = e.sublimateFX;
			lightAbsorptionFactor = e.lightAbsorptionFactor;
			defaultValues = e.defaultValues;
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write((int)id);
			writer.Write(state);
			writer.Write((sbyte)lowTempTransitionIdx);
			writer.Write((sbyte)highTempTransitionIdx);
			writer.Write(elementsTableIdx);
			writer.Write(specificHeatCapacity);
			writer.Write(thermalConductivity);
			writer.Write(molarMass);
			writer.Write(solidSurfaceAreaMultiplier);
			writer.Write(liquidSurfaceAreaMultiplier);
			writer.Write(gasSurfaceAreaMultiplier);
			writer.Write(flow);
			writer.Write(viscosity);
			writer.Write(minHorizontalFlow);
			writer.Write(minVerticalFlow);
			writer.Write(maxMass);
			writer.Write(lowTemp);
			writer.Write(highTemp);
			writer.Write(strength);
			writer.Write((int)lowTempTransitionOreID);
			writer.Write(lowTempTransitionOreMassConversion);
			writer.Write((int)highTempTransitionOreID);
			writer.Write(highTempTransitionOreMassConversion);
			writer.Write(sublimateIndex);
			writer.Write(convertIndex);
			writer.Write(pack0);
			writer.Write(pack1);
			writer.Write(colour);
			writer.Write((int)sublimateFX);
			writer.Write(lightAbsorptionFactor);
			defaultValues.Write(writer);
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct DiseaseCell
	{
		public byte diseaseIdx;

		private byte reservedInfestationTickCount;

		private byte pad1;

		private byte pad2;

		public int elementCount;

		private float reservedAccumulatedError;

		public static readonly DiseaseCell Invalid = new DiseaseCell
		{
			diseaseIdx = 255,
			elementCount = 0
		};

		public void Write(BinaryWriter writer)
		{
			writer.Write(diseaseIdx);
			writer.Write(reservedInfestationTickCount);
			writer.Write(pad1);
			writer.Write(pad2);
			writer.Write(elementCount);
			writer.Write(reservedAccumulatedError);
		}
	}

	public delegate void GAME_Callback();

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct SolidInfo
	{
		public int cellIdx;

		public int isSolid;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct LiquidChangeInfo
	{
		public int cellIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct SolidSubstanceChangeInfo
	{
		public int cellIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct SubstanceChangeInfo
	{
		public int cellIdx;

		public byte oldElemIdx;

		public byte newElemIdx;

		private byte pad0;

		private byte pad1;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct CallbackInfo
	{
		public int callbackIdx;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct GameDataUpdate
	{
		public int numFramesProcessed;

		public unsafe byte* elementIdx;

		public unsafe float* temperature;

		public unsafe float* mass;

		public unsafe byte* properties;

		public unsafe byte* insulation;

		public unsafe byte* strengthInfo;

		public unsafe byte* diseaseIdx;

		public unsafe int* diseaseCount;

		public int numSolidInfo;

		public unsafe SolidInfo* solidInfo;

		public int numLiquidChangeInfo;

		public unsafe LiquidChangeInfo* liquidChangeInfo;

		public int numSolidSubstanceChangeInfo;

		public unsafe SolidSubstanceChangeInfo* solidSubstanceChangeInfo;

		public int numSubstanceChangeInfo;

		public unsafe SubstanceChangeInfo* substanceChangeInfo;

		public int numCallbackInfo;

		public unsafe CallbackInfo* callbackInfo;

		public int numSpawnFallingLiquidInfo;

		public unsafe SpawnFallingLiquidInfo* spawnFallingLiquidInfo;

		public int numDigInfo;

		public unsafe SpawnOreInfo* digInfo;

		public int numSpawnOreInfo;

		public unsafe SpawnOreInfo* spawnOreInfo;

		public int numSpawnFXInfo;

		public unsafe SpawnFXInfo* spawnFXInfo;

		public int numUnstableCellInfo;

		public unsafe UnstableCellInfo* unstableCellInfo;

		public int numWorldDamageInfo;

		public unsafe WorldDamageInfo* worldDamageInfo;

		public int numBuildingTemperatures;

		public unsafe BuildingTemperatureInfo* buildingTemperatures;

		public int numMassConsumedCallbacks;

		public unsafe MassConsumedCallback* massConsumedCallbacks;

		public int numMassEmittedCallbacks;

		public unsafe MassEmittedCallback* massEmittedCallbacks;

		public int numDiseaseConsumptionCallbacks;

		public unsafe DiseaseConsumptionCallback* diseaseConsumptionCallbacks;

		public int numComponentStateChangedMessages;

		public unsafe ComponentStateChangedMessage* componentStateChangedMessages;

		public int numRemovedMassEntries;

		public unsafe ConsumedMassInfo* removedMassEntries;

		public int numEmittedMassEntries;

		public unsafe EmittedMassInfo* emittedMassEntries;

		public int numElementChunkInfos;

		public unsafe ElementChunkInfo* elementChunkInfos;

		public int numElementChunkMeltedInfos;

		public unsafe MeltedInfo* elementChunkMeltedInfos;

		public int numBuildingOverheatInfos;

		public unsafe MeltedInfo* buildingOverheatInfos;

		public int numBuildingNoLongerOverheatedInfos;

		public unsafe MeltedInfo* buildingNoLongerOverheatedInfos;

		public int numBuildingMeltedInfos;

		public unsafe MeltedInfo* buildingMeltedInfos;

		public int numCellMeltedInfos;

		public unsafe CellMeltedInfo* cellMeltedInfos;

		public int numDiseaseEmittedInfos;

		public unsafe DiseaseEmittedInfo* diseaseEmittedInfos;

		public int numDiseaseConsumedInfos;

		public unsafe DiseaseConsumedInfo* diseaseConsumedInfos;

		public unsafe float* accumulatedFlow;

		public IntPtr propertyTextureFlow;

		public IntPtr propertyTextureLiquid;

		public IntPtr propertyTextureExposedToSunlight;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct SpawnFallingLiquidInfo
	{
		public int cellIdx;

		public byte elemIdx;

		public byte diseaseIdx;

		public byte pad0;

		public byte pad1;

		public float mass;

		public float temperature;

		public int diseaseCount;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct SpawnOreInfo
	{
		public int cellIdx;

		public byte elemIdx;

		public byte diseaseIdx;

		private byte pad0;

		private byte pad1;

		public float mass;

		public float temperature;

		public int diseaseCount;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct SpawnFXInfo
	{
		public int cellIdx;

		public int fxHash;

		public float rotation;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct UnstableCellInfo
	{
		public enum FallingInfo
		{
			StartedFalling,
			StoppedFalling
		}

		public int cellIdx;

		public byte fallingInfo;

		public byte elemIdx;

		public byte diseaseIdx;

		private byte pad0;

		public float mass;

		public float temperature;

		public int diseaseCount;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct NewGameFrame
	{
		public float elapsedSeconds;

		public int minX;

		public int minY;

		public int maxX;

		public int maxY;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct WorldDamageInfo
	{
		public int gameCell;

		public int damageSourceOffset;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct PipeTemperatureChange
	{
		public int cellIdx;

		public float temperature;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct MassConsumedCallback
	{
		public int callbackIdx;

		public byte elemIdx;

		public byte diseaseIdx;

		private byte pad0;

		private byte pad1;

		public float mass;

		public float temperature;

		public int diseaseCount;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct MassEmittedCallback
	{
		public int callbackIdx;

		public byte suceeded;

		public byte elemIdx;

		public byte diseaseIdx;

		private byte pad0;

		public float mass;

		public float temperature;

		public int diseaseCount;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct DiseaseConsumptionCallback
	{
		public int callbackIdx;

		public byte diseaseIdx;

		private byte pad0;

		private byte pad1;

		private byte pad2;

		public int diseaseCount;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ComponentStateChangedMessage
	{
		public int callbackIdx;

		public int simHandle;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct DebugProperties
	{
		public float buildingTemperatureScale;

		public float contaminatedOxygenEmitProbability;

		public float contaminatedOxygenConversionPercent;

		public float biomeTemperatureLerpRate;

		public byte isDebugEditing;

		public byte pad0;

		public byte pad1;

		public byte pad2;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct EmittedMassInfo
	{
		public byte elemIdx;

		public byte diseaseIdx;

		public byte pad0;

		public byte pad1;

		public float mass;

		public float temperature;

		public int diseaseCount;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ConsumedMassInfo
	{
		public int simHandle;

		public byte removedElemIdx;

		public byte diseaseIdx;

		private byte pad0;

		private byte pad1;

		public float mass;

		public float temperature;

		public int diseaseCount;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ConsumedDiseaseInfo
	{
		public int simHandle;

		public byte diseaseIdx;

		private byte pad0;

		private byte pad1;

		private byte pad2;

		public int diseaseCount;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ElementChunkInfo
	{
		public float temperature;

		public float deltaKJ;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct MeltedInfo
	{
		public int handle;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct CellMeltedInfo
	{
		public int gameCell;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct BuildingTemperatureInfo
	{
		public int handle;

		public float temperature;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct BuildingConductivityData
	{
		public float temperature;

		public float heatCapacity;

		public float thermalConductivity;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct DiseaseEmittedInfo
	{
		public byte diseaseIdx;

		private byte pad0;

		private byte pad1;

		private byte pad2;

		public int count;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct DiseaseConsumedInfo
	{
		public byte diseaseIdx;

		private byte pad0;

		private byte pad1;

		private byte pad2;

		public int count;
	}

	public const int InvalidHandle = -1;

	public const int QueuedRegisterHandle = -2;

	public const byte InvalidDiseaseIdx = byte.MaxValue;

	public const byte InvalidElementIdx = byte.MaxValue;

	public const byte SpaceZoneID = byte.MaxValue;

	public const byte SolidZoneID = 0;

	public const int ChunkEdgeSize = 32;

	public const float StateTransitionEnergy = 3f;

	public const float ZeroDegreesCentigrade = 273.15f;

	public const float StandardTemperature = 293.15f;

	public const float StandardPressure = 101.3f;

	public const float Epsilon = 0.0001f;

	public const float MaxTemperature = 10000f;

	public const float MinTemperature = 0f;

	public const float MaxMass = 10000f;

	public const float MinMass = 1.0001f;

	private const int PressureUpdateInterval = 1;

	private const int TemperatureUpdateInterval = 1;

	private const int LiquidUpdateInterval = 1;

	private const int LifeUpdateInterval = 1;

	public const byte ClearSkyGridValue = 253;

	public const int PACKING_ALIGNMENT = 4;

	public static bool IsValidHandle(int h)
	{
		return h != -1 && h != -2;
	}

	public static int GetHandleIndex(int h)
	{
		return h & 0xFFFFFF;
	}

	[DllImport("SimDLL")]
	public static extern void SIM_Initialize(GAME_MessageHandler callback);

	[DllImport("SimDLL")]
	public static extern void SIM_Shutdown();

	[DllImport("SimDLL")]
	public unsafe static extern IntPtr SIM_HandleMessage(int sim_msg_id, int msg_length, byte* msg);

	[DllImport("SimDLL")]
	private unsafe static extern byte* SIM_BeginSave(int* size);

	[DllImport("SimDLL")]
	private static extern void SIM_EndSave();

	[DllImport("SimDLL")]
	public static extern void SIM_DebugCrash();

	public unsafe static IntPtr HandleMessage(SimMessageHashes sim_msg_id, int msg_length, byte[] msg)
	{
		//IL_0017: Incompatible stack types: I vs Ref
		fixed (byte* msg2 = &((msg != null && msg.Length != 0) ? ref msg[0] : ref *(byte*)null))
		{
			return SIM_HandleMessage((int)sim_msg_id, msg_length, msg2);
		}
	}

	public unsafe static void Save(BinaryWriter writer)
	{
		int num = default(int);
		byte* value = SIM_BeginSave(&num);
		byte[] array = new byte[num];
		Marshal.Copy((IntPtr)(void*)value, array, 0, num);
		SIM_EndSave();
		writer.Write(num);
		writer.Write(array);
	}

	public unsafe static int Load(FastReader reader)
	{
		//IL_0026: Incompatible stack types: I vs Ref
		int num = reader.ReadInt32();
		byte[] array = reader.ReadBytes(num);
		IntPtr intPtr;
		fixed (byte* msg = &((array != null && array.Length != 0) ? ref array[0] : ref *(byte*)null))
		{
			intPtr = SIM_HandleMessage(-672538170, num, msg);
		}
		if (intPtr == IntPtr.Zero)
		{
			return -1;
		}
		GameDataUpdate* ptr = (GameDataUpdate*)(void*)intPtr;
		Grid.elementIdx = ptr->elementIdx;
		Grid.temperature = ptr->temperature;
		Grid.mass = ptr->mass;
		Grid.properties = ptr->properties;
		Grid.strengthInfo = ptr->strengthInfo;
		Grid.insulation = ptr->insulation;
		Grid.diseaseIdx = ptr->diseaseIdx;
		Grid.diseaseCount = ptr->diseaseCount;
		Grid.AccumulatedFlowValues = ptr->accumulatedFlow;
		PropertyTextures.externalFlowTex = ptr->propertyTextureFlow;
		PropertyTextures.externalLiquidTex = ptr->propertyTextureLiquid;
		PropertyTextures.externalExposedToSunlight = ptr->propertyTextureExposedToSunlight;
		Grid.InitializeCells();
		return 0;
	}

	public unsafe static void Shutdown()
	{
		SIM_Shutdown();
		Grid.mass = null;
	}

	[DllImport("SimDLL")]
	public unsafe static extern char* SYSINFO_Acquire();

	[DllImport("SimDLL")]
	public static extern void SYSINFO_Release();

	public unsafe static int DLL_MessageHandler(int message_id, IntPtr data)
	{
		switch (message_id)
		{
		case 1:
		{
			DLLReportMessageMessage* ptr2 = (DLLReportMessageMessage*)(void*)data;
			string msg = "SimMessage: " + Marshal.PtrToStringAnsi(ptr2->message);
			string stack_trace;
			if (ptr2->callstack != IntPtr.Zero)
			{
				stack_trace = Marshal.PtrToStringAnsi(ptr2->callstack);
			}
			else
			{
				string arg = Marshal.PtrToStringAnsi(ptr2->file);
				int line = ptr2->line;
				stack_trace = arg + ":" + line;
			}
			KCrashReporter.ReportDLLCrash(msg, stack_trace, null);
			return 0;
		}
		case 0:
		{
			DLLExceptionHandlerMessage* ptr = (DLLExceptionHandlerMessage*)(void*)data;
			string text = Marshal.PtrToStringAnsi(ptr->callstack);
			string dmp_filename = Marshal.PtrToStringAnsi(ptr->dmpFilename);
			KCrashReporter.ReportDLLCrash(text, text, dmp_filename);
			return 0;
		}
		default:
			return -1;
		}
	}
}
