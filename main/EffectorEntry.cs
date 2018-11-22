using STRINGS;

internal struct EffectorEntry
{
	public string name;

	public int count;

	public float value;

	public EffectorEntry(string name, float value)
	{
		this.name = name;
		this.value = value;
		count = 1;
	}

	public override string ToString()
	{
		string arg = "";
		if (count > 1)
		{
			arg = string.Format(UI.OVERLAYS.DECOR.COUNT, count);
		}
		return string.Format(UI.OVERLAYS.DECOR.ENTRY, GameUtil.GetFormattedDecor(value), name, arg);
	}
}
