using System.Collections.Generic;
using UnityEngine;

public abstract class Reactable
{
	public delegate bool ReactablePrecondition(GameObject go, Navigator.ActiveTransition transition);

	private HandleVector<int>.Handle partitionerEntry;

	protected GameObject gameObject;

	public HashedString id;

	public bool preventChoreInterruption = true;

	public int sourceCell;

	private int rangeWidth;

	private int rangeHeight;

	private int transformId = -1;

	public float minReactableTime;

	public float minReactorTime;

	public float maxTriggerTime = float.PositiveInfinity;

	private float lastTriggerTime = -2.14748365E+09f;

	private float creationTime;

	protected GameObject reactor;

	private ChoreType choreType;

	protected LoggerFSS log;

	private List<ReactablePrecondition> additionalPreconditions;

	public bool IsReacting => (Object)reactor != (Object)null;

	public Reactable(GameObject gameObject, HashedString id, ChoreType chore_type, int range_width = 15, int range_height = 8, bool follow_transform = false, float min_reactable_time = 0f, float min_reactor_time = 0f, float max_trigger_time = float.PositiveInfinity)
	{
		rangeHeight = range_height;
		rangeWidth = range_width;
		this.id = id;
		this.gameObject = gameObject;
		choreType = chore_type;
		minReactableTime = min_reactable_time;
		minReactorTime = min_reactor_time;
		maxTriggerTime = max_trigger_time;
		creationTime = GameClock.Instance.GetTime();
		UpdateLocation();
		if (follow_transform)
		{
			transformId = Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(gameObject.transform, UpdateLocation, "Reactable follow transform");
		}
	}

	public void Begin(GameObject reactor)
	{
		this.reactor = reactor;
		lastTriggerTime = GameClock.Instance.GetTime();
		InternalBegin();
	}

	public void End()
	{
		InternalEnd();
		if ((Object)reactor != (Object)null)
		{
			GameObject gameObject = reactor;
			InternalEnd();
			reactor = null;
			if ((Object)gameObject != (Object)null)
			{
				gameObject.GetSMI<ReactionMonitor.Instance>()?.StopReaction();
			}
		}
	}

	public bool CanBegin(GameObject reactor, Navigator.ActiveTransition transition)
	{
		if (!(GameClock.Instance.GetTime() - lastTriggerTime < minReactableTime))
		{
			ChoreConsumer component = reactor.GetComponent<ChoreConsumer>();
			if (!((Object)component == (Object)null))
			{
				Chore currentChore = component.choreDriver.GetCurrentChore();
				if (currentChore != null)
				{
					if (choreType.priority > currentChore.choreType.priority)
					{
						if (additionalPreconditions != null)
						{
							foreach (ReactablePrecondition additionalPrecondition in additionalPreconditions)
							{
								if (!additionalPrecondition(reactor, transition))
								{
									return false;
								}
							}
						}
						return InternalCanBegin(reactor, transition);
					}
					return false;
				}
				return false;
			}
			return false;
		}
		return false;
	}

	public bool IsExpired()
	{
		return GameClock.Instance.GetTime() - creationTime > maxTriggerTime;
	}

	public abstract bool InternalCanBegin(GameObject reactor, Navigator.ActiveTransition transition);

	public abstract void Update(float dt);

	protected abstract void InternalBegin();

	protected abstract void InternalEnd();

	protected abstract void InternalCleanup();

	public void Cleanup()
	{
		End();
		InternalCleanup();
		if (transformId != -1)
		{
			Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(transformId, UpdateLocation);
			transformId = -1;
		}
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
	}

	public void Sim1000ms(float dt)
	{
		UpdateLocation();
	}

	private void UpdateLocation()
	{
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		if ((Object)gameObject != (Object)null)
		{
			sourceCell = Grid.PosToCell(gameObject);
			Vector2I vector2I = Grid.PosToXY(gameObject.transform.GetPosition());
			int x = vector2I.x - rangeWidth / 2;
			Vector2I vector2I2 = Grid.PosToXY(gameObject.transform.GetPosition());
			Extents extents = new Extents(x, vector2I2.y - rangeHeight / 2, rangeWidth, rangeHeight);
			partitionerEntry = GameScenePartitioner.Instance.Add("Reactable", this, extents, GameScenePartitioner.Instance.objectLayers[0], null);
		}
	}

	public Reactable AddPrecondition(ReactablePrecondition precondition)
	{
		if (additionalPreconditions == null)
		{
			additionalPreconditions = new List<ReactablePrecondition>();
		}
		additionalPreconditions.Add(precondition);
		return this;
	}
}
