using UnityEngine;

public class AnimEventHandler : KMonoBehaviour
{
	private delegate void SetPos(Vector3 pos);

	private KBatchedAnimController controller;

	private KBoxCollider2D animCollider;

	private Vector3 targetPos;

	private Vector2 baseOffset;

	private HashedString context;

	private event SetPos onWorkTargetSet;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KBatchedAnimTracker[] componentsInChildren = GetComponentsInChildren<KBatchedAnimTracker>(true);
		KBatchedAnimTracker[] array = componentsInChildren;
		foreach (KBatchedAnimTracker kBatchedAnimTracker in array)
		{
			if (kBatchedAnimTracker.useTargetPoint)
			{
				onWorkTargetSet += kBatchedAnimTracker.SetTarget;
			}
		}
		controller = GetComponent<KBatchedAnimController>();
		animCollider = GetComponent<KBoxCollider2D>();
		baseOffset = animCollider.offset;
	}

	public HashedString GetContext()
	{
		return context;
	}

	public void UpdateWorkTarget(Vector3 pos)
	{
		if (this.onWorkTargetSet != null)
		{
			this.onWorkTargetSet(pos);
		}
	}

	public void SetContext(HashedString context)
	{
		this.context = context;
	}

	public void SetTargetPos(Vector3 target_pos)
	{
		targetPos = target_pos;
	}

	public Vector3 GetTargetPos()
	{
		return targetPos;
	}

	public void ClearContext()
	{
		context = default(HashedString);
	}

	public void LateUpdate()
	{
		Vector3 pivotSymbolPosition = controller.GetPivotSymbolPosition();
		KBoxCollider2D kBoxCollider2D = animCollider;
		float num = baseOffset.x + pivotSymbolPosition.x;
		Vector3 position = base.transform.GetPosition();
		float x = num - position.x;
		float num2 = baseOffset.y + pivotSymbolPosition.y;
		Vector3 position2 = base.transform.GetPosition();
		kBoxCollider2D.offset = new Vector2(x, num2 - position2.y);
	}
}
