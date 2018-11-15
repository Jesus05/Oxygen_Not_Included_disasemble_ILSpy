using UnityEngine;

public class SceneInitializerLoader : MonoBehaviour
{
	public SceneInitializer sceneInitializer;

	private void Awake()
	{
		Camera[] array = Object.FindObjectsOfType<Camera>();
		foreach (Camera camera in array)
		{
			camera.enabled = false;
		}
		KMonoBehaviour.isLoadingScene = false;
		Singleton<StateMachineManager>.Instance.Clear();
		Util.KInstantiate(sceneInitializer, null, null);
	}
}
