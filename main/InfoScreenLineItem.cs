using UnityEngine;

public class InfoScreenLineItem : KMonoBehaviour
{
	[SerializeField]
	private LocText locText;

	[SerializeField]
	private ToolTip toolTip;

	private string text;

	private string tooltip;

	public void SetText(string text)
	{
		locText.text = text;
	}

	public void SetTooltip(string tooltip)
	{
		toolTip.toolTip = tooltip;
	}
}
