using Klei.CustomSettings;
using System;
using TMPro;
using UnityEngine;

public class ColonyDestinationSelectScreen : NewGameFlowScreen
{
	[SerializeField]
	private GameObject destinationMap;

	[SerializeField]
	private GameObject customSettings;

	[SerializeField]
	private KButton backButton;

	[SerializeField]
	private KButton customizeButton;

	[SerializeField]
	private KButton launchButton;

	[SerializeField]
	private KButton shuffleButton;

	[SerializeField]
	private AsteroidDescriptorPanel destinationProperties;

	[SerializeField]
	private AsteroidDescriptorPanel startLocationProperties;

	[SerializeField]
	private TMP_InputField coordinate;

	[MyCmpReq]
	private NewGameSettingsPanel newGameSettings;

	[MyCmpReq]
	private DestinationSelectPanel destinationMapPanel;

	private System.Random random;

	protected override void OnPrefabInit()
	{
		backButton.onClick += BackClicked;
		customizeButton.onClick += CustomizeClicked;
		launchButton.onClick += LaunchClicked;
		shuffleButton.onClick += ShuffleClicked;
		destinationMapPanel.OnAsteroidClicked += OnAsteroidClicked;
		random = new System.Random();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		newGameSettings.Init();
		newGameSettings.SetCloseAction(CustomizeClose);
		CustomGameSettings.Instance.OnSettingChanged += SettingChanged;
		ShuffleClicked();
	}

	protected override void OnCleanUp()
	{
		CustomGameSettings.Instance.OnSettingChanged -= SettingChanged;
		base.OnCleanUp();
	}

	private void BackClicked()
	{
		newGameSettings.Cancel();
		NavigateBackward();
	}

	private void CustomizeClicked()
	{
		newGameSettings.Refresh();
		customSettings.SetActive(true);
	}

	private void CustomizeClose()
	{
		customSettings.SetActive(false);
	}

	private void LaunchClicked()
	{
		NavigateForward();
	}

	private void ShuffleClicked()
	{
		int num = random.Next();
		newGameSettings.SetSetting(CustomGameSettingConfigs.WorldgenSeed, num.ToString());
	}

	private void SettingChanged(SettingConfig config, SettingLevel level)
	{
		coordinate.text = CustomGameSettings.Instance.GetSettingsCoordinate();
		string setting = newGameSettings.GetSetting(CustomGameSettingConfigs.World);
		string setting2 = newGameSettings.GetSetting(CustomGameSettingConfigs.WorldgenSeed);
		int.TryParse(setting2, out int result);
		ColonyDestinationAsteroidData colonyDestinationAsteroidData = destinationMapPanel.SelectAsteroid(setting, result);
		DebugUtil.LogArgs("Selected asteroid", setting, result);
		destinationProperties.SetDescriptors(colonyDestinationAsteroidData.GetParamDescriptors());
		startLocationProperties.SetDescriptors(colonyDestinationAsteroidData.GetTraitDescriptors());
	}

	private void OnAsteroidClicked(ColonyDestinationAsteroidData asteroid)
	{
		newGameSettings.SetSetting(CustomGameSettingConfigs.World, asteroid.worldPath);
		ShuffleClicked();
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed && e.TryConsume(Action.PanLeft))
		{
			destinationMapPanel.ScrollLeft();
		}
		else if (!e.Consumed && e.TryConsume(Action.PanRight))
		{
			destinationMapPanel.ScrollRight();
		}
		else if (customSettings.activeSelf && !e.Consumed && e.TryConsume(Action.Escape))
		{
			CustomizeClose();
		}
		base.OnKeyDown(e);
	}
}
