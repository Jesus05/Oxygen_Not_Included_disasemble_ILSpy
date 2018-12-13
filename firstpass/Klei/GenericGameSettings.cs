using System;
using System.IO;
using UnityEngine;

namespace Klei
{
	public class GenericGameSettings : YamlIO<GenericGameSettings>
	{
		private static GenericGameSettings _instance;

		public static GenericGameSettings instance
		{
			get
			{
				if (_instance == null)
				{
					try
					{
						YamlIO<GenericGameSettings>.LoadFile(Path, null);
					}
					catch
					{
						_instance = new GenericGameSettings();
					}
				}
				return _instance;
			}
		}

		public bool demoMode
		{
			get;
			private set;
		}

		public bool sleepWhenOutOfFocus
		{
			get;
			private set;
		}

		public int demoTime
		{
			get;
			private set;
		}

		public bool showDemoTimer
		{
			get;
			private set;
		}

		public bool debugEnable
		{
			get;
			private set;
		}

		public bool developerDebugEnable
		{
			get;
			private set;
		}

		public float developerCaptureGCStatsTime
		{
			get;
			set;
		}

		public bool disableGameOver
		{
			get;
			private set;
		}

		public bool disablePopFx
		{
			get;
			private set;
		}

		public bool autoResumeGame
		{
			get;
			private set;
		}

		public bool disableFogOfWar
		{
			get;
			private set;
		}

		public bool acceleratedLifecycle
		{
			get;
			private set;
		}

		public bool enableEditorCrashReporting
		{
			get;
			private set;
		}

		public bool allowInsufficientMaterialBuild
		{
			get;
			private set;
		}

		public bool keepAllAutosaves
		{
			get;
			private set;
		}

		public bool takeSaveScreenshots
		{
			get;
			private set;
		}

		public bool disableAutosave
		{
			get;
			private set;
		}

		private static string Path => System.IO.Path.GetDirectoryName(Application.dataPath) + "/settings.yml";

		public GenericGameSettings()
		{
			demoMode = false;
			demoTime = 300;
			showDemoTimer = true;
			sleepWhenOutOfFocus = true;
			debugEnable = false;
			developerDebugEnable = false;
			_instance = this;
		}

		public void SaveSettings()
		{
			try
			{
				Save(Path, null);
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Failed to save settings.yml: " + ex.ToString(), null);
			}
		}
	}
}
