namespace Delaunay.LR
{
	public class SideHelper
	{
		public static Side Other(Side leftRight)
		{
			return (leftRight == Side.LEFT) ? Side.RIGHT : Side.LEFT;
		}
	}
}
