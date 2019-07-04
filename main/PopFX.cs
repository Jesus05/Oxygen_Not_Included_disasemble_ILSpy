using UnityEngine;
using UnityEngine.UI;

public class PopFX : KMonoBehaviour
{
	private float Speed = 2f;

	private Sprite icon;

	private string text;

	private Transform targetTransform;

	private Vector3 offset;

	public Image IconDisplay;

	public LocText TextDisplay;

	public CanvasGroup canvasGroup;

	private Camera uiCamera;

	private float lifetime;

	private float lifeElapsed = 0f;

	private bool trackTarget = false;

	private Vector3 startPos;

	private bool isLive = false;

	public void Recycle()
	{
		icon = null;
		text = "";
		targetTransform = null;
		lifeElapsed = 0f;
		trackTarget = false;
		startPos = Vector3.zero;
		IconDisplay.color = Color.white;
		TextDisplay.color = Color.white;
		PopFXManager.Instance.RecycleFX(this);
		canvasGroup.alpha = 0f;
		base.gameObject.SetActive(false);
		isLive = false;
	}

	public void Spawn(Sprite Icon, string Text, Transform TargetTransform, Vector3 Offset, float LifeTime = 1.5f, bool TrackTarget = false)
	{
		icon = Icon;
		text = Text;
		targetTransform = TargetTransform;
		trackTarget = TrackTarget;
		lifetime = LifeTime;
		offset = Offset;
		if ((Object)targetTransform != (Object)null)
		{
			startPos = targetTransform.GetPosition();
			Grid.PosToXY(startPos, out int _, out int y);
			if (y % 2 != 0)
			{
				startPos.x += 0.5f;
			}
		}
		TextDisplay.text = text;
		IconDisplay.sprite = icon;
		canvasGroup.alpha = 1f;
		isLive = true;
		Update();
	}

	private void Update()
	{
		if (isLive && PopFXManager.Instance.Ready())
		{
			lifeElapsed += Time.unscaledDeltaTime;
			if (lifeElapsed >= lifetime)
			{
				Recycle();
			}
			if (trackTarget && (Object)targetTransform != (Object)null)
			{
				Vector3 v = PopFXManager.Instance.WorldToScreen(targetTransform.GetPosition() + offset + Vector3.up * lifeElapsed * (Speed * lifeElapsed));
				v.z = 0f;
				base.gameObject.rectTransform().anchoredPosition = v;
			}
			else
			{
				Vector3 v2 = PopFXManager.Instance.WorldToScreen(startPos + offset + Vector3.up * lifeElapsed * (Speed * (lifeElapsed / 2f)));
				v2.z = 0f;
				base.gameObject.rectTransform().anchoredPosition = v2;
			}
			canvasGroup.alpha = 1.5f * ((lifetime - lifeElapsed) / lifetime);
		}
	}
}
