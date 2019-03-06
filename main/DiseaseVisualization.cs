using System;
using System.Collections.Generic;
using UnityEngine;

public class DiseaseVisualization : ScriptableObject
{
	[Serializable]
	public struct Info
	{
		public string name;

		public Color32 overlayColour;

		public Info(string name)
		{
			this.name = name;
			overlayColour = Color.red;
		}
	}

	public Sprite overlaySprite;

	public List<Info> info = new List<Info>();

	public Info GetInfo(HashedString id)
	{
		foreach (Info item in info)
		{
			Info current = item;
			if (id == (HashedString)current.name)
			{
				return current;
			}
		}
		return default(Info);
	}
}
