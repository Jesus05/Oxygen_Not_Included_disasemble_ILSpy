using System;
using System.Collections.Generic;
using UnityEngine;

public class BrainScheduler : KMonoBehaviour, IRenderEveryTick, ICPULoad
{
	private class Tuning : TuningData<Tuning>
	{
		public bool disableAsyncPathProbes = false;

		public float frameTime = 5f;
	}

	private abstract class BrainGroup : ICPULoad
	{
		private List<Brain> brains = new List<Brain>();

		private List<Brain> updatedBrains = new List<Brain>();

		private string increaseLoadLabel;

		private string decreaseLoadLabel;

		private WorkItemCollection<Navigator.PathProbeTask, object> pathProbeJob = new WorkItemCollection<Navigator.PathProbeTask, object>();

		private int nextPathProbeBrain = 0;

		public Tag tag
		{
			get;
			private set;
		}

		public int probeSize
		{
			get;
			private set;
		}

		public int probeCount
		{
			get;
			private set;
		}

		protected BrainGroup(Tag tag)
		{
			this.tag = tag;
			probeSize = InitialProbeSize();
			probeCount = InitialProbeCount();
			string str = tag.ToString();
			increaseLoadLabel = "IncLoad" + str;
			decreaseLoadLabel = "DecLoad" + str;
		}

		public void AddBrain(Brain brain)
		{
			brains.Add(brain);
		}

		public void RemoveBrain(Brain brain)
		{
			int num = brains.IndexOf(brain);
			if (num != -1)
			{
				brains.RemoveAt(num);
				if (brains.Count == 0)
				{
					nextPathProbeBrain = 0;
				}
				else if (num <= nextPathProbeBrain)
				{
					nextPathProbeBrain--;
				}
			}
		}

		public bool AdjustLoad(float currentFrameTime, float frameTimeDelta)
		{
			bool flag = frameTimeDelta > 0f;
			int num = 0;
			int num2 = Math.Max(probeCount, Math.Min(brains.Count, CPUBudget.coreCount));
			num += num2 - probeCount;
			probeCount = num2;
			float num3 = Math.Min(1f, (float)probeCount / (float)CPUBudget.coreCount);
			float num4 = num3 * (float)this.probeSize;
			float num5 = num3 * (float)this.probeSize;
			float num6 = currentFrameTime / num5;
			float num7 = frameTimeDelta / num6;
			if (num == 0)
			{
				float num8 = num4 + num7 / (float)CPUBudget.coreCount;
				int num9 = MathUtil.Clamp(MinProbeSize(), IdealProbeSize(), (int)(num8 / num3));
				num += num9 - this.probeSize;
				this.probeSize = num9;
			}
			if (num == 0)
			{
				int num10 = Math.Max(1, (int)num3 + (flag ? 1 : (-1)));
				int probeSize = MathUtil.Clamp(MinProbeSize(), IdealProbeSize(), (int)((num5 + num7) / (float)num10));
				int num11 = Math.Min(brains.Count, num10 * CPUBudget.coreCount);
				num += num11 - probeCount;
				probeCount = num11;
				this.probeSize = probeSize;
			}
			if (num == 0 && flag)
			{
				int num12 = this.probeSize + ProbeSizeStep();
				num += num12 - this.probeSize;
				this.probeSize = num12;
			}
			if (num < 0)
			{
				KProfiler.AddEvent(decreaseLoadLabel);
			}
			else if (num > 0)
			{
				KProfiler.AddEvent(increaseLoadLabel);
			}
			else
			{
				Debug.LogWarning("AdjustLoad() failed", null);
			}
			return num != 0;
		}

		private void AsyncPathProbe()
		{
			int probeSize = this.probeSize;
			pathProbeJob.Reset(null);
			for (int i = 0; i != brains.Count; i++)
			{
				Navigator component = brains[nextPathProbeBrain].GetComponent<Navigator>();
				nextPathProbeBrain++;
				if (nextPathProbeBrain < 0 || brains.Count <= nextPathProbeBrain)
				{
					nextPathProbeBrain = 0;
				}
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					component.executePathProbeTaskAsync = true;
					component.PathProber.potentialCellsPerUpdate = this.probeSize;
					component.pathProbeTask.Update();
					pathProbeJob.Add(component.pathProbeTask);
					if (pathProbeJob.Count == probeCount)
					{
						break;
					}
				}
			}
			CPUBudget.Start(this);
			GlobalJobManager.Run(pathProbeJob);
			CPUBudget.End(this);
		}

		public void RenderEveryTick(float dt, bool isAsyncPathProbeEnabled)
		{
			if (isAsyncPathProbeEnabled)
			{
				AsyncPathProbe();
			}
			updatedBrains.Clear();
			int num = InitialProbeCount();
			int num2 = 0;
			while (num2 < brains.Count && num > 0)
			{
				Brain brain = brains[num2];
				if (brain.IsRunning())
				{
					brain.UpdateBrain();
					updatedBrains.Add(brain);
					brains.RemoveAt(num2);
					num--;
				}
				else
				{
					num2++;
				}
			}
			brains.AddRange(updatedBrains);
		}

		public void AccumulatePathProbeIterations(Dictionary<string, int> pathProbeIterations)
		{
			foreach (Brain brain in brains)
			{
				Navigator component = brain.GetComponent<Navigator>();
				if (!((UnityEngine.Object)component == (UnityEngine.Object)null) && !pathProbeIterations.ContainsKey(brain.name))
				{
					pathProbeIterations.Add(brain.name, component.PathProber.updateCount);
				}
			}
		}

		protected abstract int InitialProbeCount();

		protected abstract int InitialProbeSize();

		protected abstract int MinProbeSize();

		protected abstract int IdealProbeSize();

		protected abstract int ProbeSizeStep();

		public abstract float GetEstimatedFrameTime();

		public abstract float LoadBalanceThreshold();
	}

	private class DupeBrainGroup : BrainGroup
	{
		public class Tuning : TuningData<Tuning>
		{
			public int initialProbeCount = 1;

			public int initialProbeSize = 1000;

			public int minProbeSize = 100;

			public int idealProbeSize = 1000;

			public int probeSizeStep = 100;

			public float estimatedFrameTime = 2f;

			public float loadBalanceThreshold = 0.1f;
		}

		public DupeBrainGroup()
			: base(GameTags.DupeBrain)
		{
		}

		protected override int InitialProbeCount()
		{
			return TuningData<Tuning>.Get().initialProbeCount;
		}

		protected override int InitialProbeSize()
		{
			return TuningData<Tuning>.Get().initialProbeSize;
		}

		protected override int MinProbeSize()
		{
			return TuningData<Tuning>.Get().minProbeSize;
		}

		protected override int IdealProbeSize()
		{
			return TuningData<Tuning>.Get().idealProbeSize;
		}

		protected override int ProbeSizeStep()
		{
			return TuningData<Tuning>.Get().probeSizeStep;
		}

		public override float GetEstimatedFrameTime()
		{
			return TuningData<Tuning>.Get().estimatedFrameTime;
		}

		public override float LoadBalanceThreshold()
		{
			return TuningData<Tuning>.Get().loadBalanceThreshold;
		}
	}

	private class CreatureBrainGroup : BrainGroup
	{
		public class Tuning : TuningData<Tuning>
		{
			public int initialProbeCount = 1;

			public int initialProbeSize = 1000;

			public int minProbeSize = 100;

			public int idealProbeSize = 300;

			public int probeSizeStep = 100;

			public float estimatedFrameTime = 1f;

			public float loadBalanceThreshold = 0.1f;
		}

		public CreatureBrainGroup()
			: base(GameTags.CreatureBrain)
		{
		}

		protected override int InitialProbeCount()
		{
			return TuningData<Tuning>.Get().initialProbeCount;
		}

		protected override int InitialProbeSize()
		{
			return TuningData<Tuning>.Get().initialProbeSize;
		}

		protected override int MinProbeSize()
		{
			return TuningData<Tuning>.Get().minProbeSize;
		}

		protected override int IdealProbeSize()
		{
			return TuningData<Tuning>.Get().idealProbeSize;
		}

		protected override int ProbeSizeStep()
		{
			return TuningData<Tuning>.Get().probeSizeStep;
		}

		public override float GetEstimatedFrameTime()
		{
			return TuningData<Tuning>.Get().estimatedFrameTime;
		}

		public override float LoadBalanceThreshold()
		{
			return TuningData<Tuning>.Get().loadBalanceThreshold;
		}
	}

	public const float millisecondsPerFrame = 33.33333f;

	public const float secondsPerFrame = 0.0333333276f;

	public const float framesPerSecond = 30.0000057f;

	private List<BrainGroup> brainGroups = new List<BrainGroup>();

	private bool isAsyncPathProbeEnabled => !TuningData<Tuning>.Get().disableAsyncPathProbes;

	protected override void OnPrefabInit()
	{
		brainGroups.Add(new DupeBrainGroup());
		brainGroups.Add(new CreatureBrainGroup());
		Components.Brains.Register(OnAddBrain, OnRemoveBrain);
		CPUBudget.AddRoot(this);
		foreach (BrainGroup brainGroup in brainGroups)
		{
			CPUBudget.AddChild(this, brainGroup, brainGroup.LoadBalanceThreshold());
		}
		CPUBudget.FinalizeChildren(this);
	}

	private void OnAddBrain(Brain brain)
	{
		bool test = false;
		foreach (BrainGroup brainGroup in brainGroups)
		{
			if (brain.HasTag(brainGroup.tag))
			{
				brainGroup.AddBrain(brain);
				test = true;
			}
			Navigator component = brain.GetComponent<Navigator>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				component.executePathProbeTaskAsync = isAsyncPathProbeEnabled;
			}
		}
		DebugUtil.Assert(test);
	}

	private void OnRemoveBrain(Brain brain)
	{
		bool test = false;
		foreach (BrainGroup brainGroup in brainGroups)
		{
			if (brain.HasTag(brainGroup.tag))
			{
				test = true;
				brainGroup.RemoveBrain(brain);
			}
			Navigator component = brain.GetComponent<Navigator>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				component.executePathProbeTaskAsync = false;
			}
		}
		DebugUtil.Assert(test);
	}

	public float GetEstimatedFrameTime()
	{
		return TuningData<Tuning>.Get().frameTime;
	}

	public bool AdjustLoad(float currentFrameTime, float frameTimeDelta)
	{
		return false;
	}

	public void RenderEveryTick(float dt)
	{
		if (!Game.IsQuitting() && !KMonoBehaviour.isLoadingScene)
		{
			foreach (BrainGroup brainGroup in brainGroups)
			{
				brainGroup.RenderEveryTick(dt, isAsyncPathProbeEnabled);
			}
		}
	}
}
