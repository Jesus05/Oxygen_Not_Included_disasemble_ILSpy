using STRINGS;
using UnityEngine;

public class GeneShufflerSideScreen : SideScreenContent
{
	[SerializeField]
	private LocText label;

	[SerializeField]
	private KButton button;

	[SerializeField]
	private LocText buttonLabel;

	[SerializeField]
	private GeneShuffler target;

	[SerializeField]
	private GameObject contents;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		button.onClick += OnButtonClick;
		Refresh();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return (Object)target.GetComponent<GeneShuffler>() != (Object)null;
	}

	public override void SetTarget(GameObject target)
	{
		GeneShuffler component = target.GetComponent<GeneShuffler>();
		if ((Object)component == (Object)null)
		{
			Debug.LogError("Target doesn't have a GeneShuffler associated with it.", null);
		}
		else
		{
			this.target = component;
			Refresh();
		}
	}

	private void OnButtonClick()
	{
		if (target.WorkComplete)
		{
			target.SetWorkTime(0f);
		}
		else if (target.IsConsumed)
		{
			target.RequestRecharge(!target.RechargeRequested);
			Refresh();
		}
	}

	private void Refresh()
	{
		if ((Object)target != (Object)null)
		{
			if (target.WorkComplete)
			{
				contents.SetActive(true);
				label.text = UI.UISIDESCREENS.GENESHUFFLERSIDESREEN.COMPLETE;
				button.gameObject.SetActive(true);
				buttonLabel.text = UI.UISIDESCREENS.GENESHUFFLERSIDESREEN.BUTTON;
			}
			else if (target.IsConsumed)
			{
				contents.SetActive(true);
				button.gameObject.SetActive(true);
				if (target.RechargeRequested)
				{
					label.text = UI.UISIDESCREENS.GENESHUFFLERSIDESREEN.CONSUMED_WAITING;
					buttonLabel.text = UI.UISIDESCREENS.GENESHUFFLERSIDESREEN.BUTTON_RECHARGE_CANCEL;
				}
				else
				{
					label.text = UI.UISIDESCREENS.GENESHUFFLERSIDESREEN.CONSUMED;
					buttonLabel.text = UI.UISIDESCREENS.GENESHUFFLERSIDESREEN.BUTTON_RECHARGE;
				}
			}
			else if (target.IsWorking)
			{
				contents.SetActive(true);
				label.text = UI.UISIDESCREENS.GENESHUFFLERSIDESREEN.UNDERWAY;
				button.gameObject.SetActive(false);
			}
			else
			{
				contents.SetActive(false);
			}
		}
		else
		{
			contents.SetActive(false);
		}
	}
}
