using Delaunay.Geo;
using Delaunay.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Delaunay
{
	public sealed class SiteList : IDisposable
	{
		private List<Site> _sites;

		private int _currentIndex;

		private bool _sorted;

		public int Count => _sites.Count;

		public SiteList()
		{
			_sites = new List<Site>();
			_sorted = false;
		}

		public void Dispose()
		{
			if (_sites != null)
			{
				for (int i = 0; i < _sites.Count; i++)
				{
					Site site = _sites[i];
					site.Dispose();
				}
				_sites.Clear();
				_sites = null;
			}
		}

		public int Add(Site site)
		{
			_sorted = false;
			_sites.Add(site);
			return _sites.Count;
		}

		public Site Next()
		{
			if (!_sorted)
			{
				UnityEngine.Debug.LogError("SiteList::next():  sites have not been sorted");
			}
			if (_currentIndex < _sites.Count)
			{
				return _sites[_currentIndex++];
			}
			return null;
		}

		internal Rect GetSitesBounds()
		{
			if (!_sorted)
			{
				Site.SortSites(_sites);
				_currentIndex = 0;
				_sorted = true;
			}
			if (_sites.Count == 0)
			{
				return new Rect(0f, 0f, 0f, 0f);
			}
			float num = 3.40282347E+38f;
			float num2 = -3.40282347E+38f;
			for (int i = 0; i < _sites.Count; i++)
			{
				Site site = _sites[i];
				if (site.x < num)
				{
					num = site.x;
				}
				if (site.x > num2)
				{
					num2 = site.x;
				}
			}
			float y = _sites[0].y;
			float y2 = _sites[_sites.Count - 1].y;
			return new Rect(num, y, num2 - num, y2 - y);
		}

		public List<uint> SiteColors()
		{
			List<uint> list = new List<uint>();
			for (int i = 0; i < _sites.Count; i++)
			{
				list.Add(_sites[i].color);
			}
			return list;
		}

		public List<Vector2> SiteCoords()
		{
			List<Vector2> list = new List<Vector2>();
			for (int i = 0; i < _sites.Count; i++)
			{
				list.Add(_sites[i].Coord);
			}
			return list;
		}

		public void ScaleWeight(float scale)
		{
			for (int i = 0; i < _sites.Count; i++)
			{
				_sites[i].scaled_weight = _sites[i].weight * scale;
			}
		}

		public List<Circle> Circles()
		{
			List<Circle> list = new List<Circle>();
			for (int i = 0; i < _sites.Count; i++)
			{
				float radius = 0f;
				Edge edge = _sites[i].NearestEdge();
				if (!edge.IsPartOfConvexHull())
				{
					radius = edge.SitesDistance() * 0.5f;
				}
				list.Add(new Circle(_sites[i].x, _sites[i].y, radius));
			}
			return list;
		}

		public List<List<Vector2>> Regions(Rect plotBounds)
		{
			List<List<Vector2>> list = new List<List<Vector2>>();
			for (int i = 0; i < _sites.Count; i++)
			{
				Site site = _sites[i];
				list.Add(site.Region(plotBounds));
			}
			return list;
		}

		public List<List<Vector2>> Regions(Polygon plotBounds)
		{
			List<List<Vector2>> list = new List<List<Vector2>>();
			for (int i = 0; i < _sites.Count; i++)
			{
				Site site = _sites[i];
				list.Add(site.Region(plotBounds));
			}
			return list;
		}
	}
}
