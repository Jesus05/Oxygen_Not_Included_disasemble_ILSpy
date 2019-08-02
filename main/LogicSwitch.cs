using KSerialization;
using System;
using System.Collections;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicSwitch : Switch
{
	public static readonly HashedString PORT_ID = "LogicSwitch";

	private System.Action firstFrameCallback;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		UpdateVisualization();
		UpdateLogicCircuit();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	protected override void Toggle()
	{
		base.Toggle();
		UpdateVisualization();
		UpdateLogicCircuit();
	}

	private void UpdateVisualization()
	{
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		component.Play((!switchedOn) ? "on_pst" : "on_pre", KAnim.PlayMode.Once, 1f, 0f);
		component.Queue((!switchedOn) ? "off" : "on", KAnim.PlayMode.Once, 1f, 0f);
	}

	private void UpdateLogicCircuit()
	{
		LogicPorts component = GetComponent<LogicPorts>();
		component.SendSignal(PORT_ID, switchedOn ? 1 : 0);
	}

	protected override void UpdateSwitchStatus()
	{
		StatusItem status_item = (!switchedOn) ? Db.Get().BuildingStatusItems.LogicSwitchStatusInactive : Db.Get().BuildingStatusItems.LogicSwitchStatusActive;
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item, null);
	}

	public void SetFirstFrameCallback(System.Action ffCb)
	{
		firstFrameCallback = ffCb;
		StartCoroutine(RunCallback());
	}

	private IEnumerator RunCallback()
	{
		yield return (object)null;
		/*Error: Unable to find new state assignment for yield return*/;
	}
}
