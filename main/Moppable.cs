using STRINGS;
using System;
using TUNING;
using UnityEngine;

public class Moppable : Workable, ISim1000ms, ISim200ms
{
	[MyCmpReq]
	private KSelectable Selectable;

	[MyCmpAdd]
	private Prioritizable prioritizable;

	public float amountMoppedPerTick = 1000f;

	private HandleVector<int>.Handle partitionerEntry;

	private SchedulerHandle destroyHandle;

	private float amountMopped;

	private MeshRenderer childRenderer;

	private CellOffset[] offsets = new CellOffset[3]
	{
		new CellOffset(0, 0),
		new CellOffset(1, 0),
		new CellOffset(-1, 0)
	};

	private static readonly EventSystem.IntraObjectHandler<Moppable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Moppable>(delegate(Moppable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Moppable> OnReachableChangedDelegate = new EventSystem.IntraObjectHandler<Moppable>(delegate(Moppable component, object data)
	{
		component.OnReachableChanged(data);
	});

	private Moppable()
	{
		showProgressBar = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workerStatusItem = Db.Get().DuplicantStatusItems.Mopping;
		attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		childRenderer = GetComponentInChildren<MeshRenderer>();
		Prioritizable.AddRef(base.gameObject);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (!IsThereLiquid())
		{
			base.gameObject.DeleteObject();
		}
		else
		{
			Grid.Objects[Grid.PosToCell(base.gameObject), 8] = base.gameObject;
			new WorkChore<Moppable>(Db.Get().ChoreTypes.Mop, this, null, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
			SetWorkTime(float.PositiveInfinity);
			KSelectable component = GetComponent<KSelectable>();
			component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().MiscStatusItems.WaitingForMop, null);
			Subscribe(493375141, OnRefreshUserMenuDelegate);
			overrideAnims = new KAnimFile[1]
			{
				Assets.GetAnim("anim_mop_dirtywater_kanim")
			};
			partitionerEntry = GameScenePartitioner.Instance.Add("Moppable.OnSpawn", base.gameObject, new Extents(Grid.PosToCell(this), new CellOffset[1]
			{
				new CellOffset(0, 0)
			}), GameScenePartitioner.Instance.liquidChangedLayer, OnLiquidChanged);
			Refresh();
			Subscribe(-1432940121, OnReachableChangedDelegate);
			ReachabilityMonitor.Instance instance = new ReachabilityMonitor.Instance(this);
			instance.StartSM();
			SimAndRenderScheduler.instance.Remove(this);
		}
	}

	private void OnRefreshUserMenu(object data)
	{
		UserMenu userMenu = Game.Instance.userMenu;
		GameObject gameObject = base.gameObject;
		string iconName = "icon_cancel";
		string text = UI.USERMENUACTIONS.CANCELMOP.NAME;
		System.Action on_click = OnCancel;
		string tooltipText = UI.USERMENUACTIONS.CANCELMOP.TOOLTIP;
		userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true), 1f);
	}

	private void OnCancel()
	{
		DetailsScreen.Instance.Show(false);
		base.gameObject.Trigger(2127324410, null);
	}

	protected override void OnStartWork(Worker worker)
	{
		SimAndRenderScheduler.instance.Add(this, false);
		Refresh();
		MopTick();
	}

	protected override void OnStopWork(Worker worker)
	{
		SimAndRenderScheduler.instance.Remove(this);
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole(Handyman.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		SimAndRenderScheduler.instance.Remove(this);
	}

	public void Sim1000ms(float dt)
	{
		if (amountMopped > 0f)
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, GameUtil.GetFormattedMass(0f - amountMopped, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), base.transform, 1.5f, false);
			amountMopped = 0f;
		}
	}

	public void Sim200ms(float dt)
	{
		if ((UnityEngine.Object)base.worker != (UnityEngine.Object)null)
		{
			Refresh();
			MopTick();
		}
	}

	private void OnCellMopped(Sim.MassConsumedCallback mass_cb_info, object data)
	{
		if (!((UnityEngine.Object)this == (UnityEngine.Object)null) && mass_cb_info.mass > 0f)
		{
			amountMopped += mass_cb_info.mass;
			int cell = Grid.PosToCell(this);
			SubstanceChunk substanceChunk = LiquidSourceManager.Instance.CreateChunk(ElementLoader.elements[mass_cb_info.elemIdx], mass_cb_info.mass, mass_cb_info.temperature, mass_cb_info.diseaseIdx, mass_cb_info.diseaseCount, Grid.CellToPosCCC(cell, Grid.SceneLayer.Ore));
			substanceChunk.transform.SetPosition(substanceChunk.transform.GetPosition() + new Vector3((UnityEngine.Random.value - 0.5f) * 0.5f, 0f, 0f));
		}
	}

	public static void MopCell(int cell, float amount, Action<Sim.MassConsumedCallback, object> cb)
	{
		if (Grid.Element[cell].IsLiquid)
		{
			int callbackIdx = -1;
			if (cb != null)
			{
				callbackIdx = Game.Instance.massConsumedCallbackManager.Add(cb, null, "Moppable").index;
			}
			SimMessages.ConsumeMass(cell, Grid.Element[cell].id, amount, 1, callbackIdx);
		}
	}

	private void MopTick()
	{
		int cell = Grid.PosToCell(this);
		for (int i = 0; i < offsets.Length; i++)
		{
			int num = Grid.OffsetCell(cell, offsets[i]);
			if (Grid.Element[num].IsLiquid)
			{
				MopCell(num, amountMoppedPerTick, OnCellMopped);
			}
		}
	}

	private bool IsThereLiquid()
	{
		int cell = Grid.PosToCell(this);
		bool result = false;
		for (int i = 0; i < offsets.Length; i++)
		{
			int num = Grid.OffsetCell(cell, offsets[i]);
			if (Grid.Element[num].IsLiquid && Grid.Mass[num] <= MopTool.maxMopAmt)
			{
				result = true;
			}
		}
		return result;
	}

	private void Refresh()
	{
		if (!IsThereLiquid())
		{
			if (!destroyHandle.IsValid)
			{
				destroyHandle = GameScheduler.Instance.Schedule("DestroyMoppable", 1f, delegate
				{
					TryDestroy();
				}, this, null);
			}
		}
		else if (destroyHandle.IsValid)
		{
			destroyHandle.ClearScheduler();
		}
	}

	private void OnLiquidChanged(object data)
	{
		Refresh();
	}

	private void TryDestroy()
	{
		if ((UnityEngine.Object)this != (UnityEngine.Object)null)
		{
			base.gameObject.DeleteObject();
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
	}

	private void OnReachableChanged(object data)
	{
		if ((UnityEngine.Object)childRenderer != (UnityEngine.Object)null)
		{
			Material material = childRenderer.material;
			bool flag = (bool)data;
			Color color = material.color;
			Game.LocationColours dig = Game.Instance.uiColours.Dig;
			if (!(color == dig.invalidLocation))
			{
				KSelectable component = GetComponent<KSelectable>();
				if (flag)
				{
					Material material2 = material;
					Game.LocationColours dig2 = Game.Instance.uiColours.Dig;
					material2.color = dig2.validLocation;
					component.RemoveStatusItem(Db.Get().BuildingStatusItems.MopUnreachable, false);
				}
				else
				{
					component.AddStatusItem(Db.Get().BuildingStatusItems.MopUnreachable, this);
					GameScheduler.Instance.Schedule("Locomotion Tutorial", 2f, delegate
					{
						Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Locomotion);
					}, null, null);
					Material material3 = material;
					Game.LocationColours dig3 = Game.Instance.uiColours.Dig;
					material3.color = dig3.unreachable;
				}
			}
		}
	}
}
