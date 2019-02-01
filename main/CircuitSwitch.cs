using KSerialization;
using System;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class CircuitSwitch : Switch
{
	[SerializeField]
	public ObjectLayer objectLayer;

	private Wire attachedWire = null;

	private Guid wireConnectedGUID;

	private bool wasOn;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.OnToggle += CircuitOnToggle;
		int cell = Grid.PosToCell(base.transform.GetPosition());
		GameObject gameObject = Grid.Objects[cell, (int)objectLayer];
		Wire wire = (!((UnityEngine.Object)gameObject != (UnityEngine.Object)null)) ? null : gameObject.GetComponent<Wire>();
		if ((UnityEngine.Object)wire == (UnityEngine.Object)null)
		{
			wireConnectedGUID = GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoWireConnected, null);
		}
		AttachWire(wire);
		wasOn = switchedOn;
		UpdateCircuit(true);
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		component.Play((!switchedOn) ? "off" : "on", KAnim.PlayMode.Once, 1f, 0f);
	}

	protected override void OnCleanUp()
	{
		if ((UnityEngine.Object)attachedWire != (UnityEngine.Object)null)
		{
			UnsubscribeFromWire(attachedWire);
		}
		bool switchedOn = base.switchedOn;
		base.switchedOn = true;
		UpdateCircuit(false);
		base.switchedOn = switchedOn;
	}

	public bool IsConnected()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		GameObject gameObject = Grid.Objects[cell, (int)objectLayer];
		return (UnityEngine.Object)gameObject != (UnityEngine.Object)null && gameObject.GetComponent<IDisconnectable>() != null;
	}

	private void CircuitOnToggle(bool on)
	{
		UpdateCircuit(true);
	}

	public void AttachWire(Wire wire)
	{
		if (!((UnityEngine.Object)wire == (UnityEngine.Object)attachedWire))
		{
			if ((UnityEngine.Object)attachedWire != (UnityEngine.Object)null)
			{
				UnsubscribeFromWire(attachedWire);
			}
			attachedWire = wire;
			if ((UnityEngine.Object)attachedWire != (UnityEngine.Object)null)
			{
				SubscribeToWire(attachedWire);
				UpdateCircuit(true);
				wireConnectedGUID = GetComponent<KSelectable>().RemoveStatusItem(wireConnectedGUID, false);
			}
			else if (wireConnectedGUID == Guid.Empty)
			{
				wireConnectedGUID = GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoWireConnected, null);
			}
		}
	}

	private void OnWireDestroyed(object data)
	{
		if ((UnityEngine.Object)attachedWire != (UnityEngine.Object)null)
		{
			attachedWire.Unsubscribe(1969584890, OnWireDestroyed);
		}
	}

	private void OnWireStateChanged(object data)
	{
		UpdateCircuit(true);
	}

	private void SubscribeToWire(Wire wire)
	{
		wire.Subscribe(1969584890, OnWireDestroyed);
		wire.Subscribe(-1735440190, OnWireStateChanged);
		wire.Subscribe(774203113, OnWireStateChanged);
	}

	private void UnsubscribeFromWire(Wire wire)
	{
		wire.Unsubscribe(1969584890, OnWireDestroyed);
		wire.Unsubscribe(-1735440190, OnWireStateChanged);
		wire.Unsubscribe(774203113, OnWireStateChanged);
	}

	private void UpdateCircuit(bool should_update_anim = true)
	{
		if ((UnityEngine.Object)attachedWire != (UnityEngine.Object)null)
		{
			if (switchedOn)
			{
				attachedWire.Connect();
			}
			else
			{
				attachedWire.Disconnect();
			}
		}
		if (should_update_anim && wasOn != switchedOn)
		{
			KBatchedAnimController component = GetComponent<KBatchedAnimController>();
			component.Play((!switchedOn) ? "on_pst" : "on_pre", KAnim.PlayMode.Once, 1f, 0f);
			component.Queue((!switchedOn) ? "off" : "on", KAnim.PlayMode.Once, 1f, 0f);
			Game.Instance.userMenu.Refresh(base.gameObject);
		}
		wasOn = switchedOn;
	}
}
