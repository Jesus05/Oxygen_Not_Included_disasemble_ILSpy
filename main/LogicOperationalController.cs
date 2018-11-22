using STRINGS;
using System;
using System.Runtime.CompilerServices;

public class LogicOperationalController : KMonoBehaviour
{
	public static readonly HashedString PORT_ID = "LogicOperational";

	public int unNetworkedValue = 1;

	private static readonly Operational.Flag logicOperationalFlag = new Operational.Flag("LogicOperational", Operational.Flag.Type.Requirement);

	private static StatusItem infoStatusItem;

	public static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[1]
	{
		LogicPorts.Port.InputPort(PORT_ID, new CellOffset(0, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false)
	};

	private static readonly EventSystem.IntraObjectHandler<LogicOperationalController> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<LogicOperationalController>(delegate(LogicOperationalController component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	[CompilerGenerated]
	private static Func<string, object, string> _003C_003Ef__mg_0024cache0;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-801688580, OnLogicValueChangedDelegate);
		if (infoStatusItem == null)
		{
			infoStatusItem = new StatusItem("LogicOperationalInfo", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
			infoStatusItem.resolveStringCallback = ResolveInfoStatusItemString;
		}
		CheckWireState();
	}

	private LogicCircuitNetwork GetNetwork()
	{
		LogicPorts component = GetComponent<LogicPorts>();
		int portCell = component.GetPortCell(PORT_ID);
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		return logicCircuitManager.GetNetworkForCell(portCell);
	}

	private LogicCircuitNetwork CheckWireState()
	{
		LogicCircuitNetwork network = GetNetwork();
		int num = network?.OutputValue ?? unNetworkedValue;
		GetComponent<Operational>().SetFlag(logicOperationalFlag, num > 0);
		return network;
	}

	private static string ResolveInfoStatusItemString(string format_str, object data)
	{
		LogicOperationalController logicOperationalController = (LogicOperationalController)data;
		Operational component = logicOperationalController.GetComponent<Operational>();
		return (!component.GetFlag(logicOperationalFlag)) ? BUILDING.STATUSITEMS.LOGIC.LOGIC_CONTROLLED_DISABLED : BUILDING.STATUSITEMS.LOGIC.LOGIC_CONTROLLED_ENABLED;
	}

	private void OnLogicValueChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (logicValueChanged.portID == PORT_ID)
		{
			LogicCircuitNetwork logicCircuitNetwork = CheckWireState();
			GetComponent<KSelectable>().ToggleStatusItem(infoStatusItem, logicCircuitNetwork != null, this);
		}
	}
}
