using System.Collections.Generic;

public class ElementBandConfiguration : List<ElementGradient>
{
	public ElementBandConfiguration()
	{
	}

	public ElementBandConfiguration(int size)
		: base(size)
	{
	}

	public ElementBandConfiguration(IEnumerable<ElementGradient> collection)
		: base(collection)
	{
	}

	public List<float> ConvertBandSizeToMaxSize()
	{
		List<float> list = new List<float>();
		float num = 0f;
		for (int i = 0; i < base.Count; i++)
		{
			ElementGradient elementGradient = base[i];
			num += elementGradient.bandSize;
		}
		float num2 = 0f;
		for (int j = 0; j < base.Count; j++)
		{
			ElementGradient elementGradient2 = base[j];
			elementGradient2.maxValue = num2 + elementGradient2.bandSize / num;
			num2 = elementGradient2.maxValue;
			list.Add(elementGradient2.maxValue);
		}
		return list;
	}
}
