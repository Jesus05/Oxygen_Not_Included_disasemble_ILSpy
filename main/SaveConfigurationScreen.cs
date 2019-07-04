using STRINGS;
using System;
using UnityEngine;

[Serializable]
public class SaveConfigurationScreen
{
	[SerializeField]
	private KSlider autosaveFrequencySlider;

	[SerializeField]
	private LocText timelapseDescriptionLabel;

	[SerializeField]
	private KSlider timelapseResolutionSlider;

	[SerializeField]
	private LocText autosaveDescriptionLabel;

	private int[] sliderValueToCycleCount = new int[7]
	{
		-1,
		50,
		20,
		10,
		5,
		2,
		1
	};

	private Vector2I[] sliderValueToResolution = new Vector2I[6]
	{
		new Vector2I(320, 180),
		new Vector2I(640, 360),
		new Vector2I(1280, 720),
		new Vector2I(1920, 1080),
		new Vector2I(2560, 1440),
		new Vector2I(5120, 2880)
	};

	[SerializeField]
	private GameObject disabledContentPanel;

	[SerializeField]
	private GameObject disabledContentWarning;

	[SerializeField]
	private GameObject perSaveWarning;

	public void ToggleDisabledContent(bool enable)
	{
		if (enable)
		{
			disabledContentPanel.SetActive(true);
			disabledContentWarning.SetActive(false);
			perSaveWarning.SetActive(true);
		}
		else
		{
			disabledContentPanel.SetActive(false);
			disabledContentWarning.SetActive(true);
			perSaveWarning.SetActive(false);
		}
	}

	public void Init()
	{
		autosaveFrequencySlider.minValue = 0f;
		autosaveFrequencySlider.maxValue = (float)(sliderValueToCycleCount.Length - 1);
		autosaveFrequencySlider.onValueChanged.AddListener(delegate(float val)
		{
			OnAutosaveValueChanged(Mathf.FloorToInt(val));
		});
		autosaveFrequencySlider.value = (float)CycleCountToSlider(SaveGame.Instance.autoSaveCycleInterval);
		timelapseResolutionSlider.minValue = 0f;
		timelapseResolutionSlider.maxValue = (float)(sliderValueToResolution.Length - 1);
		timelapseResolutionSlider.onValueChanged.AddListener(delegate(float val)
		{
			OnTimelapseValueChanged(Mathf.FloorToInt(val));
		});
		timelapseResolutionSlider.value = (float)ResolutionToSliderValue(SaveGame.Instance.timelapseResolution);
	}

	public void Show(bool show)
	{
		if (show)
		{
			autosaveFrequencySlider.value = (float)CycleCountToSlider(SaveGame.Instance.autoSaveCycleInterval);
			timelapseResolutionSlider.value = (float)ResolutionToSliderValue(SaveGame.Instance.timelapseResolution);
			OnAutosaveValueChanged(Mathf.FloorToInt(autosaveFrequencySlider.value));
			OnTimelapseValueChanged(Mathf.FloorToInt(timelapseResolutionSlider.value));
		}
	}

	private void OnTimelapseValueChanged(int sliderValue)
	{
		Vector2I timelapseResolution = SliderValueToResolution(sliderValue);
		timelapseDescriptionLabel.SetText(string.Format(UI.FRONTEND.COLONY_SAVE_OPTIONS_SCREEN.TIMELAPSE_RESOLUTION_DESCRIPTION, timelapseResolution.x, timelapseResolution.y));
		SaveGame.Instance.timelapseResolution = timelapseResolution;
	}

	private void OnAutosaveValueChanged(int sliderValue)
	{
		int num = SliderValueToCycleCount(sliderValue);
		if (sliderValue == 0)
		{
			autosaveDescriptionLabel.SetText(UI.FRONTEND.COLONY_SAVE_OPTIONS_SCREEN.AUTOSAVE_NEVER);
		}
		else
		{
			autosaveDescriptionLabel.SetText(string.Format(UI.FRONTEND.COLONY_SAVE_OPTIONS_SCREEN.AUTOSAVE_FREQUENCY_DESCRIPTION, num));
		}
		SaveGame.Instance.autoSaveCycleInterval = num;
	}

	private int SliderValueToCycleCount(int sliderValue)
	{
		return sliderValueToCycleCount[sliderValue];
	}

	private int CycleCountToSlider(int count)
	{
		for (int i = 0; i < sliderValueToCycleCount.Length; i++)
		{
			if (sliderValueToCycleCount[i] == count)
			{
				return i;
			}
		}
		return 0;
	}

	private Vector2I SliderValueToResolution(int sliderValue)
	{
		return sliderValueToResolution[sliderValue];
	}

	private int ResolutionToSliderValue(Vector2I resolution)
	{
		for (int i = 0; i < sliderValueToResolution.Length; i++)
		{
			if (sliderValueToResolution[i] == resolution)
			{
				return i;
			}
		}
		return 0;
	}
}
