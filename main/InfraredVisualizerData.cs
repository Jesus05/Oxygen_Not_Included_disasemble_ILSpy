using Klei.AI;
using UnityEngine;

public struct InfraredVisualizerData
{
	public KAnimControllerBase controller;

	public AmountInstance temperatureAmount;

	public HandleVector<int>.Handle structureTemperature;

	public PrimaryElement primaryElement;

	public TemperatureVulnerable temperatureVulnerable;

	public InfraredVisualizerData(GameObject go)
	{
		controller = go.GetComponent<KBatchedAnimController>();
		if ((Object)controller != (Object)null)
		{
			temperatureAmount = Db.Get().Amounts.Temperature.Lookup(go);
			structureTemperature = GameComps.StructureTemperatures.GetHandle(go);
			primaryElement = go.GetComponent<PrimaryElement>();
			temperatureVulnerable = go.GetComponent<TemperatureVulnerable>();
		}
		else
		{
			temperatureAmount = null;
			structureTemperature = HandleVector<int>.InvalidHandle;
			primaryElement = null;
			temperatureVulnerable = null;
		}
	}

	public void Update()
	{
		float num = 0f;
		if (temperatureAmount != null)
		{
			num = temperatureAmount.value;
		}
		else if (structureTemperature.IsValid())
		{
			num = GameComps.StructureTemperatures.GetData(structureTemperature).Temperature;
		}
		else if ((Object)primaryElement != (Object)null)
		{
			num = primaryElement.Temperature;
		}
		else if ((Object)temperatureVulnerable != (Object)null)
		{
			num = temperatureVulnerable.InternalTemperature;
		}
		if (!(num < 0f))
		{
			Color32 c = SimDebugView.Instance.NormalizedTemperature(num);
			controller.OverlayColour = c;
		}
	}
}
