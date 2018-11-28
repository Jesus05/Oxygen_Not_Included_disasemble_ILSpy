using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MinionEquipmentPanel : KMonoBehaviour
{
	public GameObject SelectedMinion;

	public GameObject labelTemplate;

	private GameObject roomPanel;

	private GameObject ownablePanel;

	private Storage storage;

	private Dictionary<string, GameObject> labels = new Dictionary<string, GameObject>();

	private Action<object> refreshDelegate;

	public MinionEquipmentPanel()
	{
		refreshDelegate = Refresh;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		roomPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		roomPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.PERSONALITY.EQUIPMENT.GROUPNAME_ROOMS;
		roomPanel.SetActive(true);
		ownablePanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		ownablePanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.PERSONALITY.EQUIPMENT.GROUPNAME_OWNABLE;
		ownablePanel.SetActive(true);
	}

	public void SetSelectedMinion(GameObject minion)
	{
		if ((UnityEngine.Object)SelectedMinion != (UnityEngine.Object)null)
		{
			SelectedMinion.Unsubscribe(-448952673, refreshDelegate);
			SelectedMinion.Unsubscribe(-1285462312, refreshDelegate);
			SelectedMinion.Unsubscribe(-1585839766, refreshDelegate);
		}
		SelectedMinion = minion;
		SelectedMinion.Subscribe(-448952673, refreshDelegate);
		SelectedMinion.Subscribe(-1285462312, refreshDelegate);
		SelectedMinion.Subscribe(-1585839766, refreshDelegate);
		Refresh(null);
	}

	public void Refresh(object data = null)
	{
		if (!((UnityEngine.Object)SelectedMinion == (UnityEngine.Object)null))
		{
			Build();
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if ((UnityEngine.Object)SelectedMinion != (UnityEngine.Object)null)
		{
			SelectedMinion.Unsubscribe(-448952673, refreshDelegate);
			SelectedMinion.Unsubscribe(-1285462312, refreshDelegate);
			SelectedMinion.Unsubscribe(-1585839766, refreshDelegate);
		}
	}

	private GameObject AddOrGetLabel(Dictionary<string, GameObject> labels, GameObject panel, string id)
	{
		GameObject gameObject = null;
		if (labels.ContainsKey(id))
		{
			gameObject = labels[id];
		}
		else
		{
			gameObject = Util.KInstantiate(labelTemplate, panel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject, null);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			labels[id] = gameObject;
		}
		gameObject.SetActive(true);
		return gameObject;
	}

	private void Build()
	{
		ShowAssignables(SelectedMinion.GetComponent<MinionIdentity>().GetSoleOwner(), roomPanel);
		ShowAssignables(SelectedMinion.GetComponent<MinionIdentity>().GetEquipment(), ownablePanel);
	}

	private void ShowAssignables(Assignables assignables, GameObject panel)
	{
		bool flag = false;
		foreach (AssignableSlotInstance slot in assignables.Slots)
		{
			if (slot.slot.showInUI)
			{
				GameObject gameObject = AddOrGetLabel(labels, panel, slot.slot.Name);
				if (slot.IsAssigned())
				{
					gameObject.SetActive(true);
					flag = true;
					string text = (!slot.IsAssigned()) ? UI.DETAILTABS.PERSONALITY.EQUIPMENT.NO_ASSIGNABLES.text : slot.assignable.GetComponent<KSelectable>().GetName();
					gameObject.GetComponent<LocText>().text = $"{slot.slot.Name}: {text}";
					gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.DETAILTABS.PERSONALITY.EQUIPMENT.ASSIGNED_TOOLTIP, text, GetAssignedEffectsString(slot), SelectedMinion.name);
				}
				else
				{
					gameObject.SetActive(false);
					gameObject.GetComponent<LocText>().text = UI.DETAILTABS.PERSONALITY.EQUIPMENT.NO_ASSIGNABLES;
					gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.PERSONALITY.EQUIPMENT.NO_ASSIGNABLES_TOOLTIP;
				}
			}
		}
		if (assignables is Ownables)
		{
			if (!flag)
			{
				GameObject gameObject2 = AddOrGetLabel(labels, panel, "NothingAssigned");
				labels["NothingAssigned"].SetActive(true);
				gameObject2.GetComponent<LocText>().text = UI.DETAILTABS.PERSONALITY.EQUIPMENT.NO_ASSIGNABLES;
				gameObject2.GetComponent<ToolTip>().toolTip = string.Format(UI.DETAILTABS.PERSONALITY.EQUIPMENT.NO_ASSIGNABLES_TOOLTIP, SelectedMinion.name);
			}
			else if (labels.ContainsKey("NothingAssigned"))
			{
				labels["NothingAssigned"].SetActive(false);
			}
		}
		if (assignables is Equipment)
		{
			if (!flag)
			{
				GameObject gameObject3 = AddOrGetLabel(labels, panel, "NoSuitAssigned");
				labels["NoSuitAssigned"].SetActive(true);
				gameObject3.GetComponent<LocText>().text = UI.DETAILTABS.PERSONALITY.EQUIPMENT.NOEQUIPMENT;
				gameObject3.GetComponent<ToolTip>().toolTip = string.Format(UI.DETAILTABS.PERSONALITY.EQUIPMENT.NOEQUIPMENT_TOOLTIP, SelectedMinion.name);
			}
			else if (labels.ContainsKey("NoSuitAssigned"))
			{
				labels["NoSuitAssigned"].SetActive(false);
			}
		}
	}

	private string GetAssignedEffectsString(AssignableSlotInstance slot)
	{
		string text = "";
		List<Descriptor> list = new List<Descriptor>();
		list.AddRange(GameUtil.GetGameObjectEffects(slot.assignable.gameObject, false));
		if (list.Count > 0)
		{
			text += "\n";
			foreach (Descriptor item in list)
			{
				text = text + "  • " + item.IndentedText() + "\n";
			}
		}
		return text;
	}
}
