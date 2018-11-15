using UnityEngine;

public class DestroyAfter : KMonoBehaviour
{
	private ParticleSystem[] particleSystems;

	protected override void OnSpawn()
	{
		particleSystems = base.gameObject.GetComponentsInChildren<ParticleSystem>(true);
	}

	private bool IsAlive()
	{
		for (int i = 0; i < particleSystems.Length; i++)
		{
			ParticleSystem particleSystem = particleSystems[i];
			if (particleSystem.IsAlive(false))
			{
				return true;
			}
		}
		return false;
	}

	private void Update()
	{
		if (particleSystems != null && !IsAlive())
		{
			this.DeleteObject();
		}
	}
}
