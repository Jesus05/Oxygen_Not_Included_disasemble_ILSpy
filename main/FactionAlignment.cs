using KSerialization;
using STRINGS;
using System;

public class FactionAlignment : KMonoBehaviour
{
	[Serialize]
	private bool alignmentActive = true;

	public FactionManager.FactionID Alignment;

	[Serialize]
	public bool targeted;

	[Serialize]
	public bool targetable = true;

	private static readonly EventSystem.IntraObjectHandler<FactionAlignment> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<FactionAlignment>(delegate(FactionAlignment component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<FactionAlignment> OnDeathDelegate = new EventSystem.IntraObjectHandler<FactionAlignment>(delegate(FactionAlignment component, object data)
	{
		component.OnDeath(data);
	});

	private static readonly EventSystem.IntraObjectHandler<FactionAlignment> SetPlayerTargetedFalseDelegate = new EventSystem.IntraObjectHandler<FactionAlignment>(delegate(FactionAlignment component, object data)
	{
		component.SetPlayerTargeted(false);
	});

	[MyCmpAdd]
	public Health health
	{
		get;
		private set;
	}

	public AttackableBase attackable
	{
		get;
		private set;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		health = GetComponent<Health>();
		attackable = GetComponent<AttackableBase>();
		Components.FactionAlignments.Add(this);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Subscribe(2127324410, SetPlayerTargetedFalseDelegate);
		if (alignmentActive)
		{
			FactionManager.Instance.GetFaction(Alignment).Members.Add(this);
		}
		Subscribe(1623392196, OnDeathDelegate);
	}

	private void OnDeath(object data)
	{
		SetAlignmentActive(false);
	}

	public void SetAlignmentActive(bool active)
	{
		SetPlayerTargetable(active);
		alignmentActive = active;
		if (active)
		{
			FactionManager.Instance.GetFaction(Alignment).Members.Add(this);
		}
		else
		{
			FactionManager.Instance.GetFaction(Alignment).Members.Remove(this);
		}
	}

	public bool IsAlignmentActive()
	{
		return FactionManager.Instance.GetFaction(Alignment).Members.Contains(this);
	}

	public void SetPlayerTargetable(bool state)
	{
		targetable = state;
		if (!state)
		{
			SetPlayerTargeted(false);
		}
	}

	public void SetPlayerTargeted(bool state)
	{
		targeted = (state && targetable);
		if (targeted)
		{
			GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.OrderAttack, this);
		}
		else
		{
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.OrderAttack, false);
		}
	}

	public void SwitchAlignment(FactionManager.FactionID newAlignment)
	{
		SetAlignmentActive(false);
		Alignment = newAlignment;
		SetAlignmentActive(true);
	}

	protected override void OnCleanUp()
	{
		Components.FactionAlignments.Remove(this);
		FactionManager.Instance.GetFaction(Alignment).Members.Remove(this);
		base.OnCleanUp();
	}

	private void OnRefreshUserMenu(object data)
	{
		if (Alignment != 0 && IsAlignmentActive())
		{
			object buttonInfo;
			if (!targeted)
			{
				string iconName = "action_attack";
				string text = UI.USERMENUACTIONS.ATTACK.NAME;
				System.Action on_click = delegate
				{
					SetPlayerTargeted(true);
				};
				string tooltipText = UI.USERMENUACTIONS.ATTACK.TOOLTIP;
				buttonInfo = new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true);
			}
			else
			{
				string tooltipText = "action_attack";
				string text = UI.USERMENUACTIONS.CANCELATTACK.NAME;
				System.Action on_click = delegate
				{
					SetPlayerTargeted(false);
				};
				string iconName = UI.USERMENUACTIONS.CANCELATTACK.TOOLTIP;
				buttonInfo = new KIconButtonMenu.ButtonInfo(tooltipText, text, on_click, Action.NumActions, null, null, null, iconName, true);
			}
			KIconButtonMenu.ButtonInfo button = (KIconButtonMenu.ButtonInfo)buttonInfo;
			Game.Instance.userMenu.AddButton(base.gameObject, button, 1f);
		}
	}
}
