using Database;
using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Db : EntityModifierSet
{
	[Serializable]
	public class SlotInfo : Resource
	{
	}

	private static Db _Instance;

	public TextAsset researchTreeFile;

	public Diseases Diseases;

	public Database.Sicknesses Sicknesses;

	public Urges Urges;

	public AssignableSlots AssignableSlots;

	public StateMachineCategories StateMachineCategories;

	public Personalities Personalities;

	public Faces Faces;

	public Shirts Shirts;

	public Expressions Expressions;

	public Thoughts Thoughts;

	public BuildingStatusItems BuildingStatusItems;

	public MiscStatusItems MiscStatusItems;

	public CreatureStatusItems CreatureStatusItems;

	public StatusItemCategories StatusItemCategories;

	public Deaths Deaths;

	public ChoreTypes ChoreTypes;

	public Techs Techs;

	public TechItems TechItems;

	public AccessorySlots AccessorySlots;

	public Accessories Accessories;

	public ScheduleBlockTypes ScheduleBlockTypes;

	public ScheduleGroups ScheduleGroups;

	public RoomTypeCategories RoomTypeCategories;

	public RoomTypes RoomTypes;

	public ArtifactDropRates ArtifactDropRates;

	public SpaceDestinationTypes SpaceDestinationTypes;

	public SkillPerks SkillPerks;

	public SkillGroups SkillGroups;

	public Skills Skills;

	public static Db Get()
	{
		if ((UnityEngine.Object)_Instance == (UnityEngine.Object)null)
		{
			_Instance = Resources.Load<Db>("Db");
			_Instance.Initialize();
		}
		return _Instance;
	}

	public override void Initialize()
	{
		base.Initialize();
		Urges = new Urges();
		AssignableSlots = new AssignableSlots();
		StateMachineCategories = new StateMachineCategories();
		Personalities = new Personalities();
		Faces = new Faces();
		Shirts = new Shirts();
		Expressions = new Expressions(Root);
		Thoughts = new Thoughts(Root);
		Deaths = new Deaths(Root);
		StatusItemCategories = new StatusItemCategories(Root);
		Techs = new Techs(Root);
		Techs.Load(researchTreeFile);
		TechItems = new TechItems(Root);
		Accessories = new Accessories(Root);
		AccessorySlots = new AccessorySlots(Root, null, null, null);
		ScheduleBlockTypes = new ScheduleBlockTypes(Root);
		ScheduleGroups = new ScheduleGroups(Root);
		RoomTypeCategories = new RoomTypeCategories(Root);
		RoomTypes = new RoomTypes(Root);
		ArtifactDropRates = new ArtifactDropRates(Root);
		SpaceDestinationTypes = new SpaceDestinationTypes(Root);
		Diseases = new Diseases(Root);
		Sicknesses = new Database.Sicknesses(Root);
		SkillPerks = new SkillPerks(Root);
		SkillGroups = new SkillGroups(Root);
		Skills = new Skills(Root);
		MiscStatusItems = new MiscStatusItems(Root);
		CreatureStatusItems = new CreatureStatusItems(Root);
		BuildingStatusItems = new BuildingStatusItems(Root);
		ChoreTypes = new ChoreTypes(Root);
		Effect effect = new Effect("CenterOfAttention", DUPLICANTS.MODIFIERS.CENTEROFATTENTION.NAME, DUPLICANTS.MODIFIERS.CENTEROFATTENTION.TOOLTIP, 0f, true, true, false, null, 0f, null);
		effect.Add(new AttributeModifier("StressDelta", -0.008333334f, DUPLICANTS.MODIFIERS.CENTEROFATTENTION.NAME, false, false, true));
		effects.Add(effect);
		CollectResources(Root, ResourceTable);
	}

	private void CollectResources(Resource resource, List<Resource> resource_table)
	{
		if (resource.Guid != (ResourceGuid)null)
		{
			resource_table.Add(resource);
		}
		ResourceSet resourceSet = resource as ResourceSet;
		if (resourceSet != null)
		{
			for (int i = 0; i < resourceSet.Count; i++)
			{
				CollectResources(resourceSet.GetResource(i), resource_table);
			}
		}
	}

	public ResourceType GetResource<ResourceType>(ResourceGuid guid) where ResourceType : Resource
	{
		Resource resource = ResourceTable.FirstOrDefault((Resource s) => s.Guid == guid);
		if (resource == null)
		{
			Debug.LogWarning("Could not find resource: " + guid);
			return (ResourceType)null;
		}
		ResourceType val = (ResourceType)resource;
		if (val == null)
		{
			Debug.LogError("Resource type mismatch for resource: " + resource.Id + "\nExpecting Type: " + typeof(ResourceType).Name + "\nGot Type: " + resource.GetType().Name);
			return (ResourceType)null;
		}
		return val;
	}
}
