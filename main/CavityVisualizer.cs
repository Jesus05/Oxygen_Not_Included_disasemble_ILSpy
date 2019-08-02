using ProcGenGame;
using System.Collections.Generic;
using UnityEngine;

public class CavityVisualizer : KMonoBehaviour
{
	public static CavityVisualizer Instance;

	public List<int> cavityCells = new List<int>();

	public List<int> spawnCells = new List<int>();

	public bool drawCavity = true;

	public bool drawSpawnCells = true;

	protected override void OnPrefabInit()
	{
		Debug.Assert((Object)Instance == (Object)null);
		Instance = this;
		base.OnPrefabInit();
		foreach (TerrainCell key in MobSpawning.NaturalCavities.Keys)
		{
			foreach (HashSet<int> item in MobSpawning.NaturalCavities[key])
			{
				foreach (int item2 in item)
				{
					cavityCells.Add(item2);
				}
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (drawCavity)
		{
			Color[] array = new Color[2]
			{
				Color.blue,
				Color.yellow
			};
			int num = 0;
			foreach (TerrainCell key in MobSpawning.NaturalCavities.Keys)
			{
				Gizmos.color = array[num % array.Length];
				Color color = Gizmos.color;
				float r = color.r;
				Color color2 = Gizmos.color;
				float g = color2.g;
				Color color3 = Gizmos.color;
				Gizmos.color = new Color(r, g, color3.b, 0.125f);
				num++;
				foreach (HashSet<int> item in MobSpawning.NaturalCavities[key])
				{
					foreach (int item2 in item)
					{
						Vector3 a = Grid.CellToPos(item2);
						a += Vector3.right / 2f + Vector3.up / 2f;
						Gizmos.DrawCube(a, Vector3.one);
					}
				}
			}
		}
		if (spawnCells != null && drawSpawnCells)
		{
			Gizmos.color = new Color(0f, 1f, 0f, 0.15f);
			foreach (int spawnCell in spawnCells)
			{
				Vector3 a2 = Grid.CellToPos(spawnCell);
				a2 += Vector3.right / 2f + Vector3.up / 2f;
				Gizmos.DrawCube(a2, Vector3.one);
			}
		}
	}
}
