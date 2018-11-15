using System.Collections.Generic;

public class WorkItemCollection<WorkItemType, SharedDataType> : IWorkItemCollection where WorkItemType : IWorkItem<SharedDataType>
{
	private List<WorkItemType> items = new List<WorkItemType>();

	private SharedDataType sharedData;

	public int Count => items.Count;

	public WorkItemType GetWorkItem(int idx)
	{
		return items[idx];
	}

	public void Add(WorkItemType work_item)
	{
		items.Add(work_item);
	}

	public void InternalDoWorkItem(int work_item_idx)
	{
		WorkItemType value = items[work_item_idx];
		value.Run(sharedData);
		items[work_item_idx] = value;
	}

	public void Reset(SharedDataType shared_data)
	{
		sharedData = shared_data;
		items.Clear();
	}
}
