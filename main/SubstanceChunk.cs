using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SkipSaveFileSerialization]
[SerializationConfig(MemberSerialization.OptIn)]
public class SubstanceChunk : KMonoBehaviour, ISaveLoadable
{
	private static readonly KAnimHashedString symbolToTint = new KAnimHashedString("substance_tinter");

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Color color = GetComponent<PrimaryElement>().Element.substance.colour;
		color.a = 1f;
		GetComponent<KBatchedAnimController>().SetSymbolTint(symbolToTint, color);
	}

	private void OnRefreshUserMenu(object data)
	{
		UserMenu userMenu = Game.Instance.userMenu;
		GameObject gameObject = base.gameObject;
		string iconName = "action_deconstruct";
		string text = UI.USERMENUACTIONS.RELEASEELEMENT.NAME;
		System.Action on_click = OnRelease;
		string tooltipText = UI.USERMENUACTIONS.RELEASEELEMENT.TOOLTIP;
		userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true), 1f);
	}

	private void OnRelease()
	{
		int gameCell = Grid.PosToCell(base.transform.GetPosition());
		PrimaryElement component = GetComponent<PrimaryElement>();
		if (component.Mass > 0f)
		{
			SimMessages.AddRemoveSubstance(gameCell, component.ElementID, CellEventLogger.Instance.ExhaustSimUpdate, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount, true, -1);
		}
		base.gameObject.DeleteObject();
	}
}
