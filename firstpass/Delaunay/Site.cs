using Delaunay.Geo;
using Delaunay.LR;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Delaunay
{
	public sealed class Site : ICoord, IComparable
	{
		private static Stack<Site> _pool = new Stack<Site>();

		private static readonly float EPSILON = 0.005f;

		private Vector2 _coord;

		public float scaled_weight;

		private uint _siteIndex;

		private List<Edge> _edges;

		private List<Side> _edgeOrientations;

		private List<Vector2> _region;

		public uint color
		{
			get;
			private set;
		}

		public float weight
		{
			get;
			private set;
		}

		internal List<Edge> edges => _edges;

		public float x => _coord.x;

		internal float y => _coord.y;

		public Vector2 Coord => _coord;

		private Site(Vector2 p, uint index, float weight, uint color)
		{
			Init(p, index, weight, color);
		}

		public float Dist(ICoord p)
		{
			return Vector2.Distance(p.Coord, _coord);
		}

		public override string ToString()
		{
			return "Site " + _siteIndex.ToString() + ": " + Coord.ToString();
		}

		public static Site Create(Vector2 p, uint index, float weight, uint color)
		{
			if (_pool.Count <= 0)
			{
				return new Site(p, index, weight, color);
			}
			return _pool.Pop().Init(p, index, weight, color);
		}

		internal static void SortSites(List<Site> sites)
		{
			sites.Sort();
		}

		public int CompareTo(object obj)
		{
			Site site = (Site)obj;
			int num = Voronoi.CompareByYThenX(this, site);
			switch (num)
			{
			case -1:
				if (_siteIndex > site._siteIndex)
				{
					uint siteIndex = _siteIndex;
					_siteIndex = site._siteIndex;
					site._siteIndex = siteIndex;
				}
				break;
			case 1:
				if (site._siteIndex > _siteIndex)
				{
					uint siteIndex = site._siteIndex;
					site._siteIndex = _siteIndex;
					_siteIndex = siteIndex;
				}
				break;
			}
			return num;
		}

		private static bool CloseEnough(Vector2 p0, Vector2 p1)
		{
			return Vector2.Distance(p0, p1) < EPSILON;
		}

		private Site Init(Vector2 p, uint index, float weight, uint color)
		{
			scaled_weight = -1f;
			_coord = p;
			_siteIndex = index;
			this.weight = weight;
			this.color = color;
			_edges = new List<Edge>();
			_region = null;
			return this;
		}

		private void Move(Vector2 p)
		{
			Clear();
			_coord = p;
		}

		public void Dispose()
		{
			Clear();
			_pool.Push(this);
		}

		private void Clear()
		{
			if (_edges != null)
			{
				_edges.Clear();
				_edges = null;
			}
			if (_edgeOrientations != null)
			{
				_edgeOrientations.Clear();
				_edgeOrientations = null;
			}
			if (_region != null)
			{
				_region.Clear();
				_region = null;
			}
		}

		public void AddEdge(Edge edge)
		{
			_edges.Add(edge);
		}

		public Vector2 GetClosestPt(Vector2 p)
		{
			Vector2 normalized = (p - _coord).normalized;
			return _coord + normalized * weight;
		}

		public Edge NearestEdge()
		{
			_edges.Sort((Edge a, Edge b) => Edge.CompareSitesDistances(a, b));
			return _edges[0];
		}

		public List<Site> NeighborSites()
		{
			if (_edges != null && _edges.Count != 0)
			{
				if (_edgeOrientations == null)
				{
					ReorderEdges();
				}
				List<Site> list = new List<Site>();
				for (int i = 0; i < _edges.Count; i++)
				{
					Edge edge = _edges[i];
					list.Add(NeighborSite(edge));
				}
				return list;
			}
			return new List<Site>();
		}

		private Site NeighborSite(Edge edge)
		{
			if (this != edge.leftSite)
			{
				if (this != edge.rightSite)
				{
					return null;
				}
				return edge.leftSite;
			}
			return edge.rightSite;
		}

		internal List<Vector2> Region(Rect clippingBounds)
		{
			if (_edges != null && _edges.Count != 0)
			{
				if (_edgeOrientations == null)
				{
					ReorderEdges();
					_region = ClipToBounds(clippingBounds);
					if (new Polygon(_region).Winding() == Winding.CLOCKWISE)
					{
						_region.Reverse();
					}
				}
				return _region;
			}
			return new List<Vector2>();
		}

		internal List<Vector2> Region(Polygon clippingBounds)
		{
			if (_edges != null && _edges.Count != 0)
			{
				if (_edgeOrientations == null)
				{
					ReorderEdges();
					_region = ClipToBounds(clippingBounds);
					if (new Polygon(_region).Winding() == Winding.CLOCKWISE)
					{
						_region.Reverse();
					}
				}
				return _region;
			}
			return new List<Vector2>();
		}

		private void ReorderEdges()
		{
			EdgeReorderer edgeReorderer = new EdgeReorderer(_edges, VertexOrSite.VERTEX);
			_edges = edgeReorderer.edges;
			_edgeOrientations = edgeReorderer.edgeOrientations;
			edgeReorderer.Dispose();
		}

		private List<Vector2> ClipToBounds(Rect bounds)
		{
			List<Vector2> list = new List<Vector2>();
			int count = _edges.Count;
			int i;
			for (i = 0; i < count && !_edges[i].visible; i++)
			{
			}
			if (i != count)
			{
				Edge edge = _edges[i];
				Side side = _edgeOrientations[i];
				if (!edge.clippedEnds[side].HasValue)
				{
					Debug.LogError("XXX: Null detected when there should be a Vector2!");
				}
				if (!edge.clippedEnds[SideHelper.Other(side)].HasValue)
				{
					Debug.LogError("XXX: Null detected when there should be a Vector2!");
				}
				list.Add(edge.clippedEnds[side].Value);
				list.Add(edge.clippedEnds[SideHelper.Other(side)].Value);
				for (int j = i + 1; j < count; j++)
				{
					edge = _edges[j];
					if (edge.visible)
					{
						Connect(list, j, bounds, false);
					}
				}
				Connect(list, i, bounds, true);
				return list;
			}
			return new List<Vector2>();
		}

		private List<Vector2> ClipToBounds(Polygon bounds)
		{
			return ClipToBounds(bounds.bounds);
		}

		private void Connect(List<Vector2> points, int j, Rect bounds, bool closingUp = false)
		{
			Vector2 vector = points[points.Count - 1];
			Edge edge = _edges[j];
			Side side = _edgeOrientations[j];
			if (!edge.clippedEnds[side].HasValue)
			{
				Debug.LogError("XXX: Null detected when there should be a Vector2!");
			}
			Vector2 value = edge.clippedEnds[side].Value;
			if (!CloseEnough(vector, value))
			{
				if (vector.x != value.x && vector.y != value.y)
				{
					int num = BoundsCheck.Check(vector, bounds);
					int num2 = BoundsCheck.Check(value, bounds);
					if ((num & BoundsCheck.RIGHT) != 0)
					{
						float xMax = bounds.xMax;
						if ((num2 & BoundsCheck.BOTTOM) != 0)
						{
							float yMax = bounds.yMax;
							points.Add(new Vector2(xMax, yMax));
						}
						else if ((num2 & BoundsCheck.TOP) != 0)
						{
							float yMax = bounds.yMin;
							points.Add(new Vector2(xMax, yMax));
						}
						else if ((num2 & BoundsCheck.LEFT) != 0)
						{
							float yMax = (!(vector.y - bounds.y + value.y - bounds.y < bounds.height)) ? bounds.yMax : bounds.yMin;
							points.Add(new Vector2(xMax, yMax));
							points.Add(new Vector2(bounds.xMin, yMax));
						}
					}
					else if ((num & BoundsCheck.LEFT) != 0)
					{
						float xMax = bounds.xMin;
						if ((num2 & BoundsCheck.BOTTOM) != 0)
						{
							float yMax = bounds.yMax;
							points.Add(new Vector2(xMax, yMax));
						}
						else if ((num2 & BoundsCheck.TOP) != 0)
						{
							float yMax = bounds.yMin;
							points.Add(new Vector2(xMax, yMax));
						}
						else if ((num2 & BoundsCheck.RIGHT) != 0)
						{
							float yMax = (!(vector.y - bounds.y + value.y - bounds.y < bounds.height)) ? bounds.yMax : bounds.yMin;
							points.Add(new Vector2(xMax, yMax));
							points.Add(new Vector2(bounds.xMax, yMax));
						}
					}
					else if ((num & BoundsCheck.TOP) != 0)
					{
						float yMax = bounds.yMin;
						if ((num2 & BoundsCheck.RIGHT) != 0)
						{
							float xMax = bounds.xMax;
							points.Add(new Vector2(xMax, yMax));
						}
						else if ((num2 & BoundsCheck.LEFT) != 0)
						{
							float xMax = bounds.xMin;
							points.Add(new Vector2(xMax, yMax));
						}
						else if ((num2 & BoundsCheck.BOTTOM) != 0)
						{
							float xMax = (!(vector.x - bounds.x + value.x - bounds.x < bounds.width)) ? bounds.xMax : bounds.xMin;
							points.Add(new Vector2(xMax, yMax));
							points.Add(new Vector2(xMax, bounds.yMax));
						}
					}
					else if ((num & BoundsCheck.BOTTOM) != 0)
					{
						float yMax = bounds.yMax;
						if ((num2 & BoundsCheck.RIGHT) != 0)
						{
							float xMax = bounds.xMax;
							points.Add(new Vector2(xMax, yMax));
						}
						else if ((num2 & BoundsCheck.LEFT) != 0)
						{
							float xMax = bounds.xMin;
							points.Add(new Vector2(xMax, yMax));
						}
						else if ((num2 & BoundsCheck.TOP) != 0)
						{
							float xMax = (!(vector.x - bounds.x + value.x - bounds.x < bounds.width)) ? bounds.xMax : bounds.xMin;
							points.Add(new Vector2(xMax, yMax));
							points.Add(new Vector2(xMax, bounds.yMin));
						}
					}
				}
				if (closingUp)
				{
					return;
				}
				points.Add(value);
			}
			if (!edge.clippedEnds[SideHelper.Other(side)].HasValue)
			{
				Debug.LogError("XXX: Null detected when there should be a Vector2!");
			}
			Vector2 value2 = edge.clippedEnds[SideHelper.Other(side)].Value;
			if (!CloseEnough(points[0], value2))
			{
				points.Add(value2);
			}
		}
	}
}
