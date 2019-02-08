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

		public List<OverworldCell> overworldCells = null;

		public int globalWorldSeed = 0;

		public int globalWorldLayoutSeed = 0;

		public int globalTerrainSeed = 0;

		public int globalNoiseSeed = 0;

		public WorldDetailSave()
		{
			overworldCells = new List<OverworldCell>();
		}
	}
}
