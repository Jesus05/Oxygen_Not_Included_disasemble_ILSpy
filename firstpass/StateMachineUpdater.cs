using System.Collections.Generic;
using System.Diagnostics;

public class StateMachineUpdater : Singleton<StateMachineUpdater>
{
	public class BucketGroup
	{
		private List<List<BaseUpdateBucket>> bucketFrames = new List<List<BaseUpdateBucket>>();

		private string name;

		public float accumulatedTime;

		private int nextUpdateIndex;

		private int nextBucketFrame;

		public float secondsPerSubTick
		{
			get;
			private set;
		}

		public UpdateRate updateRate
		{
			get;
			private set;
		}

		public int subTickCount => bucketFrames.Count;

		public BucketGroup(int frame_count, float seconds_per_sub_tick, UpdateRate update_rate)
		{
			for (int i = 0; i < frame_count; i++)
			{
				bucketFrames.Add(new List<BaseUpdateBucket>());
			}
			secondsPerSubTick = seconds_per_sub_tick;
			updateRate = update_rate;
			name = "BucketGroup-" + update_rate.ToString();
		}

		private void InternalAdvance(float dt)
		{
			accumulatedTime += dt;
			float dt2 = (float)subTickCount * secondsPerSubTick;
			while (accumulatedTime >= secondsPerSubTick)
			{
				AdvanceOneSubTick(dt2);
				accumulatedTime -= secondsPerSubTick;
			}
		}

		public void AdvanceOneSubTick(float dt)
		{
			List<BaseUpdateBucket> list = bucketFrames[nextUpdateIndex];
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				BaseUpdateBucket baseUpdateBucket = list[i];
				if (baseUpdateBucket.count != 0)
				{
					baseUpdateBucket.Update(dt);
				}
			}
			nextUpdateIndex = (nextUpdateIndex + 1) % bucketFrames.Count;
		}

		public void Advance(float dt)
		{
			InternalAdvance(dt);
		}

		public void AddBucket(BaseUpdateBucket bucket)
		{
			bucketFrames[nextBucketFrame].Add(bucket);
			bucket.frame = nextBucketFrame;
			nextBucketFrame = (nextBucketFrame + 1) % bucketFrames.Count;
		}

		public float GetFrameTime(int frame)
		{
			int num = nextUpdateIndex - 1;
			int num2 = num - frame;
			if (num2 <= 0)
			{
				num2 += bucketFrames.Count;
			}
			return (float)num2 * secondsPerSubTick;
		}
	}

	[DebuggerDisplay("{name}")]
	public abstract class BaseUpdateBucket
	{
		public int frame;

		public string name
		{
			get;
			private set;
		}

		public abstract int count
		{
			get;
		}

		public BaseUpdateBucket(string name)
		{
			this.name = name;
		}

		public abstract void Update(float dt);

		public abstract void Remove(HandleVector<int>.Handle handle);
	}

	private List<BucketGroup> bucketGroups = new List<BucketGroup>();

	private List<BucketGroup> simBucketGroups = new List<BucketGroup>();

	private List<BucketGroup> renderBucketGroups = new List<BucketGroup>();

	private List<BucketGroup> renderEveryTickBucketGroups = new List<BucketGroup>();

	public StateMachineUpdater()
	{
		Initialize();
	}

	private void Initialize()
	{
		bucketGroups = new List<BucketGroup>();
		simBucketGroups = new List<BucketGroup>();
		renderBucketGroups = new List<BucketGroup>();
		renderEveryTickBucketGroups = new List<BucketGroup>();
		CreateBucketGroup(1, 0.0166666675f, UpdateRate.RENDER_EVERY_TICK, renderEveryTickBucketGroups);
		CreateBucketGroup(12, 0.0166666675f, UpdateRate.RENDER_200ms, renderBucketGroups);
		CreateBucketGroup(60, 0.0166666675f, UpdateRate.RENDER_1000ms, renderBucketGroups);
		CreateBucketGroup(2, 0.0166666675f, UpdateRate.SIM_33ms, simBucketGroups);
		CreateBucketGroup(12, 0.0166666675f, UpdateRate.SIM_200ms, simBucketGroups);
		CreateBucketGroup(60, 0.0166666675f, UpdateRate.SIM_1000ms, simBucketGroups);
		CreateBucketGroup(240, 0.0166666675f, UpdateRate.SIM_4000ms, simBucketGroups);
	}

	private void CreateBucketGroup(int sub_tick_count, float seconds_per_sub_tick, UpdateRate update_rate, List<BucketGroup> sub_group)
	{
		BucketGroup item = new BucketGroup(sub_tick_count, seconds_per_sub_tick, update_rate);
		bucketGroups.Add(item);
		sub_group.Add(item);
	}

	public void AdvanceOneSimSubTick()
	{
		foreach (BucketGroup simBucketGroup in simBucketGroups)
		{
			float dt = (float)simBucketGroup.subTickCount * simBucketGroup.secondsPerSubTick;
			simBucketGroup.AdvanceOneSubTick(dt);
		}
	}

	public void Render(float dt)
	{
		foreach (BucketGroup renderBucketGroup in renderBucketGroups)
		{
			renderBucketGroup.Advance(dt);
		}
	}

	public void RenderEveryTick(float dt)
	{
		foreach (BucketGroup renderEveryTickBucketGroup in renderEveryTickBucketGroups)
		{
			renderEveryTickBucketGroup.AdvanceOneSubTick(dt);
		}
	}

	public int GetFrameCount(UpdateRate update_rate)
	{
		return bucketGroups[(int)update_rate].subTickCount;
	}

	public void AddBucket(UpdateRate update_rate, BaseUpdateBucket bucket)
	{
		bucketGroups[(int)update_rate].AddBucket(bucket);
	}

	public float GetFrameTime(UpdateRate update_rate, int frame)
	{
		return bucketGroups[(int)update_rate].GetFrameTime(frame);
	}

	public void Clear()
	{
		Initialize();
	}
}
