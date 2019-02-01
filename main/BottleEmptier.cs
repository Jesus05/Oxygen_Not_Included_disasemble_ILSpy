using Klei;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class BottleEmptier : StateMachineComponent<BottleEmptier.StatesInstance>, IEffectDescriptor
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, BottleEmptier, object>.GameInstance
	{
		private FetchChore chore;

		public MeterController meter
		{
			get;
			private set;
		}

		public StatesInstance(BottleEmptier smi)
			: base(smi)
		{
			TreeFilterable component = base.master.GetComponent<TreeFilterable>();
			component.OnFilterChanged = (Action<Tag[]>)Delegate.Combine(component.OnFilterChanged, new Action<Tag[]>(OnFilterChanged));
			meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_target", "meter_arrow", "meter_scale");
			Subscribe(-1697596308, OnStorageChange);
		}

		public void CreateChore()
		{
			KBatchedAnimController component = GetComponent<KBatchedAnimController>();
			Tag[] tags = GetComponent<TreeFilterable>().GetTags();
			if (tags == null || tags.Length == 0)
			{
				component.TintColour = base.master.noFilterTint;
			}
			else
			{
				component.TintColour = base.master.filterTint;
				Tag[] array = base.master.allowManualPumpingStationFetching ? new Tag[0] : new Tag[1]
				{
					GameTags.LiquidSource
				};
				Storage component2 = GetComponent<Storage>();
				ChoreType storageFetch = Db.Get().ChoreTypes.StorageFetch;
				Storage destination = component2;
				float amount = component2.Capacity();
				Tag[] tags2 = GetComponent<TreeFilterable>().GetTags();
				Tag[] forbidden_tags = array;
				chore = new FetchChore(storageFetch, destination, amount, tags2, null, forbidden_tags, null, true, null, null, null, FetchOrder2.OperationalRequirement.Operational, 0, null);
			}
		}

		public void CancelChore()
		{
			if (chore != null)
			{
				chore.Cancel("Storage Changed");
				chore = null;
			}
		}

		public void RefreshChore()
		{
			GoTo(base.sm.unoperational);
		}

		private void OnFilterChanged(Tag[] tags)
		{
			RefreshChore();
		}

		private void OnStorageChange(object data)
		{
			Storage component = GetComponent<Storage>();
			meter.SetPositionPercent(Mathf.Clamp01(component.RemainingCapacity() / component.capacityKg));
		}

		public void StartMeter()
		{
			PrimaryElement firstPrimaryElement = GetFirstPrimaryElement();
			if (!((UnityEngine.Object)firstPrimaryElement == (UnityEngine.Object)null))
			{
				meter.SetSymbolTint(new KAnimHashedString("meter_fill"), firstPrimaryElement.Element.substance.colour);
				meter.SetSymbolTint(new KAnimHashedString("water1"), firstPrimaryElement.Element.substance.colour);
				GetComponent<KBatchedAnimController>().SetSymbolTint(new KAnimHashedString("leak_ceiling"), firstPrimaryElement.Element.substance.colour);
			}
		}

		private PrimaryElement GetFirstPrimaryElement()
		{
			Storage component = GetComponent<Storage>();
			for (int i = 0; i < component.Count; i++)
			{
				GameObject gameObject = component[i];
				if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null))
				{
					PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
					if (!((UnityEngine.Object)component2 == (UnityEngine.Object)null))
					{
						return component2;
					}
				}
			}
			return null;
		}

		public void Emit(float dt)
		{
			PrimaryElement firstPrimaryElement = GetFirstPrimaryElement();
			if (!((UnityEngine.Object)firstPrimaryElement == (UnityEngine.Object)null))
			{
				Storage component = GetComponent<Storage>();
				float mass = firstPrimaryElement.Mass;
				float num = Mathf.Min(mass, base.master.emptyRate * dt);
				if (!(num <= 0f))
				{
					Tag prefabTag = firstPrimaryElement.GetComponent<KPrefabID>().PrefabTag;
					component.ConsumeAndGetDisease(prefabTag, num, out SimUtil.DiseaseInfo disease_info, out float aggregate_temperature);
					Vector3 position = base.transform.GetPosition();
					position.y += 1.8f;
					bool flag = GetComponent<Rotatable>().GetOrientation() == Orientation.FlipH;
					position.x += ((!flag) ? 0.2f : (-0.2f));
					int num2 = Grid.PosToCell(position) + ((!flag) ? 1 : (-1));
					if (Grid.Solid[num2])
					{
						num2 += (flag ? 1 : (-1));
					}
					Element element = firstPrimaryElement.Element;
					byte idx = element.idx;
					if (element.IsLiquid)
					{
						FallingWater.instance.AddParticle(num2, idx, num, aggregate_temperature, disease_info.idx, disease_info.count, true, false, false, false);
					}
					else
					{
						SimMessages.ModifyCell(num2, idx, aggregate_temperature, num, disease_info.idx, disease_info.count, SimMessages.ReplaceType.None, false, -1);
					}
				}
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, BottleEmptier>
	{
		private StatusItem statusItem;

		public State unoperational;

		public State waitingfordelivery;

		public State emptying;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = waitingfordelivery;
			statusItem = new StatusItem("BottleEmptier", "", "", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, 63486);
			statusItem.resolveStringCallback = delegate(string str, object data)
			{
				BottleEmptier bottleEmptier2 = (BottleEmptier)data;
				if (!((UnityEngine.Object)bottleEmptier2 == (UnityEngine.Object)null))
				{
					if (!bottleEmptier2.allowManualPumpingStationFetching)
					{
						return BUILDING.STATUSITEMS.BOTTLE_EMPTIER.DENIED.NAME;
					}
					return BUILDING.STATUSITEMS.BOTTLE_EMPTIER.ALLOWED.NAME;
				}
				return str;
			};
			statusItem.resolveTooltipCallback = delegate(string str, object data)
			{
				BottleEmptier bottleEmptier = (BottleEmptier)data;
				if (!((UnityEngine.Object)bottleEmptier == (UnityEngine.Object)null))
				{
					if (!bottleEmptier.allowManualPumpingStationFetching)
					{
						return BUILDING.STATUSITEMS.BOTTLE_EMPTIER.DENIED.TOOLTIP;
					}
					return BUILDING.STATUSITEMS.BOTTLE_EMPTIER.ALLOWED.TOOLTIP;
				}
				return str;
			};
			root.ToggleStatusItem(statusItem, (StatesInstance smi) => smi.master);
			unoperational.TagTransition(GameTags.Operational, waitingfordelivery, false).PlayAnim("off");
			waitingfordelivery.TagTransition(GameTags.Operational, unoperational, true).EventTransition(GameHashes.OnStorageChange, emptying, (StatesInstance smi) => !smi.GetComponent<Storage>().IsEmpty()).Enter("CreateChore", delegate(StatesInstance smi)
			{
				smi.CreateChore();
			})
				.Exit("CancelChore", delegate(StatesInstance smi)
				{
					smi.CancelChore();
				})
				.PlayAnim("on");
			emptying.TagTransition(GameTags.Operational, unoperational, true).EventTransition(GameHashes.OnStorageChange, waitingfordelivery, (StatesInstance smi) => smi.GetComponent<Storage>().IsEmpty()).Enter("StartMeter", delegate(StatesInstance smi)
			{
				smi.StartMeter();
			})
				.Update("Emit", delegate(StatesInstance smi, float dt)
				{
					smi.Emit(dt);
				}, UpdateRate.SIM_200ms, false)
				.PlayAnim("working_loop", KAnim.PlayMode.Loop);
		}
	}

	public float emptyRate = 10f;

	[Serialize]
	public bool allowManualPumpingStationFetching;

	[SerializeField]
	public Color noFilterTint = FilteredStorage.NO_FILTER_TINT;

	[SerializeField]
	public Color filterTint = FilteredStorage.FILTER_TINT;

	private static readonly EventSystem.IntraObjectHandler<BottleEmptier> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<BottleEmptier>(delegate(BottleEmptier component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		Subscribe(493375141, OnRefreshUserMenuDelegate);
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return null;
	}

	private void OnChangeAllowManualPumpingStationFetching()
	{
		allowManualPumpingStationFetching = !allowManualPumpingStationFetching;
		base.smi.RefreshChore();
	}

	private void OnRefreshUserMenu(object data)
	{
		object buttonInfo;
		if (allowManualPumpingStationFetching)
		{
			string iconName = "action_bottler_delivery";
			string text = UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED.NAME;
			System.Action on_click = OnChangeAllowManualPumpingStationFetching;
			string tooltipText = UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true);
		}
		else
		{
			string tooltipText = "action_bottler_delivery";
			string text = UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED.NAME;
			System.Action on_click = OnChangeAllowManualPumpingStationFetching;
			string iconName = UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(tooltipText, text, on_click, Action.NumActions, null, null, null, iconName, true);
		}
		KIconButtonMenu.ButtonInfo button = (KIconButtonMenu.ButtonInfo)buttonInfo;
		Game.Instance.userMenu.AddButton(base.gameObject, button, 1f);
	}
}
