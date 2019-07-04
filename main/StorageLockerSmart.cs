#define DEBUG
using System.Diagnostics;

public class StorageLockerSmart : StorageLocker
{
	[MyCmpGet]
	private LogicPorts ports;

	[MyCmpGet]
	private Operational operational;

	private static readonly EventSystem.IntraObjectHandler<StorageLockerSmart> UpdateLogicCircuitCBDelegate = new EventSystem.IntraObjectHandler<StorageLockerSmart>(delegate(StorageLockerSmart component, object data)
	{
		component.UpdateLogicCircuitCB(data);
	});

	public override float UserMaxCapacity
	{
		get
		{
			return base.UserMaxCapacity;
		}
		set
		{
			base.UserMaxCapacity = value;
			UpdateLogicAndActiveState();
		}
	}

	protected override void OnPrefabInit()
	{
		Initialize(true);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ports = base.gameObject.GetComponent<LogicPorts>();
		Subscribe(-1697596308, UpdateLogicCircuitCBDelegate);
		Subscribe(-592767678, UpdateLogicCircuitCBDelegate);
		UpdateLogicAndActiveState();
	}

	private void UpdateLogicCircuitCB(object data)
	{
		UpdateLogicAndActiveState();
	}

	private void UpdateLogicAndActiveState()
	{
		System.Diagnostics.Debug.WriteLine("UpdateLogicAndActiveState");
		bool flag = filteredStorage.IsFull();
		bool isOperational = operational.IsOperational;
		bool flag2 = flag && isOperational;
		ports.SendSignal(FilteredStorage.FULL_PORT_ID, flag2 ? 1 : 0);
		filteredStorage.SetLogicMeter(flag2);
		operational.SetActive(isOperational, false);
	}
}
