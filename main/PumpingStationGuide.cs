using UnityEngine;

public class PumpingStationGuide : KMonoBehaviour, IRenderEveryTick
{
	private int previousDepthAvailable = -1;

	public GameObject parent;

	public bool occupyTiles;

	private KBatchedAnimController parentController;

	private KBatchedAnimController guideController;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		parentController = parent.GetComponent<KBatchedAnimController>();
		guideController = GetComponent<KBatchedAnimController>();
		RefreshTint();
		RefreshDepthAvailable();
	}

	private void RefreshTint()
	{
		guideController.TintColour = parentController.TintColour;
	}

	private void RefreshDepthAvailable()
	{
		int depthAvailable = GetDepthAvailable(Grid.PosToCell(this), parent);
		if (depthAvailable != previousDepthAvailable)
		{
			KBatchedAnimController component = GetComponent<KBatchedAnimController>();
			if (depthAvailable == 0)
			{
				component.enabled = false;
			}
			else
			{
				component.enabled = true;
				component.Play(new HashedString("place_pipe" + depthAvailable.ToString()), KAnim.PlayMode.Once, 1f, 0f);
			}
			if (occupyTiles)
			{
				OccupyArea(parent, depthAvailable);
			}
			previousDepthAvailable = depthAvailable;
		}
	}

	public void RenderEveryTick(float dt)
	{
		RefreshTint();
		RefreshDepthAvailable();
	}

	public static void OccupyArea(GameObject go, int depth_available)
	{
		int cell = Grid.PosToCell(go.transform.GetPosition());
		for (int i = 1; i <= depth_available; i++)
		{
			int key = Grid.OffsetCell(cell, 0, -i);
			int key2 = Grid.OffsetCell(cell, 1, -i);
			Grid.ObjectLayers[1][key] = go;
			Grid.ObjectLayers[1][key2] = go;
		}
	}

	public static int GetDepthAvailable(int root_cell, GameObject pump)
	{
		int num = 4;
		int result = 0;
		for (int i = 1; i <= num; i++)
		{
			int num2 = Grid.OffsetCell(root_cell, 0, -i);
			int num3 = Grid.OffsetCell(root_cell, 1, -i);
			if (!Grid.IsValidCell(num2) || Grid.Solid[num2] || !Grid.IsValidCell(num3) || Grid.Solid[num3] || (Grid.ObjectLayers[1].ContainsKey(num2) && !((Object)Grid.ObjectLayers[1][num2] == (Object)null) && !((Object)Grid.ObjectLayers[1][num2] == (Object)pump)) || (Grid.ObjectLayers[1].ContainsKey(num3) && !((Object)Grid.ObjectLayers[1][num3] == (Object)null) && !((Object)Grid.ObjectLayers[1][num3] == (Object)pump)))
			{
				break;
			}
			result = i;
		}
		return result;
	}
}
