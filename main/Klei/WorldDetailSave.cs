using Delaunay.Geo;
using KSerialization;
using ProcGen;
using ProcGenGame;
using System.Collections.Generic;

namespace Klei
{
	public class WorldDetailSave
	{
		[SerializationConfig(MemberSerialization.OptOut)]
		public class OverworldCell
		{
			public Polygon poly;

			public SubWorld.ZoneType zoneType;

			public OverworldCell()
			{
			}

			public OverworldCell(SubWorld.ZoneType zoneType, TerrainCell tc)
			{
				poly = tc.poly;
				this.zoneType = zoneType;
			}
		}

		public List<OverworldCell> overworldCells;

		public int globalWorldSeed;

		public int globalWorldLayoutSeed;

		public int globalTerrainSeed;

		public int globalNoiseSeed;

		public WorldDetailSave()
		{
			overworldCells = new List<OverworldCell>();
		}
	}
}
