namespace Geometry
{
	public struct HorizontalEvent
	{
		public float x;

		public Strip strip;

		public bool isStart;

		public HorizontalEvent(float x, Strip strip, bool isStart)
		{
			this.x = x;
			this.strip = strip;
			this.isStart = isStart;
		}
	}
}
