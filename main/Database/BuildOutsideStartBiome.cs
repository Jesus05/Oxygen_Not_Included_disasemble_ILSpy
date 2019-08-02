using Delaunay.Geo;
using Klei;
using ProcGen;
using System.IO;

namespace Database
{
	public class BuildOutsideStartBiome : ColonyAchievementRequirement
	{
		public override bool Success()
		{
			WorldDetailSave worldDetailSave = SaveLoader.Instance.worldDetailSave;
			for (int i = 0; i < worldDetailSave.overworldCells.Count; i++)
			{
				WorldDetailSave.OverworldCell overworldCell = worldDetailSave.overworldCells[i];
				if (overworldCell.tags != null && !overworldCell.tags.Contains(WorldGenTags.StartWorld))
				{
					Polygon poly = overworldCell.poly;
					foreach (BuildingComplete item in Components.BuildingCompletes.Items)
					{
						KPrefabID component = item.GetComponent<KPrefabID>();
						if (!component.HasTag(GameTags.TemplateBuilding) && poly.PointInPolygon(item.transform.GetPosition()))
						{
							Game.Instance.unlocks.Unlock("buildoutsidestartingbiome");
							return true;
						}
					}
				}
			}
			return false;
		}

		public override void Deserialize(IReader reader)
		{
		}

		public override void Serialize(BinaryWriter writer)
		{
		}
	}
}
