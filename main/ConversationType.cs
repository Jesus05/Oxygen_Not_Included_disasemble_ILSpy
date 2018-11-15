using UnityEngine;

public class ConversationType
{
	public string id;

	public string target;

	public virtual void NewTarget(MinionIdentity speaker)
	{
	}

	public virtual Conversation.Topic GetNextTopic(MinionIdentity speaker, Conversation.Topic lastTopic)
	{
		return null;
	}

	public virtual Sprite GetSprite(string topic)
	{
		return null;
	}
}
