namespace Satsuma
{
	public interface IDestroyableGraph : IClearable
	{
		bool DeleteNode(Node node);

		bool DeleteArc(Arc arc);
	}
}
