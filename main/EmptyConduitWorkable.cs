using KSerialization;
using STRINGS;
using System;
using UnityEngine;

public class EmptyConduitWorkable : Workable
{
	[MyCmpReq]
	private Conduit conduit;

	private static StatusItem emptyLiquidConduitStatusItem;

	private static StatusItem emptyGasConduitStatusItem;

	private Chore chore;

	private const float RECHECK_PIPE_INTERVAL = 2f;

	private const float TIME_TO_EMPTY_PIPE = 4f;

	private const float NO_EMPTY_SCHEDULED = -1f;

	[Serialize]
	private float elapsedTime = -1f;

	private bool emptiedPipe = true;

	private static readonly EventSystem.IntraObjectHandler<EmptyConduitWorkable> OnEmptyConduitCancelledDelegate = new EventSystem.IntraObjectHandler<EmptyConduitWorkable>(delegate(EmptyConduitWorkable component, object data)
	{
		component.OnEmptyConduitCancelled(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		SetOffsetTable(OffsetGroups.InvertedStandardTable);
		SetWorkTime(float.PositiveInfinity);
		faceTargetWhenWorking = true;
		multitoolContext = "build";
		multitoolHitEffectTag = EffectConfigs.BuildSplashId;
		Subscribe(2127324410, OnEmptyConduitCancelledDelegate);
		if (emptyLiquidConduitStatusItem == null)
		{
			emptyLiquidConduitStatusItem = new StatusItem("EmptyLiquidConduit", BUILDINGS.PREFABS.CONDUIT.STATUS_ITEM.NAME, BUILDINGS.PREFABS.CONDUIT.STATUS_ITEM.TOOLTIP, "status_item_empty_pipe", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.LiquidConduits.ID, 66);
			emptyGasConduitStatusItem = new StatusItem("EmptyGasConduit", BUILDINGS.PREFABS.CONDUIT.STATUS_ITEM.NAME, BUILDINGS.PREFABS.CONDUIT.STATUS_ITEM.TOOLTIP, "status_item_empty_pipe", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.GasConduits.ID, 130);
		}
		requiredSkillPerk = Db.Get().SkillPerks.CanDoPlumbing.Id;
		shouldShowSkillPerkStatusItem = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (elapsedTime != -1f)
		{
			MarkForEmptying();
		}
	}

	public void MarkForEmptying()
	{
		if (chore == null)
		{
			StatusItem statusItem = GetStatusItem();
			KSelectable component = GetComponent<KSelectable>();
			component.ToggleStatusItem(statusItem, true, null);
			CreateWorkChore();
		}
	}

	private void CancelEmptying()
	{
		CleanUpVisualization();
		if (chore != null)
		{
			chore.Cancel("Cancel");
			chore = null;
			shouldShowSkillPerkStatusItem = false;
			UpdateStatusItem(null);
		}
	}

	private void CleanUpVisualization()
	{
		StatusItem statusItem = GetStatusItem();
		KSelectable component = GetComponent<KSelectable>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.ToggleStatusItem(statusItem, false, null);
		}
		elapsedTime = -1f;
		if (chore != null)
		{
			GetComponent<Prioritizable>().RemoveRef();
		}
	}

	protected override void OnCleanUp()
	{
		CancelEmptying();
		base.OnCleanUp();
	}

	private ConduitFlow GetFlowManager()
	{
		return (conduit.type != ConduitType.Gas) ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow;
	}

	private void OnEmptyConduitCancelled(object data)
	{
		CancelEmptying();
	}

	private StatusItem GetStatusItem()
	{
		switch (conduit.type)
		{
		case ConduitType.Gas:
			return emptyGasConduitStatusItem;
		case ConduitType.Liquid:
			return emptyLiquidConduitStatusItem;
		default:
			throw new ArgumentException();
		}
	}

	private void CreateWorkChore()
	{
		GetComponent<Prioritizable>().AddRef();
		chore = new WorkChore<EmptyConduitWorkable>(Db.Get().ChoreTypes.EmptyStorage, this, null, true, null, null, null, true, null, false, false, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
		chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanDoPlumbing.Id);
		elapsedTime = 0f;
		emptiedPipe = false;
		shouldShowSkillPerkStatusItem = true;
		UpdateStatusItem(null);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (elapsedTime == -1f)
		{
			return true;
		}
		bool result = false;
		elapsedTime += dt;
		if (!emptiedPipe)
		{
			if (elapsedTime > 4f)
			{
				EmptyPipeContents();
				emptiedPipe = true;
				elapsedTime = 0f;
			}
		}
		else if (elapsedTime > 2f)
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			if (GetFlowManager().GetContents(cell).mass > 0f)
			{
				elapsedTime = 0f;
				emptiedPipe = false;
			}
			else
			{
				CleanUpVisualization();
				chore = null;
				result = true;
				shouldShowSkillPerkStatusItem = false;
				UpdateStatusItem(null);
			}
		}
		return result;
	}

	public void EmptyPipeContents()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		ConduitFlow.ConduitContents conduitContents = GetFlowManager().RemoveElement(cell, float.PositiveInfinity);
		elapsedTime = 0f;
		if (conduitContents.mass > 0f && conduitContents.element != SimHashes.Vacuum)
		{
			IChunkManager instance;
			switch (conduit.type)
			{
			case ConduitType.Gas:
				instance = GasSourceManager.Instance;
				break;
			case ConduitType.Liquid:
				instance = LiquidSourceManager.Instance;
				break;
			default:
				throw new ArgumentException();
			}
			instance.CreateChunk(conduitContents.element, conduitContents.mass, conduitContents.temperature, conduitContents.diseaseIdx, conduitContents.diseaseCount, Grid.CellToPosCCC(cell, Grid.SceneLayer.Ore));
		}
	}

	public override float GetPercentComplete()
	{
		return Mathf.Clamp01(elapsedTime / 4f);
	}
}
