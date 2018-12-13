using STRINGS;
using System;
using System.IO;
using UnityEngine;

namespace Klei
{
	public static class FileUtil
	{
		public static FileStream Create(string filename)
		{
			FileStream result = null;
			try
			{
				result = File.Create(filename);
				return result;
			}
			catch (Exception ex)
			{
				string text = null;
				if (ex is UnauthorizedAccessException)
				{
					text = string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_UNAUTHORIZED, filename);
				}
				else if (ex is IOException)
				{
					text = string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_SUFFICIENT_SPACE, filename);
				}
				if (text == null)
				{
					throw ex;
				}
				Debug.Log(text, null);
				GameObject parent = (!((UnityEngine.Object)FrontEndManager.Instance == (UnityEngine.Object)null)) ? FrontEndManager.Instance.gameObject : GameScreenManager.Instance.ssOverlayCanvas;
				ConfirmDialogScreen component = Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, parent, true).GetComponent<ConfirmDialogScreen>();
				component.PopupConfirmDialog(text, null, null, null, null, null, null, null, null);
				UnityEngine.Object.DontDestroyOnLoad(component.gameObject);
				return result;
			}
		}

		public static bool CreateDirectory(string path)
		{
			bool result = false;
			try
			{
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				result = true;
				return result;
			}
			catch (Exception ex)
			{
				string text = null;
				if (ex is UnauthorizedAccessException)
				{
					text = string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_UNAUTHORIZED, path);
				}
				else if (ex is IOException)
				{
					text = string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_SUFFICIENT_SPACE, path);
				}
				if (text == null)
				{
					throw ex;
				}
				Debug.Log(text, null);
				GameObject parent = (!((UnityEngine.Object)FrontEndManager.Instance == (UnityEngine.Object)null)) ? FrontEndManager.Instance.gameObject : GameScreenManager.Instance.ssOverlayCanvas;
				ConfirmDialogScreen component = Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, parent, true).GetComponent<ConfirmDialogScreen>();
				component.PopupConfirmDialog(text, null, null, null, null, null, null, null, null);
				UnityEngine.Object.DontDestroyOnLoad(component.gameObject);
				return result;
			}
		}

		public static bool DeleteDirectory(string path)
		{
			if (Directory.Exists(path))
			{
				try
				{
					Directory.Delete(path, true);
					return true;
				}
				catch (Exception ex)
				{
					string text = null;
					if (ex is UnauthorizedAccessException)
					{
						text = string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_UNAUTHORIZED, path);
					}
					if (text == null)
					{
						throw ex;
					}
					Debug.Log(text, null);
					GameObject parent = (!((UnityEngine.Object)FrontEndManager.Instance == (UnityEngine.Object)null)) ? FrontEndManager.Instance.gameObject : GameScreenManager.Instance.ssOverlayCanvas;
					ConfirmDialogScreen component = Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, parent, true).GetComponent<ConfirmDialogScreen>();
					component.PopupConfirmDialog(text, null, null, null, null, null, null, null, null);
					UnityEngine.Object.DontDestroyOnLoad(component.gameObject);
					return false;
				}
			}
			return true;
		}
	}
}
