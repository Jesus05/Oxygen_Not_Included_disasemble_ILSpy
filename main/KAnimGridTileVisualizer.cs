using Rendering;
using UnityEngine;

[SkipSaveFileSerialization]
public class KAnimGridTileVisualizer : KMonoBehaviour, IBlockTileInfo
{
	[SerializeField]
	public int blockTileConnectorID;

	private static readonly EventSystem.IntraObjectHandler<KAnimGridTileVisualizer> OnSelectionChangedDelegate = new EventSystem.IntraObjectHandler<KAnimGridTileVisualizer>(delegate(KAnimGridTileVisualizer component, object data)
	{
		component.OnSelectionChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<KAnimGridTileVisualizer> OnHighlightChangedDelegate = new EventSystem.IntraObjectHandler<KAnimGridTileVisualizer>(delegate(KAnimGridTileVisualizer component, object data)
	{
		component.OnHighlightChanged(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-1503271301, OnSelectionChangedDelegate);
		Subscribe(-1201923725, OnHighlightChangedDelegate);
	}

	protected override void OnCleanUp()
	{
		Building component = GetComponent<Building>();
		if ((Object)component != (Object)null)
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			ObjectLayer tileLayer = component.Def.TileLayer;
			if ((Object)Grid.Objects[cell, (int)tileLayer] == (Object)base.gameObject)
			{
				Grid.Objects[cell, (int)tileLayer] = null;
			}
			TileVisualizer.RefreshCell(cell, tileLayer, component.Def.ReplacementLayer);
		}
		base.OnCleanUp();
	}

	private void OnSelectionChanged(object data)
	{
		bool enabled = (bool)data;
		World.Instance.blockTileRenderer.SelectCell(Grid.PosToCell(base.transform.GetPosition()), enabled);
	}

	private void OnHighlightChanged(object data)
	{
		bool enabled = (bool)data;
		World.Instance.blockTileRenderer.HighlightCell(Grid.PosToCell(base.transform.GetPosition()), enabled);
	}

	public int GetBlockTileConnectorID()
	{
		return blockTileConnectorID;
	}
}
