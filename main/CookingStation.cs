using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class CookingStation : ComplexFabricator, IEffectDescriptor
{
	[SerializeField]
	private int diseaseCountKillRate = 100;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		choreType = Db.Get().ChoreTypes.Cook;
		fetchChoreTypeIdHash = Db.Get().ChoreTypes.CookFetch.IdHash;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.workable.requiredSkillPerk = Db.Get().SkillPerks.CanElectricGrill.Id;
		base.workable.WorkerStatusItem = Db.Get().DuplicantStatusItems.Cooking;
		base.workable.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_cookstation_kanim")
		};
		base.workable.AttributeConverter = Db.Get().AttributeConverters.CookingSpeed;
		base.workable.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		base.workable.SkillExperienceSkillGroup = Db.Get().SkillGroups.Cooking.Id;
		base.workable.SkillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		ComplexFabricatorWorkable workable = base.workable;
		workable.OnWorkTickActions = (Action<Worker, float>)Delegate.Combine(workable.OnWorkTickActions, (Action<Worker, float>)delegate(Worker worker, float dt)
		{
			Debug.Assert((UnityEngine.Object)worker != (UnityEngine.Object)null, "How did we get a null worker?");
			if (diseaseCountKillRate > 0)
			{
				PrimaryElement component = GetComponent<PrimaryElement>();
				int num = Math.Max(1, (int)((float)diseaseCountKillRate * dt));
				component.ModifyDiseaseCount(-num, "CookingStation");
			}
		});
	}

	protected override List<GameObject> SpawnOrderProduct(ComplexRecipe recipe)
	{
		List<GameObject> list = base.SpawnOrderProduct(recipe);
		foreach (GameObject item in list)
		{
			PrimaryElement component = item.GetComponent<PrimaryElement>();
			component.ModifyDiseaseCount(-component.DiseaseCount, "CookingStation.CompleteOrder");
		}
		GetComponent<Operational>().SetActive(false, false);
		return list;
	}

	public override List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> descriptors = base.GetDescriptors(def);
		descriptors.Add(new Descriptor(UI.BUILDINGEFFECTS.REMOVES_DISEASE, UI.BUILDINGEFFECTS.TOOLTIPS.REMOVES_DISEASE, Descriptor.DescriptorType.Effect, false));
		return descriptors;
	}
}
