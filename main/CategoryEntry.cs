using System.Collections.Generic;

public class CategoryEntry : CodexEntry
{
	public List<CodexEntry> entriesInCategory = new List<CodexEntry>();

	public CategoryEntry(string category, List<ContentContainer> contentContainers, string name, List<CodexEntry> entriesInCategory)
		: base(category, contentContainers, name)
	{
		this.entriesInCategory = entriesInCategory;
	}
}
