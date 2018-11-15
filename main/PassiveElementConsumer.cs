public class PassiveElementConsumer : ElementConsumer, IEffectDescriptor
{
	protected override bool IsActive()
	{
		return true;
	}
}
