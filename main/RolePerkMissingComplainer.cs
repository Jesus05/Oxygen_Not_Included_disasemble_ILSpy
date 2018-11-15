using System;
using UnityEngine;

public class RolePerkMissingComplainer : KMonoBehaviour
{
	public HashedString requiredRolePerk;

	private int roleUpdateHandle = -1;

	private Guid workStatusItemHandle;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (requiredRolePerk.IsValid)
		{
			roleUpdateHandle = Game.Instance.Subscribe(-1523247426, UpdateStatusItem);
		}
		UpdateStatusItem(null);
	}

	protected override void OnCleanUp()
	{
		if (roleUpdateHandle != -1)
		{
			Game.Instance.Unsubscribe(roleUpdateHandle);
		}
		base.OnCleanUp();
	}

	protected virtual void UpdateStatusItem(object data = null)
	{
		KSelectable component = GetComponent<KSelectable>();
		if (!((UnityEngine.Object)component == (UnityEngine.Object)null) && requiredRolePerk.IsValid)
		{
			bool flag = Game.Instance.roleManager.GetRoleAssigneesWithPerk(requiredRolePerk).Count > 0;
			if (!flag && workStatusItemHandle == Guid.Empty)
			{
				workStatusItemHandle = component.AddStatusItem(Db.Get().BuildingStatusItems.ColonyLacksRequiredRolePerk, requiredRolePerk);
			}
			else if (flag && workStatusItemHandle != Guid.Empty)
			{
				component.RemoveStatusItem(workStatusItemHandle, false);
				workStatusItemHandle = Guid.Empty;
			}
		}
	}
}
