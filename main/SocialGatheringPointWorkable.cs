using Klei.AI;
using TUNING;
using UnityEngine;

public class SocialGatheringPointWorkable : Workable, IWorkerPrioritizable
{
	private GameObject lastTalker;

	public int basePriority;

	public string specificEffect;

	private SocialGatheringPointWorkable()
	{
		SetReportType(ReportManager.ReportType.PersonalTime);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_generic_convo_kanim")
		};
		workAnims = new HashedString[1]
		{
			"idle"
		};
		faceTargetWhenWorking = true;
		workerStatusItem = Db.Get().DuplicantStatusItems.Socializing;
		synchronizeAnims = false;
		showProgressBar = false;
		resetProgressOnStop = true;
		lightEfficiencyBonus = false;
	}

	public override Vector3 GetFacingTarget()
	{
		if (!((Object)lastTalker != (Object)null))
		{
			return base.GetFacingTarget();
		}
		return lastTalker.transform.GetPosition();
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		Schedulable component = worker.GetComponent<Schedulable>();
		if (!component.IsAllowed(Db.Get().ScheduleBlockTypes.Recreation))
		{
			Effects component2 = worker.GetComponent<Effects>();
			if (string.IsNullOrEmpty(specificEffect) || component2.HasEffect(specificEffect))
			{
				return true;
			}
		}
		return false;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		KPrefabID component = worker.GetComponent<KPrefabID>();
		component.AddTag(GameTags.AlwaysConverse);
		worker.Subscribe(-594200555, OnStartedTalking);
		worker.Subscribe(25860745, OnStoppedTalking);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		KPrefabID component = worker.GetComponent<KPrefabID>();
		component.RemoveTag(GameTags.AlwaysConverse);
		worker.Unsubscribe(-594200555, OnStartedTalking);
		worker.Unsubscribe(25860745, OnStoppedTalking);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Effects component = worker.GetComponent<Effects>();
		if (!string.IsNullOrEmpty(specificEffect))
		{
			component.Add(specificEffect, true);
		}
	}

	private void OnStartedTalking(object data)
	{
		ConversationManager.StartedTalkingEvent startedTalkingEvent = (ConversationManager.StartedTalkingEvent)data;
		GameObject talker = startedTalkingEvent.talker;
		if ((Object)talker == (Object)base.worker.gameObject)
		{
			KBatchedAnimController component = base.worker.GetComponent<KBatchedAnimController>();
			string anim = startedTalkingEvent.anim;
			anim += Random.Range(1, 9).ToString();
			component.Play(anim, KAnim.PlayMode.Once, 1f, 0f);
			component.Queue("idle", KAnim.PlayMode.Loop, 1f, 0f);
		}
		else
		{
			Facing component2 = base.worker.GetComponent<Facing>();
			component2.Face(talker.transform.GetPosition());
			lastTalker = talker;
		}
	}

	private void OnStoppedTalking(object data)
	{
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = basePriority;
		if (!string.IsNullOrEmpty(specificEffect))
		{
			Effects component = worker.GetComponent<Effects>();
			if (component.HasEffect(specificEffect))
			{
				priority = RELAXATION.PRIORITY.RECENTLY_USED;
			}
		}
		return true;
	}
}
