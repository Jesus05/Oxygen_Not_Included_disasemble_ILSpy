using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class DoorToggleSideScreen : SideScreenContent
{
	private struct DoorButtonInfo
	{
		public KToggle button;

		public Door.ControlState state;

		public string currentString;

		public string pendingString;
	}

	[SerializeField]
	private KToggle openButton;

	[SerializeField]
	private KToggle autoButton;

	[SerializeField]
	private KToggle closeButton;

	[SerializeField]
	private LocText description;

	private Door target;

	private AccessControl accessTarget;

	private List<DoorButtonInfo> buttonList = new List<DoorButtonInfo>();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		InitButtons();
	}

	private void InitButtons()
	{
		buttonList.Add(new DoorButtonInfo
		{
			button = openButton,
			state = Door.ControlState.Opened,
			currentString = (string)UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.OPEN,
			pendingString = (string)UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.OPEN_PENDING
		});
		buttonList.Add(new DoorButtonInfo
		{
			button = autoButton,
			state = Door.ControlState.Auto,
			currentString = (string)UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.AUTO,
			pendingString = (string)UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.AUTO_PENDING
		});
		buttonList.Add(new DoorButtonInfo
		{
			button = closeButton,
			state = Door.ControlState.Locked,
			currentString = (string)UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.CLOSE,
			pendingString = (string)UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.CLOSE_PENDING
		});
		foreach (DoorButtonInfo button in buttonList)
		{
			DoorButtonInfo info = button;
			info.button.onClick += delegate
			{
				target.QueueStateChange(info.state);
				Refresh();
			};
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return (Object)target.GetComponent<Door>() != (Object)null;
	}

	public override void SetTarget(GameObject target)
	{
		if ((Object)this.target != (Object)null)
		{
			ClearTarget();
		}
		base.SetTarget(target);
		this.target = target.GetComponent<Door>();
		accessTarget = target.GetComponent<AccessControl>();
		if (!((Object)this.target == (Object)null))
		{
			target.Subscribe(1734268753, OnDoorStateChanged);
			target.Subscribe(-1525636549, OnAccessControlChanged);
			Refresh();
			base.gameObject.SetActive(true);
		}
	}

	public override void ClearTarget()
	{
		if ((Object)target != (Object)null)
		{
			target.Unsubscribe(1734268753, OnDoorStateChanged);
			target.Unsubscribe(-1525636549, OnAccessControlChanged);
		}
		target = null;
	}

	private void Refresh()
	{
		string text = null;
		string text2 = null;
		if (buttonList == null || buttonList.Count == 0)
		{
			InitButtons();
		}
		foreach (DoorButtonInfo button in buttonList)
		{
			DoorButtonInfo current = button;
			if (target.CurrentState == current.state && target.RequestedState == current.state)
			{
				current.button.GetComponent<ImageToggleStateThrobber>().enabled = false;
				current.button.isOn = true;
				KImage[] componentsInChildren = current.button.GetComponentsInChildren<KImage>();
				foreach (KImage kImage in componentsInChildren)
				{
					kImage.ColorState = KImage.ColorSelector.Active;
				}
				ImageToggleState[] componentsInChildren2 = current.button.GetComponentsInChildren<ImageToggleState>();
				foreach (ImageToggleState imageToggleState in componentsInChildren2)
				{
					imageToggleState.SetActive();
					imageToggleState.SetActive();
				}
				text = current.currentString;
			}
			else if (target.RequestedState == current.state)
			{
				current.button.GetComponent<ImageToggleStateThrobber>().enabled = true;
				current.button.isOn = true;
				text2 = current.pendingString;
				KImage[] componentsInChildren3 = current.button.GetComponentsInChildren<KImage>();
				foreach (KImage kImage2 in componentsInChildren3)
				{
					kImage2.ColorState = KImage.ColorSelector.Active;
				}
			}
			else
			{
				current.button.GetComponent<ImageToggleStateThrobber>().enabled = false;
				KImage[] componentsInChildren4 = current.button.GetComponentsInChildren<KImage>();
				foreach (KImage kImage3 in componentsInChildren4)
				{
					kImage3.ColorState = KImage.ColorSelector.Inactive;
				}
				current.button.isOn = false;
				ImageToggleState[] componentsInChildren5 = current.button.GetComponentsInChildren<ImageToggleState>();
				foreach (ImageToggleState imageToggleState2 in componentsInChildren5)
				{
					imageToggleState2.SetInactive();
					imageToggleState2.SetInactive();
				}
			}
		}
		string text3 = text;
		if (text2 != null)
		{
			text3 = string.Format(UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.PENDING_FORMAT, text3, text2);
		}
		if ((Object)accessTarget != (Object)null && !accessTarget.Online)
		{
			text3 = string.Format(UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.ACCESS_FORMAT, text3, UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.ACCESS_OFFLINE);
		}
		if (target.building.Def.PrefabID == POIDoorInternalConfig.ID)
		{
			text3 = UI.UISIDESCREENS.DOOR_TOGGLE_SIDE_SCREEN.POI_INTERNAL;
			foreach (DoorButtonInfo button2 in buttonList)
			{
				DoorButtonInfo current2 = button2;
				current2.button.gameObject.SetActive(false);
			}
		}
		else
		{
			foreach (DoorButtonInfo button3 in buttonList)
			{
				DoorButtonInfo current3 = button3;
				bool active = current3.state != 0 || target.allowAutoControl;
				current3.button.gameObject.SetActive(active);
			}
		}
		description.text = text3;
		description.gameObject.SetActive(!string.IsNullOrEmpty(text3));
		ContentContainer.SetActive(!target.isSealed);
	}

	private void OnDoorStateChanged(object data)
	{
		Refresh();
	}

	private void OnAccessControlChanged(object data)
	{
		Refresh();
	}
}
