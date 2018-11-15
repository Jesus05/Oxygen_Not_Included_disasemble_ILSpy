using KSerialization;
using System.Collections.Generic;

public class Messenger : KMonoBehaviour
{
	[Serialize]
	private SerializedList<Message> messages = new SerializedList<Message>();

	public static Messenger Instance;

	public int Count => messages.Count;

	public SerializedList<Message> Messages => messages;

	public IEnumerator<Message> GetEnumerator()
	{
		return messages.GetEnumerator();
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
	}

	protected override void OnSpawn()
	{
		int num = 0;
		while (num < messages.Count)
		{
			if (messages[num].IsValid())
			{
				num++;
			}
			else
			{
				messages.RemoveAt(num);
			}
		}
		Trigger(-599791736, null);
	}

	public void QueueMessage(Message message)
	{
		messages.Add(message);
		Trigger(1558809273, message);
	}

	public Message DequeueMessage()
	{
		Message result = null;
		if (messages.Count > 0)
		{
			result = messages[0];
			messages.RemoveAt(0);
		}
		return result;
	}

	public void ClearAllMessages()
	{
		for (int num = messages.Count - 1; num >= 0; num--)
		{
			messages.RemoveAt(num);
		}
	}

	public void RemoveMessage(Message m)
	{
		messages.Remove(m);
		Trigger(-599791736, null);
	}
}
