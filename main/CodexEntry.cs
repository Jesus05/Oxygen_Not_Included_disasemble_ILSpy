using System.Collections.Generic;
using UnityEngine;

public class CodexEntry
{
	public List<SubEntry> subEntries = new List<SubEntry>();

	public Color iconColor = Color.white;

	public bool searchOnly = false;

	public int customContentLength = 0;

	public List<ContentContainer> contentContainers
	{
		get;
		set;
	}

	public string id
	{
		get;
		set;
	}

	public string parentId
	{
		get;
		set;
	}

	public string category
	{
		get;
		set;
	}

	public string title
	{
		get;
		set;
	}

	public string name
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

	public string iconPrefabID
	{
		get;
		set;
	}

	public bool disabled
	{
		get;
		set;
	}

	public CodexEntry()
	{
	}

	public CodexEntry(string category, List<ContentContainer> contentContainers, string name)
	{
		this.category = category;
		this.name = name;
		this.contentContainers = contentContainers;
	}

	public CodexEntry(string category, string titleKey, List<ContentContainer> contentContainers)
	{
		this.category = category;
		title = titleKey;
		this.contentContainers = contentContainers;
	}

	public ICodexWidget GetFirstWidget()
	{
		for (int i = 0; i < contentContainers.Count; i++)
		{
			if (contentContainers[i].content != null)
			{
				for (int j = 0; j < contentContainers[i].content.Count; j++)
				{
					if (contentContainers[i].content[j] != null)
					{
						return contentContainers[i].content[j];
					}
				}
			}
		}
		return null;
	}
}
