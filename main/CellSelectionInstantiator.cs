using UnityEngine;

public class CellSelectionInstantiator : MonoBehaviour
{
	public GameObject CellSelectionPrefab;

	private void Awake()
	{
		GameObject gameObject = Util.KInstantiate(CellSelectionPrefab, null, "WorldSelectionCollider");
		GameObject gameObject2 = Util.KInstantiate(CellSelectionPrefab, null, "WorldSelectionCollider");
		CellSelectionObject component = gameObject.GetComponent<CellSelectionObject>();
		(component.alternateSelectionObject = gameObject2.GetComponent<CellSelectionObject>()).alternateSelectionObject = component;
	}
}
