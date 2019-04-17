using FMOD.Studio;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PriorityScreen : KScreen
{
	public enum PriorityClass
	{
		idle = -1,
		basic,
		high,
		personalNeeds,
		topPriority,
		compulsory
	}

	[SerializeField]
	protected PriorityButton buttonPrefab_basic;

	[SerializeField]
	protected GameObject EmergencyContainer;

	[SerializeField]
	protected PriorityButton button_emergency;

	[SerializeField]
	protected GameObject PriorityMenuContainer;

	[SerializeField]
	protected KButton button_priorityMenu;

	[SerializeField]
	protected KToggle button_toggleHigh;

	[SerializeField]
	protected GameObject diagram;

	protected List<PriorityButton> buttons_basic = new List<PriorityButton>();

	protected List<PriorityButton> buttons_emergency = new List<PriorityButton>();

	private PrioritySetting priority;

	private PrioritySetting lastSelectedPriority = new PrioritySetting(PriorityClass.basic, -1);

	private Action<PrioritySetting> onClick;

	public void InstantiateButtons(Action<PrioritySetting> on_click, bool playSelectionSound = true)
	{
		onClick = on_click;
		for (int i = 1; i <= 9; i++)
		{
			int num = i;
			PriorityButton priorityButton = Util.KInstantiateUI<PriorityButton>(buttonPrefab_basic.gameObject, buttonPrefab_basic.transform.parent.gameObject, false);
			buttons_basic.Add(priorityButton);
			priorityButton.playSelectionSound = playSelectionSound;
			priorityButton.onClick = onClick;
			priorityButton.text.text = num.ToString();
			priorityButton.priority = new PrioritySetting(PriorityClass.basic, num);
			priorityButton.tooltip.SetSimpleTooltip(string.Format(UI.PRIORITYSCREEN.BASIC, num));
		}
		buttonPrefab_basic.gameObject.SetActive(false);
		button_emergency.playSelectionSound = playSelectionSound;
		button_emergency.onClick = onClick;
		button_emergency.priority = new PrioritySetting(PriorityClass.topPriority, 1);
		button_emergency.tooltip.SetSimpleTooltip(UI.PRIORITYSCREEN.TOP_PRIORITY);
		button_toggleHigh.gameObject.SetActive(false);
		PriorityMenuContainer.SetActive(true);
		button_priorityMenu.gameObject.SetActive(true);
		button_priorityMenu.onClick += PriorityButtonClicked;
		button_priorityMenu.GetComponent<ToolTip>().SetSimpleTooltip(UI.PRIORITYSCREEN.OPEN_JOBS_SCREEN);
		diagram.SetActive(false);
		SetScreenPriority(new PrioritySetting(PriorityClass.basic, 5), false);
	}

	private void OnClick(PrioritySetting priority)
	{
		if (onClick != null)
		{
			onClick(priority);
		}
	}

	public void ShowDiagram(bool show)
	{
		diagram.SetActive(show);
	}

	public void ResetPriority()
	{
		SetScreenPriority(new PrioritySetting(PriorityClass.basic, 5), false);
	}

	public void PriorityButtonClicked()
	{
		ManagementMenu.Instance.TogglePriorities();
	}

	private void RefreshButton(PriorityButton b, PrioritySetting priority, bool play_sound)
	{
		if (b.priority == priority)
		{
			b.toggle.Select();
			b.toggle.isOn = true;
			if (play_sound)
			{
				b.toggle.soundPlayer.Play(0);
			}
		}
		else
		{
			b.toggle.isOn = false;
		}
	}

	public void SetScreenPriority(PrioritySetting priority, bool play_sound = false)
	{
		if (!(lastSelectedPriority == priority))
		{
			lastSelectedPriority = priority;
			if (priority.priority_class == PriorityClass.high)
			{
				button_toggleHigh.isOn = true;
			}
			else if (priority.priority_class == PriorityClass.basic)
			{
				button_toggleHigh.isOn = false;
			}
			for (int i = 0; i < buttons_basic.Count; i++)
			{
				buttons_basic[i].priority = new PrioritySetting(button_toggleHigh.isOn ? PriorityClass.high : PriorityClass.basic, i + 1);
				buttons_basic[i].tooltip.SetSimpleTooltip(string.Format((!button_toggleHigh.isOn) ? UI.PRIORITYSCREEN.BASIC : UI.PRIORITYSCREEN.HIGH, i + 1));
				RefreshButton(buttons_basic[i], lastSelectedPriority, play_sound);
			}
			RefreshButton(button_emergency, lastSelectedPriority, play_sound);
		}
	}

	public PrioritySetting GetLastSelectedPriority()
	{
		return lastSelectedPriority;
	}

	public static void PlayPriorityConfirmSound(PrioritySetting priority)
	{
		EventInstance instance = KFMOD.BeginOneShot(GlobalAssets.GetSound("Priority_Tool_Confirm", false), Vector3.zero);
		if (instance.isValid())
		{
			float num = 0f;
			if (priority.priority_class >= PriorityClass.high)
			{
				num += 10f;
			}
			if (priority.priority_class >= PriorityClass.topPriority)
			{
				num = num;
			}
			num += (float)priority.priority_value;
			instance.setParameterValue("priority", num);
			KFMOD.EndOneShot(instance);
		}
	}
}
