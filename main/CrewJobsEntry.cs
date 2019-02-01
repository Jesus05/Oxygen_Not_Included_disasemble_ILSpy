using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrewJobsEntry : CrewListEntry
{
	[Serializable]
	public struct PriorityButton
	{
		public Button button;

		public GameObject ToggleIcon;

		public ChoreGroup choreGroup;

		public ToolTip tooltip;

		public Image border;

		public Image background;

		public Color baseBorderColor;

		public Color baseBackgroundColor;
	}

	public GameObject Prefab_JobPriorityButton;

	public GameObject Prefab_JobPriorityButtonAllTasks;

	private List<PriorityButton> PriorityButtons = new List<PriorityButton>();

	private PriorityButton AllTasksButton;

	public TextStyleSetting TooltipTextStyle_Title;

	public TextStyleSetting TooltipTextStyle_Ability;

	public TextStyleSetting TooltipTextStyle_AbilityPositiveModifier;

	public TextStyleSetting TooltipTextStyle_AbilityNegativeModifier;

	private bool dirty;

	private CrewJobsScreen.everyoneToggleState rowToggleState;

	public ChoreConsumer consumer
	{
		get;
		private set;
	}

	public override void Populate(MinionIdentity _identity)
	{
		base.Populate(_identity);
		this.consumer = _identity.GetComponent<ChoreConsumer>();
		ChoreConsumer consumer = this.consumer;
		consumer.choreRulesChanged = (System.Action)Delegate.Combine(consumer.choreRulesChanged, new System.Action(Dirty));
		foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
		{
			CreateChoreButton(resource);
		}
		CreateAllTaskButton();
		dirty = true;
	}

	private void CreateChoreButton(ChoreGroup chore_group)
	{
		GameObject gameObject = Util.KInstantiateUI(Prefab_JobPriorityButton, base.transform.gameObject, false);
		gameObject.GetComponent<OverviewColumnIdentity>().columnID = chore_group.Id;
		gameObject.GetComponent<OverviewColumnIdentity>().Column_DisplayName = chore_group.Name;
		PriorityButton priorityButton = default(PriorityButton);
		priorityButton.button = gameObject.GetComponent<Button>();
		priorityButton.border = gameObject.transform.GetChild(1).GetComponent<Image>();
		priorityButton.baseBorderColor = priorityButton.border.color;
		priorityButton.background = gameObject.transform.GetChild(0).GetComponent<Image>();
		priorityButton.baseBackgroundColor = priorityButton.background.color;
		priorityButton.choreGroup = chore_group;
		priorityButton.ToggleIcon = gameObject.transform.GetChild(2).gameObject;
		priorityButton.tooltip = gameObject.GetComponent<ToolTip>();
		priorityButton.tooltip.OnToolTip = (() => OnPriorityButtonTooltip(priorityButton));
		priorityButton.button.onClick.AddListener(delegate
		{
			OnPriorityPress(chore_group);
		});
		PriorityButtons.Add(priorityButton);
	}

	private void CreateAllTaskButton()
	{
		GameObject gameObject = Util.KInstantiateUI(Prefab_JobPriorityButtonAllTasks, base.transform.gameObject, false);
		gameObject.GetComponent<OverviewColumnIdentity>().columnID = "AllTasks";
		gameObject.GetComponent<OverviewColumnIdentity>().Column_DisplayName = "";
		Button b = gameObject.GetComponent<Button>();
		b.onClick.AddListener(delegate
		{
			ToggleTasksAll(b);
		});
		PriorityButton allTasksButton = default(PriorityButton);
		allTasksButton.button = gameObject.GetComponent<Button>();
		allTasksButton.border = gameObject.transform.GetChild(1).GetComponent<Image>();
		allTasksButton.baseBorderColor = allTasksButton.border.color;
		allTasksButton.background = gameObject.transform.GetChild(0).GetComponent<Image>();
		allTasksButton.baseBackgroundColor = allTasksButton.background.color;
		allTasksButton.ToggleIcon = gameObject.transform.GetChild(2).gameObject;
		allTasksButton.tooltip = gameObject.GetComponent<ToolTip>();
		AllTasksButton = allTasksButton;
	}

	private void ToggleTasksAll(Button button)
	{
		bool flag = rowToggleState != CrewJobsScreen.everyoneToggleState.on;
		string name = "HUD_Click_Deselect";
		if (flag)
		{
			name = "HUD_Click";
		}
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound(name, false));
		foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
		{
			consumer.SetPermittedByUser(resource, flag);
		}
	}

	private void OnPriorityPress(ChoreGroup chore_group)
	{
		bool flag = consumer.IsPermittedByUser(chore_group) ? true : false;
		string name = "HUD_Click";
		if (flag)
		{
			name = "HUD_Click_Deselect";
		}
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound(name, false));
		consumer.SetPermittedByUser(chore_group, !consumer.IsPermittedByUser(chore_group));
	}

	private void Refresh(object data = null)
	{
		if ((UnityEngine.Object)identity == (UnityEngine.Object)null)
		{
			dirty = false;
		}
		else if (dirty)
		{
			Attributes attributes = identity.GetAttributes();
			foreach (PriorityButton priorityButton in PriorityButtons)
			{
				PriorityButton current = priorityButton;
				bool flag = consumer.IsPermittedByUser(current.choreGroup);
				if (current.ToggleIcon.activeSelf != flag)
				{
					current.ToggleIcon.SetActive(flag);
				}
				float num = 0f;
				AttributeInstance attributeInstance = attributes.Get(current.choreGroup.attribute);
				num = Mathf.Min(attributeInstance.GetTotalValue() / 10f, 1f);
				Color baseBorderColor = current.baseBorderColor;
				baseBorderColor.r = Mathf.Lerp(current.baseBorderColor.r, 0.721568644f, num);
				baseBorderColor.g = Mathf.Lerp(current.baseBorderColor.g, 0.443137258f, num);
				baseBorderColor.b = Mathf.Lerp(current.baseBorderColor.b, 0.5803922f, num);
				if (current.border.color != baseBorderColor)
				{
					current.border.color = baseBorderColor;
				}
				Color color = current.baseBackgroundColor;
				color.a = Mathf.Lerp(0f, 1f, num);
				bool flag2 = consumer.IsPermittedByTraits(current.choreGroup);
				if (!flag2)
				{
					color = Color.clear;
					current.border.color = Color.clear;
					current.ToggleIcon.SetActive(false);
				}
				current.button.interactable = flag2;
				if (current.background.color != color)
				{
					current.background.color = color;
				}
			}
			int num2 = 0;
			int num3 = 0;
			foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
			{
				if (consumer.IsPermittedByTraits(resource))
				{
					num3++;
					if (consumer.IsPermittedByUser(resource))
					{
						num2++;
					}
				}
			}
			if (num2 == 0)
			{
				rowToggleState = CrewJobsScreen.everyoneToggleState.off;
			}
			else if (num2 < num3)
			{
				rowToggleState = CrewJobsScreen.everyoneToggleState.mixed;
			}
			else
			{
				rowToggleState = CrewJobsScreen.everyoneToggleState.on;
			}
			ImageToggleState component = AllTasksButton.ToggleIcon.GetComponent<ImageToggleState>();
			switch (rowToggleState)
			{
			case CrewJobsScreen.everyoneToggleState.mixed:
				component.SetInactive();
				break;
			case CrewJobsScreen.everyoneToggleState.on:
				component.SetActive();
				break;
			case CrewJobsScreen.everyoneToggleState.off:
				component.SetDisabled();
				break;
			}
			dirty = false;
		}
	}

	private string OnPriorityButtonTooltip(PriorityButton b)
	{
		b.tooltip.ClearMultiStringTooltip();
		if ((UnityEngine.Object)identity != (UnityEngine.Object)null)
		{
			Attributes attributes = identity.GetAttributes();
			if (attributes != null)
			{
				if (!consumer.IsPermittedByTraits(b.choreGroup))
				{
					string newString = string.Format(UI.TOOLTIPS.JOBSSCREEN_CANNOTPERFORMTASK, consumer.GetComponent<MinionIdentity>().GetProperName());
					b.tooltip.AddMultiStringTooltip(newString, TooltipTextStyle_AbilityNegativeModifier);
					return "";
				}
				b.tooltip.AddMultiStringTooltip(UI.TOOLTIPS.JOBSSCREEN_RELEVANT_ATTRIBUTES, TooltipTextStyle_Ability);
				Klei.AI.Attribute attribute = b.choreGroup.attribute;
				AttributeInstance attributeInstance = attributes.Get(attribute);
				float totalValue = attributeInstance.GetTotalValue();
				TextStyleSetting styleSetting = TooltipTextStyle_Ability;
				if (totalValue > 0f)
				{
					styleSetting = TooltipTextStyle_AbilityPositiveModifier;
				}
				else if (totalValue < 0f)
				{
					styleSetting = TooltipTextStyle_AbilityNegativeModifier;
				}
				b.tooltip.AddMultiStringTooltip(attribute.Name + " " + attributeInstance.GetTotalValue(), styleSetting);
			}
		}
		return "";
	}

	private void LateUpdate()
	{
		Refresh(null);
	}

	private void OnLevelUp(object data)
	{
		Dirty();
	}

	private void Dirty()
	{
		dirty = true;
		CrewJobsScreen.Instance.Dirty(null);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if ((UnityEngine.Object)this.consumer != (UnityEngine.Object)null)
		{
			ChoreConsumer consumer = this.consumer;
			consumer.choreRulesChanged = (System.Action)Delegate.Remove(consumer.choreRulesChanged, new System.Action(Dirty));
		}
	}
}
