using System;
using System.Collections.Generic;

namespace MIConvexHull
{
	public class DelaunayTriangulation<TVertex, TCell> : ITriangulation<TVertex, TCell> where TVertex : IVertex where TCell : TriangulationCell<TVertex, TCell>, new()
	{
		public IEnumerable<TCell> Cells
		{
			get;
			private set;
		}

		private DelaunayTriangulation()
		{
		}

		public static DelaunayTriangulation<TVertex, TCell> Create(IList<TVertex> data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			DelaunayTriangulation<TVertex, TCell> delaunayTriangulation2;
			if (data.Count != 0)
			{
				TCell[] delaunayTriangulation = ConvexHullAlgorithm.GetDelaunayTriangulation<TVertex, TCell>(data);
				delaunayTriangulation2 = new DelaunayTriangulation<TVertex, TCell>();
				delaunayTriangulation2.Cells = delaunayTriangulation;
				return delaunayTriangulation2;
			}
			delaunayTriangulation2 = new DelaunayTriangulation<TVertex, TCell>();
			delaunayTriangulation2.Cells = new TCell[0];
			return delaunayTriangulation2;
		}
	}
}
