using System.Collections.Generic;
using System.Diagnostics;

[DebuggerDisplay("{IdHash}")]
public class ChoreType : Resource
{
	public StatusItem statusItem;

	public HashSet<Tag> tags = new HashSet<Tag>();

	public HashSet<Tag> interruptExclusion;

	public string reportName;

	public Urge urge
	{
		get;
		private set;
	}

	public ChoreGroup[] groups
	{
		get;
		private set;
	}

	public int priority
	{
		get;
		private set;
	}

	public int interruptPriority
	{
		get;
		set;
	}

	public int explicitPriority
	{
		get;
		private set;
	}

	public ChoreType(string id, ResourceSet parent, string[] chore_groups, string urge, string name, string status_message, string tooltip, IEnumerable<Tag> interrupt_exclusion, int implicit_priority, int explicit_priority)
		: base(id, parent, name)
	{
		statusItem = new StatusItem(id, status_message, tooltip, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, 129022);
		statusItem.resolveStringCallback = ResolveStringCallback;
		tags.Add(TagManager.Create(id));
		interruptExclusion = new HashSet<Tag>(interrupt_exclusion);
		Db.Get().DuplicantStatusItems.Add(statusItem);
		groups = new ChoreGroup[chore_groups.Length];
		for (int i = 0; i < groups.Length; i++)
		{
			ChoreGroup choreGroup = Db.Get().ChoreGroups.Get(chore_groups[i]);
			if (!choreGroup.choreTypes.Contains(this))
			{
				choreGroup.choreTypes.Add(this);
			}
			groups[i] = choreGroup;
		}
		if (!string.IsNullOrEmpty(urge))
		{
			this.urge = Db.Get().Urges.Get(urge);
		}
		priority = implicit_priority;
		explicitPriority = explicit_priority;
	}

	private string ResolveStringCallback(string str, object data)
	{
		Chore chore = (Chore)data;
		return chore.ResolveString(str);
	}
}
