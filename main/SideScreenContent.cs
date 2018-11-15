using UnityEngine;

public abstract class SideScreenContent : KScreen
{
	[SerializeField]
	protected string titleKey;

	public GameObject ContentContainer;

	public virtual void SetTarget(GameObject target)
	{
	}

	public virtual void ClearTarget()
	{
	}

	public abstract bool IsValidForTarget(GameObject target);

	public virtual string GetTitle()
	{
		return Strings.Get(titleKey);
	}
}
