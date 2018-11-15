public abstract class MessageDialog : KMonoBehaviour
{
	public virtual bool CanDontShowAgain => false;

	public abstract bool CanDisplay(Message message);

	public abstract void SetMessage(Message message);

	public abstract void OnClickAction();

	public virtual void OnDontShowAgain()
	{
	}
}
