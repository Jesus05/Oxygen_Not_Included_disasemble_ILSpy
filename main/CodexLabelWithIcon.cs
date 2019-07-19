using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodexLabelWithIcon : CodexWidget<CodexLabelWithIcon>
{
	public CodexImage icon
	{
		get;
		set;
	}

	public CodexText label
	{
		get;
		set;
	}

	public CodexLabelWithIcon(string text, CodexTextStyle style, Tuple<Sprite, Color> coloredSprite)
	{
		icon = new CodexImage(coloredSprite);
		label = new CodexText(text, style);
	}

	public CodexLabelWithIcon(string text, CodexTextStyle style, Tuple<Sprite, Color> coloredSprite, int iconWidth, int iconHeight)
	{
		icon = new CodexImage(iconWidth, iconHeight, coloredSprite);
		label = new CodexText(text, style);
	}

	public override void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		icon.ConfigureImage(contentGameObject.GetComponentInChildren<Image>());
		if (icon.preferredWidth != -1 && icon.preferredHeight != -1)
		{
			LayoutElement component = contentGameObject.GetComponentInChildren<Image>().GetComponent<LayoutElement>();
			component.minWidth = (float)icon.preferredHeight;
			component.minHeight = (float)icon.preferredWidth;
			component.preferredHeight = (float)icon.preferredHeight;
			component.preferredWidth = (float)icon.preferredWidth;
		}
		label.ConfigureLabel(contentGameObject.GetComponentInChildren<LocText>(), textStyles);
	}
}
