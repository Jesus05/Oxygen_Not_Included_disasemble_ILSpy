using UnityEngine;
using UnityEngine.UI;

public class Slideshow : KMonoBehaviour
{
	public RawImage imageTarget;

	private Sprite[] sprites;

	public float timePerSlide = 1f;

	private int currentSlide;

	private float timeUntilNextSlide;

	public bool playInThumbnail;

	[SerializeField]
	private bool isExpandable;

	[SerializeField]
	private KButton button;

	[SerializeField]
	private bool transparentIfEmpty = true;

	[SerializeField]
	private KButton closeButton;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		timeUntilNextSlide = timePerSlide;
		if (transparentIfEmpty && sprites != null && sprites.Length == 0)
		{
			imageTarget.color = Color.clear;
		}
		if (isExpandable)
		{
			button = GetComponent<KButton>();
			button.onClick += delegate
			{
				VideoScreen.Instance.PlaySlideShow(sprites);
			};
		}
		if ((Object)closeButton != (Object)null)
		{
			closeButton.onClick += delegate
			{
				VideoScreen.Instance.Stop();
			};
		}
	}

	public void SetSprites(Sprite[] sprites)
	{
		this.sprites = sprites;
		timeUntilNextSlide = timePerSlide;
		currentSlide = 0;
		if (sprites.Length > 0 && (Object)sprites[0] != (Object)null)
		{
			imageTarget.color = Color.white;
			imageTarget.texture = sprites[0].texture;
			Vector2 fittedSize = GetFittedSize(960f, 960f);
			RectTransform component = GetComponent<RectTransform>();
			component.sizeDelta = fittedSize;
		}
		else if (transparentIfEmpty)
		{
			imageTarget.color = Color.clear;
		}
	}

	public Vector2 GetFittedSize(float maxWidth, float maxHeight)
	{
		if (sprites == null || (Object)sprites[0] == (Object)null || (Object)sprites[0].texture == (Object)null)
		{
			return Vector2.zero;
		}
		int width = sprites[0].texture.width;
		int height = sprites[0].texture.height;
		float num = maxWidth / (float)width;
		float num2 = maxHeight / (float)height;
		if (num < num2)
		{
			return new Vector2((float)width * num, (float)height * num);
		}
		return new Vector2((float)width * num2, (float)height * num2);
	}

	private void Update()
	{
		if (sprites != null && sprites.Length > 0)
		{
			timeUntilNextSlide -= Time.unscaledDeltaTime;
			if (timeUntilNextSlide <= 0f)
			{
				timeUntilNextSlide = timePerSlide;
				currentSlide = (currentSlide + 1) % sprites.Length;
				if (playInThumbnail && (Object)sprites[currentSlide] != (Object)null)
				{
					imageTarget.texture = sprites[currentSlide].texture;
				}
			}
		}
	}
}
