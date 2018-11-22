using UnityEngine;

public class SuitDiseaseHandler : KMonoBehaviour
{
	private static readonly EventSystem.IntraObjectHandler<SuitDiseaseHandler> OnEquippedDelegate = new EventSystem.IntraObjectHandler<SuitDiseaseHandler>(delegate(SuitDiseaseHandler component, object data)
	{
		component.OnEquipped(data);
	});

	private static readonly EventSystem.IntraObjectHandler<SuitDiseaseHandler> OnUnequippedDelegate = new EventSystem.IntraObjectHandler<SuitDiseaseHandler>(delegate(SuitDiseaseHandler component, object data)
	{
		component.OnUnequipped(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-1617557748, OnEquippedDelegate);
		Subscribe(-170173755, OnUnequippedDelegate);
	}

	private void OnEquipped(object data)
	{
		Equipment equipment = (Equipment)data;
		PrimaryElement component = equipment.GetComponent<MinionAssignablesProxy>().GetTargetGameObject().GetComponent<PrimaryElement>();
		if ((Object)component != (Object)null)
		{
			component.ModifyDiseaseCountHandler = OnModifyDiseaseCount;
			component.AddDiseaseHandler = OnAddDisease;
			component.ForcePermanentDiseaseContainer(true);
			component.SetDiseaseVisualProvider(base.gameObject);
		}
	}

	private void OnUnequipped(object data)
	{
		Equipment equipment = (Equipment)data;
		PrimaryElement component = equipment.GetComponent<MinionAssignablesProxy>().GetTargetGameObject().GetComponent<PrimaryElement>();
		if ((Object)component != (Object)null)
		{
			component.ModifyDiseaseCountHandler = null;
			component.AddDiseaseHandler = null;
			component.ForcePermanentDiseaseContainer(false);
			component.SetDiseaseVisualProvider(null);
		}
	}

	private void OnModifyDiseaseCount(int delta, string reason)
	{
		GetComponent<PrimaryElement>().ModifyDiseaseCount(delta, reason);
	}

	private void OnAddDisease(byte disease_idx, int delta, string reason)
	{
		GetComponent<PrimaryElement>().AddDisease(disease_idx, delta, reason);
	}
}
