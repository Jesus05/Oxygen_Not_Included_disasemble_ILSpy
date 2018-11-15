using System.Collections.Generic;
using UnityEngine;

public class GridSettings : KMonoBehaviour
{
	public int GridWidthInCells;

	public int GridHeightInCells;

	public int SimChunkEdgeSize = 32;

	public const float CellSizeInMeters = 1f;

	public static void Reset(int width, int height)
	{
		Grid.WidthInCells = width;
		Grid.HeightInCells = height;
		Grid.CellCount = width * height;
		Grid.WidthInMeters = 1f * (float)width;
		Grid.HeightInMeters = 1f * (float)height;
		Grid.CellSizeInMeters = 1f;
		Grid.HalfCellSizeInMeters = 0.5f;
		Grid.Element = new Element[Grid.CellCount];
		Grid.Revealed = new bool[Grid.CellCount];
		Grid.Reserved = new bool[Grid.CellCount];
		Grid.Visible = new byte[Grid.CellCount];
		Grid.Spawnable = new byte[Grid.CellCount];
		Grid.BitFields = new ushort[Grid.CellCount];
		Grid.LightCount = new int[Grid.CellCount];
		Grid.Damage = new float[Grid.CellCount];
		Grid.HasDoor = new bool[Grid.CellCount];
		Grid.HasAccessDoor = new bool[Grid.CellCount];
		Grid.HasLadder = new bool[Grid.CellCount];
		Grid.HasPole = new bool[Grid.CellCount];
		Grid.HasTube = new bool[Grid.CellCount];
		Grid.HasTubeEntrance = new bool[Grid.CellCount];
		Grid.Decor = new float[Grid.CellCount];
		Grid.Loudness = new float[Grid.CellCount];
		Grid.PreventFogOfWarReveal = new bool[Grid.CellCount];
		Grid.IsTileUnderConstruction = new bool[Grid.CellCount];
		Grid.PreventIdleTraversal = new bool[Grid.CellCount];
		Grid.AllowPathfinding = new bool[Grid.CellCount];
		Grid.GravitasFacility = new bool[Grid.CellCount];
		Grid.ObjectLayers = new Dictionary<int, GameObject>[37];
		for (int i = 0; i < Grid.ObjectLayers.Length; i++)
		{
			Grid.ObjectLayers[i] = new Dictionary<int, GameObject>();
		}
		for (int j = 0; j < Grid.CellCount; j++)
		{
			Grid.Loudness[j] = 0f;
		}
		if ((Object)Game.Instance != (Object)null)
		{
			Game.Instance.gasConduitSystem.Initialize(Grid.WidthInCells, Grid.HeightInCells);
			Game.Instance.liquidConduitSystem.Initialize(Grid.WidthInCells, Grid.HeightInCells);
			Game.Instance.electricalConduitSystem.Initialize(Grid.WidthInCells, Grid.HeightInCells);
			Game.Instance.travelTubeSystem.Initialize(Grid.WidthInCells, Grid.HeightInCells);
			Game.Instance.gasConduitFlow.Initialize(Grid.CellCount);
			Game.Instance.liquidConduitFlow.Initialize(Grid.CellCount);
		}
		if (Application.isPlaying)
		{
			Singleton<KBatchedAnimUpdater>.Instance.InitializeGrid();
		}
		Grid.OnReveal = null;
	}

	public static void ClearGrid()
	{
		Grid.WidthInCells = 0;
		Grid.HeightInCells = 0;
		Grid.CellCount = 0;
		Grid.WidthInMeters = 0f;
		Grid.HeightInMeters = 0f;
		Grid.CellSizeInMeters = 0f;
		Grid.HalfCellSizeInMeters = 0f;
		Grid.Element = null;
		Grid.Revealed = null;
		Grid.Reserved = null;
		Grid.Visible = null;
		Grid.Spawnable = null;
		Grid.BitFields = null;
		Grid.LightCount = null;
		Grid.Damage = null;
		Grid.HasDoor = null;
		Grid.HasAccessDoor = null;
		Grid.HasLadder = null;
		Grid.HasPole = null;
		Grid.HasTube = null;
		Grid.HasTubeEntrance = null;
		Grid.Decor = null;
		Grid.Loudness = null;
		Grid.PreventFogOfWarReveal = null;
		Grid.IsTileUnderConstruction = null;
		Grid.PreventIdleTraversal = null;
		Grid.AllowPathfinding = null;
		Grid.GravitasFacility = null;
		Grid.ObjectLayers = null;
	}
}
