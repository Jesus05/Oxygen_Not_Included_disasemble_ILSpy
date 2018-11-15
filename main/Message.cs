using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public abstract class Message : ISaveLoadable
{
	public abstract string GetTitle();

	public abstract string GetSound();

	public abstract string GetMessageBody();

	public abstract string GetTooltip();

	public virtual bool ShowDialog()
	{
		return true;
	}

	public virtual void OnCleanUp()
	{
	}

	public virtual bool IsValid()
	{
		return true;
	}

	public virtual bool PlayNotificationSound()
	{
		return true;
	}

	public virtual void OnClick()
	{
	}
}
