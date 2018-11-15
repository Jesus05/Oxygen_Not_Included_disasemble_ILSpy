using System;
using System.Collections.Generic;

public abstract class AsyncLoader
{
	public virtual Type[] GatherDependencies()
	{
		return null;
	}

	public virtual void CollectLoaders(List<AsyncLoader> loaders)
	{
	}

	public abstract void Run();
}
