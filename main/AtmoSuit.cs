using Klei.AI;
using UnityEngine;

public class AtmoSuit : KMonoBehaviour
{
	private static readonly EventSystem.IntraObjectHandler<AtmoSuit> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<AtmoSuit>(delegate(AtmoSuit component, object data)
	{
		component.RefreshStatusEffects(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-1697596308, OnStorageChangedDelegate);
	}

	private void RefreshStatusEffects(object data)
	{
		Equippable component = GetComponent<Equippable>();
		Storage component2 = GetComponent<Storage>();
		bool flag = component2.Has(GameTags.AnyWater);
		if (component.assignee != null && flag)
		{
			Ownables soleOwner = component.assignee.GetSoleOwner();
			if ((Object)soleOwner != (Object)null)
			{
				GameObject targetGameObject = soleOwner.GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
				if ((bool)targetGameObject)
				{
					Effects component3 = targetGameObject.GetComponent<Effects>();
					if (!component3.HasEffect("SoiledSuit"))
					{
						component3.Add("SoiledSuit", true);
					}
				}
			}
		}
	}
}
