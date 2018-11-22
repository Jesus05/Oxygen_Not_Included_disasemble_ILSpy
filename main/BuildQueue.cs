using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BuildQueue : KButtonMenu
{
	private IHasBuildQueue fabricator;

	private int prevLength = 0;

	private Dictionary<Tag, float> allocatedMaterials = new Dictionary<Tag, float>();

	public List<Storage> availableMaterialStorages = new List<Storage>();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		keepMenuOpen = true;
		buttons = new ButtonInfo[fabricator.MaxOrders];
		for (int i = 0; i < fabricator.MaxOrders; i++)
		{
			string text = (i + 1).ToString();
			int order_idx = i;
			buttons[i] = new ButtonInfo(text, Action.NumActions, delegate
			{
				CancelOrder(order_idx);
			}, null, null);
		}
	}

	public void CancelOrder(int order_idx)
	{
		if (fabricator != null && fabricator.NumOrders != 0 && order_idx < fabricator.NumOrders)
		{
			fabricator.CancelUserOrder(order_idx);
		}
	}

	private void Update()
	{
	}

	public override void RefreshButtons()
	{
		if (buttonObjects != null)
		{
			for (int i = 0; i < buttonObjects.Length; i++)
			{
				UnityEngine.Object.Destroy(buttonObjects[i]);
			}
			buttonObjects = null;
		}
		if (buttons != null)
		{
			buttonObjects = new GameObject[buttons.Count];
			for (int j = 0; j < buttons.Count; j++)
			{
				ButtonInfo binfo = buttons[j];
				GameObject gameObject = UnityEngine.Object.Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity);
				buttonObjects[j] = gameObject;
				Transform parent = (!((UnityEngine.Object)buttonParent != (UnityEngine.Object)null)) ? base.transform : buttonParent;
				gameObject.transform.SetParent(parent, false);
				gameObject.SetActive(true);
				gameObject.name = binfo.text + "Button";
				LocText[] componentsInChildren = gameObject.GetComponentsInChildren<LocText>(true);
				if (componentsInChildren != null)
				{
					LocText[] array = componentsInChildren;
					foreach (LocText locText in array)
					{
						locText.text = ((!(locText.name == "Hotkey")) ? binfo.text : GameUtil.GetActionString(binfo.shortcutKey));
						locText.color = ((!binfo.isEnabled) ? new Color(0.5f, 0.5f, 0.5f) : new Color(1f, 1f, 1f));
					}
				}
				ToolTip componentInChildren = gameObject.GetComponentInChildren<ToolTip>();
				if (binfo.toolTip != null && binfo.toolTip != "" && (UnityEngine.Object)componentInChildren != (UnityEngine.Object)null)
				{
					componentInChildren.toolTip = binfo.toolTip;
				}
				KButton button = gameObject.GetComponentInChildren<KButton>();
				button.isInteractable = binfo.isEnabled;
				if (binfo.popupOptions == null && binfo.onPopulatePopup == null)
				{
					UnityAction onClick = binfo.onClick;
					System.Action value = delegate
					{
						onClick();
						if (!keepMenuOpen && (UnityEngine.Object)this != (UnityEngine.Object)null)
						{
							Deactivate();
						}
					};
					button.onClick += value;
				}
				else
				{
					button.onClick += delegate
					{
						SetupPopupMenu(binfo, button);
					};
				}
				binfo.uibutton = button;
				if (binfo.onHover == null)
				{
					continue;
				}
			}
			Update();
		}
	}

	public void SetFabricator(IHasBuildQueue fabricator)
	{
		availableMaterialStorages.Clear();
		this.fabricator = fabricator;
		if (!base.gameObject.activeInHierarchy)
		{
			base.gameObject.SetActive(true);
		}
		RefreshButtons();
		ConfigureJumpToOrderButtons();
	}

	public void ConfigureJumpToOrderButtons()
	{
		if (buttonObjects != null)
		{
			for (int i = 0; i < buttonObjects.Length; i++)
			{
				int index = i;
				buttonObjects[i].gameObject.GetComponent<BuildQueueButton>().jumpToOrderButton.GetComponent<MultiToggle>().onClick = delegate
				{
					fabricator.SetCurrentUserOrderByIndex(index);
				};
			}
		}
	}

	public void AddAvailableMaterialStorage(Storage storage)
	{
		if (!availableMaterialStorages.Contains(storage))
		{
			availableMaterialStorages.Add(storage);
		}
	}

	protected override void OnDeactivate()
	{
		for (int i = 0; i < fabricator.MaxOrders; i++)
		{
			BuildQueueButton componentInChildren = buttonObjects[i].GetComponentInChildren<BuildQueueButton>();
			if ((UnityEngine.Object)componentInChildren != (UnityEngine.Object)null)
			{
				componentInChildren.SetOrder(null);
			}
		}
	}
}
