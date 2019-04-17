using Delaunay.Geo;
using KSerialization;
using ProcGen;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using VoronoiTree;

namespace ProcGenGame
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class TerrainCell
	{
		public delegate void SetValuesFunction(int index, object elem, Sim.PhysicsData pd, Sim.DiseaseCell dc);

		public struct ElementOverride
		{
			public Element element;

			public Sim.PhysicsData pdelement;

			public Sim.DiseaseCell dc;

			public float mass;

			public float temperature;

			public byte diseaseIdx;

			public int diseaseAmount;

			public bool overrideMass;

			public bool overrideTemperature;

			public bool overrideDiseaseIdx;

			public bool overrideDiseaseAmount;
		}

		public List<KeyValuePair<int, Tag>> terrainPositions;

		public List<KeyValuePair<int, Tag>> poi;

		public List<uint> terrain_neighbors_idx = new List<uint>();

		private float finalSize;

		private bool debugMode;

		private List<int> allCells;

		private HashSet<Vector2I> availableTerrainPoints;

		private List<Vector2I> centerPoints;

		private List<List<Vector2I>> borders;

		private HashSet<Vector2I> availableSpawnPoints;

		private static HashSet<int> claimedCells = new HashSet<int>();

		public const int DONT_SET_TEMPERATURE_DEFAULTS = -1;

		private static readonly Tag[] noSpawnTags = new Tag[6]
		{
			WorldGenTags.StartLocation,
			WorldGenTags.NearStartLocation,
			WorldGenTags.POI,
			WorldGenTags.AtEdge,
			WorldGenTags.AtDepths,
			WorldGenTags.AtSurface
		};

		private static readonly TagSet noSpawnTagSet = new TagSet(noSpawnTags);

		public Polygon poly => site.poly;

		[Serialize]
		public ProcGen.Node node
		{
			get;
			private set;
		}

		[Serialize]
		public Diagram.Site site
		{
			get;
			private set;
		}

		public bool HasMobs => mobs != null && mobs.Count > 0;

		public List<KeyValuePair<int, Tag>> mobs
		{
			get;
			private set;
		}

		protected TerrainCell()
		{
		}

		protected TerrainCell(ProcGen.Node node, Diagram.Site site)
		{
			this.node = node;
			this.site = site;
			this.node.SetPosition(site.position);
		}

		public void SetNode(ProcGen.Node newNode)
		{
			node = newNode;
		}

		public virtual void LogInfo(string evt, string param, float value)
		{
			Debug.Log(evt + ":" + param + "=" + value);
		}

		public static void ClearClaimedCells()
		{
			claimedCells.Clear();
		}

		public List<int> GetAllCells()
		{
			if (allCells == null)
			{
				allCells = new List<int>();
				availableTerrainPoints = new HashSet<Vector2I>();
				availableSpawnPoints = new HashSet<Vector2I>();
				for (int i = 0; i < Grid.HeightInCells; i++)
				{
					for (int j = 0; j < Grid.WidthInCells; j++)
					{
						if (poly.Contains(new Vector2((float)j, (float)i)))
						{
							int num = Grid.XYToCell(j, i);
							availableTerrainPoints.Add(Grid.CellToXY(num));
							availableSpawnPoints.Add(Grid.CellToXY(num));
							if (claimedCells.Add(num))
							{
								allCells.Add(num);
							}
						}
					}
				}
				LogInfo("Initialise cells", string.Empty, (float)allCells.Count);
			}
			return allCells;
		}

		public List<int> GetAvailableSpawnCells()
		{
			List<int> list = new List<int>();
			foreach (Vector2I availableSpawnPoint in availableSpawnPoints)
			{
				Vector2I current = availableSpawnPoint;
				list.Add(Grid.XYToCell(current.x, current.y));
			}
			return list;
		}

		public List<int> GetAvailableTerrainCells()
		{
			List<int> list = new List<int>();
			foreach (Vector2I availableTerrainPoint in availableTerrainPoints)
			{
				Vector2I current = availableTerrainPoint;
				list.Add(Grid.XYToCell(current.x, current.y));
			}
			return list;
		}

		private bool RemoveFromAvailableSpawnCells(int cell)
		{
			Grid.CellToXY(cell, out int x, out int y);
			Vector2I item = new Vector2I(x, y);
			return availableSpawnPoints.Remove(item);
		}

		public void AddMobs(IEnumerable<KeyValuePair<int, Tag>> newMobs)
		{
			foreach (KeyValuePair<int, Tag> newMob in newMobs)
			{
				AddMob(newMob);
			}
		}

		private void AddMob(int cellIdx, string tag)
		{
			AddMob(new KeyValuePair<int, Tag>(cellIdx, new Tag(tag)));
		}

		public void AddMob(KeyValuePair<int, Tag> mob)
		{
			if (mobs == null)
			{
				mobs = new List<KeyValuePair<int, Tag>>();
			}
			mobs.Add(mob);
			bool flag = RemoveFromAvailableSpawnCells(mob.Key);
			LogInfo("\t\tRemoveFromAvailableCells", mob.Value.Name + ": " + ((!flag) ? "failed" : "success"), (float)mob.Key);
			if (!flag)
			{
				if (!allCells.Contains(mob.Key))
				{
					Debug.Assert(false, "Couldnt find cell [" + mob.Key + "] we dont own, to remove for mob [" + mob.Value.Name + "]");
				}
				else
				{
					Debug.Assert(false, "Couldnt find cell [" + mob.Key + "] to remove for mob [" + mob.Value.Name + "]");
				}
			}
		}

		protected string GetSubWorldType(WorldGen worldGen)
		{
			Vector2 vector = site.poly.Centroid();
			int a = (int)vector.x;
			Vector2 vector2 = site.poly.Centroid();
			Vector2I pos = new Vector2I(a, (int)vector2.y);
			return worldGen.GetSubWorldType(pos);
		}

		protected Temperature.Range GetTemperatureRange(WorldGen worldGen)
		{
			string subWorldType = GetSubWorldType(worldGen);
			if (subWorldType == null)
			{
				return Temperature.Range.Mild;
			}
			if (!worldGen.Settings.GetSubWorlds().ContainsKey(subWorldType))
			{
				return Temperature.Range.Mild;
			}
			return worldGen.Settings.GetSubWorld(subWorldType).temperatureRange;
		}

		protected void GetTemperatureRange(WorldGen worldGen, ref float min, ref float range)
		{
			Temperature.Range temperatureRange = GetTemperatureRange(worldGen);
			min = SettingsCache.temperatures.ranges[temperatureRange].min;
			range = SettingsCache.temperatures.ranges[temperatureRange].max - min;
		}

		protected float GetDensityMassForCell(Chunk world, int cellIdx, float mass)
		{
			if (!Grid.IsValidCell(cellIdx))
			{
				return 0f;
			}
			Debug.Assert(world.density[cellIdx] >= 0f && world.density[cellIdx] <= 1f, "Density [" + world.density[cellIdx] + "] out of range [0-1]");
			float num = world.density[cellIdx] - 0.5f;
			float num2 = mass + mass * num;
			if (num2 > 10000f)
			{
				num2 = 10000f;
			}
			return num2;
		}

		private void HandleSprinkleOfElement(WorldGenSettings settings, Tag targetTag, Chunk world, SetValuesFunction SetValues, float temperatureMin, float temperatureRange, SeededRandom rnd)
		{
			FeatureSettings feature = SettingsCache.GetFeature(targetTag.Name);
			string element = feature.GetOneWeightedSimHash("SprinkleOfElementChoices", rnd).element;
			Element element2 = ElementLoader.FindElementByName(element);
			SampleDescriber desription = SettingsCache.rooms.GetDesription(targetTag);
			Sim.PhysicsData defaultValues = element2.defaultValues;
			Sim.DiseaseCell invalid = Sim.DiseaseCell.Invalid;
			for (int i = 0; i < terrainPositions.Count; i++)
			{
				if (!(terrainPositions[i].Value != targetTag))
				{
					float radius = rnd.RandomRange(desription.blobSize.min, desription.blobSize.max);
					Vector2 center = Grid.CellToPos2D(terrainPositions[i].Key);
					List<Vector2I> filledCircle = ProcGen.Util.GetFilledCircle(center, radius);
					for (int j = 0; j < filledCircle.Count; j++)
					{
						Vector2I vector2I = filledCircle[j];
						int x = vector2I.x;
						Vector2I vector2I2 = filledCircle[j];
						int num = Grid.XYToCell(x, vector2I2.y);
						if (Grid.IsValidCell(num))
						{
							defaultValues.mass = GetDensityMassForCell(world, num, element2.defaultValues.mass);
							defaultValues.temperature = temperatureMin + world.heatOffset[num] * temperatureRange;
							SetValues(num, element2, defaultValues, invalid);
						}
					}
				}
			}
		}

		private HashSet<Vector2I> DigFeature(ProcGen.Room.Shape shape, float size, List<int> bordersWidths, SeededRandom rnd)
		{
			HashSet<Vector2I> hashSet = new HashSet<Vector2I>();
			if (size < 1f)
			{
				return hashSet;
			}
			Vector2 center = site.poly.Centroid();
			finalSize = size;
			switch (shape)
			{
			case ProcGen.Room.Shape.Blob:
				centerPoints = ProcGen.Util.GetBlob(center, finalSize, rnd.RandomSource());
				break;
			case ProcGen.Room.Shape.Circle:
				centerPoints = ProcGen.Util.GetFilledCircle(center, finalSize);
				break;
			case ProcGen.Room.Shape.Square:
				centerPoints = ProcGen.Util.GetFilledRectangle(center, finalSize, finalSize, rnd, 2f, 2f);
				break;
			case ProcGen.Room.Shape.TallThin:
				centerPoints = ProcGen.Util.GetFilledRectangle(center, finalSize / 4f, finalSize, rnd, 2f, 2f);
				break;
			case ProcGen.Room.Shape.ShortWide:
				centerPoints = ProcGen.Util.GetFilledRectangle(center, finalSize, finalSize / 4f, rnd, 2f, 2f);
				break;
			}
			if (centerPoints.Count == 0)
			{
				Debug.LogWarning("Room has no centerpoints. Terrain Cell [ shape: " + shape.ToString() + " size: " + finalSize + "] [" + node.node.Id + " " + node.type + " " + node.position + "]");
			}
			else if (bordersWidths != null && bordersWidths.Count > 0 && bordersWidths[0] > 0)
			{
				borders = new List<List<Vector2I>>();
				hashSet.UnionWith(new HashSet<Vector2I>(centerPoints));
				for (int i = 0; i < bordersWidths.Count && bordersWidths[i] > 0; i++)
				{
					borders.Add(ProcGen.Util.GetBorder(hashSet, bordersWidths[i]));
					hashSet.UnionWith(borders[i]);
				}
			}
			return hashSet;
		}

		public static ElementOverride GetElementOverride(string element, SampleDescriber.Override overrides)
		{
			Debug.Assert(element != null && element.Length > 0);
			ElementOverride result = default(ElementOverride);
			result.element = ElementLoader.FindElementByName(element);
			result.pdelement = result.element.defaultValues;
			result.dc = Sim.DiseaseCell.Invalid;
			result.mass = result.element.defaultValues.mass;
			result.temperature = result.element.defaultValues.temperature;
			if (overrides == null)
			{
				return result;
			}
			result.overrideMass = false;
			result.overrideTemperature = false;
			result.overrideDiseaseIdx = false;
			result.overrideDiseaseAmount = false;
			if (overrides.massMultiplier.HasValue)
			{
				result.mass *= overrides.massMultiplier.Value;
				result.overrideMass = true;
			}
			if (overrides.massOverride.HasValue)
			{
				result.mass = overrides.massOverride.Value;
				result.overrideMass = true;
			}
			if (overrides.temperatureMultiplier.HasValue)
			{
				result.temperature *= overrides.temperatureMultiplier.Value;
				result.overrideTemperature = true;
			}
			if (overrides.temperatureOverride.HasValue)
			{
				result.temperature = overrides.temperatureOverride.Value;
				result.overrideTemperature = true;
			}
			if (overrides.diseaseOverride != null)
			{
				result.diseaseIdx = (byte)WorldGen.GetDiseaseIdx(overrides.diseaseOverride);
				result.overrideDiseaseIdx = true;
			}
			if (overrides.diseaseAmountOverride.HasValue)
			{
				result.diseaseAmount = overrides.diseaseAmountOverride.Value;
				result.overrideDiseaseAmount = true;
			}
			if (result.overrideTemperature)
			{
				result.pdelement.temperature = result.temperature;
			}
			if (result.overrideMass)
			{
				result.pdelement.mass = result.mass;
			}
			if (result.overrideDiseaseIdx)
			{
				result.dc.diseaseIdx = result.diseaseIdx;
			}
			if (result.overrideDiseaseAmount)
			{
				result.dc.elementCount = result.diseaseAmount;
			}
			return result;
		}

		private void ApplyPlaceElementForRoom(FeatureSettings feature, string group, List<Vector2I> cells, Chunk world, SetValuesFunction SetValues, float temperatureMin, float temperatureRange, SeededRandom rnd)
		{
			if (cells != null && cells.Count != 0 && feature.HasGroup(group))
			{
				switch (feature.ElementChoiceGroups[group].selectionMethod)
				{
				case ProcGen.Room.Selection.WeightedResample:
					for (int j = 0; j < cells.Count; j++)
					{
						Vector2I vector2I3 = cells[j];
						int x2 = vector2I3.x;
						Vector2I vector2I4 = cells[j];
						int num2 = Grid.XYToCell(x2, vector2I4.y);
						if (Grid.IsValidCell(num2))
						{
							WeightedSimHash oneWeightedSimHash2 = feature.GetOneWeightedSimHash(group, rnd);
							ElementOverride elementOverride2 = GetElementOverride(oneWeightedSimHash2.element, oneWeightedSimHash2.overrides);
							if (!elementOverride2.overrideTemperature)
							{
								elementOverride2.pdelement.temperature = temperatureMin + world.heatOffset[num2] * temperatureRange;
							}
							if (!elementOverride2.overrideMass)
							{
								elementOverride2.pdelement.mass = GetDensityMassForCell(world, num2, elementOverride2.mass);
							}
							SetValues(num2, elementOverride2.element, elementOverride2.pdelement, elementOverride2.dc);
						}
					}
					break;
				default:
					for (int i = 0; i < cells.Count; i++)
					{
						Vector2I vector2I = cells[i];
						int x = vector2I.x;
						Vector2I vector2I2 = cells[i];
						int num = Grid.XYToCell(x, vector2I2.y);
						if (Grid.IsValidCell(num))
						{
							WeightedSimHash oneWeightedSimHash = feature.GetOneWeightedSimHash(group, rnd);
							ElementOverride elementOverride = GetElementOverride(oneWeightedSimHash.element, oneWeightedSimHash.overrides);
							if (!elementOverride.overrideTemperature)
							{
								elementOverride.pdelement.temperature = temperatureMin + world.heatOffset[num] * temperatureRange;
							}
							if (!elementOverride.overrideMass)
							{
								elementOverride.pdelement.mass = GetDensityMassForCell(world, num, elementOverride.mass);
							}
							SetValues(num, elementOverride.element, elementOverride.pdelement, elementOverride.dc);
						}
					}
					break;
				}
			}
		}

		private int GetIndexForLocation(List<Vector2I> points, Mob.Location location, SeededRandom rnd)
		{
			int num = -1;
			if (points == null || points.Count == 0)
			{
				return num;
			}
			if (location == Mob.Location.Air || location == Mob.Location.Solid)
			{
				return rnd.RandomRange(0, points.Count);
			}
			for (int i = 0; i < points.Count; i++)
			{
				Vector2I vector2I = points[i];
				int x = vector2I.x;
				Vector2I vector2I2 = points[i];
				int cell = Grid.XYToCell(x, vector2I2.y);
				if (Grid.IsValidCell(cell))
				{
					if (num == -1)
					{
						num = i;
					}
					else
					{
						switch (location)
						{
						case Mob.Location.Ceiling:
						{
							Vector2I vector2I5 = points[i];
							int y2 = vector2I5.y;
							Vector2I vector2I6 = points[num];
							if (y2 > vector2I6.y)
							{
								num = i;
							}
							break;
						}
						case Mob.Location.Floor:
						{
							Vector2I vector2I3 = points[i];
							int y = vector2I3.y;
							Vector2I vector2I4 = points[num];
							if (y < vector2I4.y)
							{
								num = i;
							}
							break;
						}
						}
					}
				}
			}
			return num;
		}

		private void PlaceMobInRoom(List<MobReference> mobTags, List<Vector2I> points, SeededRandom rnd)
		{
			if (points != null)
			{
				if (mobs == null)
				{
					mobs = new List<KeyValuePair<int, Tag>>();
				}
				for (int i = 0; i < mobTags.Count; i++)
				{
					if (!SettingsCache.mobs.HasMob(mobTags[i].type))
					{
						Debug.LogError("Missing sample description for tag [" + mobTags[i].type + "]");
					}
					else
					{
						Mob mob = SettingsCache.mobs.GetMob(mobTags[i].type);
						int num = Mathf.RoundToInt(mobTags[i].count.GetRandomValueWithinRange(rnd));
						for (int j = 0; j < num; j++)
						{
							int indexForLocation = GetIndexForLocation(points, mob.location, rnd);
							if (indexForLocation == -1)
							{
								break;
							}
							if (points.Count <= indexForLocation)
							{
								return;
							}
							Vector2I vector2I = points[indexForLocation];
							int x = vector2I.x;
							Vector2I vector2I2 = points[indexForLocation];
							int cellIdx = Grid.XYToCell(x, vector2I2.y);
							points.RemoveAt(indexForLocation);
							AddMob(cellIdx, mobTags[i].type);
						}
					}
				}
			}
		}

		private int[] ConvertNoiseToPoints(float[] basenoise, float minThreshold = 0.9f, float maxThreshold = 1f)
		{
			if (basenoise == null)
			{
				return null;
			}
			List<int> list = new List<int>();
			float width = site.poly.bounds.width;
			float height = site.poly.bounds.height;
			for (float num = site.position.y - height / 2f; num < site.position.y + height / 2f; num += 1f)
			{
				for (float num2 = site.position.x - width / 2f; num2 < site.position.x + width / 2f; num2 += 1f)
				{
					int num3 = Grid.PosToCell(new Vector2(num2, num));
					if (site.poly.Contains(new Vector2(num2, num)))
					{
						float num4 = (float)(int)basenoise[num3];
						if (!(num4 < minThreshold) && !(num4 > maxThreshold) && !list.Contains(num3))
						{
							list.Add(Grid.PosToCell(new Vector2(num2, num)));
						}
					}
				}
			}
			return list.ToArray();
		}

		private void ApplyForeground(Chunk world, SetValuesFunction SetValues, float temperatureMin, float temperatureRange, SeededRandom rnd)
		{
			LogInfo("Apply foregreound", (node.tags != null).ToString(), (float)((node.tags != null) ? node.tags.Count : 0));
			if (node.tags != null)
			{
				FeatureSettings feature = SettingsCache.GetFeature(node.type);
				LogInfo("\tFeature?", (feature != null).ToString(), 0f);
				if (feature == null && node.tags != null)
				{
					List<Tag> list = new List<Tag>();
					foreach (Tag tag2 in node.tags)
					{
						FeatureSettings feature2 = SettingsCache.GetFeature(tag2.Name);
						if (feature2 != null)
						{
							list.Add(tag2);
						}
					}
					LogInfo("\tNo feature, checking possible feature tags, found", string.Empty, (float)list.Count);
					if (list.Count > 0)
					{
						Tag tag = list[rnd.RandomSource().Next(list.Count)];
						feature = SettingsCache.GetFeature(tag.Name);
						LogInfo("\tPicked feature", tag.Name, 0f);
					}
				}
				if (feature != null)
				{
					LogInfo("APPLY FOREGROUND", node.type, 0f);
					float num = feature.blobSize.GetRandomValueWithinRange(rnd);
					float num2 = poly.DistanceToClosestEdge(null);
					if (!node.tags.Contains(WorldGenTags.AllowExceedNodeBorders) && num2 < num)
					{
						if (debugMode)
						{
							Debug.LogWarning(node.type + " " + feature.shape + "  blob size too large to fit in node. Size reduced. " + num + "->" + (num2 - 6f).ToString());
						}
						num = num2 - 6f;
					}
					if (!(num <= 0f))
					{
						HashSet<Vector2I> hashSet = DigFeature(feature.shape, num, feature.borders, rnd);
						availableTerrainPoints.ExceptWith(hashSet);
						LogInfo("\t\t", "claimed points", (float)hashSet.Count);
						ApplyPlaceElementForRoom(feature, "RoomCenterElements", centerPoints, world, SetValues, temperatureMin, temperatureRange, rnd);
						if (borders != null)
						{
							for (int i = 0; i < borders.Count; i++)
							{
								ApplyPlaceElementForRoom(feature, "RoomBorderChoices" + i, borders[i], world, SetValues, temperatureMin, temperatureRange, rnd);
							}
						}
					}
				}
			}
		}

		private void ApplyBackground(WorldGen worldGen, Chunk world, SetValuesFunction SetValues, float temperatureMin, float temperatureRange, SeededRandom rnd)
		{
			float floatSetting = worldGen.Settings.GetFloatSetting("CaveOverrideMaxValue");
			float floatSetting2 = worldGen.Settings.GetFloatSetting("CaveOverrideSliverValue");
			Leaf leafForTerrainCell = worldGen.GetLeafForTerrainCell(this);
			bool flag = leafForTerrainCell.tags.Contains(WorldGenTags.IgnoreCaveOverride);
			bool flag2 = leafForTerrainCell.tags.Contains(WorldGenTags.CaveVoidSliver);
			bool flag3 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToCentroid);
			bool flag4 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToCentroidInv);
			bool flag5 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToEdge);
			bool flag6 = leafForTerrainCell.tags.Contains(WorldGenTags.ErodePointToEdgeInv);
			bool flag7 = leafForTerrainCell.tags.Contains(WorldGenTags.DistFunctionPointCentroid);
			bool flag8 = leafForTerrainCell.tags.Contains(WorldGenTags.DistFunctionPointEdge);
			Sim.DiseaseCell diseaseCell = default(Sim.DiseaseCell);
			diseaseCell.diseaseIdx = byte.MaxValue;
			if (node.tags.Contains(WorldGenTags.Infected))
			{
				diseaseCell.diseaseIdx = (byte)rnd.RandomRange(0, WorldGen.diseaseIds.Count);
				node.tags.Add(new Tag("Infected:" + WorldGen.diseaseIds[diseaseCell.diseaseIdx]));
				diseaseCell.elementCount = rnd.RandomRange(10000, 1000000);
			}
			foreach (Vector2I availableTerrainPoint in availableTerrainPoints)
			{
				Vector2I current = availableTerrainPoint;
				int num = Grid.XYToCell(current.x, current.y);
				float num2 = world.overrides[num];
				if (!flag && num2 >= 100f)
				{
					if (num2 >= 300f)
					{
						SetValues(num, WorldGen.voidElement, WorldGen.voidElement.defaultValues, Sim.DiseaseCell.Invalid);
					}
					else if (num2 >= 200f)
					{
						SetValues(num, WorldGen.unobtaniumElement, WorldGen.unobtaniumElement.defaultValues, Sim.DiseaseCell.Invalid);
					}
					else
					{
						SetValues(num, WorldGen.katairiteElement, WorldGen.katairiteElement.defaultValues, Sim.DiseaseCell.Invalid);
					}
				}
				else
				{
					float num3 = 1f;
					Vector2 vector = new Vector2((float)current.x, (float)current.y);
					if (flag3 || flag4)
					{
						float num4 = 15f;
						if (flag8)
						{
							float timeOnEdge = 0f;
							MathUtil.Pair<Vector2, Vector2> closestEdge = poly.GetClosestEdge(vector, ref timeOnEdge);
							Vector2 a = closestEdge.First + (closestEdge.Second - closestEdge.First) * timeOnEdge;
							num4 = Vector2.Distance(a, vector);
						}
						num3 = Vector2.Distance(poly.Centroid(), vector) / num4;
						num3 = Mathf.Max(0f, Mathf.Min(1f, num3));
						if (flag4)
						{
							num3 = 1f - num3;
						}
					}
					if (flag6 || flag5)
					{
						float timeOnEdge2 = 0f;
						MathUtil.Pair<Vector2, Vector2> closestEdge2 = poly.GetClosestEdge(vector, ref timeOnEdge2);
						Vector2 a2 = closestEdge2.First + (closestEdge2.Second - closestEdge2.First) * timeOnEdge2;
						float num5 = 15f;
						if (flag7)
						{
							num5 = Vector2.Distance(poly.Centroid(), vector);
						}
						num3 = Vector2.Distance(a2, vector) / num5;
						num3 = Mathf.Max(0f, Mathf.Min(1f, num3));
						if (flag6)
						{
							num3 = 1f - num3;
						}
					}
					worldGen.GetElementForBiome(world, node.type, current, out Element element, out Sim.PhysicsData pd, out Sim.DiseaseCell dc, num3);
					if (!element.IsVacuum && element.id != SimHashes.Katairite && element.id != SimHashes.Unobtanium)
					{
						if (element.lowTempTransition != null && temperatureMin < element.lowTemp)
						{
							temperatureMin = element.lowTemp + 20f;
						}
						pd.temperature = temperatureMin + world.heatOffset[num] * temperatureRange;
					}
					if (element.IsSolid)
					{
						pd.mass = GetDensityMassForCell(world, num, pd.mass);
						if (!flag && num2 > floatSetting && num2 < 100f)
						{
							element = ((!flag2 || !(num2 > floatSetting2)) ? WorldGen.vacuumElement : WorldGen.voidElement);
							pd = element.defaultValues;
						}
					}
					if (dc.diseaseIdx == 255)
					{
						dc = diseaseCell;
					}
					SetValues(num, element, pd, dc);
				}
			}
			if (node.tags.Contains(WorldGenTags.SprinkleOfOxyRock))
			{
				HandleSprinkleOfElement(worldGen.Settings, WorldGenTags.SprinkleOfOxyRock, world, SetValues, temperatureMin, temperatureRange, rnd);
			}
			if (node.tags.Contains(WorldGenTags.SprinkleOfMetal))
			{
				HandleSprinkleOfElement(worldGen.Settings, WorldGenTags.SprinkleOfMetal, world, SetValues, temperatureMin, temperatureRange, rnd);
			}
		}

		private void GenerateActionCells(Tag tag, HashSet<Vector2I> possiblePoints, SeededRandom rnd)
		{
			ProcGen.Room desription = SettingsCache.rooms.GetDesription(tag);
			SampleDescriber sampleDescriber = desription;
			if (sampleDescriber == null && SettingsCache.mobs.GetMobTags().Contains(tag))
			{
				sampleDescriber = SettingsCache.mobs.GetMob(tag.Name);
			}
			if (sampleDescriber != null)
			{
				HashSet<Vector2I> hashSet = new HashSet<Vector2I>();
				float randomValueWithinRange = sampleDescriber.density.GetRandomValueWithinRange(rnd);
				List<Vector2> list;
				switch (sampleDescriber.selectMethod)
				{
				case SampleDescriber.PointSelectionMethod.RandomPoints:
					list = PointGenerator.GetRandomPoints(poly, randomValueWithinRange, 0f, null, sampleDescriber.sampleBehaviour, true, rnd, true, true);
					break;
				default:
					list = new List<Vector2>();
					list.Add(node.position);
					break;
				}
				foreach (Vector2 item2 in list)
				{
					Vector2 current = item2;
					Vector2I item = new Vector2I((int)current.x, (int)current.y);
					if (possiblePoints.Contains(item))
					{
						hashSet.Add(item);
					}
				}
				if (desription != null && desription.mobselection == ProcGen.Room.Selection.None)
				{
					if (terrainPositions == null)
					{
						terrainPositions = new List<KeyValuePair<int, Tag>>();
					}
					foreach (Vector2I item3 in hashSet)
					{
						Vector2I current2 = item3;
						int num = Grid.XYToCell(current2.x, current2.y);
						if (Grid.IsValidCell(num))
						{
							terrainPositions.Add(new KeyValuePair<int, Tag>(num, tag));
						}
					}
				}
			}
		}

		private void DoProcess(WorldGen worldGen, Chunk world, SetValuesFunction SetValues, SeededRandom rnd)
		{
			float min = 265f;
			float range = 30f;
			GetAllCells();
			GetTemperatureRange(worldGen, ref min, ref range);
			ApplyForeground(world, SetValues, min, range, rnd);
			for (int i = 0; i < node.tags.Count; i++)
			{
				GenerateActionCells(node.tags[i], availableTerrainPoints, rnd);
			}
			ApplyBackground(worldGen, world, SetValues, min, range, rnd);
		}

		public void Process(WorldGen worldGen, Sim.Cell[] cells, float[] bgTemp, Sim.DiseaseCell[] dcs, Chunk world, SeededRandom rnd)
		{
			SetValuesFunction setValues = delegate(int index, object elem, Sim.PhysicsData pd, Sim.DiseaseCell dc)
			{
				if (Grid.IsValidCell(index))
				{
					if (pd.temperature == 0f || (elem as Element).HasTag(GameTags.Special))
					{
						bgTemp[index] = -1f;
					}
					cells[index].SetValues(elem as Element, pd, ElementLoader.elements);
					dcs[index] = dc;
				}
				else
				{
					Debug.LogError("Process::SetValuesFunction Index [" + index + "] is not valid. cells.Length [" + cells.Length + "]");
				}
			};
			DoProcess(worldGen, world, setValues, rnd);
		}

		public void Process(WorldGen worldGen, Chunk world, SeededRandom rnd)
		{
			SetValuesFunction setValues = delegate(int index, object elem, Sim.PhysicsData pd, Sim.DiseaseCell dc)
			{
				SimMessages.ModifyCell(index, ElementLoader.GetElementIndex((elem as Element).id), pd.temperature, pd.mass, dc.diseaseIdx, dc.elementCount, SimMessages.ReplaceType.Replace, false, -1);
			};
			DoProcess(worldGen, world, setValues, rnd);
		}

		[OnDeserializing]
		internal void OnDeserializingMethod()
		{
			node = new ProcGen.Node();
			site = new Diagram.Site();
		}

		public bool IsSafeToSpawnFeatureTemplate()
		{
			return !node.tags.ContainsOne(noSpawnTagSet);
		}
	}
}
