using KSerialization;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class JetSuitTank : KMonoBehaviour, IGameObjectEffectDescriptor
{
	[MyCmpGet]
	private ElementEmitter elementConverter;

	[Serialize]
	public float amount;

	public const float FUEL_CAPACITY = 25f;

	public const float FUEL_BURN_RATE = 0.1f;

	public const float CO2_EMITTED_PER_FUEL_BURNED = 3f;

	public const float EMIT_TEMPERATURE = 473.15f;

	public const float REFILL_PERCENT = 0.25f;

	private JetSuitMonitor.Instance jetSuitMonitor;

	private static readonly EventSystem.IntraObjectHandler<JetSuitTank> OnEquippedDelegate = new EventSystem.IntraObjectHandler<JetSuitTank>(delegate(JetSuitTank component, object data)
	{
		component.OnEquipped(data);
	});

	private static readonly EventSystem.IntraObjectHandler<JetSuitTank> OnUnequippedDelegate = new EventSystem.IntraObjectHandler<JetSuitTank>(delegate(JetSuitTank component, object data)
	{
		component.OnUnequipped(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		amount = 25f;
		Subscribe(-1617557748, OnEquippedDelegate);
		Subscribe(-170173755, OnUnequippedDelegate);
	}

	public float PercentFull()
	{
		return amount / 25f;
	}

	public bool IsEmpty()
	{
		return amount <= 0f;
	}

	public bool IsFull()
	{
		return PercentFull() >= 1f;
	}

	public bool NeedsRecharging()
	{
		return PercentFull() < 0.25f;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		string text = string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.EFFECTS.JETSUIT_TANK, GameUtil.GetFormattedMass(amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
		list.Add(new Descriptor(text, text, Descriptor.DescriptorType.Effect, false));
		return list;
	}

	private void OnEquipped(object data)
	{
		Equipment equipment = (Equipment)data;
		NameDisplayScreen.Instance.SetSuitFuelDisplay(equipment.GetComponent<MinionAssignablesProxy>().GetTargetGameObject(), PercentFull, true);
		jetSuitMonitor = new JetSuitMonitor.Instance(this, equipment.GetComponent<MinionAssignablesProxy>().GetTargetGameObject());
		jetSuitMonitor.StartSM();
		if (IsEmpty())
		{
			equipment.GetComponent<MinionAssignablesProxy>().GetTargetGameObject().AddTag(GameTags.JetSuitOutOfFuel);
		}
	}

	private void OnUnequipped(object data)
	{
		Equipment equipment = (Equipment)data;
		if (!equipment.destroyed)
		{
			equipment.GetComponent<MinionAssignablesProxy>().GetTargetGameObject().RemoveTag(GameTags.JetSuitOutOfFuel);
			NameDisplayScreen.Instance.SetSuitFuelDisplay(equipment.GetComponent<MinionAssignablesProxy>().GetTargetGameObject(), null, false);
			Navigator component = equipment.GetComponent<MinionAssignablesProxy>().GetTargetGameObject().GetComponent<Navigator>();
			if ((bool)component && component.CurrentNavType == NavType.Hover)
			{
				component.SetCurrentNavType(NavType.Floor);
			}
		}
		if (jetSuitMonitor != null)
		{
			jetSuitMonitor.StopSM("Removed jetsuit tank");
			jetSuitMonitor = null;
		}
	}
}
