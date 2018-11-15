using KSerialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Filterable : KMonoBehaviour
{
	public enum ElementState
	{
		None,
		Solid,
		Liquid,
		Gas
	}

	[Serialize]
	public ElementState filterElementState;

	[Serialize]
	private Tag selectedTag;

	private static readonly Operational.Flag filterSelected = new Operational.Flag("filterSelected", Operational.Flag.Type.Requirement);

	public Tag SelectedTag
	{
		get
		{
			return selectedTag;
		}
		set
		{
			selectedTag = value;
			OnFilterChanged();
		}
	}

	public event Action<Tag> onFilterChanged;

	public virtual IList<Tag> GetTagOptions()
	{
		List<Tag> list = new List<Tag>();
		IEnumerator enumerator = Enum.GetValues(typeof(SimHashes)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				SimHashes simHashes = (SimHashes)enumerator.Current;
				bool flag = true;
				if (filterElementState != 0)
				{
					Element element = ElementLoader.FindElementByHash(simHashes);
					switch (filterElementState)
					{
					case ElementState.Gas:
						flag = element.IsGas;
						break;
					case ElementState.Liquid:
						flag = element.IsLiquid;
						break;
					case ElementState.Solid:
						flag = element.IsSolid;
						break;
					}
				}
				if (flag)
				{
					Tag item = GameTagExtensions.Create(simHashes);
					list.Add(item);
				}
			}
			return list;
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	protected override void OnSpawn()
	{
		OnFilterChanged();
	}

	private void OnFilterChanged()
	{
		if (this.onFilterChanged != null)
		{
			this.onFilterChanged(selectedTag);
		}
		Operational component = GetComponent<Operational>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.SetFlag(filterSelected, selectedTag.IsValid);
		}
	}
}
