using System;

public class ScenePartitionerEntry
{
	public int x;

	public int y;

	public int width;

	public int height;

	public int layer;

	public int queryId;

	public ScenePartitioner partitioner;

	public Action<object> eventCallback;

	public object obj;

	public ScenePartitionerEntry(string name, object obj, int x, int y, int width, int height, ScenePartitionerLayer layer, ScenePartitioner partitioner, Action<object> event_callback)
	{
		if (x >= 0 && y >= 0 && width >= 0 && height >= 0)
		{
			goto IL_0025;
		}
		goto IL_0025;
		IL_0025:
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
		this.layer = layer.layer;
		this.partitioner = partitioner;
		eventCallback = event_callback;
		this.obj = obj;
	}

	public void UpdatePosition(int x, int y)
	{
		partitioner.UpdatePosition(x, y, this);
	}

	public void Release()
	{
		if (partitioner != null)
		{
			partitioner.Remove(this);
		}
	}
}
