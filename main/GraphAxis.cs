using System;

[Serializable]
public struct GraphAxis
{
	public string name;

	public float min_value;

	public float max_value;

	public float guide_frequency;

	public float range => max_value - min_value;
}
