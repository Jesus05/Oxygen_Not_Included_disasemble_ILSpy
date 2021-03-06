using System.Collections.Generic;
using UnityEngine;

public class Room : IAssignableIdentity
{
	public CavityInfo cavity;

	public RoomType roomType;

	private List<KPrefabID> primary_buildings = new List<KPrefabID>();

	private List<Ownables> current_owners = new List<Ownables>();

	public List<KPrefabID> buildings => cavity.buildings;

	public List<KPrefabID> plants => cavity.plants;

	public string GetProperName()
	{
		return roomType.Name;
	}

	public List<Ownables> GetOwners()
	{
		current_owners.Clear();
		foreach (KPrefabID primaryEntity in GetPrimaryEntities())
		{
			if ((Object)primaryEntity != (Object)null)
			{
				Ownable component = primaryEntity.GetComponent<Ownable>();
				if ((Object)component != (Object)null && component.assignee != null && component.assignee != this)
				{
					foreach (Ownables owner in component.assignee.GetOwners())
					{
						if (!current_owners.Contains(owner))
						{
							current_owners.Add(owner);
						}
					}
				}
			}
		}
		return current_owners;
	}

	public Ownables GetSoleOwner()
	{
		List<Ownables> owners = GetOwners();
		return (owners.Count <= 0) ? null : owners[0];
	}

	public List<KPrefabID> GetPrimaryEntities()
	{
		primary_buildings.Clear();
		RoomType roomType = this.roomType;
		if (roomType.primary_constraint != null)
		{
			foreach (KPrefabID building in buildings)
			{
				if ((Object)building != (Object)null && roomType.primary_constraint.building_criteria(building))
				{
					primary_buildings.Add(building);
				}
			}
			foreach (KPrefabID plant in plants)
			{
				if ((Object)plant != (Object)null && roomType.primary_constraint.building_criteria(plant))
				{
					primary_buildings.Add(plant);
				}
			}
		}
		return primary_buildings;
	}

	public void RetriggerBuildings()
	{
		foreach (KPrefabID building in buildings)
		{
			if (!((Object)building == (Object)null))
			{
				building.Trigger(144050788, this);
			}
		}
		foreach (KPrefabID plant in plants)
		{
			if (!((Object)plant == (Object)null))
			{
				plant.Trigger(144050788, this);
			}
		}
	}

	public bool IsNull()
	{
		return false;
	}

	public void CleanUp()
	{
		Game.Instance.assignmentManager.RemoveFromAllGroups(this);
	}
}
