internal struct EffectorEntryDecibel
{
	public string name;

	public int count;

	public float value;

	public EffectorEntryDecibel(string name, float value)
	{
		this.name = name;
		this.value = value;
		count = 1;
	}
}
