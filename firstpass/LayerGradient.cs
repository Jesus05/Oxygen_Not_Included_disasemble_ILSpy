using System.Collections.Generic;

public class LayerGradient : Gradient<List<string>>
{
	public LayerGradient()
		: base((List<string>)null, 0f)
	{
	}

	public LayerGradient(List<string> content, float bandSize)
		: base(content, bandSize)
	{
	}
}
