using KSerialization;
using System.Collections.Generic;
using TUNING;
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

	[Serialize]
	private string chosenState;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.MonumentParts.Add(this);
		if (!string.IsNullOrEmpty(chosenState))
		{
			SetState(chosenState);
		}
		UpdateMonumentDecor();
	}

	protected override void OnCleanUp()
	{
		Components.MonumentParts.Remove(this);
		RemoveMonumentPiece();
		base.OnCleanUp();
	}

	public void SetState(string state)
	{
		GetComponent<KBatchedAnimController>().Play(state, KAnim.PlayMode.Once, 1f, 0f);
		chosenState = state;
	}

	public bool IsMonumentCompleted()
	{
		bool flag = (Object)GetMonumentPart(Part.Top) != (Object)null;
		bool flag2 = (Object)GetMonumentPart(Part.Middle) != (Object)null;
		bool flag3 = (Object)GetMonumentPart(Part.Bottom) != (Object)null;
		return flag && flag3 && flag2;
	}

	public void UpdateMonumentDecor()
	{
		GameObject monumentPart = GetMonumentPart(Part.Middle);
		if (IsMonumentCompleted())
		{
			DecorProvider component = monumentPart.GetComponent<DecorProvider>();
			component.SetValues(BUILDINGS.DECOR.BONUS.MONUMENT.COMPLETE);
			List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(GetComponent<AttachableBuilding>());
			foreach (GameObject item in attachedNetwork)
			{
				if ((Object)item != (Object)monumentPart)
				{
					DecorProvider component2 = item.GetComponent<DecorProvider>();
					component2.SetValues(BUILDINGS.DECOR.NONE);
				}
			}
		}
	}

	public void RemoveMonumentPiece()
	{
		if (IsMonumentCompleted())
		{
			List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(GetComponent<AttachableBuilding>());
			foreach (GameObject item in attachedNetwork)
			{
				if ((Object)item.GetComponent<MonumentPart>() != (Object)this)
				{
					DecorProvider component = item.GetComponent<DecorProvider>();
					component.SetValues(BUILDINGS.DECOR.BONUS.MONUMENT.INCOMPLETE);
				}
			}
		}
	}

	private GameObject GetMonumentPart(Part requestPart)
	{
		List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(GetComponent<AttachableBuilding>());
		foreach (GameObject item in attachedNetwork)
		{
			MonumentPart component = item.GetComponent<MonumentPart>();
			if (!((Object)component == (Object)null) && component.part == requestPart)
			{
				return item;
			}
		}
		return null;
	}
}
