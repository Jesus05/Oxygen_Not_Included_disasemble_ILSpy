using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioMixerSnapshots : ScriptableObject
{
	[EventRef]
	public string TechFilterOnMigrated;

	[EventRef]
	public string TechFilterLogicOn;

	[EventRef]
	public string NightStartedMigrated;

	[EventRef]
	public string MenuOpenMigrated;

	[EventRef]
	public string MenuOpenHalfEffect;

	[EventRef]
	public string SpeedPausedMigrated;

	[EventRef]
	public string DuplicantCountAttenuatorMigrated;

	[EventRef]
	public string NewBaseSetupSnapshot;

	[EventRef]
	public string FrontEndSnapshot;

	[EventRef]
	public string FrontEndWelcomeScreenSnapshot;

	[EventRef]
	public string FrontEndWorldGenerationSnapshot;

	[EventRef]
	public string IntroNIS;

	[EventRef]
	public string PulseSnapshot;

	[EventRef]
	public string ESCPauseSnapshot;

	[EventRef]
	public string MENUNewDuplicantSnapshot;

	[EventRef]
	public string UserVolumeSettingsSnapshot;

	[EventRef]
	public string DuplicantCountMovingSnapshot;

	[EventRef]
	public string DuplicantCountSleepingSnapshot;

	[EventRef]
	public string PortalLPDimmedSnapshot;

	[EventRef]
	public string DynamicMusicPlayingSnapshot;

	[EventRef]
	public string FabricatorSideScreenOpenSnapshot;

	[EventRef]
	public string SpaceVisibleSnapshot;

	[EventRef]
	public string MENUStarmapSnapshot;

	[EventRef]
	public string GameNotFocusedSnapshot;

	[EventRef]
	public string FacilityVisibleSnapshot;

	[EventRef]
	public string TutorialVideoPlayingSnapshot;

	[EventRef]
	public string VictoryMessageSnapshot;

	[EventRef]
	public string VictoryNISGenericSnapshot;

	[EventRef]
	public string VictoryNISRocketSnapshot;

	[EventRef]
	public string VictoryCinematicSnapshot;

	[EventRef]
	public string VictoryFadeToBlackSnapshot;

	[EventRef]
	public string MuteDynamicMusicSnapshot;

	[SerializeField]
	[EventRef]
	private string[] snapshots;

	[NonSerialized]
	public List<string> snapshotMap = new List<string>();

	private static AudioMixerSnapshots instance;

	public static AudioMixerSnapshots Get()
	{
		if ((UnityEngine.Object)instance == (UnityEngine.Object)null)
		{
			instance = Resources.Load<AudioMixerSnapshots>("AudioMixerSnapshots");
		}
		return instance;
	}

	[ContextMenu("Reload")]
	public void ReloadSnapshots()
	{
		snapshotMap.Clear();
		string[] array = snapshots;
		foreach (string item in array)
		{
			snapshotMap.Add(item);
		}
	}
}
