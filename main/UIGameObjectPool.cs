using System;
using System.Collections.Generic;
using UnityEngine;

public class UIGameObjectPool
{
	private GameObject prefab;

	private List<GameObject> freeElements = new List<GameObject>();

	private List<GameObject> activeElements = new List<GameObject>();

	public Transform disabledElementParent;

	public int ActiveElementsCount => activeElements.Count;

	public int FreeElementsCount => freeElements.Count;

	public int TotalElementsCount => ActiveElementsCount + FreeElementsCount;

	public UIGameObjectPool(GameObject prefab)
	{
		this.prefab = prefab;
		freeElements = new List<GameObject>();
		activeElements = new List<GameObject>();
	}

	public GameObject GetFreeElement(GameObject instantiateParent = null, bool forceActive = false)
	{
		if (freeElements.Count == 0)
		{
			activeElements.Add(Util.KInstantiateUI(prefab.gameObject, instantiateParent, false));
		}
		else
		{
			GameObject gameObject = freeElements[0];
			activeElements.Add(gameObject);
			if ((UnityEngine.Object)gameObject.transform.parent != (UnityEngine.Object)instantiateParent)
			{
				gameObject.transform.SetParent(instantiateParent.transform);
			}
			freeElements.RemoveAt(0);
		}
		GameObject gameObject2 = activeElements[activeElements.Count - 1];
		if (gameObject2.gameObject.activeInHierarchy != forceActive)
		{
			gameObject2.gameObject.SetActive(forceActive);
		}
		return gameObject2;
	}

	public void ClearElement(GameObject element)
	{
		if (!activeElements.Contains(element))
		{
			string obj = (!freeElements.Contains(element)) ? (element.name + ": The element provided does not belong to this pool") : (element.name + ": The element provided is already inactive");
			element.SetActive(false);
			if ((UnityEngine.Object)disabledElementParent != (UnityEngine.Object)null)
			{
				element.transform.SetParent(disabledElementParent);
			}
			Debug.LogError(obj);
		}
		else
		{
			if ((UnityEngine.Object)disabledElementParent != (UnityEngine.Object)null)
			{
				element.transform.SetParent(disabledElementParent);
			}
			element.SetActive(false);
			freeElements.Add(element);
			activeElements.Remove(element);
		}
	}

	public void ClearAll()
	{
		while (activeElements.Count > 0)
		{
			if ((UnityEngine.Object)disabledElementParent != (UnityEngine.Object)null)
			{
				activeElements[0].transform.SetParent(disabledElementParent);
			}
			activeElements[0].SetActive(false);
			freeElements.Add(activeElements[0]);
			activeElements.RemoveAt(0);
		}
	}

	public void DestroyAll()
	{
		DestroyAllActive();
		DestroyAllFree();
	}

	public void DestroyAllActive()
	{
		activeElements.ForEach(delegate(GameObject ae)
		{
			UnityEngine.Object.Destroy(ae);
		});
		activeElements.Clear();
	}

	public void DestroyAllFree()
	{
		freeElements.ForEach(delegate(GameObject ae)
		{
			UnityEngine.Object.Destroy(ae);
		});
		freeElements.Clear();
	}

	public void ForEachActiveElement(Action<GameObject> predicate)
	{
		activeElements.ForEach(predicate);
	}

	public void ForEachFreeElement(Action<GameObject> predicate)
	{
		freeElements.ForEach(predicate);
	}
}
