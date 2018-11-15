using Klei;
using KSerialization.Converters;
using System.Collections.Generic;

public class ContentContainer : YamlIO<ContentContainer>
{
	public enum ContentLayout
	{
		Vertical,
		Horizontal,
		Grid
	}

	public List<CodexWidget> content
	{
		get;
		set;
	}

	public string lockID
	{
		get;
		set;
	}

	[StringEnumConverter]
	public ContentLayout contentLayout
	{
		get;
		set;
	}

	public bool showBeforeGeneratedContent
	{
		get;
		set;
	}

	public ContentContainer()
	{
		content = new List<CodexWidget>();
	}

	public ContentContainer(List<CodexWidget> content, ContentLayout contentLayout)
	{
		this.content = content;
		this.contentLayout = contentLayout;
	}
}
