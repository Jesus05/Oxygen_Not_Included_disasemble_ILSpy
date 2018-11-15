using FMOD.Studio;
using System;
using UnityEngine;

internal class LogicEventHandler : ILogicEventReceiver, ILogicUIElement, ILogicNetworkConnection, IUniformGridObject
{
	private int cell;

	private int value;

	private Action<int> onValueChanged;

	private Action<int, bool> onConnectionChanged;

	private LogicPortSpriteType spriteType;

	public int Value => value;

	public LogicEventHandler(int cell, Action<int> on_value_changed, Action<int, bool> on_connection_changed, LogicPortSpriteType sprite_type)
	{
		this.cell = cell;
		onValueChanged = on_value_changed;
		onConnectionChanged = on_connection_changed;
		spriteType = sprite_type;
	}

	public void ReceiveLogicEvent(int value)
	{
		TriggerAudio(value);
		this.value = value;
		onValueChanged(value);
	}

	public int GetLogicUICell()
	{
		return cell;
	}

	public LogicPortSpriteType GetLogicPortSpriteType()
	{
		return spriteType;
	}

	public Vector2 PosMin()
	{
		return Grid.CellToPos2D(cell);
	}

	public Vector2 PosMax()
	{
		return Grid.CellToPos2D(cell);
	}

	public int GetLogicCell()
	{
		return cell;
	}

	private void TriggerAudio(int new_value)
	{
		LogicCircuitNetwork networkForCell = Game.Instance.logicCircuitManager.GetNetworkForCell(cell);
		SpeedControlScreen instance = SpeedControlScreen.Instance;
		if (networkForCell != null && new_value != value && (UnityEngine.Object)instance != (UnityEngine.Object)null && !instance.IsPaused && (!KPlayerPrefs.HasKey(AudioOptionsScreen.AlwaysPlayAutomation) || KPlayerPrefs.GetInt(AudioOptionsScreen.AlwaysPlayAutomation) == 1 || OverlayScreen.Instance.GetMode() == SimViewMode.Logic))
		{
			string name = "Logic_Building_Toggle";
			if (CameraController.Instance.IsAudibleSound(Grid.CellToPosCCC(cell, Grid.SceneLayer.BuildingFront)))
			{
				EventInstance instance2 = KFMOD.BeginOneShot(GlobalAssets.GetSound(name, false), Grid.CellToPos(cell));
				instance2.setParameterValue("wireCount", (float)(networkForCell.Wires.Count % 24));
				instance2.setParameterValue("enabled", (float)new_value);
				KFMOD.EndOneShot(instance2);
			}
		}
	}

	public void OnLogicNetworkConnectionChanged(bool connected)
	{
		if (onConnectionChanged != null)
		{
			onConnectionChanged(cell, connected);
		}
	}
}
