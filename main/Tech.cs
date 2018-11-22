using System.Collections.Generic;
using UnityEngine;

public class Tech : Resource
{
	public List<Tech> requiredTech = new List<Tech>();

	public List<Tech> unlockedTech = new List<Tech>();

	public List<TechItem> unlockedItems = new List<TechItem>();

	public int tier;

	public Dictionary<string, float> costsByResearchTypeID = new Dictionary<string, float>();

	public string desc;

	private ResourceTreeNode node;

	public Vector2 center => node.center;

	public float width => node.width;

	public float height => node.height;

	public List<ResourceTreeNode.Edge> edges => node.edges;

	public Tech(string id, ResourceSet parent, string name, string desc, ResourceTreeNode node)
		: base(id, parent, name)
	{
		this.desc = desc;
		this.node = node;
	}

	public bool CanAfford(ResearchPointInventory pointInventory)
	{
		foreach (KeyValuePair<string, float> item in costsByResearchTypeID)
		{
			if (pointInventory.PointsByTypeID[item.Key] < item.Value)
			{
				return false;
			}
		}
		return true;
	}

	public string CostString(ResearchTypes types)
	{
		string text = "";
		foreach (KeyValuePair<string, float> item in costsByResearchTypeID)
		{
			text += $"{types.GetResearchType(item.Key).name.ToString()}:{item.Value.ToString()}";
			text += "\n";
		}
		return text;
	}

	public bool IsComplete()
	{
		if (!((Object)Research.Instance != (Object)null))
		{
			return false;
		}
		return Research.Instance.Get(this)?.IsComplete() ?? false;
	}

	public bool ArePrerequisitesComplete()
	{
		foreach (Tech item in requiredTech)
		{
			if (!item.IsComplete())
			{
				return false;
			}
		}
		return true;
	}
}
