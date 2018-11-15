using System.Collections.Generic;

namespace Satsuma
{
	public interface ITsp<TNode>
	{
		IEnumerable<TNode> Tour
		{
			get;
		}

		double TourCost
		{
			get;
		}
	}
}
