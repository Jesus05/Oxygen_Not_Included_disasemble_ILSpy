using ProcGen;
using ProcGenGame;
using VoronoiTree;

namespace Klei
{
	public class TerrainCellLogged : TerrainCell
	{
		public TerrainCellLogged()
		{
		}

		public TerrainCellLogged(ProcGen.Node node, Diagram.Site site)
			: base(node, site)
		{
		}

		public override void LogInfo(string evt, string param, float value)
		{
		}
	}
}
