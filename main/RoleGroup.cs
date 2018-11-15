using System.Collections.Generic;

public class RoleGroup
{
	public HashedString id;

	public string choreGroupID;

	public string Name;

	public List<RoleConfig> roles = new List<RoleConfig>();

	public RoleGroup(HashedString id, string choreGroupID, string Name)
	{
		this.id = id;
		this.choreGroupID = choreGroupID;
		this.Name = Name;
	}
}
