using UnityEngine;

public class FrontEndManager : KMonoBehaviour
{
	public static FrontEndManager Instance;

	public static bool firstInit = true;

	public GameObject[] SpawnOnLoadScreens;

	public GameObject[] SpawnOnLaunchScreens;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		if (SpawnOnLoadScreens != null && SpawnOnLoadScreens.Length != 0)
		{
			GameObject[] spawnOnLoadScreens = SpawnOnLoadScreens;
			foreach (GameObject gameObject in spawnOnLoadScreens)
			{
				if ((Object)gameObject != (Object)null)
				{
					Util.KInstantiateUI(gameObject, base.gameObject, true);
				}
			}
		}
		if (firstInit)
		{
			firstInit = false;
			if (SpawnOnLaunchScreens != null && SpawnOnLoadScreens.Length != 0)
			{
				GameObject[] spawnOnLaunchScreens = SpawnOnLaunchScreens;
				foreach (GameObject gameObject2 in spawnOnLaunchScreens)
				{
					if ((Object)gameObject2 != (Object)null)
					{
						Util.KInstantiateUI(gameObject2, base.gameObject, true);
					}
				}
			}
		}
	}

	private void LateUpdate()
	{
		if (Debug.developerConsoleVisible)
		{
			Debug.developerConsoleVisible = false;
		}
		KAnimBatchManager.Instance().UpdateActiveArea(new Vector2I(0, 0), new Vector2I(9999, 9999));
		KAnimBatchManager.Instance().UpdateDirty(Time.frameCount);
		KAnimBatchManager.Instance().Render();
	}
}
