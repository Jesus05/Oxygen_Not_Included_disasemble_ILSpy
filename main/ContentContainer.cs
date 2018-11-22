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

	public List<ICodexWidget> content
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
		content = new List<ICodexWidget>();
	}

	public ContentContainer(List<ICodexWidget> content, ContentLayout contentLayout)
	{
		this.content = content;
		this.contentLayout = contentLayout;
	}
}
