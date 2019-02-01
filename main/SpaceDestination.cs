using Database;
using KSerialization;
using STRINGS;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SpaceDestination
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class ResearchOpportunity
	{
		[Serialize]
		public string description;

		[Serialize]
		public int dataValue;

		[Serialize]
		public bool completed;

		[Serialize]
		public SimHashes discoveredRareResource = SimHashes.Void;

		[Serialize]
		public string discoveredRareItem;

		public ResearchOpportunity(string description, int pointValue)
		{
			this.description = description;
			dataValue = pointValue;
		}

		[OnDeserialized]
		private void OnDeserialized()
		{
			if (discoveredRareResource == (SimHashes)0)
			{
				discoveredRareResource = SimHashes.Void;
			}
			if (dataValue > 50)
			{
				dataValue = 50;
			}
		}

		public bool TryComplete(SpaceDestination destination)
		{
			if (completed)
			{
				return false;
			}
			completed = true;
			if (discoveredRareResource != SimHashes.Void && !destination.recoverableElements.ContainsKey(discoveredRareResource))
			{
				destination.recoverableElements.Add(discoveredRareResource, Random.value);
			}
			return true;
		}
	}

	private static List<Tuple<float, int>> RARE_ELEMENT_CHANCES = new List<Tuple<float, int>>
	{
		new Tuple<float, int>(1f, 0),
		new Tuple<float, int>(0.33f, 1),
		new Tuple<float, int>(0.03f, 2)
	};

	private static readonly List<Tuple<SimHashes, MathUtil.MinMax>> RARE_ELEMENTS = new List<Tuple<SimHashes, MathUtil.MinMax>>
	{
		new Tuple<SimHashes, MathUtil.MinMax>(SimHashes.Katairite, new MathUtil.MinMax(1f, 10f)),
		new Tuple<SimHashes, MathUtil.MinMax>(SimHashes.Niobium, new MathUtil.MinMax(1f, 10f)),
		new Tuple<SimHashes, MathUtil.MinMax>(SimHashes.Fullerene, new MathUtil.MinMax(1f, 10f)),
		new Tuple<SimHashes, MathUtil.MinMax>(SimHashes.Isoresin, new MathUtil.MinMax(1f, 10f))
	};

	private const float RARE_ITEM_CHANCE = 0.33f;

	private static readonly List<Tuple<string, MathUtil.MinMax>> RARE_ITEMS = new List<Tuple<string, MathUtil.MinMax>>
	{
		new Tuple<string, MathUtil.MinMax>("GeneShufflerRecharge", new MathUtil.MinMax(1f, 2f))
	};

	[Serialize]
	public int id;

	[Serialize]
	public string type;

	public bool startAnalyzed;

	[Serialize]
	public int distance;

	[Serialize]
	public float activePeriod = 20f;

	[Serialize]
	public float inactivePeriod = 10f;

	[Serialize]
	public float startingOrbitPercentage;

	[Serialize]
	public Dictionary<SimHashes, float> recoverableElements = new Dictionary<SimHashes, float>();

	[Serialize]
	public List<ResearchOpportunity> researchOpportunities = new List<ResearchOpportunity>();

	public List<SpaceMission> missions = new List<SpaceMission>();

	public int OneBasedDistance => distance + 1;

	public SpaceDestination(int id, string type, int distance)
	{
		this.id = id;
		this.type = type;
		this.distance = distance;
		GenerateSurfaceElements();
		GenerateMissions();
		GenerateResearchOpportunities();
	}

	private static Tuple<SimHashes, MathUtil.MinMax> GetRareElement(SimHashes id)
	{
		foreach (Tuple<SimHashes, MathUtil.MinMax> rARE_ELEMENT in RARE_ELEMENTS)
		{
			if (rARE_ELEMENT.first == id)
			{
				return rARE_ELEMENT;
			}
		}
		return null;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
	}

	public SpaceDestinationType GetDestinationType()
	{
		return Db.Get().SpaceDestinationTypes.Get(type);
	}

	public float GetCurrentOrbitPercentage()
	{
		float num = 0.1f * Mathf.Pow((float)OneBasedDistance, 2f);
		float num2 = (float)GameClock.Instance.GetCycle() + GameClock.Instance.GetCurrentCycleAsPercentage();
		return (num2 + startingOrbitPercentage * num) % num / num;
	}

	public ResearchOpportunity TryCompleteResearchOpportunity()
	{
		foreach (ResearchOpportunity researchOpportunity in researchOpportunities)
		{
			if (researchOpportunity.TryComplete(this))
			{
				return researchOpportunity;
			}
		}
		return null;
	}

	public void GenerateSurfaceElements()
	{
		foreach (KeyValuePair<SimHashes, MathUtil.MinMax> item in GetDestinationType().elementTable)
		{
			recoverableElements.Add(item.Key, Random.value);
		}
	}

	public SpacecraftManager.DestinationAnalysisState AnalysisState()
	{
		return SpacecraftManager.instance.GetDestinationAnalysisState(this);
	}

	public void GenerateResearchOpportunities()
	{
		researchOpportunities.Add(new ResearchOpportunity(UI.STARMAP.DESTINATIONSTUDY.UPPERATMO, ROCKETRY.DESTINATION_RESEARCH.BASIC));
		researchOpportunities.Add(new ResearchOpportunity(UI.STARMAP.DESTINATIONSTUDY.LOWERATMO, ROCKETRY.DESTINATION_RESEARCH.BASIC));
		researchOpportunities.Add(new ResearchOpportunity(UI.STARMAP.DESTINATIONSTUDY.MAGNETICFIELD, ROCKETRY.DESTINATION_RESEARCH.BASIC));
		researchOpportunities.Add(new ResearchOpportunity(UI.STARMAP.DESTINATIONSTUDY.SURFACE, ROCKETRY.DESTINATION_RESEARCH.BASIC));
		researchOpportunities.Add(new ResearchOpportunity(UI.STARMAP.DESTINATIONSTUDY.SUBSURFACE, ROCKETRY.DESTINATION_RESEARCH.BASIC));
		float num = 0f;
		foreach (Tuple<float, int> rARE_ELEMENT_CHANCE in RARE_ELEMENT_CHANCES)
		{
			num += rARE_ELEMENT_CHANCE.first;
		}
		float num2 = Random.value * num;
		int num3 = 0;
		foreach (Tuple<float, int> rARE_ELEMENT_CHANCE2 in RARE_ELEMENT_CHANCES)
		{
			num2 -= rARE_ELEMENT_CHANCE2.first;
			if (num2 <= 0f)
			{
				num3 = rARE_ELEMENT_CHANCE2.second;
			}
		}
		for (int i = 0; i < num3; i++)
		{
			researchOpportunities[Random.Range(0, researchOpportunities.Count)].discoveredRareResource = RARE_ELEMENTS[Random.Range(0, RARE_ELEMENTS.Count)].first;
		}
		if (Random.value < 0.33f)
		{
			int index = Random.Range(0, researchOpportunities.Count);
			ResearchOpportunity researchOpportunity = researchOpportunities[index];
			researchOpportunity.discoveredRareItem = RARE_ITEMS[Random.Range(0, RARE_ITEMS.Count)].first;
		}
	}

	public void GenerateMissions()
	{
		bool flag = true;
		foreach (SpaceMission mission in missions)
		{
			if (mission.craft == null)
			{
				flag = false;
			}
		}
		if (flag)
		{
			missions.Add(new SpaceMission(this));
		}
	}

	public float GetResourceValue(SimHashes resource, float roll)
	{
		if (!GetDestinationType().elementTable.ContainsKey(resource))
		{
			if (!SpaceDestinationTypes.extendedElementTable.ContainsKey(resource))
			{
				return 0f;
			}
			return SpaceDestinationTypes.extendedElementTable[resource].Lerp(roll);
		}
		return GetDestinationType().elementTable[resource].Lerp(roll);
	}

	public Dictionary<SimHashes, float> GetMissionResourceResult(float totalCargoSpace, bool solids = true, bool liquids = true, bool gasses = true)
	{
		Dictionary<SimHashes, float> dictionary = new Dictionary<SimHashes, float>();
		float num = 0f;
		foreach (KeyValuePair<SimHashes, float> recoverableElement in recoverableElements)
		{
			if ((ElementLoader.FindElementByHash(recoverableElement.Key).IsSolid && solids) || (ElementLoader.FindElementByHash(recoverableElement.Key).IsLiquid && liquids) || (ElementLoader.FindElementByHash(recoverableElement.Key).IsGas && gasses))
			{
				num += GetResourceValue(recoverableElement.Key, recoverableElement.Value);
			}
		}
		foreach (KeyValuePair<SimHashes, float> recoverableElement2 in recoverableElements)
		{
			if ((ElementLoader.FindElementByHash(recoverableElement2.Key).IsSolid && solids) || (ElementLoader.FindElementByHash(recoverableElement2.Key).IsLiquid && liquids) || (ElementLoader.FindElementByHash(recoverableElement2.Key).IsGas && gasses))
			{
				dictionary.Add(recoverableElement2.Key, totalCargoSpace * (GetResourceValue(recoverableElement2.Key, recoverableElement2.Value) / num));
			}
		}
		return dictionary;
	}

	public Dictionary<Tag, int> GetRecoverableEntities()
	{
		Dictionary<Tag, int> dictionary = new Dictionary<Tag, int>();
		Dictionary<string, int> recoverableEntities = GetDestinationType().recoverableEntities;
		if (recoverableEntities != null)
		{
			foreach (KeyValuePair<string, int> item in recoverableEntities)
			{
				dictionary.Add(item.Key, item.Value);
			}
		}
		return dictionary;
	}

	public Dictionary<Tag, int> GetMissionEntityResult()
	{
		return GetRecoverableEntities();
	}
}
