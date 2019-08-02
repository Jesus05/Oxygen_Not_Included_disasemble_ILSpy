using STRINGS;
using System;
using System.IO;
using System.Threading;
using UnityEngine;

namespace Klei
{
	public static class FileUtil
	{
		private enum Test
		{
			NoTesting,
			RetryOnce
		}

		private const Test TEST = Test.NoTesting;

		private const int DEFAULT_RETRY_COUNT = 0;

		private const int RETRY_MILLISECONDS = 100;

		public static void ErrorDialog(string msg)
		{
			Debug.Log(msg);
			GameObject parent = (!((UnityEngine.Object)FrontEndManager.Instance == (UnityEngine.Object)null)) ? FrontEndManager.Instance.gameObject : GameScreenManager.Instance.ssOverlayCanvas;
			ConfirmDialogScreen component = Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, parent, true).GetComponent<ConfirmDialogScreen>();
			component.PopupConfirmDialog(msg, null, null, null, null, null, null, null, null, true);
			UnityEngine.Object.DontDestroyOnLoad(component.gameObject);
		}

		public static T DoIOFunc<T>(Func<T> io_op, int retry_count = 0)
		{
			UnauthorizedAccessException ex = null;
			IOException ex2 = null;
			Exception ex3 = null;
			for (int i = 0; i <= retry_count; i++)
			{
				try
				{
					return io_op();
				}
				catch (UnauthorizedAccessException ex4)
				{
					ex = ex4;
				}
				catch (IOException ex5)
				{
					ex2 = ex5;
				}
				catch (Exception ex6)
				{
					ex3 = ex6;
				}
				Thread.Sleep(i * 100);
			}
			if (ex != null)
			{
				throw ex;
			}
			if (ex2 != null)
			{
				throw ex2;
			}
			if (ex3 != null)
			{
				throw ex3;
			}
			throw new Exception("Unreachable code path in FileUtil::DoIOFunc()");
		}

		public static void DoIOAction(System.Action io_op, int retry_count = 0)
		{
			UnauthorizedAccessException ex = null;
			IOException ex2 = null;
			Exception ex3 = null;
			for (int i = 0; i <= retry_count; i++)
			{
				try
				{
					io_op();
					return;
				}
				catch (UnauthorizedAccessException ex4)
				{
					ex = ex4;
				}
				catch (IOException ex5)
				{
					ex2 = ex5;
				}
				catch (Exception ex6)
				{
					ex3 = ex6;
				}
				Thread.Sleep(i * 100);
			}
			if (ex != null)
			{
				throw ex;
			}
			if (ex2 != null)
			{
				throw ex2;
			}
			if (ex3 != null)
			{
				throw ex3;
			}
			throw new Exception("Unreachable code path in FileUtil::DoIOAction()");
		}

		public static T DoIODialog<T>(Func<T> io_op, string io_subject, T fail_result, int retry_count = 0)
		{
			try
			{
				return DoIOFunc(io_op, retry_count);
			}
			catch (UnauthorizedAccessException)
			{
				ErrorDialog(string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_UNAUTHORIZED, io_subject));
				return fail_result;
			}
			catch (IOException)
			{
				ErrorDialog(string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_SUFFICIENT_SPACE, io_subject));
				return fail_result;
			}
			catch
			{
				throw;
			}
		}

		public static FileStream Create(string filename, int retry_count = 0)
		{
			return DoIODialog(() => File.Create(filename), filename, null, retry_count);
		}

		public static bool CreateDirectory(string path, int retry_count = 0)
		{
			return DoIODialog(delegate
			{
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				return true;
			}, path, false, retry_count);
		}

		public static bool DeleteDirectory(string path, int retry_count = 0)
		{
			return DoIODialog(delegate
			{
				if (!Directory.Exists(path))
				{
					return true;
				}
				Directory.Delete(path, true);
				return true;
			}, path, false, retry_count);
		}

		public static bool FileExists(string filename, int retry_count = 0)
		{
			return DoIODialog(() => File.Exists(filename), filename, false, retry_count);
		}
	}
}
