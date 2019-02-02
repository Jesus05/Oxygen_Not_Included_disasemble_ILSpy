using Klei.AI;
using TUNING;
using UnityEngine;

public class PhonoboxWorkable : Workable, IWorkerPrioritizable
{
	public Phonobox owner;

	public int basePriority = RELAXATION.PRIORITY.TIER3;

	public string specificEffect = "Danced";

	public string trackingEffect = "RecentlyDanced";

	public KAnimFile[][] workerOverrideAnims = new KAnimFile[3][]
	{
		new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_phonobox_danceone_kanim")
		},
		new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_phonobox_dancetwo_kanim")
		},
		new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_phonobox_dancethree_kanim")
		}
	};

	private PhonoboxWorkable()
	{
		SetReportType(ReportManager.ReportType.PersonalTime);
	}

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

	protected override void OnStartWork(Worker worker)
	{
		owner.AddWorker(worker);
	}

	protected override void OnStopWork(Worker worker)
	{
		owner.RemoveWorker(worker);
	}

	public override AnimInfo GetAnim(Worker worker)
	{
		int num = Random.Range(0, workerOverrideAnims.Length);
		overrideAnims = workerOverrideAnims[num];
		return base.GetAnim(worker);
	}
}
