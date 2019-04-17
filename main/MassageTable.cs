using Klei.AI;
using KSerialization;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class MassageTable : RelaxationPoint, IEffectDescriptor, IActivationRangeTarget
{
	[Serialize]
	private float activateValue = 50f;

	private static readonly string[] EffectsRemoved = new string[1]
	{
		"SoreBack"
	};

	private static readonly EventSystem.IntraObjectHandler<MassageTable> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<MassageTable>(delegate(MassageTable component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly Chore.Precondition IsStressAboveActivationRange = new Chore.Precondition
	{
		id = "IsStressAboveActivationRange",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_STRESS_ABOVE_ACTIVATION_RANGE,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			IActivationRangeTarget activationRangeTarget = (IActivationRangeTarget)data;
			AmountInstance amountInstance = Db.Get().Amounts.Stress.Lookup(context.consumerState.gameObject);
			float value = amountInstance.value;
			return value >= activationRangeTarget.ActivateValue;
		}
	};

	public string ActivateTooltip => BUILDINGS.PREFABS.MASSAGETABLE.ACTIVATE_TOOLTIP;

	public string DeactivateTooltip => BUILDINGS.PREFABS.MASSAGETABLE.DEACTIVATE_TOOLTIP;

	public float ActivateValue
	{
		get
		{
			return activateValue;
		}
		set
		{
			activateValue = value;
		}
	}

	public float DeactivateValue
	{
		get
		{
			return stopStressingValue;
		}
		set
		{
			stopStressingValue = value;
		}
	}

	public bool UseWholeNumbers => true;

	public float MinValue => 0f;

	public float MaxValue => 100f;

	public string ActivationRangeTitleText => UI.UISIDESCREENS.ACTIVATION_RANGE_SIDE_SCREEN.NAME;

	public string ActivateSliderLabelText => UI.UISIDESCREENS.ACTIVATION_RANGE_SIDE_SCREEN.ACTIVATE;

	public string DeactivateSliderLabelText => UI.UISIDESCREENS.ACTIVATION_RANGE_SIDE_SCREEN.DEACTIVATE;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		MassageTable component = gameObject.GetComponent<MassageTable>();
		if ((Object)component != (Object)null)
		{
			ActivateValue = component.ActivateValue;
			DeactivateValue = component.DeactivateValue;
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		Effects component = worker.GetComponent<Effects>();
		for (int i = 0; i < EffectsRemoved.Length; i++)
		{
			string effect_id = EffectsRemoved[i];
			component.Remove(effect_id);
		}
	}

	public new List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.STRESSREDUCEDPERMINUTE, GameUtil.GetFormattedPercent(stressModificationValue / 600f * 60f, GameUtil.TimeSlice.None)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.STRESSREDUCEDPERMINUTE, GameUtil.GetFormattedPercent(stressModificationValue / 600f * 60f, GameUtil.TimeSlice.None)), Descriptor.DescriptorType.Effect);
		list.Add(item);
		if (EffectsRemoved.Length > 0)
		{
			Descriptor item2 = default(Descriptor);
			item2.SetupDescriptor(UI.BUILDINGEFFECTS.REMOVESEFFECTSUBTITLE, UI.BUILDINGEFFECTS.TOOLTIPS.REMOVESEFFECTSUBTITLE, Descriptor.DescriptorType.Effect);
			list.Add(item2);
			for (int i = 0; i < EffectsRemoved.Length; i++)
			{
				string text = EffectsRemoved[i];
				string arg = Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + text.ToUpper() + ".NAME");
				string arg2 = Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + text.ToUpper() + ".CAUSE");
				Descriptor item3 = default(Descriptor);
				item3.IncreaseIndent();
				item3.SetupDescriptor("• " + string.Format(UI.BUILDINGEFFECTS.REMOVEDEFFECT, arg), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.REMOVEDEFFECT, arg2), Descriptor.DescriptorType.Effect);
				list.Add(item3);
			}
		}
		return list;
	}

	protected override WorkChore<RelaxationPoint> CreateWorkChore()
	{
		WorkChore<RelaxationPoint> workChore = new WorkChore<RelaxationPoint>(Db.Get().ChoreTypes.StressHeal, this, null, null, true, null, null, null, false, null, true, true, null, false, true, false, PriorityScreen.PriorityClass.high, 5, false, true);
		workChore.AddPrecondition(IsStressAboveActivationRange, this);
		return workChore;
	}
}
