using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultiToggle : KMonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	[Header("Settings")]
	[SerializeField]
	public ToggleState[] states;

	public bool play_sound_on_click = true;

	public Image toggle_image;

	protected int state;

	public System.Action onClick;

	public System.Action onEnter;

	public System.Action onExit;

	private bool pointerOver;

	public int CurrentState => state;

	public void NextState()
	{
		ChangeState((state + 1) % states.Length);
	}

	public void ChangeState(int new_state_index)
	{
		state = new_state_index;
		try
		{
			toggle_image.sprite = states[new_state_index].sprite;
			toggle_image.color = states[new_state_index].color;
			if (states[new_state_index].use_rect_margins)
			{
				toggle_image.rectTransform().sizeDelta = states[new_state_index].rect_margins;
			}
		}
		catch
		{
			string text = base.gameObject.name;
			Transform transform = base.transform;
			while ((UnityEngine.Object)transform.parent != (UnityEngine.Object)null)
			{
				text = text.Insert(0, transform.name + ">");
				transform = transform.parent;
			}
			Debug.LogError("Multi Toggle state index out of range: " + text + " idx:" + new_state_index, base.gameObject);
		}
		StatePresentationSetting[] additional_display_settings = states[state].additional_display_settings;
		for (int i = 0; i < additional_display_settings.Length; i++)
		{
			StatePresentationSetting statePresentationSetting = additional_display_settings[i];
			if (!((UnityEngine.Object)statePresentationSetting.image_target == (UnityEngine.Object)null))
			{
				statePresentationSetting.image_target.sprite = statePresentationSetting.sprite;
				statePresentationSetting.image_target.color = statePresentationSetting.color;
			}
		}
		RefreshHoverColor();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (states.Length - 1 < state)
		{
			Debug.LogWarning("Multi toggle has too few / no states", null);
		}
		if (play_sound_on_click)
		{
			if (states[state].on_click_override_sound_path == string.Empty)
			{
				KFMOD.PlayOneShot(GlobalAssets.GetSound("HUD_Click", false));
			}
			else
			{
				KFMOD.PlayOneShot(GlobalAssets.GetSound(states[state].on_click_override_sound_path, false));
			}
		}
		if (onClick != null)
		{
			onClick();
		}
		RefreshHoverColor();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		pointerOver = true;
		if (KInputManager.isFocused)
		{
			KInputManager.SetUserActive();
			if (states.Length != 0)
			{
				if (states[state].use_color_on_hover && states[state].color_on_hover != states[state].color)
				{
					toggle_image.color = states[state].color_on_hover;
				}
				if (states[state].use_rect_margins)
				{
					toggle_image.rectTransform().sizeDelta = states[state].rect_margins;
				}
				StatePresentationSetting[] additional_display_settings = states[state].additional_display_settings;
				for (int i = 0; i < additional_display_settings.Length; i++)
				{
					StatePresentationSetting statePresentationSetting = additional_display_settings[i];
					if (!((UnityEngine.Object)statePresentationSetting.image_target == (UnityEngine.Object)null) && statePresentationSetting.use_color_on_hover)
					{
						statePresentationSetting.image_target.color = statePresentationSetting.color_on_hover;
					}
				}
				if (onEnter != null)
				{
					onEnter();
				}
			}
		}
	}

	private void RefreshHoverColor()
	{
		if (pointerOver)
		{
			if (states[state].use_color_on_hover && states[state].color_on_hover != states[state].color)
			{
				toggle_image.color = states[state].color_on_hover;
			}
			StatePresentationSetting[] additional_display_settings = states[state].additional_display_settings;
			for (int i = 0; i < additional_display_settings.Length; i++)
			{
				StatePresentationSetting statePresentationSetting = additional_display_settings[i];
				if (!((UnityEngine.Object)statePresentationSetting.image_target == (UnityEngine.Object)null) && !((UnityEngine.Object)statePresentationSetting.image_target == (UnityEngine.Object)null) && statePresentationSetting.use_color_on_hover)
				{
					statePresentationSetting.image_target.color = statePresentationSetting.color_on_hover;
				}
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		pointerOver = false;
		if (KInputManager.isFocused)
		{
			KInputManager.SetUserActive();
			if (states.Length != 0)
			{
				if (states[state].use_color_on_hover && states[state].color_on_hover != states[state].color)
				{
					toggle_image.color = states[state].color;
				}
				if (states[state].use_rect_margins)
				{
					toggle_image.rectTransform().sizeDelta = states[state].rect_margins;
				}
				StatePresentationSetting[] additional_display_settings = states[state].additional_display_settings;
				for (int i = 0; i < additional_display_settings.Length; i++)
				{
					StatePresentationSetting statePresentationSetting = additional_display_settings[i];
					if (!((UnityEngine.Object)statePresentationSetting.image_target == (UnityEngine.Object)null) && statePresentationSetting.use_color_on_hover)
					{
						statePresentationSetting.image_target.color = statePresentationSetting.color;
					}
				}
				if (onExit != null)
				{
					onExit();
				}
			}
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
	}

	public void OnPointerUp(PointerEventData eventData)
	{
	}
}
