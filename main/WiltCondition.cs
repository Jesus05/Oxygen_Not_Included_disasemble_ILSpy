using KSerialization;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WiltCondition : KMonoBehaviour
{
	public enum Condition
	{
		Temperature,
		Pressure,
		AtmosphereElement,
		Drowning,
		Fertilized,
		DryingOut,
		Irrigation,
		IlluminationComfort,
		Darkness,
		Receptacle,
		Entombed,
		Count
	}

	[MyCmpGet]
	private Growing growing;

	[Serialize]
	private bool goingToWilt = false;

	[Serialize]
	private bool wilting = false;

	private Dictionary<int, bool> WiltConditions = new Dictionary<int, bool>();

	public float WiltDelay = 1f;

	public float RecoveryDelay = 1f;

	private SchedulerHandle wiltSchedulerHandler;

	private SchedulerHandle recoverSchedulerHandler;

	private static readonly EventSystem.IntraObjectHandler<WiltCondition> SetTemperatureFalseDelegate = new EventSystem.IntraObjectHandler<WiltCondition>(delegate(WiltCondition component, object data)
	{
		component.SetCondition(Condition.Temperature, false);
	});

	private static readonly EventSystem.IntraObjectHandler<WiltCondition> SetTemperatureTrueDelegate = new EventSystem.IntraObjectHandler<WiltCondition>(delegate(WiltCondition component, object data)
	{
		component.SetCondition(Condition.Temperature, true);
	});

	private static readonly EventSystem.IntraObjectHandler<WiltCondition> SetPressureFalseDelegate = new EventSystem.IntraObjectHandler<WiltCondition>(delegate(WiltCondition component, object data)
	{
		component.SetCondition(Condition.Pressure, false);
	});

	private static readonly EventSystem.IntraObjectHandler<WiltCondition> SetPressureTrueDelegate = new EventSystem.IntraObjectHandler<WiltCondition>(delegate(WiltCondition component, object data)
	{
		component.SetCondition(Condition.Pressure, true);
	});

	private static readonly EventSystem.IntraObjectHandler<WiltCondition> SetAtmosphereElementFalseDelegate = new EventSystem.IntraObjectHandler<WiltCondition>(delegate(WiltCondition component, object data)
	{
		component.SetCondition(Condition.AtmosphereElement, false);
	});

	private static readonly EventSystem.IntraObjectHandler<WiltCondition> SetAtmosphereElementTrueDelegate = new EventSystem.IntraObjectHandler<WiltCondition>(delegate(WiltCondition component, object data)
	{
		component.SetCondition(Condition.AtmosphereElement, true);
	});

	private static readonly EventSystem.IntraObjectHandler<WiltCondition> SetDrowningFalseDelegate = new EventSystem.IntraObjectHandler<WiltCondition>(delegate(WiltCondition component, object data)
	{
		component.SetCondition(Condition.Drowning, false);
	});

	private static readonly EventSystem.IntraObjectHandler<WiltCondition> SetDrowningTrueDelegate = new EventSystem.IntraObjectHandler<WiltCondition>(delegate(WiltCondition component, object data)
	{
		component.SetCondition(Condition.Drowning, true);
	});

	private static readonly EventSystem.IntraObjectHandler<WiltCondition> SetDryingOutFalseDelegate = new EventSystem.IntraObjectHandler<WiltCondition>(delegate(WiltCondition component, object data)
	{
		component.SetCondition(Condition.DryingOut, false);
	});

	private static readonly EventSystem.IntraObjectHandler<WiltCondition> SetDryingOutTrueDelegate = new EventSystem.IntraObjectHandler<WiltCondition>(delegate(WiltCondition component, object data)
	{
		component.SetCondition(Condition.DryingOut, true);
	});

	private static readonly EventSystem.IntraObjectHandler<WiltCondition> SetIrrigationFalseDelegate = new EventSystem.IntraObjectHandler<WiltCondition>(delegate(WiltCondition component, object data)
	{
		component.SetCondition(Condition.Irrigation, false);
	});

	private static readonly EventSystem.IntraObjectHandler<WiltCondition> SetIrrigationTrueDelegate = new EventSystem.IntraObjectHandler<WiltCondition>(delegate(WiltCondition component, object data)
	{
		component.SetCondition(Condition.Irrigation, true);
	});

	private static readonly EventSystem.IntraObjectHandler<WiltCondition> SetFertilizedFalseDelegate = new EventSystem.IntraObjectHandler<WiltCondition>(delegate(WiltCondition component, object data)
	{
		component.SetCondition(Condition.Fertilized, false);
	});

	private static readonly EventSystem.IntraObjectHandler<WiltCondition> SetFertilizedTrueDelegate = new EventSystem.IntraObjectHandler<WiltCondition>(delegate(WiltCondition component, object data)
	{
		component.SetCondition(Condition.Fertilized, true);
	});

	private static readonly EventSystem.IntraObjectHandler<WiltCondition> SetIlluminationComfortFalseDelegate = new EventSystem.IntraObjectHandler<WiltCondition>(delegate(WiltCondition component, object data)
	{
		component.SetCondition(Condition.IlluminationComfort, false);
	});

	private static readonly EventSystem.IntraObjectHandler<WiltCondition> SetIlluminationComfortTrueDelegate = new EventSystem.IntraObjectHandler<WiltCondition>(delegate(WiltCondition component, object data)
	{
		component.SetCondition(Condition.IlluminationComfort, true);
	});

	private static readonly EventSystem.IntraObjectHandler<WiltCondition> SetReceptacleFalseDelegate = new EventSystem.IntraObjectHandler<WiltCondition>(delegate(WiltCondition component, object data)
	{
		component.SetCondition(Condition.Receptacle, false);
	});

	private static readonly EventSystem.IntraObjectHandler<WiltCondition> SetReceptacleTrueDelegate = new EventSystem.IntraObjectHandler<WiltCondition>(delegate(WiltCondition component, object data)
	{
		component.SetCondition(Condition.Receptacle, true);
	});

	private static readonly EventSystem.IntraObjectHandler<WiltCondition> SetEntombedDelegate = new EventSystem.IntraObjectHandler<WiltCondition>(delegate(WiltCondition component, object data)
	{
		component.SetCondition(Condition.Entombed, !(bool)data);
	});

	[CompilerGenerated]
	private static Action<object> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Action<object> _003C_003Ef__mg_0024cache1;

	public bool IsWilting()
	{
		return wilting;
	}

	public List<Condition> CurrentWiltSources()
	{
		List<Condition> list = new List<Condition>();
		foreach (KeyValuePair<int, bool> wiltCondition in WiltConditions)
		{
			if (!wiltCondition.Value)
			{
				list.Add((Condition)wiltCondition.Key);
			}
		}
		return list;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		WiltConditions.Add(0, true);
		WiltConditions.Add(1, true);
		WiltConditions.Add(2, true);
		WiltConditions.Add(3, true);
		WiltConditions.Add(4, true);
		WiltConditions.Add(5, true);
		WiltConditions.Add(6, true);
		WiltConditions.Add(7, true);
		WiltConditions.Add(9, true);
		WiltConditions.Add(10, true);
		Subscribe(-107174716, SetTemperatureFalseDelegate);
		Subscribe(-1758196852, SetTemperatureFalseDelegate);
		Subscribe(-1234705021, SetTemperatureFalseDelegate);
		Subscribe(-55477301, SetTemperatureFalseDelegate);
		Subscribe(115888613, SetTemperatureTrueDelegate);
		Subscribe(-593125877, SetPressureFalseDelegate);
		Subscribe(-1175525437, SetPressureFalseDelegate);
		Subscribe(-907106982, SetPressureTrueDelegate);
		Subscribe(103243573, SetPressureFalseDelegate);
		Subscribe(646131325, SetPressureFalseDelegate);
		Subscribe(221594799, SetAtmosphereElementFalseDelegate);
		Subscribe(777259436, SetAtmosphereElementTrueDelegate);
		Subscribe(1949704522, SetDrowningFalseDelegate);
		Subscribe(99949694, SetDrowningTrueDelegate);
		Subscribe(-2057657673, SetDryingOutFalseDelegate);
		Subscribe(1555379996, SetDryingOutTrueDelegate);
		Subscribe(-370379773, SetIrrigationFalseDelegate);
		Subscribe(207387507, SetIrrigationTrueDelegate);
		Subscribe(-1073674739, SetFertilizedFalseDelegate);
		Subscribe(-1396791468, SetFertilizedTrueDelegate);
		Subscribe(1113102781, SetIlluminationComfortTrueDelegate);
		Subscribe(1387626797, SetIlluminationComfortFalseDelegate);
		Subscribe(1628751838, SetReceptacleTrueDelegate);
		Subscribe(960378201, SetReceptacleFalseDelegate);
		Subscribe(-1089732772, SetEntombedDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		CheckShouldWilt();
		if (wilting)
		{
			DoWilt();
			if (!goingToWilt)
			{
				goingToWilt = true;
				Recover();
			}
		}
		else
		{
			DoRecover();
			if (goingToWilt)
			{
				goingToWilt = false;
				Wilt();
			}
		}
	}

	protected override void OnCleanUp()
	{
		wiltSchedulerHandler.ClearScheduler();
		recoverSchedulerHandler.ClearScheduler();
		base.OnCleanUp();
	}

	private void SetCondition(Condition condition, bool satisfiedState)
	{
		if (WiltConditions.ContainsKey((int)condition))
		{
			WiltConditions[(int)condition] = satisfiedState;
			CheckShouldWilt();
		}
	}

	private void CheckShouldWilt()
	{
		bool flag = false;
		foreach (KeyValuePair<int, bool> wiltCondition in WiltConditions)
		{
			if (!wiltCondition.Value)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			if (!goingToWilt)
			{
				Wilt();
			}
		}
		else if (goingToWilt)
		{
			Recover();
		}
	}

	private void Wilt()
	{
		if (!goingToWilt)
		{
			goingToWilt = true;
			recoverSchedulerHandler.ClearScheduler();
			if (!wiltSchedulerHandler.IsValid)
			{
				wiltSchedulerHandler = GameScheduler.Instance.Schedule("Wilt", WiltDelay, DoWiltCallback, this, null);
			}
		}
	}

	private void Recover()
	{
		if (goingToWilt)
		{
			goingToWilt = false;
			wiltSchedulerHandler.ClearScheduler();
			if (!recoverSchedulerHandler.IsValid)
			{
				recoverSchedulerHandler = GameScheduler.Instance.Schedule("Recover", RecoveryDelay, DoRecoverCallback, this, null);
			}
		}
	}

	private static void DoWiltCallback(object data)
	{
		((WiltCondition)data).DoWilt();
	}

	private void DoWilt()
	{
		wiltSchedulerHandler.ClearScheduler();
		KSelectable component = GetComponent<KSelectable>();
		component.GetComponent<KPrefabID>().AddTag(GameTags.Wilting);
		if (!wilting)
		{
			wilting = true;
			Trigger(-724860998, null);
		}
		if ((UnityEngine.Object)growing != (UnityEngine.Object)null)
		{
			if (growing.Replanted)
			{
				component.AddStatusItem(Db.Get().CreatureStatusItems.WiltingDomestic, GetComponent<Growing>());
			}
			else
			{
				component.AddStatusItem(Db.Get().CreatureStatusItems.Wilting, GetComponent<Growing>());
			}
		}
		else
		{
			ReceptacleMonitor.StatesInstance sMI = component.GetSMI<ReceptacleMonitor.StatesInstance>();
			if (sMI != null && !sMI.IsInsideState(sMI.sm.wild))
			{
				component.AddStatusItem(Db.Get().CreatureStatusItems.WiltingNonGrowingDomestic, this);
			}
			else
			{
				component.AddStatusItem(Db.Get().CreatureStatusItems.WiltingNonGrowing, this);
			}
		}
	}

	public string WiltCausesString()
	{
		string text = "";
		List<IWiltCause> allSMI = this.GetAllSMI<IWiltCause>();
		allSMI.AddRange(GetComponents<IWiltCause>());
		foreach (IWiltCause item in allSMI)
		{
			Condition[] conditions = item.Conditions;
			foreach (Condition key in conditions)
			{
				if (WiltConditions.ContainsKey((int)key) && !WiltConditions[(int)key])
				{
					text += "\n";
					text += item.WiltStateString;
					break;
				}
			}
		}
		return text;
	}

	private static void DoRecoverCallback(object data)
	{
		((WiltCondition)data).DoRecover();
	}

	private void DoRecover()
	{
		recoverSchedulerHandler.ClearScheduler();
		KSelectable component = GetComponent<KSelectable>();
		wilting = false;
		component.RemoveStatusItem(Db.Get().CreatureStatusItems.WiltingDomestic, false);
		component.RemoveStatusItem(Db.Get().CreatureStatusItems.Wilting, false);
		component.RemoveStatusItem(Db.Get().CreatureStatusItems.WiltingNonGrowing, false);
		component.RemoveStatusItem(Db.Get().CreatureStatusItems.WiltingNonGrowingDomestic, false);
		component.GetComponent<KPrefabID>().RemoveTag(GameTags.Wilting);
		Trigger(712767498, null);
	}
}
