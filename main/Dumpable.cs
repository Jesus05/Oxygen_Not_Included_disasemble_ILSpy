using KSerialization;
using STRINGS;
using System;

public class Dumpable : Workable
{
	private Chore chore;

	[Serialize]
	private bool isMarkedForDumping;

	private static readonly EventSystem.IntraObjectHandler<Dumpable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Dumpable>(delegate(Dumpable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		workerStatusItem = Db.Get().DuplicantStatusItems.Emptying;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (isMarkedForDumping)
		{
			chore = new WorkChore<Dumpable>(Db.Get().ChoreTypes.EmptyStorage, this, null, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
		}
		SetWorkTime(0.1f);
	}

	public void ToggleDumping()
	{
		if (DebugHandler.InstantBuildMode)
		{
			OnCompleteWork(null);
		}
		else if (isMarkedForDumping)
		{
			isMarkedForDumping = false;
			chore.Cancel("Cancel Dumping!");
			chore = null;
			ShowProgressBar(false);
		}
		else
		{
			isMarkedForDumping = true;
			chore = new WorkChore<Dumpable>(Db.Get().ChoreTypes.EmptyStorage, this, null, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		isMarkedForDumping = false;
		chore = null;
		Dump();
	}

	public void Dump()
	{
		PrimaryElement component = GetComponent<PrimaryElement>();
		if (component.Mass > 0f)
		{
			SimMessages.AddRemoveSubstance(Grid.PosToCell(this), component.ElementID, CellEventLogger.Instance.Dumpable, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount, true, -1);
		}
		Util.KDestroyGameObject(base.gameObject);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (!this.HasTag(GameTags.Stored))
		{
			object buttonInfo;
			if (isMarkedForDumping)
			{
				string iconName = "action_empty_contents";
				string text = UI.USERMENUACTIONS.DUMP.NAME_OFF;
				System.Action on_click = ToggleDumping;
				string tooltipText = UI.USERMENUACTIONS.DUMP.TOOLTIP_OFF;
				buttonInfo = new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true);
			}
			else
			{
				string tooltipText = "action_empty_contents";
				string text = UI.USERMENUACTIONS.DUMP.NAME;
				System.Action on_click = ToggleDumping;
				string iconName = UI.USERMENUACTIONS.DUMP.TOOLTIP;
				buttonInfo = new KIconButtonMenu.ButtonInfo(tooltipText, text, on_click, Action.NumActions, null, null, null, iconName, true);
			}
			KIconButtonMenu.ButtonInfo button = (KIconButtonMenu.ButtonInfo)buttonInfo;
			Game.Instance.userMenu.AddButton(base.gameObject, button, 1f);
		}
	}
}
