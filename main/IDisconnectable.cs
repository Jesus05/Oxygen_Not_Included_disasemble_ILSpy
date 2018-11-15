public interface IDisconnectable
{
	bool Connect();

	void Disconnect();

	bool IsDisconnected();
}
