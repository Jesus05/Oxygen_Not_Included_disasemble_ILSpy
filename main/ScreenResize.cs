using System;
using UnityEngine;

public class ScreenResize : MonoBehaviour
{
	public System.Action OnResize;

	public static ScreenResize Instance;

	private int Width;

	private int Height;

	private bool isFullscreen;

	private void Awake()
	{
		Instance = this;
		isFullscreen = Screen.fullScreen;
		OnResize = (System.Action)Delegate.Combine(OnResize, new System.Action(SaveResolutionToPrefs));
	}

	private void LateUpdate()
	{
		if (Screen.width != Width || Screen.height != Height || isFullscreen != Screen.fullScreen)
		{
			Width = Screen.width;
			Height = Screen.height;
			isFullscreen = Screen.fullScreen;
			if (OnResize != null)
			{
				OnResize();
			}
		}
	}

	private void SaveResolutionToPrefs()
	{
		GraphicsOptionsScreen.OnResize();
	}
}
