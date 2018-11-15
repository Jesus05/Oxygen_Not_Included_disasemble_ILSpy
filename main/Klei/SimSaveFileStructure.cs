namespace Klei
{
	public class SimSaveFileStructure
	{
		public int WidthInCells;

		public int HeightInCells;

		public byte[] Sim;

		public WorldDetailSave worldDetail;

		public SimSaveFileStructure()
		{
			worldDetail = new WorldDetailSave();
		}
	}
}
