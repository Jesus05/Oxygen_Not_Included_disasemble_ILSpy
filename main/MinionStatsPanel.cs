using Database;
using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class MinionStatsPanel : TargetScreen
{
	public GameObject attributesLabelTemplate;

	private GameObject resumePanel;

	private GameObject attributesPanel;

	private DetailsPanelDrawer resumeDrawer;

	private DetailsPanelDrawer attributesDrawer;

	private SchedulerHandle updateHandle;

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<MinionIdentity>();
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		resumePanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		attributesPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		resumeDrawer = new DetailsPanelDrawer(attributesLabelTemplate, resumePanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
		attributesDrawer = new DetailsPanelDrawer(attributesLabelTemplate, attributesPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
	}

	protected override void OnCleanUp()
	{
		updateHandle.ClearScheduler();
		base.OnCleanUp();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Refresh();
		ScheduleUpdate();
	}

	public override void OnSelectTarget(GameObject target)
	{
		base.OnSelectTarget(target);
		Refresh();
	}

	private void ScheduleUpdate()
	{
		updateHandle = UIScheduler.Instance.Schedule("RefreshMinionStatsPanel", 1f, delegate
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
			RefreshResume();
			RefreshAttributes();
		}
	}

	private void RefreshAttributes()
	{
		MinionIdentity component = selectedTarget.GetComponent<MinionIdentity>();
		if (!(bool)component)
		{
			attributesPanel.SetActive(false);
		}
		else
		{
			attributesPanel.SetActive(true);
			attributesPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.STATS.GROUPNAME_ATTRIBUTES;
			List<AttributeInstance> list = new List<AttributeInstance>(selectedTarget.GetAttributes().AttributeTable);
			List<AttributeInstance> list2 = list.FindAll((AttributeInstance a) => a.Attribute.ShowInUI == Attribute.Display.Skill);
			attributesDrawer.BeginDrawing();
			if (list2.Count > 0)
			{
				foreach (AttributeInstance item in list2)
				{
					attributesDrawer.NewLabel($"{item.Name}: {item.GetFormattedValue()}").Tooltip(item.GetAttributeValueTooltip());
				}
			}
			attributesDrawer.EndDrawing();
		}
	}

	private void RefreshResume()
	{
		MinionResume component = selectedTarget.GetComponent<MinionResume>();
		if (!(bool)component)
		{
			resumePanel.SetActive(false);
		}
		else
		{
			resumePanel.SetActive(true);
			resumePanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = string.Format(UI.DETAILTABS.PERSONALITY.GROUPNAME_RESUME, selectedTarget.name.ToUpper());
			resumeDrawer.BeginDrawing();
			List<Skill> list = new List<Skill>();
			foreach (KeyValuePair<string, bool> item2 in component.MasteryBySkillID)
			{
				if (item2.Value)
				{
					Skill item = Db.Get().Skills.Get(item2.Key);
					list.Add(item);
				}
			}
			resumeDrawer.NewLabel(UI.DETAILTABS.PERSONALITY.RESUME.MASTERED_SKILLS).Tooltip(UI.DETAILTABS.PERSONALITY.RESUME.MASTERED_SKILLS_TOOLTIP);
			if (list.Count == 0)
			{
				resumeDrawer.NewLabel("  • " + UI.DETAILTABS.PERSONALITY.RESUME.NO_MASTERED_SKILLS.NAME).Tooltip(string.Format(UI.DETAILTABS.PERSONALITY.RESUME.NO_MASTERED_SKILLS.TOOLTIP, selectedTarget.name));
			}
			else
			{
				foreach (Skill item3 in list)
				{
					string text = string.Empty;
					foreach (SkillPerk perk in item3.perks)
					{
						text = text + "  • " + perk.Name + "\n";
					}
					resumeDrawer.NewLabel("  • " + item3.Name).Tooltip(item3.description + "\n" + text);
				}
			}
			resumeDrawer.EndDrawing();
		}
	}
}
