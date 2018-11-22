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

	public override void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		icon.ConfigureImage(contentGameObject.GetComponentInChildren<Image>());
		label.ConfigureLabel(contentGameObject.GetComponentInChildren<LocText>(), textStyles);
	}
}
