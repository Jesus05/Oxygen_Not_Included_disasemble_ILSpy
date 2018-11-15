using UnityEngine;
using UnityEngine.UI;

public class KImageButton : KButton
{
	public Text text;

	public Sprite Sprite
	{
		get
		{
			return fgImage.sprite;
		}
		set
		{
			fgImage.enabled = ((Object)value != (Object)null);
			fgImage.sprite = value;
		}
	}

	public Sprite BackgroundSprite
	{
		get
		{
			return bgImage.sprite;
		}
		set
		{
			bgImage.enabled = ((Object)value != (Object)null);
			bgImage.sprite = value;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		fgImage.enabled = false;
	}
}
