using System.Collections.Generic;

public class LevelLayer : List<LayerGradient>
{
	public LevelLayer()
	{
	}

	public LevelLayer(int size)
		: base(size)
	{
	}

	public LevelLayer(IEnumerable<LayerGradient> collection)
		: base(collection)
	{
	}

	public void ConvertBandSizeToMaxSize()
	{
		float num = 0f;
		for (int i = 0; i < base.Count; i++)
		{
			LayerGradient layerGradient = base[i];
			num += layerGradient.bandSize;
		}
		float num2 = 0f;
		for (int j = 0; j < base.Count; j++)
		{
			LayerGradient layerGradient2 = base[j];
			layerGradient2.maxValue = num2 + layerGradient2.bandSize / num;
			num2 = layerGradient2.maxValue;
		}
	}
}
