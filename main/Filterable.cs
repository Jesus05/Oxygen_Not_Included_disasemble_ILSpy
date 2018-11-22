using KSerialization;
using System;
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

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	[Serialize]
	public ElementState filterElementState = ElementState.None;

	[Serialize]
	private Tag selectedTag;

	private static readonly Operational.Flag filterSelected = new Operational.Flag("filterSelected", Operational.Flag.Type.Requirement);

	private static readonly EventSystem.IntraObjectHandler<Filterable> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<Filterable>(delegate(Filterable component, object data)
	{
		component.OnCopySettings(data);
	});

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
		foreach (Element element in ElementLoader.elements)
		{
			bool flag = true;
			if (filterElementState != 0)
			{
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
				Tag item = GameTagExtensions.Create(element.id);
				list.Add(item);
			}
		}
		return list;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		Filterable component = gameObject.GetComponent<Filterable>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			SelectedTag = component.SelectedTag;
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
