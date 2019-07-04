using System.Collections.Generic;
using UnityEngine;

public class KBatchedAnimTracker : MonoBehaviour
{
	[SerializeField]
	public KBatchedAnimController controller;

	[SerializeField]
	public Vector3 offset = Vector3.zero;

	public HashedString symbol;

	public Vector3 targetPoint = Vector3.zero;

	public bool useTargetPoint = false;

	public bool fadeOut = true;

	public bool skipInitialDisable = false;

	public bool forceAlwaysVisible = false;

	private bool alive = true;

	private bool forceUpdate = false;

	private Matrix2x3 previousMatrix;

	private Vector3 previousPosition;

	private KBatchedAnimController myAnim = null;

	private void Start()
	{
		if ((Object)controller == (Object)null)
		{
			Transform parent = base.transform.parent;
			while ((Object)parent != (Object)null)
			{
				controller = parent.GetComponent<KBatchedAnimController>();
				if ((Object)controller != (Object)null)
				{
					break;
				}
				parent = parent.parent;
			}
		}
		if ((Object)controller == (Object)null)
		{
			Debug.Log("Controller Null for tracker on " + base.gameObject.name, base.gameObject);
			base.enabled = false;
		}
		else
		{
			controller.onAnimEnter += OnAnimStart;
			controller.onAnimComplete += OnAnimStop;
			controller.onLayerChanged += OnLayerChanged;
			forceUpdate = true;
			myAnim = GetComponent<KBatchedAnimController>();
			List<KAnimControllerBase> list = new List<KAnimControllerBase>(GetComponentsInChildren<KAnimControllerBase>(true));
			if (!skipInitialDisable)
			{
				for (int i = 0; i < base.transform.childCount; i++)
				{
					base.transform.GetChild(i).gameObject.SetActive(false);
				}
			}
			for (int num = list.Count - 1; num >= 0; num--)
			{
				if ((Object)list[num].gameObject == (Object)base.gameObject)
				{
					list.RemoveAt(num);
				}
			}
		}
	}

	private void OnDestroy()
	{
		if ((Object)controller != (Object)null)
		{
			controller.onAnimEnter -= OnAnimStart;
			controller.onAnimComplete -= OnAnimStop;
			controller.onLayerChanged -= OnLayerChanged;
			controller = null;
		}
		myAnim = null;
	}

	private void LateUpdate()
	{
		if ((Object)controller != (Object)null && (controller.IsVisible() || forceAlwaysVisible || forceUpdate))
		{
			UpdateFrame();
		}
		if (!alive)
		{
			base.enabled = false;
		}
	}

	private void UpdateFrame()
	{
		forceUpdate = false;
		bool symbolVisible = false;
		KAnim.Anim currentAnim = controller.CurrentAnim;
		if (currentAnim != null)
		{
			Matrix2x3 symbolLocalTransform = controller.GetSymbolLocalTransform(symbol, out symbolVisible);
			Vector3 position = controller.transform.GetPosition();
			if (symbolVisible && (previousMatrix != symbolLocalTransform || position != previousPosition || useTargetPoint))
			{
				previousMatrix = symbolLocalTransform;
				previousPosition = position;
				Matrix2x3 overrideTransformMatrix = controller.GetTransformMatrix() * symbolLocalTransform;
				Vector3 position2 = base.transform.GetPosition();
				float z = position2.z;
				base.transform.SetPosition(overrideTransformMatrix.MultiplyPoint(offset));
				if (useTargetPoint)
				{
					Vector3 position3 = base.transform.GetPosition();
					position3.z = 0f;
					Vector3 from = targetPoint - position3;
					float num = Vector3.Angle(from, Vector3.right);
					if (from.y < 0f)
					{
						num = 360f - num;
					}
					base.transform.localRotation = Quaternion.identity;
					base.transform.RotateAround(position3, new Vector3(0f, 0f, 1f), num);
					float sqrMagnitude = from.sqrMagnitude;
					KBatchedAnimInstanceData batchInstanceData = myAnim.GetBatchInstanceData();
					Vector3 position4 = base.transform.GetPosition();
					float x = position4.x;
					Vector3 position5 = base.transform.GetPosition();
					batchInstanceData.SetClipRadius(x, position5.y, sqrMagnitude, true);
				}
				else
				{
					Vector3 v = (!controller.FlipX) ? Vector3.right : Vector3.left;
					Vector3 v2 = (!controller.FlipY) ? Vector3.up : Vector3.down;
					base.transform.up = overrideTransformMatrix.MultiplyVector(v2);
					base.transform.right = overrideTransformMatrix.MultiplyVector(v);
					if ((Object)myAnim != (Object)null)
					{
						myAnim.GetBatchInstanceData()?.SetOverrideTransformMatrix(overrideTransformMatrix);
					}
				}
				Transform transform = base.transform;
				Vector3 position6 = base.transform.GetPosition();
				float x2 = position6.x;
				Vector3 position7 = base.transform.GetPosition();
				transform.SetPosition(new Vector3(x2, position7.y, z));
				myAnim.SetDirty();
			}
		}
		if ((Object)myAnim != (Object)null && symbolVisible != myAnim.enabled)
		{
			myAnim.enabled = symbolVisible;
		}
	}

	[ContextMenu("ForceAlive")]
	private void OnAnimStart(HashedString name)
	{
		alive = true;
		base.enabled = true;
		forceUpdate = true;
	}

	private void OnAnimStop(HashedString name)
	{
		alive = false;
	}

	private void OnLayerChanged(int layer)
	{
		myAnim.SetLayer(layer);
	}

	public void SetTarget(Vector3 target)
	{
		targetPoint = target;
		targetPoint.z = 0f;
	}
}
