using System;
using System.Collections.Generic;

public class MaterialNeeds : KMonoBehaviour
{
	private Dictionary<Tag, float> Needs = new Dictionary<Tag, float>();

	public System.Action OnDirty;

	public static MaterialNeeds Instance
	{
		get;
		private set;
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
	}

	public void UpdateNeed(Tag tag, float amount)
	{
		float value = 0f;
		if (!Needs.TryGetValue(tag, out value))
		{
			Needs[tag] = 0f;
		}
		Needs[tag] = value + amount;
	}

	public float GetAmount(Tag tag)
	{
		float value = 0f;
		Needs.TryGetValue(tag, out value);
		return value;
	}

	public Dictionary<Tag, float> GetNeeds()
	{
		return Needs;
	}
}
