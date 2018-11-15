using UnityEngine;
using UnityEngine.UI;

public class GenericUIProgressBar : KMonoBehaviour
{
	public Image fill;

	public LocText label;

	private float maxValue;

	public void SetMaxValue(float max)
	{
		maxValue = max;
	}

	public void SetFillPercentage(float value)
	{
		fill.fillAmount = value;
		label.text = Util.FormatWholeNumber(Mathf.Min(maxValue, maxValue * value)) + "/" + maxValue.ToString();
	}
}
