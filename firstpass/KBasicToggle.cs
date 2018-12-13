using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class KBasicToggle : KMonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	private const float DoubleClickTime = 0.15f;

	private bool _isOn;

	private bool didDoubleClick;

	private IEnumerator doubleClickCoroutine;

	public bool isOn
	{
		get
		{
			return _isOn;
		}
		set
		{
			_isOn = value;
			if (this.onValueChanged != null)
			{
				this.onValueChanged(value);
			}
		}
	}

	public event System.Action onClick;

	public event System.Action onDoubleClick;

	public event System.Action onPointerEnter;

	public event System.Action onPointerExit;

	public event Action<bool> onValueChanged;

	public void OnPointerClick(PointerEventData eventData)
	{
		if (doubleClickCoroutine != null && this.onDoubleClick != null)
		{
			this.onDoubleClick();
			didDoubleClick = true;
		}
		else
		{
			doubleClickCoroutine = DoubleClickTimer(eventData);
			StartCoroutine(doubleClickCoroutine);
		}
	}

	private IEnumerator DoubleClickTimer(PointerEventData eventData)
	{
		float startTime = Time.unscaledTime;
		if (Time.unscaledTime - startTime < 0.15f && !didDoubleClick)
		{
			yield return (object)null;
			/*Error: Unable to find new state assignment for yield return*/;
		}
		if (!didDoubleClick && this.onClick != null)
		{
			isOn = !isOn;
			this.onClick();
			this.onValueChanged(isOn);
		}
		doubleClickCoroutine = null;
		didDoubleClick = false;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (this.onPointerEnter != null)
		{
			this.onPointerEnter();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (this.onPointerExit != null)
		{
			this.onPointerExit();
		}
	}
}
