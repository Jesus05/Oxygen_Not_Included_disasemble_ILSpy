using FMOD.Studio;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KScrollRect : ScrollRect
{
	public enum SoundType
	{
		OnMouseScroll
	}

	public static Dictionary<SoundType, string> DefaultSounds = new Dictionary<SoundType, string>();

	private Dictionary<SoundType, string> currentSounds = new Dictionary<SoundType, string>();

	private float scrollVelocity = 0f;

	private bool default_intertia = true;

	private float default_elasticity = 0.2f;

	private float default_decelerationRate = 0.02f;

	private float verticalScrollInertiaScale = 10f;

	private float horizontalScrollInertiaScale = 5f;

	private float scrollDeceleration = 0.25f;

	[SerializeField]
	public bool forceContentMatchWidth = false;

	[SerializeField]
	public bool forceContentMatchHeight = false;

	[SerializeField]
	public bool allowHorizontalScrollWheel = true;

	[SerializeField]
	public bool allowVerticalScrollWheel = true;

	[SerializeField]
	public bool allowRightMouseScroll;

	private bool panUp = false;

	private bool panDown = false;

	private bool panRight = false;

	private bool panLeft = false;

	private Vector3 keyboardScrollDelta = default(Vector3);

	private float keyboardScrollSpeed = 1f;

	private bool startDrag = false;

	private bool stopDrag = false;

	private bool autoScrolling = false;

	private float autoScrollTargetVerticalPos;

	public bool isDragging
	{
		get;
		private set;
	}

	protected override void Awake()
	{
		base.Awake();
		base.elasticity = default_elasticity;
		base.inertia = default_intertia;
		base.decelerationRate = default_decelerationRate;
		base.scrollSensitivity = 1f;
		foreach (KeyValuePair<SoundType, string> defaultSound in DefaultSounds)
		{
			currentSounds[defaultSound.Key] = defaultSound.Value;
		}
	}

	public override void OnScroll(PointerEventData data)
	{
		if (base.vertical && allowVerticalScrollWheel)
		{
			float num = scrollVelocity;
			Vector2 scrollDelta = data.scrollDelta;
			scrollVelocity = num + scrollDelta.y * verticalScrollInertiaScale;
		}
		else if (base.horizontal && allowHorizontalScrollWheel)
		{
			float num2 = scrollVelocity;
			Vector2 scrollDelta2 = data.scrollDelta;
			scrollVelocity = num2 - scrollDelta2.y * horizontalScrollInertiaScale;
		}
		Vector2 scrollDelta3 = data.scrollDelta;
		if (Mathf.Abs(scrollDelta3.y) > 0.2f)
		{
			EventInstance instance = KFMOD.BeginOneShot(currentSounds[SoundType.OnMouseScroll], Vector3.zero);
			float boundsExceedAmount = GetBoundsExceedAmount();
			instance.setParameterValue("scrollbarPosition", boundsExceedAmount);
			KFMOD.EndOneShot(instance);
		}
	}

	private float GetBoundsExceedAmount()
	{
		if (base.vertical && (Object)base.verticalScrollbar != (Object)null)
		{
			RectTransform rectTransform = (!((Object)base.viewport == (Object)null)) ? base.viewport.rectTransform() : base.gameObject.GetComponent<RectTransform>();
			Vector2 size = rectTransform.rect.size;
			float y = size.y;
			Vector2 sizeDelta = base.content.sizeDelta;
			float num = Mathf.Min(y, sizeDelta.y);
			Vector2 sizeDelta2 = base.content.sizeDelta;
			float num2 = num / sizeDelta2.y;
			float num3 = Mathf.Abs(base.verticalScrollbar.size - num2);
			if (Mathf.Abs(num3) < 0.001f)
			{
				num3 = 0f;
			}
			return num3;
		}
		if (base.horizontal && (Object)base.horizontalScrollbar != (Object)null)
		{
			RectTransform rectTransform2 = (!((Object)base.viewport == (Object)null)) ? base.viewport.rectTransform() : base.gameObject.GetComponent<RectTransform>();
			Vector2 size2 = rectTransform2.rect.size;
			float x = size2.x;
			Vector2 sizeDelta3 = base.content.sizeDelta;
			float num4 = Mathf.Min(x, sizeDelta3.x);
			Vector2 sizeDelta4 = base.content.sizeDelta;
			float num5 = num4 / sizeDelta4.x;
			float num6 = Mathf.Abs(base.horizontalScrollbar.size - num5);
			if (Mathf.Abs(num6) < 0.001f)
			{
				num6 = 0f;
			}
			return num6;
		}
		return 0f;
	}

	public void SetSmoothAutoScrollTarget(float normalizedVerticalPos)
	{
		autoScrollTargetVerticalPos = normalizedVerticalPos;
		autoScrolling = true;
	}

	private void PlaySound(SoundType soundType)
	{
		if (currentSounds.ContainsKey(soundType))
		{
			KFMOD.PlayOneShot(currentSounds[soundType]);
		}
	}

	public void SetSound(SoundType soundType, string soundPath)
	{
		currentSounds[soundType] = soundPath;
	}

	public override void OnBeginDrag(PointerEventData eventData)
	{
		startDrag = true;
		base.OnBeginDrag(eventData);
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		stopDrag = true;
		base.OnEndDrag(eventData);
	}

	public override void OnDrag(PointerEventData eventData)
	{
		if (allowRightMouseScroll && (eventData.button == PointerEventData.InputButton.Right || eventData.button == PointerEventData.InputButton.Middle))
		{
			RectTransform content = base.content;
			Vector3 localPosition = base.content.localPosition;
			Vector2 delta = eventData.delta;
			float x = delta.x;
			Vector2 delta2 = eventData.delta;
			content.localPosition = localPosition + new Vector3(x, delta2.y);
			Vector2 normalizedPosition = base.normalizedPosition;
			float x2 = Mathf.Clamp(normalizedPosition.x, 0f, 1f);
			Vector2 normalizedPosition2 = base.normalizedPosition;
			base.normalizedPosition = new Vector2(x2, Mathf.Clamp(normalizedPosition2.y, 0f, 1f));
		}
		base.OnDrag(eventData);
		scrollVelocity = 0f;
	}

	protected override void LateUpdate()
	{
		UpdateScrollIntertia();
		if (allowRightMouseScroll)
		{
			if (panUp)
			{
				keyboardScrollDelta.y -= keyboardScrollSpeed;
			}
			if (panDown)
			{
				keyboardScrollDelta.y += keyboardScrollSpeed;
			}
			if (panLeft)
			{
				keyboardScrollDelta.x += keyboardScrollSpeed;
			}
			if (panRight)
			{
				keyboardScrollDelta.x -= keyboardScrollSpeed;
			}
			if (panUp || panDown || panLeft || panRight)
			{
				base.content.localPosition = base.content.localPosition + keyboardScrollDelta;
				Vector2 normalizedPosition = base.normalizedPosition;
				float x = Mathf.Clamp(normalizedPosition.x, 0f, 1f);
				Vector2 normalizedPosition2 = base.normalizedPosition;
				base.normalizedPosition = new Vector2(x, Mathf.Clamp(normalizedPosition2.y, 0f, 1f));
			}
		}
		if (startDrag)
		{
			startDrag = false;
			isDragging = true;
		}
		else if (stopDrag)
		{
			stopDrag = false;
			isDragging = false;
		}
		if (autoScrolling)
		{
			Vector2 normalizedPosition3 = base.normalizedPosition;
			float x2 = normalizedPosition3.x;
			Vector2 normalizedPosition4 = base.normalizedPosition;
			base.normalizedPosition = new Vector2(x2, Mathf.Lerp(normalizedPosition4.y, autoScrollTargetVerticalPos, Time.unscaledDeltaTime * 3f));
			float num = autoScrollTargetVerticalPos;
			Vector2 normalizedPosition5 = base.normalizedPosition;
			if (Mathf.Abs(num - normalizedPosition5.y) < 0.01f)
			{
				autoScrolling = false;
			}
		}
		base.LateUpdate();
	}

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		if (forceContentMatchWidth)
		{
			Vector2 sizeDelta = base.content.GetComponent<RectTransform>().sizeDelta;
			Vector2 sizeDelta2 = base.viewport.rectTransform().sizeDelta;
			sizeDelta.x = sizeDelta2.x;
			base.content.GetComponent<RectTransform>().sizeDelta = sizeDelta;
		}
		if (forceContentMatchHeight)
		{
			Vector2 sizeDelta3 = base.content.GetComponent<RectTransform>().sizeDelta;
			Vector2 sizeDelta4 = base.viewport.rectTransform().sizeDelta;
			sizeDelta3.y = sizeDelta4.y;
			base.content.GetComponent<RectTransform>().sizeDelta = sizeDelta3;
		}
	}

	private void UpdateScrollIntertia()
	{
		scrollVelocity *= 1f - Mathf.Clamp(scrollDeceleration, 0f, 1f);
		if (Mathf.Abs(scrollVelocity) < 0.001f)
		{
			scrollVelocity = 0f;
		}
		else
		{
			Vector2 anchoredPosition = base.content.anchoredPosition;
			if (base.vertical && allowVerticalScrollWheel)
			{
				anchoredPosition.y -= scrollVelocity;
			}
			if (base.horizontal && allowHorizontalScrollWheel)
			{
				anchoredPosition.x -= scrollVelocity;
			}
			if (base.content.anchoredPosition != anchoredPosition)
			{
				base.content.anchoredPosition = anchoredPosition;
			}
		}
		if (base.vertical && allowVerticalScrollWheel && (base.verticalNormalizedPosition < -0.05f || base.verticalNormalizedPosition > 1.05f))
		{
			scrollVelocity *= 0.9f;
		}
		if (base.horizontal && allowHorizontalScrollWheel && (base.horizontalNormalizedPosition < -0.05f || base.horizontalNormalizedPosition > 1.05f))
		{
			scrollVelocity *= 0.9f;
		}
	}

	public void OnKeyDown(KButtonEvent e)
	{
		if (allowRightMouseScroll)
		{
			if (e.TryConsume(Action.PanLeft))
			{
				panLeft = true;
			}
			else if (e.TryConsume(Action.PanRight))
			{
				panRight = true;
			}
			else if (e.TryConsume(Action.PanUp))
			{
				panUp = true;
			}
			else if (e.TryConsume(Action.PanDown))
			{
				panDown = true;
			}
		}
	}

	public void OnKeyUp(KButtonEvent e)
	{
		if (allowRightMouseScroll)
		{
			if (panUp && e.TryConsume(Action.PanUp))
			{
				panUp = false;
				keyboardScrollDelta.y = 0f;
			}
			else if (panDown && e.TryConsume(Action.PanDown))
			{
				panDown = false;
				keyboardScrollDelta.y = 0f;
			}
			else if (panRight && e.TryConsume(Action.PanRight))
			{
				panRight = false;
				keyboardScrollDelta.x = 0f;
			}
			else if (panLeft && e.TryConsume(Action.PanLeft))
			{
				panLeft = false;
				keyboardScrollDelta.x = 0f;
			}
		}
	}
}
