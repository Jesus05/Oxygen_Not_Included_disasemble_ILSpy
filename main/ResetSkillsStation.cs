using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class ResetSkillsStation : Workable
{
	[MyCmpReq]
	public Assignable assignable;

	private Notification notification;

	private Chore chore;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		lightEfficiencyBonus = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		OnAssign(assignable.assignee);
		assignable.OnAssign += OnAssign;
	}

	private void OnAssign(IAssignableIdentity obj)
	{
		if (obj != null)
		{
			CreateChore();
		}
		else if (chore != null)
		{
			chore.Cancel("Unassigned");
			chore = null;
		}
	}

	private void CreateChore()
	{
		chore = new WorkChore<ResetSkillsStation>(Db.Get().ChoreTypes.Train, this, null, true, null, null, null, false, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		GetComponent<Operational>().SetActive(true, false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		assignable.Unassign();
		MinionResume component = worker.GetComponent<MinionResume>();
		if ((Object)component != (Object)null)
		{
			component.ResetSkillLevels(true);
			component.SetHats(component.CurrentHat, null);
			component.ApplyTargetHat();
			notification = new Notification(MISC.NOTIFICATIONS.RESETSKILL.NAME, NotificationType.Good, HashedString.Invalid, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.RESETSKILL.TOOLTIP + notificationList.ReduceMessages(false), null, true, 0f, null, null, null);
			worker.GetComponent<Notifier>().Add(notification, "");
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		GetComponent<Operational>().SetActive(false, false);
		chore = null;
	}
}
