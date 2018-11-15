using UnityEngine;

public class KTime : KMonoBehaviour
{
	public float UnscaledGameTime
	{
		get;
		set;
	}

	public static KTime Instance
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
		UnscaledGameTime = Time.unscaledTime;
	}

	protected override void OnCleanUp()
	{
		Instance = null;
	}

	public void Update()
	{
		if (!SpeedControlScreen.Instance.IsPaused)
		{
			UnscaledGameTime += Time.unscaledDeltaTime;
		}
	}
}
