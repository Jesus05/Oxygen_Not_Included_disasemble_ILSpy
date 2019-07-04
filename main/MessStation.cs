using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class MessStation : Workable, IGameObjectEffectDescriptor
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_use_machine_kanim")
		};
	}

	protected override void OnCompleteWork(Worker worker)
	{
		worker.workable.GetComponent<Edible>().CompleteWork(worker);
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (go.GetComponent<Storage>().Has(TableSaltConfig.ID.ToTag()))
		{
			list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.MESS_TABLE_SALT, TableSaltTuning.MORALE_MODIFIER), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.MESS_TABLE_SALT, TableSaltTuning.MORALE_MODIFIER), Descriptor.DescriptorType.Effect, false));
		}
		return list;
	}
}
