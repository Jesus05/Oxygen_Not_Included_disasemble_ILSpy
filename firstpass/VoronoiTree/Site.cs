using Delaunay.Geo;
using KSerialization;
using MIConvexHull;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

namespace VoronoiTree
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Site : TriangulationCell<PowerDiagram.DualSite2d, Site>
	{
		public float weight;

		public float currentWeight;

		public float previousWeightAdaption;

		[Serialize]
		public int id = -1;

		[Serialize]
		public Vector2 position;

		[Serialize]
		public Polygon poly;

		[Serialize]
		public List<Site> neighbours;

		private Vector2? circumCenter;

		private Vector2? centroid;

		public bool dummy
		{
			get;
			set;
		}

		public Vector2 Circumcenter
		{
			get
			{
				Vector2? nullable = circumCenter;
				circumCenter = ((!nullable.HasValue) ? GetCircumcenter() : nullable.Value);
				return circumCenter.Value;
			}
		}

		public Vector2 Centroid
		{
			get
			{
				if (poly != null)
				{
					return poly.Centroid();
				}
				Vector2? nullable = centroid;
				centroid = ((!nullable.HasValue) ? GetCentroid() : nullable.Value);
				return centroid.Value;
			}
		}

		public Site()
		{
			dummy = true;
		}

		public Site(Vector2 pos)
		{
			position = pos;
			weight = Mathf.Epsilon;
			dummy = false;
			previousWeightAdaption = 1f;
		}

		public Site(float x, float y)
			: this(new Vector2(x, y))
		{
		}

		public Site(uint siteId, Vector2 pos, float siteWeight = 1f)
		{
			dummy = false;
			neighbours = new List<Site>();
			id = (int)siteId;
			position = pos;
			weight = siteWeight;
			currentWeight = weight;
		}

		[OnDeserializing]
		internal void OnDeserializingMethod()
		{
			neighbours = new List<Site>();
		}

		public PowerDiagram.DualSite3d ToDualSite()
		{
			return new PowerDiagram.DualSite3d((double)position.x, (double)position.y, (double)(position.x * position.x + position.y * position.y - currentWeight), this);
		}

		private double Det(double[,] m)
		{
			return m[0, 0] * (m[1, 1] * m[2, 2] - m[2, 1] * m[1, 2]) - m[0, 1] * (m[1, 0] * m[2, 2] - m[2, 0] * m[1, 2]) + m[0, 2] * (m[1, 0] * m[2, 1] - m[2, 0] * m[1, 1]);
		}

		private double LengthSquared(double[] v)
		{
			double num = 0.0;
			foreach (double num2 in v)
			{
				num += num2 * num2;
			}
			return num;
		}

		private Vector2 GetCircumcenter()
		{
			PowerDiagram.DualSite2d[] vertices = base.Vertices;
			double[,] array = new double[3, 3];
			for (int i = 0; i < 3; i++)
			{
				double[,] array2 = array;
				int num = i;
				double num2 = vertices[i].Position[0];
				array2[num, 0] = num2;
				double[,] array3 = array;
				int num3 = i;
				double num4 = vertices[i].Position[1];
				array3[num3, 1] = num4;
				array[i, 2] = 1.0;
			}
			double num5 = Det(array);
			double num6 = -1.0 / (2.0 * num5);
			for (int j = 0; j < 3; j++)
			{
				double[,] array4 = array;
				int num7 = j;
				double num8 = LengthSquared(vertices[j].Position);
				array4[num7, 0] = num8;
			}
			double num9 = 0.0 - Det(array);
			for (int k = 0; k < 3; k++)
			{
				double[,] array5 = array;
				int num10 = k;
				double num11 = vertices[k].Position[0];
				array5[num10, 1] = num11;
			}
			double num12 = Det(array);
			return new Vector2((float)(num6 * num9), (float)(num6 * num12));
		}

		private Vector2 GetCentroid()
		{
			return new Vector2((float)(from v in base.Vertices
			select v.Position[0]).Average(), (float)(from v in base.Vertices
			select v.Position[1]).Average());
		}
	}
}
