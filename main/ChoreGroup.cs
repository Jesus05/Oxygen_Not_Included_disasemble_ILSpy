using Klei.AI;
using System.Collections.Generic;
using System.Diagnostics;

[DebuggerDisplay("{IdHash}")]
public class ChoreGroup : Resource
{
	public List<ChoreType> choreTypes = new List<ChoreType>();

	public Attribute attribute;

	public string description;

	private int defaultPersonalPriority;

	public int DefaultPersonalPriority => defaultPersonalPriority;

	public ChoreGroup(string id, string name, string attribute, int default_personal_priority)
		: base(id, name)
	{
		this.attribute = Db.Get().Attributes.Get(attribute);
		description = Strings.Get("STRINGS.DUPLICANTS.CHOREGROUPS." + id.ToUpper() + ".DESC").String;
		defaultPersonalPriority = default_personal_priority;
	}
}
