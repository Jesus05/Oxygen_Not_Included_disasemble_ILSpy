using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Automatable : KMonoBehaviour
{
	[Serialize]
	private bool automationOnly = true;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<Automatable> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<Automatable>(delegate(Automatable component, object data)
	{
		component.OnCopySettings(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		Automatable component = gameObject.GetComponent<Automatable>();
		if ((Object)component != (Object)null)
		{
			automationOnly = component.automationOnly;
		}
	}

	public bool GetAutomationOnly()
	{
		return automationOnly;
	}

	public void SetAutomationOnly(bool only)
	{
		automationOnly = only;
	}

	public bool AllowedByAutomation(bool is_transfer_arm)
	{
		return !GetAutomationOnly() || is_transfer_arm;
	}
}
