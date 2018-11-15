using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class SeedSplicer : Fabricator, IEffectDescriptor
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		choreType = Db.Get().ChoreTypes.Cook;
		choreTags = new Tag[1]
		{
			GameTags.ChoreTypes.Cooking
		};
		workerStatusItem = Db.Get().DuplicantStatusItems.Cooking;
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_cookstation_kanim")
		};
		attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		return false;
	}

	protected override GameObject CompleteOrder(UserOrder completed_order)
	{
		GameObject result = base.CompleteOrder(completed_order);
		GetComponent<Operational>().SetActive(false, false);
		return result;
	}

	public override List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return base.GetDescriptors(def);
	}
}
