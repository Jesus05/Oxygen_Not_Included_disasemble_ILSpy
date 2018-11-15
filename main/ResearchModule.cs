using TUNING;
using UnityEngine;

public class ResearchModule : KMonoBehaviour
{
	private static readonly EventSystem.IntraObjectHandler<ResearchModule> OnLaunchDelegate = new EventSystem.IntraObjectHandler<ResearchModule>(delegate(ResearchModule component, object data)
	{
		component.OnLaunch(data);
	});

	private static readonly EventSystem.IntraObjectHandler<ResearchModule> OnLandDelegate = new EventSystem.IntraObjectHandler<ResearchModule>(delegate(ResearchModule component, object data)
	{
		component.OnLand(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GetComponent<KBatchedAnimController>().Play("grounded", KAnim.PlayMode.Loop, 1f, 0f);
		Subscribe(-1056989049, OnLaunchDelegate);
		Subscribe(238242047, OnLandDelegate);
	}

	public void OnLaunch(object data)
	{
	}

	public void OnLand(object data)
	{
		SpaceDestination destination = SpacecraftManager.instance.GetDestination(SpacecraftManager.instance.savedSpacecraftDestinations[SpacecraftManager.instance.GetSpacecraftID(GetComponent<RocketModule>().conditionManager.GetComponent<LaunchableRocket>())]);
		SpaceDestination.ResearchOpportunity researchOpportunity = destination.TryCompleteResearchOpportunity();
		if (researchOpportunity != null)
		{
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab("ResearchDatabank"), base.gameObject.transform.GetPosition(), Grid.SceneLayer.Ore, null, 0);
			gameObject.SetActive(true);
			gameObject.GetComponent<PrimaryElement>().Mass = (float)researchOpportunity.dataValue;
			if (!string.IsNullOrEmpty(researchOpportunity.discoveredRareItem))
			{
				GameObject prefab = Assets.GetPrefab(researchOpportunity.discoveredRareItem);
				if ((Object)prefab == (Object)null)
				{
					KCrashReporter.Assert(false, "Missing prefab: " + researchOpportunity.discoveredRareItem);
				}
				else
				{
					GameObject gameObject2 = GameUtil.KInstantiate(prefab, base.gameObject.transform.GetPosition(), Grid.SceneLayer.Ore, null, 0);
					gameObject2.SetActive(true);
				}
			}
		}
		GameObject gameObject3 = GameUtil.KInstantiate(Assets.GetPrefab("ResearchDatabank"), base.gameObject.transform.GetPosition(), Grid.SceneLayer.Ore, null, 0);
		gameObject3.SetActive(true);
		gameObject3.GetComponent<PrimaryElement>().Mass = (float)ROCKETRY.DESTINATION_RESEARCH.EVERGREEN;
	}
}
