using System.Collections.Generic;

public static class TagExtensions
{
	public static Tag ToTag(this string str)
	{
		return new Tag(str);
	}

	public static Tag[] ToTagArray(this string[] strArray)
	{
		Tag[] array = new Tag[strArray.Length];
		for (int i = 0; i < strArray.Length; i++)
		{
			array[i] = strArray[i].ToTag();
		}
		return array;
	}

	public static List<Tag> ToTagList(this string[] strArray)
	{
		List<Tag> list = new List<Tag>();
		foreach (string str in strArray)
		{
			list.Add(str.ToTag());
		}
		return list;
	}

	public static List<Tag> ToTagList(this List<string> strList)
	{
		List<Tag> tagList = new List<Tag>();
		strList.ForEach(delegate(string str)
		{
			tagList.Add(str.ToTag());
		});
		return tagList;
	}
}
