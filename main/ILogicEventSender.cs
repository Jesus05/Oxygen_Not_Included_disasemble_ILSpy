public interface ILogicEventSender : ILogicNetworkConnection
{
	void LogicTick();

	int GetLogicCell();

	int GetLogicValue();
}
