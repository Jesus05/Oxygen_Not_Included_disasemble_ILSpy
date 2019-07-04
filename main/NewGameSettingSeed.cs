using Klei.CustomSettings;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewGameSettingSeed : NewGameSettingWidget
{
	[SerializeField]
	private LocText Label;

	[SerializeField]
	private ToolTip ToolTip;

	[SerializeField]
	private TMP_InputField Input;

	[SerializeField]
	private KButton RandomizeButton;

	[SerializeField]
	private Image BG;

	private const int MAX_VALID_SEED = int.MaxValue;

	private SeedSettingConfig config;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Input.onEndEdit.AddListener(OnEndEdit);
		Input.onValueChanged.AddListener(OnValueChanged);
		RandomizeButton.onClick += GetNewRandomSeed;
	}

	public void Initialize(SeedSettingConfig config)
	{
		this.config = config;
		Label.text = config.label;
		ToolTip.toolTip = config.tooltip;
		GetNewRandomSeed();
	}

	public override void Refresh()
	{
		string currentQualitySettingLevelId = CustomGameSettings.Instance.GetCurrentQualitySettingLevelId(config);
		Input.text = currentQualitySettingLevelId;
		DebugUtil.LogArgs("Set worldgen seed to", int.Parse(currentQualitySettingLevelId));
	}

	private char ValidateInput(string text, int charIndex, char addedChar)
	{
		return ('0' <= addedChar && addedChar <= '9') ? addedChar : '\0';
	}

	private void OnEndEdit(string text)
	{
		int seed;
		try
		{
			seed = Convert.ToInt32(text);
		}
		catch
		{
			seed = 0;
		}
		SetSeed(seed);
	}

	public void SetSeed(int seed)
	{
		seed = Mathf.Min(seed, 2147483647);
		CustomGameSettings.Instance.SetQualitySetting(config, seed.ToString());
		Refresh();
	}

	private void OnValueChanged(string text)
	{
		int num = 0;
		try
		{
			num = Convert.ToInt32(text);
		}
		catch
		{
			if (text.Length > 0)
			{
				Input.text = text.Substring(0, text.Length - 1);
			}
			else
			{
				Input.text = "";
			}
		}
		if (num > 2147483647)
		{
			Input.text = text.Substring(0, text.Length - 1);
		}
	}

	private void GetNewRandomSeed()
	{
		int seed = UnityEngine.Random.Range(0, 2147483647);
		SetSeed(seed);
	}
}
