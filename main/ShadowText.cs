using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class ShadowText : ShadowRect
{
	private Text shadowText;

	private Text mainText;

	protected override void MatchRect()
	{
		if (!((Object)RectMain == (Object)null) && !((Object)RectShadow == (Object)null))
		{
			if ((Object)shadowText == (Object)null)
			{
				shadowText = RectShadow.GetComponent<Text>();
			}
			if ((Object)mainText == (Object)null)
			{
				mainText = RectMain.GetComponent<Text>();
			}
			if (!((Object)shadowText == (Object)null) && !((Object)mainText == (Object)null))
			{
				if ((Object)shadowText.font != (Object)mainText.font)
				{
					shadowText.font = mainText.font;
				}
				if (shadowText.fontSize != mainText.fontSize)
				{
					shadowText.fontSize = mainText.fontSize;
				}
				if (shadowText.alignment != mainText.alignment)
				{
					shadowText.alignment = mainText.alignment;
				}
				if (shadowText.lineSpacing != mainText.lineSpacing)
				{
					shadowText.lineSpacing = mainText.lineSpacing;
				}
				string text = mainText.text;
				text = Regex.Replace(text, "\\</?color\\b.*?\\>", string.Empty);
				if (shadowText.text != text)
				{
					shadowText.text = text;
				}
				if (shadowText.color != shadowColor)
				{
					shadowText.color = shadowColor;
				}
				if (shadowText.horizontalOverflow != mainText.horizontalOverflow)
				{
					shadowText.horizontalOverflow = mainText.horizontalOverflow;
				}
				if (shadowText.verticalOverflow != mainText.verticalOverflow)
				{
					shadowText.verticalOverflow = mainText.verticalOverflow;
				}
				base.MatchRect();
			}
		}
	}
}
