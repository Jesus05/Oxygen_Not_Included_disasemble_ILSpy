using System.Collections.Generic;
using UnityEngine;

public class SubstanceTable : ScriptableObject, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<Substance> list;

	public List<Substance> GetList()
	{
		return list;
	}

	public Substance GetSubstance(SimHashes substance)
	{
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			if (list[i].elementID == substance)
			{
				return list[i];
			}
		}
		return null;
	}

	public void OnBeforeSerialize()
	{
		BindAnimList();
	}

	public void OnAfterDeserialize()
	{
		BindAnimList();
	}

	private void BindAnimList()
	{
		foreach (Substance item in list)
		{
			if ((Object)item.anim != (Object)null && (item.anims == null || item.anims.Length == 0))
			{
				item.anims = new KAnimFile[1];
				item.anims[0] = item.anim;
			}
		}
	}
}
