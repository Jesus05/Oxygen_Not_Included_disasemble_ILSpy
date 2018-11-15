using System.Collections.Generic;
using UnityEngine;

public class DoorTransitionLayer : TransitionDriver.OverrideLayer
{
	private List<Door> doors = new List<Door>();

	private Door targetDoor;

	public DoorTransitionLayer(Navigator navigator)
		: base(navigator)
	{
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	private bool AreAllDoorsOpen()
	{
		foreach (Door door in doors)
		{
			if ((Object)door != (Object)null && !door.IsOpen())
			{
				return false;
			}
		}
		return true;
	}

	public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.BeginTransition(navigator, transition);
		int cell = Grid.PosToCell(navigator);
		int cell2 = Grid.OffsetCell(cell, transition.x, transition.y);
		AddDoor(cell2);
		if (navigator.CurrentNavType != NavType.Tube)
		{
			AddDoor(Grid.CellAbove(cell2));
		}
		for (int i = 0; i < transition.navGridTransition.voidOffsets.Length; i++)
		{
			int cell3 = Grid.OffsetCell(cell, transition.navGridTransition.voidOffsets[i]);
			AddDoor(cell3);
		}
		if (doors.Count > 0 && !AreAllDoorsOpen())
		{
			transition.anim = navigator.NavGrid.GetIdleAnim(navigator.CurrentNavType);
			transition.isLooping = false;
			transition.end = transition.start;
			transition.speed = 1f;
			transition.animSpeed = 1f;
			transition.x = 0;
			transition.y = 0;
			transition.isCompleteCB = (() => AreAllDoorsOpen());
		}
		foreach (Door door in doors)
		{
			door.Open();
		}
	}

	public override void UpdateTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.UpdateTransition(navigator, transition);
	}

	public override void EndTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.EndTransition(navigator, transition);
		foreach (Door door in doors)
		{
			if ((Object)door != (Object)null)
			{
				door.Close();
			}
		}
		doors.Clear();
	}

	private void AddDoor(int cell)
	{
		Door door = GetDoor(cell);
		if ((Object)door != (Object)null && !doors.Contains(door))
		{
			doors.Add(door);
		}
	}

	private Door GetDoor(int cell)
	{
		if (!Grid.HasDoor[cell])
		{
			return null;
		}
		GameObject gameObject = Grid.Objects[cell, 1];
		if ((Object)gameObject != (Object)null)
		{
			Door component = gameObject.GetComponent<Door>();
			if ((Object)component != (Object)null && component.isSpawned)
			{
				return component;
			}
		}
		return null;
	}
}
