using Database;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SpacecraftManager : KMonoBehaviour, ISim1000ms
{
	public enum DestinationAnalysisState
	{
		Hidden,
		Discovered,
		Complete
	}

	public static SpacecraftManager instance;

	[Serialize]
	private List<Spacecraft> spacecraft = new List<Spacecraft>();

	[Serialize]
	public List<SpaceDestination> destinations;

	[Serialize]
	public Dictionary<int, int> savedSpacecraftDestinations;

	[Serialize]
	private int nextSpacecraftID = 0;

	[Serialize]
	public bool destinationsGenerated = false;

	public const int INVALID_DESTINATION_ID = -1;

	[Serialize]
	private int analyzeDestinationID = -1;

	[Serialize]
	public Dictionary<int, float> destinationAnalysisScores = new Dictionary<int, float>();

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		instance = this;
		SpaceDestinationTypes spaceDestinationTypes = Db.Get().SpaceDestinationTypes;
		if (savedSpacecraftDestinations == null)
		{
			savedSpacecraftDestinations = new Dictionary<int, int>();
		}
		if (destinations == null)
		{
			destinations = new List<SpaceDestination>
			{
				new SpaceDestination(0, spaceDestinationTypes.CarbonaceousAsteroid.Id, 0),
				new SpaceDestination(1, spaceDestinationTypes.CarbonaceousAsteroid.Id, 0),
				new SpaceDestination(2, spaceDestinationTypes.MetallicAsteroid.Id, 1),
				new SpaceDestination(3, spaceDestinationTypes.RockyAsteroid.Id, 2),
				new SpaceDestination(4, spaceDestinationTypes.IcyDwarf.Id, 3),
				new SpaceDestination(5, spaceDestinationTypes.OrganicDwarf.Id, 4)
			};
		}
	}

	private void GenerateRandomDestinations()
	{
		System.Random random = new System.Random(SaveLoader.Instance.worldDetailSave.globalWorldSeed);
		SpaceDestinationTypes spaceDestinationTypes = Db.Get().SpaceDestinationTypes;
		List<List<string>> list = new List<List<string>>();
		list.Add(new List<string>());
		list.Add(new List<string>());
		list.Add(new List<string>
		{
			spaceDestinationTypes.Satellite.Id
		});
		list.Add(new List<string>
		{
			spaceDestinationTypes.Satellite.Id,
			spaceDestinationTypes.MetallicAsteroid.Id,
			spaceDestinationTypes.RockyAsteroid.Id,
			spaceDestinationTypes.CarbonaceousAsteroid.Id
		});
		list.Add(new List<string>
		{
			spaceDestinationTypes.MetallicAsteroid.Id,
			spaceDestinationTypes.RockyAsteroid.Id,
			spaceDestinationTypes.CarbonaceousAsteroid.Id
		});
		list.Add(new List<string>
		{
			spaceDestinationTypes.MetallicAsteroid.Id,
			spaceDestinationTypes.RockyAsteroid.Id,
			spaceDestinationTypes.CarbonaceousAsteroid.Id,
			spaceDestinationTypes.IcyDwarf.Id,
			spaceDestinationTypes.OrganicDwarf.Id
		});
		list.Add(new List<string>
		{
			spaceDestinationTypes.IcyDwarf.Id,
			spaceDestinationTypes.OrganicDwarf.Id,
			spaceDestinationTypes.DustyMoon.Id
		});
		list.Add(new List<string>
		{
			spaceDestinationTypes.IcyDwarf.Id,
			spaceDestinationTypes.OrganicDwarf.Id,
			spaceDestinationTypes.DustyMoon.Id
		});
		list.Add(new List<string>
		{
			spaceDestinationTypes.DustyMoon.Id,
			spaceDestinationTypes.TerraPlanet.Id,
			spaceDestinationTypes.VolcanoPlanet.Id
		});
		list.Add(new List<string>
		{
			spaceDestinationTypes.TerraPlanet.Id,
			spaceDestinationTypes.VolcanoPlanet.Id,
			spaceDestinationTypes.GasGiant.Id,
			spaceDestinationTypes.IceGiant.Id
		});
		list.Add(new List<string>
		{
			spaceDestinationTypes.GasGiant.Id,
			spaceDestinationTypes.IceGiant.Id
		});
		List<List<string>> list2 = list;
		List<int> list3 = new List<int>();
		int num = 3;
		int minValue = 10;
		int maxValue = 20;
		for (int i = 0; i < list2.Count; i++)
		{
			if (list2[i].Count != 0)
			{
				for (int j = 0; j < num; j++)
				{
					list3.Add(i);
				}
			}
		}
		int num2 = random.Next(minValue, maxValue);
		for (int k = 0; k < num2; k++)
		{
			int index = random.Next(0, list3.Count - 1);
			int num3 = list3[index];
			list3.RemoveAt(index);
			List<string> list4 = list2[num3];
			string type = list4[random.Next(0, list4.Count)];
			SpaceDestination item = new SpaceDestination(destinations.Count, type, num3);
			destinations.Add(item);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Game.Instance.spacecraftManager = this;
		if (!destinationsGenerated)
		{
			GenerateRandomDestinations();
			destinations.Sort((SpaceDestination a, SpaceDestination b) => a.distance.CompareTo(b.distance));
			List<float> list = new List<float>();
			for (int i = 0; i < 10; i++)
			{
				list.Add((float)i / 10f);
			}
			for (int j = 0; j < 20; j++)
			{
				list.Shuffle();
				int num = 0;
				foreach (SpaceDestination destination in destinations)
				{
					if (destination.distance == j)
					{
						num++;
						destination.startingOrbitPercentage = list[num];
					}
				}
			}
			destinationsGenerated = true;
		}
	}

	public SpaceDestination GetSpacecraftDestination(LaunchConditionManager lcm)
	{
		Spacecraft spacecraftFromLaunchConditionManager = GetSpacecraftFromLaunchConditionManager(lcm);
		return GetSpacecraftDestination(spacecraftFromLaunchConditionManager.id);
	}

	public SpaceDestination GetSpacecraftDestination(int spacecraftID)
	{
		CleanSavedSpacecraftDestinations();
		if (!savedSpacecraftDestinations.ContainsKey(spacecraftID))
		{
			return null;
		}
		return GetDestination(savedSpacecraftDestinations[spacecraftID]);
	}

	public List<int> GetSpacecraftsForDestination(SpaceDestination destination)
	{
		CleanSavedSpacecraftDestinations();
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, int> savedSpacecraftDestination in savedSpacecraftDestinations)
		{
			if (savedSpacecraftDestination.Value == destination.id)
			{
				list.Add(savedSpacecraftDestination.Key);
			}
		}
		return list;
	}

	private void CleanSavedSpacecraftDestinations()
	{
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, int> savedSpacecraftDestination in savedSpacecraftDestinations)
		{
			bool flag = false;
			foreach (Spacecraft item in spacecraft)
			{
				if (item.id == savedSpacecraftDestination.Key)
				{
					flag = true;
					break;
				}
			}
			bool flag2 = false;
			foreach (SpaceDestination destination in destinations)
			{
				if (destination.id == savedSpacecraftDestination.Value)
				{
					flag2 = true;
					break;
				}
			}
			if (!flag || !flag2)
			{
				list.Add(savedSpacecraftDestination.Key);
			}
		}
		foreach (int item2 in list)
		{
			savedSpacecraftDestinations.Remove(item2);
		}
	}

	public void SetSpacecraftDestination(LaunchConditionManager lcm, SpaceDestination destination)
	{
		Spacecraft spacecraftFromLaunchConditionManager = GetSpacecraftFromLaunchConditionManager(lcm);
		savedSpacecraftDestinations[spacecraftFromLaunchConditionManager.id] = destination.id;
	}

	public int GetSpacecraftID(LaunchableRocket rocket)
	{
		foreach (Spacecraft item in spacecraft)
		{
			if ((UnityEngine.Object)item.launchConditions.gameObject == (UnityEngine.Object)rocket.gameObject)
			{
				return item.id;
			}
		}
		return -1;
	}

	public SpaceDestination GetDestination(int destinationID)
	{
		foreach (SpaceDestination destination in destinations)
		{
			if (destination.id == destinationID)
			{
				return destination;
			}
		}
		Debug.LogErrorFormat("No space destination with ID {0}", destinationID);
		return null;
	}

	public void RegisterSpacecraft(Spacecraft craft)
	{
		if (!spacecraft.Contains(craft))
		{
			if (craft.HasInvalidID())
			{
				craft.SetID(nextSpacecraftID);
				nextSpacecraftID++;
			}
			spacecraft.Add(craft);
		}
	}

	public void UnregisterSpacecraft(LaunchConditionManager conditionManager)
	{
		Spacecraft spacecraftFromLaunchConditionManager = GetSpacecraftFromLaunchConditionManager(conditionManager);
		spacecraftFromLaunchConditionManager.SetState(Spacecraft.MissionState.Destroyed);
		spacecraft.Remove(spacecraftFromLaunchConditionManager);
	}

	public List<Spacecraft> GetSpacecraft()
	{
		return spacecraft;
	}

	public Spacecraft GetSpacecraftFromLaunchConditionManager(LaunchConditionManager lcm)
	{
		foreach (Spacecraft item in spacecraft)
		{
			if ((UnityEngine.Object)item.launchConditions == (UnityEngine.Object)lcm)
			{
				return item;
			}
		}
		return null;
	}

	public void Sim1000ms(float dt)
	{
		foreach (Spacecraft item in spacecraft)
		{
			item.ProgressMission(dt);
		}
	}

	public void PushReadyToLandNotification(Spacecraft spacecraft)
	{
		Notification notification = new Notification(BUILDING.STATUSITEMS.SPACECRAFTREADYTOLAND.NOTIFICATION, NotificationType.Good, HashedString.Invalid, (List<Notification> notificationList, object data) => BUILDING.STATUSITEMS.SPACECRAFTREADYTOLAND.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false), spacecraft.launchConditions.GetProperName(), false, 0f, null, null, null);
		base.gameObject.AddOrGet<Notifier>().Add(notification, "");
	}

	private void SpawnMissionResults(Dictionary<SimHashes, float> results)
	{
		foreach (KeyValuePair<SimHashes, float> result in results)
		{
			ElementLoader.FindElementByHash(result.Key).substance.SpawnResource(PlayerController.GetCursorPos(KInputManager.GetMousePos()), result.Value, 300f, 0, 0, false, false);
		}
	}

	public float GetDestinationAnalysisScore(SpaceDestination destination)
	{
		return GetDestinationAnalysisScore(destination.id);
	}

	public float GetDestinationAnalysisScore(int destinationID)
	{
		if (!destinationAnalysisScores.ContainsKey(destinationID))
		{
			return 0f;
		}
		return destinationAnalysisScores[destinationID];
	}

	public void EarnDestinationAnalysisPoints(int destinationID, float points)
	{
		if (!destinationAnalysisScores.ContainsKey(destinationID))
		{
			destinationAnalysisScores.Add(destinationID, 0f);
		}
		SpaceDestination destination = GetDestination(destinationID);
		DestinationAnalysisState destinationAnalysisState = GetDestinationAnalysisState(destination);
		Dictionary<int, float> dictionary;
		int key;
		(dictionary = destinationAnalysisScores)[key = destinationID] = dictionary[key] + points;
		DestinationAnalysisState destinationAnalysisState2 = GetDestinationAnalysisState(destination);
		if (destinationAnalysisState != destinationAnalysisState2)
		{
			int starmapAnalysisDestinationID = instance.GetStarmapAnalysisDestinationID();
			if (starmapAnalysisDestinationID == destinationID)
			{
				if (destinationAnalysisState2 == DestinationAnalysisState.Complete)
				{
					instance.SetStarmapAnalysisDestinationID(-1);
				}
				Trigger(532901469, null);
			}
		}
	}

	public DestinationAnalysisState GetDestinationAnalysisState(SpaceDestination destination)
	{
		if (!destination.startAnalyzed)
		{
			float destinationAnalysisScore = GetDestinationAnalysisScore(destination);
			if (!(destinationAnalysisScore >= (float)ROCKETRY.DESTINATION_ANALYSIS.COMPLETE))
			{
				if (!(destinationAnalysisScore >= (float)ROCKETRY.DESTINATION_ANALYSIS.DISCOVERED))
				{
					return DestinationAnalysisState.Hidden;
				}
				return DestinationAnalysisState.Discovered;
			}
			return DestinationAnalysisState.Complete;
		}
		return DestinationAnalysisState.Complete;
	}

	public void SetStarmapAnalysisDestinationID(int id)
	{
		analyzeDestinationID = id;
		Trigger(532901469, id);
	}

	public int GetStarmapAnalysisDestinationID()
	{
		return analyzeDestinationID;
	}

	public bool HasAnalysisTarget()
	{
		return analyzeDestinationID != -1;
	}
}
