using ProcGen;
using ProcGenGame;
using System.Collections.Generic;
using VoronoiTree;

namespace Klei
{
	public class Data
	{
		public int globalWorldSeed;

		public int globalWorldLayoutSeed;

		public int globalTerrainSeed;

		public int globalNoiseSeed;

		public int chunkEdgeSize = 32;

		public Vector2I subWorldSize = new Vector2I(512, 256);

		public WorldLayout worldLayout;

		public List<TerrainCell> terrainCells;

		public List<TerrainCell> overworldCells;

		public List<ProcGen.River> rivers;

		public GameSpawnData gameSpawnData;

		public Chunk world;

		public Tree voronoiTree;

		public Data()
		{
			worldLayout = new WorldLayout(0);
			terrainCells = new List<TerrainCell>();
			overworldCells = new List<TerrainCell>();
			rivers = new List<ProcGen.River>();
			gameSpawnData = new GameSpawnData();
			world = new Chunk();
			voronoiTree = new Tree(0);
		}
	}
}
