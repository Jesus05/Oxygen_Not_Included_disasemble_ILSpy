namespace Satsuma
{
	public interface IDisjointSet<T> : IReadOnlyDisjointSet<T>, IClearable
	{
		DisjointSetSet<T> Union(DisjointSetSet<T> a, DisjointSetSet<T> b);
	}
}
