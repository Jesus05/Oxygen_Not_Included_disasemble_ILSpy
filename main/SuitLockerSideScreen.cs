using STRINGS;
using UnityEngine;

public class SuitLockerSideScreen : SideScreenContent
{
	[SerializeField]
	private GameObject initialConfigScreen;

	[SerializeField]
	private GameObject regularConfigScreen;

	[SerializeField]
	private LocText initialConfigLabel;

	[SerializeField]
	private KButton initialConfigRequestSuitButton;

	[SerializeField]
	private KButton initialConfigNoSuitButton;

	[SerializeField]
	private LocText regularConfigLabel;

	[SerializeField]
	private KButton regularConfigRequestSuitButton;

	[SerializeField]
	private KButton regularConfigDropSuitButton;

	private SuitLocker suitLocker;

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return (Object)target.GetComponent<SuitLocker>() != (Object)null;
	}

	public override void SetTarget(GameObject target)
	{
		suitLocker = target.GetComponent<SuitLocker>();
		initialConfigRequestSuitButton.GetComponentInChildren<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_REQUEST_SUIT_TOOLTIP);
		initialConfigNoSuitButton.GetComponentInChildren<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_NO_SUIT_TOOLTIP);
		initialConfigRequestSuitButton.ClearOnClick();
		initialConfigRequestSuitButton.onClick += delegate
		{
			suitLocker.ConfigRequestSuit();
		};
		initialConfigNoSuitButton.ClearOnClick();
		initialConfigNoSuitButton.onClick += delegate
		{
			suitLocker.ConfigNoSuit();
		};
		regularConfigRequestSuitButton.ClearOnClick();
		regularConfigRequestSuitButton.onClick += delegate
		{
			if (suitLocker.smi.sm.isWaitingForSuit.Get(suitLocker.smi))
			{
				suitLocker.ConfigNoSuit();
			}
			else
			{
				suitLocker.ConfigRequestSuit();
			}
		};
		regularConfigDropSuitButton.ClearOnClick();
		regularConfigDropSuitButton.onClick += delegate
		{
			suitLocker.DropSuit();
		};
	}

	private void Update()
	{
		bool flag = suitLocker.smi.sm.isConfigured.Get(suitLocker.smi);
		initialConfigScreen.gameObject.SetActive(!flag);
		regularConfigScreen.gameObject.SetActive(flag);
		bool flag2 = (Object)suitLocker.GetStoredOutfit() != (Object)null;
		bool flag3 = suitLocker.smi.sm.isWaitingForSuit.Get(suitLocker.smi);
		regularConfigRequestSuitButton.isInteractable = !flag2;
		if (!flag3)
		{
			regularConfigRequestSuitButton.GetComponentInChildren<LocText>().text = UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_REQUEST_SUIT;
			regularConfigRequestSuitButton.GetComponentInChildren<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_REQUEST_SUIT_TOOLTIP);
		}
		else
		{
			regularConfigRequestSuitButton.GetComponentInChildren<LocText>().text = UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_CANCEL_REQUEST;
			regularConfigRequestSuitButton.GetComponentInChildren<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_CANCEL_REQUEST_TOOLTIP);
		}
		if (flag2)
		{
			regularConfigDropSuitButton.isInteractable = true;
			regularConfigDropSuitButton.GetComponentInChildren<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_DROP_SUIT_TOOLTIP);
		}
		else
		{
			regularConfigDropSuitButton.isInteractable = false;
			regularConfigDropSuitButton.GetComponentInChildren<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_DROP_SUIT_NO_SUIT_TOOLTIP);
		}
		KSelectable component = suitLocker.GetComponent<KSelectable>();
		if ((Object)component != (Object)null)
		{
			StatusItemGroup.Entry statusItem = component.GetStatusItem(Db.Get().StatusItemCategories.Main);
			if (statusItem.item != null)
			{
				regularConfigLabel.text = statusItem.item.GetName(statusItem.data);
				regularConfigLabel.GetComponentInChildren<ToolTip>().SetSimpleTooltip(statusItem.item.GetTooltip(statusItem.data));
			}
		}
	}
}
