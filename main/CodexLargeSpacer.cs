using System.Collections.Generic;
using UnityEngine;

public class CodexLargeSpacer : CodexWidget<CodexLargeSpacer>
{
	public override void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		ConfigurePreferredLayout(contentGameObject);
	}
}
