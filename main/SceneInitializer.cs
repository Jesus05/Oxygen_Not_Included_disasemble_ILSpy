using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
	public const int MAXDEPTH = -30000;

	public const int SCREENDEPTH = -1000;

	public GameObject prefab_NewSaveGame;

	public List<GameObject> preloadPrefabs = new List<GameObject>();

	public List<GameObject> prefabs = new List<GameObject>();

	public static SceneInitializer Instance
	{
		get;
		private set;
	}

	private void Awake()
	{
		Localization.SwapToLocalizedFont();
		string environmentVariable = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
		string text = Application.dataPath + Path.DirectorySeparatorChar + "Plugins";
		if (!environmentVariable.Contains(text))
		{
			Environment.SetEnvironmentVariable("PATH", environmentVariable + Path.PathSeparator + text, EnvironmentVariableTarget.Process);
		}
		Instance = this;
		PreLoadPrefabs();
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	private void PreLoadPrefabs()
	{
		foreach (GameObject preloadPrefab in preloadPrefabs)
		{
			if ((UnityEngine.Object)preloadPrefab != (UnityEngine.Object)null)
			{
				Util.KInstantiate(preloadPrefab, preloadPrefab.transform.GetPosition(), Quaternion.identity, base.gameObject, null, true, 0);
			}
		}
	}

	public void NewSaveGamePrefab()
	{
		if ((UnityEngine.Object)prefab_NewSaveGame != (UnityEngine.Object)null && (UnityEngine.Object)SaveGame.Instance == (UnityEngine.Object)null)
		{
			Util.KInstantiate(prefab_NewSaveGame, base.gameObject, null);
		}
	}

	public void PostLoadPrefabs()
	{
		foreach (GameObject prefab in prefabs)
		{
			if ((UnityEngine.Object)prefab != (UnityEngine.Object)null)
			{
				Util.KInstantiate(prefab, base.gameObject, null);
			}
		}
	}
}
