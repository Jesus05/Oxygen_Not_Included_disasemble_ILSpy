using System.Collections.Generic;

public class KComponents
{
	private List<IComponentManager> managers = new List<IComponentManager>();

	private bool spawned;

	public virtual void Shutdown()
	{
		managers.Clear();
	}

	protected T Add<T>(T manager) where T : IComponentManager
	{
		managers.Add((IComponentManager)(object)manager);
		return manager;
	}

	public void Spawn()
	{
		if (!spawned)
		{
			spawned = true;
			foreach (IComponentManager manager in managers)
			{
				manager.Spawn();
			}
		}
	}

	public void Sim33ms(float dt)
	{
		foreach (IComponentManager manager in managers)
		{
			manager.FixedUpdate(dt);
		}
	}

	public void RenderEveryTick(float dt)
	{
		foreach (IComponentManager manager in managers)
		{
			manager.RenderEveryTick(dt);
		}
	}

	public void Sim200ms(float dt)
	{
		foreach (IComponentManager manager in managers)
		{
			manager.Sim200ms(dt);
		}
	}

	public void CleanUp()
	{
		foreach (IComponentManager manager in managers)
		{
			manager.CleanUp();
		}
		spawned = false;
	}

	public void Clear()
	{
		foreach (IComponentManager manager in managers)
		{
			manager.Clear();
		}
		spawned = false;
	}
}
