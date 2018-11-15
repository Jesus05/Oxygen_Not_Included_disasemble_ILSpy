using Klei;
using KSerialization.Converters;
using System.Collections.Generic;

public class CodexWidget : YamlIO<CodexWidget>
{
	public enum ContentType
	{
		Text,
		Image,
		DividerLine,
		Spacer,
		LabelWithIcon,
		ContentLockedIndicator,
		LargeSpacer,
		LENGTH
	}

	[StringEnumConverter]
	public ContentType type
	{
		get;
		set;
	}

	public Dictionary<string, string> properties
	{
		get;
		set;
	}

	public Dictionary<string, object> objectProperties
	{
		get;
		set;
	}

	public CodexWidget()
	{
		properties = new Dictionary<string, string>();
		objectProperties = new Dictionary<string, object>();
		properties["preferredWidth"] = "-1";
		properties["preferredHeight"] = "-1";
	}

	public CodexWidget(ContentType type)
		: this(type, new Dictionary<string, string>())
	{
	}

	public CodexWidget(ContentType type, Dictionary<string, string> properties, Dictionary<string, object> objectProperties)
	{
		this.type = type;
		this.properties = properties;
		this.objectProperties = objectProperties;
		if (!properties.ContainsKey("preferredWidth"))
		{
			properties["preferredWidth"] = "-1";
		}
		if (!properties.ContainsKey("preferredHeight"))
		{
			properties["preferredHeight"] = "-1";
		}
	}

	public CodexWidget(ContentType type, Dictionary<string, string> properties)
	{
		this.type = type;
		this.properties = properties;
		objectProperties = new Dictionary<string, object>();
		if (!properties.ContainsKey("preferredWidth"))
		{
			properties["preferredWidth"] = "-1";
		}
		if (!properties.ContainsKey("preferredHeight"))
		{
			properties["preferredHeight"] = "-1";
		}
	}
}
