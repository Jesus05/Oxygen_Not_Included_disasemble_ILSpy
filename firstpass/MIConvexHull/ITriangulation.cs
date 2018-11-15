using System.Collections.Generic;

namespace MIConvexHull
{
	public interface ITriangulation<TVertex, TCell> where TVertex : IVertex where TCell : TriangulationCell<TVertex, TCell>, new()
	{
		IEnumerable<TCell> Cells
		{
			get;
		}
	}
}
