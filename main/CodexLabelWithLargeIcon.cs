using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodexLabelWithLargeIcon : CodexLabelWithIcon
{
	public string linkID
	{
		get;
		set;
	}

	public CodexLabelWithLargeIcon(string text, CodexTextStyle style, Tuple<Sprite, Color> coloredSprite, string targetEntrylinkID)
		: base(text, style, coloredSprite, 128, 128)
	{
		base.icon = new CodexImage(128, 128, coloredSprite);
		base.label = new CodexText(text, style);
		linkID = targetEntrylinkID;
	}

	public override void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		base.icon.ConfigureImage(contentGameObject.GetComponentsInChildren<Image>()[1]);
		if (base.icon.preferredWidth != -1 && base.icon.preferredHeight != -1)
		{
			LayoutElement component = contentGameObject.GetComponentsInChildren<Image>()[1].GetComponent<LayoutElement>();
			component.minWidth = (float)base.icon.preferredHeight;
			component.minHeight = (float)base.icon.preferredWidth;
			component.preferredHeight = (float)base.icon.preferredHeight;
			component.preferredWidth = (float)base.icon.preferredWidth;
		}
		base.label.text = UI.StripLinkFormatting(base.label.text);
		base.label.ConfigureLabel(contentGameObject.GetComponentInChildren<LocText>(), textStyles);
		contentGameObject.GetComponent<KButton>().ClearOnClick();
		contentGameObject.GetComponent<KButton>().onClick += delegate
		{
			ManagementMenu.Instance.codexScreen.ChangeArticle(linkID, false);
		};
	}
}
