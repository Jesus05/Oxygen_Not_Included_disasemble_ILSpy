using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class Grid
{
	[Flags]
	public enum BitField : ushort
	{
		Unused = 0x1,
		FakeFloor = 0x2,
		ForceField = 0x4,
		SuitRequired = 0x8,
		Foundation = 0x10,
		Solid = 0x20,
		PreviousSolid = 0x40,
		RenderedByWorld = 0x80,
		Impassable = 0x100,
		LiquidPumpFloor = 0x200
	}

	public enum SceneLayer
	{
		NoLayer = -2,
		Background = -1,
		TempShiftPlate = 1,
		GasConduits = 2,
		GasConduitBridges = 3,
		LiquidConduits = 4,
		LiquidConduitBridges = 5,
		SolidConduits = 6,
		SolidConduitContents = 7,
		SolidConduitBridges = 8,
		Wires = 9,
		WireBridges = 10,
		WireBridgesFront = 11,
		LogicWires = 12,
		LogicWireBridges = 13,
		LogicWireBridgesFront = 14,
		Paintings = 0xF,
		BuildingBack = 0x10,
		Building = 17,
		BuildingUse = 18,
		BuildingFront = 19,
		TransferArm = 20,
		Ore = 21,
		Creatures = 22,
		Move = 23,
		Front = 24,
		Liquid = 25,
		Ground = 26,
		TileMain = 27,
		TileFront = 28,
		FXFront = 29,
		FXFront2 = 30,
		SceneMAX = 0x1F
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

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct FoundationIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (BitFields[i] & 0x10) != 0;
			}
			set
			{
				BitFields[i] = (ushort)(BitFields[i] & 0xFFEF);
				BitFields[i] |= (ushort)(value ? 16 : 0);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct SolidIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (BitFields[i] & 0x20) != 0;
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct PreviousSolidIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (BitFields[i] & 0x40) != 0;
			}
			set
			{
				BitFields[i] = (ushort)(BitFields[i] & 0xFFBF);
				BitFields[i] |= (ushort)(value ? 64 : 0);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct RenderedByWorldIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (BitFields[i] & 0x80) != 0;
			}
			set
			{
				BitFields[i] = (ushort)(BitFields[i] & 0xFF7F);
				BitFields[i] |= (ushort)(value ? 128 : 0);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct FakeFloorIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (BitFields[i] & 2) != 0;
			}
			set
			{
				BitFields[i] = (ushort)(BitFields[i] & 0xFFFD);
				BitFields[i] |= (ushort)(value ? 2 : 0);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct LiquidPumpFloorIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (BitFields[i] & 0x200) != 0;
			}
			set
			{
				BitFields[i] = (ushort)(BitFields[i] & 0xFDFF);
				BitFields[i] |= (ushort)(value ? 512 : 0);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct ForceFieldIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (BitFields[i] & 4) != 0;
			}
			set
			{
				BitFields[i] = (ushort)(BitFields[i] & 0xFFFB);
				BitFields[i] |= (ushort)(value ? 4 : 0);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct ImpassableIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (BitFields[i] & 0x100) != 0;
			}
			set
			{
				BitFields[i] = (ushort)(BitFields[i] & 0xFEFF);
				BitFields[i] |= (ushort)(value ? 256 : 0);
			}
		}
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

	public static bool[] Revealed;

	public static bool[] Reserved;

	public static byte[] Visible;

	public static byte[] Spawnable;

	public static float[] Damage;

	public static bool[] HasDoor;

	public static bool[] HasAccessDoor;

	public static bool[] HasLadder;

	public static bool[] HasPole;

	public static bool[] HasTube;

	public static bool[] AllowPathfinding;

	public static bool[] HasTubeEntrance;

	public static bool[] IsTileUnderConstruction;

	public static bool[] PreventFogOfWarReveal;

	public static bool[] PreventIdleTraversal;

	public static float[] Decor;

	public static bool[] GravitasFacility;

	public static float[] Loudness;

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

	public static FoundationIndexer Foundation;

	public static SolidIndexer Solid;

	public static PreviousSolidIndexer PreviousSolid;

	public static RenderedByWorldIndexer RenderedByWorld;

	public static FakeFloorIndexer FakeFloor;

	public static LiquidPumpFloorIndexer LiquidPumpFloor;

	public static ForceFieldIndexer ForceField;

	public static ImpassableIndexer Impassable;

	public static ushort[] BitFields;

	public static Element[] Element;

	public static int[] LightCount;

	public static ObjectLayerIndexer Objects;

	public static Dictionary<int, GameObject>[] ObjectLayers;

	public static Action<int> OnReveal;

	public static float LayerMultiplier = 1f;

	[CompilerGenerated]
	private static Func<int, bool> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Func<int, bool> _003C_003Ef__mg_0024cache1;

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

	public static void SetSolid(int cell, bool solid, CellSolidEvent ev)
	{
		BitFields[cell] = (ushort)(BitFields[cell] & 0xFFDF);
		BitFields[cell] |= (ushort)(solid ? 32 : 0);
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

	public unsafe static void InitializeCells()
	{
		int widthInCells = WidthInCells;
		int heightInCells = HeightInCells;
		List<Element> elements = ElementLoader.elements;
		for (int i = 0; i < heightInCells; i++)
		{
			for (int j = 0; j < widthInCells; j++)
			{
				int num = i * widthInCells + j;
				byte index = elementIdx[num];
				Element element = elements[index];
				Element[num] = element;
				int num2 = BitFields[num];
				num2 &= 0xFF17;
				num2 |= (element.IsSolid ? 96 : 0);
				num2 |= ((element.substance != null && element.substance.renderedByWorld && (UnityEngine.Object)Objects[num, 9] == (UnityEngine.Object)null) ? 128 : 0);
				BitFields[num] = (ushort)num2;
			}
		}
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
