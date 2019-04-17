using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPool<T> where T : MonoBehaviour
{
	private T prefab;

	private List<T> freeElements = new List<T>();

	private List<T> activeElements = new List<T>();

	public Transform disabledElementParent;

	public int ActiveElementsCount => activeElements.Count;

	public int FreeElementsCount => freeElements.Count;

	public int TotalElementsCount => ActiveElementsCount + FreeElementsCount;

	public UIPool(T prefab)
	{
		this.prefab = prefab;
		freeElements = new List<T>();
		activeElements = new List<T>();
	}

	public T GetFreeElement(GameObject instantiateParent = null, bool forceActive = false)
	{
		if (freeElements.Count == 0)
		{
			activeElements.Add(Util.KInstantiateUI<T>(prefab.gameObject, instantiateParent, false));
		}
		else
		{
			T item = freeElements[0];
			activeElements.Add(item);
			if ((UnityEngine.Object)item.transform.parent != (UnityEngine.Object)instantiateParent)
			{
				item.transform.SetParent(instantiateParent.transform);
			}
			freeElements.RemoveAt(0);
		}
		T result = activeElements[activeElements.Count - 1];
		if (result.gameObject.activeInHierarchy != forceActive)
		{
			result.gameObject.SetActive(forceActive);
		}
		return result;
	}

	public void ClearElement(T element)
	{
		if (!activeElements.Contains(element))
		{
			string obj = (!freeElements.Contains(element)) ? "The element provided does not belong to this pool" : "The element provided is already inactive";
			Debug.LogError(obj);
		}
		else
		{
			if ((UnityEngine.Object)disabledElementParent != (UnityEngine.Object)null)
			{
				element.gameObject.transform.SetParent(disabledElementParent);
			}
			element.gameObject.SetActive(false);
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
				T val = activeElements[0];
				val.gameObject.transform.SetParent(disabledElementParent);
			}
			T val2 = activeElements[0];
			val2.gameObject.SetActive(false);
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
		activeElements.ForEach(delegate(T ae)
		{
			UnityEngine.Object.Destroy(ae.gameObject);
		});
		activeElements.Clear();
	}

	public void DestroyAllFree()
	{
		freeElements.ForEach(delegate(T ae)
		{
			UnityEngine.Object.Destroy(ae.gameObject);
		});
		freeElements.Clear();
	}

	public void ForEachActiveElement(Action<T> predicate)
	{
		activeElements.ForEach(predicate);
	}

	public void ForEachFreeElement(Action<T> predicate)
	{
		freeElements.ForEach(predicate);
	}
}
