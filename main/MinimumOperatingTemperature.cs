using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class MinimumOperatingTemperature : KMonoBehaviour, ISim200ms, IGameObjectEffectDescriptor
{
	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	public float minimumTemperature = 275.15f;

	private const float TURN_ON_DELAY = 5f;

	private float lastOffTime;

	public static Operational.Flag warmEnoughFlag = new Operational.Flag("warm_enough", Operational.Flag.Type.Functional);

	private bool isWarm;

	private HandleVector<int>.Handle partitionerEntry;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		TestTemperature(true);
	}

	public void Sim200ms(float dt)
	{
		TestTemperature(false);
	}

	private void TestTemperature(bool force)
	{
		bool flag = false;
		if (primaryElement.Temperature < minimumTemperature)
		{
			flag = false;
		}
		else
		{
			flag = true;
			for (int i = 0; i < building.PlacementCells.Length; i++)
			{
				int i2 = building.PlacementCells[i];
				float num = Grid.Temperature[i2];
				float num2 = Grid.Mass[i2];
				if ((num != 0f || num2 != 0f) && num < minimumTemperature)
				{
					flag = false;
					break;
				}
			}
		}
		if (!flag)
		{
			lastOffTime = Time.time;
		}
		if ((flag != isWarm && !flag) || (flag != isWarm && flag && Time.time > lastOffTime + 5f) || force)
		{
			isWarm = flag;
			operational.SetFlag(warmEnoughFlag, isWarm);
			GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.TooCold, !isWarm, this);
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor item = new Descriptor(string.Format(UI.BUILDINGEFFECTS.MINIMUM_TEMP, GameUtil.GetFormattedTemperature(minimumTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.MINIMUM_TEMP, GameUtil.GetFormattedTemperature(minimumTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), Descriptor.DescriptorType.Effect, false);
		list.Add(item);
		return list;
	}
}
