using Klei.AI;
using System.Collections.Generic;
using System.Diagnostics;

[DebuggerDisplay("{IdHash}")]
public class ChoreGroup : Resource
{
	public List<ChoreType> choreTypes = new List<ChoreType>();

	public Attribute attribute;

	public string description;

	public string sprite;

	private int defaultPersonalPriority;

	public int DefaultPersonalPriority => defaultPersonalPriority;

	public ChoreGroup(string id, string name, Attribute attribute, string sprite, int default_personal_priority)
		: base(id, name)
	{
		this.attribute = attribute;
		description = Strings.Get("STRINGS.DUPLICANTS.CHOREGROUPS." + id.ToUpper() + ".DESC").String;
		this.sprite = sprite;
		defaultPersonalPriority = default_personal_priority;
	}
}
