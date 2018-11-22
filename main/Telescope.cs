using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TUNING;
using UnityEngine;

public class Telescope : Workable, OxygenBreather.IGasProvider, IEffectDescriptor, ISim200ms
{
	public int clearScanCellRadius = 15;

	private OxygenBreather.IGasProvider workerGasProvider = null;

	private Operational operational;

	private float percentClear = 0f;

	private static readonly Operational.Flag visibleSkyFlag = new Operational.Flag("VisibleSky", Operational.Flag.Type.Requirement);

	private static StatusItem reducedVisibilityStatusItem;

	private static StatusItem noVisibilityStatusItem;

	private Storage storage;

	public static readonly Chore.Precondition ContainsOxygen = new Chore.Precondition
	{
		id = "ContainsOxygen",
		sortOrder = 1,
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.CONTAINS_OXYGEN,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			Storage component = context.chore.target.GetComponent<Storage>();
			PrimaryElement x = component.FindFirstWithMass(GameTags.Oxygen);
			return (UnityEngine.Object)x != (UnityEngine.Object)null;
		}
	};

	private Chore chore;

	private Operational.Flag flag = new Operational.Flag("ValidTarget", Operational.Flag.Type.Requirement);

	[CompilerGenerated]
	private static Func<string, object, string> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Func<string, object, string> _003C_003Ef__mg_0024cache1;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		SpacecraftManager.instance.Subscribe(532901469, UpdateWorkingState);
		Components.Telescopes.Add(this);
		if (reducedVisibilityStatusItem == null)
		{
			reducedVisibilityStatusItem = new StatusItem("SPACE_VISIBILITY_REDUCED", "BUILDING", "status_item_no_sky", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			reducedVisibilityStatusItem.resolveStringCallback = GetStatusItemString;
			noVisibilityStatusItem = new StatusItem("SPACE_VISIBILITY_NONE", "BUILDING", "status_item_no_sky", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 63486);
			noVisibilityStatusItem.resolveStringCallback = GetStatusItemString;
		}
		OnWorkableEventCB = (Action<WorkableEvent>)Delegate.Combine(OnWorkableEventCB, new Action<WorkableEvent>(OnWorkableEvent));
		operational = GetComponent<Operational>();
		storage = GetComponent<Storage>();
	}

	protected override void OnCleanUp()
	{
		Components.Telescopes.Remove(this);
		SpacecraftManager.instance.Unsubscribe(532901469, UpdateWorkingState);
		base.OnCleanUp();
	}

	public void Sim200ms(float dt)
	{
		Building component = GetComponent<Building>();
		Extents extents = component.GetExtents();
		int num = Mathf.Max(0, extents.x - clearScanCellRadius);
		int num2 = Mathf.Min(extents.x + clearScanCellRadius);
		int y = extents.y + extents.height - 3;
		int num3 = num2 - num + 1;
		int num4 = Grid.XYToCell(num, y);
		int num5 = Grid.XYToCell(num2, y);
		int num6 = 0;
		for (int i = num4; i <= num5; i++)
		{
			if (Grid.ExposedToSunlight[i] >= 253)
			{
				num6++;
			}
		}
		Operational component2 = GetComponent<Operational>();
		component2.SetFlag(visibleSkyFlag, num6 > 0);
		bool on = num6 < num3;
		KSelectable component3 = GetComponent<KSelectable>();
		if (num6 > 0)
		{
			component3.ToggleStatusItem(noVisibilityStatusItem, false, null);
			component3.ToggleStatusItem(reducedVisibilityStatusItem, on, this);
		}
		else
		{
			component3.ToggleStatusItem(noVisibilityStatusItem, true, this);
			component3.ToggleStatusItem(reducedVisibilityStatusItem, false, null);
		}
		percentClear = (float)num6 / (float)num3;
		if (!component2.IsActive && component2.IsOperational && chore == null)
		{
			chore = CreateChore();
			SetWorkTime(float.PositiveInfinity);
		}
	}

	private static string GetStatusItemString(string src_str, object data)
	{
		Telescope telescope = (Telescope)data;
		string text = src_str.Replace("{VISIBILITY}", GameUtil.GetFormattedPercent(telescope.percentClear * 100f, GameUtil.TimeSlice.None));
		return text.Replace("{RADIUS}", telescope.clearScanCellRadius.ToString());
	}

	private void OnWorkableEvent(WorkableEvent ev)
	{
		Worker worker = base.worker;
		if (!((UnityEngine.Object)worker == (UnityEngine.Object)null))
		{
			OxygenBreather component = worker.GetComponent<OxygenBreather>();
			KPrefabID component2 = worker.GetComponent<KPrefabID>();
			switch (ev)
			{
			case WorkableEvent.WorkStarted:
				ShowProgressBar(true);
				progressBar.SetUpdateFunc(delegate
				{
					if (!SpacecraftManager.instance.HasAnalysisTarget())
					{
						return 0f;
					}
					return SpacecraftManager.instance.GetDestinationAnalysisScore(SpacecraftManager.instance.GetStarmapAnalysisDestinationID()) / (float)ROCKETRY.DESTINATION_ANALYSIS.COMPLETE;
				});
				workerGasProvider = component.GetGasProvider();
				component.SetGasProvider(this);
				component.GetComponent<CreatureSimTemperatureTransfer>().enabled = false;
				component2.AddTag(GameTags.Shaded);
				break;
			case WorkableEvent.WorkStopped:
				component.SetGasProvider(workerGasProvider);
				component.GetComponent<CreatureSimTemperatureTransfer>().enabled = true;
				ShowProgressBar(false);
				component2.RemoveTag(GameTags.Shaded);
				break;
			}
		}
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (SpacecraftManager.instance.HasAnalysisTarget())
		{
			float num = 1f + Db.Get().AttributeConverters.ResearchSpeed.Lookup(worker).Evaluate();
			int starmapAnalysisDestinationID = SpacecraftManager.instance.GetStarmapAnalysisDestinationID();
			SpaceDestination destination = SpacecraftManager.instance.GetDestination(starmapAnalysisDestinationID);
			float num2 = 1f / (float)destination.OneBasedDistance;
			float num3 = (float)ROCKETRY.DESTINATION_ANALYSIS.DISCOVERED;
			float dEFAULT_CYCLES_PER_DISCOVERY = ROCKETRY.DESTINATION_ANALYSIS.DEFAULT_CYCLES_PER_DISCOVERY;
			float num4 = num3 / dEFAULT_CYCLES_PER_DISCOVERY;
			float num5 = num4 / 600f;
			float points = dt * num * num2 * num5;
			SpacecraftManager.instance.EarnDestinationAnalysisPoints(starmapAnalysisDestinationID, points);
		}
		return base.OnWorkTick(worker, dt);
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Element element = ElementLoader.FindElementByHash(SimHashes.Oxygen);
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(element.tag.ProperName(), string.Format(STRINGS.BUILDINGS.PREFABS.TELESCOPE.REQUIREMENT_TOOLTIP, element.tag.ProperName()), Descriptor.DescriptorType.Requirement);
		list.Add(item);
		return list;
	}

	protected Chore CreateChore()
	{
		ChoreType research = Db.Get().ChoreTypes.Research;
		Tag[] researchChores = GameTags.ChoreTypes.ResearchChores;
		WorkChore<Telescope> workChore = new WorkChore<Telescope>(research, this, null, researchChores, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false);
		workChore.AddPrecondition(ContainsOxygen, null);
		return workChore;
	}

	protected void UpdateWorkingState(object data)
	{
		bool flag = false;
		if (SpacecraftManager.instance.HasAnalysisTarget() && SpacecraftManager.instance.GetDestinationAnalysisState(SpacecraftManager.instance.GetDestination(SpacecraftManager.instance.GetStarmapAnalysisDestinationID())) != SpacecraftManager.DestinationAnalysisState.Complete)
		{
			flag = true;
		}
		KSelectable component = GetComponent<KSelectable>();
		component.ToggleStatusItem(Db.Get().BuildingStatusItems.NoApplicableAnalysisSelected, !flag, null);
		operational.SetFlag(this.flag, flag);
		if (!flag && (bool)base.worker)
		{
			StopWork(base.worker, true);
		}
	}

	public void OnSetOxygenBreather(OxygenBreather oxygen_breather)
	{
	}

	public void OnClearOxygenBreather(OxygenBreather oxygen_breather)
	{
	}

	public bool ShouldEmitCO2()
	{
		return false;
	}

	public bool ConsumeGas(OxygenBreather oxygen_breather, float amount)
	{
		if (storage.items.Count > 0)
		{
			GameObject gameObject = storage.items[0];
			if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null))
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				bool result = component.Mass >= amount;
				component.Mass = Mathf.Max(0f, component.Mass - amount);
				return result;
			}
			return false;
		}
		return false;
	}
}
