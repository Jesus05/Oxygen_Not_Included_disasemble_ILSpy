using UnityEngine;

public class SingleButtonSideScreen : SideScreenContent
{
	private ISidescreenButtonControl target;

	public LocText statusText;

	public KButton button;

	public LocText buttonLabel;

	protected override void OnPrefabInit()
	{
		button.onClick += OnButonClick;
	}

	private void OnButonClick()
	{
		if (target != null)
		{
			target.OnSidescreenButtonPressed();
			Refresh();
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<ISidescreenButtonControl>() != null;
	}

	public override void SetTarget(GameObject new_target)
	{
		if ((Object)new_target == (Object)null)
		{
			Debug.LogError("Invalid gameObject received");
		}
		else
		{
			target = new_target.GetComponent<ISidescreenButtonControl>();
			if (target == null)
			{
				DebugUtil.LogErrorArgs("The gameObject received does not contain a", typeof(ISidescreenButtonControl).ToString());
			}
			else
			{
				Refresh();
			}
		}
	}

	private void Refresh()
	{
		titleKey = target.SidescreenTitleKey;
		statusText.text = target.SidescreenStatusMessage;
		buttonLabel.text = target.SidescreenButtonText;
	}
}
