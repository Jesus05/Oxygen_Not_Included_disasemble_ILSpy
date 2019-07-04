using Database;
using FMOD.Studio;
using KSerialization;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ColonyAchievementTracker : KMonoBehaviour, ISaveLoadableDetails
{
	public Dictionary<string, ColonyAchievementStatus> achievements = new Dictionary<string, ColonyAchievementStatus>();

	private SchedulerHandle checkAchievementsHandle;

	private int forceCheckAchievementHandle = -1;

	[Serialize]
	private List<string> completedAchievementsToDisplay = new List<string>();

	private List<string> newlyCompletedAchievements = new List<string>();

	private SchedulerHandle victorySchedulerHandle;

	public List<string> achievementsToDisplay => completedAchievementsToDisplay;

	public void ClearDisplayAchievements()
	{
		achievementsToDisplay.Clear();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		foreach (ColonyAchievement resource in Db.Get().ColonyAchievements.resources)
		{
			if (!achievements.ContainsKey(resource.Id))
			{
				ColonyAchievementStatus colonyAchievementStatus = new ColonyAchievementStatus();
				colonyAchievementStatus.SetRequirements(resource.requirementChecklist);
				achievements.Add(resource.Id, colonyAchievementStatus);
			}
		}
		forceCheckAchievementHandle = Game.Instance.Subscribe(395452326, CheckAchievements);
		GameScheduler.Instance.Schedule("CheckColonyAchievements", 5f, CheckAchievements, null, null);
	}

	private void CheckAchievements(object data = null)
	{
		foreach (KeyValuePair<string, ColonyAchievementStatus> achievement in achievements)
		{
			if (!achievement.Value.success && !achievement.Value.failed)
			{
				achievement.Value.UpdateAchievement();
				if (achievement.Value.success && !achievement.Value.failed)
				{
					newlyCompletedAchievements.Add(achievement.Key);
				}
			}
		}
		if (newlyCompletedAchievements.Count > 0)
		{
			foreach (string newlyCompletedAchievement in newlyCompletedAchievements)
			{
				completedAchievementsToDisplay.Add(newlyCompletedAchievement);
			}
			TriggerNewAchievementCompleted(null);
			RetireColonyUtility.SaveColonySummaryData();
		}
		newlyCompletedAchievements.Clear();
		checkAchievementsHandle = GameScheduler.Instance.Schedule("CheckColonyAchievements", 12f, CheckAchievements, null, null);
	}

	public void DebugTriggerAchievement(string id)
	{
		newlyCompletedAchievements.Add(id);
		achievements[id].failed = false;
		achievements[id].success = true;
	}

	private void BeginVictorySequence(GameObject cameraTaget, string achievementID)
	{
		RootMenu.Instance.canTogglePauseScreen = false;
		CameraController.Instance.DisableUserCameraControl = true;
		if (!SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Pause(false);
		}
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().VictoryMessageSnapshot);
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().MuteDynamicMusicSnapshot);
		ToggleVictoryUI(true);
		StoryMessageScreen component = GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.StoryMessageScreen.gameObject, null, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay).GetComponent<StoryMessageScreen>();
		component.restoreInterfaceOnClose = false;
		component.title = COLONY_ACHIEVEMENTS.PRE_VICTORY_MESSAGE_HEADER;
		component.body = string.Format(COLONY_ACHIEVEMENTS.PRE_VICTORY_MESSAGE_BODY, "<b>" + Db.Get().ColonyAchievements.Get(achievementID).Name + "</b>\n" + Db.Get().ColonyAchievements.Get(achievementID).description);
		component.Show(true);
		CameraController.Instance.SetWorldInteractive(false);
		StoryMessageScreen storyMessageScreen = component;
		storyMessageScreen.OnClose = (System.Action)Delegate.Combine(storyMessageScreen.OnClose, (System.Action)delegate
		{
			SpeedControlScreen.Instance.SetSpeed(1);
			if (!SpeedControlScreen.Instance.IsPaused)
			{
				SpeedControlScreen.Instance.Pause(false);
			}
			CameraController.Instance.SetWorldInteractive(true);
			StartCoroutine(VictoryNIS(cameraTaget, achievementID));
		});
	}

	public IEnumerator VictoryNIS(GameObject cameraTaget, string achievementID)
	{
		if (!SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Pause(false);
		}
		CameraController.Instance.SetWorldInteractive(false);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().VictoryMessageSnapshot, STOP_MODE.ALLOWFADEOUT);
		CameraController.Instance.FadeOut(1f);
		yield return (object)new WaitForSecondsRealtime(3f);
		/*Error: Unable to find new state assignment for yield return*/;
	}

	protected override void OnCleanUp()
	{
		victorySchedulerHandle.ClearScheduler();
		Game.Instance.Unsubscribe(forceCheckAchievementHandle);
		checkAchievementsHandle.ClearScheduler();
		base.OnCleanUp();
	}

	private void TriggerNewAchievementCompleted(GameObject cameraTarget = null)
	{
		bool flag = false;
		for (int i = 0; i < newlyCompletedAchievements.Count; i++)
		{
			if (Db.Get().ColonyAchievements.Get(newlyCompletedAchievements[i]).isVictoryCondition)
			{
				flag = true;
				GameObject cameraTaget = Db.Get().ColonyAchievements.Get(newlyCompletedAchievements[i]).GetSuccessTargetEntity();
				BeginVictorySequence(cameraTaget, newlyCompletedAchievements[i]);
				break;
			}
		}
		if (!flag)
		{
			AchievementEarnedMessage message = new AchievementEarnedMessage();
			Messenger.Instance.QueueMessage(message);
		}
	}

	private void ToggleVictoryUI(bool victoryUIActive)
	{
		List<KScreen> list = new List<KScreen>();
		list.Add(NotificationScreen.Instance);
		list.Add(OverlayMenu.Instance);
		if ((UnityEngine.Object)PlanScreen.Instance != (UnityEngine.Object)null)
		{
			list.Add(PlanScreen.Instance);
		}
		if ((UnityEngine.Object)BuildMenu.Instance != (UnityEngine.Object)null)
		{
			list.Add(BuildMenu.Instance);
		}
		list.Add(ManagementMenu.Instance);
		list.Add(ToolMenu.Instance);
		list.Add(ToolMenu.Instance.PriorityScreen);
		list.Add(ResourceCategoryScreen.Instance);
		list.Add(TopLeftControlScreen.Instance);
		list.Add(DateTime.Instance);
		list.Add(BuildWatermark.Instance);
		list.Add(HoverTextScreen.Instance);
		list.Add(DetailsScreen.Instance);
		list.Add(DebugPaintElementScreen.Instance);
		list.Add(DebugBaseTemplateButton.Instance);
		list.Add(StarmapScreen.Instance);
		foreach (KScreen item in list)
		{
			if ((UnityEngine.Object)item != (UnityEngine.Object)null)
			{
				item.Show(!victoryUIActive);
			}
		}
	}

	public void Serialize(BinaryWriter writer)
	{
		writer.Write(achievements.Count);
		foreach (KeyValuePair<string, ColonyAchievementStatus> achievement in achievements)
		{
			writer.WriteKleiString(achievement.Key);
			achievement.Value.Serialize(writer);
		}
	}

	public void Deserialize(IReader reader)
	{
		if (!SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 10))
		{
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string text = reader.ReadKleiString();
				ColonyAchievementStatus colonyAchievementStatus = new ColonyAchievementStatus();
				colonyAchievementStatus.Deserialize(reader);
				if (Db.Get().ColonyAchievements.Exists(text))
				{
					achievements.Add(text, colonyAchievementStatus);
				}
			}
		}
	}
}
