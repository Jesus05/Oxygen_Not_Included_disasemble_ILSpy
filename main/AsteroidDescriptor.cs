using System.Collections.Generic;
using UnityEngine;

public struct AsteroidDescriptor
{
	public string text;

	public string tooltip;

	public List<Tuple<string, Color, float>> bands;

	public AsteroidDescriptor(string text, string tooltip, List<Tuple<string, Color, float>> bands = null)
	{
		this.text = text;
		this.tooltip = tooltip;
		this.bands = bands;
	}
}
