using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreen : KModalScreen
{
	private struct SaveGameFileDetails
	{
		public string BaseName;

		public string FileName;

		public System.DateTime FileDate;

		public SaveGame.Header FileHeader;

		public SaveGame.GameInfo FileInfo;
	}

	private InspectSaveScreen inspectScreenInstance;

	[SerializeField]
	private HierarchyReferences saveButtonPrefab;

	[SerializeField]
	private GameObject saveButtonRoot;

	[SerializeField]
	private LocText saveDetails;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton loadButton;

	[SerializeField]
	private KButton deleteButton;

	[SerializeField]
	private GameObject previewImageRoot;

	[SerializeField]
	private ColorStyleSetting validSaveFileStyle;

	[SerializeField]
	private ColorStyleSetting invalidSaveFileStyle;

	public LocText FileName;

	public LocText CyclesSurvivedValue;

	public LocText DuplicantsAliveValue;

	public LocText InfoText;

	public Action<string, string> onClick;

	public bool requireConfirmation = true;

	private UIPool<HierarchyReferences> savenameRowPool;

	private Dictionary<string, KButton> fileButtonMap = new Dictionary<string, KButton>();

	private ConfirmDialogScreen confirmScreen;

	private string selectedFileName;

	private KButton currentExpanded;

	private Dictionary<string, List<SaveGameFileDetails>> saveFiles;

	public static LoadScreen Instance
	{
		get;
		private set;
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Debug.Assert((UnityEngine.Object)Instance == (UnityEngine.Object)null);
		Instance = this;
		base.OnPrefabInit();
		savenameRowPool = new UIPool<HierarchyReferences>(saveButtonPrefab);
		if ((UnityEngine.Object)SpeedControlScreen.Instance != (UnityEngine.Object)null)
		{
			SpeedControlScreen.Instance.Pause(false);
		}
		if (onClick == null)
		{
			onClick = SetSelectedGame;
		}
		if ((UnityEngine.Object)closeButton != (UnityEngine.Object)null)
		{
			closeButton.onClick += delegate
			{
				Deactivate();
			};
		}
		if ((UnityEngine.Object)loadButton != (UnityEngine.Object)null)
		{
			loadButton.onClick += Load;
		}
		if ((UnityEngine.Object)deleteButton != (UnityEngine.Object)null)
		{
			deleteButton.onClick += Delete;
			deleteButton.isInteractable = false;
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		RefreshFiles();
	}

	private void GetFilesList()
	{
		saveFiles = new Dictionary<string, List<SaveGameFileDetails>>();
		List<string> allFiles = SaveLoader.GetAllFiles();
		if (allFiles.Count > 0)
		{
			for (int i = 0; i < allFiles.Count; i++)
			{
				if (IsFileValid(allFiles[i]))
				{
					Tuple<SaveGame.Header, SaveGame.GameInfo> fileInfo = GetFileInfo(allFiles[i]);
					SaveGame.Header first = fileInfo.first;
					SaveGame.GameInfo second = fileInfo.second;
					System.DateTime lastWriteTime = File.GetLastWriteTime(allFiles[i]);
					string path = (!(second.originalSaveName != string.Empty)) ? allFiles[i] : second.originalSaveName;
					path = Path.GetFileNameWithoutExtension(path);
					SaveGameFileDetails item = default(SaveGameFileDetails);
					item.BaseName = second.baseName;
					item.FileName = allFiles[i];
					item.FileDate = lastWriteTime;
					item.FileHeader = first;
					item.FileInfo = second;
					if (!saveFiles.ContainsKey(path))
					{
						saveFiles.Add(path, new List<SaveGameFileDetails>());
					}
					saveFiles[path].Add(item);
				}
			}
		}
	}

	private bool IsFileValid(string filename)
	{
		bool result = false;
		try
		{
			SaveGame.Header header;
			SaveGame.GameInfo gameInfo = SaveLoader.LoadHeader(filename, out header);
			result = (gameInfo.saveMajorVersion >= 7);
			return result;
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Corrupted save file: " + filename + "\n" + ex.ToString());
			return result;
		}
	}

	private Tuple<SaveGame.Header, SaveGame.GameInfo> GetFileInfo(string filename)
	{
		try
		{
			SaveGame.Header header;
			SaveGame.GameInfo b = SaveLoader.LoadHeader(filename, out header);
			if (b.saveMajorVersion >= 7)
			{
				return new Tuple<SaveGame.Header, SaveGame.GameInfo>(header, b);
			}
		}
		catch (Exception obj)
		{
			Debug.LogWarning(obj);
			InfoText.text = string.Format(UI.FRONTEND.LOADSCREEN.CORRUPTEDSAVE, filename);
		}
		return null;
	}

	private void RefreshFiles()
	{
		if (savenameRowPool != null)
		{
			savenameRowPool.ClearAll();
		}
		if (fileButtonMap != null)
		{
			fileButtonMap.Clear();
		}
		GetFilesList();
		if (saveFiles.Count > 0)
		{
			foreach (KeyValuePair<string, List<SaveGameFileDetails>> saveFile in saveFiles)
			{
				AddExistingSaveFile(saveFile.Key, saveFile.Value);
			}
		}
		InfoText.text = string.Empty;
		CyclesSurvivedValue.text = "-";
		DuplicantsAliveValue.text = "-";
		deleteButton.isInteractable = false;
		loadButton.isInteractable = false;
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		RefreshFiles();
	}

	protected override void OnDeactivate()
	{
		if ((UnityEngine.Object)SpeedControlScreen.Instance != (UnityEngine.Object)null)
		{
			SpeedControlScreen.Instance.Unpause(false);
		}
		selectedFileName = null;
		base.OnDeactivate();
	}

	private void SetHeaderButtonActive(KButton headerButton, bool activeState)
	{
		ImageToggleState component = headerButton.GetComponent<ImageToggleState>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.SetActiveState(activeState);
		}
	}

	private void SetChildrenActive(HierarchyReferences hierarchy, bool state)
	{
		for (int i = 0; i < hierarchy.transform.childCount; i++)
		{
			GameObject gameObject = hierarchy.transform.GetChild(i).gameObject;
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
			{
				gameObject.SetActive(state);
			}
		}
	}

	private void AddExistingSaveFile(string savename, List<SaveGameFileDetails> fileDetailsList)
	{
		HierarchyReferences savenameRow = savenameRowPool.GetFreeElement(saveButtonRoot, true);
		KButton headerButton = savenameRow.GetReference<RectTransform>("Button").GetComponent<KButton>();
		headerButton.ClearOnClick();
		LocText headerTitle = savenameRow.GetReference<RectTransform>("HeaderTitle").GetComponent<LocText>();
		LocText component = savenameRow.GetReference<RectTransform>("SaveTitle").GetComponent<LocText>();
		LocText component2 = savenameRow.GetReference<RectTransform>("HeaderDate").GetComponent<LocText>();
		RectTransform saveDetailsRow = savenameRow.GetReference<RectTransform>("SaveDetailsRow");
		LocText component3 = savenameRow.GetReference<RectTransform>("SaveDetailsBaseName").GetComponent<LocText>();
		RectTransform savefileRowTemplate = savenameRow.GetReference<RectTransform>("SavefileRowTemplate");
		fileDetailsList.Sort((SaveGameFileDetails x, SaveGameFileDetails y) => y.FileDate.CompareTo(x.FileDate));
		LocText locText = headerTitle;
		SaveGameFileDetails saveGameFileDetails = fileDetailsList[0];
		locText.text = saveGameFileDetails.BaseName;
		component.text = savename;
		LocText locText2 = component2;
		string format = "{0:H:mm:ss} - " + Localization.GetFileDateFormat(0);
		SaveGameFileDetails saveGameFileDetails2 = fileDetailsList[0];
		locText2.text = string.Format(format, saveGameFileDetails2.FileDate);
		LocText locText3 = component3;
		LocString bASE_NAME = UI.FRONTEND.LOADSCREEN.BASE_NAME;
		SaveGameFileDetails saveGameFileDetails3 = fileDetailsList[0];
		locText3.text = $"{bASE_NAME}: {saveGameFileDetails3.BaseName}";
		for (int i = 0; i < savenameRow.transform.childCount; i++)
		{
			GameObject gameObject = savenameRow.transform.GetChild(i).gameObject;
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null && gameObject.name.Contains("Clone"))
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
		bool active = true;
		foreach (SaveGameFileDetails fileDetails2 in fileDetailsList)
		{
			SaveGameFileDetails fileDetails = fileDetails2;
			RectTransform rectTransform = UnityEngine.Object.Instantiate(savefileRowTemplate, savenameRow.transform);
			HierarchyReferences component4 = rectTransform.GetComponent<HierarchyReferences>();
			KButton component5 = rectTransform.GetComponent<KButton>();
			RectTransform reference = component4.GetReference<RectTransform>("NewestLabel");
			RectTransform reference2 = component4.GetReference<RectTransform>("AutoLabel");
			LocText component6 = component4.GetReference<RectTransform>("SaveText").GetComponent<LocText>();
			LocText component7 = component4.GetReference<RectTransform>("DateText").GetComponent<LocText>();
			reference.gameObject.SetActive(active);
			active = false;
			reference2.gameObject.SetActive(fileDetails.FileInfo.isAutoSave);
			component6.text = Path.GetFileNameWithoutExtension(fileDetails.FileName);
			component7.text = string.Format("{0:H:mm:ss} - " + Localization.GetFileDateFormat(0), fileDetails.FileDate);
			component5.onClick += delegate
			{
				onClick(fileDetails.FileName, savename);
			};
			component5.onDoubleClick += delegate
			{
				onClick(fileDetails.FileName, savename);
				Load();
			};
			fileButtonMap.Add(fileDetails.FileName, component5);
		}
		headerButton.onClick += delegate
		{
			bool activeSelf = saveDetailsRow.gameObject.activeSelf;
			bool flag = (UnityEngine.Object)headerButton == (UnityEngine.Object)currentExpanded;
			if (flag)
			{
				SetChildrenActive(savenameRow, !activeSelf);
			}
			else if (!activeSelf && !flag)
			{
				SetChildrenActive(savenameRow, true);
			}
			if ((UnityEngine.Object)currentExpanded != (UnityEngine.Object)null && !flag)
			{
				SetHeaderButtonActive(currentExpanded, false);
			}
			currentExpanded = headerButton;
			SetHeaderButtonActive(currentExpanded, true);
			headerTitle.transform.parent.gameObject.SetActive(true);
			savefileRowTemplate.gameObject.SetActive(false);
			Action<string, string> action2 = onClick;
			SaveGameFileDetails saveGameFileDetails5 = fileDetailsList[0];
			action2(saveGameFileDetails5.FileName, savename);
		};
		headerButton.onDoubleClick += delegate
		{
			Action<string, string> action = onClick;
			SaveGameFileDetails saveGameFileDetails4 = fileDetailsList[0];
			action(saveGameFileDetails4.FileName, savename);
			LoadingOverlay.Load(DoLoad);
		};
		savenameRow.transform.SetAsLastSibling();
	}

	public static void ForceStopGame()
	{
		ThreadedHttps<KleiMetrics>.Instance.SendProfileStats();
		Game.Instance.SetIsLoading();
		Grid.CellCount = 0;
		Sim.Shutdown();
	}

	private static bool IsSaveFileFromUnsupportedFutureBuild(SaveGame.Header header)
	{
		return header.buildVersion > 365655;
	}

	private void SetSelectedGame(string filename, string savename)
	{
		if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
		{
			Debug.LogError("The filename provided is not valid.");
			deleteButton.isInteractable = false;
		}
		else
		{
			deleteButton.isInteractable = true;
			KButton kButton = (selectedFileName == null) ? null : fileButtonMap[selectedFileName];
			if ((UnityEngine.Object)kButton != (UnityEngine.Object)null)
			{
				kButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
			}
			selectedFileName = filename;
			FileName.text = Path.GetFileName(selectedFileName);
			kButton = fileButtonMap[selectedFileName];
			kButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Active);
			try
			{
				SaveGame.Header header;
				SaveGame.GameInfo gameInfo = SaveLoader.LoadHeader(filename, out header);
				string fileName = Path.GetFileName(filename);
				if (gameInfo.isAutoSave)
				{
					fileName = fileName + "\n" + UI.FRONTEND.LOADSCREEN.AUTOSAVEWARNING;
				}
				CyclesSurvivedValue.text = gameInfo.numberOfCycles.ToString();
				DuplicantsAliveValue.text = gameInfo.numberOfDuplicants.ToString();
				InfoText.text = string.Empty;
				if (IsSaveFileFromUnsupportedFutureBuild(header))
				{
					InfoText.text = string.Format(UI.FRONTEND.LOADSCREEN.SAVE_TOO_NEW, filename, header.buildVersion, 365655u);
					loadButton.isInteractable = false;
					loadButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Disabled);
				}
				else if (gameInfo.saveMajorVersion < 7)
				{
					InfoText.text = string.Format(UI.FRONTEND.LOADSCREEN.UNSUPPORTED_SAVE_VERSION, filename, gameInfo.saveMajorVersion, gameInfo.saveMinorVersion, 7, 11);
					loadButton.isInteractable = false;
					loadButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Disabled);
				}
				else if (!loadButton.isInteractable)
				{
					loadButton.isInteractable = true;
					loadButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
				}
				if (InfoText.text == string.Empty && gameInfo.isAutoSave)
				{
					InfoText.text = UI.FRONTEND.LOADSCREEN.AUTOSAVEWARNING;
				}
			}
			catch (Exception obj)
			{
				Debug.LogWarning(obj);
				InfoText.text = string.Format(UI.FRONTEND.LOADSCREEN.CORRUPTEDSAVE, filename);
				if (loadButton.isInteractable)
				{
					loadButton.isInteractable = false;
					loadButton.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Disabled);
				}
				deleteButton.isInteractable = false;
			}
			try
			{
				Sprite sprite = RetireColonyUtility.LoadColonyPreview(selectedFileName, savename);
				Image component = previewImageRoot.GetComponent<Image>();
				component.sprite = sprite;
				component.color = ((!(bool)sprite) ? Color.black : Color.white);
			}
			catch (Exception obj2)
			{
				Debug.Log(obj2);
			}
		}
	}

	private void Load()
	{
		LoadingOverlay.Load(DoLoad);
	}

	private void DoLoad()
	{
		DoLoad(selectedFileName);
		Deactivate();
	}

	private static void DoLoad(string filename)
	{
		ReportErrorDialog.MOST_RECENT_SAVEFILE = filename;
		bool flag = true;
		SaveGame.Header header;
		SaveGame.GameInfo gameInfo = SaveLoader.LoadHeader(filename, out header);
		string arg = null;
		string arg2 = null;
		if (header.buildVersion > 365655)
		{
			arg = header.buildVersion.ToString();
			arg2 = 365655.ToString();
		}
		else if (gameInfo.saveMajorVersion < 7)
		{
			arg = $"v{gameInfo.saveMajorVersion}.{gameInfo.saveMinorVersion}";
			arg2 = $"v{7}.{11}";
		}
		if (!flag)
		{
			GameObject parent = (!((UnityEngine.Object)FrontEndManager.Instance == (UnityEngine.Object)null)) ? FrontEndManager.Instance.gameObject : GameScreenManager.Instance.ssOverlayCanvas;
			ConfirmDialogScreen component = Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, parent, true).GetComponent<ConfirmDialogScreen>();
			component.PopupConfirmDialog(string.Format(UI.CRASHSCREEN.LOADFAILED, "Version Mismatch", arg, arg2), null, null, null, null, null, null, null, null, true);
		}
		else
		{
			if ((UnityEngine.Object)Game.Instance != (UnityEngine.Object)null)
			{
				ForceStopGame();
			}
			SaveLoader.SetActiveSaveFilePath(filename);
			Time.timeScale = 0f;
			App.LoadScene("backend");
		}
	}

	private void MoreInfo()
	{
		Application.OpenURL("http://support.kleientertainment.com/customer/portal/articles/2776550");
	}

	private void Delete()
	{
		if (string.IsNullOrEmpty(selectedFileName))
		{
			Debug.LogError("The path provided is not valid and cannot be deleted.");
		}
		else
		{
			ConfirmDoAction(string.Format(UI.FRONTEND.LOADSCREEN.CONFIRMDELETE, Path.GetFileName(selectedFileName)), delegate
			{
				fileButtonMap[selectedFileName].GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
				fileButtonMap[selectedFileName].isInteractable = true;
				File.Delete(selectedFileName);
				selectedFileName = null;
				RefreshFiles();
			});
		}
	}

	private void ConfirmDoAction(string message, System.Action action)
	{
		if ((UnityEngine.Object)confirmScreen == (UnityEngine.Object)null)
		{
			confirmScreen = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, false);
			confirmScreen.PopupConfirmDialog(message, action, delegate
			{
			}, null, null, null, null, null, null, true);
			confirmScreen.gameObject.SetActive(true);
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape))
		{
			Deactivate();
		}
		base.OnKeyUp(e);
	}
}
