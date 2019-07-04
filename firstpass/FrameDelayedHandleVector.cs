using System.Collections.Generic;

public class FrameDelayedHandleVector<T> : HandleVector<T>
{
	private List<Handle>[] frameDelayedFreeHandles = new List<Handle>[3];

	private int curFrame = 0;

	public FrameDelayedHandleVector(int initial_size)
		: base(initial_size)
	{
		for (int i = 0; i < frameDelayedFreeHandles.Length; i++)
		{
			frameDelayedFreeHandles[i] = new List<Handle>();
		}
	}

	public override void Clear()
	{
		freeHandles.Clear();
		items.Clear();
		List<Handle>[] array = frameDelayedFreeHandles;
		foreach (List<Handle> list in array)
		{
			list.Clear();
		}
	}

	public override T Release(Handle handle)
	{
		frameDelayedFreeHandles[curFrame].Add(handle);
		return GetItem(handle);
	}

	public void NextFrame()
	{
		int num = (curFrame + 1) % frameDelayedFreeHandles.Length;
		List<Handle> list = frameDelayedFreeHandles[num];
		foreach (Handle item in list)
		{
			base.Release(item);
		}
		list.Clear();
		curFrame = num;
	}
}
