using STRINGS;
using UnityEngine;

public class TelepadSideScreen : SideScreenContent
{
	[SerializeField]
	private LocText timeLabel;

	[SerializeField]
	private KButton viewImmigrantsBtn;

	[SerializeField]
	private Telepad targetTelepad;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		viewImmigrantsBtn.onClick += delegate
		{
			ImmigrantScreen.InitializeImmigrantScreen(targetTelepad);
			Game.Instance.Trigger(288942073, null);
		};
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return (Object)target.GetComponent<Telepad>() != (Object)null;
	}

	public override void SetTarget(GameObject target)
	{
		Telepad component = target.GetComponent<Telepad>();
		if ((Object)component == (Object)null)
		{
			Debug.LogError("Target doesn't have a telepad associated with it.", null);
		}
		else
		{
			targetTelepad = component;
			if ((Object)targetTelepad != (Object)null)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				base.gameObject.SetActive(true);
			}
		}
	}

	private void Update()
	{
		if ((Object)targetTelepad != (Object)null)
		{
			if ((Object)GameFlowManager.Instance != (Object)null && GameFlowManager.Instance.IsGameOver())
			{
				base.gameObject.SetActive(false);
				timeLabel.text = UI.UISIDESCREENS.TELEPADSIDESCREEN.GAMEOVER;
				SetContentState(true);
			}
			else
			{
				if (targetTelepad.GetComponent<Operational>().IsOperational)
				{
					timeLabel.text = string.Format(UI.UISIDESCREENS.TELEPADSIDESCREEN.NEXTPRODUCTION, GameUtil.GetFormattedCycles(targetTelepad.GetTimeRemaining(), "F1"));
				}
				else
				{
					base.gameObject.SetActive(false);
				}
				SetContentState(!Immigration.Instance.ImmigrantsAvailable);
			}
		}
	}

	private void SetContentState(bool isLabel)
	{
		if (timeLabel.gameObject.activeInHierarchy != isLabel)
		{
			timeLabel.gameObject.SetActive(isLabel);
		}
		if (viewImmigrantsBtn.gameObject.activeInHierarchy == isLabel)
		{
			viewImmigrantsBtn.gameObject.SetActive(!isLabel);
		}
	}
}
