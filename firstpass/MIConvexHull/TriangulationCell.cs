namespace MIConvexHull
{
	public abstract class TriangulationCell<TVertex, TCell> : ConvexFace<TVertex, TCell> where TVertex : IVertex where TCell : ConvexFace<TVertex, TCell>
	{
	}
}
