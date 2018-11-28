using STRINGS;

public class SuitEquipper : KMonoBehaviour
{
	private static readonly EventSystem.IntraObjectHandler<SuitEquipper> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<SuitEquipper>(delegate(SuitEquipper component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(493375141, OnRefreshUserMenuDelegate);
	}

	private void OnRefreshUserMenu(object data)
	{
		MinionIdentity component = GetComponent<MinionIdentity>();
		Equipment equipment = component.GetEquipment();
		foreach (EquipmentSlotInstance slot in equipment.Slots)
		{
			Equippable equippable = slot.assignable as Equippable;
			if ((bool)equippable)
			{
				string text = string.Format(UI.USERMENUACTIONS.UNEQUIP.NAME, equippable.def.GenericName);
				Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("iconDown", text, delegate
				{
					equippable.Unassign();
				}, Action.NumActions, null, null, null, "", true), 2f);
			}
		}
	}
}
