using STRINGS;
using System;
using UnityEngine;

public class AccessControlSideScreenRow : AccessControlSideScreenDoor
{
	[SerializeField]
	private CrewPortrait crewPortraitPrefab;

	private CrewPortrait portraitInstance;

	public KToggle defaultButton;

	public GameObject defaultControls;

	public GameObject customControls;

	private Action<MinionAssignablesProxy, bool> defaultClickedCallback;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		defaultButton.onValueChanged += OnDefaultButtonChanged;
	}

	private void OnDefaultButtonChanged(bool state)
	{
		UpdateButtonStates(!state);
		if (defaultClickedCallback != null)
		{
			defaultClickedCallback(targetIdentity, !state);
		}
	}

	protected override void UpdateButtonStates(bool isDefault)
	{
		base.UpdateButtonStates(isDefault);
		ToolTip component = defaultButton.GetComponent<ToolTip>();
		component.SetSimpleTooltip((!isDefault) ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.SET_TO_DEFAULT : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.SET_TO_CUSTOM);
		defaultControls.SetActive(isDefault);
		customControls.SetActive(!isDefault);
	}

	public void SetMinionContent(MinionAssignablesProxy identity, AccessControl.Permission permission, bool isDefault, Action<MinionAssignablesProxy, AccessControl.Permission> onPermissionChange, Action<MinionAssignablesProxy, bool> onDefaultClick)
	{
		SetContent(permission, onPermissionChange);
		if ((UnityEngine.Object)identity == (UnityEngine.Object)null)
		{
			Debug.LogError("Invalid data received.");
		}
		else
		{
			if ((UnityEngine.Object)portraitInstance == (UnityEngine.Object)null)
			{
				portraitInstance = Util.KInstantiateUI<CrewPortrait>(crewPortraitPrefab.gameObject, defaultButton.gameObject, false);
				portraitInstance.SetAlpha(1f);
			}
			targetIdentity = identity;
			portraitInstance.SetIdentityObject(identity, false);
			portraitInstance.SetSubTitle((!isDefault) ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.USING_CUSTOM : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.USING_DEFAULT);
			defaultClickedCallback = null;
			defaultButton.isOn = !isDefault;
			defaultClickedCallback = onDefaultClick;
		}
	}
}
