using UnityEngine;

public static class SoundUtil
{
	public static float GetLiquidDepth(int cell)
	{
		float num = 0f;
		num += Grid.Mass[cell] * ((!Grid.Element[cell].IsLiquid) ? 0f : 1f);
		int num2 = Grid.CellBelow(cell);
		if (Grid.IsValidCell(num2))
		{
			num += Grid.Mass[num2] * ((!Grid.Element[num2].IsLiquid) ? 0f : 1f);
		}
		return Mathf.Min(num / 1000f, 1f);
	}

	public static float GetLiquidVolume(float mass)
	{
		return Mathf.Min(mass / 100f, 1f);
	}
}
