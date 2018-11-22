using System.Collections.Generic;

public class BrainScheduler : KMonoBehaviour, IRenderEveryTick
{
	private class BrainGroup
	{
		private const int BRAIN_UPDATES_PER_FRAME = 1;

		private List<Brain> brains = new List<Brain>();

		private List<Brain> upodatedBrains = new List<Brain>();

		public Tag tag
		{
			get;
			private set;
		}

		public BrainGroup(Tag tag)
		{
			this.tag = tag;
		}

		public void AddBrain(Brain brain)
		{
			brains.Add(brain);
		}

		public void RemoveBrain(Brain brain)
		{
			brains.Remove(brain);
		}

		public void RenderEveryTick(float dt)
		{
			upodatedBrains.Clear();
			int num = 1;
			int num2 = 0;
			while (num2 < brains.Count && num > 0)
			{
				Brain brain = brains[num2];
				if (brain.IsRunning())
				{
					brain.UpdateBrain();
					upodatedBrains.Add(brain);
					brains.RemoveAt(num2);
					num--;
				}
				else
				{
					num2++;
				}
			}
			brains.AddRange(upodatedBrains);
		}
	}

	private List<BrainGroup> brainGroups = new List<BrainGroup>();

	protected override void OnPrefabInit()
	{
		var array = new[]
		{
			new
			{
				tag = GameTags.DupeBrain
			},
			new
			{
				tag = GameTags.CreatureBrain
			}
		};
		var array2 = array;
		foreach (var anon in array2)
		{
			BrainGroup item = new BrainGroup(anon.tag);
			brainGroups.Add(item);
		}
		Components.Brains.Register(OnAddBrain, OnRemoveBrain);
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
		}
		DebugUtil.Assert(test);
	}

	public void RenderEveryTick(float dt)
	{
		if (!Game.IsQuitting() && !KMonoBehaviour.isLoadingScene)
		{
			foreach (BrainGroup brainGroup in brainGroups)
			{
				brainGroup.RenderEveryTick(dt);
			}
		}
	}
}
