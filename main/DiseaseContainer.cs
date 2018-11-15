using UnityEngine;

public struct DiseaseContainer
{
	public PrimaryElement primaryElement;

	public AutoDisinfectable autoDisinfectable;

	public byte elemIdx;

	public byte diseaseIdx;

	public int diseaseCount;

	public bool isContainer;

	public ConduitType conduitType;

	public KBatchedAnimController controller;

	public GameObject visualDiseaseProvider;

	public int overpopulationCount;

	public float instanceGrowthRate;

	public float accumulatedError;

	public DiseaseContainer(GameObject go, byte disease_idx, int disease_count)
	{
		primaryElement = go.GetComponent<PrimaryElement>();
		elemIdx = primaryElement.Element.idx;
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
		diseaseIdx = disease_idx;
		diseaseCount = disease_count;
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

	public void GetVisualDiseaseIdxAndCount(out int disease_idx, out int disease_count)
	{
		disease_idx = diseaseIdx;
		disease_count = diseaseCount;
		if ((Object)visualDiseaseProvider != (Object)null)
		{
			disease_idx = 255;
			disease_count = 0;
			HandleVector<int>.Handle handle = GameComps.DiseaseContainers.GetHandle(visualDiseaseProvider);
			if (handle != HandleVector<int>.InvalidHandle)
			{
				DiseaseContainer data = GameComps.DiseaseContainers.GetData(handle);
				disease_idx = data.diseaseIdx;
				disease_count = data.diseaseCount;
			}
		}
	}

	public void Clear()
	{
		controller = null;
	}
}
