using System;
using System.Collections.Generic;

namespace Satsuma
{
	public interface IFlow<TCapacity>
	{
		IGraph Graph
		{
			get;
		}

		Func<Arc, TCapacity> Capacity
		{
			get;
		}

		Node Source
		{
			get;
		}

		Node Target
		{
			get;
		}

		TCapacity FlowSize
		{
			get;
		}

		IEnumerable<KeyValuePair<Arc, TCapacity>> NonzeroArcs
		{
			get;
		}

		TCapacity Flow(Arc arc);
	}
}
