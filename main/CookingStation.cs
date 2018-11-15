using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class CookingStation : Fabricator, IEffectDescriptor
{
	[SerializeField]
	private int diseaseCountKillRate = 100;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		choreType = Db.Get().ChoreTypes.Cook;
		fetchChoreTypeIdHash = Db.Get().ChoreTypes.CookFetch.IdHash;
		choreTags = GameTags.ChoreTypes.CookingChores;
		requiredRolePerk = RoleManager.rolePerks.CanElectricGrill.id;
		workerStatusItem = Db.Get().DuplicantStatusItems.Cooking;
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_cookstation_kanim")
		};
		attributeConverter = Db.Get().AttributeConverters.CookingSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (diseaseCountKillRate > 0)
		{
			PrimaryElement component = GetComponent<PrimaryElement>();
			int num = Math.Max(1, (int)((float)diseaseCountKillRate * dt));
			component.ModifyDiseaseCount(-num, "CookingStation");
		}
		return false;
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole(Cook.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
	}

	protected override GameObject CompleteOrder(UserOrder completed_order)
	{
		GameObject gameObject = base.CompleteOrder(completed_order);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.ModifyDiseaseCount(-component.DiseaseCount, "CookingStation.CompleteOrder");
		component.Temperature = 368.15f;
		GetComponent<Operational>().SetActive(false, false);
		return gameObject;
	}

	public override List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> descriptors = base.GetDescriptors(def);
		descriptors.Add(new Descriptor(UI.BUILDINGEFFECTS.REMOVES_DISEASE, UI.BUILDINGEFFECTS.TOOLTIPS.REMOVES_DISEASE, Descriptor.DescriptorType.Effect, false));
		return descriptors;
	}
}
