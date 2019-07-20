using System.Collections.Generic;

public class CategoryEntry : CodexEntry
{
	public List<CodexEntry> entriesInCategory = new List<CodexEntry>();

	public bool largeFormat
	{
		get;
		set;
	}

	public bool sort
	{
		get;
		set;
	}

	public CategoryEntry(string category, List<ContentContainer> contentContainers, string name, List<CodexEntry> entriesInCategory, bool largeFormat, bool sort)
		: base(category, contentContainers, name)
	{
		this.entriesInCategory = entriesInCategory;
		this.largeFormat = largeFormat;
		this.sort = sort;
	}
}
