#define UNITY_ASSERTIONS
using System.Collections.Generic;
using UnityEngine.Assertions;

public class TagManager
{
	private static Dictionary<Tag, string> ProperNames = new Dictionary<Tag, string>();

	public static readonly Tag Invalid = default(Tag);

	public static Tag Create(string tag_string)
	{
		Tag tag = default(Tag);
		tag.Name = tag_string;
		if (!ProperNames.ContainsKey(tag))
		{
			ProperNames[tag] = "";
		}
		return tag;
	}

	public static Tag Create(string tag_string, string proper_name)
	{
		Tag tag = Create(tag_string);
		if (string.IsNullOrEmpty(proper_name))
		{
			DebugUtil.Assert(false, "Attempting to set proper name for tag: " + tag_string + "to null or empty.");
		}
		ProperNames[tag] = proper_name;
		return tag;
	}

	public static Tag[] Create(IList<string> strings)
	{
		Assert.IsTrue(strings != null && strings.Count > 0);
		Tag[] array = new Tag[strings.Count];
		for (int i = 0; i < strings.Count; i++)
		{
			array[i] = Create(strings[i]);
		}
		return array;
	}

	public static void FillMissingProperNames()
	{
		foreach (Tag item in new List<Tag>(ProperNames.Keys))
		{
			if (string.IsNullOrEmpty(ProperNames[item]))
			{
				ProperNames[item] = TagDescriptions.GetDescription(item.Name);
			}
		}
	}

	public static string GetProperName(Tag tag)
	{
		string value = null;
		if (!ProperNames.TryGetValue(tag, out value))
		{
			value = tag.Name;
		}
		return value;
	}
}
