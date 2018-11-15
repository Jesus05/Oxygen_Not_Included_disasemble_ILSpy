using UnityEngine;

public class WorkableReactable : Reactable
{
	public enum AllowedDirection
	{
		Any,
		Left,
		Right
	}

	protected Workable workable;

	private Worker worker;

	public AllowedDirection allowedDirection;

	public WorkableReactable(Workable workable, HashedString id, ChoreType chore_type, AllowedDirection allowed_direction = AllowedDirection.Any)
		: base(workable.gameObject, id, chore_type, 1, 1, false, 0f, 0f, float.PositiveInfinity)
	{
		this.workable = workable;
		allowedDirection = allowed_direction;
	}

	public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
	{
		if ((Object)workable == (Object)null)
		{
			return false;
		}
		if ((Object)reactor != (Object)null)
		{
			return false;
		}
		Brain component = new_reactor.GetComponent<Brain>();
		if ((Object)component == (Object)null)
		{
			return false;
		}
		if (!component.IsRunning())
		{
			return false;
		}
		Navigator component2 = new_reactor.GetComponent<Navigator>();
		if ((Object)component2 == (Object)null)
		{
			return false;
		}
		if (!component2.IsMoving())
		{
			return false;
		}
		if (allowedDirection == AllowedDirection.Any)
		{
			return true;
		}
		Facing component3 = new_reactor.GetComponent<Facing>();
		if ((Object)component3 == (Object)null)
		{
			return false;
		}
		bool facing = component3.GetFacing();
		if (facing && allowedDirection == AllowedDirection.Right)
		{
			return false;
		}
		if (!facing && allowedDirection == AllowedDirection.Left)
		{
			return false;
		}
		return true;
	}

	protected override void InternalBegin()
	{
		worker = reactor.GetComponent<Worker>();
		worker.StartWork(new Worker.StartWorkInfo(workable));
	}

	public override void Update(float dt)
	{
		if ((Object)worker.workable == (Object)null)
		{
			End();
		}
		else if (worker.Work(dt) != Worker.WorkResult.InProgress)
		{
			End();
		}
	}

	protected override void InternalEnd()
	{
		if ((Object)worker != (Object)null)
		{
			worker.StopWork();
		}
	}

	protected override void InternalCleanup()
	{
	}
}
