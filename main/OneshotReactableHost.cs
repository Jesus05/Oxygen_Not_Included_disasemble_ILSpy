using UnityEngine;

public class OneshotReactableHost : KMonoBehaviour
{
	private Reactable reactable;

	public float lifetime = 1f;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.Schedule("CleanupOneshotReactable", lifetime, OnExpire, null, null);
	}

	public void SetReactable(Reactable reactable)
	{
		this.reactable = reactable;
	}

	private void OnExpire(object obj)
	{
		if (!reactable.IsReacting)
		{
			reactable.Cleanup();
			Object.Destroy(base.gameObject);
		}
		else
		{
			GameScheduler.Instance.Schedule("CleanupOneshotReactable", 0.5f, OnExpire, null, null);
		}
	}
}
