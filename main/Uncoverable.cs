using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Uncoverable : KMonoBehaviour
{
	[MyCmpReq]
	private OccupyArea occupyArea;

	[Serialize]
	private bool hasBeenUncovered;

	private HandleVector<int>.Handle partitionerEntry;

	[CompilerGenerated]
	private static Func<int, object, bool> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Func<List<Notification>, object, string> _003C_003Ef__mg_0024cache1;

	private bool IsAnyCellShowing()
	{
		int rootCell = Grid.PosToCell(this);
		bool flag = occupyArea.TestArea(rootCell, null, IsCellBlocked);
		return !flag;
	}

	private static bool IsCellBlocked(int cell, object data)
	{
		return Grid.Element[cell].IsSolid && !Grid.Foundation[cell];
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (IsAnyCellShowing())
		{
			hasBeenUncovered = true;
		}
		if (!hasBeenUncovered)
		{
			GetComponent<KSelectable>().IsSelectable = false;
			Extents extents = occupyArea.GetExtents();
			partitionerEntry = GameScenePartitioner.Instance.Add("Uncoverable.OnSpawn", base.gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, OnSolidChanged);
		}
	}

	private void OnSolidChanged(object data)
	{
		if (IsAnyCellShowing() && !hasBeenUncovered && partitionerEntry.IsValid())
		{
			GameScenePartitioner.Instance.Free(ref partitionerEntry);
			hasBeenUncovered = true;
			GetComponent<KSelectable>().IsSelectable = true;
			Notification notification = new Notification(MISC.STATUSITEMS.BURIEDITEM.NOTIFICATION, NotificationType.Good, HashedString.Invalid, OnNotificationToolTip, this, true, 0f, null, null);
			base.gameObject.AddOrGet<Notifier>().Add(notification, string.Empty);
		}
	}

	private static string OnNotificationToolTip(List<Notification> notifications, object data)
	{
		Uncoverable cmp = (Uncoverable)data;
		return MISC.STATUSITEMS.BURIEDITEM.NOTIFICATION_TOOLTIP.Replace("{Uncoverable}", cmp.GetProperName());
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
	}
}
