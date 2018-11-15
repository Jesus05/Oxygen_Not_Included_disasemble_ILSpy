namespace Satsuma
{
	public interface IArcLookup
	{
		Node U(Arc arc);

		Node V(Arc arc);

		bool IsEdge(Arc arc);
	}
}
