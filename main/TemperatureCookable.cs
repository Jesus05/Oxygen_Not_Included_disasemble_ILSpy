using UnityEngine;

public class TemperatureCookable : KMonoBehaviour, ISim1000ms
{
	[MyCmpReq]
	private PrimaryElement element;

	public float cookTemperature = 273150f;

	public string cookedID;

	public void Sim1000ms(float dt)
	{
		if (element.Temperature > cookTemperature && cookedID != null)
		{
			Cook();
		}
	}

	private void Cook()
	{
		Vector3 position = base.transform.GetPosition();
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(cookedID), position);
		gameObject.SetActive(true);
		KSelectable component = base.gameObject.GetComponent<KSelectable>();
		if ((Object)SelectTool.Instance != (Object)null && (Object)SelectTool.Instance.selected != (Object)null && (Object)SelectTool.Instance.selected == (Object)component)
		{
			SelectTool.Instance.Select(gameObject.GetComponent<KSelectable>(), false);
		}
		PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
		component2.Temperature = element.Temperature;
		component2.Mass = element.Mass;
		base.gameObject.DeleteObject();
	}
}
