using UnityEngine;

public struct DiseaseContainer
{
	public AutoDisinfectable autoDisinfectable;

	public byte elemIdx;

	public bool isContainer;

	public ConduitType conduitType;

	public KBatchedAnimController controller;

	public GameObject visualDiseaseProvider;

	public int overpopulationCount;

	public float instanceGrowthRate;

	public float accumulatedError;

	public DiseaseContainer(GameObject go, byte elemIdx)
	{
		this.elemIdx = elemIdx;
		isContainer = (go.GetComponent<IUserControlledCapacity>() != null);
		Conduit component = go.GetComponent<Conduit>();
		if ((Object)component != (Object)null)
		{
			conduitType = component.type;
		}
		else
		{
			conduitType = ConduitType.None;
		}
		controller = go.GetComponent<KBatchedAnimController>();
		overpopulationCount = 1;
		instanceGrowthRate = 1f;
		accumulatedError = 0f;
		visualDiseaseProvider = null;
		autoDisinfectable = go.GetComponent<AutoDisinfectable>();
		if ((Object)autoDisinfectable != (Object)null)
		{
			AutoDisinfectableManager.Instance.AddAutoDisinfectable(autoDisinfectable);
		}
	}

	public void Clear()
	{
		controller = null;
	}
}
