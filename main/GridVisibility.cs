using UnityEngine;

public class GridVisibility : KMonoBehaviour
{
	public float radius = 18f;

	public float innerRadius = 16.5f;

	protected override void OnSpawn()
	{
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnCellChange, "GridVisibility.OnSpawn");
		OnCellChange();
	}

	private void OnCellChange()
	{
		if (!base.gameObject.HasTag(GameTags.Dead))
		{
			int num = Grid.PosToCell(this);
			if (Grid.IsValidCell(num))
			{
				if (!Grid.Revealed[num])
				{
					Grid.PosToXY(base.transform.GetPosition(), out int x, out int y);
					Reveal(x, y, radius, innerRadius);
					Grid.Revealed[num] = true;
				}
				FogOfWarMask.ClearMask(num);
			}
		}
	}

	public static void Reveal(int baseX, int baseY, float radius, float innerRadius)
	{
		for (float num = 0f - radius; num <= radius; num += 1f)
		{
			for (float num2 = 0f - radius; num2 <= radius; num2 += 1f)
			{
				float num3 = (float)baseY + num;
				float num4 = (float)baseX + num2;
				if (!(num3 < 0f) && !((float)(Grid.HeightInCells - 1) < num3) && !(num4 < 0f) && !((float)(Grid.WidthInCells - 1) < num4))
				{
					int num5 = (int)(num3 * (float)Grid.WidthInCells + num4);
					byte b = Grid.Visible[num5];
					if (b < 255)
					{
						float num6 = Mathf.Lerp(1f, 0f, (new Vector2(num2, num).magnitude - innerRadius) / (radius - innerRadius));
						Grid.Reveal(num5, (byte)(255f * num6));
					}
				}
			}
		}
		int num7 = Mathf.CeilToInt(radius);
		Game.Instance.UpdateGameActiveRegion(baseX - num7, baseY - num7, baseX + num7, baseY + num7);
	}

	protected override void OnCleanUp()
	{
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnCellChange);
	}
}
