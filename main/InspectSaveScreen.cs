using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class InspectSaveScreen : KModalScreen
{
	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton mainSaveBtn;

	[SerializeField]
	private KButton backupBtnPrefab;

	[SerializeField]
	private KButton deleteSaveBtn;

	[SerializeField]
	private GameObject buttonGroup;

	private UIPool<KButton> buttonPool;

	private Dictionary<KButton, string> buttonFileMap = new Dictionary<KButton, string>();

	private ConfirmDialogScreen confirmScreen;

	private string currentPath = string.Empty;

	[CompilerGenerated]
	private static Func<string, System.DateTime> _003C_003Ef__mg_0024cache0;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		closeButton.onClick += CloseScreen;
		deleteSaveBtn.onClick += DeleteSave;
	}

	private void CloseScreen()
	{
		LoadScreen.Instance.Show(true);
		Show(false);
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (!show)
		{
			buttonPool.ClearAll();
			buttonFileMap.Clear();
		}
	}

	public void SetTarget(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			Debug.LogError("The directory path provided is empty.", null);
			Show(false);
		}
		else if (!Directory.Exists(path))
		{
			Debug.LogError("The directory provided does not exist.", null);
			Show(false);
		}
		else
		{
			if (buttonPool == null)
			{
				buttonPool = new UIPool<KButton>(backupBtnPrefab);
			}
			currentPath = path;
			List<string> list = (from filename in Directory.GetFiles(path)
			where Path.GetExtension(filename).ToLower() == ".sav"
			select filename).OrderByDescending(File.GetLastWriteTime).ToList();
			string text = list[0];
			if (File.Exists(text))
			{
				mainSaveBtn.gameObject.SetActive(true);
				AddNewSave(mainSaveBtn, text);
			}
			else
			{
				mainSaveBtn.gameObject.SetActive(false);
			}
			if (list.Count > 1)
			{
				for (int i = 1; i < list.Count; i++)
				{
					AddNewSave(buttonPool.GetFreeElement(buttonGroup, true), list[i]);
				}
			}
			Show(true);
		}
	}

	private void ConfirmDoAction(string message, System.Action action)
	{
		if ((UnityEngine.Object)confirmScreen == (UnityEngine.Object)null)
		{
			confirmScreen = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, false);
			confirmScreen.PopupConfirmDialog(message, action, delegate
			{
			}, null, null, null, null, null, null);
			confirmScreen.GetComponent<LayoutElement>().ignoreLayout = true;
			confirmScreen.gameObject.SetActive(true);
		}
	}

	private void DeleteSave()
	{
		if (string.IsNullOrEmpty(currentPath))
		{
			Debug.LogError("The path provided is not valid and cannot be deleted.", null);
		}
		else
		{
			ConfirmDoAction(UI.FRONTEND.LOADSCREEN.CONFIRMDELETE, delegate
			{
				string[] files = Directory.GetFiles(currentPath);
				string[] array = files;
				foreach (string path in array)
				{
					File.Delete(path);
				}
				Directory.Delete(currentPath);
				CloseScreen();
			});
		}
	}

	private void AddNewSave(KButton btn, string file)
	{
	}

	private void ButtonClicked(KButton btn)
	{
		LoadingOverlay.Load(delegate
		{
			Load(buttonFileMap[btn]);
		});
	}

	private void Load(string filename)
	{
		if ((UnityEngine.Object)Game.Instance != (UnityEngine.Object)null)
		{
			LoadScreen.ForceStopGame();
		}
		SaveLoader.SetActiveSaveFilePath(filename);
		App.LoadScene("backend");
		Deactivate();
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
		{
			CloseScreen();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}
}
