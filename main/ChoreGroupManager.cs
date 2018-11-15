using KSerialization;
using System.Collections.Generic;

[SerializationConfig(MemberSerialization.OptIn)]
public class ChoreGroupManager : KMonoBehaviour, ISaveLoadable
{
	public static ChoreGroupManager instance;

	[Serialize]
	private List<Tag> defaultForbiddenTagsList = new List<Tag>();

	[Serialize]
	private Dictionary<Tag, int> defaultChorePermissions = new Dictionary<Tag, int>();

	public List<Tag> DefaultForbiddenTagsList => defaultForbiddenTagsList;

	public Dictionary<Tag, int> DefaultChorePermission => defaultChorePermissions;

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		instance = this;
		ConvertOldVersion();
		foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
		{
			if (!defaultChorePermissions.ContainsKey(resource.Id.ToTag()))
			{
				defaultChorePermissions.Add(resource.Id.ToTag(), 2);
			}
		}
	}

	private void ConvertOldVersion()
	{
		foreach (Tag defaultForbiddenTags in defaultForbiddenTagsList)
		{
			if (!defaultChorePermissions.ContainsKey(defaultForbiddenTags))
			{
				defaultChorePermissions.Add(defaultForbiddenTags, -1);
			}
			defaultChorePermissions[defaultForbiddenTags] = 0;
		}
		defaultForbiddenTagsList.Clear();
	}
}
