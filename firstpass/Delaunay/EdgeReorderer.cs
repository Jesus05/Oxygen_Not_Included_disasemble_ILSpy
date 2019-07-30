using Delaunay.LR;
using Delaunay.Utils;
using System.Collections.Generic;

namespace Delaunay
{
	internal sealed class EdgeReorderer : IDisposable
	{
		private List<Edge> _edges;

		private List<Side> _edgeOrientations;

		public List<Edge> edges => _edges;

		public List<Side> edgeOrientations => _edgeOrientations;

		public EdgeReorderer(List<Edge> origEdges, VertexOrSite criterion)
		{
			_edges = new List<Edge>();
			_edgeOrientations = new List<Side>();
			if (origEdges.Count > 0)
			{
				_edges = ReorderEdges(origEdges, criterion);
			}
		}

		public void Dispose()
		{
			_edges = null;
			_edgeOrientations = null;
		}

		private List<Edge> ReorderEdges(List<Edge> origEdges, VertexOrSite criterion)
		{
			int count = origEdges.Count;
			bool[] array = new bool[count];
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				array[i] = false;
			}
			List<Edge> list = new List<Edge>();
			int num2 = 0;
			Edge edge = origEdges[num2];
			list.Add(edge);
			_edgeOrientations.Add(Side.LEFT);
			object obj;
			if (criterion == VertexOrSite.VERTEX)
			{
				ICoord leftVertex = edge.leftVertex;
				obj = leftVertex;
			}
			else
			{
				obj = edge.leftSite;
			}
			ICoord coord = (ICoord)obj;
			object obj2;
			if (criterion == VertexOrSite.VERTEX)
			{
				ICoord leftVertex = edge.rightVertex;
				obj2 = leftVertex;
			}
			else
			{
				obj2 = edge.rightSite;
			}
			ICoord coord2 = (ICoord)obj2;
			if (coord == Vertex.VERTEX_AT_INFINITY || coord2 == Vertex.VERTEX_AT_INFINITY)
			{
				return new List<Edge>();
			}
			array[num2] = true;
			num++;
			while (num < count)
			{
				for (num2 = 1; num2 < count; num2++)
				{
					if (!array[num2])
					{
						edge = origEdges[num2];
						object obj3;
						if (criterion == VertexOrSite.VERTEX)
						{
							ICoord leftVertex = edge.leftVertex;
							obj3 = leftVertex;
						}
						else
						{
							obj3 = edge.leftSite;
						}
						ICoord coord3 = (ICoord)obj3;
						object obj4;
						if (criterion == VertexOrSite.VERTEX)
						{
							ICoord leftVertex = edge.rightVertex;
							obj4 = leftVertex;
						}
						else
						{
							obj4 = edge.rightSite;
						}
						ICoord coord4 = (ICoord)obj4;
						if (coord3 == Vertex.VERTEX_AT_INFINITY || coord4 == Vertex.VERTEX_AT_INFINITY)
						{
							return new List<Edge>();
						}
						if (coord3 == coord2)
						{
							coord2 = coord4;
							_edgeOrientations.Add(Side.LEFT);
							list.Add(edge);
							array[num2] = true;
						}
						else if (coord4 == coord)
						{
							coord = coord3;
							_edgeOrientations.Insert(0, Side.LEFT);
							list.Insert(0, edge);
							array[num2] = true;
						}
						else if (coord3 == coord)
						{
							coord = coord4;
							_edgeOrientations.Insert(0, Side.RIGHT);
							list.Insert(0, edge);
							array[num2] = true;
						}
						else if (coord4 == coord2)
						{
							coord2 = coord3;
							_edgeOrientations.Add(Side.RIGHT);
							list.Add(edge);
							array[num2] = true;
						}
						if (array[num2])
						{
							num++;
						}
					}
				}
			}
			return list;
		}
	}
}
