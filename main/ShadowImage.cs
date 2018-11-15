using UnityEngine;
using UnityEngine.UI;

public class ShadowImage : ShadowRect
{
	private Image shadowImage;

	private Image mainImage;

	protected override void MatchRect()
	{
		base.MatchRect();
		if (!((Object)RectMain == (Object)null) && !((Object)RectShadow == (Object)null))
		{
			if ((Object)shadowImage == (Object)null)
			{
				shadowImage = RectShadow.GetComponent<Image>();
			}
			if ((Object)mainImage == (Object)null)
			{
				mainImage = RectMain.GetComponent<Image>();
			}
			if ((Object)mainImage == (Object)null)
			{
				if ((Object)shadowImage != (Object)null)
				{
					shadowImage.color = Color.clear;
				}
			}
			else if (!((Object)shadowImage == (Object)null))
			{
				if ((Object)shadowImage.sprite != (Object)mainImage.sprite)
				{
					shadowImage.sprite = mainImage.sprite;
				}
				if (shadowImage.color != shadowColor)
				{
					if ((Object)shadowImage.sprite != (Object)null)
					{
						shadowImage.color = shadowColor;
					}
					else
					{
						shadowImage.color = Color.clear;
					}
				}
			}
		}
	}
}
