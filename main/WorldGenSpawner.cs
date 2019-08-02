using KSerialization;
using ProcGen;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using TemplateClasses;
using UnityEngine;

public class WorldGenSpawner : KMonoBehaviour
{
	private class Spawnable
	{
		public delegate GameObject PlaceEntityFn(Prefab prefab, int root_cell);

		private HandleVector<int>.Handle fogOfWarPartitionerEntry;

		private HandleVector<int>.Handle solidChangedPartitionerEntry;

		[CompilerGenerated]
		private static PlaceEntityFn _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static PlaceEntityFn _003C_003Ef__mg_0024cache1;

		[CompilerGenerated]
		private static PlaceEntityFn _003C_003Ef__mg_0024cache2;

		[CompilerGenerated]
		private static PlaceEntityFn _003C_003Ef__mg_0024cache3;

		[CompilerGenerated]
		private static PlaceEntityFn _003C_003Ef__mg_0024cache4;

		public Prefab spawnInfo
		{
			get;
			private set;
		}

		public bool isSpawned
		{
			get;
			private set;
		}

		public int cell
		{
			get;
			private set;
		}

		public Spawnable(Prefab spawn_info)
		{
			spawnInfo = spawn_info;
			int num = Grid.XYToCell(spawnInfo.location_x, spawnInfo.location_y);
			GameObject prefab = Assets.GetPrefab(spawn_info.id);
			if ((Object)prefab != (Object)null)
			{
				WorldSpawnableMonitor.Def def = prefab.GetDef<WorldSpawnableMonitor.Def>();
				if (def != null && def.adjustSpawnLocationCb != null)
				{
					num = def.adjustSpawnLocationCb(num);
				}
			}
			cell = num;
			Debug.Assert(Grid.IsValidCell(cell));
			if (Grid.Spawnable[cell] > 0)
			{
				TrySpawn();
			}
			else
			{
				fogOfWarPartitionerEntry = GameScenePartitioner.Instance.Add("WorldGenSpawner.OnReveal", this, cell, GameScenePartitioner.Instance.fogOfWarChangedLayer, OnReveal);
			}
		}

		private void OnReveal(object data)
		{
			if (Grid.Spawnable[cell] > 0)
			{
				TrySpawn();
			}
		}

		private void OnSolidChanged(object data)
		{
			if (!Grid.Solid[cell])
			{
				GameScenePartitioner.Instance.Free(ref solidChangedPartitionerEntry);
				Game.Instance.GetComponent<EntombedItemVisualizer>().RemoveItem(cell);
				Spawn();
			}
		}

		public void FreeResources()
		{
			if (solidChangedPartitionerEntry.IsValid())
			{
				GameScenePartitioner.Instance.Free(ref solidChangedPartitionerEntry);
				if ((Object)Game.Instance != (Object)null)
				{
					Game.Instance.GetComponent<EntombedItemVisualizer>().RemoveItem(cell);
				}
			}
			GameScenePartitioner.Instance.Free(ref fogOfWarPartitionerEntry);
			isSpawned = true;
		}

		public void TrySpawn()
		{
			if (!isSpawned && !solidChangedPartitionerEntry.IsValid())
			{
				GameScenePartitioner.Instance.Free(ref fogOfWarPartitionerEntry);
				GameObject prefab = Assets.GetPrefab(GetPrefabTag());
				if ((Object)prefab != (Object)null)
				{
					bool flag = false;
					if ((Object)prefab.GetComponent<Pickupable>() != (Object)null && !prefab.HasTag(GameTags.Creatures.Digger))
					{
						flag = true;
					}
					else if (prefab.GetDef<BurrowMonitor.Def>() != null)
					{
						flag = true;
					}
					if (flag && Grid.Solid[cell])
					{
						solidChangedPartitionerEntry = GameScenePartitioner.Instance.Add("WorldGenSpawner.OnSolidChanged", this, cell, GameScenePartitioner.Instance.solidChangedLayer, OnSolidChanged);
						Game.Instance.GetComponent<EntombedItemVisualizer>().AddItem(cell);
					}
					else
					{
						Spawn();
					}
				}
				else
				{
					Spawn();
				}
			}
		}

		private Tag GetPrefabTag()
		{
			Mob mob = SettingsCache.mobs.GetMob(spawnInfo.id);
			if (mob != null && mob.prefabName != null)
			{
				return new Tag(mob.prefabName);
			}
			return new Tag(spawnInfo.id);
		}

		private void Spawn()
		{
			isSpawned = true;
			GameObject gameObject = GetSpawnableCallback(spawnInfo.type)(spawnInfo, 0);
			if ((Object)gameObject != (Object)null && (bool)gameObject)
			{
				gameObject.SetActive(true);
				gameObject.Trigger(1119167081, null);
			}
			FreeResources();
		}

		public static PlaceEntityFn GetSpawnableCallback(Prefab.Type type)
		{
			switch (type)
			{
			case Prefab.Type.Building:
				return TemplateLoader.PlaceBuilding;
			case Prefab.Type.Ore:
				return TemplateLoader.PlaceElementalOres;
			case Prefab.Type.Other:
				return TemplateLoader.PlaceOtherEntities;
			case Prefab.Type.Pickupable:
				return TemplateLoader.PlacePickupables;
			default:
				return TemplateLoader.PlaceOtherEntities;
			}
		}
	}

	[Serialize]
	private Prefab[] spawnInfos;

	[Serialize]
	private bool hasPlacedTemplates;

	private List<Spawnable> spawnables = new List<Spawnable>();

	public bool SpawnsRemain()
	{
		return spawnables.Count > 0;
	}

	public void SpawnEverything()
	{
		for (int i = 0; i < spawnables.Count; i++)
		{
			spawnables[i].TrySpawn();
		}
	}

	public void ClearSpawnersInArea(Vector2 root_position, CellOffset[] area)
	{
		for (int i = 0; i < spawnables.Count; i++)
		{
			if (Grid.IsCellOffsetOf(Grid.PosToCell(root_position), spawnables[i].cell, area))
			{
				spawnables[i].FreeResources();
			}
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (!hasPlacedTemplates)
		{
			DoReveal();
		}
	}

	protected override void OnSpawn()
	{
		if (!hasPlacedTemplates)
		{
			PlaceTemplates();
			hasPlacedTemplates = true;
		}
		if (spawnInfos != null)
		{
			for (int i = 0; i < spawnInfos.Length; i++)
			{
				AddSpawnable(spawnInfos[i]);
			}
		}
	}

	[OnSerializing]
	private void OnSerializing()
	{
		List<Prefab> list = new List<Prefab>();
		for (int i = 0; i < spawnables.Count; i++)
		{
			Spawnable spawnable = spawnables[i];
			if (!spawnable.isSpawned)
			{
				list.Add(spawnable.spawnInfo);
			}
		}
		spawnInfos = list.ToArray();
	}

	private void AddSpawnable(Prefab prefab)
	{
		spawnables.Add(new Spawnable(prefab));
	}

	public void AddLegacySpawner(Tag tag, int cell)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		AddSpawnable(new Prefab(tag.Name, Prefab.Type.Other, vector2I.x, vector2I.y, SimHashes.Carbon, -1f, 1f, null, 0, Orientation.Neutral, null, null, 0));
	}

	private void PlaceTemplates()
	{
		spawnables = new List<Spawnable>();
		foreach (Prefab building in SaveGame.Instance.worldGen.SpawnData.buildings)
		{
			building.type = Prefab.Type.Building;
			AddSpawnable(building);
		}
		foreach (Prefab elementalOre in SaveGame.Instance.worldGen.SpawnData.elementalOres)
		{
			elementalOre.type = Prefab.Type.Ore;
			AddSpawnable(elementalOre);
		}
		foreach (Prefab otherEntity in SaveGame.Instance.worldGen.SpawnData.otherEntities)
		{
			otherEntity.type = Prefab.Type.Other;
			AddSpawnable(otherEntity);
		}
		foreach (Prefab pickupable in SaveGame.Instance.worldGen.SpawnData.pickupables)
		{
			pickupable.type = Prefab.Type.Pickupable;
			AddSpawnable(pickupable);
		}
		SaveGame.Instance.worldGen.SpawnData.buildings.Clear();
		SaveGame.Instance.worldGen.SpawnData.elementalOres.Clear();
		SaveGame.Instance.worldGen.SpawnData.otherEntities.Clear();
		SaveGame.Instance.worldGen.SpawnData.pickupables.Clear();
	}

	private void DoReveal()
	{
		Game.Instance.Reset(SaveGame.Instance.worldGen.SpawnData);
		for (int i = 0; i < Grid.CellCount; i++)
		{
			Grid.Revealed[i] = false;
			Grid.Spawnable[i] = 0;
		}
		float innerRadius = 16.5f;
		float radius = 18f;
		Vector2I baseStartPos = SaveGame.Instance.worldGen.SpawnData.baseStartPos;
		GridVisibility.Reveal(baseStartPos.x, baseStartPos.y, radius, innerRadius);
	}
}
