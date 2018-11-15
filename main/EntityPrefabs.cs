using UnityEngine;

public class EntityPrefabs : KMonoBehaviour
{
	public GameObject SelectMarker;

	public GameObject ForegroundLayer;

	public static EntityPrefabs Instance
	{
		get;
		private set;
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
	}
}
