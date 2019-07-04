using System.Collections.Generic;
using UnityEngine;

public class MonumentPart : KMonoBehaviour
{
	public enum Part
	{
		Bottom,
		Middle,
		Top
	}

	public Part part;

	public List<Tuple<string, string>> selectableStatesAndSymbols = new List<Tuple<string, string>>();

	public string stateUISymbol;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.MonumentParts.Add(this);
	}

	protected override void OnCleanUp()
	{
		Components.MonumentParts.Remove(this);
		base.OnCleanUp();
	}

	public void SetState(HashedString state)
	{
		GetComponent<KBatchedAnimController>().Play(state, KAnim.PlayMode.Once, 1f, 0f);
	}

	public bool IsMonumentCompleted()
	{
		if (part == Part.Top)
		{
			bool flag = true;
			bool flag2 = false;
			bool flag3 = false;
			List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(GetComponent<AttachableBuilding>());
			foreach (GameObject item in attachedNetwork)
			{
				MonumentPart component = item.GetComponent<MonumentPart>();
				if (!((Object)component == (Object)null))
				{
					if (component.part == Part.Middle)
					{
						flag2 = true;
					}
					if (component.part == Part.Bottom)
					{
						flag3 = true;
					}
				}
			}
			return flag3 && flag2 && flag;
		}
		return false;
	}
}
