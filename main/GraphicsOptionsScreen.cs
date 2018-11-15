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
	private Toggle fullscreenToggle;

	[SerializeField]
	private KButton applyButton;

	[SerializeField]
	private KButton revertButton;

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

	private bool resDropdownAlwaysActive;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.LinuxEditor)
		{
			resDropdownAlwaysActive = true;
		}
		title.SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.TITLE);
		originalSettings = CaptureSettings();
		applyButton.isInteractable = false;
		applyButton.onClick += OnApply;
		applyButton.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.APPLYBUTTON);
		revertButton.isInteractable = false;
		revertButton.onClick += OnRevert;
		revertButton.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.REVERTBUTTON);
		doneButton.onClick += OnDone;
		closeButton.onClick += OnDone;
		doneButton.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.DONE_BUTTON);
		resolutionDropdown.ClearOptions();
		BuildOptions();
		resolutionDropdown.options = options;
		resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
		fullscreenToggle.isOn = Screen.fullScreen;
		fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggle);
		fullscreenToggle.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.FULLSCREEN);
		resolutionDropdown.interactable = (resDropdownAlwaysActive || fullscreenToggle.isOn);
		resolutionDropdown.transform.parent.GetComponentInChildren<LocText>().SetText(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.RESOLUTION);
		if (fullscreenToggle.isOn)
		{
			int resolutionIndex = GetResolutionIndex(originalSettings.resolution);
			if (resolutionIndex != -1)
			{
				resolutionDropdown.value = resolutionIndex;
			}
		}
		CanvasScalers = UnityEngine.Object.FindObjectsOfType<KCanvasScaler>();
		UpdateSliderLabel();
		uiScaleSlider.onValueChanged.AddListener(UpdateUIScale);
	}

	public static void SetResolutionFromPrefs()
	{
		if ((Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor) && KPlayerPrefs.HasKey(ResolutionWidthKey) && KPlayerPrefs.HasKey(ResolutionHeightKey))
		{
			int @int = KPlayerPrefs.GetInt(ResolutionWidthKey);
			int int2 = KPlayerPrefs.GetInt(ResolutionHeightKey);
			int int3 = KPlayerPrefs.GetInt(RefreshRateKey, Screen.currentResolution.refreshRate);
			bool fullscreen = (KPlayerPrefs.GetInt(FullScreenKey, Screen.fullScreen ? 1 : 0) == 1) ? true : false;
			Screen.SetResolution(@int, int2, fullscreen, int3);
		}
	}

	private void SaveResolutionToPrefs(Settings settings)
	{
		if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
		{
			KPlayerPrefs.SetInt(ResolutionWidthKey, settings.resolution.width);
			KPlayerPrefs.SetInt(ResolutionHeightKey, settings.resolution.height);
			KPlayerPrefs.SetInt(RefreshRateKey, settings.resolution.refreshRate);
			KPlayerPrefs.SetInt(FullScreenKey, settings.fullscreen ? 1 : 0);
		}
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
			new_settings.fullscreen = fullscreenToggle.isOn;
			ApplyConfirmSettings(new_settings, delegate
			{
				applyButton.isInteractable = false;
				revertButton.isInteractable = true;
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
			stringBuilder.Append("FullScreen: " + fullscreenToggle.isOn.ToString());
			Output.LogError(stringBuilder.ToString());
			throw ex;
		}
	}

	private void OnRevert()
	{
		ApplyConfirmSettings(originalSettings, delegate
		{
			applyButton.isInteractable = false;
			revertButton.isInteractable = false;
			SaveResolutionToPrefs(originalSettings);
		});
	}

	public void OnDone()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void RefreshApplyButton()
	{
		Settings settings = CaptureSettings();
		if (fullscreenToggle.isOn != settings.fullscreen)
		{
			applyButton.isInteractable = true;
		}
		else if (resDropdownAlwaysActive || fullscreenToggle.isOn)
		{
			int resolutionIndex = GetResolutionIndex(settings.resolution);
			applyButton.isInteractable = (resolutionDropdown.value != resolutionIndex);
		}
		else
		{
			applyButton.isInteractable = false;
		}
	}

	private void OnFullscreenToggle(bool enabled)
	{
		resolutionDropdown.interactable = (resDropdownAlwaysActive || fullscreenToggle.isOn);
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
		confirmDialog.PopupConfirmDialog(UI.FRONTEND.GRAPHICS_OPTIONS_SCREEN.ACCEPT_CHANGES.text, on_confirm, action, null, null, null, null, null, null);
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
