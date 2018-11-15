namespace Klei
{
	public struct SolidInfo
	{
		public int cellIdx;

		public bool isSolid;

		public SolidInfo(int cellIdx, bool isSolid)
		{
			this.cellIdx = cellIdx;
			this.isSolid = isSolid;
		}
	}
}
