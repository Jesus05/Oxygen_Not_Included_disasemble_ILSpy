using FMODUnity;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioOptionsScreen : KModalScreen
{
	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton doneButton;

	[SerializeField]
	private SliderContainer sliderPrefab;

	[SerializeField]
	private GameObject sliderGroup;

	[SerializeField]
	private Image jambell;

	[SerializeField]
	private GameObject alwaysPlayMusicButton;

	[SerializeField]
	private GameObject alwaysPlayAutomationButton;

	[SerializeField]
	private GameObject muteOnFocusLostToggle;

	[SerializeField]
	private Dropdown deviceDropdown;

	private UIPool<SliderContainer> sliderPool;

	private Dictionary<KSlider, string> sliderBusMap = new Dictionary<KSlider, string>();

	public static readonly string AlwaysPlayMusicKey = "AlwaysPlayMusic";

	public static readonly string AlwaysPlayAutomation = "AlwaysPlayAutomation";

	public static readonly string MuteOnFocusLost = "MuteOnFocusLost";

	private Dictionary<string, object> alwaysPlayMusicMetric = new Dictionary<string, object>
	{
		{
			AlwaysPlayMusicKey,
			null
		}
	};

	private List<KFMOD.AudioDevice> audioDevices = new List<KFMOD.AudioDevice>();

	private List<Dropdown.OptionData> audioDeviceOptions = new List<Dropdown.OptionData>();

	protected override void OnSpawn()
	{
		base.OnSpawn();
		closeButton.onClick += delegate
		{
			OnClose(base.gameObject);
		};
		doneButton.onClick += delegate
		{
			OnClose(base.gameObject);
		};
		sliderPool = new UIPool<SliderContainer>(sliderPrefab);
		Dictionary<string, AudioMixer.UserVolumeBus> userVolumeSettings = AudioMixer.instance.userVolumeSettings;
		foreach (KeyValuePair<string, AudioMixer.UserVolumeBus> item in userVolumeSettings)
		{
			SliderContainer newSlider = sliderPool.GetFreeElement(sliderGroup, true);
			sliderBusMap.Add(newSlider.slider, item.Key);
			newSlider.slider.value = item.Value.busLevel;
			newSlider.nameLabel.text = item.Value.labelString;
			newSlider.UpdateSliderLabel(item.Value.busLevel);
			newSlider.slider.ClearReleaseHandleEvent();
			newSlider.slider.onValueChanged.AddListener(delegate
			{
				OnReleaseHandle(newSlider.slider);
			});
			if (item.Key == "Master")
			{
				newSlider.transform.SetSiblingIndex(1);
				newSlider.slider.onValueChanged.AddListener(CheckMasterValue);
				CheckMasterValue(item.Value.busLevel);
			}
		}
		HierarchyReferences component = alwaysPlayMusicButton.GetComponent<HierarchyReferences>();
		GameObject gameObject = component.GetReference("Button").gameObject;
		gameObject.GetComponent<ToolTip>().SetSimpleTooltip(UI.FRONTEND.AUDIO_OPTIONS_SCREEN.MUSIC_EVERY_CYCLE_TOOLTIP);
		component.GetReference("CheckMark").gameObject.SetActive(MusicManager.instance.alwaysPlayMusic);
		gameObject.GetComponent<KButton>().onClick += delegate
		{
			ToggleAlwaysPlayMusic();
		};
		LocText reference = component.GetReference<LocText>("Label");
		reference.SetText(UI.FRONTEND.AUDIO_OPTIONS_SCREEN.MUSIC_EVERY_CYCLE);
		if (!KPlayerPrefs.HasKey(AlwaysPlayAutomation))
		{
			KPlayerPrefs.SetInt(AlwaysPlayAutomation, 1);
		}
		HierarchyReferences component2 = alwaysPlayAutomationButton.GetComponent<HierarchyReferences>();
		GameObject gameObject2 = component2.GetReference("Button").gameObject;
		gameObject2.GetComponent<ToolTip>().SetSimpleTooltip(UI.FRONTEND.AUDIO_OPTIONS_SCREEN.AUTOMATION_SOUNDS_ALWAYS_TOOLTIP);
		gameObject2.GetComponent<KButton>().onClick += delegate
		{
			ToggleAlwaysPlayAutomation();
		};
		LocText reference2 = component2.GetReference<LocText>("Label");
		reference2.SetText(UI.FRONTEND.AUDIO_OPTIONS_SCREEN.AUTOMATION_SOUNDS_ALWAYS);
		component2.GetReference("CheckMark").gameObject.SetActive((KPlayerPrefs.GetInt(AlwaysPlayAutomation) == 1) ? true : false);
		if (!KPlayerPrefs.HasKey(MuteOnFocusLost))
		{
			KPlayerPrefs.SetInt(MuteOnFocusLost, 0);
		}
		HierarchyReferences component3 = muteOnFocusLostToggle.GetComponent<HierarchyReferences>();
		GameObject gameObject3 = component3.GetReference("Button").gameObject;
		gameObject3.GetComponent<ToolTip>().SetSimpleTooltip(UI.FRONTEND.AUDIO_OPTIONS_SCREEN.MUTE_ON_FOCUS_LOST_TOOLTIP);
		gameObject3.GetComponent<KButton>().onClick += delegate
		{
			ToggleMuteOnFocusLost();
		};
		LocText reference3 = component3.GetReference<LocText>("Label");
		reference3.SetText(UI.FRONTEND.AUDIO_OPTIONS_SCREEN.MUTE_ON_FOCUS_LOST);
		component3.GetReference("CheckMark").gameObject.SetActive((KPlayerPrefs.GetInt(MuteOnFocusLost) == 1) ? true : false);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
		{
			Deactivate();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	private void CheckMasterValue(float value)
	{
		jambell.enabled = (value == 0f);
	}

	private void OnReleaseHandle(KSlider slider)
	{
		AudioMixer.instance.SetUserVolume(sliderBusMap[slider], slider.value);
	}

	private void ToggleAlwaysPlayMusic()
	{
		MusicManager.instance.alwaysPlayMusic = !MusicManager.instance.alwaysPlayMusic;
		alwaysPlayMusicButton.GetComponent<HierarchyReferences>().GetReference("CheckMark").gameObject.SetActive(MusicManager.instance.alwaysPlayMusic);
		KPlayerPrefs.SetInt(AlwaysPlayMusicKey, MusicManager.instance.alwaysPlayMusic ? 1 : 0);
	}

	private void ToggleAlwaysPlayAutomation()
	{
		KPlayerPrefs.SetInt(AlwaysPlayAutomation, (KPlayerPrefs.GetInt(AlwaysPlayAutomation) != 1) ? 1 : 0);
		alwaysPlayAutomationButton.GetComponent<HierarchyReferences>().GetReference("CheckMark").gameObject.SetActive((KPlayerPrefs.GetInt(AlwaysPlayAutomation) == 1) ? true : false);
	}

	private void ToggleMuteOnFocusLost()
	{
		KPlayerPrefs.SetInt(MuteOnFocusLost, (KPlayerPrefs.GetInt(MuteOnFocusLost) != 1) ? 1 : 0);
		muteOnFocusLostToggle.GetComponent<HierarchyReferences>().GetReference("CheckMark").gameObject.SetActive((KPlayerPrefs.GetInt(MuteOnFocusLost) == 1) ? true : false);
	}

	private void BuildAudioDeviceList()
	{
		audioDevices.Clear();
		audioDeviceOptions.Clear();
		RuntimeManager.LowlevelSystem.getNumDrivers(out int numdrivers);
		for (int i = 0; i < numdrivers; i++)
		{
			KFMOD.AudioDevice item = default(KFMOD.AudioDevice);
			RuntimeManager.LowlevelSystem.getDriverInfo(i, out string name, 64, out item.guid, out item.systemRate, out item.speakerMode, out item.speakerModeChannels);
			item.name = name;
			item.fmod_id = i;
			audioDevices.Add(item);
			audioDeviceOptions.Add(new Dropdown.OptionData(item.name));
		}
	}

	private void OnAudioDeviceChanged(int idx)
	{
		RuntimeManager.LowlevelSystem.setDriver(idx);
		int num = 0;
		while (true)
		{
			if (num >= audioDevices.Count)
			{
				return;
			}
			KFMOD.AudioDevice audioDevice = audioDevices[num];
			if (idx == audioDevice.fmod_id)
			{
				break;
			}
			num++;
		}
		KFMOD.currentDevice = audioDevices[num];
		KPlayerPrefs.SetString("AudioDeviceGuid", KFMOD.currentDevice.guid.ToString());
	}

	private void OnClose(GameObject go)
	{
		alwaysPlayMusicMetric[AlwaysPlayMusicKey] = MusicManager.instance.alwaysPlayMusic;
		ThreadedHttps<KleiMetrics>.Instance.SendEvent(alwaysPlayMusicMetric);
		Object.Destroy(go);
	}
}
