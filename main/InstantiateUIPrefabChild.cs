using UnityEngine;

public class InstantiateUIPrefabChild : KMonoBehaviour
{
	public GameObject[] prefabs;

	public bool InstantiateOnAwake = true;

	private bool alreadyInstantiated;

	public bool setAsFirstSibling;

	protected override void OnPrefabInit()
	{
		if (InstantiateOnAwake)
		{
			Instantiate();
		}
	}

	public void Instantiate()
	{
		if (alreadyInstantiated)
		{
			Debug.LogWarning(base.gameObject.name + "trying to instantiate UI prefabs multiple times.");
		}
		else
		{
			alreadyInstantiated = true;
			GameObject[] array = prefabs;
			foreach (GameObject gameObject in array)
			{
				if (!((Object)gameObject == (Object)null))
				{
					Vector3 v = gameObject.rectTransform().anchoredPosition;
					GameObject gameObject2 = Object.Instantiate(gameObject);
					gameObject2.transform.SetParent(base.transform);
					gameObject2.rectTransform().anchoredPosition = v;
					gameObject2.rectTransform().localScale = Vector3.one;
					if (setAsFirstSibling)
					{
						gameObject2.transform.SetAsFirstSibling();
					}
				}
			}
		}
	}
}
