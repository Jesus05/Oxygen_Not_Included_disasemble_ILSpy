using System;
using UnityEngine;

public class GlassForge : Refinery
{
	private Guid statusHandle;

	private static readonly EventSystem.IntraObjectHandler<GlassForge> CheckPipesDelegate = new EventSystem.IntraObjectHandler<GlassForge>(delegate(GlassForge component, object data)
	{
		component.CheckPipes(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-2094018600, CheckPipesDelegate);
	}

	private void CheckPipes(object data)
	{
		KSelectable component = GetComponent<KSelectable>();
		int cell = Grid.OffsetCell(Grid.PosToCell(this), GlassForgeConfig.outPipeOffset);
		GameObject gameObject = Grid.Objects[cell, 16];
		if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
		{
			PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
			if (component2.Element.highTemp > ElementLoader.FindElementByHash(SimHashes.MoltenGlass).lowTemp)
			{
				component.RemoveStatusItem(statusHandle, false);
			}
			else
			{
				statusHandle = component.AddStatusItem(Db.Get().BuildingStatusItems.PipeMayMelt, null);
			}
		}
		else
		{
			component.RemoveStatusItem(statusHandle, false);
		}
	}
}
