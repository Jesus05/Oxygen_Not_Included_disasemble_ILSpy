using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class CellEventInstance : EventInstanceBase, ISaveLoadable
{
	[Serialize]
	public int cell;

	[Serialize]
	public int data;

	[Serialize]
	public int data2;

	public CellEventInstance(int cell, int data, int data2, CellEvent ev)
		: base(ev)
	{
		this.cell = cell;
		this.data = data;
		this.data2 = data2;
	}
}
