using KSerialization;

public abstract class TargetMessage : Message
{
	[Serialize]
	private MessageTarget target;

	protected TargetMessage()
	{
	}

	public TargetMessage(KPrefabID prefab_id)
	{
		target = new MessageTarget(prefab_id);
	}

	public MessageTarget GetTarget()
	{
		return target;
	}

	public override void OnCleanUp()
	{
		target.OnCleanUp();
	}
}
