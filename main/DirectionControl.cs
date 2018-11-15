using KSerialization;
using STRINGS;
using System;
using UnityEngine;

public class DirectionControl : KMonoBehaviour
{
	private struct DirectionInfo
	{
		public bool allowLeft;

		public bool allowRight;

		public string iconName;

		public string name;

		public string tooltip;
	}

	[Serialize]
	public WorkableReactable.AllowedDirection allowedDirection;

	private DirectionInfo[] directionInfos;

	public Action<WorkableReactable.AllowedDirection> onDirectionChanged;

	private static readonly EventSystem.IntraObjectHandler<DirectionControl> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<DirectionControl>(delegate(DirectionControl component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		allowedDirection = WorkableReactable.AllowedDirection.Any;
		directionInfos = new DirectionInfo[3]
		{
			new DirectionInfo
			{
				allowLeft = true,
				allowRight = true,
				iconName = "action_direction_both",
				name = (string)UI.USERMENUACTIONS.WORKABLE_DIRECTION_BOTH.NAME,
				tooltip = (string)UI.USERMENUACTIONS.WORKABLE_DIRECTION_BOTH.TOOLTIP
			},
			new DirectionInfo
			{
				allowLeft = true,
				allowRight = false,
				iconName = "action_direction_left",
				name = (string)UI.USERMENUACTIONS.WORKABLE_DIRECTION_LEFT.NAME,
				tooltip = (string)UI.USERMENUACTIONS.WORKABLE_DIRECTION_LEFT.TOOLTIP
			},
			new DirectionInfo
			{
				allowLeft = false,
				allowRight = true,
				iconName = "action_direction_right",
				name = (string)UI.USERMENUACTIONS.WORKABLE_DIRECTION_RIGHT.NAME,
				tooltip = (string)UI.USERMENUACTIONS.WORKABLE_DIRECTION_RIGHT.TOOLTIP
			}
		};
		GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.DirectionControl, this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		SetAllowedDirection(allowedDirection);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
	}

	private void SetAllowedDirection(WorkableReactable.AllowedDirection new_direction)
	{
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		DirectionInfo directionInfo = directionInfos[(int)new_direction];
		bool flag = directionInfo.allowLeft && directionInfo.allowRight;
		bool is_visible = !flag && directionInfo.allowLeft;
		bool is_visible2 = !flag && directionInfo.allowRight;
		component.SetSymbolVisiblity("arrow2", flag);
		component.SetSymbolVisiblity("arrow_left", is_visible);
		component.SetSymbolVisiblity("arrow_right", is_visible2);
		if (new_direction != allowedDirection)
		{
			allowedDirection = new_direction;
			if (onDirectionChanged != null)
			{
				onDirectionChanged(allowedDirection);
			}
		}
	}

	private void OnChangeWorkableDirection()
	{
		SetAllowedDirection((WorkableReactable.AllowedDirection)((int)(1 + allowedDirection) % directionInfos.Length));
	}

	private void OnRefreshUserMenu(object data)
	{
		int num = (int)(1 + allowedDirection) % directionInfos.Length;
		DirectionInfo directionInfo = directionInfos[num];
		UserMenu userMenu = Game.Instance.userMenu;
		GameObject gameObject = base.gameObject;
		string iconName = directionInfo.iconName;
		string name = directionInfo.name;
		System.Action on_click = OnChangeWorkableDirection;
		string tooltip = directionInfo.tooltip;
		userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo(iconName, name, on_click, Action.NumActions, null, null, null, tooltip, true), 0f);
	}
}
