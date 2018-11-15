namespace LibNoiseDotNet.Graphics.Tools.Noise.Builder
{
	public interface IBuilderFilter
	{
		float ConstantValue
		{
			get;
			set;
		}

		float FilterValue(int x, int y, float source);

		FilterLevel IsFiltered(int x, int y);
	}
}
