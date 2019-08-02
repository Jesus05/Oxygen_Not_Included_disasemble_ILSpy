using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KToggle : Toggle
{
	public delegate void PointerEvent();

	[SerializeField]
	public ToggleSoundPlayer soundPlayer;

	public Image bgImage;

	public Image fgImage;

	public KToggleArtExtensions artExtension;

	protected bool mouseOver;

	public bool GetMouseOver => mouseOver;

	public new bool isOn
	{
		get
		{
			return base.isOn;
		}
		set
		{
			base.isOn = value;
			OnValueChanged(base.isOn);
		}
	}

	public event System.Action onClick;

	public event System.Action onDoubleClick;

	public event Action<GameObject> onRefresh;

	public new event Action<bool> onValueChanged;

	public event PointerEvent onPointerEnter;

	public event PointerEvent onPointerExit;

	public void ClearOnClick()
	{
		this.onClick = null;
	}

	public void ClearPointerCallbacks()
	{
		this.onPointerEnter = null;
		this.onPointerExit = null;
	}

	public void ClearAllCallbacks()
	{
		ClearOnClick();
		ClearPointerCallbacks();
		this.onDoubleClick = null;
		this.onRefresh = null;
	}

	public void Click()
	{
		if (KInputManager.isFocused && IsInteractable() && !((UnityEngine.Object)UnityEngine.EventSystems.EventSystem.current == (UnityEngine.Object)null) && UnityEngine.EventSystems.EventSystem.current.enabled)
		{
			if (isOn)
			{
				Deselect();
				isOn = false;
			}
			else
			{
				Select();
				isOn = true;
			}
			if (soundPlayer.AcceptClickCondition != null && !soundPlayer.AcceptClickCondition())
			{
				soundPlayer.Play(3);
			}
			else
			{
				soundPlayer.Play((!isOn) ? 1 : 0);
			}
			base.gameObject.Trigger(2098165161, null);
			this.onClick.Signal();
		}
	}

	private void OnValueChanged(bool value)
	{
		if (IsInteractable())
		{
			ImageToggleState[] components = GetComponents<ImageToggleState>();
			if (components != null && components.Length > 0)
			{
				ImageToggleState[] array = components;
				foreach (ImageToggleState imageToggleState in array)
				{
					imageToggleState.SetActiveState(value);
				}
			}
			ActivateFlourish(value);
			this.onValueChanged.Signal(value);
		}
	}

	public void ForceUpdateVisualState()
	{
		ImageToggleState[] components = GetComponents<ImageToggleState>();
		if (components != null && components.Length > 0)
		{
			ImageToggleState[] array = components;
			foreach (ImageToggleState imageToggleState in array)
			{
				imageToggleState.ResetColor();
			}
		}
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		if (KInputManager.isFocused && eventData.button != PointerEventData.InputButton.Right && IsInteractable())
		{
			if (eventData.clickCount == 1 || this.onDoubleClick == null)
			{
				Click();
			}
			else if (eventData.clickCount == 2 && this.onDoubleClick != null)
			{
				this.onDoubleClick();
			}
		}
	}

	public override void OnDeselect(BaseEventData eventData)
	{
		ToggleGroup parentToggleGroup = GetParentToggleGroup(eventData);
		if ((UnityEngine.Object)parentToggleGroup == (UnityEngine.Object)base.group)
		{
			base.OnDeselect(eventData);
		}
	}

	public void Deselect()
	{
		base.OnDeselect(null);
	}

	public void ClearAnimState()
	{
		if ((UnityEngine.Object)artExtension.animator != (UnityEngine.Object)null && artExtension.animator.isInitialized)
		{
			Animator animator = artExtension.animator;
			animator.SetBool("Toggled", false);
			animator.Play("idle", 0);
		}
	}

	public override void OnSelect(BaseEventData eventData)
	{
		if ((UnityEngine.Object)base.group != (UnityEngine.Object)null)
		{
			foreach (KToggle item in base.group.ActiveToggles())
			{
				item.Deselect();
			}
			base.group.SetAllTogglesOff();
		}
		base.OnSelect(eventData);
	}

	public void ActivateFlourish(bool state)
	{
		if ((UnityEngine.Object)artExtension.animator != (UnityEngine.Object)null && artExtension.animator.isInitialized)
		{
			artExtension.animator.SetBool("Toggled", state);
		}
		if ((UnityEngine.Object)artExtension.SelectedFlourish != (UnityEngine.Object)null)
		{
			artExtension.SelectedFlourish.enabled = state;
		}
	}

	public void ActivateFlourish(bool state, ImageToggleState.State ImageState)
	{
		ImageToggleState[] components = GetComponents<ImageToggleState>();
		if (components != null && components.Length > 0)
		{
			ImageToggleState[] array = components;
			foreach (ImageToggleState imageToggleState in array)
			{
				imageToggleState.SetState(ImageState);
			}
		}
		ActivateFlourish(state);
	}

	private ToggleGroup GetParentToggleGroup(BaseEventData eventData)
	{
		PointerEventData pointerEventData = eventData as PointerEventData;
		if (pointerEventData == null)
		{
			return null;
		}
		GameObject gameObject = pointerEventData.pointerPressRaycast.gameObject;
		if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
		{
			return null;
		}
		Toggle componentInParent = gameObject.GetComponentInParent<Toggle>();
		if ((UnityEngine.Object)componentInParent == (UnityEngine.Object)null || (UnityEngine.Object)componentInParent.group == (UnityEngine.Object)null)
		{
			return null;
		}
		return componentInParent.group;
	}

	public void OnPointerEnter()
	{
		if (KInputManager.isFocused)
		{
			KInputManager.SetUserActive();
			ImageToggleState[] components = GetComponents<ImageToggleState>();
			if (components != null && components.Length > 0)
			{
				ImageToggleState[] array = components;
				foreach (ImageToggleState imageToggleState in array)
				{
					imageToggleState.OnHoverIn();
				}
			}
			soundPlayer.Play(2);
			mouseOver = true;
			if (this.onPointerEnter != null)
			{
				this.onPointerEnter();
			}
		}
	}

	public void OnPointerExit()
	{
		if (KInputManager.isFocused)
		{
			KInputManager.SetUserActive();
			ImageToggleState[] components = GetComponents<ImageToggleState>();
			if (components != null && components.Length > 0)
			{
				ImageToggleState[] array = components;
				foreach (ImageToggleState imageToggleState in array)
				{
					imageToggleState.OnHoverOut();
				}
			}
			mouseOver = false;
			if (this.onPointerExit != null)
			{
				this.onPointerExit();
			}
		}
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		if (KInputManager.isFocused)
		{
			OnPointerEnter();
			base.OnPointerEnter(eventData);
		}
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		if (KInputManager.isFocused)
		{
			OnPointerExit();
			base.OnPointerExit(eventData);
		}
	}
}
