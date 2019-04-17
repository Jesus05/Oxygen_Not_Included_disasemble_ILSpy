using Database;
using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class MinionPersonalityPanel : TargetScreen
{
	public GameObject attributesLabelTemplate;

	private GameObject bioPanel;

	private GameObject traitsPanel;

	private DetailsPanelDrawer bioDrawer;

	private DetailsPanelDrawer traitsDrawer;

	public MinionEquipmentPanel panel;

	private SchedulerHandle updateHandle;

	public override bool IsValidForTarget(GameObject target)
	{
		return (Object)target.GetComponent<MinionIdentity>() != (Object)null;
	}

	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
	}

	public override void OnSelectTarget(GameObject target)
	{
		panel.SetSelectedMinion(target);
		panel.Refresh(null);
		base.OnSelectTarget(target);
		Refresh();
	}

	public override void OnDeselectTarget(GameObject target)
	{
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		if ((Object)panel == (Object)null)
		{
			panel = GetComponent<MinionEquipmentPanel>();
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		bioPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		traitsPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		bioDrawer = new DetailsPanelDrawer(attributesLabelTemplate, bioPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
		traitsDrawer = new DetailsPanelDrawer(attributesLabelTemplate, traitsPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
	}

	protected override void OnCleanUp()
	{
		updateHandle.ClearScheduler();
		base.OnCleanUp();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if ((Object)panel == (Object)null)
		{
			panel = GetComponent<MinionEquipmentPanel>();
		}
		Refresh();
		ScheduleUpdate();
	}

	private void ScheduleUpdate()
	{
		updateHandle = UIScheduler.Instance.Schedule("RefreshMinionPersonalityPanel", 1f, delegate
		{
			Refresh();
			ScheduleUpdate();
		}, null, null);
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
			gameObject = Util.KInstantiate(attributesLabelTemplate, panel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject, null);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			labels[id] = gameObject;
		}
		gameObject.SetActive(true);
		return gameObject;
	}

	private void Refresh()
	{
		if (base.gameObject.activeSelf && !((Object)selectedTarget == (Object)null) && !((Object)selectedTarget.GetComponent<MinionIdentity>() == (Object)null))
		{
			RefreshBio();
			RefreshTraits();
		}
	}

	private void RefreshBio()
	{
		MinionIdentity component = selectedTarget.GetComponent<MinionIdentity>();
		if (!(bool)component)
		{
			bioPanel.SetActive(false);
		}
		else
		{
			bioPanel.SetActive(true);
			bioPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.PERSONALITY.GROUPNAME_BIO;
			bioDrawer.BeginDrawing().NewLabel(DUPLICANTS.NAMETITLE + component.name).NewLabel((string)DUPLICANTS.ARRIVALTIME + ((float)GameClock.Instance.GetCycle() - component.arrivalTime) + " Cycles")
				.Tooltip(string.Format(DUPLICANTS.ARRIVALTIME_TOOLTIP, component.arrivalTime, component.name))
				.NewLabel(DUPLICANTS.GENDERTITLE + string.Format(Strings.Get($"STRINGS.DUPLICANTS.GENDER.{component.genderStringKey.ToUpper()}.NAME"), component.gender))
				.NewLabel(string.Format(Strings.Get($"STRINGS.DUPLICANTS.PERSONALITIES.{component.nameStringKey.ToUpper()}.DESC"), component.name))
				.Tooltip(string.Format(Strings.Get(string.Format("STRINGS.DUPLICANTS.DESC_TOOLTIP", component.nameStringKey.ToUpper())), component.name));
			MinionResume component2 = selectedTarget.GetComponent<MinionResume>();
			if ((Object)component2 != (Object)null && component2.AptitudeBySkillGroup.Count > 0)
			{
				bioDrawer.NewLabel(UI.DETAILTABS.PERSONALITY.RESUME.APTITUDES.NAME + "\n").Tooltip(string.Format(UI.DETAILTABS.PERSONALITY.RESUME.APTITUDES.TOOLTIP, selectedTarget.name));
				foreach (KeyValuePair<HashedString, float> item in component2.AptitudeBySkillGroup)
				{
					if (item.Value != 0f)
					{
						SkillGroup skillGroup = Db.Get().SkillGroups.Get(item.Key);
						bioDrawer.NewLabel("  • " + skillGroup.Name).Tooltip(string.Format(DUPLICANTS.ROLES.GROUPS.APTITUDE_DESCRIPTION, skillGroup.Name, item.Value * ROLES.APTITUDE_EXPERIENCE_SCALE));
					}
				}
			}
			bioDrawer.EndDrawing();
		}
	}

	private void RefreshTraits()
	{
		MinionIdentity component = selectedTarget.GetComponent<MinionIdentity>();
		if (!(bool)component)
		{
			traitsPanel.SetActive(false);
		}
		else
		{
			traitsPanel.SetActive(true);
			traitsPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.STATS.GROUPNAME_TRAITS;
			traitsDrawer.BeginDrawing();
			foreach (Trait trait in selectedTarget.GetComponent<Traits>().TraitList)
			{
				traitsDrawer.NewLabel(trait.Name).Tooltip(trait.GetTooltip());
			}
			traitsDrawer.EndDrawing();
		}
	}
}
