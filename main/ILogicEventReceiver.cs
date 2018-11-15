public interface ILogicEventReceiver : ILogicNetworkConnection
{
	void ReceiveLogicEvent(int value);

	int GetLogicCell();
}
