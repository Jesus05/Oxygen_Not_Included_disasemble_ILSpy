using UnityEngine;

public static class ChoreHelpers
{
	public static GameObject CreateLocator(string name, Vector3 pos)
	{
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(ApproachableLocator.ID), null, null);
		gameObject.name = name;
		gameObject.transform.SetPosition(pos);
		gameObject.gameObject.SetActive(true);
		return gameObject;
	}

	public static GameObject CreateSleepLocator(Vector3 pos)
	{
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(SleepLocator.ID), null, null);
		gameObject.name = "SLeepLocator";
		gameObject.transform.SetPosition(pos);
		gameObject.gameObject.SetActive(true);
		return gameObject;
	}

	public static void DestroyLocator(GameObject locator)
	{
		if ((Object)locator != (Object)null)
		{
			locator.gameObject.DeleteObject();
		}
	}
}
