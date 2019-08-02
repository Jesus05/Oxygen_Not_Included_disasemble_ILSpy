using System.Collections.Generic;
using UnityEngine;

public class CodexText : CodexWidget<CodexText>
{
	public string text
	{
		get;
		set;
	}

	public CodexTextStyle style
	{
		get;
		set;
	}

	public string stringKey
	{
		get
		{
			return "--> " + (text ?? "NULL");
		}
		set
		{
			text = Strings.Get(value);
		}
	}

	public CodexText()
	{
		style = CodexTextStyle.Body;
	}

	public CodexText(string text, CodexTextStyle style = CodexTextStyle.Body)
	{
		this.text = text;
		this.style = style;
	}

	public void ConfigureLabel(LocText label, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		label.gameObject.SetActive(true);
		label.AllowLinks = (style == CodexTextStyle.Body);
		label.textStyleSetting = textStyles[style];
		label.text = text;
		label.ApplySettings();
	}

	public override void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		ConfigureLabel(contentGameObject.GetComponent<LocText>(), textStyles);
		ConfigurePreferredLayout(contentGameObject);
	}
}
