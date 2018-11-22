using System.Collections.Generic;

public interface IHasBuildQueue
{
	int NumOrders
	{
		get;
	}

	int CurrentOrderIdx
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

	int MaxOrders
	{
		get;
	}

	void CancelUserOrder(int idx);

	void SetCurrentUserOrderByIndex(int userOrderIndex);
}
