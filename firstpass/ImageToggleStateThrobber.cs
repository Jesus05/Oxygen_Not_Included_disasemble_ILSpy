using System.Collections.Generic;
using UnityEngine;

public class ImageToggleStateThrobber : KMonoBehaviour
{
	public ImageToggleState[] targetImageToggleStates;

	public ImageToggleState.State state1;

	public ImageToggleState.State state2;

	public float period = 2f;

	public bool useScaledTime = false;

	private float t = 0f;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		List<ImageToggleState> list = new List<ImageToggleState>(targetImageToggleStates);
		targetImageToggleStates = list.ToArray();
	}

	public void OnEnable()
	{
		t = 0f;
	}

	public void OnDisable()
	{
		ImageToggleState[] array = targetImageToggleStates;
		foreach (ImageToggleState imageToggleState in array)
		{
			imageToggleState.ResetColor();
		}
	}

	public void Update()
	{
		float num = (!useScaledTime) ? Time.unscaledDeltaTime : Time.deltaTime;
		t = (t + num) % period;
		float num2 = Mathf.Cos(t / period * 2f * 3.14159274f) * 0.5f + 0.5f;
		ImageToggleState[] array = targetImageToggleStates;
		foreach (ImageToggleState imageToggleState in array)
		{
			Color a = ColorForState(imageToggleState, state1);
			Color b = ColorForState(imageToggleState, state2);
			Color color = Color.Lerp(a, b, num2);
			imageToggleState.TargetImage.color = color;
		}
	}

	private Color ColorForState(ImageToggleState its, ImageToggleState.State state)
	{
		switch (state)
		{
		default:
			return its.ActiveColour;
		case ImageToggleState.State.Inactive:
			return its.InactiveColour;
		case ImageToggleState.State.Disabled:
			return its.DisabledColour;
		case ImageToggleState.State.DisabledActive:
			return its.DisabledActiveColour;
		}
	}
}
