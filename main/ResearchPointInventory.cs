using System.Collections.Generic;
using System.Runtime.Serialization;

public class ResearchPointInventory
{
	public Dictionary<string, float> PointsByTypeID = new Dictionary<string, float>();

	public ResearchPointInventory()
	{
		foreach (ResearchType type in Research.Instance.researchTypes.Types)
		{
			PointsByTypeID.Add(type.id, 0f);
		}
	}

	public void AddResearchPoints(string researchTypeID, float points)
	{
		if (!PointsByTypeID.ContainsKey(researchTypeID))
		{
			Debug.LogWarning("Research inventory is missing research point key " + researchTypeID);
		}
		else
		{
			Dictionary<string, float> pointsByTypeID;
			string key;
			(pointsByTypeID = PointsByTypeID)[key = researchTypeID] = pointsByTypeID[key] + points;
		}
	}

	public void RemoveResearchPoints(string researchTypeID, float points)
	{
		AddResearchPoints(researchTypeID, 0f - points);
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		foreach (ResearchType type in Research.Instance.researchTypes.Types)
		{
			if (!PointsByTypeID.ContainsKey(type.id))
			{
				PointsByTypeID.Add(type.id, 0f);
			}
		}
	}
}
