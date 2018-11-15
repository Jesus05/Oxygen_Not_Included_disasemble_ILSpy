using System;
using System.Collections;
using UnityEngine;

[SkipSaveFileSerialization]
public class TravelTube : KMonoBehaviour, IFirstFrameCallback, ITravelTubePiece, IHaveUtilityNetworkMgr
{
	[MyCmpReq]
	private KSelectable selectable;

	private HandleVector<int>.Handle dirtyNavCellUpdatedEntry;

	private bool isExitTube;

	private bool hasValidExitTransitions;

	private UtilityConnections connections;

	private static readonly EventSystem.IntraObjectHandler<TravelTube> OnConnectionsChangedDelegate = new EventSystem.IntraObjectHandler<TravelTube>(delegate(TravelTube component, object data)
	{
		component.OnConnectionsChanged(data);
	});

	private Guid connectedStatus;

	private System.Action firstFrameCallback;

	public Vector3 Position => base.transform.GetPosition();

	public IUtilityNetworkMgr GetNetworkManager()
	{
		return Game.Instance.travelTubeSystem;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Grid.HasTube[Grid.PosToCell(this)] = true;
		Components.ITravelTubePieces.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		int cell = Grid.PosToCell(base.transform.GetPosition());
		Game.Instance.travelTubeSystem.AddToNetworks(cell, this, false);
		Subscribe(-1041684577, OnConnectionsChangedDelegate);
	}

	protected override void OnCleanUp()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		BuildingComplete component = GetComponent<BuildingComplete>();
		if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || (UnityEngine.Object)Grid.Objects[cell, (int)component.Def.ReplacementLayer] == (UnityEngine.Object)null)
		{
			Game.Instance.travelTubeSystem.RemoveFromNetworks(cell, this, false);
		}
		Unsubscribe(-1041684577);
		Grid.HasTube[Grid.PosToCell(this)] = false;
		Components.ITravelTubePieces.Remove(this);
		GameScenePartitioner.Instance.Free(ref dirtyNavCellUpdatedEntry);
		base.OnCleanUp();
	}

	private void OnConnectionsChanged(object data)
	{
		connections = (UtilityConnections)data;
		bool flag = connections == UtilityConnections.Up || connections == UtilityConnections.Down || connections == UtilityConnections.Left || connections == UtilityConnections.Right;
		if (flag != isExitTube)
		{
			isExitTube = flag;
			UpdateExitListener(isExitTube);
			UpdateExitStatus();
		}
	}

	private void UpdateExitListener(bool enable)
	{
		if (enable && !dirtyNavCellUpdatedEntry.IsValid())
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			dirtyNavCellUpdatedEntry = GameScenePartitioner.Instance.Add("TravelTube.OnDirtyNavCellUpdated", this, cell, GameScenePartitioner.Instance.dirtyNavCellUpdateLayer, OnDirtyNavCellUpdated);
			OnDirtyNavCellUpdated(null);
		}
		else if (!enable && dirtyNavCellUpdatedEntry.IsValid())
		{
			GameScenePartitioner.Instance.Free(ref dirtyNavCellUpdatedEntry);
		}
	}

	private void OnDirtyNavCellUpdated(object data)
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		NavGrid navGrid = Pathfinding.Instance.GetNavGrid("MinionNavGrid");
		int num2 = num * navGrid.maxLinksPerCell;
		bool flag = false;
		if (isExitTube)
		{
			NavGrid.Link link = navGrid.Links[num2];
			while (link.link != PathFinder.InvalidHandle)
			{
				if (link.startNavType == NavType.Tube)
				{
					if (link.endNavType != NavType.Tube)
					{
						flag = true;
						break;
					}
					UtilityConnections utilityConnections = UtilityConnectionsExtensions.DirectionFromToCell(link.link, num);
					if (connections == utilityConnections)
					{
						flag = true;
						break;
					}
				}
				num2++;
				link = navGrid.Links[num2];
			}
		}
		if (flag != hasValidExitTransitions)
		{
			hasValidExitTransitions = flag;
			UpdateExitStatus();
		}
	}

	private void UpdateExitStatus()
	{
		if (!isExitTube || hasValidExitTransitions)
		{
			connectedStatus = selectable.RemoveStatusItem(connectedStatus, false);
		}
		else if (connectedStatus == Guid.Empty)
		{
			connectedStatus = selectable.AddStatusItem(Db.Get().BuildingStatusItems.NoTubeExits, null);
		}
	}

	public void SetFirstFrameCallback(System.Action ffCb)
	{
		firstFrameCallback = ffCb;
		StartCoroutine(RunCallback());
	}

	private IEnumerator RunCallback()
	{
		yield return (object)null;
		/*Error: Unable to find new state assignment for yield return*/;
	}
}
