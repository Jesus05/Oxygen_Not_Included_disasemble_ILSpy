using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class Ladder : KMonoBehaviour, IEffectDescriptor
{
	public float upwardsMovementSpeedMultiplier = 1f;

	public float downwardsMovementSpeedMultiplier = 1f;

	public bool isPole = false;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		int i = Grid.PosToCell(this);
		Grid.HasPole[i] = isPole;
		Grid.HasLadder[i] = !isPole;
		GetComponent<KPrefabID>().AddTag(GameTags.Ladders, false);
		Components.Ladders.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Normal, null);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		int num = Grid.PosToCell(this);
		GameObject x = Grid.Objects[num, 24];
		if ((Object)x == (Object)null)
		{
			Grid.HasPole[num] = false;
			Grid.HasLadder[num] = false;
		}
		Components.Ladders.Remove(this);
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = null;
		if (upwardsMovementSpeedMultiplier != 1f)
		{
			list = new List<Descriptor>();
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.DUPLICANTMOVEMENTBOOST, GameUtil.GetFormattedPercent(upwardsMovementSpeedMultiplier * 100f - 100f, GameUtil.TimeSlice.None)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.DUPLICANTMOVEMENTBOOST, GameUtil.GetFormattedPercent(upwardsMovementSpeedMultiplier * 100f - 100f, GameUtil.TimeSlice.None)), Descriptor.DescriptorType.Effect);
			list.Add(item);
		}
		return list;
	}
}
