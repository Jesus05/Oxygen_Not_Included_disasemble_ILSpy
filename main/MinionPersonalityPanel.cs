using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class MinionPersonalityPanel : TargetScreen
{
	public GameObject attributesLabelTemplate;

	private GameObject bioPanel;

	private GameObject resumePanel;

	private DetailsPanelDrawer bioDrawer;

	private DetailsPanelDrawer resumeDrawer;

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
		resumePanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		bioDrawer = new DetailsPanelDrawer(attributesLabelTemplate, bioPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
		resumeDrawer = new DetailsPanelDrawer(attributesLabelTemplate, resumePanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
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
			RefreshResume();
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
				.Tooltip(string.Format(Strings.Get(string.Format("STRINGS.DUPLICANTS.DESC_TOOLTIP", component.nameStringKey.ToUpper())), component.name))
				.EndDrawing();
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
			RoleConfig role = Game.Instance.roleManager.GetRole(component.CurrentRole);
			if (role.id == "NoRole")
			{
				resumeDrawer.NewLabel(string.Format(UI.DETAILTABS.PERSONALITY.RESUME.CURRENT_ROLE.NAME, role.name) + "\n").Tooltip(string.Format(UI.DETAILTABS.PERSONALITY.RESUME.CURRENT_ROLE.NOJOB_TOOLTIP, selectedTarget.name, role.name));
			}
			else
			{
				resumeDrawer.NewLabel(string.Format(UI.DETAILTABS.PERSONALITY.RESUME.CURRENT_ROLE.NAME, role.name) + "\n").Tooltip(string.Format(UI.DETAILTABS.PERSONALITY.RESUME.CURRENT_ROLE.TOOLTIP, selectedTarget.name, role.name));
			}
			int num = 0;
			if (num != 0)
			{
				resumeDrawer.NewLabel(UI.DETAILTABS.PERSONALITY.RESUME.MASTERED_ROLES).Tooltip(UI.DETAILTABS.PERSONALITY.RESUME.MASTERED_ROLES_TOOLTIP);
			}
			foreach (KeyValuePair<string, bool> item in component.MasteryByRoleID)
			{
				if (item.Value && !(item.Key == "NoRole"))
				{
					role = Game.Instance.roleManager.GetRole(item.Key);
					resumeDrawer.NewLabel(role.name).Tooltip(Game.Instance.roleManager.RoleTooltip(role.id));
					num++;
				}
			}
			int num2 = 0;
			if (num2 != 0)
			{
				resumeDrawer.NewLabel(UI.DETAILTABS.PERSONALITY.RESUME.PERKS.NAME + "\n").Tooltip(string.Format(UI.DETAILTABS.PERSONALITY.RESUME.PERKS.TOOLTIP, selectedTarget.name));
			}
			foreach (KeyValuePair<HashedString, RoleGroup> roleGroup in Game.Instance.roleManager.RoleGroups)
			{
				foreach (RoleConfig role2 in roleGroup.Value.roles)
				{
					if (role2.id == component.CurrentRole || component.MasteryByRoleID[role2.id])
					{
						RolePerk[] perks = role2.perks;
						foreach (RolePerk rolePerk in perks)
						{
							resumeDrawer.NewLabel("  • " + rolePerk.description).Tooltip(string.Format(UI.DETAILTABS.PERSONALITY.RESUME.JOBTRAINING_TOOLTIP, selectedTarget.name, role2.GetProperName()));
							num2++;
						}
					}
				}
			}
			resumeDrawer.NewLabel(UI.DETAILTABS.PERSONALITY.RESUME.APTITUDES.NAME + "\n").Tooltip(string.Format(UI.DETAILTABS.PERSONALITY.RESUME.APTITUDES.TOOLTIP, selectedTarget.name));
			if (component.AptitudeByRoleGroup.Count > 0)
			{
				foreach (KeyValuePair<HashedString, float> item2 in component.AptitudeByRoleGroup)
				{
					if (item2.Value != 0f)
					{
						resumeDrawer.NewLabel("  • " + Game.Instance.roleManager.RoleGroups[item2.Key].Name).Tooltip(string.Format(DUPLICANTS.ROLES.GROUPS.APTITUDE_DESCRIPTION, Game.Instance.roleManager.RoleGroups[item2.Key].Name, item2.Value * ROLES.APTITUDE_EXPERIENCE_SCALE));
					}
				}
			}
			resumeDrawer.EndDrawing();
		}
	}
}
