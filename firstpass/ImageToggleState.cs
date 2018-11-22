using UnityEngine;
using UnityEngine.UI;

public class ImageToggleState : KMonoBehaviour
{
	public enum State
	{
		Disabled,
		Inactive,
		Active,
		DisabledActive
	}

	public Image TargetImage;

	public Sprite ActiveSprite;

	public Sprite InactiveSprite;

	public Sprite DisabledSprite;

	public Sprite DisabledActiveSprite;

	public bool useSprites = false;

	public Color ActiveColour = Color.white;

	public Color InactiveColour = Color.white;

	public Color DisabledColour = Color.white;

	public Color DisabledActiveColour = Color.white;

	public Color HoverColour = Color.white;

	public Color DisabledHoverColor = Color.white;

	public ColorStyleSetting colorStyleSetting;

	private bool isActive = false;

	private State currentState = State.Inactive;

	public bool useStartingState = false;

	public State startingState = State.Inactive;

	public bool IsDisabled => currentState == State.Disabled || currentState == State.DisabledActive;

	public new void Awake()
	{
		base.Awake();
		RefreshColorStyle();
		if (useStartingState)
		{
			SetState(startingState);
		}
	}

	[ContextMenu("Refresh Colour Style")]
	public void RefreshColorStyle()
	{
		if ((Object)colorStyleSetting != (Object)null)
		{
			ActiveColour = colorStyleSetting.activeColor;
			InactiveColour = colorStyleSetting.inactiveColor;
			DisabledColour = colorStyleSetting.disabledColor;
			DisabledActiveColour = colorStyleSetting.disabledActiveColor;
			HoverColour = colorStyleSetting.hoverColor;
			DisabledHoverColor = colorStyleSetting.disabledhoverColor;
		}
	}

	public void SetSprites(Sprite disabled, Sprite inactive, Sprite active, Sprite disabledActive)
	{
		if ((Object)disabled != (Object)null)
		{
			DisabledSprite = disabled;
		}
		if ((Object)inactive != (Object)null)
		{
			InactiveSprite = inactive;
		}
		if ((Object)active != (Object)null)
		{
			ActiveSprite = active;
		}
		if ((Object)disabledActive != (Object)null)
		{
			DisabledActiveSprite = disabledActive;
		}
		useSprites = true;
	}

	public bool GetIsActive()
	{
		return isActive;
	}

	private void SetTargetImageColor(Color color)
	{
		TargetImage.color = color;
	}

	public void SetState(State newState)
	{
		if (currentState != newState)
		{
			switch (newState)
			{
			case State.Inactive:
				SetInactive();
				break;
			case State.Active:
				SetActive();
				break;
			case State.Disabled:
				SetDisabled();
				break;
			case State.DisabledActive:
				SetDisabledActive();
				break;
			}
		}
	}

	public void SetActiveState(bool active)
	{
		if (active)
		{
			SetActive();
		}
		else
		{
			SetInactive();
		}
	}

	public void SetActive()
	{
		if (currentState != State.Active)
		{
			isActive = true;
			currentState = State.Active;
			if (!((Object)TargetImage == (Object)null))
			{
				SetTargetImageColor(ActiveColour);
				if (useSprites)
				{
					if ((Object)ActiveSprite != (Object)null && (Object)TargetImage.sprite != (Object)ActiveSprite)
					{
						TargetImage.sprite = ActiveSprite;
					}
					else if ((Object)ActiveSprite == (Object)null)
					{
						TargetImage.sprite = null;
					}
				}
			}
		}
	}

	public void SetColorStyle(ColorStyleSetting style)
	{
		colorStyleSetting = style;
		RefreshColorStyle();
		ResetColor();
	}

	public void ResetColor()
	{
		switch (currentState)
		{
		case State.Active:
			SetTargetImageColor(ActiveColour);
			break;
		case State.Inactive:
			SetTargetImageColor(InactiveColour);
			break;
		case State.Disabled:
			SetTargetImageColor(DisabledColour);
			break;
		case State.DisabledActive:
			SetTargetImageColor(DisabledActiveColour);
			break;
		}
	}

	public void OnHoverIn()
	{
		SetTargetImageColor((currentState != 0 && currentState != State.DisabledActive) ? HoverColour : DisabledHoverColor);
	}

	public void OnHoverOut()
	{
		ResetColor();
	}

	public void SetInactive()
	{
		if (currentState != State.Inactive)
		{
			isActive = false;
			currentState = State.Inactive;
			SetTargetImageColor(InactiveColour);
			if (!((Object)TargetImage == (Object)null) && useSprites)
			{
				if ((Object)InactiveSprite != (Object)null && (Object)TargetImage.sprite != (Object)InactiveSprite)
				{
					TargetImage.sprite = InactiveSprite;
				}
				else if ((Object)InactiveSprite == (Object)null)
				{
					TargetImage.sprite = null;
				}
			}
		}
	}

	public void SetDisabled()
	{
		if (currentState == State.Disabled)
		{
			SetTargetImageColor(DisabledColour);
		}
		else
		{
			isActive = false;
			currentState = State.Disabled;
			SetTargetImageColor(DisabledColour);
			if (!((Object)TargetImage == (Object)null) && useSprites)
			{
				if ((Object)DisabledSprite != (Object)null && (Object)TargetImage.sprite != (Object)DisabledSprite)
				{
					TargetImage.sprite = DisabledSprite;
				}
				else if ((Object)DisabledSprite == (Object)null)
				{
					TargetImage.sprite = null;
				}
			}
		}
	}

	public void SetDisabledActive()
	{
		isActive = false;
		currentState = State.DisabledActive;
		if (!((Object)TargetImage == (Object)null))
		{
			SetTargetImageColor(DisabledActiveColour);
			if (useSprites)
			{
				if ((Object)DisabledActiveSprite != (Object)null && (Object)TargetImage.sprite != (Object)DisabledActiveSprite)
				{
					TargetImage.sprite = DisabledActiveSprite;
				}
				else if ((Object)DisabledActiveSprite == (Object)null)
				{
					TargetImage.sprite = null;
				}
			}
		}
	}
}
