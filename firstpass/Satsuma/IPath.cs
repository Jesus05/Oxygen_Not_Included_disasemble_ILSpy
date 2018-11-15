namespace Satsuma
{
	public interface IPath : IGraph, IArcLookup
	{
		Node FirstNode
		{
			get;
		}

		Node LastNode
		{
			get;
		}

		Arc NextArc(Node node);

		Arc PrevArc(Node node);
	}
}
