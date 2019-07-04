using System.Collections.Generic;
using UnityEngine;

public class SubEntry
{
	public ContentContainer lockedContentContainer;

	public Color iconColor = Color.white;

	public List<ContentContainer> contentContainers
	{
		get;
		set;
	}

	public string parentEntryID
	{
		get;
		set;
	}

	public string id
	{
		get;
		set;
	}

	public string name
	{
		get;
		set;
	}

	public string title
	{
		get;
		set;
	}

	public string subtitle
	{
		get;
		set;
	}

	public Sprite icon
	{
		get;
		set;
	}

	public int layoutPriority
	{
		get;
		set;
	}

	public bool disabled
	{
		get;
		set;
	}

	public string lockID
	{
		get;
		set;
	}

	public SubEntry()
	{
	}

	public SubEntry(string id, string parentEntryID, List<ContentContainer> contentContainers, string name)
	{
		this.id = id;
		this.parentEntryID = parentEntryID;
		this.name = name;
		this.contentContainers = contentContainers;
		if (!string.IsNullOrEmpty(lockID))
		{
			foreach (ContentContainer contentContainer in contentContainers)
			{
				contentContainer.lockID = lockID;
			}
		}
	}
}
