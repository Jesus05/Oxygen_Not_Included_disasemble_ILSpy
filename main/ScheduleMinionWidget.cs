using Klei.AI;
using STRINGS;
using System.Linq;
using UnityEngine;

public class ScheduleMinionWidget : KMonoBehaviour
{
	[SerializeField]
	private CrewPortrait portrait;

	[SerializeField]
	private DropDown dropDown;

	[SerializeField]
	private LocText label;

	[SerializeField]
	private GameObject nightOwlIcon;

	[SerializeField]
	private GameObject earlyBirdIcon;

	public Schedulable schedulable
	{
		get;
		private set;
	}

	public void ChangeAssignment(Schedule targetSchedule, Schedulable schedulable)
	{
		DebugUtil.LogArgs("Assigning", schedulable, "from", ScheduleManager.Instance.GetSchedule(schedulable).name, "to", targetSchedule.name);
		ScheduleManager.Instance.GetSchedule(schedulable).Unassign(schedulable);
		targetSchedule.Assign(schedulable);
	}

	public void Setup(Schedulable schedulable)
	{
		this.schedulable = schedulable;
		IAssignableIdentity component = schedulable.GetComponent<IAssignableIdentity>();
		portrait.SetIdentityObject(component, true);
		label.text = component.GetProperName();
		MinionIdentity minionIdentity = (MinionIdentity)component;
		Traits component2 = minionIdentity.GetComponent<Traits>();
		if (component2.HasTrait("NightOwl"))
		{
			nightOwlIcon.SetActive(true);
		}
		else if (component2.HasTrait("EarlyBird"))
		{
			earlyBirdIcon.SetActive(true);
		}
		dropDown.Initialize(ScheduleManager.Instance.GetSchedules().Cast<IListableOption>(), OnDropEntryClick, null, DropEntryRefreshAction, false, schedulable);
	}

	private void OnDropEntryClick(IListableOption option, object obj)
	{
		Schedule targetSchedule = (Schedule)option;
		ChangeAssignment(targetSchedule, schedulable);
	}

	private void DropEntryRefreshAction(DropDownEntry entry, object obj)
	{
		Schedule schedule = (Schedule)entry.entryData;
		Schedulable schedulable = (Schedulable)obj;
		if (schedulable.GetSchedule() == schedule)
		{
			entry.label.text = string.Format(UI.SCHEDULESCREEN.SCHEDULE_DROPDOWN_ASSIGNED, schedule.name);
			entry.button.isInteractable = false;
		}
		else
		{
			entry.label.text = schedule.name;
			entry.button.isInteractable = true;
		}
	}

	public void SetupBlank(Schedule schedule)
	{
		label.text = UI.SCHEDULESCREEN.SCHEDULE_DROPDOWN_BLANK;
		dropDown.Initialize(Components.LiveMinionIdentities.Items.Cast<IListableOption>(), OnBlankDropEntryClick, BlankDropEntrySort, BlankDropEntryRefreshAction, false, schedule);
		Components.LiveMinionIdentities.OnAdd += OnLivingMinionsChanged;
		Components.LiveMinionIdentities.OnRemove += OnLivingMinionsChanged;
	}

	private void OnLivingMinionsChanged(MinionIdentity minion)
	{
		dropDown.ChangeContent(Components.LiveMinionIdentities.Items.Cast<IListableOption>());
	}

	private void OnBlankDropEntryClick(IListableOption option, object obj)
	{
		Schedule targetSchedule = (Schedule)obj;
		MinionIdentity minionIdentity = (MinionIdentity)option;
		ChangeAssignment(targetSchedule, minionIdentity.GetComponent<Schedulable>());
	}

	private void BlankDropEntryRefreshAction(DropDownEntry entry, object obj)
	{
		Schedule schedule = (Schedule)obj;
		MinionIdentity minionIdentity = (MinionIdentity)entry.entryData;
		if (schedule.IsAssigned(minionIdentity.GetComponent<Schedulable>()))
		{
			entry.label.text = string.Format(UI.SCHEDULESCREEN.SCHEDULE_DROPDOWN_ASSIGNED, minionIdentity.GetProperName());
			entry.button.isInteractable = false;
		}
		else
		{
			entry.label.text = minionIdentity.GetProperName();
			entry.button.isInteractable = true;
		}
		Traits component = minionIdentity.GetComponent<Traits>();
		entry.gameObject.GetComponent<HierarchyReferences>().GetReference<RectTransform>("NightOwlIcon").gameObject.SetActive(component.HasTrait("NightOwl"));
		entry.gameObject.GetComponent<HierarchyReferences>().GetReference<RectTransform>("EarlyBirdIcon").gameObject.SetActive(component.HasTrait("EarlyBird"));
	}

	private int BlankDropEntrySort(IListableOption a, IListableOption b, object obj)
	{
		Schedule schedule = (Schedule)obj;
		MinionIdentity minionIdentity = (MinionIdentity)a;
		MinionIdentity minionIdentity2 = (MinionIdentity)b;
		bool flag = schedule.IsAssigned(minionIdentity.GetComponent<Schedulable>());
		bool flag2 = schedule.IsAssigned(minionIdentity2.GetComponent<Schedulable>());
		if (flag && !flag2)
		{
			return -1;
		}
		if (!flag && flag2)
		{
			return 1;
		}
		return 0;
	}

	protected override void OnCleanUp()
	{
		Components.LiveMinionIdentities.OnAdd -= OnLivingMinionsChanged;
		Components.LiveMinionIdentities.OnRemove -= OnLivingMinionsChanged;
	}
}
