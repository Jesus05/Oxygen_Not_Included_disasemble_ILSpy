using System;
using System.Collections.Generic;
using System.Linq;

namespace MIConvexHull
{
	public static class ConvexHull
	{
		public static ConvexHull<TVertex, TFace> Create<TVertex, TFace>(IList<TVertex> data, double PlaneDistanceTolerance = 1E-10) where TVertex : IVertex where TFace : ConvexFace<TVertex, TFace>, new()
		{
			return ConvexHull<TVertex, TFace>.Create(data, PlaneDistanceTolerance);
		}

		public static ConvexHull<TVertex, DefaultConvexFace<TVertex>> Create<TVertex>(IList<TVertex> data, double PlaneDistanceTolerance = 1E-10) where TVertex : IVertex
		{
			return ConvexHull<TVertex, DefaultConvexFace<TVertex>>.Create(data, PlaneDistanceTolerance);
		}

		public static ConvexHull<DefaultVertex, DefaultConvexFace<DefaultVertex>> Create(IList<double[]> data, double PlaneDistanceTolerance = 1E-10)
		{
			List<DefaultVertex> data2 = (from p in data
			select new DefaultVertex
			{
				Position = p
			}).ToList();
			return ConvexHull<DefaultVertex, DefaultConvexFace<DefaultVertex>>.Create(data2, PlaneDistanceTolerance);
		}
	}
	public class ConvexHull<TVertex, TFace> where TVertex : IVertex where TFace : ConvexFace<TVertex, TFace>, new()
	{
		public IEnumerable<TVertex> Points
		{
			get;
			internal set;
		}

		public IEnumerable<TFace> Faces
		{
			get;
			internal set;
		}

		internal ConvexHull()
		{
		}

		public static ConvexHull<TVertex, TFace> Create(IList<TVertex> data, double PlaneDistanceTolerance)
		{
			if (data == null)
			{
				throw new ArgumentNullException("The supplied data is null.");
			}
			return ConvexHullAlgorithm.GetConvexHull<TVertex, TFace>(data, PlaneDistanceTolerance);
		}
	}
}
