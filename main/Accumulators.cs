using System.Collections.Generic;

public class Accumulators
{
	private const float TIME_WINDOW = 3f;

	private float elapsedTime;

	private KCompactedVector<float> accumulated;

	private KCompactedVector<float> average;

	public Accumulators()
	{
		elapsedTime = 0f;
		accumulated = new KCompactedVector<float>(0);
		average = new KCompactedVector<float>(0);
	}

	public HandleVector<int>.Handle Add(string name, KMonoBehaviour owner)
	{
		HandleVector<int>.Handle result = accumulated.Allocate(0f);
		average.Allocate(0f);
		return result;
	}

	public HandleVector<int>.Handle Remove(HandleVector<int>.Handle handle)
	{
		if (!handle.IsValid())
		{
			return HandleVector<int>.InvalidHandle;
		}
		accumulated.Free(handle);
		average.Free(handle);
		return HandleVector<int>.InvalidHandle;
	}

	public void Sim200ms(float dt)
	{
		elapsedTime += dt;
		if (!(elapsedTime < 3f))
		{
			elapsedTime -= 3f;
			List<float> dataList = accumulated.GetDataList();
			List<float> dataList2 = average.GetDataList();
			int count = dataList.Count;
			float num = 0.333333343f;
			for (int i = 0; i < count; i++)
			{
				dataList2[i] = dataList[i] * num;
				dataList[i] = 0f;
			}
		}
	}

	public float GetAverageRate(HandleVector<int>.Handle handle)
	{
		return (!handle.IsValid()) ? 0f : average.GetData(handle);
	}

	public void Accumulate(HandleVector<int>.Handle handle, float amount)
	{
		float data = accumulated.GetData(handle);
		accumulated.SetData(handle, data + amount);
	}
}
