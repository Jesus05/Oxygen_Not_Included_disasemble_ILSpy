using System;
using System.IO;
using UnityEngine;

namespace Klei
{
	public class GenericGameSettings
	{
		public class PerformanceCapture
		{
			public string saveGame
			{
				get;
				set;
			}

			public float waitTime
			{
				get;
				set;
			}

			public bool gcStats
			{
				get;
				set;
			}
		}

		private static GenericGameSettings _instance = null;

		public static GenericGameSettings instance
		{
			get
			{
				if (_instance == null)
				{
					try
					{
						_instance = YamlIO.LoadFile<GenericGameSettings>(Path, null, null);
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

		public PerformanceCapture performanceCapture
		{
			get;
			set;
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
			performanceCapture = new PerformanceCapture();
			_instance = this;
		}

		public void SaveSettings()
		{
			try
			{
				YamlIO.Save(this, Path, null);
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Failed to save settings.yml: " + ex.ToString());
			}
		}
	}
}
