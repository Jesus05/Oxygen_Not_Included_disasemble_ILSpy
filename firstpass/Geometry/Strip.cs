namespace Geometry
{
	public class Strip
	{
		public float yMin;

		public float yMax;

		public bool subtract;

		public Strip(float yMin, float yMax, bool subtract)
		{
			this.yMin = yMin;
			this.yMax = yMax;
			this.subtract = subtract;
		}
	}
}
