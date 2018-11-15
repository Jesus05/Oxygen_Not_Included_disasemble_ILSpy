using System;

public class Brain : KMonoBehaviour
{
	private bool running;

	private bool suspend;

	public event System.Action onPreUpdate;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		running = true;
		Components.Brains.Add(this);
	}

	public virtual void UpdateBrain()
	{
		if (this.onPreUpdate != null)
		{
			this.onPreUpdate();
		}
		if (IsRunning())
		{
			UpdateChores();
		}
	}

	private bool FindBetterChore(ref Chore.Precondition.Context context)
	{
		return GetComponent<ChoreConsumer>().FindNextChore(ref context);
	}

	private void UpdateChores()
	{
		if (!GetComponent<KPrefabID>().HasTag(GameTags.PreventChoreInterruption))
		{
			Chore.Precondition.Context context = default(Chore.Precondition.Context);
			if (FindBetterChore(ref context))
			{
				if (this.HasTag(GameTags.PerformingWorkRequest))
				{
					Trigger(1485595942, null);
				}
				else
				{
					GetComponent<ChoreDriver>().SetChore(context);
				}
			}
		}
	}

	public bool IsRunning()
	{
		return running && !suspend;
	}

	public void Reset(string reason)
	{
		Stop("Reset");
		running = true;
	}

	public void Stop(string reason)
	{
		GetComponent<ChoreDriver>().StopChore();
		running = false;
	}

	public void Resume(string caller)
	{
		suspend = false;
	}

	public void Suspend(string caller)
	{
		suspend = true;
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		Stop("OnCmpDisable");
	}

	protected override void OnCleanUp()
	{
		Stop("OnCleanUp");
		Components.Brains.Remove(this);
	}
}
