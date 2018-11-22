using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class MicrobeMusher : ComplexFabricator
{
	[SerializeField]
	public Vector3 mushbarSpawnOffset = Vector3.right;

	private static readonly KAnimHashedString meterRationHash = new KAnimHashedString("meter_ration");

	private static readonly KAnimHashedString canHash = new KAnimHashedString("can");

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		choreType = Db.Get().ChoreTypes.Cook;
		choreTags = GameTags.ChoreTypes.CookingChores;
		base.GetWorkable.WorkerStatusItem = Db.Get().DuplicantStatusItems.Mushing;
		base.GetWorkable.AttributeConvertor = Db.Get().AttributeConverters.CookingSpeed;
		base.GetWorkable.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		fetchChoreTypeIdHash = Db.Get().ChoreTypes.CookFetch.IdHash;
		workable.meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, "meter_target", "meter_ration");
		workable.meter.meterController.SetSymbolVisiblity(canHash, false);
		workable.meter.meterController.SetSymbolVisiblity(meterRationHash, false);
		workable.meter.meterController.GetComponent<KBatchedAnimTracker>().skipInitialDisable = true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.Schedule("WaterFetchingTutorial", 2f, delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_FetchingWater);
		}, null, null);
	}

	protected override List<GameObject> SpawnOrderProduct(UserOrder completed_order)
	{
		List<GameObject> list = base.SpawnOrderProduct(completed_order);
		foreach (GameObject item in list)
		{
			if ((Object)item.GetComponent<PrimaryElement>() != (Object)null && item.GetComponent<PrimaryElement>().DiseaseCount > 0)
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_DiseaseCooking);
			}
		}
		return list;
	}
}
