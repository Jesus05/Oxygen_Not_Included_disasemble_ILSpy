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
			int width = sprites[0].texture.width;
			int height = sprites[0].texture.height;
			float num = (float)width / (float)height;
			if (num > 1f)
			{
				float num2 = 960f / (float)width;
				RectTransform component = GetComponent<RectTransform>();
				component.sizeDelta = new Vector2((float)width * num2, (float)height * num2);
			}
			else
			{
				float num3 = 960f / (float)height;
				RectTransform component2 = GetComponent<RectTransform>();
				component2.sizeDelta = new Vector2((float)width * num3, (float)height * num3);
			}
		}
		else if (transparentIfEmpty)
		{
			imageTarget.color = Color.clear;
		}
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
