using STRINGS;
using UnityEngine;

public class AstronautTrainingCenter : Workable
{
	public float daysToMasterRole;

	private Chore chore;

	public Chore.Precondition IsNotMarkedForDeconstruction = new Chore.Precondition
	{
		id = "IsNotMarkedForDeconstruction",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_MARKED_FOR_DECONSTRUCTION,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			Deconstructable deconstructable = data as Deconstructable;
			return (Object)deconstructable == (Object)null || !deconstructable.IsMarkedForDeconstruction();
		}
	};

	protected override void OnSpawn()
	{
		base.OnSpawn();
		chore = CreateChore();
	}

	private Chore CreateChore()
	{
		return new WorkChore<AstronautTrainingCenter>(Db.Get().ChoreTypes.Train, this, null, true, null, null, null, false, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		GetComponent<Operational>().SetActive(true, false);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if ((Object)worker == (Object)null)
		{
			return true;
		}
		return true;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		if (chore != null && !chore.isComplete)
		{
			chore.Cancel("completed but not complete??");
		}
		chore = CreateChore();
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		GetComponent<Operational>().SetActive(false, false);
	}

	public override float GetPercentComplete()
	{
		if ((Object)base.worker == (Object)null)
		{
			return 0f;
		}
		return 0f;
	}
}
