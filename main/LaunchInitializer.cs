using System;
using System.IO;
using System.Threading;
using UnityEngine;

public class LaunchInitializer : MonoBehaviour
{
	public const string BUILD_PREFIX = "LU";

	public GameObject[] SpawnPrefabs;

	[SerializeField]
	private int numWaitFrames = 1;

	private void Update()
	{
		if (numWaitFrames <= Time.renderedFrameCount)
		{
			if (!SystemInfo.SupportsTextureFormat(TextureFormat.RGBAFloat))
			{
				Debug.LogError("Machine does not support RGBAFloat32");
			}
			GraphicsOptionsScreen.SetResolutionFromPrefs();
			Util.ApplyInvariantCultureToThread(Thread.CurrentThread);
			Debug.Log("preview Build: LU-" + 354426.ToString());
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			KPlayerPrefs.instance.Load();
			KFMOD.Initialize();
			for (int i = 0; i < SpawnPrefabs.Length; i++)
			{
				GameObject gameObject = SpawnPrefabs[i];
				if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
				{
					Util.KInstantiate(gameObject, base.gameObject, null);
				}
			}
			DeleteLingeringFiles();
			base.enabled = false;
		}
	}

	private static void DeleteLingeringFiles()
	{
		string[] array = new string[3]
		{
			"fmod.log",
			"load_stats_0.json",
			"OxygenNotIncluded_Data/output_log.txt"
		};
		string directoryName = Path.GetDirectoryName(Application.dataPath);
		string[] array2 = array;
		foreach (string path in array2)
		{
			string path2 = Path.Combine(directoryName, path);
			try
			{
				if (File.Exists(path2))
				{
					File.Delete(path2);
				}
			}
			catch (Exception obj)
			{
				Debug.LogWarning(obj);
			}
		}
	}
}
