using System.Collections.Generic;

namespace Database
{
	public class SkillGroup : Resource
	{
		public string choreGroupID;

		public List<string> relevantAttributes;

		public List<string> requiredChoreGroups;

		public SkillGroup(string id, string choreGroupID, string name)
			: base(id, name)
		{
			this.choreGroupID = choreGroupID;
		}
	}
}
