using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

internal class GraphicsOptionsScreen : KModalScreen
{
	private struct Settings
	{
		public bool fullscreen;

		public Resolution resolution;
	}

	[SerializeField]
	private Dropdown resolutionDropdown;

	[SerializeField]
	private MultiToggle fullscreenToggle;

	[SerializeField]
	private KButton applyButton;

	[SerializeField]
	private KButton doneButton;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private ConfirmDialogScreen confirmPrefab;

	[SerializeField]
	private KSlider uiScaleSlider;

	[SerializeField]
	private LocText sliderLabel;

	[SerializeField]
	private LocText title;

	public static readonly string ResolutionWidthKey = "ResolutionWidth";

	public static readonly string ResolutionHeightKey = "ResolutionHeight";

	public static readonly string RefreshRateKey = "RefreshRate";

	public static readonly string FullScreenKey = "FullScreen";

	private KCanvasScaler[] CanvasScalers;

	private ConfirmDialogScreen confirmDialog;

	private List<Resolution> resolutions = new List<Resolution>();

	private List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

	private Settings originalSettings;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		title.SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.TITLE);
		originalSettings = CaptureSettings();
		applyButton.isInteractable = false;
		applyButton.onClick += OnApply;
		applyButton.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.APPLYBUTTON);
		doneButton.onClick += OnDone;
		closeButton.onClick += OnDone;
		doneButton.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.DONE_BUTTON);
		resolutionDropdown.ClearOptions();
		BuildOptions();
		resolutionDropdown.options = options;
		resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
		fullscreenToggle.ChangeState(Screen.fullScreen ? 1 : 0);
		MultiToggle multiToggle = fullscreenToggle;
		multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, new System.Action(OnFullscreenToggle));
		fullscreenToggle.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.FULLSCREEN);
		resolutionDropdown.transform.parent.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.RESOLUTION);
		if (fullscreenToggle.CurrentState == 1)
		{
			int resolutionIndex = GetResolutionIndex(originalSettings.resolution);
			if (resolutionIndex != -1)
			{
				resolutionDropdown.value = resolutionIndex;
			}
		}
		CanvasScalers = UnityEngine.Object.FindObjectsOfType<KCanvasScaler>();
		UpdateSliderLabel();
		uiScaleSlider.onValueChanged.AddListener(delegate
		{
			sliderLabel.text = uiScaleSlider.value + "%";
		});
		uiScaleSlider.onReleaseHandle += delegate
		{
			UpdateUIScale(uiScaleSlider.value);
		};
	}

	public static void SetResolutionFromPrefs()
	{
		int num = Screen.currentResolution.width;
		int num2 = Screen.currentResolution.height;
		int num3 = Screen.currentResolution.refreshRate;
		bool flag = Screen.fullScreen;
		DebugUtil.LogArgs($"Starting up with a resolution of {num}x{num2} @{num3}hz (fullscreen: {flag})");
		if (KPlayerPrefs.HasKey(ResolutionWidthKey) && KPlayerPrefs.HasKey(ResolutionHeightKey))
		{
			int @int = KPlayerPrefs.GetInt(ResolutionWidthKey);
			int int2 = KPlayerPrefs.GetInt(ResolutionHeightKey);
			int int3 = KPlayerPrefs.GetInt(RefreshRateKey, Screen.currentResolution.refreshRate);
			bool flag2 = (KPlayerPrefs.GetInt(FullScreenKey, Screen.fullScreen ? 1 : 0) == 1) ? true : false;
			DebugUtil.LogArgs($"Found player prefs resolution {@int}x{int2} @{int3}hz (fullscreen: {flag2})");
			if (int2 <= 1 || @int <= 1)
			{
				DebugUtil.LogArgs("Saved resolution was invalid, ignoring...");
			}
			else
			{
				num = @int;
				num2 = int2;
				num3 = int3;
				flag = flag2;
			}
		}
		if (num <= 1 || num2 <= 1)
		{
			DebugUtil.LogWarningArgs("Detected a degenerate resolution, attempting to fix...");
			Resolution[] array = Screen.resolutions;
			for (int i = 0; i < array.Length; i++)
			{
				Resolution resolution = array[i];
				if (resolution.width == 1920)
				{
					num = resolution.width;
					num2 = resolution.height;
					num3 = 0;
				}
			}
			if (num <= 1 || num2 <= 1)
			{
				Resolution[] array2 = Screen.resolutions;
				for (int j = 0; j < array2.Length; j++)
				{
					Resolution resolution2 = array2[j];
					if (resolution2.width == 1280)
					{
						num = resolution2.width;
						num2 = resolution2.height;
						num3 = 0;
					}
				}
			}
			if (num <= 1 || num2 <= 1)
			{
				Resolution[] array3 = Screen.resolutions;
				for (int k = 0; k < array3.Length; k++)
				{
					Resolution resolution3 = array3[k];
					if (resolution3.width > 1 && resolution3.height > 1 && resolution3.refreshRate > 0)
					{
						num = resolution3.width;
						num2 = resolution3.height;
						num3 = 0;
					}
				}
			}
			if (num <= 1 || num2 <= 1)
			{
				string text = "Could not find a suitable resolution for this screen! Reported available resolutions are:";
				Resolution[] array4 = Screen.resolutions;
				for (int l = 0; l < array4.Length; l++)
				{
					Resolution resolution4 = array4[l];
					text += $"\n{resolution4.width}x{resolution4.height} @ {resolution4.refreshRate}hz";
				}
				Debug.LogError(text);
				num = 1280;
				num2 = 720;
				flag = false;
				num3 = 0;
			}
		}
		DebugUtil.LogArgs($"Applying resolution {num}x{num2} @{num3}hz (fullscreen: {flag})");
		Screen.SetResolution(num, num2, flag, num3);
	}

	public static void OnResize()
	{
		Settings settings = default(Settings);
		settings.resolution = Screen.currentResolution;
		settings.resolution.width = Screen.width;
		settings.resolution.height = Screen.height;
		settings.fullscreen = Screen.fullScreen;
		SaveResolutionToPrefs(settings);
	}

	private static void SaveResolutionToPrefs(Settings settings)
	{
		Debug.LogFormat("Screen resolution updated, saving values to prefs: {0}x{1} @ {2}, fullscreen: {3}", settings.resolution.width, settings.resolution.height, settings.resolution.refreshRate, settings.fullscreen);
		KPlayerPrefs.SetInt(ResolutionWidthKey, settings.resolution.width);
		KPlayerPrefs.SetInt(ResolutionHeightKey, settings.resolution.height);
		KPlayerPrefs.SetInt(RefreshRateKey, settings.resolution.refreshRate);
		KPlayerPrefs.SetInt(FullScreenKey, settings.fullscreen ? 1 : 0);
	}

	private void UpdateUIScale(float value)
	{
		KCanvasScaler[] canvasScalers = CanvasScalers;
		foreach (KCanvasScaler kCanvasScaler in canvasScalers)
		{
			float userScale = value / 100f;
			kCanvasScaler.SetUserScale(userScale);
			KPlayerPrefs.SetFloat(KCanvasScaler.UIScalePrefKey, value);
		}
		UpdateSliderLabel();
	}

	private void UpdateSliderLabel()
	{
		if (CanvasScalers != null && CanvasScalers.Length > 0 && (UnityEngine.Object)CanvasScalers[0] != (UnityEngine.Object)null)
		{
			uiScaleSlider.value = CanvasScalers[0].GetUserScale() * 100f;
			sliderLabel.text = uiScaleSlider.value + "%";
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
		{
			resolutionDropdown.Hide();
			Deactivate();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	private void BuildOptions()
	{
		options.Clear();
		resolutions.Clear();
		Resolution[] array = Screen.resolutions;
		for (int i = 0; i < array.Length; i++)
		{
			Resolution item = array[i];
			if (item.height >= 720)
			{
				options.Add(new Dropdown.OptionData(item.ToString()));
				resolutions.Add(item);
			}
		}
	}

	private int GetResolutionIndex(Resolution resolution)
	{
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < resolutions.Count; i++)
		{
			Resolution resolution2 = resolutions[i];
			if (resolution2.width == resolution.width && resolution2.height == resolution.height && resolution2.refreshRate == 0)
			{
				num2 = i;
			}
			if (resolution2.width == resolution.width && resolution2.height == resolution.height && Math.Abs(resolution2.refreshRate - resolution.refreshRate) <= 1)
			{
				num = i;
				break;
			}
		}
		return (num != -1) ? num : num2;
	}

	private Settings CaptureSettings()
	{
		Settings result = default(Settings);
		result.fullscreen = Screen.fullScreen;
		Resolution resolution = default(Resolution);
		resolution.width = Screen.width;
		resolution.height = Screen.height;
		resolution.refreshRate = Screen.currentResolution.refreshRate;
		result.resolution = resolution;
		return result;
	}

	private void OnApply()
	{
		try
		{
			Settings new_settings = default(Settings);
			new_settings.resolution = resolutions[resolutionDropdown.value];
			new_settings.fullscreen = ((fullscreenToggle.CurrentState != 0) ? true : false);
			ApplyConfirmSettings(new_settings, delegate
			{
				applyButton.isInteractable = false;
				SaveResolutionToPrefs(new_settings);
			});
		}
		catch (Exception ex)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Failed to apply graphics options!\nResolutions:");
			foreach (Resolution resolution in resolutions)
			{
				stringBuilder.Append("\t" + resolution.ToString() + "\n");
			}
			stringBuilder.Append("Selected Resolution Idx: " + resolutionDropdown.value.ToString());
			stringBuilder.Append("FullScreen: " + fullscreenToggle.CurrentState.ToString());
			Debug.LogError(stringBuilder.ToString());
			throw ex;
		}
	}

	public void OnDone()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void RefreshApplyButton()
	{
		Settings settings = CaptureSettings();
		if (settings.fullscreen && fullscreenToggle.CurrentState == 0)
		{
			applyButton.isInteractable = true;
		}
		else if (!settings.fullscreen && fullscreenToggle.CurrentState == 1)
		{
			applyButton.isInteractable = true;
		}
		else
		{
			int resolutionIndex = GetResolutionIndex(settings.resolution);
			applyButton.isInteractable = (resolutionDropdown.value != resolutionIndex);
		}
	}

	private void OnFullscreenToggle()
	{
		fullscreenToggle.ChangeState((fullscreenToggle.CurrentState == 0) ? 1 : 0);
		RefreshApplyButton();
	}

	private void OnResolutionChanged(int idx)
	{
		RefreshApplyButton();
	}

	private void ApplyConfirmSettings(Settings new_settings, System.Action on_confirm)
	{
		Settings current_settings = CaptureSettings();
		ApplySettings(new_settings);
		confirmDialog = Util.KInstantiateUI(confirmPrefab.gameObject, base.transform.gameObject, false).GetComponent<ConfirmDialogScreen>();
		System.Action action = delegate
		{
			ApplySettings(current_settings);
		};
		Coroutine timer = StartCoroutine(Timer(15f, action));
		confirmDialog.onDeactivateCB = delegate
		{
			StopCoroutine(timer);
		};
		confirmDialog.PopupConfirmDialog(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.ACCEPT_CHANGES.text, on_confirm, action, null, null, null, null, null, null, true);
		confirmDialog.gameObject.SetActive(true);
	}

	private void ApplySettings(Settings new_settings)
	{
		Resolution resolution = new_settings.resolution;
		Screen.SetResolution(resolution.width, resolution.height, new_settings.fullscreen, resolution.refreshRate);
		Screen.fullScreen = new_settings.fullscreen;
		int resolutionIndex = GetResolutionIndex(new_settings.resolution);
		if (resolutionIndex != -1)
		{
			resolutionDropdown.value = resolutionIndex;
		}
	}

	private IEnumerator Timer(float time, System.Action revert)
	{
		yield return (object)new WaitForSeconds(time);
		/*Error: Unable to find new state assignment for yield return*/;
	}

	private void Update()
	{
		Debug.developerConsoleVisible = false;
	}
}
