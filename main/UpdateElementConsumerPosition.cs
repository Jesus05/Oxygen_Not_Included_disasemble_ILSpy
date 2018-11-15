public class UpdateElementConsumerPosition : KMonoBehaviour, ISim200ms
{
	public void Sim200ms(float dt)
	{
		GetComponent<ElementConsumer>().RefreshConsumptionRate();
	}
}
