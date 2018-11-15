using Delaunay.Utils;
using System.Collections.Generic;

namespace Delaunay
{
	public sealed class Triangle : IDisposable
	{
		private List<Site> _sites;

		public List<Site> sites => _sites;

		public Triangle(Site a, Site b, Site c)
		{
			_sites = new List<Site>
			{
				a,
				b,
				c
			};
		}

		public void Dispose()
		{
			_sites.Clear();
			_sites = null;
		}
	}
}
