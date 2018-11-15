using System;
using System.Collections.Generic;
using System.Linq;

namespace MIConvexHull
{
	public static class VoronoiMesh
	{
		public static VoronoiMesh<TVertex, TCell, TEdge> Create<TVertex, TCell, TEdge>(IList<TVertex> data) where TVertex : IVertex where TCell : TriangulationCell<TVertex, TCell>, new()where TEdge : VoronoiEdge<TVertex, TCell>, new()
		{
			return VoronoiMesh<TVertex, TCell, TEdge>.Create(data);
		}

		public static VoronoiMesh<TVertex, DefaultTriangulationCell<TVertex>, VoronoiEdge<TVertex, DefaultTriangulationCell<TVertex>>> Create<TVertex>(IList<TVertex> data) where TVertex : IVertex
		{
			return VoronoiMesh<TVertex, DefaultTriangulationCell<TVertex>, VoronoiEdge<TVertex, DefaultTriangulationCell<TVertex>>>.Create(data);
		}

		public static VoronoiMesh<DefaultVertex, DefaultTriangulationCell<DefaultVertex>, VoronoiEdge<DefaultVertex, DefaultTriangulationCell<DefaultVertex>>> Create(IList<double[]> data)
		{
			List<DefaultVertex> data2 = (from p in data
			select new DefaultVertex
			{
				Position = p.ToArray()
			}).ToList();
			return VoronoiMesh<DefaultVertex, DefaultTriangulationCell<DefaultVertex>, VoronoiEdge<DefaultVertex, DefaultTriangulationCell<DefaultVertex>>>.Create(data2);
		}

		public static VoronoiMesh<TVertex, TCell, VoronoiEdge<TVertex, TCell>> Create<TVertex, TCell>(IList<TVertex> data) where TVertex : IVertex where TCell : TriangulationCell<TVertex, TCell>, new()
		{
			return VoronoiMesh<TVertex, TCell, VoronoiEdge<TVertex, TCell>>.Create(data);
		}
	}
	public class VoronoiMesh<TVertex, TCell, TEdge> where TVertex : IVertex where TCell : TriangulationCell<TVertex, TCell>, new()where TEdge : VoronoiEdge<TVertex, TCell>, new()
	{
		private class EdgeComparer : IEqualityComparer<TEdge>
		{
			public bool Equals(TEdge x, TEdge y)
			{
				return (x.Source == y.Source && x.Target == y.Target) || (x.Source == y.Target && x.Target == y.Source);
			}

			public int GetHashCode(TEdge obj)
			{
				TCell source = obj.Source;
				int hashCode = source.GetHashCode();
				TCell target = obj.Target;
				return hashCode ^ target.GetHashCode();
			}
		}

		public IEnumerable<TCell> Vertices
		{
			get;
			private set;
		}

		public IEnumerable<TEdge> Edges
		{
			get;
			private set;
		}

		private VoronoiMesh()
		{
		}

		public static VoronoiMesh<TVertex, TCell, TEdge> Create(IList<TVertex> data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			DelaunayTriangulation<TVertex, TCell> delaunayTriangulation = DelaunayTriangulation<TVertex, TCell>.Create(data);
			List<TCell> list = Enumerable.ToList<TCell>(delaunayTriangulation.Cells);
			HashSet<TEdge> hashSet = new HashSet<TEdge>(new EdgeComparer());
			foreach (TCell item2 in list)
			{
				TCell current = item2;
				for (int i = 0; i < current.Adjacency.Length; i++)
				{
					TCell val = current.Adjacency[i];
					if (val != null)
					{
						HashSet<TEdge> hashSet2 = hashSet;
						TEdge item = new TEdge();
						item.Source = current;
						item.Target = val;
						hashSet2.Add(item);
					}
				}
			}
			VoronoiMesh<TVertex, TCell, TEdge> voronoiMesh = new VoronoiMesh<TVertex, TCell, TEdge>();
			voronoiMesh.Vertices = list;
			voronoiMesh.Edges = Enumerable.ToList<TEdge>((IEnumerable<TEdge>)hashSet);
			return voronoiMesh;
		}
	}
}
