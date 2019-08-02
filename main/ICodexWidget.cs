using System.Collections.Generic;
using UnityEngine;

public interface ICodexWidget
{
	void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles);
}
