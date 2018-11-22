using Klei;
using System;
using TUNING;
using UnityEngine;

public class LiquidPumpingStation : Workable, ISim200ms
{
	private class WorkSession
	{
		private int cell;

		private float amountToPickup;

		private float consumedAmount;

		private float temperature;

		private float amountPerTick;

		private SimHashes element;

		private float lastTickAmount;

		private SubstanceChunk source;

		private SimUtil.DiseaseInfo diseaseInfo;

		private GameObject pump;

		public WorkSession(int cell, SimHashes element, SubstanceChunk source, float amount_to_pickup, GameObject pump)
		{
			this.cell = cell;
			this.element = element;
			this.source = source;
			amountToPickup = amount_to_pickup;
			temperature = ElementLoader.FindElementByHash(element).defaultValues.temperature;
			diseaseInfo = SimUtil.DiseaseInfo.Invalid;
			amountPerTick = 40f;
			this.pump = pump;
			lastTickAmount = amountPerTick;
			ConsumeMass();
		}

		private void OnSimConsumeCallback(Sim.MassConsumedCallback mass_cb_info, object data)
		{
			((WorkSession)data).OnSimConsume(mass_cb_info);
		}

		private void OnSimConsume(Sim.MassConsumedCallback mass_cb_info)
		{
			if (consumedAmount == 0f)
			{
				temperature = mass_cb_info.temperature;
			}
			else
			{
				temperature = GameUtil.GetFinalTemperature(temperature, consumedAmount, mass_cb_info.temperature, mass_cb_info.mass);
			}
			consumedAmount += mass_cb_info.mass;
			lastTickAmount = mass_cb_info.mass;
			diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(diseaseInfo.idx, diseaseInfo.count, mass_cb_info.diseaseIdx, mass_cb_info.diseaseCount);
			if (consumedAmount >= amountToPickup)
			{
				amountPerTick = 0f;
				lastTickAmount = 0f;
			}
			ConsumeMass();
		}

		private void ConsumeMass()
		{
			if (amountPerTick > 0f)
			{
				float a = Mathf.Min(amountPerTick, amountToPickup - consumedAmount);
				a = Mathf.Max(a, 1f);
				HandleVector<Game.ComplexCallbackInfo<Sim.MassConsumedCallback>>.Handle handle = Game.Instance.massConsumedCallbackManager.Add(OnSimConsumeCallback, this, "LiquidPumpingStation");
				int gameCell = Grid.OffsetCell(cell, new CellOffset(0, -PumpingStationGuide.GetDepthAvailable(cell, pump)));
				SimMessages.ConsumeMass(gameCell, element, a, 3, handle.index);
			}
		}

		public float GetPercentComplete()
		{
			return consumedAmount / amountToPickup;
		}

		public float GetLastTickAmount()
		{
			return lastTickAmount;
		}

		public SimUtil.DiseaseInfo GetDiseaseInfo()
		{
			return diseaseInfo;
		}

		public SubstanceChunk GetSource()
		{
			return source;
		}

		public float GetConsumedAmount()
		{
			return consumedAmount;
		}

		public float GetTemperature()
		{
			if (!(temperature <= 0f))
			{
				return temperature;
			}
			Debug.LogWarning("TODO(YOG): Fix bad temperature in liquid pumping station.", null);
			return ElementLoader.FindElementByHash(element).defaultValues.temperature;
		}

		public void Cleanup()
		{
			amountPerTick = 0f;
			diseaseInfo = SimUtil.DiseaseInfo.Invalid;
		}
	}

	private struct LiquidInfo
	{
		public float amount;

		public Element element;

		public SubstanceChunk source;
	}

	private static readonly CellOffset[] floorOffsets = new CellOffset[2]
	{
		new CellOffset(0, 0),
		new CellOffset(1, 0)
	};

	private static readonly CellOffset[] liquidOffsets = new CellOffset[10]
	{
		new CellOffset(0, 0),
		new CellOffset(1, 0),
		new CellOffset(0, -1),
		new CellOffset(1, -1),
		new CellOffset(0, -2),
		new CellOffset(1, -2),
		new CellOffset(0, -3),
		new CellOffset(1, -3),
		new CellOffset(0, -4),
		new CellOffset(1, -4)
	};

	private LiquidInfo[] infos;

	private int infoCount = 0;

	private int depthAvailable = -1;

	private WorkSession session;

	private MeterController meter;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		resetProgressOnStop = true;
		showProgressBar = false;
		int cell = Grid.PosToCell(this);
		for (int i = 0; i < floorOffsets.Length; i++)
		{
			int num = Grid.OffsetCell(cell, floorOffsets[i]);
			Grid.LiquidPumpFloor[num] = true;
			Pathfinding.Instance.AddDirtyNavGridCell(num);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		infos = new LiquidInfo[liquidOffsets.Length * 2];
		GetComponent<KSelectable>().SetStatusIndicatorOffset(GetComponent<Building>().Def.placementPivot);
		RefreshStatusItem();
		Sim200ms(0f);
		SetWorkTime(10f);
		RefreshDepthAvailable();
		meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, "meter_target", "meter_arrow", "meter_scale");
		foreach (GameObject item in GetComponent<Storage>().items)
		{
			if (!((UnityEngine.Object)item == (UnityEngine.Object)null) && (UnityEngine.Object)item != (UnityEngine.Object)null)
			{
				item.DeleteObject();
			}
		}
	}

	private void RefreshDepthAvailable()
	{
		int num = PumpingStationGuide.GetDepthAvailable(Grid.PosToCell(this), base.gameObject);
		int num2 = 4;
		if (num > depthAvailable)
		{
			KAnimControllerBase component = GetComponent<KAnimControllerBase>();
			for (int i = 1; i <= num2; i++)
			{
				component.SetSymbolVisiblity("pipe" + i.ToString(), i <= num);
			}
			PumpingStationGuide.OccupyArea(base.gameObject, num);
			depthAvailable = num;
		}
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole("Hauler", work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
		resume.AddExperienceIfRole(MaterialsManager.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
	}

	public void Sim200ms(float dt)
	{
		if (session == null)
		{
			int num = infoCount;
			for (int i = 0; i < infoCount; i++)
			{
				infos[i].amount = 0f;
			}
			if (GetComponent<Operational>().IsOperational)
			{
				int cell = Grid.PosToCell(this);
				for (int j = 0; j < liquidOffsets.Length; j++)
				{
					if (depthAvailable >= Math.Abs(liquidOffsets[j].y))
					{
						int num2 = Grid.OffsetCell(cell, liquidOffsets[j]);
						bool flag = false;
						Element element = Grid.Element[num2];
						if (element.IsLiquid)
						{
							float num3 = Grid.Mass[num2];
							for (int k = 0; k < infoCount; k++)
							{
								if (infos[k].element == element)
								{
									infos[k].amount += num3;
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								infos[infoCount].amount = num3;
								infos[infoCount].element = element;
								infoCount++;
							}
						}
					}
				}
			}
			int num4 = 0;
			while (num4 < infoCount)
			{
				LiquidInfo liquidInfo = infos[num4];
				if (liquidInfo.amount <= 1f)
				{
					if ((UnityEngine.Object)liquidInfo.source != (UnityEngine.Object)null)
					{
						liquidInfo.source.DeleteObject();
					}
					infos[num4] = infos[infoCount - 1];
					infoCount--;
				}
				else
				{
					if ((UnityEngine.Object)liquidInfo.source == (UnityEngine.Object)null)
					{
						liquidInfo.source = GetComponent<Storage>().AddLiquid(liquidInfo.element.id, liquidInfo.amount, liquidInfo.element.defaultValues.temperature, byte.MaxValue, 0, false, true).GetComponent<SubstanceChunk>();
						Pickupable component = liquidInfo.source.GetComponent<Pickupable>();
						component.GetComponent<KPrefabID>().AddTag(GameTags.LiquidSource);
						component.SetOffsets(new CellOffset[1]
						{
							new CellOffset(0, 1)
						});
						component.targetWorkable = this;
						Pickupable pickupable = component;
						pickupable.OnReservationsChanged = (System.Action)Delegate.Combine(pickupable.OnReservationsChanged, new System.Action(OnReservationsChanged));
					}
					liquidInfo.source.GetComponent<Pickupable>().TotalAmount = liquidInfo.amount;
					infos[num4] = liquidInfo;
					num4++;
				}
			}
			if (num != infoCount)
			{
				RefreshStatusItem();
			}
			RefreshDepthAvailable();
		}
	}

	private void RefreshStatusItem()
	{
		if (infoCount > 0)
		{
			GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.PumpingStation, this);
		}
		else
		{
			GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.EmptyPumpingStation, this);
		}
	}

	public string ResolveString(string base_string)
	{
		string text = "";
		for (int i = 0; i < infoCount; i++)
		{
			if ((UnityEngine.Object)infos[i].source != (UnityEngine.Object)null)
			{
				string text2 = text;
				text = text2 + "\n" + infos[i].element.name + ": " + GameUtil.GetFormattedMass(infos[i].amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
			}
		}
		return base_string.Replace("{Liquids}", text);
	}

	public static bool IsLiquidAccessible(Element element)
	{
		return true;
	}

	public override float GetPercentComplete()
	{
		if (session == null)
		{
			return 0f;
		}
		return session.GetPercentComplete();
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		Pickupable.PickupableStartWorkInfo pickupableStartWorkInfo = (Pickupable.PickupableStartWorkInfo)worker.startWorkInfo;
		float amount = pickupableStartWorkInfo.amount;
		Element element = pickupableStartWorkInfo.originalPickupable.GetComponent<PrimaryElement>().Element;
		session = new WorkSession(Grid.PosToCell(this), element.id, pickupableStartWorkInfo.originalPickupable.GetComponent<SubstanceChunk>(), amount, base.gameObject);
		meter.SetPositionPercent(0f);
		meter.SetSymbolTint(new KAnimHashedString("meter_target"), element.substance.colour);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		if (session != null)
		{
			session.Cleanup();
			session = null;
		}
		KAnimControllerBase component = GetComponent<KAnimControllerBase>();
		component.Play("on", KAnim.PlayMode.Once, 1f, 0f);
	}

	private void OnReservationsChanged()
	{
		bool forceUnfetchable = false;
		for (int i = 0; i < infoCount; i++)
		{
			if ((UnityEngine.Object)infos[i].source != (UnityEngine.Object)null && infos[i].source.GetComponent<Pickupable>().ReservedAmount > 0f)
			{
				forceUnfetchable = true;
				break;
			}
		}
		for (int j = 0; j < infoCount; j++)
		{
			if ((UnityEngine.Object)infos[j].source != (UnityEngine.Object)null)
			{
				infos[j].source.GetSMI<FetchableMonitor.Instance>()?.SetForceUnfetchable(forceUnfetchable);
			}
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		if (session != null)
		{
			Storage component = worker.GetComponent<Storage>();
			float consumedAmount = session.GetConsumedAmount();
			if (consumedAmount > 0f)
			{
				SubstanceChunk source = session.GetSource();
				SimUtil.DiseaseInfo diseaseInfo = (session == null) ? SimUtil.DiseaseInfo.Invalid : session.GetDiseaseInfo();
				PrimaryElement component2 = source.GetComponent<PrimaryElement>();
				Pickupable component3 = LiquidSourceManager.Instance.CreateChunk(component2.Element, consumedAmount, session.GetTemperature(), diseaseInfo.idx, diseaseInfo.count, base.transform.GetPosition()).GetComponent<Pickupable>();
				component3.TotalAmount = consumedAmount;
				component3.Trigger(1335436905, source.GetComponent<Pickupable>());
				worker.workCompleteData = component3;
				Sim200ms(0f);
				if ((UnityEngine.Object)component3 != (UnityEngine.Object)null)
				{
					component.Store(component3.gameObject, false, false, true, false);
				}
			}
			session.Cleanup();
			session = null;
		}
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (session != null)
		{
			meter.SetPositionPercent(session.GetPercentComplete());
			if (session.GetLastTickAmount() <= 0f)
			{
				return true;
			}
		}
		return false;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (session != null)
		{
			session.Cleanup();
			session = null;
		}
		for (int i = 0; i < infoCount; i++)
		{
			if ((UnityEngine.Object)infos[i].source != (UnityEngine.Object)null)
			{
				infos[i].source.DeleteObject();
			}
		}
		int cell = Grid.PosToCell(this);
		for (int j = 0; j < floorOffsets.Length; j++)
		{
			int num = Grid.OffsetCell(cell, floorOffsets[j]);
			Grid.LiquidPumpFloor[num] = false;
			Pathfinding.Instance.AddDirtyNavGridCell(num);
		}
	}
}
