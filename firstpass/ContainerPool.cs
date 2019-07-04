using System.Collections.Generic;

public abstract class ContainerPool
{
	public abstract string GetName();
}
public class ContainerPool<ContainerType, PoolIdentifier> : ContainerPool where ContainerType : new()
{
	private Stack<ContainerType> freeContainers = new Stack<ContainerType>();

	public ContainerType Allocate()
	{
		if (freeContainers.Count != 0)
		{
			return freeContainers.Pop();
		}
		return new ContainerType();
	}

	public void Free(ContainerType container)
	{
		freeContainers.Push(container);
	}

	public override string GetName()
	{
		return typeof(PoolIdentifier).Name + "." + typeof(ContainerType).Name;
	}
}
