using ProcGenGame;
using STRINGS;
using System;
using System.IO;
using UnityEngine;

public class InitializeCheck : MonoBehaviour
{
	public enum SavePathIssue
	{
		Ok,
		WriteTestFail,
		SpaceTestFail,
		WorldGenFilesFail
	}

	private static readonly string testFile = "testfile";

	private static readonly string testSave = "testsavefile";

	public Canvas rootCanvasPrefab;

	public ConfirmDialogScreen confirmDialogScreen;

	public Sprite sadDupe;

	private SavePathIssue test_issue;

	public static SavePathIssue savePathState
	{
		get;
		private set;
	}

	private void Awake()
	{
		CheckForSavePathIssue();
		if (savePathState == SavePathIssue.Ok && !ReportErrorDialog.hasCrash)
		{
			AudioMixer.Create();
			App.LoadScene("frontend");
		}
		else
		{
			Canvas cmp = base.gameObject.AddComponent<Canvas>();
			cmp.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500f);
			cmp.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 500f);
			Camera camera = base.gameObject.AddComponent<Camera>();
			camera.orthographic = true;
			camera.orthographicSize = 200f;
			camera.backgroundColor = Color.black;
			camera.clearFlags = CameraClearFlags.Color;
			camera.nearClipPlane = 0f;
			Debug.Log("Cannot initialize filesystem. [" + savePathState.ToString() + "]");
			Localization.Initialize(true);
			ShowFileErrorDialogs();
		}
	}

	private GameObject CreateUIRoot()
	{
		return Util.KInstantiate(rootCanvasPrefab, null, "CanvasRoot");
	}

	private void ShowErrorDialog(string msg)
	{
		GameObject parent = CreateUIRoot();
		ConfirmDialogScreen confirmDialogScreen = Util.KInstantiateUI<ConfirmDialogScreen>(this.confirmDialogScreen.gameObject, parent, true);
		ConfirmDialogScreen obj = confirmDialogScreen;
		System.Action on_confirm = Quit;
		System.Action on_cancel = null;
		Sprite image_sprite = sadDupe;
		obj.PopupConfirmDialog(msg, on_confirm, on_cancel, null, null, null, null, null, image_sprite, true);
	}

	private void ShowFileErrorDialogs()
	{
		string text = null;
		switch (savePathState)
		{
		case SavePathIssue.WriteTestFail:
			text = string.Format(UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_READ_ONLY, SaveLoader.GetSavePrefix());
			break;
		case SavePathIssue.SpaceTestFail:
			text = string.Format(UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_INSUFFICIENT_SPACE, SaveLoader.GetSavePrefix());
			break;
		case SavePathIssue.WorldGenFilesFail:
			text = string.Format(UI.FRONTEND.SUPPORTWARNINGS.WORLD_GEN_FILES, WorldGen.WORLDGEN_SAVE_FILENAME + "\n" + WorldGen.SIM_SAVE_FILENAME);
			break;
		}
		if (text != null)
		{
			ShowErrorDialog(text);
		}
	}

	private void CheckForSavePathIssue()
	{
		if (test_issue != 0)
		{
			savePathState = test_issue;
			return;
		}
		string savePrefix = SaveLoader.GetSavePrefix();
		savePathState = SavePathIssue.Ok;
		try
		{
			SaveLoader.GetSavePrefixAndCreateFolder();
			using (FileStream fileStream = File.Open(savePrefix + testFile, FileMode.Create, FileAccess.Write))
			{
				new BinaryWriter(fileStream);
				fileStream.Close();
			}
		}
		catch
		{
			savePathState = SavePathIssue.WriteTestFail;
			goto IL_0111;
		}
		using (FileStream fileStream2 = File.Open(savePrefix + testSave, FileMode.Create, FileAccess.Write))
		{
			try
			{
				fileStream2.SetLength(15000000L);
				new BinaryWriter(fileStream2);
				fileStream2.Close();
			}
			catch
			{
				fileStream2.Close();
				savePathState = SavePathIssue.SpaceTestFail;
				goto IL_0111;
			}
		}
		try
		{
			using (File.Open(WorldGen.WORLDGEN_SAVE_FILENAME, FileMode.Append))
			{
			}
			using (File.Open(WorldGen.SIM_SAVE_FILENAME, FileMode.Append))
			{
			}
		}
		catch
		{
			savePathState = SavePathIssue.WorldGenFilesFail;
		}
		goto IL_0111;
		IL_0111:
		try
		{
			if (File.Exists(savePrefix + testFile))
			{
				File.Delete(savePrefix + testFile);
			}
			if (File.Exists(savePrefix + testSave))
			{
				File.Delete(savePrefix + testSave);
			}
		}
		catch
		{
		}
	}

	private void Quit()
	{
		Debug.Log("Quitting...");
		App.Quit();
	}
}
