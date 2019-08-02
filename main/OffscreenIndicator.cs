using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffscreenIndicator : KMonoBehaviour
{
	public GameObject IndicatorPrefab;

	public GameObject IndicatorContainer;

	private Dictionary<GameObject, GameObject> targets = new Dictionary<GameObject, GameObject>();

	public static OffscreenIndicator Instance;

	[SerializeField]
	private float edgeInset = 25f;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Instance = this;
	}

	private void Update()
	{
		foreach (KeyValuePair<GameObject, GameObject> target in targets)
		{
			UpdateArrow(target.Value, target.Key);
		}
	}

	public void ActivateIndicator(GameObject target)
	{
		if (!targets.ContainsKey(target))
		{
			Tuple<Sprite, Color> uISprite = Def.GetUISprite(target, "ui", false);
			if (uISprite != null)
			{
				ActivateIndicator(target, uISprite);
			}
		}
	}

	public void ActivateIndicator(GameObject target, GameObject iconSource)
	{
		if (!targets.ContainsKey(target))
		{
			MinionIdentity component = iconSource.GetComponent<MinionIdentity>();
			if ((Object)component != (Object)null)
			{
				GameObject gameObject = Util.KInstantiateUI(IndicatorPrefab, IndicatorContainer, true);
				Image reference = gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("icon");
				reference.gameObject.SetActive(false);
				CrewPortrait reference2 = gameObject.GetComponent<HierarchyReferences>().GetReference<CrewPortrait>("Portrait");
				reference2.gameObject.SetActive(true);
				reference2.SetIdentityObject(component, true);
				targets.Add(target, gameObject);
			}
		}
	}

	public void ActivateIndicator(GameObject target, Tuple<Sprite, Color> icon)
	{
		if (!targets.ContainsKey(target))
		{
			GameObject gameObject = Util.KInstantiateUI(IndicatorPrefab, IndicatorContainer, true);
			Image reference = gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("icon");
			if (icon != null)
			{
				reference.sprite = icon.first;
				reference.color = icon.second;
				targets.Add(target, gameObject);
			}
		}
	}

	public void DeactivateIndicator(GameObject target)
	{
		if (targets.ContainsKey(target))
		{
			Object.Destroy(targets[target]);
			targets.Remove(target);
		}
	}

	private void UpdateArrow(GameObject arrow, GameObject target)
	{
		if ((Object)target == (Object)null)
		{
			Object.Destroy(arrow);
			targets.Remove(target);
		}
		else
		{
			Vector3 vector = Camera.main.WorldToViewportPoint(target.transform.position);
			if ((double)vector.x > 0.3 && (double)vector.x < 0.7 && (double)vector.y > 0.3 && (double)vector.y < 0.7)
			{
				arrow.GetComponent<HierarchyReferences>().GetReference<CrewPortrait>("Portrait").SetIdentityObject(null, true);
				arrow.SetActive(false);
			}
			else
			{
				arrow.SetActive(true);
				arrow.rectTransform().SetLocalPosition(Vector3.zero);
				Vector3 b = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
				Vector3 position = target.transform.position;
				b.z = position.z;
				Vector3 normalized = (target.transform.position - b).normalized;
				arrow.transform.up = normalized;
				UpdateTargetIconPosition(target, arrow);
			}
		}
	}

	private void UpdateTargetIconPosition(GameObject goTarget, GameObject indicator)
	{
		Vector3 vector = goTarget.transform.position;
		vector = Camera.main.WorldToViewportPoint(vector);
		if (vector.z < 0f)
		{
			vector.x = 1f - vector.x;
			vector.y = 1f - vector.y;
			vector.z = 0f;
			vector = Vector3Maxamize(vector);
		}
		vector = Camera.main.ViewportToScreenPoint(vector);
		vector.x = Mathf.Clamp(vector.x, edgeInset, (float)Screen.width - edgeInset);
		vector.y = Mathf.Clamp(vector.y, edgeInset, (float)Screen.height - edgeInset);
		indicator.transform.position = vector;
		indicator.GetComponent<HierarchyReferences>().GetReference<Image>("icon").rectTransform.up = Vector3.up;
		indicator.GetComponent<HierarchyReferences>().GetReference<CrewPortrait>("Portrait").transform.up = Vector3.up;
	}

	public Vector3 Vector3Maxamize(Vector3 vector)
	{
		Vector3 a = vector;
		float num = 0f;
		num = ((!(vector.x > num)) ? num : vector.x);
		num = ((!(vector.y > num)) ? num : vector.y);
		num = ((!(vector.z > num)) ? num : vector.z);
		return a / num;
	}
}
