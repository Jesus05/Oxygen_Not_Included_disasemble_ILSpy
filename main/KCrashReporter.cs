using Klei;
using Newtonsoft.Json;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class KCrashReporter : MonoBehaviour
{
	private class Error
	{
		public string game = "simgame";

		public int build = -1;

		public string platform = Environment.OSVersion.ToString();

		public string user = "unknown";

		public ulong steam64_verified = 0uL;

		public string callstack = "";

		public string fullstack = "";

		public string log = "";

		public string summaryline = "";

		public string user_message = "";

		public bool is_server = false;

		public bool is_dedicated = false;

		public string save_hash = "";
	}

	public static string MOST_RECENT_SAVEFILE = null;

	public const string CRASH_REPORTER_SERVER = "http://crashes.klei.ca";

	public static bool ignoreAll = false;

	public static bool debugWasUsed = false;

	public static string error_canvas_name = "ErrorCanvas";

	private static bool disableDeduping = false;

	private static bool hasReportedError;

	private static readonly Regex failedToLoadModuleRegEx = new Regex("^Failed to load '(.*?)' with error (.*)", RegexOptions.Multiline);

	[SerializeField]
	private LoadScreen loadScreenPrefab;

	[SerializeField]
	private GameObject reportErrorPrefab;

	[SerializeField]
	private ConfirmDialogScreen confirmDialogPrefab;

	private ReportErrorDialog errorDialog = null;

	public static bool terminateOnError = true;

	private static string dataRoot;

	private static readonly string[] IgnoreStrings = new string[3]
	{
		"Releasing render texture whose render buffer is set as Camera's target buffer with Camera.SetTargetBuffers!",
		"The profiler has run out of samples for this frame. This frame will be skipped. Increase the sample limit using Profiler.maxNumberOfSamplesPerFrame",
		"Trying to add Text (LocText) for graphic rebuild while we are already inside a graphic rebuild loop. This is not supported."
	};

	public static event Action<string> onCrashReported;

	private void OnEnable()
	{
		dataRoot = Application.dataPath;
		Application.logMessageReceived += HandleLog;
		ignoreAll = true;
		string path = Path.Combine(dataRoot, "hashes.json");
		bool flag;
		if (File.Exists(path))
		{
			StringBuilder stringBuilder = new StringBuilder();
			MD5 mD = MD5.Create();
			string value = File.ReadAllText(path);
			Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
			if (dictionary.Count > 0)
			{
				flag = true;
				foreach (KeyValuePair<string, string> item in dictionary)
				{
					string key = item.Key;
					string value2 = item.Value;
					stringBuilder.Length = 0;
					string path2 = Path.Combine(dataRoot, key);
					using (FileStream inputStream = new FileStream(path2, FileMode.Open, FileAccess.Read))
					{
						byte[] array = mD.ComputeHash(inputStream);
						byte[] array2 = array;
						foreach (byte b in array2)
						{
							stringBuilder.AppendFormat("{0:x2}", b);
						}
						string a = stringBuilder.ToString();
						if (a != value2)
						{
							flag = false;
							goto IL_014e;
						}
					}
				}
				goto IL_014e;
			}
			ignoreAll = false;
		}
		else
		{
			ignoreAll = false;
		}
		goto IL_0179;
		IL_014e:
		if (flag)
		{
			ignoreAll = false;
		}
		goto IL_0179;
		IL_0179:
		if (ignoreAll)
		{
			Debug.Log("Ignoring crash due to mismatched hashes.json entries.", null);
		}
		if (File.Exists("ignorekcrashreporter.txt"))
		{
			ignoreAll = true;
			Debug.Log("Ignoring crash due to ignorekcrashreporter.txt", null);
		}
		if (Application.isEditor && !GenericGameSettings.instance.enableEditorCrashReporting)
		{
			terminateOnError = false;
		}
	}

	private void OnDisable()
	{
	}

	private void HandleLog(string msg, string stack_trace, LogType type)
	{
		if (!ignoreAll && Array.IndexOf(IgnoreStrings, msg) == -1 && (msg == null || !msg.StartsWith("<RI.Hid>")))
		{
			if (type == LogType.Exception)
			{
				RestartWarning.ShouldWarn = true;
			}
			if ((UnityEngine.Object)errorDialog == (UnityEngine.Object)null && (type == LogType.Exception || type == LogType.Error))
			{
				if (!terminateOnError || !ReportErrorDialog.hasCrash)
				{
					if ((UnityEngine.Object)SpeedControlScreen.Instance != (UnityEngine.Object)null)
					{
						SpeedControlScreen.Instance.Pause(true);
					}
					GameObject gameObject = GameObject.Find(error_canvas_name);
					if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
					{
						gameObject = new GameObject();
						gameObject.name = error_canvas_name;
						Canvas canvas = gameObject.AddComponent<Canvas>();
						canvas.renderMode = RenderMode.ScreenSpaceOverlay;
						canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;
						gameObject.AddComponent<GraphicRaycaster>();
					}
					GameObject gameObject2 = UnityEngine.Object.Instantiate(reportErrorPrefab, Vector3.zero, Quaternion.identity);
					gameObject2.transform.SetParent(gameObject.transform, false);
					errorDialog = gameObject2.GetComponentInChildren<ReportErrorDialog>();
					errorDialog.PopupConfirmDialog("ERROR OCCURRED!\nDo you want to report this error?", delegate
					{
						string save_file_hash = null;
						if (MOST_RECENT_SAVEFILE != null)
						{
							save_file_hash = UploadSaveFile(MOST_RECENT_SAVEFILE, stack_trace, null);
						}
						ReportError(msg, stack_trace, save_file_hash, confirmDialogPrefab, errorDialog.UserMessage());
					}, delegate
					{
						OnQuitToDesktop();
					}, delegate
					{
						OnCloseErrorDialog();
					});
				}
			}
		}
	}

	private void OnCloseErrorDialog()
	{
		UnityEngine.Object.Destroy(errorDialog.gameObject);
		errorDialog = null;
		if ((UnityEngine.Object)SpeedControlScreen.Instance != (UnityEngine.Object)null)
		{
			SpeedControlScreen.Instance.Unpause(true);
		}
	}

	private void OnQuitToDesktop()
	{
		Application.Quit();
	}

	private static string UploadSaveFile(string save_file, string stack_trace, Dictionary<string, string> metadata = null)
	{
		Debug.Log($"Save_file: {save_file}", null);
		if (!KPrivacyPrefs.instance.disableDataCollection)
		{
			if (save_file != null && File.Exists(save_file))
			{
				using (WebClient webClient = new WebClient())
				{
					byte[] array = File.ReadAllBytes(save_file);
					string text = "----" + System.DateTime.Now.Ticks.ToString("x");
					webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + text);
					string @string = webClient.Encoding.GetString(array);
					string str = "";
					string text2 = default(string);
					using (SHA1CryptoServiceProvider sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider())
					{
						text2 = BitConverter.ToString(sHA1CryptoServiceProvider.ComputeHash(array)).Replace("-", "");
					}
					str += string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", text, "hash", text2);
					if (metadata != null)
					{
						string arg = JsonConvert.SerializeObject(metadata);
						str += string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", text, "metadata", arg);
					}
					str += string.Format("--{0}\r\nContent-Disposition: form-data; name=\"save\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n{3}", text, save_file, "application/x-spss-sav", @string);
					str += $"\r\n--{text}--\r\n";
					byte[] bytes = webClient.Encoding.GetBytes(str);
					Uri address = new Uri("http://crashes.klei.ca/submitSave");
					try
					{
						webClient.UploadData(address, "POST", bytes);
						return text2;
					}
					catch (Exception obj)
					{
						Debug.Log(obj, null);
						return "";
					}
				}
			}
			return "";
		}
		return "";
	}

	private static string GetUserID()
	{
		if (!DistributionPlatform.Initialized)
		{
			return Environment.UserName;
		}
		return DistributionPlatform.Inst.Name + "ID_" + DistributionPlatform.Inst.LocalUser.Name + "_" + DistributionPlatform.Inst.LocalUser.Id;
	}

	private static string GetLogContents()
	{
		string text = "";
		if (Application.platform == RuntimePlatform.WindowsEditor)
		{
			text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Unity/Editor/Editor.log");
		}
		else if (Application.platform == RuntimePlatform.WindowsPlayer)
		{
			text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "../LocalLow/Klei/Oxygen Not Included/output_log.txt");
		}
		else if (Application.platform == RuntimePlatform.OSXEditor)
		{
			text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Library/Logs/Unity/Editor.log");
		}
		else if (Application.platform == RuntimePlatform.OSXPlayer)
		{
			text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Library/Logs/Unity/Player.log");
		}
		else
		{
			if (Application.platform != RuntimePlatform.LinuxPlayer)
			{
				return "";
			}
			text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "unity3d/Klei/Oxygen Not Included/Player.log");
		}
		if (File.Exists(text))
		{
			using (FileStream stream = File.Open(text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (StreamReader streamReader = new StreamReader(stream))
				{
					return streamReader.ReadToEnd();
				}
			}
		}
		return "";
	}

	public static void ReportError(string msg, string stack_trace, string save_file_hash, ConfirmDialogScreen confirm_prefab, string userMessage = "")
	{
		if (!ignoreAll)
		{
			Debug.Log("Reporting error.\n", null);
			if (msg != null)
			{
				Debug.Log(msg, null);
			}
			if (stack_trace != null)
			{
				Debug.Log(stack_trace, null);
			}
			hasReportedError = true;
			if (!KPrivacyPrefs.instance.disableDataCollection)
			{
				string obj2 = default(string);
				using (WebClient webClient = new WebClient())
				{
					webClient.Encoding = Encoding.UTF8;
					if (string.IsNullOrEmpty(msg))
					{
						msg = "No message";
					}
					Match match = failedToLoadModuleRegEx.Match(msg);
					if (match.Success)
					{
						string path = match.Groups[1].ToString();
						string text = match.Groups[2].ToString();
						string fileName = Path.GetFileName(path);
						msg = "Failed to load '" + fileName + "' with error '" + text + "'.";
					}
					if (string.IsNullOrEmpty(stack_trace))
					{
						stack_trace = $"No stack trace.\n\n{msg}";
					}
					string str = "";
					string[] array = new string[9]
					{
						"Debug:LogError",
						"UnityEngine.Debug:LogError",
						"UnityEngine.Debug:Assert(Boolean, String)",
						"Output:LogError(String)",
						"Output:LogErrorWithObj(Object, String)",
						"Output:LogErrorWithObj(Object, Object[])",
						"DebugUtil:Assert(Boolean, String)",
						"KCrashReporter.Assert(Boolean condition, System.String message)",
						"No stack trace."
					};
					string[] array2 = stack_trace.Split('\n');
					foreach (string text2 in array2)
					{
						if (!string.IsNullOrEmpty(text2))
						{
							bool flag = false;
							string[] array3 = array;
							foreach (string value in array3)
							{
								if (text2.StartsWith(value))
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								str = str + text2 + "\n";
							}
						}
					}
					if (userMessage == UI.CRASHSCREEN.BODY.text)
					{
						userMessage = "";
					}
					Error error = new Error();
					error.user = GetUserID();
					error.callstack = stack_trace;
					if (disableDeduping)
					{
						error.callstack = error.callstack + "\n" + Guid.NewGuid().ToString();
					}
					if (debugWasUsed)
					{
						msg = "Debug tools were used in this game.\n\n" + msg;
					}
					error.fullstack = msg;
					error.build = 295825;
					error.log = GetLogContents();
					error.summaryline = msg;
					error.user_message = userMessage;
					if (!string.IsNullOrEmpty(save_file_hash))
					{
						error.save_hash = save_file_hash;
					}
					if (DistributionPlatform.Initialized)
					{
						error.steam64_verified = DistributionPlatform.Inst.LocalUser.Id.ToInt64();
					}
					string data = JsonConvert.SerializeObject(error);
					string text3 = "";
					Uri address = new Uri("http://crashes.klei.ca/submitCrash");
					Debug.Log("Submitting crash:", null);
					try
					{
						webClient.UploadStringAsync(address, data);
					}
					catch (Exception obj)
					{
						Debug.Log(obj, null);
					}
					if ((UnityEngine.Object)confirm_prefab != (UnityEngine.Object)null)
					{
						ConfirmDialogScreen confirmDialogScreen = (ConfirmDialogScreen)KScreenManager.Instance.StartScreen(confirm_prefab.gameObject, null);
						confirmDialogScreen.PopupConfirmDialog("Reported Error", null, null, null, null, null, null, null, null);
					}
					obj2 = text3;
				}
				if (KCrashReporter.onCrashReported != null)
				{
					KCrashReporter.onCrashReported(obj2);
				}
			}
		}
	}

	public static void ReportBug(string msg, string save_file)
	{
		string stack_trace = "Bug Report From: " + GetUserID() + " at " + System.DateTime.Now.ToString();
		string save_file_hash = UploadSaveFile(save_file, stack_trace, new Dictionary<string, string>
		{
			{
				"user",
				GetUserID()
			}
		});
		ReportError(msg, stack_trace, save_file_hash, ScreenPrefabs.Instance.ConfirmDialogScreen, "");
	}

	public static void Assert(bool condition, string message)
	{
		if (!condition && !hasReportedError)
		{
			StackTrace stackTrace = new StackTrace(1, true);
			ReportError("ASSERT: " + message, stackTrace.ToString(), null, null, "");
		}
	}

	public static void ReportDLLCrash(string msg, string stack_trace, string dmp_filename)
	{
		if (!hasReportedError)
		{
			string save_file_hash = null;
			string text = null;
			string text2 = null;
			if (dmp_filename != null)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(dmp_filename);
				text = Path.Combine(Path.GetDirectoryName(dataRoot), dmp_filename);
				text2 = Path.Combine(Path.GetDirectoryName(dataRoot), fileNameWithoutExtension + ".sav");
				File.Move(text, text2);
				save_file_hash = UploadSaveFile(text2, stack_trace, new Dictionary<string, string>
				{
					{
						"user",
						GetUserID()
					}
				});
			}
			ReportError(msg, stack_trace, save_file_hash, null, "");
			if (dmp_filename != null)
			{
				File.Move(text2, text);
			}
		}
	}

	public static void Assert(bool condition)
	{
		if (!condition && !hasReportedError)
		{
			StackTrace stackTrace = new StackTrace(0, true);
			ReportError("Assertion failed", stackTrace.ToString(), null, null, "");
		}
	}
}
