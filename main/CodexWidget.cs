using Klei;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CodexWidget<SubClass> : YamlIO<SubClass>, ICodexWidget
{
	public int preferredWidth
	{
		get;
		set;
	}

	public int preferredHeight
	{
		get;
		set;
	}

	protected CodexWidget()
	{
		preferredWidth = -1;
		preferredHeight = -1;
	}

	protected CodexWidget(int preferredWidth, int preferredHeight)
	{
		this.preferredWidth = preferredWidth;
		this.preferredHeight = preferredHeight;
	}

	public abstract void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles);

	protected void ConfigurePreferredLayout(GameObject contentGameObject)
	{
		LayoutElement componentInChildren = contentGameObject.GetComponentInChildren<LayoutElement>();
		componentInChildren.preferredHeight = (float)preferredHeight;
		componentInChildren.preferredWidth = (float)preferredWidth;
	}
}
