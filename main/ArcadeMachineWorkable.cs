using Klei.AI;
using TUNING;

public class ArcadeMachineWorkable : Workable, IWorkerPrioritizable
{
	public int basePriority = RELAXATION.PRIORITY.TIER4;

	private static string specificEffect = "PlayedArcade";

	private static string trackingEffect = "RecentlyPlayedArcade";

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		synchronizeAnims = false;
		showProgressBar = true;
		resetProgressOnStop = true;
		SetWorkTime(15f);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(trackingEffect))
		{
			component.Add(trackingEffect, true);
		}
		if (!string.IsNullOrEmpty(specificEffect))
		{
			component.Add(specificEffect, true);
		}
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = basePriority;
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(trackingEffect) && component.HasEffect(trackingEffect))
		{
			priority = 0;
			return false;
		}
		if (!string.IsNullOrEmpty(specificEffect) && component.HasEffect(specificEffect))
		{
			priority = RELAXATION.PRIORITY.RECENTLY_USED;
		}
		return true;
	}
}
