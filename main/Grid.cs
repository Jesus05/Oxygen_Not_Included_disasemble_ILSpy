using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class Grid
{
	[Flags]
	public enum BuildFlags : byte
	{
		FakeFloor = 0x1,
		ForceField = 0x2,
		Foundation = 0x4,
		Solid = 0x8,
		PreviousSolid = 0x10,
		Impassable = 0x20,
		LiquidPumpFloor = 0x40,
		Door = 0x80
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct BuildFlagsFoundationIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (BuildMasks[i] & BuildFlags.Foundation) != ~(BuildFlags.FakeFloor | BuildFlags.ForceField | BuildFlags.Foundation | BuildFlags.Solid | BuildFlags.PreviousSolid | BuildFlags.Impassable | BuildFlags.LiquidPumpFloor | BuildFlags.Door);
			}
			set
			{
				UpdateBuildMask(i, BuildFlags.Foundation, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct BuildFlagsSolidIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (BuildMasks[i] & BuildFlags.Solid) != ~(BuildFlags.FakeFloor | BuildFlags.ForceField | BuildFlags.Foundation | BuildFlags.Solid | BuildFlags.PreviousSolid | BuildFlags.Impassable | BuildFlags.LiquidPumpFloor | BuildFlags.Door);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct BuildFlagsPreviousSolidIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (BuildMasks[i] & BuildFlags.PreviousSolid) != ~(BuildFlags.FakeFloor | BuildFlags.ForceField | BuildFlags.Foundation | BuildFlags.Solid | BuildFlags.PreviousSolid | BuildFlags.Impassable | BuildFlags.LiquidPumpFloor | BuildFlags.Door);
			}
			set
			{
				UpdateBuildMask(i, BuildFlags.PreviousSolid, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct BuildFlagsFakeFloorIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (BuildMasks[i] & BuildFlags.FakeFloor) != ~(BuildFlags.FakeFloor | BuildFlags.ForceField | BuildFlags.Foundation | BuildFlags.Solid | BuildFlags.PreviousSolid | BuildFlags.Impassable | BuildFlags.LiquidPumpFloor | BuildFlags.Door);
			}
			set
			{
				UpdateBuildMask(i, BuildFlags.FakeFloor, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct BuildFlagsLiquidPumpFloorIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (BuildMasks[i] & BuildFlags.LiquidPumpFloor) != ~(BuildFlags.FakeFloor | BuildFlags.ForceField | BuildFlags.Foundation | BuildFlags.Solid | BuildFlags.PreviousSolid | BuildFlags.Impassable | BuildFlags.LiquidPumpFloor | BuildFlags.Door);
			}
			set
			{
				UpdateBuildMask(i, BuildFlags.LiquidPumpFloor, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct BuildFlagsForceFieldIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (BuildMasks[i] & BuildFlags.ForceField) != ~(BuildFlags.FakeFloor | BuildFlags.ForceField | BuildFlags.Foundation | BuildFlags.Solid | BuildFlags.PreviousSolid | BuildFlags.Impassable | BuildFlags.LiquidPumpFloor | BuildFlags.Door);
			}
			set
			{
				UpdateBuildMask(i, BuildFlags.ForceField, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct BuildFlagsImpassableIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (BuildMasks[i] & BuildFlags.Impassable) != ~(BuildFlags.FakeFloor | BuildFlags.ForceField | BuildFlags.Foundation | BuildFlags.Solid | BuildFlags.PreviousSolid | BuildFlags.Impassable | BuildFlags.LiquidPumpFloor | BuildFlags.Door);
			}
			set
			{
				UpdateBuildMask(i, BuildFlags.Impassable, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct BuildFlagsDoorIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (BuildMasks[i] & BuildFlags.Door) != ~(BuildFlags.FakeFloor | BuildFlags.ForceField | BuildFlags.Foundation | BuildFlags.Solid | BuildFlags.PreviousSolid | BuildFlags.Impassable | BuildFlags.LiquidPumpFloor | BuildFlags.Door);
			}
			set
			{
				UpdateBuildMask(i, BuildFlags.Door, value);
			}
		}
	}

	[Flags]
	public enum VisFlags : byte
	{
		Revealed = 0x1,
		PreventFogOfWarReveal = 0x2,
		RenderedByWorld = 0x4,
		AllowPathfinding = 0x8
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct VisFlagsRevealedIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (VisMasks[i] & VisFlags.Revealed) != (VisFlags)0;
			}
			set
			{
				UpdateVisMask(i, VisFlags.Revealed, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct VisFlagsPreventFogOfWarRevealIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (VisMasks[i] & VisFlags.PreventFogOfWarReveal) != (VisFlags)0;
			}
			set
			{
				UpdateVisMask(i, VisFlags.PreventFogOfWarReveal, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct VisFlagsRenderedByWorldIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (VisMasks[i] & VisFlags.RenderedByWorld) != (VisFlags)0;
			}
			set
			{
				UpdateVisMask(i, VisFlags.RenderedByWorld, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct VisFlagsAllowPathfindingIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (VisMasks[i] & VisFlags.AllowPathfinding) != (VisFlags)0;
			}
			set
			{
				UpdateVisMask(i, VisFlags.AllowPathfinding, value);
			}
		}
	}

	[Flags]
	public enum NavValidatorFlags : byte
	{
		Ladder = 0x1,
		Pole = 0x2,
		Tube = 0x4,
		UnderConstruction = 0x8
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct NavValidatorFlagsLadderIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (NavValidatorMasks[i] & NavValidatorFlags.Ladder) != (NavValidatorFlags)0;
			}
			set
			{
				UpdateNavValidatorMask(i, NavValidatorFlags.Ladder, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct NavValidatorFlagsPoleIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (NavValidatorMasks[i] & NavValidatorFlags.Pole) != (NavValidatorFlags)0;
			}
			set
			{
				UpdateNavValidatorMask(i, NavValidatorFlags.Pole, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct NavValidatorFlagsTubeIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (NavValidatorMasks[i] & NavValidatorFlags.Tube) != (NavValidatorFlags)0;
			}
			set
			{
				UpdateNavValidatorMask(i, NavValidatorFlags.Tube, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct NavValidatorFlagsUnderConstructionIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (NavValidatorMasks[i] & NavValidatorFlags.UnderConstruction) != (NavValidatorFlags)0;
			}
			set
			{
				UpdateNavValidatorMask(i, NavValidatorFlags.UnderConstruction, value);
			}
		}
	}

	[Flags]
	public enum NavFlags : byte
	{
		AccessDoor = 0x1,
		TubeEntrance = 0x2,
		PreventIdleTraversal = 0x4,
		Reserved = 0x8,
		SuitMarker = 0x10
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct NavFlagsAccessDoorIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (NavMasks[i] & NavFlags.AccessDoor) != (NavFlags)0;
			}
			set
			{
				UpdateNavMask(i, NavFlags.AccessDoor, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct NavFlagsTubeEntranceIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (NavMasks[i] & NavFlags.TubeEntrance) != (NavFlags)0;
			}
			set
			{
				UpdateNavMask(i, NavFlags.TubeEntrance, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct NavFlagsPreventIdleTraversalIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (NavMasks[i] & NavFlags.PreventIdleTraversal) != (NavFlags)0;
			}
			set
			{
				UpdateNavMask(i, NavFlags.PreventIdleTraversal, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct NavFlagsReservedIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (NavMasks[i] & NavFlags.Reserved) != (NavFlags)0;
			}
			set
			{
				UpdateNavMask(i, NavFlags.Reserved, value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct NavFlagsSuitMarkerIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (NavMasks[i] & NavFlags.SuitMarker) != (NavFlags)0;
			}
			set
			{
				UpdateNavMask(i, NavFlags.SuitMarker, value);
			}
		}
	}

	public struct Restriction
	{
		[Flags]
		public enum Directions : byte
		{
			Left = 0x1,
			Right = 0x2
		}

		public enum Orientation : byte
		{
			Vertical,
			Horizontal
		}

		public const int DefaultID = -1;

		public Dictionary<int, Directions> directionMasks;

		public Orientation orientation;
	}

	private struct TubeEntrance
	{
		public int reservationCapacity;

		public HashSet<int> reservations;
	}

	public struct SuitMarker
	{
		[Flags]
		public enum Flags : byte
		{
			OnlyTraverseIfUnequipAvailable = 0x1,
			Operational = 0x2,
			Rotated = 0x4
		}

		public int suitCount;

		public int lockerCount;

		public Flags flags;

		public PathFinder.PotentialPath.Flags pathFlags;

		public HashSet<int> suitReservations;

		public HashSet<int> emptyLockerReservations;

		public int emptyLockerCount => lockerCount - suitCount;
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct ObjectLayerIndexer
	{
		public GameObject this[int cell, int layer]
		{
			get
			{
				GameObject value = null;
				ObjectLayers[layer].TryGetValue(cell, out value);
				return value;
			}
			set
			{
				if ((UnityEngine.Object)value == (UnityEngine.Object)null)
				{
					ObjectLayers[layer].Remove(cell);
				}
				else
				{
					ObjectLayers[layer][cell] = value;
				}
				GameScenePartitioner.Instance.TriggerEvent(cell, GameScenePartitioner.Instance.objectLayers[layer], value);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct PressureIndexer
	{
		public unsafe float this[int i]
		{
			get
			{
				return mass[i] * 101.3f;
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct TransparentIndexer
	{
		public unsafe bool this[int i]
		{
			get
			{
				return (properties[i] & 0x10) != 0;
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct ElementIdxIndexer
	{
		public unsafe byte this[int i]
		{
			get
			{
				return elementIdx[i];
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct TemperatureIndexer
	{
		public unsafe float this[int i]
		{
			get
			{
				return temperature[i];
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct MassIndexer
	{
		public unsafe float this[int i]
		{
			get
			{
				return mass[i];
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct PropertiesIndexer
	{
		public unsafe byte this[int i]
		{
			get
			{
				return properties[i];
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct ExposedToSunlightIndexer
	{
		public unsafe byte this[int i]
		{
			get
			{
				return exposedToSunlight[i];
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct StrengthInfoIndexer
	{
		public unsafe byte this[int i]
		{
			get
			{
				return strengthInfo[i];
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct Insulationndexer
	{
		public unsafe byte this[int i]
		{
			get
			{
				return insulation[i];
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct DiseaseIdxIndexer
	{
		public unsafe byte this[int i]
		{
			get
			{
				return diseaseIdx[i];
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct DiseaseCountIndexer
	{
		public unsafe int this[int i]
		{
			get
			{
				return diseaseCount[i];
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct AccumulatedFlowIndexer
	{
		public unsafe float this[int i]
		{
			get
			{
				return AccumulatedFlowValues[i];
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct LightIntensityIndexer
	{
		public unsafe int this[int i]
		{
			get
			{
				int num = (int)((float)(int)exposedToSunlight[i] / 255f * Game.Instance.currentSunlightIntensity);
				int num2 = LightCount[i];
				return num + num2;
			}
		}
	}

	public enum SceneLayer
	{
		NoLayer = -2,
		Background = -1,
		Backwall = 1,
		Gas = 2,
		GasConduits = 3,
		GasConduitBridges = 4,
		LiquidConduits = 5,
		LiquidConduitBridges = 6,
		SolidConduits = 7,
		SolidConduitContents = 8,
		SolidConduitBridges = 9,
		Wires = 10,
		WireBridges = 11,
		WireBridgesFront = 12,
		LogicWires = 13,
		LogicWireBridges = 14,
		LogicWireBridgesFront = 0xF,
		InteriorWall = 0x10,
		GasFront = 17,
		BuildingBack = 18,
		Building = 19,
		BuildingUse = 20,
		BuildingFront = 21,
		TransferArm = 22,
		Ore = 23,
		Creatures = 24,
		Move = 25,
		Front = 26,
		GlassTile = 27,
		Liquid = 28,
		Ground = 29,
		TileMain = 30,
		TileFront = 0x1F,
		FXFront = 0x20,
		FXFront2 = 33,
		SceneMAX = 34
	}

	public static readonly CellOffset[] DefaultOffset = new CellOffset[1]
	{
		default(CellOffset)
	};

	public static float WidthInMeters;

	public static float HeightInMeters;

	public static int WidthInCells;

	public static int HeightInCells;

	public static float CellSizeInMeters;

	public static float InverseCellSizeInMeters;

	public static float HalfCellSizeInMeters;

	public static int CellCount;

	public static int InvalidCell = -1;

	public static int TopBorderHeight = 2;

	public static Dictionary<int, GameObject>[] ObjectLayers;

	public static Action<int> OnReveal;

	public static BuildFlags[] BuildMasks;

	public static BuildFlagsFoundationIndexer Foundation;

	public static BuildFlagsSolidIndexer Solid;

	public static BuildFlagsPreviousSolidIndexer PreviousSolid;

	public static BuildFlagsFakeFloorIndexer FakeFloor;

	public static BuildFlagsLiquidPumpFloorIndexer LiquidPumpFloor;

	public static BuildFlagsForceFieldIndexer ForceField;

	public static BuildFlagsImpassableIndexer Impassable;

	public static BuildFlagsDoorIndexer HasDoor;

	public static VisFlags[] VisMasks;

	public static VisFlagsRevealedIndexer Revealed;

	public static VisFlagsPreventFogOfWarRevealIndexer PreventFogOfWarReveal;

	public static VisFlagsRenderedByWorldIndexer RenderedByWorld;

	public static VisFlagsAllowPathfindingIndexer AllowPathfinding;

	public static NavValidatorFlags[] NavValidatorMasks;

	public static NavValidatorFlagsLadderIndexer HasLadder;

	public static NavValidatorFlagsPoleIndexer HasPole;

	public static NavValidatorFlagsTubeIndexer HasTube;

	public static NavValidatorFlagsUnderConstructionIndexer IsTileUnderConstruction;

	public static NavFlags[] NavMasks;

	public static NavFlagsAccessDoorIndexer HasAccessDoor;

	public static NavFlagsTubeEntranceIndexer HasTubeEntrance;

	public static NavFlagsPreventIdleTraversalIndexer PreventIdleTraversal;

	public static NavFlagsReservedIndexer Reserved;

	public static NavFlagsSuitMarkerIndexer HasSuitMarker;

	private static Dictionary<int, Restriction> restrictions = new Dictionary<int, Restriction>();

	private static Dictionary<int, TubeEntrance> tubeEntrances = new Dictionary<int, TubeEntrance>();

	private static Dictionary<int, SuitMarker> suitMarkers = new Dictionary<int, SuitMarker>();

	public unsafe static byte* elementIdx;

	public unsafe static float* temperature;

	public unsafe static float* mass;

	public unsafe static byte* properties;

	public unsafe static byte* strengthInfo;

	public unsafe static byte* insulation;

	public unsafe static byte* diseaseIdx;

	public unsafe static int* diseaseCount;

	public unsafe static byte* exposedToSunlight;

	public unsafe static float* AccumulatedFlowValues = null;

	public static byte[] Visible;

	public static byte[] Spawnable;

	public static float[] Damage;

	public static float[] Decor;

	public static bool[] GravitasFacility;

	public static float[] Loudness;

	public static Element[] Element;

	public static int[] LightCount;

	public static PressureIndexer Pressure;

	public static TransparentIndexer Transparent;

	public static ElementIdxIndexer ElementIdx;

	public static TemperatureIndexer Temperature;

	public static MassIndexer Mass;

	public static PropertiesIndexer Properties;

	public static ExposedToSunlightIndexer ExposedToSunlight;

	public static StrengthInfoIndexer StrengthInfo;

	public static Insulationndexer Insulation;

	public static DiseaseIdxIndexer DiseaseIdx;

	public static DiseaseCountIndexer DiseaseCount;

	public static LightIntensityIndexer LightIntensity;

	public static AccumulatedFlowIndexer AccumulatedFlow;

	public static ObjectLayerIndexer Objects;

	public static float LayerMultiplier = 1f;

	[CompilerGenerated]
	private static Func<int, bool> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Func<int, bool> _003C_003Ef__mg_0024cache1;

	private static void UpdateBuildMask(int i, BuildFlags flag, bool state)
	{
		if (state)
		{
			BuildFlags[] buildMasks;
			int num;
			(buildMasks = BuildMasks)[num = i] = (buildMasks[num] | flag);
		}
		else
		{
			BuildFlags[] buildMasks;
			int num2;
			(buildMasks = BuildMasks)[num2 = i] = (BuildFlags)((int)buildMasks[num2] & (int)(byte)(~(uint)flag));
		}
	}

	public static void SetSolid(int cell, bool solid, CellSolidEvent ev)
	{
		UpdateBuildMask(cell, BuildFlags.Solid, solid);
	}

	private static void UpdateVisMask(int i, VisFlags flag, bool state)
	{
		if (state)
		{
			VisFlags[] visMasks;
			int num;
			(visMasks = VisMasks)[num = i] = (visMasks[num] | flag);
		}
		else
		{
			VisFlags[] visMasks;
			int num2;
			(visMasks = VisMasks)[num2 = i] = (VisFlags)((int)visMasks[num2] & (int)(byte)(~(uint)flag));
		}
	}

	private static void UpdateNavValidatorMask(int i, NavValidatorFlags flag, bool state)
	{
		if (state)
		{
			NavValidatorFlags[] navValidatorMasks;
			int num;
			(navValidatorMasks = NavValidatorMasks)[num = i] = (navValidatorMasks[num] | flag);
		}
		else
		{
			NavValidatorFlags[] navValidatorMasks;
			int num2;
			(navValidatorMasks = NavValidatorMasks)[num2 = i] = (NavValidatorFlags)((int)navValidatorMasks[num2] & (int)(byte)(~(uint)flag));
		}
	}

	private static void UpdateNavMask(int i, NavFlags flag, bool state)
	{
		if (state)
		{
			NavFlags[] navMasks;
			int num;
			(navMasks = NavMasks)[num = i] = (navMasks[num] | flag);
		}
		else
		{
			NavFlags[] navMasks;
			int num2;
			(navMasks = NavMasks)[num2 = i] = (NavFlags)((int)navMasks[num2] & (int)(byte)(~(uint)flag));
		}
	}

	public static void ResetNavMasksAndDetails()
	{
		NavMasks = null;
		tubeEntrances.Clear();
		restrictions.Clear();
		suitMarkers.Clear();
	}

	public static void RegisterRestriction(int cell, Restriction.Orientation orientation)
	{
		restrictions.Add(cell, new Restriction
		{
			directionMasks = new Dictionary<int, Restriction.Directions>(),
			orientation = orientation
		});
	}

	public static void UnregisterRestriction(int cell)
	{
		restrictions.Remove(cell);
	}

	public static void SetRestriction(int cell, int minion, Restriction.Directions directions)
	{
		Restriction restriction = restrictions[cell];
		restriction.directionMasks[minion] = directions;
	}

	public static void ClearRestriction(int cell, int minion)
	{
		Restriction restriction = restrictions[cell];
		restriction.directionMasks.Remove(minion);
	}

	public static bool HasPermission(int cell, int minion, int fromCell)
	{
		DebugUtil.Assert(HasAccessDoor[cell]);
		Restriction restriction = restrictions[cell];
		Vector2I vector2I = CellToXY(cell);
		Vector2I vector2I2 = CellToXY(fromCell);
		Restriction.Directions directions = (Restriction.Directions)0;
		switch (restriction.orientation)
		{
		case Restriction.Orientation.Vertical:
		{
			int num2 = vector2I.x - vector2I2.x;
			if (num2 < 0)
			{
				directions |= Restriction.Directions.Left;
			}
			if (num2 > 0)
			{
				directions |= Restriction.Directions.Right;
			}
			break;
		}
		case Restriction.Orientation.Horizontal:
		{
			int num = vector2I.y - vector2I2.y;
			if (num > 0)
			{
				directions |= Restriction.Directions.Left;
			}
			if (num < 0)
			{
				directions |= Restriction.Directions.Right;
			}
			break;
		}
		}
		Restriction.Directions value = (Restriction.Directions)0;
		if (restriction.directionMasks.TryGetValue(minion, out value) || restriction.directionMasks.TryGetValue(-1, out value))
		{
			return (value & directions) == (Restriction.Directions)0;
		}
		return true;
	}

	public static void RegisterTubeEntrance(int cell, int reservationCapacity)
	{
		DebugUtil.Assert(!tubeEntrances.ContainsKey(cell));
		HasTubeEntrance[cell] = true;
		tubeEntrances.Add(cell, new TubeEntrance
		{
			reservationCapacity = reservationCapacity,
			reservations = new HashSet<int>()
		});
	}

	public static void UnregisterTubeEntrance(int cell)
	{
		DebugUtil.Assert(tubeEntrances.ContainsKey(cell));
		HasTubeEntrance[cell] = false;
		tubeEntrances.Remove(cell);
	}

	public static bool ReserveTubeEntrance(int cell, int minion, bool reserve)
	{
		TubeEntrance tubeEntrance = tubeEntrances[cell];
		HashSet<int> reservations = tubeEntrance.reservations;
		if (reserve)
		{
			DebugUtil.Assert(HasTubeEntrance[cell]);
			if (reservations.Count == tubeEntrance.reservationCapacity)
			{
				return false;
			}
			bool test = reservations.Add(minion);
			DebugUtil.Assert(test);
			return true;
		}
		return reservations.Remove(minion);
	}

	public static void SetTubeEntranceReservationCapacity(int cell, int newReservationCapacity)
	{
		DebugUtil.Assert(HasTubeEntrance[cell]);
		TubeEntrance value = tubeEntrances[cell];
		value.reservationCapacity = newReservationCapacity;
		tubeEntrances[cell] = value;
	}

	public static bool HasUsableTubeEntrance(int cell, int minion)
	{
		if (!HasTubeEntrance[cell])
		{
			return false;
		}
		TubeEntrance tubeEntrance = tubeEntrances[cell];
		HashSet<int> reservations = tubeEntrance.reservations;
		return reservations.Count < tubeEntrance.reservationCapacity || reservations.Contains(minion);
	}

	public static bool HasReservedTubeEntrance(int cell, int minion)
	{
		DebugUtil.Assert(HasTubeEntrance[cell]);
		TubeEntrance tubeEntrance = tubeEntrances[cell];
		return tubeEntrance.reservations.Contains(minion);
	}

	public static void ActivateTubeEntrance(int cell, bool activate)
	{
		DebugUtil.Assert(tubeEntrances.ContainsKey(cell));
		HasTubeEntrance[cell] = activate;
	}

	public static void RegisterSuitMarker(int cell)
	{
		DebugUtil.Assert(!HasSuitMarker[cell]);
		HasSuitMarker[cell] = true;
		suitMarkers.Add(cell, new SuitMarker
		{
			suitCount = 0,
			lockerCount = 0,
			flags = SuitMarker.Flags.Operational,
			suitReservations = new HashSet<int>(),
			emptyLockerReservations = new HashSet<int>()
		});
	}

	public static void UnregisterSuitMarker(int cell)
	{
		DebugUtil.Assert(HasSuitMarker[cell]);
		HasSuitMarker[cell] = false;
		suitMarkers.Remove(cell);
	}

	public static bool ReserveSuit(int cell, int minion, bool reserve)
	{
		DebugUtil.Assert(HasSuitMarker[cell]);
		SuitMarker suitMarker = suitMarkers[cell];
		HashSet<int> suitReservations = suitMarker.suitReservations;
		if (reserve)
		{
			if (suitReservations.Count == suitMarker.suitCount)
			{
				return false;
			}
			bool test = suitReservations.Add(minion);
			DebugUtil.Assert(test);
			return true;
		}
		return suitReservations.Remove(minion);
	}

	public static bool ReserveEmptyLocker(int cell, int minion, bool reserve)
	{
		DebugUtil.Assert(HasSuitMarker[cell]);
		SuitMarker suitMarker = suitMarkers[cell];
		HashSet<int> emptyLockerReservations = suitMarker.emptyLockerReservations;
		if (reserve)
		{
			if (emptyLockerReservations.Count == suitMarker.emptyLockerCount)
			{
				return false;
			}
			bool test = emptyLockerReservations.Add(minion);
			DebugUtil.Assert(test);
			return true;
		}
		return emptyLockerReservations.Remove(minion);
	}

	public static void UpdateSuitMarker(int cell, int fullLockerCount, int emptyLockerCount, SuitMarker.Flags flags, PathFinder.PotentialPath.Flags pathFlags)
	{
		DebugUtil.Assert(HasSuitMarker[cell]);
		SuitMarker value = suitMarkers[cell];
		value.suitCount = fullLockerCount;
		value.lockerCount = fullLockerCount + emptyLockerCount;
		value.flags = flags;
		value.pathFlags = pathFlags;
		suitMarkers[cell] = value;
	}

	public static bool TryGetSuitMarkerFlags(int cell, out SuitMarker.Flags flags, out PathFinder.PotentialPath.Flags pathFlags)
	{
		if (HasSuitMarker[cell])
		{
			SuitMarker suitMarker = suitMarkers[cell];
			flags = suitMarker.flags;
			SuitMarker suitMarker2 = suitMarkers[cell];
			pathFlags = suitMarker2.pathFlags;
			return true;
		}
		flags = (SuitMarker.Flags)0;
		pathFlags = PathFinder.PotentialPath.Flags.None;
		return false;
	}

	public static bool HasSuit(int cell, int minion)
	{
		if (!HasSuitMarker[cell])
		{
			return false;
		}
		SuitMarker suitMarker = suitMarkers[cell];
		HashSet<int> suitReservations = suitMarker.suitReservations;
		return suitReservations.Count < suitMarker.suitCount || suitReservations.Contains(minion);
	}

	public static bool HasEmptyLocker(int cell, int minion)
	{
		if (!HasSuitMarker[cell])
		{
			return false;
		}
		SuitMarker suitMarker = suitMarkers[cell];
		HashSet<int> emptyLockerReservations = suitMarker.emptyLockerReservations;
		return emptyLockerReservations.Count < suitMarker.emptyLockerCount || emptyLockerReservations.Contains(minion);
	}

	public unsafe static void InitializeCells()
	{
		for (int i = 0; i != WidthInCells * HeightInCells; i++)
		{
			byte index = elementIdx[i];
			Element element = ElementLoader.elements[index];
			Element[i] = element;
			if (element.IsSolid)
			{
				BuildFlags[] buildMasks;
				int num;
				(buildMasks = BuildMasks)[num = i] = (buildMasks[num] | (BuildFlags.Solid | BuildFlags.PreviousSolid));
			}
			else
			{
				BuildFlags[] buildMasks;
				int num2;
				(buildMasks = BuildMasks)[num2 = i] = (buildMasks[num2] & ~(BuildFlags.Solid | BuildFlags.PreviousSolid));
			}
			RenderedByWorld[i] = (element.substance != null && element.substance.renderedByWorld && (UnityEngine.Object)Objects[i, 9] == (UnityEngine.Object)null);
		}
	}

	public unsafe static bool IsInitialized()
	{
		return mass != null;
	}

	public static int GetCellInDirection(int cell, Direction d)
	{
		switch (d)
		{
		case Direction.Up:
			return cell + WidthInCells;
		case Direction.Down:
			return cell - WidthInCells;
		case Direction.Left:
			return cell - 1;
		case Direction.Right:
			return cell + 1;
		case Direction.None:
			return cell;
		default:
			return -1;
		}
	}

	public static int CellAbove(int cell)
	{
		return cell + WidthInCells;
	}

	public static int CellBelow(int cell)
	{
		return cell - WidthInCells;
	}

	public static int CellLeft(int cell)
	{
		return (cell % WidthInCells <= 0) ? (-1) : (cell - 1);
	}

	public static int CellRight(int cell)
	{
		return (cell % WidthInCells >= WidthInCells - 1) ? (-1) : (cell + 1);
	}

	public static CellOffset GetOffset(int cell)
	{
		int x = 0;
		int y = 0;
		CellToXY(cell, out x, out y);
		return new CellOffset(x, y);
	}

	public static int CellUpLeft(int cell)
	{
		int result = -1;
		if (cell < (HeightInCells - 1) * WidthInCells && cell % WidthInCells > 0)
		{
			result = cell - 1 + WidthInCells;
		}
		return result;
	}

	public static int CellUpRight(int cell)
	{
		int result = -1;
		if (cell < (HeightInCells - 1) * WidthInCells && cell % WidthInCells < WidthInCells - 1)
		{
			result = cell + 1 + WidthInCells;
		}
		return result;
	}

	public static int CellDownLeft(int cell)
	{
		int result = -1;
		if (cell > WidthInCells && cell % WidthInCells > 0)
		{
			result = cell - 1 - WidthInCells;
		}
		return result;
	}

	public static int CellDownRight(int cell)
	{
		int result = -1;
		if (cell >= WidthInCells && cell % WidthInCells < WidthInCells - 1)
		{
			result = cell + 1 - WidthInCells;
		}
		return result;
	}

	public static bool IsCellLeftOf(int cell, int other_cell)
	{
		return CellColumn(cell) < CellColumn(other_cell);
	}

	public static bool IsCellOffsetOf(int cell, int target_cell, CellOffset[] target_offsets)
	{
		int num = target_offsets.Length;
		for (int i = 0; i < num; i++)
		{
			if (cell == OffsetCell(target_cell, target_offsets[i]))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsCellOffsetOf(int cell, GameObject target, CellOffset[] target_offsets)
	{
		int target_cell = PosToCell(target);
		return IsCellOffsetOf(cell, target_cell, target_offsets);
	}

	public static int GetCellDistance(int cell_a, int cell_b)
	{
		CellOffset offset = GetOffset(cell_a, cell_b);
		return Math.Abs(offset.x) + Math.Abs(offset.y);
	}

	public static int GetCellRange(int cell_a, int cell_b)
	{
		CellOffset offset = GetOffset(cell_a, cell_b);
		return Math.Max(Math.Abs(offset.x), Math.Abs(offset.y));
	}

	public static CellOffset GetOffset(int base_cell, int offset_cell)
	{
		CellToXY(base_cell, out int x, out int y);
		CellToXY(offset_cell, out int x2, out int y2);
		return new CellOffset(x2 - x, y2 - y);
	}

	public static int OffsetCell(int cell, CellOffset offset)
	{
		return cell + offset.x + offset.y * WidthInCells;
	}

	public static int OffsetCell(int cell, int x, int y)
	{
		return cell + x + y * WidthInCells;
	}

	public static bool IsCellOffsetValid(int cell, int x, int y)
	{
		CellToXY(cell, out int x2, out int y2);
		return x2 + x >= 0 && x2 + x < WidthInCells && y2 + y >= 0 && y2 + y < HeightInCells;
	}

	public static bool IsCellOffsetValid(int cell, CellOffset offset)
	{
		return IsCellOffsetValid(cell, offset.x, offset.y);
	}

	public static int PosToCell(StateMachine.Instance smi)
	{
		return PosToCell(smi.transform.GetPosition());
	}

	public static int PosToCell(GameObject go)
	{
		return PosToCell(go.transform.GetPosition());
	}

	public static int PosToCell(KMonoBehaviour cmp)
	{
		return PosToCell(cmp.transform.GetPosition());
	}

	public static bool IsValidBuildingCell(int cell)
	{
		return cell >= 0 && cell < CellCount - WidthInCells * TopBorderHeight;
	}

	public static bool IsValidCell(int cell)
	{
		return cell >= 0 && cell < CellCount;
	}

	public static int PosToCell(Vector2 pos)
	{
		float x = pos.x;
		float num = pos.y + 0.05f;
		int num2 = (int)num;
		int num3 = (int)x;
		return num2 * WidthInCells + num3;
	}

	public static int PosToCell(Vector3 pos)
	{
		float x = pos.x;
		float num = pos.y + 0.05f;
		int num2 = (int)num;
		int num3 = (int)x;
		return num2 * WidthInCells + num3;
	}

	public static void PosToXY(Vector3 pos, out int x, out int y)
	{
		int cell = PosToCell(pos);
		CellToXY(cell, out x, out y);
	}

	public static void PosToXY(Vector3 pos, out Vector2I xy)
	{
		int cell = PosToCell(pos);
		CellToXY(cell, out xy.x, out xy.y);
	}

	public static Vector2I PosToXY(Vector3 pos)
	{
		int cell = PosToCell(pos);
		Vector2I result = default(Vector2I);
		CellToXY(cell, out result.x, out result.y);
		return result;
	}

	public static int XYToCell(int x, int y)
	{
		return x + y * WidthInCells;
	}

	public static void CellToXY(int cell, out int x, out int y)
	{
		x = CellColumn(cell);
		y = CellRow(cell);
	}

	public static Vector2I CellToXY(int cell)
	{
		return new Vector2I(CellColumn(cell), CellRow(cell));
	}

	public static Vector3 CellToPos(int cell, float x_offset, float y_offset, float z_offset)
	{
		int widthInCells = WidthInCells;
		float num = CellSizeInMeters * (float)(cell % widthInCells);
		float num2 = CellSizeInMeters * (float)(cell / widthInCells);
		return new Vector3(num + x_offset, num2 + y_offset, z_offset);
	}

	public static Vector3 CellToPos(int cell)
	{
		int widthInCells = WidthInCells;
		float x = CellSizeInMeters * (float)(cell % widthInCells);
		float y = CellSizeInMeters * (float)(cell / widthInCells);
		return new Vector3(x, y, 0f);
	}

	public static Vector3 CellToPos2D(int cell)
	{
		int widthInCells = WidthInCells;
		float x = CellSizeInMeters * (float)(cell % widthInCells);
		float y = CellSizeInMeters * (float)(cell / widthInCells);
		return new Vector2(x, y);
	}

	public static int CellRow(int cell)
	{
		return cell / WidthInCells;
	}

	public static int CellColumn(int cell)
	{
		return cell % WidthInCells;
	}

	public static int ClampX(int x)
	{
		return Math.Min(Math.Max(x, 0), WidthInCells - 1);
	}

	public static int ClampY(int y)
	{
		return Math.Min(Math.Max(y, 0), HeightInCells - 1);
	}

	public static Vector2I Constrain(Vector2I val)
	{
		val.x = Mathf.Max(0, Mathf.Min(val.x, WidthInCells - 1));
		val.y = Mathf.Max(0, Mathf.Min(val.y, HeightInCells - 1));
		return val;
	}

	public static void Reveal(int cell, byte visibility = byte.MaxValue)
	{
		bool flag = Spawnable[cell] == 0 && visibility > 0;
		Spawnable[cell] = Math.Max(visibility, Visible[cell]);
		if (!PreventFogOfWarReveal[cell])
		{
			Visible[cell] = Math.Max(visibility, Visible[cell]);
		}
		if (flag && OnReveal != null)
		{
			OnReveal(cell);
		}
	}

	public static ObjectLayer GetObjectLayerForConduitType(ConduitType conduit_type)
	{
		switch (conduit_type)
		{
		case ConduitType.Gas:
			return ObjectLayer.GasConduitConnection;
		case ConduitType.Liquid:
			return ObjectLayer.LiquidConduitConnection;
		case ConduitType.Solid:
			return ObjectLayer.SolidConduitConnection;
		default:
			throw new ArgumentException("Invalid value.", "conduit_type");
		}
	}

	public static Vector3 CellToPos(int cell, CellAlignment alignment, SceneLayer layer)
	{
		switch (alignment)
		{
		case CellAlignment.Bottom:
			return CellToPosCBC(cell, layer);
		case CellAlignment.Left:
			return CellToPosLCC(cell, layer);
		case CellAlignment.Right:
			return CellToPosRCC(cell, layer);
		case CellAlignment.Top:
			return CellToPosCTC(cell, layer);
		case CellAlignment.RandomInternal:
		{
			Vector3 b = new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), 0f, 0f);
			return CellToPosCCC(cell, layer) + b;
		}
		default:
			return CellToPosCCC(cell, layer);
		}
	}

	public static float GetLayerZ(SceneLayer layer)
	{
		return 0f - HalfCellSizeInMeters - CellSizeInMeters * (float)layer * LayerMultiplier;
	}

	public static Vector3 CellToPosCCC(int cell, SceneLayer layer)
	{
		return CellToPos(cell, HalfCellSizeInMeters, HalfCellSizeInMeters, GetLayerZ(layer));
	}

	public static Vector3 CellToPosCBC(int cell, SceneLayer layer)
	{
		return CellToPos(cell, HalfCellSizeInMeters, 0.01f, GetLayerZ(layer));
	}

	public static Vector3 CellToPosCCF(int cell, SceneLayer layer)
	{
		return CellToPos(cell, HalfCellSizeInMeters, HalfCellSizeInMeters, (0f - CellSizeInMeters) * (float)layer * LayerMultiplier);
	}

	public static Vector3 CellToPosLCC(int cell, SceneLayer layer)
	{
		return CellToPos(cell, 0.01f, HalfCellSizeInMeters, GetLayerZ(layer));
	}

	public static Vector3 CellToPosRCC(int cell, SceneLayer layer)
	{
		return CellToPos(cell, CellSizeInMeters - 0.01f, HalfCellSizeInMeters, GetLayerZ(layer));
	}

	public static Vector3 CellToPosCTC(int cell, SceneLayer layer)
	{
		return CellToPos(cell, HalfCellSizeInMeters, CellSizeInMeters - 0.01f, GetLayerZ(layer));
	}

	public static bool IsSolidCell(int cell)
	{
		return IsValidCell(cell) && Solid[cell];
	}

	public unsafe static bool IsSubstantialLiquid(int cell, float threshold = 0.35f)
	{
		if (IsValidCell(cell))
		{
			byte b = elementIdx[cell];
			if (b < ElementLoader.elements.Count)
			{
				Element element = ElementLoader.elements[b];
				if (element.IsLiquid && mass[cell] >= element.defaultValues.mass * threshold)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool IsVisiblyInLiquid(Vector2 pos)
	{
		int num = PosToCell(pos);
		if (IsValidCell(num) && IsLiquid(num))
		{
			int cell = CellAbove(num);
			if (IsValidCell(cell) && IsLiquid(cell))
			{
				return true;
			}
			float num2 = Mass[num];
			float num3 = (float)(int)pos.y - pos.y;
			if (num2 / 1000f <= num3)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsLiquid(int cell)
	{
		Element element = ElementLoader.elements[ElementIdx[cell]];
		if (element.IsLiquid)
		{
			return true;
		}
		return false;
	}

	public static bool IsGas(int cell)
	{
		Element element = ElementLoader.elements[ElementIdx[cell]];
		if (element.IsGas)
		{
			return true;
		}
		return false;
	}

	public static void GetVisibleExtents(out int min_x, out int min_y, out int max_x, out int max_y)
	{
		Camera main = Camera.main;
		Vector3 position = Camera.main.transform.GetPosition();
		Vector3 vector = main.ViewportToWorldPoint(new Vector3(1f, 1f, position.z));
		Camera main2 = Camera.main;
		Vector3 position2 = Camera.main.transform.GetPosition();
		Vector3 vector2 = main2.ViewportToWorldPoint(new Vector3(0f, 0f, position2.z));
		min_y = (int)vector2.y;
		max_y = (int)(vector.y + 0.5f);
		min_x = (int)vector2.x;
		max_x = (int)(vector.x + 0.5f);
	}

	public static void GetVisibleExtents(out Vector2I min, out Vector2I max)
	{
		GetVisibleExtents(out min.x, out min.y, out max.x, out max.y);
	}

	public static bool IsVisible(int cell)
	{
		return Visible[cell] > 0 || !PropertyTextures.IsFogOfWarEnabled;
	}

	public static bool VisibleBlockingCB(int cell)
	{
		return !Transparent[cell] && IsSolidCell(cell);
	}

	public static bool VisibilityTest(int x, int y, int x2, int y2, bool blocking_tile_visible = false)
	{
		return TestLineOfSight(x, y, x2, y2, VisibleBlockingCB, blocking_tile_visible);
	}

	public static bool VisibilityTest(int cell, int target_cell, bool blocking_tile_visible = false)
	{
		int x = 0;
		int y = 0;
		CellToXY(cell, out x, out y);
		int x2 = 0;
		int y2 = 0;
		CellToXY(target_cell, out x2, out y2);
		return VisibilityTest(x, y, x2, y2, blocking_tile_visible);
	}

	public static bool PhysicalBlockingCB(int cell)
	{
		return Solid[cell];
	}

	public static bool IsPhysicallyAccessible(int x, int y, int x2, int y2, bool blocking_tile_visible = false)
	{
		return TestLineOfSight(x, y, x2, y2, PhysicalBlockingCB, blocking_tile_visible);
	}

	public static bool TestLineOfSight(int x, int y, int x2, int y2, Func<int, bool> blocking_cb, bool blocking_tile_visible = false)
	{
		int num = x;
		int num2 = y;
		int num3 = x2 - x;
		int num4 = y2 - y;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		int num8 = 0;
		if (num3 < 0)
		{
			num5 = -1;
		}
		else if (num3 > 0)
		{
			num5 = 1;
		}
		if (num4 < 0)
		{
			num6 = -1;
		}
		else if (num4 > 0)
		{
			num6 = 1;
		}
		if (num3 < 0)
		{
			num7 = -1;
		}
		else if (num3 > 0)
		{
			num7 = 1;
		}
		int num9 = Math.Abs(num3);
		int num10 = Math.Abs(num4);
		if (num9 <= num10)
		{
			num9 = Math.Abs(num4);
			num10 = Math.Abs(num3);
			if (num4 < 0)
			{
				num8 = -1;
			}
			else if (num4 > 0)
			{
				num8 = 1;
			}
			num7 = 0;
		}
		int num11 = num9 >> 1;
		for (int i = 0; i <= num9; i++)
		{
			int num12 = XYToCell(x, y);
			if (!IsValidCell(num12))
			{
				return false;
			}
			bool flag = blocking_cb(num12);
			if ((x != num || y != num2) && flag)
			{
				if (blocking_tile_visible && x == x2 && y == y2)
				{
					return true;
				}
				return false;
			}
			num11 += num10;
			if (num11 >= num9)
			{
				num11 -= num9;
				x += num5;
				y += num6;
			}
			else
			{
				x += num7;
				y += num8;
			}
		}
		return true;
	}

	[Conditional("UNITY_EDITOR")]
	public static void DrawBoxOnCell(int cell, Color color, float offset = 0f)
	{
		Vector3 vector = CellToPos(cell) + new Vector3(0.5f, 0.5f, 0f);
		float num = 0.5f + offset;
	}
}
