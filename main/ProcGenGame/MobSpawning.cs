using ProcGen;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGenGame
{
	public static class MobSpawning
	{
		public static Dictionary<TerrainCell, List<HashSet<int>>> NaturalCavities = new Dictionary<TerrainCell, List<HashSet<int>>>();

		public static HashSet<int> allNaturalCavityCells = new HashSet<int>();

		public static Dictionary<int, string> PlaceAmbientMobs(TerrainCell tc, SeededRandom rnd, Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dc, HashSet<int> avoidCells)
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			Node node = tc.node;
			HashSet<int> alreadyOccupiedCells = new HashSet<int>();
			List<Tag> list = new List<Tag>();
			bool flag = false;
			int num = 0;
			if (node.tags != null && node.biomeSpecificTags != null)
			{
				foreach (Tag biomeSpecificTag in node.biomeSpecificTags)
				{
					if (WorldGen.Settings.mobs.HasMob(biomeSpecificTag.Name) && WorldGen.Settings.mobs.GetMob(biomeSpecificTag.Name) != null)
					{
						list.Add(biomeSpecificTag);
						num++;
						flag = true;
					}
				}
				if (flag)
				{
					List<int> availableSpawnCells = tc.GetAvailableSpawnCells();
					tc.LogInfo("PlaceAmbientMobs", "possibleSpawnPoints", (float)availableSpawnCells.Count);
					for (int num2 = availableSpawnCells.Count - 1; num2 > 0; num2--)
					{
						int num3 = availableSpawnCells[num2];
						if (ElementLoader.elements[cells[num3].elementIdx].id == SimHashes.Katairite || ElementLoader.elements[cells[num3].elementIdx].id == SimHashes.Unobtanium || avoidCells.Contains(num3))
						{
							availableSpawnCells.RemoveAt(num2);
						}
					}
					tc.LogInfo("mob spawns", "Id:" + node.node.Id + " possible cells", (float)availableSpawnCells.Count);
					if (availableSpawnCells.Count != 0)
					{
						for (int i = 0; i < MobSettings.AmbientMobDensity; i++)
						{
							if (availableSpawnCells.Count <= 0)
							{
								break;
							}
							list.ShuffleSeeded(rnd.RandomSource());
							for (int j = 0; j < list.Count; j++)
							{
								if (!WorldGen.Settings.mobs.GetMobTags().Contains(list[j]))
								{
									Debug.LogError("Missing sample description for tag [" + list[j].Name + "]", null);
								}
								else
								{
									Mob mob = WorldGen.Settings.mobs.MobLookupTable[list[j].Name];
									List<int> list2 = null;
									list2 = availableSpawnCells.FindAll((int cell) => isSuitableMobSpawnPoint(cell, mob, cells, bgTemp, dc, ref alreadyOccupiedCells));
									if (list2.Count == 0)
									{
										if (WorldGen.isRunningDebugGen)
										{
											Debug.LogWarning("No SuitableMobSpawnPoint to put mobs mobPossibleSpawnPoints [" + list[j].Name + "] [" + tc.node.node.Id + "]", null);
										}
									}
									else
									{
										list2.ShuffleSeeded(rnd.RandomSource());
										tc.LogInfo("\t\tpossible", list[j].ToString() + " mps: " + list2.Count + " ps:", (float)availableSpawnCells.Count);
										float num4 = mob.density.GetRandomValueWithinRange(rnd);
										if (num4 > 1f)
										{
											if (WorldGen.isRunningDebugGen)
											{
												Debug.LogWarning("Got a mob density greater than 1.0 for " + list[j].Name + ". Probably using density as spacing!", null);
											}
											num4 = 1f;
										}
										int num5 = Mathf.RoundToInt((float)list2.Count * num4);
										tc.LogInfo("\t\tcount", list[j].ToString(), (float)num5);
										Tag value = (mob.prefabName != null) ? new Tag(mob.prefabName) : list[j];
										for (int k = 0; k < num5; k++)
										{
											if (list2.Count == 0)
											{
												break;
											}
											int num6 = list2[0];
											for (int l = 0; l < mob.width; l++)
											{
												for (int m = 0; m < mob.height; m++)
												{
													int item = MobWidthOffset(num6, l);
													alreadyOccupiedCells.Add(item);
													if (list2.Contains(item))
													{
														list2.Remove(item);
													}
													if (availableSpawnCells.Contains(item))
													{
														availableSpawnCells.Remove(item);
													}
												}
											}
											tc.AddMob(new KeyValuePair<int, Tag>(num6, value));
											dictionary.Add(num6, value.Name);
										}
									}
								}
							}
						}
						return dictionary;
					}
					if (WorldGen.isRunningDebugGen)
					{
						Debug.LogWarning("No where to put mobs possibleSpawnPoints [" + tc.node.node.Id + "]", null);
					}
					return null;
				}
				tc.LogInfo("PlaceAmbientMobs", "No biome MOBS", (float)node.node.Id);
				return null;
			}
			tc.LogInfo("PlaceAmbientMobs", "No tags", (float)node.node.Id);
			return null;
		}

		public static int MobWidthOffset(int occupiedCell, int widthIterator)
		{
			return Grid.OffsetCell(occupiedCell, (widthIterator % 2 != 0) ? (widthIterator / 2 + widthIterator % 2) : (-(widthIterator / 2)), 0);
		}

		private static bool isSuitableMobSpawnPoint(int cell, Mob mob, Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dc, ref HashSet<int> alreadyOccupiedCells)
		{
			for (int i = 0; i < mob.width; i++)
			{
				for (int j = 0; j < mob.height; j++)
				{
					int num = MobWidthOffset(cell, i);
					if (!Grid.IsValidCell(num) || !Grid.IsValidCell(Grid.CellAbove(num)) || !Grid.IsValidCell(Grid.CellBelow(num)))
					{
						return false;
					}
					if (alreadyOccupiedCells.Contains(num))
					{
						return false;
					}
				}
			}
			switch (mob.location)
			{
			case Mob.Location.Solid:
				return !isNaturalCavity(cell) && Grid.Solid[cell];
			case Mob.Location.Ceiling:
				return isNaturalCavity(cell) && !Grid.Solid[cell] && Grid.Solid[Grid.CellAbove(cell)] && !Grid.Solid[Grid.CellBelow(cell)] && !Grid.IsLiquid(cell);
			case Mob.Location.Floor:
				return isNaturalCavity(cell) && !Grid.Solid[cell] && !Grid.Solid[Grid.CellAbove(cell)] && Grid.Solid[Grid.CellBelow(cell)] && !Grid.IsLiquid(cell);
			case Mob.Location.Air:
				return !Grid.Solid[cell] && !Grid.Solid[Grid.CellAbove(cell)] && !Grid.IsLiquid(cell);
			case Mob.Location.Water:
				return (Grid.Element[cell].id == SimHashes.Water || Grid.Element[cell].id == SimHashes.DirtyWater) && (Grid.Element[Grid.CellAbove(cell)].id == SimHashes.Water || Grid.Element[Grid.CellAbove(cell)].id == SimHashes.DirtyWater);
			case Mob.Location.Surface:
			{
				bool flag = true;
				for (int k = 0; k < mob.width; k++)
				{
					int num2 = MobWidthOffset(cell, k);
					flag = (flag && Grid.Element[num2].id == SimHashes.Vacuum && Grid.Solid[Grid.CellBelow(num2)]);
				}
				return flag;
			}
			default:
				return isNaturalCavity(cell) && !Grid.Solid[cell];
			}
		}

		public static bool isNaturalCavity(int cell)
		{
			if (NaturalCavities != null)
			{
				if (!allNaturalCavityCells.Contains(cell))
				{
					return false;
				}
				return true;
			}
			return false;
		}

		public static void DetectNaturalCavities(WorldGen.OfflineCallbackFunction updateProgressFn)
		{
			updateProgressFn(UI.WORLDGEN.ANALYZINGWORLD.key, 0.8f, WorldGenProgressStages.Stages.DetectNaturalCavities);
			HashSet<int> invalidCells = new HashSet<int>();
			for (int i = 0; i < WorldGen.TerrainCells.Count; i++)
			{
				TerrainCell terrainCell = WorldGen.TerrainCells[i];
				float completePercent = (float)i / (float)WorldGen.TerrainCells.Count * 100f;
				updateProgressFn(UI.WORLDGEN.ANALYZINGWORLDCOMPLETE.key, completePercent, WorldGenProgressStages.Stages.DetectNaturalCavities);
				NaturalCavities.Add(terrainCell, new List<HashSet<int>>());
				invalidCells.Clear();
				List<int> allCells = terrainCell.GetAllCells();
				for (int j = 0; j < allCells.Count; j++)
				{
					int num = allCells[j];
					if (!Grid.Solid[num] && !invalidCells.Contains(num))
					{
						HashSet<int> hashSet = GameUtil.FloodCollectCells(num, (int checkCell) => !invalidCells.Contains(checkCell) && !Grid.Solid[checkCell], 300, invalidCells, true);
						if (hashSet != null && hashSet.Count > 0)
						{
							NaturalCavities[terrainCell].Add(hashSet);
							allNaturalCavityCells.UnionWith(hashSet);
						}
					}
				}
			}
			updateProgressFn(UI.WORLDGEN.ANALYZINGWORLDCOMPLETE.key, 100f, WorldGenProgressStages.Stages.DetectNaturalCavities);
		}
	}
}
