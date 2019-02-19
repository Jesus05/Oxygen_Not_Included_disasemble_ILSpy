using STRINGS;
using System.Collections.Generic;

[SkipSaveFileSerialization]
public class Ladder : KMonoBehaviour, IEffectDescriptor
{
	public float upwardsMovementSpeedMultiplier = 1f;

	public float downwardsMovementSpeedMultiplier = 1f;

	public bool isPole;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (isPole)
		{
			Grid.HasPole[Grid.PosToCell(this)] = true;
		}
		else
		{
			Grid.HasLadder[Grid.PosToCell(this)] = true;
		}
		GetComponent<KPrefabID>().AddTag(GameTags.Ladders);
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
		if (isPole)
		{
			Grid.HasPole[Grid.PosToCell(this)] = false;
		}
		else
		{
			Grid.HasLadder[Grid.PosToCell(this)] = false;
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
