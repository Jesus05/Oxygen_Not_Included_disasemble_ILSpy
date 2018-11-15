using System;

[Serializable]
public struct GraphAxis
{
	private string name;

	public float min_value;

	public float max_value;

	private LocText name_label;

	public float guide_frequency;

	public float range => max_value - min_value;
}
