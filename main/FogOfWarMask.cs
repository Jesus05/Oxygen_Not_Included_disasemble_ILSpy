using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FogOfWarMask : KMonoBehaviour
{
	[CompilerGenerated]
	private static Func<int, bool> _003C_003Ef__mg_0024cache0;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Grid.OnReveal = (Action<int>)Delegate.Combine(Grid.OnReveal, new Action<int>(OnReveal));
	}

	private void OnReveal(int cell)
	{
		if (Grid.PosToCell(this) == cell)
		{
			Grid.OnReveal = (Action<int>)Delegate.Remove(Grid.OnReveal, new Action<int>(OnReveal));
			base.gameObject.DeleteObject();
		}
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		GameUtil.FloodCollectCells(Grid.PosToCell(this), delegate(int cell)
		{
			Grid.Visible[cell] = 0;
			Grid.PreventFogOfWarReveal[cell] = true;
			return !Grid.Solid[cell];
		}, 300, null, true);
		GameUtil.FloodCollectCells(Grid.PosToCell(this), delegate(int cell)
		{
			bool flag = Grid.PreventFogOfWarReveal[cell];
			if (Grid.Solid[cell] && Grid.Foundation[cell])
			{
				Grid.PreventFogOfWarReveal[cell] = true;
				Grid.Visible[cell] = 0;
				GameObject gameObject = Grid.Objects[cell, 1];
				if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null && gameObject.GetComponent<KPrefabID>().PrefabTag.ToString() == "POIBunkerExteriorDoor")
				{
					Grid.PreventFogOfWarReveal[cell] = false;
					Grid.Visible[cell] = byte.MaxValue;
				}
			}
			return flag || Grid.Foundation[cell];
		}, 300, null, true);
	}

	public static void ClearMask(int cell)
	{
		if (Grid.PreventFogOfWarReveal[cell])
		{
			GameUtil.FloodCollectCells(cell, RevealFogOfWarMask, 300, null, true);
		}
	}

	public static bool RevealFogOfWarMask(int cell)
	{
		bool flag = Grid.PreventFogOfWarReveal[cell];
		if (flag)
		{
			Grid.PreventFogOfWarReveal[cell] = false;
			Grid.Reveal(cell, byte.MaxValue);
		}
		return flag;
	}
}
