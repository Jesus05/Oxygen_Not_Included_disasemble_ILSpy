using FMOD.Studio;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KSlider : Slider
{
	public enum SoundType
	{
		Start,
		Move,
		End,
		BoundaryLow,
		BoundaryHigh,
		Num
	}

	public enum MoveSource
	{
		Keyboard,
		MouseDrag,
		MouseClick,
		Num
	}

	public AnimationCurve sliderWeightCurve;

	public static string[] DefaultSounds = new string[5];

	private string[] currentSounds;

	private bool playSounds = true;

	private float lastMoveTime;

	private float movePlayRate = 0.025f;

	private float lastMoveValue;

	public bool playedBoundaryBump;

	private ToolTip tooltip;

	public event System.Action onReleaseHandle;

	public event System.Action onDrag;

	public event System.Action onPointerDown;

	public event System.Action onMove;

	private new void Awake()
	{
		currentSounds = new string[DefaultSounds.Length];
		for (int i = 0; i < DefaultSounds.Length; i++)
		{
			currentSounds[i] = DefaultSounds[i];
		}
		lastMoveTime = Time.unscaledTime;
		lastMoveValue = -1f;
		tooltip = base.handleRect.gameObject.GetComponent<ToolTip>();
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		PlayEndSound();
		if ((UnityEngine.Object)tooltip != (UnityEngine.Object)null)
		{
			tooltip.enabled = true;
			tooltip.OnPointerEnter(eventData);
		}
		if (this.onReleaseHandle != null)
		{
			this.onReleaseHandle();
		}
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		PlayStartSound();
		if (value != lastMoveValue)
		{
			PlayMoveSound(MoveSource.MouseClick);
		}
		if ((UnityEngine.Object)tooltip != (UnityEngine.Object)null)
		{
			tooltip.enabled = false;
		}
		if (this.onPointerDown != null)
		{
			this.onPointerDown();
		}
	}

	public override void OnDrag(PointerEventData eventData)
	{
		base.OnDrag(eventData);
		PlayMoveSound(MoveSource.MouseDrag);
		if (this.onDrag != null)
		{
			this.onDrag();
		}
	}

	public override void OnMove(AxisEventData eventData)
	{
		base.OnMove(eventData);
		PlayMoveSound(MoveSource.Keyboard);
		if (this.onMove != null)
		{
			this.onMove();
		}
	}

	public void ClearReleaseHandleEvent()
	{
		this.onReleaseHandle = null;
	}

	public void SetTooltipText(string tooltipText)
	{
		if ((UnityEngine.Object)tooltip != (UnityEngine.Object)null)
		{
			tooltip.SetSimpleTooltip(tooltipText);
		}
	}

	public void PlayStartSound()
	{
		if (KInputManager.isFocused && playSounds)
		{
			string text = currentSounds[0];
			if (text != null && text.Length > 0)
			{
				KFMOD.PlayOneShot(text);
			}
		}
	}

	public void PlayMoveSound(MoveSource moveSource)
	{
		if (KInputManager.isFocused && playSounds)
		{
			float num = Time.unscaledTime - lastMoveTime;
			if (!(num < movePlayRate))
			{
				if (moveSource != MoveSource.MouseDrag)
				{
					playedBoundaryBump = false;
				}
				float num2 = Mathf.InverseLerp(base.minValue, base.maxValue, value);
				string text = null;
				if (num2 == 1f && lastMoveValue == 1f)
				{
					if (!playedBoundaryBump)
					{
						text = currentSounds[4];
						playedBoundaryBump = true;
					}
				}
				else if (num2 == 0f && lastMoveValue == 0f)
				{
					if (!playedBoundaryBump)
					{
						text = currentSounds[3];
						playedBoundaryBump = true;
					}
				}
				else if (num2 >= 0f && num2 <= 1f)
				{
					text = currentSounds[1];
					playedBoundaryBump = false;
				}
				if (text != null && text.Length > 0)
				{
					lastMoveTime = Time.unscaledTime;
					lastMoveValue = num2;
					EventInstance instance = KFMOD.BeginOneShot(text, Vector3.zero);
					instance.setParameterValue("sliderValue", num2);
					instance.setParameterValue("timeSinceLast", num);
					KFMOD.EndOneShot(instance);
				}
			}
		}
	}

	public void PlayEndSound()
	{
		if (KInputManager.isFocused && playSounds)
		{
			string text = currentSounds[2];
			if (text != null && text.Length > 0)
			{
				EventInstance instance = KFMOD.BeginOneShot(text, Vector3.zero);
				instance.setParameterValue("sliderValue", value);
				KFMOD.EndOneShot(instance);
			}
		}
	}
}
