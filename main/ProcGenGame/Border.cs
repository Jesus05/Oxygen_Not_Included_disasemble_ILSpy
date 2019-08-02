using ProcGen;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGenGame
{
	public class Border : Path, SymbolicMapElement
	{
		public Neighbors neighbors;

		public List<WeightedSimHash> element;

		public float width;

		public Border(Neighbors neighbors, Vector2 e0, Vector2 e1)
		{
			this.neighbors = neighbors;
			AddSegment(e0, e1);
		}

		public Border(TerrainCell a, TerrainCell b, Vector2 e0, Vector2 e1)
		{
			Debug.Assert(a != null && b != null, "NULL neighbor for Border");
			neighbors.n0 = a;
			neighbors.n1 = b;
			AddSegment(e0, e1);
		}

		public void ConvertToMap(Chunk world, TerrainCell.SetValuesFunction SetValues, float temperatureMin, float temperatureRange, SeededRandom rnd)
		{
			Sim.DiseaseCell invalid = Sim.DiseaseCell.Invalid;
			for (int i = 0; i < pathElements.Count; i++)
			{
				Segment segment = pathElements[i];
				Vector2 e = segment.e1;
				Segment segment2 = pathElements[i];
				Vector2 vector = e - segment2.e0;
				Vector2 normalized = new Vector2(0f - vector.y, vector.x).normalized;
				Segment segment3 = pathElements[i];
				Vector2 e2 = segment3.e0;
				Segment segment4 = pathElements[i];
				List<Vector2I> line = ProcGen.Util.GetLine(e2, segment4.e1);
				for (int j = 0; j < line.Count; j++)
				{
					Vector2I vector2I = line[j];
					int x = vector2I.x;
					Vector2I vector2I2 = line[j];
					int num = Grid.XYToCell(x, vector2I2.y);
					if (Grid.IsValidCell(num))
					{
						Element element = ElementLoader.FindElementByName(WeightedRandom.Choose(this.element, rnd).element);
						Sim.PhysicsData defaultValues = element.defaultValues;
						defaultValues.temperature = temperatureMin + world.heatOffset[num] * temperatureRange;
						SetValues(num, element, defaultValues, invalid);
					}
					for (float num2 = 0.5f; num2 <= width; num2 += 1f)
					{
						Vector2 vector2 = (Vector2)line[j] + normalized * num2;
						num = Grid.XYToCell((int)vector2.x, (int)vector2.y);
						if (Grid.IsValidCell(num))
						{
							Element element2 = ElementLoader.FindElementByName(WeightedRandom.Choose(this.element, rnd).element);
							Sim.PhysicsData defaultValues2 = element2.defaultValues;
							defaultValues2.temperature = temperatureMin + world.heatOffset[num] * temperatureRange;
							SetValues(num, element2, defaultValues2, invalid);
						}
						Vector2 vector3 = (Vector2)line[j] - normalized * num2;
						num = Grid.XYToCell((int)vector3.x, (int)vector3.y);
						if (Grid.IsValidCell(num))
						{
							Element element3 = ElementLoader.FindElementByName(WeightedRandom.Choose(this.element, rnd).element);
							Sim.PhysicsData defaultValues3 = element3.defaultValues;
							defaultValues3.temperature = temperatureMin + world.heatOffset[num] * temperatureRange;
							SetValues(num, element3, defaultValues3, invalid);
						}
					}
				}
			}
		}
	}
}
