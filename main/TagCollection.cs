using System.Collections.Generic;

public class TagCollection : IReadonlyTags
{
	private HashSet<int> tags = new HashSet<int>();

	public TagCollection()
	{
	}

	public TagCollection(int[] initialTags)
	{
		for (int i = 0; i < initialTags.Length; i++)
		{
			tags.Add(initialTags[i]);
		}
	}

	public TagCollection(string[] initialTags)
	{
		for (int i = 0; i < initialTags.Length; i++)
		{
			tags.Add(Hash.SDBMLower(initialTags[i]));
		}
	}

	public TagCollection(TagCollection initialTags)
	{
		if (initialTags != null && initialTags.tags != null)
		{
			tags.UnionWith(initialTags.tags);
		}
	}

	public TagCollection Append(TagCollection others)
	{
		foreach (int tag in others.tags)
		{
			tags.Add(tag);
		}
		return this;
	}

	public void AddTag(string tag)
	{
		tags.Add(Hash.SDBMLower(tag));
	}

	public void AddTag(int tag)
	{
		tags.Add(tag);
	}

	public void RemoveTag(string tag)
	{
		tags.Remove(Hash.SDBMLower(tag));
	}

	public void RemoveTag(int tag)
	{
		tags.Remove(tag);
	}

	public bool HasTag(string tag)
	{
		return tags.Contains(Hash.SDBMLower(tag));
	}

	public bool HasTag(int tag)
	{
		return tags.Contains(tag);
	}

	public bool HasTags(int[] searchTags)
	{
		for (int i = 0; i < searchTags.Length; i++)
		{
			if (!tags.Contains(searchTags[i]))
			{
				return false;
			}
		}
		return true;
	}
}
