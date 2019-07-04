using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicGate : LogicGateBase, ILogicEventSender, ILogicNetworkConnection
{
	public class LogicGateDescriptions
	{
		public class Description
		{
			public string name;

			public string active;

			public string inactive;
		}

		public Description inputOne;

		public Description inputTwo;

		public Description output;
	}

	private static readonly LogicGateDescriptions.Description INPUT_ONE_SINGLE_DESCRIPTION = new LogicGateDescriptions.Description
	{
		name = (string)UI.LOGIC_PORTS.GATE_SINGLE_INPUT_ONE_NAME,
		active = (string)UI.LOGIC_PORTS.GATE_SINGLE_INPUT_ONE_ACTIVE,
		inactive = (string)UI.LOGIC_PORTS.GATE_SINGLE_INPUT_ONE_INACTIVE
	};

	private static readonly LogicGateDescriptions.Description INPUT_ONE_DOUBLE_DESCRIPTION = new LogicGateDescriptions.Description
	{
		name = (string)UI.LOGIC_PORTS.GATE_DOUBLE_INPUT_ONE_NAME,
		active = (string)UI.LOGIC_PORTS.GATE_DOUBLE_INPUT_ONE_ACTIVE,
		inactive = (string)UI.LOGIC_PORTS.GATE_DOUBLE_INPUT_ONE_INACTIVE
	};

	private static readonly LogicGateDescriptions.Description INPUT_TWO_DESCRIPTION = new LogicGateDescriptions.Description
	{
		name = (string)UI.LOGIC_PORTS.GATE_DOUBLE_INPUT_TWO_NAME,
		active = (string)UI.LOGIC_PORTS.GATE_DOUBLE_INPUT_TWO_ACTIVE,
		inactive = (string)UI.LOGIC_PORTS.GATE_DOUBLE_INPUT_TWO_INACTIVE
	};

	private LogicGateDescriptions descriptions;

	private const bool IS_CIRCUIT_ENDPOINT = true;

	private bool connected = false;

	protected bool cleaningUp = false;

	[Serialize]
	protected int outputValue;

	private LogicEventHandler inputOne;

	private LogicEventHandler inputTwo;

	private LogicPortVisualizer output;

	private static readonly EventSystem.IntraObjectHandler<LogicGate> OnBuildingBrokenDelegate = new EventSystem.IntraObjectHandler<LogicGate>(delegate(LogicGate component, object data)
	{
		component.OnBuildingBroken(data);
	});

	private static readonly EventSystem.IntraObjectHandler<LogicGate> OnBuildingFullyRepairedDelegate = new EventSystem.IntraObjectHandler<LogicGate>(delegate(LogicGate component, object data)
	{
		component.OnBuildingFullyRepaired(data);
	});

	protected override void OnSpawn()
	{
		inputOne = new LogicEventHandler(base.InputCellOne, UpdateState, null, LogicPortSpriteType.Input);
		if (base.RequiresTwoInputs)
		{
			inputTwo = new LogicEventHandler(base.InputCellTwo, UpdateState, null, LogicPortSpriteType.Input);
		}
		Subscribe(774203113, OnBuildingBrokenDelegate);
		Subscribe(-1735440190, OnBuildingFullyRepairedDelegate);
		BuildingHP component = GetComponent<BuildingHP>();
		if ((Object)component == (Object)null || !component.IsBroken)
		{
			Connect();
		}
	}

	protected override void OnCleanUp()
	{
		cleaningUp = true;
		Disconnect();
		Unsubscribe(774203113, OnBuildingBrokenDelegate, false);
		Unsubscribe(-1735440190, OnBuildingFullyRepairedDelegate, false);
		base.OnCleanUp();
	}

	private void OnBuildingBroken(object data)
	{
		Disconnect();
	}

	private void OnBuildingFullyRepaired(object data)
	{
		Connect();
	}

	private void Connect()
	{
		if (!connected)
		{
			LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
			UtilityNetworkManager<LogicCircuitNetwork, LogicWire> logicCircuitSystem = Game.Instance.logicCircuitSystem;
			connected = true;
			int outputCell = base.OutputCell;
			logicCircuitSystem.AddToNetworks(outputCell, this, true);
			output = new LogicPortVisualizer(outputCell, LogicPortSpriteType.Output);
			logicCircuitManager.AddVisElem(output);
			int inputCellOne = base.InputCellOne;
			logicCircuitSystem.AddToNetworks(inputCellOne, inputOne, true);
			logicCircuitManager.AddVisElem(inputOne);
			if (base.RequiresTwoInputs)
			{
				int inputCellTwo = base.InputCellTwo;
				logicCircuitSystem.AddToNetworks(inputCellTwo, inputTwo, true);
				logicCircuitManager.AddVisElem(inputTwo);
			}
			RefreshAnimation();
		}
	}

	private void Disconnect()
	{
		if (connected)
		{
			LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
			UtilityNetworkManager<LogicCircuitNetwork, LogicWire> logicCircuitSystem = Game.Instance.logicCircuitSystem;
			connected = false;
			int outputCell = base.OutputCell;
			logicCircuitSystem.RemoveFromNetworks(outputCell, this, true);
			logicCircuitManager.RemoveVisElem(output);
			output = null;
			int inputCellOne = base.InputCellOne;
			logicCircuitSystem.RemoveFromNetworks(inputCellOne, inputOne, true);
			logicCircuitManager.RemoveVisElem(inputOne);
			inputOne = null;
			if (base.RequiresTwoInputs)
			{
				int inputCellTwo = base.InputCellTwo;
				logicCircuitSystem.RemoveFromNetworks(inputCellTwo, inputTwo, true);
				logicCircuitManager.RemoveVisElem(inputTwo);
				inputTwo = null;
			}
			RefreshAnimation();
		}
	}

	private void UpdateState(int new_value)
	{
		if (!cleaningUp)
		{
			int value = inputOne.Value;
			int num = (inputTwo != null) ? inputTwo.Value : 0;
			outputValue = 0;
			switch (op)
			{
			case Op.And:
				outputValue = (value & num);
				break;
			case Op.Or:
				outputValue = (value | num);
				break;
			case Op.Xor:
				outputValue = (value ^ num);
				break;
			case Op.Not:
				outputValue = ((value == 0) ? 1 : 0);
				break;
			case Op.CustomSingle:
				outputValue = GetCustomValue(value, num);
				break;
			}
			RefreshAnimation();
		}
	}

	public virtual void LogicTick()
	{
	}

	protected virtual int GetCustomValue(int val1, int val2)
	{
		return val1;
	}

	public int GetPortValue(PortId port)
	{
		switch (port)
		{
		case PortId.InputOne:
			return inputOne.Value;
		case PortId.InputTwo:
			return base.RequiresTwoInputs ? inputTwo.Value : 0;
		default:
			return outputValue;
		}
	}

	public bool GetPortConnected(PortId port)
	{
		if (port == PortId.InputTwo && !base.RequiresTwoInputs)
		{
			return false;
		}
		int cell = PortCell(port);
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		LogicCircuitNetwork networkForCell = logicCircuitManager.GetNetworkForCell(cell);
		return networkForCell != null;
	}

	public void SetPortDescriptions(LogicGateDescriptions descriptions)
	{
		this.descriptions = descriptions;
	}

	public LogicGateDescriptions.Description GetPortDescription(PortId port)
	{
		switch (port)
		{
		case PortId.InputOne:
			return (descriptions.inputOne != null) ? descriptions.inputOne : ((!base.RequiresTwoInputs) ? INPUT_ONE_SINGLE_DESCRIPTION : INPUT_ONE_DOUBLE_DESCRIPTION);
		case PortId.InputTwo:
			return (descriptions.inputTwo == null) ? INPUT_TWO_DESCRIPTION : descriptions.inputTwo;
		default:
			return descriptions.output;
		}
	}

	public int GetLogicValue()
	{
		return outputValue;
	}

	public int GetLogicCell()
	{
		return GetLogicUICell();
	}

	public int GetLogicUICell()
	{
		return base.OutputCell;
	}

	public bool IsLogicInput()
	{
		return false;
	}

	protected void RefreshAnimation()
	{
		if (!cleaningUp)
		{
			KBatchedAnimController component = GetComponent<KBatchedAnimController>();
			int outputCell = base.OutputCell;
			LogicCircuitNetwork logicCircuitNetwork = Game.Instance.logicCircuitSystem.GetNetworkForCell(outputCell) as LogicCircuitNetwork;
			if (logicCircuitNetwork == null)
			{
				component.Play("off", KAnim.PlayMode.Once, 1f, 0f);
			}
			else if (base.RequiresTwoInputs)
			{
				component.Play("on_" + (inputOne.Value + inputTwo.Value * 2 + outputValue * 4).ToString(), KAnim.PlayMode.Once, 1f, 0f);
			}
			else
			{
				component.Play("on_" + (inputOne.Value + outputValue * 4).ToString(), KAnim.PlayMode.Once, 1f, 0f);
			}
		}
	}

	public void OnLogicNetworkConnectionChanged(bool connected)
	{
	}
}
