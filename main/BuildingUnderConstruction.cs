using UnityEngine;

public class BuildingUnderConstruction : Building
{
	[MyCmpAdd]
	private KSelectable selectable;

	[MyCmpAdd]
	private SaveLoadRoot saveLoadRoot;

	[MyCmpAdd]
	private KPrefabID kPrefabID;

	[MyCmpAdd]
	private Cancellable cancellable;

	protected override void OnPrefabInit()
	{
		Vector3 position = base.transform.GetPosition();
		position.z = Grid.GetLayerZ(Def.SceneLayer);
		base.transform.SetPosition(position);
		base.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Construction"));
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		Rotatable component2 = GetComponent<Rotatable>();
		if ((Object)component != (Object)null && (Object)component2 == (Object)null)
		{
			component.Offset = Def.GetVisualizerOffset() + Def.placementPivot;
		}
		KBoxCollider2D component3 = GetComponent<KBoxCollider2D>();
		if ((Object)component3 != (Object)null)
		{
			Vector3 visualizerOffset = Def.GetVisualizerOffset();
			component3.offset += new Vector2(visualizerOffset.x, visualizerOffset.y);
		}
		if (Def.IsTilePiece)
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			Def.RunOnArea(cell, base.Orientation, delegate(int c)
			{
				TileVisualizer.RefreshCell(c, Def.TileLayer, Def.ReplacementLayer);
			});
		}
		RegisterBlockTileRenderer();
	}

	protected override void OnCleanUp()
	{
		UnregisterBlockTileRenderer();
		base.OnCleanUp();
	}
}
