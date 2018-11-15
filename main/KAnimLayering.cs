using UnityEngine;

public class KAnimLayering
{
	private bool isForeground;

	private KAnimControllerBase controller;

	private KAnimControllerBase foregroundController;

	private KAnimLink link;

	private Grid.SceneLayer layer = Grid.SceneLayer.BuildingFront;

	public static readonly KAnimHashedString UI = new KAnimHashedString("ui");

	public KAnimLayering(KAnimControllerBase controller, Grid.SceneLayer layer)
	{
		this.controller = controller;
		this.layer = layer;
	}

	public void SetLayer(Grid.SceneLayer layer)
	{
		this.layer = layer;
		if ((Object)foregroundController != (Object)null)
		{
			float layerZ = Grid.GetLayerZ(layer);
			Vector3 position = controller.gameObject.transform.GetPosition();
			Vector3 position2 = new Vector3(0f, 0f, layerZ - position.z - 0.1f);
			foregroundController.transform.SetLocalPosition(position2);
		}
	}

	public void SetIsForeground(bool is_foreground)
	{
		isForeground = is_foreground;
	}

	public bool GetIsForeground()
	{
		return isForeground;
	}

	private static bool IsAnimLayered(KAnimFile[] anims)
	{
		foreach (KAnimFile kAnimFile in anims)
		{
			if (!((Object)kAnimFile == (Object)null))
			{
				KAnimFileData data = kAnimFile.GetData();
				if (data.build != null)
				{
					KAnim.Build.Symbol[] symbols = data.build.symbols;
					for (int j = 0; j < symbols.Length; j++)
					{
						if ((symbols[j].flags & 8) != 0)
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	private void HideSymbolsInternal()
	{
		KAnimFile[] animFiles = controller.AnimFiles;
		foreach (KAnimFile kAnimFile in animFiles)
		{
			if (!((Object)kAnimFile == (Object)null))
			{
				KAnimFileData data = kAnimFile.GetData();
				if (data.build != null)
				{
					KAnim.Build.Symbol[] symbols = data.build.symbols;
					for (int j = 0; j < symbols.Length; j++)
					{
						bool flag = (symbols[j].flags & 8) != 0;
						if (flag != isForeground && !(symbols[j].hash == UI))
						{
							controller.SetSymbolVisiblity(symbols[j].hash, false);
						}
					}
				}
			}
		}
	}

	public void HideSymbols()
	{
		if (!((Object)EntityPrefabs.Instance == (Object)null) && !isForeground)
		{
			KAnimFile[] animFiles = controller.AnimFiles;
			bool flag = IsAnimLayered(animFiles);
			if (flag && (Object)foregroundController == (Object)null && layer != Grid.SceneLayer.NoLayer)
			{
				GameObject gameObject = Util.KInstantiate(EntityPrefabs.Instance.ForegroundLayer, controller.gameObject, null);
				gameObject.name = controller.name + "_fg";
				foregroundController = gameObject.GetComponent<KAnimControllerBase>();
				foregroundController.AnimFiles = animFiles;
				foregroundController.GetLayering().SetIsForeground(true);
				foregroundController.initialAnim = controller.initialAnim;
				link = new KAnimLink(controller, foregroundController);
				Dirty();
				KAnimSynchronizer synchronizer = controller.GetSynchronizer();
				synchronizer.Add(foregroundController);
				synchronizer.Sync(foregroundController);
				float layerZ = Grid.GetLayerZ(layer);
				Vector3 position = controller.gameObject.transform.GetPosition();
				Vector3 position2 = new Vector3(0f, 0f, layerZ - position.z - 0.1f);
				gameObject.transform.SetLocalPosition(position2);
				gameObject.SetActive(true);
			}
			else if (!flag && (Object)foregroundController != (Object)null)
			{
				controller.GetSynchronizer().Remove(foregroundController);
				foregroundController.gameObject.DeleteObject();
				link = null;
			}
			if ((Object)foregroundController != (Object)null)
			{
				HideSymbolsInternal();
				foregroundController.GetLayering()?.HideSymbolsInternal();
			}
		}
	}

	public void Dirty()
	{
		if (!((Object)foregroundController == (Object)null))
		{
			foregroundController.Offset = controller.Offset;
			foregroundController.Pivot = controller.Pivot;
			foregroundController.Rotation = controller.Rotation;
			foregroundController.FlipX = controller.FlipX;
			foregroundController.FlipY = controller.FlipY;
		}
	}
}
