using UnityEngine;

public class SealedDoorSideScreen : SideScreenContent
{
	[SerializeField]
	private LocText label;

	[SerializeField]
	private KButton button;

	[SerializeField]
	private Door target;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		button.onClick += delegate
		{
			target.OrderUnseal();
		};
		Refresh();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return (Object)target.GetComponent<Door>() != (Object)null;
	}

	public override void SetTarget(GameObject target)
	{
		Door component = target.GetComponent<Door>();
		if ((Object)component == (Object)null)
		{
			Debug.LogError("Target doesn't have a Door associated with it.");
		}
		else
		{
			this.target = component;
			Refresh();
		}
	}

	private void Refresh()
	{
		if (!target.isSealed)
		{
			ContentContainer.SetActive(false);
		}
		else
		{
			ContentContainer.SetActive(true);
		}
	}
}
