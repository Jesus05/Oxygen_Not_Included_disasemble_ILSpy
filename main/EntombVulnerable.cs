using KSerialization;
using STRINGS;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EntombVulnerable : KMonoBehaviour, IWiltCause
{
	[MyCmpReq]
	private KSelectable selectable;

	private OccupyArea _occupyArea;

	[Serialize]
	private bool isEntombed = false;

	private HandleVector<int>.Handle partitionerEntry;

	[CompilerGenerated]
	private static Func<int, object, bool> _003C_003Ef__mg_0024cache0;

	private OccupyArea occupyArea
	{
		get
		{
			if ((UnityEngine.Object)_occupyArea == (UnityEngine.Object)null)
			{
				_occupyArea = GetComponent<OccupyArea>();
			}
			return _occupyArea;
		}
	}

	public bool GetEntombed => isEntombed;

	public string WiltStateString => Db.Get().CreatureStatusItems.Entombed.resolveStringCallback(CREATURES.STATUSITEMS.ENTOMBED.LINE_ITEM, base.gameObject);

	public WiltCondition.Condition[] Conditions => new WiltCondition.Condition[1]
	{
		WiltCondition.Condition.Entombed
	};

	protected override void OnSpawn()
	{
		base.OnSpawn();
		partitionerEntry = GameScenePartitioner.Instance.Add("EntombVulnerable", base.gameObject, occupyArea.GetExtents(), GameScenePartitioner.Instance.solidChangedLayer, OnSolidChanged);
		CheckEntombed();
		if (isEntombed)
		{
			Trigger(-1089732772, true);
		}
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		base.OnCleanUp();
	}

	private void OnSolidChanged(object data)
	{
		CheckEntombed();
	}

	private void CheckEntombed()
	{
		int cell = Grid.PosToCell(base.gameObject.transform.GetPosition());
		if (Grid.IsValidCell(cell))
		{
			if (!IsCellSafe(cell))
			{
				if (!isEntombed)
				{
					isEntombed = true;
					selectable.AddStatusItem(Db.Get().CreatureStatusItems.Entombed, base.gameObject);
					GetComponent<KPrefabID>().AddTag(GameTags.Entombed, false);
					Trigger(-1089732772, true);
				}
			}
			else if (isEntombed)
			{
				isEntombed = false;
				selectable.RemoveStatusItem(Db.Get().CreatureStatusItems.Entombed, false);
				GetComponent<KPrefabID>().RemoveTag(GameTags.Entombed);
				Trigger(-1089732772, false);
			}
		}
	}

	public bool IsCellSafe(int cell)
	{
		return occupyArea.TestArea(cell, null, IsCellSafeCB);
	}

	private static bool IsCellSafeCB(int cell, object data)
	{
		return Grid.IsValidCell(cell) && !Grid.Solid[cell];
	}
}
