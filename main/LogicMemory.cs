using KSerialization;
using STRINGS;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicMemory : KMonoBehaviour
{
	[MyCmpGet]
	private LogicPorts ports;

	[Serialize]
	private int value;

	private static StatusItem infoStatusItem;

	public static readonly HashedString READ_PORT_ID = new HashedString("LogicMemoryRead");

	public static readonly HashedString SET_PORT_ID = new HashedString("LogicMemorySet");

	public static readonly HashedString RESET_PORT_ID = new HashedString("LogicMemoryReset");

	private static readonly EventSystem.IntraObjectHandler<LogicMemory> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<LogicMemory>(delegate(LogicMemory component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	[CompilerGenerated]
	private static Func<string, object, string> _003C_003Ef__mg_0024cache0;

	protected override void OnSpawn()
	{
		if (infoStatusItem == null)
		{
			infoStatusItem = new StatusItem("StoredValue", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			infoStatusItem.resolveStringCallback = ResolveInfoStatusItemString;
		}
		Subscribe(-801688580, OnLogicValueChangedDelegate);
	}

	public void OnLogicValueChanged(object data)
	{
		if (!((UnityEngine.Object)ports == (UnityEngine.Object)null) && !((UnityEngine.Object)base.gameObject == (UnityEngine.Object)null) && !((UnityEngine.Object)this == (UnityEngine.Object)null))
		{
			LogicValueChanged logicValueChanged = (LogicValueChanged)data;
			if (logicValueChanged.portID != READ_PORT_ID)
			{
				int inputValue = ports.GetInputValue(SET_PORT_ID);
				int inputValue2 = ports.GetInputValue(RESET_PORT_ID);
				int num = value;
				if (inputValue2 == 1)
				{
					num = 0;
				}
				else if (inputValue == 1)
				{
					num = 1;
				}
				if (num != value)
				{
					value = num;
					ports.SendSignal(READ_PORT_ID, value);
					KBatchedAnimController component = GetComponent<KBatchedAnimController>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null)
					{
						component.Play((num == 0) ? "off" : "on", KAnim.PlayMode.Once, 1f, 0f);
					}
				}
			}
		}
	}

	private static string ResolveInfoStatusItemString(string format_str, object data)
	{
		LogicMemory logicMemory = (LogicMemory)data;
		int outputValue = logicMemory.ports.GetOutputValue(READ_PORT_ID);
		return string.Format(BUILDINGS.PREFABS.LOGICMEMORY.STATUS_ITEM_VALUE, outputValue);
	}
}
