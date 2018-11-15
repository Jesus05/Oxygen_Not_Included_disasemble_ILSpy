using System.Collections.Generic;

public interface IHasBuildQueue
{
	int NumOrders
	{
		get;
	}

	List<IBuildQueueOrder> Orders
	{
		get;
	}

	bool HasWorker
	{
		get;
	}

	bool WaitingForWorker
	{
		get;
	}

	void CancelOrder(int idx);
}
