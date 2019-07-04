using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodexImage : CodexWidget<CodexImage>
{
	public Sprite sprite
	{
		get;
		set;
	}

	public Color color
	{
		get;
		set;
	}

	public string spriteName
	{
		get
		{
			return "--> " + ((!((Object)sprite == (Object)null)) ? sprite.ToString() : "NULL");
		}
		set
		{
			sprite = Assets.GetSprite(value);
		}
	}

	public string batchedAnimPrefabSourceID
	{
		get
		{
			return "--> " + ((!((Object)sprite == (Object)null)) ? sprite.ToString() : "NULL");
		}
		set
		{
			GameObject prefab = Assets.GetPrefab(value);
			KBatchedAnimController kBatchedAnimController = (!((Object)prefab != (Object)null)) ? null : prefab.GetComponent<KBatchedAnimController>();
			KAnimFile kAnimFile = (!((Object)kBatchedAnimController != (Object)null)) ? null : kBatchedAnimController.AnimFiles[0];
			sprite = ((!((Object)kAnimFile != (Object)null)) ? null : Def.GetUISpriteFromMultiObjectAnim(kAnimFile, "ui", false, ""));
		}
	}

	public CodexImage()
	{
	}

	public CodexImage(int preferredWidth, int preferredHeight, Sprite sprite, Color color)
		: base(preferredWidth, preferredHeight)
	{
		this.sprite = sprite;
		this.color = color;
	}

	public CodexImage(int preferredWidth, int preferredHeight, Sprite sprite)
		: this(preferredWidth, preferredHeight, sprite, Color.white)
	{
	}

	public CodexImage(int preferredWidth, int preferredHeight, Tuple<Sprite, Color> coloredSprite)
		: this(preferredWidth, preferredHeight, coloredSprite.first, coloredSprite.second)
	{
	}

	public CodexImage(Tuple<Sprite, Color> coloredSprite)
		: this(-1, -1, coloredSprite)
	{
	}

	public void ConfigureImage(Image image)
	{
		image.sprite = sprite;
		image.color = color;
	}

	public override void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		ConfigureImage(contentGameObject.GetComponent<Image>());
		ConfigurePreferredLayout(contentGameObject);
	}
}
