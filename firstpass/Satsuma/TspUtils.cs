using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public static class TspUtils
	{
		public static double GetTourCost<TNode>(IEnumerable<TNode> tour, Func<TNode, TNode, double> cost)
		{
			double num = 0.0;
			if (tour.Any())
			{
				TNode arg = tour.First();
				foreach (TNode item in tour.Skip(1))
				{
					num += cost(arg, item);
					arg = item;
				}
			}
			return num;
		}
	}
}
