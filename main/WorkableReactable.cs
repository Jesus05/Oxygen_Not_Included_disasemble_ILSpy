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
		if (!((Object)workable == (Object)null))
		{
			if (!((Object)reactor != (Object)null))
			{
				Brain component = new_reactor.GetComponent<Brain>();
				if (!((Object)component == (Object)null))
				{
					if (component.IsRunning())
					{
						Navigator component2 = new_reactor.GetComponent<Navigator>();
						if (!((Object)component2 == (Object)null))
						{
							if (component2.IsMoving())
							{
								if (allowedDirection != 0)
								{
									Facing component3 = new_reactor.GetComponent<Facing>();
									if (!((Object)component3 == (Object)null))
									{
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
									return false;
								}
								return true;
							}
							return false;
						}
						return false;
					}
					return false;
				}
				return false;
			}
			return false;
		}
		return false;
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
