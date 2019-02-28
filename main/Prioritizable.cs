using KSerialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Prioritizable : KMonoBehaviour
{
	[SerializeField]
	[Serialize]
	private int masterPriority = -2147483648;

	[SerializeField]
	[Serialize]
	private PrioritySetting masterPrioritySetting = new PrioritySetting(PriorityScreen.PriorityClass.basic, 5);

	public Action<PrioritySetting> onPriorityChanged;

	public bool showIcon = true;

	public Vector2 iconOffset;

	public float iconScale = 1f;

	[SerializeField]
	private int refCount;

	private static readonly EventSystem.IntraObjectHandler<Prioritizable> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<Prioritizable>(delegate(Prioritizable component, object data)
	{
		component.OnCopySettings(data);
	});

	private static Dictionary<PrioritySetting, PrioritySetting> conversions = new Dictionary<PrioritySetting, PrioritySetting>
	{
		{
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 1),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 4)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 2),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 5)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 3),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 6)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 4),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 7)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 5),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 8)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.high, 1),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 6)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.high, 2),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 7)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.high, 3),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 8)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.high, 4),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 9)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.high, 5),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 9)
		}
	};

	private Guid highPriorityStatusItem;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		Prioritizable component = gameObject.GetComponent<Prioritizable>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			SetMasterPriority(component.GetMasterPriority());
		}
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (masterPriority != -2147483648)
		{
			masterPrioritySetting = new PrioritySetting(PriorityScreen.PriorityClass.basic, 5);
			masterPriority = -2147483648;
		}
		if (SaveLoader.Instance.GameInfo.IsVersionExactly(7, 2) && conversions.TryGetValue(masterPrioritySetting, out PrioritySetting value))
		{
			masterPrioritySetting = value;
		}
	}

	protected override void OnSpawn()
	{
		if (onPriorityChanged != null)
		{
			onPriorityChanged(masterPrioritySetting);
		}
		RefreshHighPriorityNotification();
		Components.Prioritizables.Add(this);
	}

	public PrioritySetting GetMasterPriority()
	{
		return masterPrioritySetting;
	}

	public void SetMasterPriority(PrioritySetting priority)
	{
		if (!priority.Equals(masterPrioritySetting))
		{
			masterPrioritySetting = priority;
			if (onPriorityChanged != null)
			{
				onPriorityChanged(masterPrioritySetting);
			}
			RefreshHighPriorityNotification();
		}
	}

	public void AddRef()
	{
		refCount++;
		RefreshHighPriorityNotification();
	}

	public void RemoveRef()
	{
		refCount--;
		RefreshHighPriorityNotification();
	}

	public bool IsPrioritizable()
	{
		return refCount > 0;
	}

	public bool IsEmergency()
	{
		return masterPrioritySetting.priority_class == PriorityScreen.PriorityClass.emergency && IsPrioritizable();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Prioritizables.Remove(this);
	}

	public static void AddRef(GameObject go)
	{
		Prioritizable component = go.GetComponent<Prioritizable>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.AddRef();
		}
	}

	public static void RemoveRef(GameObject go)
	{
		Prioritizable component = go.GetComponent<Prioritizable>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.RemoveRef();
		}
	}

	private void RefreshHighPriorityNotification()
	{
		bool flag = masterPrioritySetting.priority_class == PriorityScreen.PriorityClass.emergency && IsPrioritizable();
		if (flag && highPriorityStatusItem == Guid.Empty)
		{
			highPriorityStatusItem = GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.EmergencyPriority, null);
		}
		else if (!flag && highPriorityStatusItem != Guid.Empty)
		{
			highPriorityStatusItem = GetComponent<KSelectable>().RemoveStatusItem(highPriorityStatusItem, false);
		}
		if ((UnityEngine.Object)GlobalChoreProvider.Instance != (UnityEngine.Object)null)
		{
			GlobalChoreProvider.Instance.RefreshEmergencyChoreStatus();
		}
	}
}
