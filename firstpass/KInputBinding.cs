public class KInputBinding
{
	public KKeyCode mKeyCode;

	public Action mAction;

	public Modifier mModifier;

	public KInputBinding(KKeyCode key_code, Modifier modifier, Action action)
	{
		mKeyCode = key_code;
		mAction = action;
		mModifier = modifier;
	}
}
