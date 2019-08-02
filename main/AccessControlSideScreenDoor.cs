using STRINGS;
using System;

public class AccessControlSideScreenDoor : KMonoBehaviour
{
	public KToggle leftButton;

	public KToggle rightButton;

	private Action<MinionAssignablesProxy, AccessControl.Permission> permissionChangedCallback;

	private bool isUpDown;

	protected MinionAssignablesProxy targetIdentity;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		leftButton.onClick += OnPermissionButtonClicked;
		rightButton.onClick += OnPermissionButtonClicked;
	}

	private void OnPermissionButtonClicked()
	{
		AccessControl.Permission arg = leftButton.isOn ? ((!rightButton.isOn) ? AccessControl.Permission.GoLeft : AccessControl.Permission.Both) : ((!rightButton.isOn) ? AccessControl.Permission.Neither : AccessControl.Permission.GoRight);
		UpdateButtonStates(false);
		permissionChangedCallback(targetIdentity, arg);
	}

	protected virtual void UpdateButtonStates(bool isDefault)
	{
		ToolTip component = leftButton.GetComponent<ToolTip>();
		ToolTip component2 = rightButton.GetComponent<ToolTip>();
		if (isUpDown)
		{
			component.SetSimpleTooltip((!leftButton.isOn) ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_UP_DISABLED : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_UP_ENABLED);
			component2.SetSimpleTooltip((!rightButton.isOn) ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_DOWN_DISABLED : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_DOWN_ENABLED);
		}
		else
		{
			component.SetSimpleTooltip((!leftButton.isOn) ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_LEFT_DISABLED : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_LEFT_ENABLED);
			component2.SetSimpleTooltip((!rightButton.isOn) ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_RIGHT_DISABLED : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_RIGHT_ENABLED);
		}
	}

	public void SetRotated(bool rotated)
	{
		isUpDown = rotated;
	}

	public void SetContent(AccessControl.Permission permission, Action<MinionAssignablesProxy, AccessControl.Permission> onPermissionChange)
	{
		permissionChangedCallback = onPermissionChange;
		leftButton.isOn = (permission == AccessControl.Permission.Both || permission == AccessControl.Permission.GoLeft);
		rightButton.isOn = (permission == AccessControl.Permission.Both || permission == AccessControl.Permission.GoRight);
		UpdateButtonStates(false);
	}
}
