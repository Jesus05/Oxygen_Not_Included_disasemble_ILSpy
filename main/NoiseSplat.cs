using System;
using System.Collections.Generic;
using UnityEngine;

public class NoiseSplat : IUniformGridObject
{
	public const float noiseFalloff = 0.05f;

	private IPolluter provider;

	private Vector2 position;

	private int radius;

	private Extents effectExtents;

	private Extents baseExtents;

	private HandleVector<int>.Handle partitionerEntry;

	private HandleVector<int>.Handle solidChangedPartitionerEntry;

	private List<Pair<int, float>> decibels = new List<Pair<int, float>>();

	public int dB
	{
		get;
		private set;
	}

	public float deathTime
	{
		get;
		private set;
	}

	public NoiseSplat(NoisePolluter setProvider, float death_time = 0f)
	{
		deathTime = death_time;
		dB = 0;
		radius = 5;
		if (setProvider.dB != null)
		{
			dB = (int)setProvider.dB.GetTotalValue();
		}
		int cell = Grid.PosToCell(setProvider.gameObject);
		if (!NoisePolluter.IsNoiseableCell(cell))
		{
			dB = 0;
		}
		if (dB != 0)
		{
			setProvider.Clear();
			OccupyArea occupyArea = setProvider.occupyArea;
			baseExtents = occupyArea.GetExtents();
			provider = setProvider;
			position = setProvider.transform.GetPosition();
			if (setProvider.dBRadius != null)
			{
				radius = (int)setProvider.dBRadius.GetTotalValue();
			}
			if (radius != 0)
			{
				int x = 0;
				int y = 0;
				Grid.CellToXY(cell, out x, out y);
				int widthInCells = occupyArea.GetWidthInCells();
				int heightInCells = occupyArea.GetHeightInCells();
				Vector2I vector2I = new Vector2I(x - radius, y - radius);
				Vector2I v = vector2I + new Vector2I(radius * 2 + widthInCells, radius * 2 + heightInCells);
				vector2I = Vector2I.Max(vector2I, Vector2I.zero);
				v = Vector2I.Min(v, new Vector2I(Grid.WidthInCells - 1, Grid.HeightInCells - 1));
				effectExtents = new Extents(vector2I.x, vector2I.y, v.x - vector2I.x, v.y - vector2I.y);
				partitionerEntry = GameScenePartitioner.Instance.Add("NoiseSplat.SplatCollectNoisePolluters", setProvider.gameObject, effectExtents, GameScenePartitioner.Instance.noisePolluterLayer, setProvider.onCollectNoisePollutersCallback);
				solidChangedPartitionerEntry = GameScenePartitioner.Instance.Add("NoiseSplat.SplatSolidCheck", setProvider.gameObject, effectExtents, GameScenePartitioner.Instance.solidChangedLayer, setProvider.refreshPartionerCallback);
			}
		}
	}

	public NoiseSplat(IPolluter setProvider, float death_time = 0f)
	{
		deathTime = death_time;
		provider = setProvider;
		provider.Clear();
		position = provider.GetPosition();
		dB = provider.GetNoise();
		int cell = Grid.PosToCell(position);
		if (!NoisePolluter.IsNoiseableCell(cell))
		{
			dB = 0;
		}
		if (dB != 0)
		{
			radius = provider.GetRadius();
			if (radius != 0)
			{
				int x = 0;
				int y = 0;
				Grid.CellToXY(cell, out x, out y);
				Vector2I vector2I = new Vector2I(x - radius, y - radius);
				Vector2I v = vector2I + new Vector2I(radius * 2, radius * 2);
				vector2I = Vector2I.Max(vector2I, Vector2I.zero);
				v = Vector2I.Min(v, new Vector2I(Grid.WidthInCells - 1, Grid.HeightInCells - 1));
				effectExtents = new Extents(vector2I.x, vector2I.y, v.x - vector2I.x, v.y - vector2I.y);
				baseExtents = new Extents(x, y, 1, 1);
				AddNoise();
			}
		}
	}

	public string GetName()
	{
		return provider.GetName();
	}

	public IPolluter GetProvider()
	{
		return provider;
	}

	public Vector2 PosMin()
	{
		return new Vector2(position.x - (float)radius, position.y - (float)radius);
	}

	public Vector2 PosMax()
	{
		return new Vector2(position.x + (float)radius, position.y + (float)radius);
	}

	public void Clear()
	{
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		GameScenePartitioner.Instance.Free(ref solidChangedPartitionerEntry);
		RemoveNoise();
	}

	private void AddNoise()
	{
		int cell = Grid.PosToCell(position);
		int val = effectExtents.x + effectExtents.width;
		int val2 = effectExtents.y + effectExtents.height;
		int x = effectExtents.x;
		int y = effectExtents.y;
		int x2 = 0;
		int y2 = 0;
		Grid.CellToXY(cell, out x2, out y2);
		val = Math.Min(val, Grid.WidthInCells);
		val2 = Math.Min(val2, Grid.HeightInCells);
		x = Math.Max(0, x);
		y = Math.Max(0, y);
		for (int i = y; i < val2; i++)
		{
			for (int j = x; j < val; j++)
			{
				if (Grid.VisibilityTest(x2, y2, j, i, false))
				{
					int num = Grid.XYToCell(j, i);
					float dBForCell = GetDBForCell(num);
					if (dBForCell > 0f)
					{
						float num2 = AudioEventManager.DBToLoudness(dBForCell);
						Grid.Loudness[num] += num2;
						Pair<int, float> item = new Pair<int, float>(num, num2);
						decibels.Add(item);
					}
				}
			}
		}
	}

	public float GetDBForCell(int cell)
	{
		Vector2 b = Grid.CellToPos2D(cell);
		float num = Mathf.Floor(Vector2.Distance(position, b));
		if (b.x >= (float)baseExtents.x && b.x < (float)(baseExtents.x + baseExtents.width) && b.y >= (float)baseExtents.y && b.y < (float)(baseExtents.y + baseExtents.height))
		{
			num = 0f;
		}
		float f = (float)dB - (float)dB * num * 0.05f;
		return Mathf.Round(f);
	}

	private void RemoveNoise()
	{
		for (int i = 0; i < decibels.Count; i++)
		{
			Pair<int, float> pair = decibels[i];
			float num = Math.Max(0f, Grid.Loudness[pair.first] - pair.second);
			Grid.Loudness[pair.first] = ((!(num < 1f)) ? num : 0f);
		}
		decibels.Clear();
	}

	public float GetLoudness(int cell)
	{
		float result = 0f;
		for (int i = 0; i < decibels.Count; i++)
		{
			Pair<int, float> pair = decibels[i];
			if (pair.first == cell)
			{
				result = pair.second;
				break;
			}
		}
		return result;
	}
}
