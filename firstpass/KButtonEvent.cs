public class KButtonEvent : KInputEvent
{
	private bool[] mIsAction;

	public KButtonEvent(KInputController controller, InputEventType event_type, bool[] is_action)
		: base(controller, event_type)
	{
		mIsAction = is_action;
	}

	public bool TryConsume(Action action)
	{
		if (base.Consumed)
		{
			Debug.LogError(action.ToString() + " was already consumed", null);
		}
		if (action != Action.NumActions && mIsAction[(int)action])
		{
			base.Consumed = true;
		}
		return base.Consumed;
	}

	public bool IsAction(Action action)
	{
		return mIsAction[(int)action];
	}

	public Action GetAction()
	{
		for (int i = 0; i < mIsAction.Length; i++)
		{
			if (mIsAction[i])
			{
				return (Action)i;
			}
		}
		return Action.NumActions;
	}
}
