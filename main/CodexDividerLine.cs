using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodexDividerLine : CodexWidget<CodexDividerLine>
{
	public override void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		contentGameObject.GetComponent<LayoutElement>().minWidth = 530f;
	}
}
