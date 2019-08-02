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
		Grid.VisMasks = new Grid.VisFlags[Grid.CellCount];
		Grid.Visible = new byte[Grid.CellCount];
		Grid.Spawnable = new byte[Grid.CellCount];
		Grid.BuildMasks = new Grid.BuildFlags[Grid.CellCount];
		Grid.LightCount = new int[Grid.CellCount];
		Grid.RadiationCount = new int[Grid.CellCount];
		Grid.Damage = new float[Grid.CellCount];
		Grid.NavMasks = new Grid.NavFlags[Grid.CellCount];
		Grid.NavValidatorMasks = new Grid.NavValidatorFlags[Grid.CellCount];
		Grid.Decor = new float[Grid.CellCount];
		Grid.Loudness = new float[Grid.CellCount];
		Grid.GravitasFacility = new bool[Grid.CellCount];
		Grid.ObjectLayers = new Dictionary<int, GameObject>[39];
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
		Grid.VisMasks = null;
		Grid.Visible = null;
		Grid.Spawnable = null;
		Grid.BuildMasks = null;
		Grid.NavValidatorMasks = null;
		Grid.LightCount = null;
		Grid.RadiationCount = null;
		Grid.Damage = null;
		Grid.Decor = null;
		Grid.Loudness = null;
		Grid.GravitasFacility = null;
		Grid.ObjectLayers = null;
		Grid.ResetNavMasksAndDetails();
	}
}
