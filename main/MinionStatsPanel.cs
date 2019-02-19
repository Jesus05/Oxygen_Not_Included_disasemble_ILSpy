using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class MinionStatsPanel : TargetScreen
{
	public GameObject attributesLabelTemplate;

	private GameObject attributesPanel;

	private GameObject stressPanel;

	private GameObject traitsPanel;

	private DetailsPanelDrawer attributesDrawer;

	private DetailsPanelDrawer stressDrawer;

	private DetailsPanelDrawer traitsDrawer;

	private SchedulerHandle updateHandle;

	private List<ReportManager.ReportEntry.Note> stressNotes = new List<ReportManager.ReportEntry.Note>();

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<MinionIdentity>();
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		stressPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		attributesPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		traitsPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		attributesDrawer = new DetailsPanelDrawer(attributesLabelTemplate, attributesPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
		stressDrawer = new DetailsPanelDrawer(attributesLabelTemplate, stressPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
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
			RefreshAttributes();
			RefreshTraits();
			RefreshStress();
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

	private void RefreshStress()
	{
		MinionIdentity identity = selectedTarget.GetComponent<MinionIdentity>();
		if (!(bool)identity)
		{
			stressPanel.SetActive(false);
		}
		else
		{
			stressPanel.SetActive(true);
			stressPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.STATS.GROUPNAME_STRESS;
			ReportManager.ReportEntry reportEntry = ReportManager.Instance.TodaysReport.reportEntries.Find((ReportManager.ReportEntry entry) => entry.reportType == ReportManager.ReportType.StressDelta);
			stressDrawer.BeginDrawing();
			float num = 0f;
			stressNotes.Clear();
			int num2 = reportEntry.contextEntries.FindIndex((ReportManager.ReportEntry entry) => entry.context == identity.GetProperName());
			ReportManager.ReportEntry reportEntry2 = (num2 == -1) ? null : reportEntry.contextEntries[num2];
			if (reportEntry2 != null)
			{
				reportEntry2.IterateNotes(delegate(ReportManager.ReportEntry.Note note)
				{
					stressNotes.Add(note);
				});
				stressNotes.Sort((ReportManager.ReportEntry.Note a, ReportManager.ReportEntry.Note b) => a.value.CompareTo(b.value));
				for (int i = 0; i < stressNotes.Count; i++)
				{
					DetailsPanelDrawer detailsPanelDrawer = stressDrawer;
					string[] obj = new string[6];
					ReportManager.ReportEntry.Note note2 = stressNotes[i];
					obj[0] = ((!(note2.value > 0f)) ? string.Empty : UIConstants.ColorPrefixRed);
					ReportManager.ReportEntry.Note note3 = stressNotes[i];
					obj[1] = note3.note;
					obj[2] = ": ";
					ReportManager.ReportEntry.Note note4 = stressNotes[i];
					obj[3] = Util.FormatTwoDecimalPlace(note4.value);
					obj[4] = "%";
					ReportManager.ReportEntry.Note note5 = stressNotes[i];
					obj[5] = ((!(note5.value > 0f)) ? string.Empty : UIConstants.ColorSuffix);
					detailsPanelDrawer.NewLabel(string.Concat(obj));
					float num3 = num;
					ReportManager.ReportEntry.Note note6 = stressNotes[i];
					num = num3 + note6.value;
				}
			}
			stressDrawer.NewLabel(((!(num > 0f)) ? string.Empty : UIConstants.ColorPrefixRed) + string.Format(UI.DETAILTABS.DETAILS.NET_STRESS, Util.FormatTwoDecimalPlace(num)) + ((!(num > 0f)) ? string.Empty : UIConstants.ColorSuffix));
			stressDrawer.EndDrawing();
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
