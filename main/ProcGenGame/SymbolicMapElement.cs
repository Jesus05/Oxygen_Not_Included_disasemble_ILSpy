namespace ProcGenGame
{
	public interface SymbolicMapElement
	{
		void ConvertToMap(Chunk world, TerrainCell.SetValuesFunction SetValues, float temperatureMin, float temperatureRange, SeededRandom rnd);
	}
}
