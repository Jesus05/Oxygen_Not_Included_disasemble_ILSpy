using Klei;
using STRINGS;
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class DebugHandler : IInputHandler
{
	public enum PaintMode
	{
		None,
		Element,
		Hot,
		Cold
	}

	public static bool InstantBuildMode;

	public static bool InvincibleMode;

	public static bool SelectInEditor;

	public static bool DebugPathFinding;

	public static bool HideUI;

	public static bool DebugCellInfo;

	public static bool DebugNextCall;

	private bool superTestMode = false;

	private bool ultraTestMode = false;

	private bool slowTestMode = false;

	public static bool enabled
	{
		get;
		private set;
	}

	public KInputHandler inputHandler
	{
		get;
		set;
	}

	public DebugHandler()
	{
		enabled = File.Exists(Path.Combine(Application.dataPath, "debug_enable.txt"));
		enabled = (enabled || File.Exists(Path.Combine(Application.dataPath, "../debug_enable.txt")));
		enabled = (enabled || GenericGameSettings.instance.debugEnable);
	}

	public static int GetMouseCell()
	{
		Vector3 mousePos = KInputManager.GetMousePos();
		Vector3 position = Camera.main.transform.GetPosition();
		mousePos.z = 0f - position.z - Grid.CellSizeInMeters;
		Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);
		return Grid.PosToCell(pos);
	}

	public static Vector3 GetMousePos()
	{
		Vector3 mousePos = KInputManager.GetMousePos();
		Vector3 position = Camera.main.transform.GetPosition();
		mousePos.z = 0f - position.z - Grid.CellSizeInMeters;
		return Camera.main.ScreenToWorldPoint(mousePos);
	}

	private void SpawnMinion()
	{
		if (!((UnityEngine.Object)Immigration.Instance == (UnityEngine.Object)null))
		{
			if (!Grid.IsValidBuildingCell(GetMouseCell()))
			{
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, UI.DEBUG_TOOLS.INVALID_LOCATION, null, GetMousePos(), 1.5f, false, true);
			}
			else
			{
				GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(MinionConfig.ID), null, null);
				gameObject.name = Assets.GetPrefab(MinionConfig.ID).name;
				Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
				Vector3 position = Grid.CellToPosCBC(GetMouseCell(), Grid.SceneLayer.Move);
				gameObject.transform.SetLocalPosition(position);
				gameObject.SetActive(true);
				MinionStartingStats minionStartingStats = new MinionStartingStats(false);
				minionStartingStats.Apply(gameObject);
			}
		}
	}

	public void OnKeyDown(KButtonEvent e)
	{
		if (!enabled)
		{
			return;
		}
		if (e.TryConsume(Action.DebugSpawnMinion))
		{
			SpawnMinion();
		}
		else if (e.TryConsume(Action.DebugSpawnStressTest))
		{
			for (int i = 0; i < 60; i++)
			{
				SpawnMinion();
			}
		}
		else if (e.TryConsume(Action.DebugSuperTestMode))
		{
			if (!superTestMode)
			{
				Time.timeScale = 15f;
				superTestMode = true;
			}
			else
			{
				Time.timeScale = 1f;
				superTestMode = false;
			}
		}
		else if (e.TryConsume(Action.DebugUltraTestMode))
		{
			if (!ultraTestMode)
			{
				Time.timeScale = 30f;
				ultraTestMode = true;
			}
			else
			{
				Time.timeScale = 1f;
				ultraTestMode = false;
			}
		}
		else if (e.TryConsume(Action.DebugSlowTestMode))
		{
			if (!slowTestMode)
			{
				Time.timeScale = 0.06f;
				slowTestMode = true;
			}
			else
			{
				Time.timeScale = 1f;
				slowTestMode = false;
			}
		}
		else if (e.TryConsume(Action.DebugDig))
		{
			int mouseCell = GetMouseCell();
			SimMessages.Dig(mouseCell, -1);
		}
		else if (e.TryConsume(Action.DebugInstantBuildMode))
		{
			InstantBuildMode = !InstantBuildMode;
			if ((UnityEngine.Object)PlanScreen.Instance != (UnityEngine.Object)null)
			{
				PlanScreen.Instance.Refresh();
			}
			if ((UnityEngine.Object)BuildMenu.Instance != (UnityEngine.Object)null)
			{
				BuildMenu.Instance.Refresh();
			}
			if ((UnityEngine.Object)OverlayMenu.Instance != (UnityEngine.Object)null)
			{
				OverlayMenu.Instance.Refresh();
			}
			ConsumerManager.instance.RefreshDiscovered(null);
			if ((UnityEngine.Object)ManagementMenu.Instance != (UnityEngine.Object)null)
			{
				ManagementMenu.Instance.CheckResearch(null);
				ManagementMenu.Instance.CheckRoles(null);
				ManagementMenu.Instance.CheckStarmap(null);
			}
			Game.Instance.Trigger(1594320620, "all_the_things");
		}
		else if (e.TryConsume(Action.DebugExplosion))
		{
			Vector3 mousePos = KInputManager.GetMousePos();
			Vector3 position = Camera.main.transform.GetPosition();
			mousePos.z = 0f - position.z - Grid.CellSizeInMeters;
			Vector3 explosion_pos = Camera.main.ScreenToWorldPoint(mousePos);
			GameUtil.CreateExplosion(explosion_pos);
		}
		else if (e.TryConsume(Action.DebugLockCursor))
		{
			if (GenericGameSettings.instance.developerDebugEnable)
			{
				KInputManager.isMousePosLocked = !KInputManager.isMousePosLocked;
				KInputManager.lockedMousePos = KInputManager.GetMousePos();
			}
		}
		else if (e.TryConsume(Action.DebugDiscoverAllElements))
		{
			if ((UnityEngine.Object)WorldInventory.Instance != (UnityEngine.Object)null)
			{
				foreach (Element element in ElementLoader.elements)
				{
					WorldInventory.Instance.Discover(element.tag, element.GetMaterialCategoryTag());
				}
			}
		}
		else if (e.TryConsume(Action.DebugToggleUI))
		{
			ToggleScreenshotMode();
		}
		else if (e.TryConsume(Action.SreenShot1x))
		{
			string filename = Path.ChangeExtension(SaveLoader.GetActiveSaveFilePath(), ".png");
			ScreenCapture.CaptureScreenshot(filename, 1);
		}
		else if (e.TryConsume(Action.SreenShot2x))
		{
			string filename2 = Path.ChangeExtension(SaveLoader.GetActiveSaveFilePath(), ".png");
			ScreenCapture.CaptureScreenshot(filename2, 2);
		}
		else if (e.TryConsume(Action.SreenShot8x))
		{
			string filename3 = Path.ChangeExtension(SaveLoader.GetActiveSaveFilePath(), ".png");
			ScreenCapture.CaptureScreenshot(filename3, 8);
		}
		else if (e.TryConsume(Action.SreenShot32x))
		{
			string filename4 = Path.ChangeExtension(SaveLoader.GetActiveSaveFilePath(), ".png");
			ScreenCapture.CaptureScreenshot(filename4, 32);
		}
		else if (e.TryConsume(Action.DebugCellInfo))
		{
			DebugCellInfo = !DebugCellInfo;
		}
		else if (e.TryConsume(Action.DebugToggle))
		{
			PropertyTextures.FogOfWarScale = 1f - PropertyTextures.FogOfWarScale;
			if ((UnityEngine.Object)CameraController.Instance != (UnityEngine.Object)null)
			{
				CameraController.Instance.FreeCameraEnabled = !CameraController.Instance.FreeCameraEnabled;
			}
			if ((UnityEngine.Object)Game.Instance != (UnityEngine.Object)null)
			{
				Game.Instance.UpdateGameActiveRegion(0, 0, Grid.WidthInCells, Grid.HeightInCells);
				SaveGame.Instance.worldGenSpawner.SpawnEverything();
			}
			if ((UnityEngine.Object)DebugPaintElementScreen.Instance != (UnityEngine.Object)null)
			{
				bool activeSelf = DebugPaintElementScreen.Instance.gameObject.activeSelf;
				DebugPaintElementScreen.Instance.gameObject.SetActive(!activeSelf);
				if ((bool)DebugElementMenu.Instance && DebugElementMenu.Instance.root.activeSelf)
				{
					DebugElementMenu.Instance.root.SetActive(false);
				}
				DebugBaseTemplateButton.Instance.gameObject.SetActive(!activeSelf);
			}
		}
		else if (e.TryConsume(Action.DebugCollectGarbage))
		{
			GC.Collect();
		}
		else if (e.TryConsume(Action.DebugInvincible))
		{
			InvincibleMode = !InvincibleMode;
		}
		else if (e.TryConsume(Action.DebugVisualTest))
		{
			Scenario.Instance.SetupVisualTest();
		}
		else if (e.TryConsume(Action.DebugGameplayTest))
		{
			Scenario.Instance.SetupGameplayTest();
		}
		else if (e.TryConsume(Action.DebugElementTest))
		{
			Scenario.Instance.SetupElementTest();
		}
		else if (e.TryConsume(Action.ToggleProfiler))
		{
			Sim.SIM_HandleMessage(-409964931, 0, null);
		}
		else if (e.TryConsume(Action.DebugRefreshNavCell))
		{
			Pathfinding.Instance.RefreshNavCell(GetMouseCell());
		}
		else if (e.TryConsume(Action.DebugToggleSelectInEditor))
		{
			SetSelectInEditor(!SelectInEditor);
		}
		else if (e.TryConsume(Action.DebugGotoTarget))
		{
			Debug.Log("Debug GoTo", null);
			Game.Instance.Trigger(775300118, null);
			foreach (Brain item in Components.Brains.Items)
			{
				item.GetSMI<DebugGoToMonitor.Instance>()?.GoToCursor();
				item.GetSMI<CreatureDebugGoToMonitor.Instance>()?.GoToCursor();
			}
		}
		else if (e.TryConsume(Action.DebugTeleport))
		{
			if ((UnityEngine.Object)SelectTool.Instance == (UnityEngine.Object)null)
			{
				return;
			}
			KSelectable selected = SelectTool.Instance.selected;
			if ((UnityEngine.Object)selected != (UnityEngine.Object)null)
			{
				int mouseCell2 = GetMouseCell();
				if (!Grid.IsValidBuildingCell(mouseCell2))
				{
					PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, UI.DEBUG_TOOLS.INVALID_LOCATION, null, GetMousePos(), 1.5f, false, true);
					return;
				}
				selected.transform.SetPosition(Grid.CellToPosCBC(mouseCell2, Grid.SceneLayer.Move));
			}
		}
		else if (!e.TryConsume(Action.DebugPlace) && !e.TryConsume(Action.DebugSelectMaterial))
		{
			if (e.TryConsume(Action.DebugNotification))
			{
				if (GenericGameSettings.instance.developerDebugEnable)
				{
					Tutorial.Instance.DebugNotification();
				}
			}
			else if (e.TryConsume(Action.DebugNotificationMessage))
			{
				if (GenericGameSettings.instance.developerDebugEnable)
				{
					Tutorial.Instance.DebugNotificationMessage();
				}
			}
			else if (e.TryConsume(Action.DebugSuperSpeed))
			{
				if ((UnityEngine.Object)SpeedControlScreen.Instance != (UnityEngine.Object)null)
				{
					SpeedControlScreen.Instance.ToggleRidiculousSpeed();
				}
			}
			else if (e.TryConsume(Action.DebugGameStep))
			{
				if ((UnityEngine.Object)SpeedControlScreen.Instance != (UnityEngine.Object)null)
				{
					SpeedControlScreen.Instance.DebugStepFrame();
				}
			}
			else if (e.TryConsume(Action.DebugSimStep))
			{
				Game.Instance.ForceSimStep();
			}
			else if (e.TryConsume(Action.DebugToggleMusic))
			{
				AudioDebug.Get().ToggleMusic();
			}
			else if (e.TryConsume(Action.DebugRiverTest))
			{
				Scenario.Instance.SetupRiverTest();
			}
			else if (e.TryConsume(Action.DebugTileTest))
			{
				Scenario.Instance.SetupTileTest();
			}
			else if (e.TryConsume(Action.DebugForceLightEverywhere))
			{
				PropertyTextures.instance.ForceLightEverywhere = !PropertyTextures.instance.ForceLightEverywhere;
			}
			else if (e.TryConsume(Action.DebugPathFinding))
			{
				DebugPathFinding = !DebugPathFinding;
				Debug.Log("DebugPathFinding=" + DebugPathFinding, null);
			}
			else if (!e.TryConsume(Action.DebugFocus))
			{
				if (e.TryConsume(Action.DebugReportBug))
				{
					if (GenericGameSettings.instance.developerDebugEnable)
					{
						int num = 0;
						string validSaveFilename;
						while (true)
						{
							validSaveFilename = SaveScreen.GetValidSaveFilename("bug_report_savefile_" + num.ToString());
							if (!File.Exists(validSaveFilename))
							{
								break;
							}
							num++;
						}
						string save_file = "No save file (front end)";
						if ((UnityEngine.Object)SaveLoader.Instance != (UnityEngine.Object)null)
						{
							save_file = SaveLoader.Instance.Save(validSaveFilename, false, false);
						}
						KCrashReporter.ReportBug("Bug Report", save_file);
					}
					else
					{
						Debug.Log("Debug crash keys are not enabled.", null);
					}
				}
				else if (e.TryConsume(Action.DebugTriggerException))
				{
					if (GenericGameSettings.instance.developerDebugEnable)
					{
						string str = Guid.NewGuid().ToString();
						StackTrace stackTrace = new StackTrace(1, true);
						str = str + "\n" + stackTrace.ToString();
						KCrashReporter.ReportError("Debug crash with random stack", str, null, ScreenPrefabs.Instance.ConfirmDialogScreen, "");
					}
				}
				else if (e.TryConsume(Action.DebugTriggerError))
				{
					if (GenericGameSettings.instance.developerDebugEnable)
					{
						Debug.LogError("Oooops! Testing error!", null);
					}
				}
				else if (e.TryConsume(Action.DebugDumpGCRoots))
				{
					GarbageProfiler.DebugDumpRootItems();
				}
				else if (e.TryConsume(Action.DebugDumpGarbageReferences))
				{
					GarbageProfiler.DebugDumpGarbageStats();
				}
				else if (e.TryConsume(Action.DebugDumpEventData))
				{
					if (GenericGameSettings.instance.developerDebugEnable)
					{
						KObjectManager.Instance.DumpEventData();
					}
				}
				else if (e.TryConsume(Action.DebugDumpSceneParitionerLeakData))
				{
					if (!GenericGameSettings.instance.developerDebugEnable)
					{
						goto IL_0bfc;
					}
				}
				else if (e.TryConsume(Action.DebugCrashSim))
				{
					if (GenericGameSettings.instance.developerDebugEnable)
					{
						Sim.SIM_DebugCrash();
					}
				}
				else if (e.TryConsume(Action.DebugNextCall))
				{
					DebugNextCall = true;
				}
				else if (e.TryConsume(Action.DebugTogglePersonalPriorityComparison))
				{
					Chore.ENABLE_PERSONAL_PRIORITIES = !Chore.ENABLE_PERSONAL_PRIORITIES;
				}
			}
		}
		goto IL_0bfc;
		IL_0bfc:
		if (e.Consumed && (UnityEngine.Object)Game.Instance != (UnityEngine.Object)null)
		{
			Game.Instance.debugWasUsed = true;
			KCrashReporter.debugWasUsed = true;
		}
	}

	public static void SetSelectInEditor(bool select_in_editor)
	{
	}

	public static void ToggleScreenshotMode()
	{
		SetHideUI(!HideUI);
		if ((UnityEngine.Object)CameraController.Instance != (UnityEngine.Object)null)
		{
			CameraController.Instance.FreeCameraEnabled = !CameraController.Instance.FreeCameraEnabled;
		}
	}

	public static void SetHideUI(bool hide)
	{
		HideUI = hide;
		Canvas[] array = Resources.FindObjectsOfTypeAll<Canvas>();
		foreach (Canvas canvas in array)
		{
			CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
			if ((UnityEngine.Object)canvasGroup == (UnityEngine.Object)null)
			{
				canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
			}
			if (HideUI)
			{
				canvasGroup.alpha = 0f;
			}
			else
			{
				canvasGroup.alpha = 1f;
			}
		}
	}
}
