using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodexDividerLine : CodexWidget<CodexDividerLine>
{
	public override void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		LayoutElement component = contentGameObject.GetComponent<LayoutElement>();
		Vector2 sizeDelta = displayPane.rectTransform().sizeDelta;
		component.minWidth = sizeDelta.x - 64f;
	}
}
