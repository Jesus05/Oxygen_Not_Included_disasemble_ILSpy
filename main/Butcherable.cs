using STRINGS;
using UnityEngine;

public class Butcherable : Workable, ISaveLoadable
{
	[MyCmpGet]
	private KAnimControllerBase controller;

	[MyCmpGet]
	private Harvestable harvestable;

	private bool readyToButcher;

	private bool butchered;

	public string[] Drops;

	private Chore chore;

	private static readonly EventSystem.IntraObjectHandler<Butcherable> SetReadyToButcherDelegate = new EventSystem.IntraObjectHandler<Butcherable>(delegate(Butcherable component, object data)
	{
		component.SetReadyToButcher(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Butcherable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Butcherable>(delegate(Butcherable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	public void SetDrops(string[] drops)
	{
		Drops = drops;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(1272413801, SetReadyToButcherDelegate);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		workTime = 3f;
		multitoolContext = "harvest";
		multitoolHitEffectTag = "fx_harvest_splash";
	}

	public void SetReadyToButcher(object param)
	{
		readyToButcher = true;
	}

	public void SetReadyToButcher(bool ready)
	{
		readyToButcher = ready;
	}

	public void ActivateChore(object param)
	{
		if (chore == null)
		{
			chore = new WorkChore<Butcherable>(Db.Get().ChoreTypes.Harvest, this, null, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
			OnRefreshUserMenu(null);
		}
	}

	public void CancelChore(object param)
	{
		if (chore != null)
		{
			chore.Cancel("User cancelled");
			chore = null;
		}
	}

	private void OnClickCancel()
	{
		CancelChore(null);
	}

	private void OnClickButcher()
	{
		if (DebugHandler.InstantBuildMode)
		{
			OnButcherComplete();
		}
		else
		{
			ActivateChore(null);
		}
	}

	private void OnRefreshUserMenu(object data)
	{
		if (readyToButcher)
		{
			KIconButtonMenu.ButtonInfo button = (chore == null) ? new KIconButtonMenu.ButtonInfo("action_harvest", "Meatify", OnClickButcher, Action.NumActions, null, null, null, string.Empty, true) : new KIconButtonMenu.ButtonInfo("action_harvest", "Cancel Meatify", OnClickCancel, Action.NumActions, null, null, null, string.Empty, true);
			Game.Instance.userMenu.AddButton(base.gameObject, button, 1f);
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		OnButcherComplete();
	}

	public void OnButcherComplete()
	{
		if (!butchered)
		{
			KSelectable component = GetComponent<KSelectable>();
			if ((bool)component && component.IsSelected)
			{
				SelectTool.Instance.Select(null, false);
			}
			for (int i = 0; i < Drops.Length; i++)
			{
				GameObject gameObject = Scenario.SpawnPrefab(GetDropSpawnLocation(), 0, 0, Drops[i], Grid.SceneLayer.Ore);
				gameObject.SetActive(true);
				Edible component2 = gameObject.GetComponent<Edible>();
				if ((bool)component2)
				{
					ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, component2.Calories, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.BUTCHERED, "{0}", gameObject.GetProperName()), UI.ENDOFDAYREPORT.NOTES.BUTCHERED_CONTEXT);
				}
			}
			chore = null;
			butchered = true;
			readyToButcher = false;
			Game.Instance.userMenu.Refresh(base.gameObject);
			Trigger(395373363, null);
		}
	}

	private int GetDropSpawnLocation()
	{
		int num = Grid.PosToCell(base.gameObject);
		int num2 = Grid.CellAbove(num);
		if (Grid.IsValidCell(num2) && !Grid.Solid[num2])
		{
			return num2;
		}
		return num;
	}
}
