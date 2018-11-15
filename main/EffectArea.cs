using Klei.AI;
using System;

public class EffectArea : KMonoBehaviour
{
	public string EffectName;

	public int Area;

	private Effect Effect;

	protected override void OnPrefabInit()
	{
		Effect = Db.Get().effects.Get(EffectName);
	}

	private void Update()
	{
		int x = 0;
		int y = 0;
		Grid.PosToXY(base.transform.GetPosition(), out x, out y);
		foreach (MinionIdentity item in Components.MinionIdentities.Items)
		{
			int x2 = 0;
			int y2 = 0;
			Grid.PosToXY(item.transform.GetPosition(), out x2, out y2);
			if (Math.Abs(x2 - x) <= Area && Math.Abs(y2 - y) <= Area)
			{
				item.GetComponent<Effects>().Add(Effect, true);
			}
		}
	}
}
