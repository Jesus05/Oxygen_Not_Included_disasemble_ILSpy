using KSerialization;
using System;
using System.Diagnostics;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[DebuggerDisplay("{name} {WattsUsed}W")]
public class EnergyConsumerSelfSustaining : EnergyConsumer
{
	private bool isSustained;

	private CircuitManager.ConnectionStatus connectionStatus;

	public override bool IsPowered
	{
		get
		{
			if (isSustained)
			{
				return true;
			}
			return connectionStatus == CircuitManager.ConnectionStatus.Powered;
		}
	}

	public bool IsExternallyPowered => connectionStatus == CircuitManager.ConnectionStatus.Powered;

	public event System.Action OnConnectionChanged;

	public void SetSustained(bool isSustained)
	{
		this.isSustained = isSustained;
	}

	public override void SetConnectionStatus(CircuitManager.ConnectionStatus connection_status)
	{
		CircuitManager.ConnectionStatus connectionStatus = this.connectionStatus;
		switch (connection_status)
		{
		case CircuitManager.ConnectionStatus.NotConnected:
			this.connectionStatus = CircuitManager.ConnectionStatus.NotConnected;
			break;
		case CircuitManager.ConnectionStatus.Unpowered:
			if (this.connectionStatus == CircuitManager.ConnectionStatus.Powered && (UnityEngine.Object)GetComponent<Battery>() == (UnityEngine.Object)null)
			{
				this.connectionStatus = CircuitManager.ConnectionStatus.Unpowered;
			}
			break;
		case CircuitManager.ConnectionStatus.Powered:
			if (this.connectionStatus != CircuitManager.ConnectionStatus.Powered)
			{
				this.connectionStatus = CircuitManager.ConnectionStatus.Powered;
			}
			break;
		}
		UpdatePoweredStatus();
		if (connectionStatus != this.connectionStatus && this.OnConnectionChanged != null)
		{
			this.OnConnectionChanged();
		}
	}

	public void UpdatePoweredStatus()
	{
		operational.SetFlag(EnergyConsumer.PoweredFlag, IsPowered);
	}
}
