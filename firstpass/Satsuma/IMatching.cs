namespace Satsuma
{
	public interface IMatching : IGraph, IArcLookup
	{
		IGraph Graph
		{
			get;
		}

		Arc MatchedArc(Node node);
	}
}
