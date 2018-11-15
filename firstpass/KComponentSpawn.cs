using UnityEngine;

public class KComponentSpawn : MonoBehaviour, ISim200ms, ISim33ms
{
	public static KComponentSpawn instance;

	public KComponents comps;

	private void FixedUpdate()
	{
		KComponentCleanUp.SetInCleanUpPhase(false);
		comps.Spawn();
	}

	private void Update()
	{
		KComponentCleanUp.SetInCleanUpPhase(false);
		comps.Spawn();
		comps.RenderEveryTick(Time.deltaTime);
	}

	public void Sim33ms(float dt)
	{
		comps.Sim33ms(dt);
	}

	public void Sim200ms(float dt)
	{
		comps.Sim200ms(dt);
	}

	private void OnDestroy()
	{
		comps.Shutdown();
	}
}
