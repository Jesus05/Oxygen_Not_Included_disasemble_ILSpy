using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class AccessControlSideScreen : SideScreenContent
{
	private static class MinionIdentitySort
	{
		public class SortInfo : IListableOption
		{
			public LocString name;

			public Comparison<MinionIdentity> compare;

			public string GetProperName()
			{
				return name;
			}
		}

		public static readonly SortInfo[] SortInfos = new SortInfo[2]
		{
			new SortInfo
			{
				name = UI.MINION_IDENTITY_SORT.NAME,
				compare = new Comparison<MinionIdentity>(CompareByName)
			},
			new SortInfo
			{
				name = UI.MINION_IDENTITY_SORT.ROLE,
				compare = new Comparison<MinionIdentity>(CompareByRole)
			}
		};

		[CompilerGenerated]
		private static Comparison<MinionIdentity> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Comparison<MinionIdentity> _003C_003Ef__mg_0024cache1;

		public static int CompareByName(MinionIdentity a, MinionIdentity b)
		{
			return a.GetProperName().CompareTo(b.GetProperName());
		}

		public static int CompareByRole(MinionIdentity a, MinionIdentity b)
		{
			ChoreConsumer component = a.GetComponent<ChoreConsumer>();
			ChoreConsumer component2 = b.GetComponent<ChoreConsumer>();
			return component.resume.CurrentRole.CompareTo(component2.resume.CurrentRole);
		}
	}

	[SerializeField]
	private AccessControlSideScreenRow rowPrefab;

	[SerializeField]
	private GameObject rowGroup;

	[SerializeField]
	private AccessControlSideScreenDoor defaultsRow;

	[SerializeField]
	private Toggle sortByNameToggle;

	[SerializeField]
	private Toggle sortByPermissionToggle;

	[SerializeField]
	private Toggle sortByRoleToggle;

	[SerializeField]
	private GameObject disabledOverlay;

	[SerializeField]
	private KImage headerBG;

	private AccessControl target;

	private Door doorTarget;

	private UIPool<AccessControlSideScreenRow> rowPool;

	private MinionIdentitySort.SortInfo sortInfo = MinionIdentitySort.SortInfos[0];

	private Dictionary<MinionIdentity, AccessControlSideScreenRow> identityRowMap = new Dictionary<MinionIdentity, AccessControlSideScreenRow>();

	private List<MinionIdentity> identityList = new List<MinionIdentity>();

	[CompilerGenerated]
	private static Comparison<MinionIdentity> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Comparison<MinionIdentity> _003C_003Ef__mg_0024cache1;

	public override string GetTitle()
	{
		if ((UnityEngine.Object)target != (UnityEngine.Object)null)
		{
			return string.Format(base.GetTitle(), target.GetProperName());
		}
		return base.GetTitle();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		sortByNameToggle.onValueChanged.AddListener(delegate(bool reverse_sort)
		{
			SortEntries(reverse_sort, MinionIdentitySort.CompareByName);
		});
		sortByRoleToggle.onValueChanged.AddListener(delegate(bool reverse_sort)
		{
			SortEntries(reverse_sort, MinionIdentitySort.CompareByRole);
		});
		sortByPermissionToggle.onValueChanged.AddListener(SortByPermission);
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return (UnityEngine.Object)target.GetComponent<AccessControl>() != (UnityEngine.Object)null;
	}

	public override void SetTarget(GameObject target)
	{
		if ((UnityEngine.Object)this.target != (UnityEngine.Object)null)
		{
			ClearTarget();
		}
		this.target = target.GetComponent<AccessControl>();
		doorTarget = target.GetComponent<Door>();
		if (!((UnityEngine.Object)this.target == (UnityEngine.Object)null))
		{
			target.Subscribe(1734268753, OnDoorStateChanged);
			target.Subscribe(-1525636549, OnAccessControlChanged);
			if (rowPool == null)
			{
				rowPool = new UIPool<AccessControlSideScreenRow>(rowPrefab);
			}
			base.gameObject.SetActive(true);
			identityList = new List<MinionIdentity>(Components.LiveMinionIdentities.Items);
			Refresh(identityList, true);
		}
	}

	public override void ClearTarget()
	{
		base.ClearTarget();
		if ((UnityEngine.Object)target != (UnityEngine.Object)null)
		{
			target.Unsubscribe(1734268753, OnDoorStateChanged);
			target.Unsubscribe(-1525636549, OnAccessControlChanged);
		}
	}

	private void Refresh(List<MinionIdentity> identities, bool rebuild)
	{
		Rotatable component = target.GetComponent<Rotatable>();
		bool rotated = (UnityEngine.Object)component != (UnityEngine.Object)null && component.IsRotated;
		defaultsRow.SetRotated(rotated);
		defaultsRow.SetContent(target.DefaultPermission, OnDefaultPermissionChanged);
		if (rebuild)
		{
			ClearContent();
		}
		foreach (MinionIdentity identity in identities)
		{
			AccessControlSideScreenRow accessControlSideScreenRow;
			if (rebuild)
			{
				accessControlSideScreenRow = rowPool.GetFreeElement(rowGroup, true);
				identityRowMap.Add(identity, accessControlSideScreenRow);
			}
			else
			{
				accessControlSideScreenRow = identityRowMap[identity];
			}
			AccessControl.Permission setPermission = target.GetSetPermission(identity.gameObject);
			bool isDefault = target.IsDefaultPermission(identity.gameObject);
			accessControlSideScreenRow.SetRotated(rotated);
			accessControlSideScreenRow.SetMinionContent(identity, setPermission, isDefault, OnPermissionChanged, OnPermissionDefault);
		}
		RefreshOnline();
		ContentContainer.SetActive(target.controlEnabled);
	}

	private void RefreshOnline()
	{
		bool flag = target.Online && ((UnityEngine.Object)doorTarget == (UnityEngine.Object)null || doorTarget.CurrentState == Door.ControlState.Auto);
		disabledOverlay.SetActive(!flag);
		headerBG.ColorState = ((!flag) ? KImage.ColorSelector.Inactive : KImage.ColorSelector.Active);
	}

	private void SortByPermission(bool state)
	{
		ExecuteSort(sortByPermissionToggle, state, (MinionIdentity identity) => (int)((!target.IsDefaultPermission(identity.gameObject)) ? target.GetSetPermission(identity.gameObject) : ((AccessControl.Permission)(-1))), false);
	}

	private void ExecuteSort<T>(Toggle toggle, bool state, Func<MinionIdentity, T> sortFunction, bool refresh = false)
	{
		toggle.GetComponent<ImageToggleState>().SetActiveState(state);
		if (state)
		{
			identityList = ((!state) ? identityList.OrderByDescending(sortFunction).ToList() : identityList.OrderBy(sortFunction).ToList());
			if (refresh)
			{
				Refresh(identityList, false);
			}
			else
			{
				for (int i = 0; i < identityList.Count; i++)
				{
					if (identityRowMap.ContainsKey(identityList[i]))
					{
						identityRowMap[identityList[i]].transform.SetSiblingIndex(i);
					}
				}
			}
		}
	}

	private void SortEntries(bool reverse_sort, Comparison<MinionIdentity> compare)
	{
		identityList.Sort(compare);
		if (reverse_sort)
		{
			identityList.Reverse();
		}
		for (int i = 0; i < identityList.Count; i++)
		{
			if (identityRowMap.ContainsKey(identityList[i]))
			{
				identityRowMap[identityList[i]].transform.SetSiblingIndex(i);
			}
		}
	}

	private void ClearContent()
	{
		if (rowPool != null)
		{
			rowPool.ClearAll();
		}
		identityRowMap.Clear();
	}

	private void OnDefaultPermissionChanged(MinionIdentity identity, AccessControl.Permission permission)
	{
		target.DefaultPermission = permission;
		Refresh(identityList, false);
	}

	private void OnPermissionChanged(MinionIdentity identity, AccessControl.Permission permission)
	{
		target.SetPermission(identity.gameObject, permission);
	}

	private void OnPermissionDefault(MinionIdentity identity, bool isDefault)
	{
		if (isDefault)
		{
			target.ClearPermission(identity.gameObject);
		}
		else
		{
			target.SetPermission(identity.gameObject, target.DefaultPermission);
		}
		Refresh(identityList, false);
	}

	private void OnAccessControlChanged(object data)
	{
		RefreshOnline();
	}

	private void OnDoorStateChanged(object data)
	{
		RefreshOnline();
	}

	private void OnSelectSortFunc(IListableOption role, object data)
	{
		if (role != null)
		{
			MinionIdentitySort.SortInfo[] sortInfos = MinionIdentitySort.SortInfos;
			int num = 0;
			MinionIdentitySort.SortInfo sortInfo;
			while (true)
			{
				if (num >= sortInfos.Length)
				{
					return;
				}
				sortInfo = sortInfos[num];
				if ((string)sortInfo.name == role.GetProperName())
				{
					break;
				}
				num++;
			}
			this.sortInfo = sortInfo;
			identityList.Sort(this.sortInfo.compare);
			for (int i = 0; i < identityList.Count; i++)
			{
				if (identityRowMap.ContainsKey(identityList[i]))
				{
					identityRowMap[identityList[i]].transform.SetSiblingIndex(i);
				}
			}
		}
	}
}
