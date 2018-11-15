using UnityEngine.UI;

public class KLayoutElement : LayoutElement
{
	public bool makeDirtyOnDisable = true;

	private bool hasEnabledOnce;

	protected override void OnEnable()
	{
		bool flag = makeDirtyOnDisable;
		if (!hasEnabledOnce)
		{
			hasEnabledOnce = true;
			flag = true;
		}
		if (flag)
		{
			base.OnEnable();
		}
	}

	protected override void OnDisable()
	{
		if (makeDirtyOnDisable)
		{
			base.OnDisable();
		}
	}
}
