namespace Geometry
{
	public struct VerticalEvent
	{
		public float y;

		public bool isStart;

		public bool subtract;

		public VerticalEvent(float y, bool isStart, bool subtract)
		{
			this.y = y;
			this.isStart = isStart;
			this.subtract = subtract;
		}
	}
}
