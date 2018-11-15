using Klei.AI;
using UnityEngine;
using UnityEngine.UI;

public class ValueTrendImageToggle : MonoBehaviour
{
	public Image targetImage;

	public Sprite Up_One;

	public Sprite Up_Two;

	public Sprite Up_Three;

	public Sprite Down_One;

	public Sprite Down_Two;

	public Sprite Down_Three;

	public Sprite Zero;

	public void SetValue(AmountInstance ainstance)
	{
		float delta = ainstance.GetDelta();
		Sprite sprite = null;
		if (ainstance.paused || delta == 0f)
		{
			targetImage.gameObject.SetActive(false);
		}
		else
		{
			targetImage.gameObject.SetActive(true);
			if (delta <= (0f - ainstance.amount.visualDeltaThreshold) * 2f)
			{
				sprite = Down_Three;
			}
			else if (delta <= 0f - ainstance.amount.visualDeltaThreshold)
			{
				sprite = Down_Two;
			}
			else if (delta <= 0f)
			{
				sprite = Down_One;
			}
			else if (delta > ainstance.amount.visualDeltaThreshold * 2f)
			{
				sprite = Up_Three;
			}
			else if (delta > ainstance.amount.visualDeltaThreshold)
			{
				sprite = Up_Two;
			}
			else if (delta > 0f)
			{
				sprite = Up_One;
			}
		}
		targetImage.sprite = sprite;
	}
}
