using UnityEngine;

public class KComponentCleanUp : MonoBehaviour
{
	public static KComponentCleanUp instance;

	private static bool inCleanUpPhase = false;

	private KComponents comps;

	public static bool InCleanUpPhase => inCleanUpPhase;

	public static void SetInCleanUpPhase(bool in_cleanup_phase)
	{
		inCleanUpPhase = in_cleanup_phase;
	}

	private void Awake()
	{
		instance = this;
		comps = GetComponent<KComponentSpawn>().comps;
	}

	private void FixedUpdate()
	{
		SetInCleanUpPhase(true);
	}

	private void Update()
	{
		SetInCleanUpPhase(true);
		comps.CleanUp();
	}
}
