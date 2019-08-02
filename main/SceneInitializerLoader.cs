using UnityEngine;

public class SceneInitializerLoader : MonoBehaviour
{
	public struct DeferredError
	{
		public string msg;

		public string stack_trace;

		public bool IsValid => !string.IsNullOrEmpty(msg);
	}

	public delegate void DeferredErrorDelegate(DeferredError deferred_error);

	public SceneInitializer sceneInitializer;

	public static DeferredError deferred_error;

	public static DeferredErrorDelegate ReportDeferredError;

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
		if (ReportDeferredError != null && deferred_error.IsValid)
		{
			ReportDeferredError(deferred_error);
			deferred_error = default(DeferredError);
		}
	}
}
