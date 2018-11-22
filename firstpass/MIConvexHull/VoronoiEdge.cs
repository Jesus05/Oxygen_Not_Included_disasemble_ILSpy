namespace MIConvexHull
{
	public class VoronoiEdge<TVertex, TCell> where TVertex : IVertex where TCell : TriangulationCell<TVertex, TCell>
	{
		public TCell Source
		{
			get;
			internal set;
		}

		public TCell Target
		{
			get;
			internal set;
		}

		public VoronoiEdge()
		{
		}

		public VoronoiEdge(TCell source, TCell target)
		{
			Source = source;
			Target = target;
		}

		public override bool Equals(object obj)
		{
			VoronoiEdge<TVertex, TCell> voronoiEdge = obj as VoronoiEdge<TVertex, TCell>;
			if (voronoiEdge != null)
			{
				if (!object.ReferenceEquals(this, voronoiEdge))
				{
					return (Source == voronoiEdge.Source && Target == voronoiEdge.Target) || (Source == voronoiEdge.Target && Target == voronoiEdge.Source);
				}
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int num = 23;
			int num2 = num * 31;
			TCell source = Source;
			num = num2 + source.GetHashCode();
			int num3 = num * 31;
			TCell target = Target;
			return num3 + target.GetHashCode();
		}
	}
}
