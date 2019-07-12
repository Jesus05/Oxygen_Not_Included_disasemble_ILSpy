using Klei.AI;
using System.Collections.Generic;

namespace Database
{
	public class SkillGroup : Resource, IListableOption
	{
		public string choreGroupID;

		public List<Attribute> relevantAttributes;

		public List<string> requiredChoreGroups;

		public string choreGroupIcon;

		public SkillGroup(string id, string choreGroupID, string name, string icon)
			: base(id, name)
		{
			this.choreGroupID = choreGroupID;
			choreGroupIcon = icon;
		}

		string IListableOption.GetProperName()
		{
			return Strings.Get("STRINGS.DUPLICANTS.SKILLGROUPS." + Id.ToUpper() + ".NAME");
		}
	}
}
