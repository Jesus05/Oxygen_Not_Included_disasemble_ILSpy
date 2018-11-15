using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class Opt2Tsp<TNode> : ITsp<TNode>
	{
		private List<TNode> tour;

		public Func<TNode, TNode, double> Cost
		{
			get;
			private set;
		}

		public IEnumerable<TNode> Tour => tour;

		public double TourCost
		{
			get;
			private set;
		}

		public Opt2Tsp(Func<TNode, TNode, double> cost, IEnumerable<TNode> tour, double? tourCost)
		{
			Cost = cost;
			this.tour = Enumerable.ToList<TNode>(tour);
			TourCost = ((!tourCost.HasValue) ? TspUtils.GetTourCost<TNode>(tour, cost) : tourCost.Value);
		}

		public bool Step()
		{
			bool result = false;
			for (int i = 0; i < tour.Count - 3; i++)
			{
				int j = i + 2;
				for (int num = tour.Count - ((i != 0) ? 1 : 2); j < num; j++)
				{
					double num2 = Cost(tour[i], tour[j]) + Cost(tour[i + 1], tour[j + 1]) - (Cost(tour[i], tour[i + 1]) + Cost(tour[j], tour[j + 1]));
					if (num2 < 0.0)
					{
						TourCost += num2;
						tour.Reverse(i + 1, j - i);
						result = true;
					}
				}
			}
			return result;
		}

		public void Run()
		{
			while (Step())
			{
			}
		}
	}
}
