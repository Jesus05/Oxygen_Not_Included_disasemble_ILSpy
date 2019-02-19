using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class Overheatable : StateMachineComponent<Overheatable.StatesInstance>, IEffectDescriptor, IGameObjectEffectDescriptor
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Overheatable, object>.GameInstance
	{
		public float lastOverheatDamageTime;

		public StatesInstance(Overheatable smi)
			: base(smi)
		{
		}

		public void TryDoOverheatDamage()
		{
			if (!(Time.time - lastOverheatDamageTime < 7.5f))
			{
				lastOverheatDamageTime += 7.5f;
				base.master.Trigger(-794517298, new BuildingHP.DamageSourceInfo
				{
					damage = 1,
					source = (string)BUILDINGS.DAMAGESOURCES.BUILDING_OVERHEATED,
					popString = (string)UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.OVERHEAT
				});
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Overheatable>
	{
		public State invulnerable;

		public State safeTemperature;

		public State overheated;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = safeTemperature;
			root.EventTransition(GameHashes.BuildingBroken, invulnerable, null);
			invulnerable.EventHandler(GameHashes.BuildingPartiallyRepaired, delegate(StatesInstance smi)
			{
				smi.master.ResetTemperature();
			}).EventTransition(GameHashes.BuildingPartiallyRepaired, safeTemperature, null);
			safeTemperature.TriggerOnEnter(GameHashes.OptimalTemperatureAchieved, null).EventTransition(GameHashes.BuildingOverheated, overheated, null);
			overheated.Enter(delegate
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_OverheatingBuildings);
			}).EventTransition(GameHashes.BuildingNoLongerOverheated, safeTemperature, null).ToggleStatusItem(Db.Get().BuildingStatusItems.Overheated, (object)null)
				.ToggleNotification((StatesInstance smi) => smi.master.CreateOverheatedNotification())
				.TriggerOnEnter(GameHashes.TooHotWarning, null)
				.Enter("InitOverheatTime", delegate(StatesInstance smi)
				{
					smi.lastOverheatDamageTime = Time.time;
				})
				.Update("OverheatDamage", delegate(StatesInstance smi, float dt)
				{
					smi.TryDoOverheatDamage();
				}, UpdateRate.SIM_4000ms, false);
		}
	}

	private bool modifiersInitialized;

	private AttributeInstance overheatTemp;

	private AttributeInstance fatalTemp;

	public float baseOverheatTemp;

	public float baseFatalTemp;

	public float OverheatTemperature
	{
		get
		{
			InitializeModifiers();
			return (overheatTemp == null) ? 10000f : overheatTemp.GetTotalValue();
		}
	}

	public void ResetTemperature()
	{
		PrimaryElement component = GetComponent<PrimaryElement>();
		component.Temperature = 293.15f;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		overheatTemp = this.GetAttributes().Add(Db.Get().BuildingAttributes.OverheatTemperature);
		fatalTemp = this.GetAttributes().Add(Db.Get().BuildingAttributes.FatalTemperature);
	}

	private void InitializeModifiers()
	{
		if (!modifiersInitialized)
		{
			modifiersInitialized = true;
			AttributeModifier modifier = new AttributeModifier(overheatTemp.Id, baseOverheatTemp, UI.TOOLTIPS.BASE_VALUE, false, false, true);
			AttributeModifier modifier2 = new AttributeModifier(fatalTemp.Id, baseFatalTemp, UI.TOOLTIPS.BASE_VALUE, false, false, true);
			this.GetAttributes().Add(modifier);
			this.GetAttributes().Add(modifier2);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		InitializeModifiers();
		HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(base.gameObject);
		if (handle.IsValid() && GameComps.StructureTemperatures.IsEnabled(handle))
		{
			GameComps.StructureTemperatures.Disable(handle);
			GameComps.StructureTemperatures.Enable(handle);
		}
		base.smi.StartSM();
	}

	public Notification CreateOverheatedNotification()
	{
		KSelectable component = GetComponent<KSelectable>();
		return new Notification(MISC.NOTIFICATIONS.BUILDINGOVERHEATED.NAME, NotificationType.BadMinor, HashedString.Invalid, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.BUILDINGOVERHEATED.TOOLTIP + notificationList.ReduceMessages(false), "/t• " + component.GetProperName(), false, 0f, null, null, null);
	}

	private static string ToolTipResolver(List<Notification> notificationList, object data)
	{
		string text = string.Empty;
		for (int i = 0; i < notificationList.Count; i++)
		{
			Notification notification = notificationList[i];
			text += (string)notification.tooltipData;
			if (i < notificationList.Count - 1)
			{
				text += "\n";
			}
		}
		return string.Format(MISC.NOTIFICATIONS.BUILDINGOVERHEATED.TOOLTIP, text);
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return GetDescriptors(def.BuildingComplete);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (overheatTemp != null && fatalTemp != null)
		{
			string formattedValue = overheatTemp.GetFormattedValue();
			string formattedValue2 = fatalTemp.GetFormattedValue();
			string str = UI.BUILDINGEFFECTS.TOOLTIPS.OVERHEAT_TEMP;
			str = str + "\n\n" + overheatTemp.GetAttributeValueTooltip();
			Descriptor item = new Descriptor(string.Format(UI.BUILDINGEFFECTS.OVERHEAT_TEMP, formattedValue, formattedValue2), string.Format(str, formattedValue, formattedValue2), Descriptor.DescriptorType.Effect, false);
			list.Add(item);
		}
		else if (baseOverheatTemp != 0f)
		{
			string formattedTemperature = GameUtil.GetFormattedTemperature(baseOverheatTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false);
			string formattedTemperature2 = GameUtil.GetFormattedTemperature(baseFatalTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false);
			string format = UI.BUILDINGEFFECTS.TOOLTIPS.OVERHEAT_TEMP;
			Descriptor item2 = new Descriptor(string.Format(UI.BUILDINGEFFECTS.OVERHEAT_TEMP, formattedTemperature, formattedTemperature2), string.Format(format, formattedTemperature, formattedTemperature2), Descriptor.DescriptorType.Effect, false);
			list.Add(item2);
		}
		return list;
	}
}
