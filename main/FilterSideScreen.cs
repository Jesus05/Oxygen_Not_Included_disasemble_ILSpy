using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterSideScreen : SideScreenContent
{
	public GameObject elementEntryPrefab;

	public GameObject elementEntryContainer;

	public Image outputIcon;

	public Image everythingElseIcon;

	public LocText outputElementHeaderLabel;

	public LocText everythingElseHeaderLabel;

	public LocText selectElementHeaderLabel;

	public LocText currentSelectionLabel;

	public Dictionary<Element, FilterSideScreenRow> filterRowMap = new Dictionary<Element, FilterSideScreenRow>();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		filterRowMap.Clear();
		PopulateElements();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return (Object)target.GetComponent<Filterable>() != (Object)null;
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show && !((Object)DetailsScreen.Instance.target == (Object)null))
		{
			Filterable component = DetailsScreen.Instance.target.GetComponent<Filterable>();
			if (!((Object)component == (Object)null))
			{
				outputElementHeaderLabel.text = UI.UISIDESCREENS.FILTERSIDESCREEN.OUTPUTELEMENTHEADER;
				selectElementHeaderLabel.text = UI.UISIDESCREENS.FILTERSIDESCREEN.SELECTELEMENTHEADER;
				everythingElseHeaderLabel.text = ((component.filterElementState != Filterable.ElementState.Gas) ? UI.UISIDESCREENS.FILTERSIDESCREEN.UNFILTEREDELEMENTS.LIQUID : UI.UISIDESCREENS.FILTERSIDESCREEN.UNFILTEREDELEMENTS.GAS);
				Element filterElement = (!component.SelectedTag.IsValid) ? null : ElementLoader.GetElement(component.SelectedTag);
				SetFilterElement(filterElement);
				Configure(component);
			}
		}
	}

	private void PopulateElements()
	{
		List<Element> list = new List<Element>(ElementLoader.elements);
		list.Sort((Element a, Element b) => a.name.CompareTo(b.name));
		foreach (Element item in list)
		{
			FilterSideScreenRow row = Util.KInstantiateUI(elementEntryPrefab, elementEntryContainer, false).GetComponent<FilterSideScreenRow>();
			row.SetElement(item);
			row.button.onClick += delegate
			{
				SetFilterElement(row.element);
			};
			filterRowMap.Add(row.element, row);
		}
	}

	private void Configure(Filterable filterable)
	{
		IList<Tag> tagOptions = filterable.GetTagOptions();
		foreach (KeyValuePair<Element, FilterSideScreenRow> item in filterRowMap)
		{
			Element key = item.Key;
			bool active = tagOptions.Contains(key.tag);
			item.Value.gameObject.SetActive(active);
		}
	}

	private void SetFilterElement(Element element)
	{
		Filterable component = DetailsScreen.Instance.target.GetComponent<Filterable>();
		if (!((Object)component == (Object)null))
		{
			LocString loc_string = (component.filterElementState != Filterable.ElementState.Gas) ? UI.UISIDESCREENS.FILTERSIDESCREEN.FILTEREDELEMENT.LIQUID : UI.UISIDESCREENS.FILTERSIDESCREEN.FILTEREDELEMENT.GAS;
			currentSelectionLabel.text = string.Format(loc_string, UI.UISIDESCREENS.FILTERSIDESCREEN.NOELEMENTSELECTED);
			if (element != null)
			{
				component.SelectedTag = element.tag;
				foreach (KeyValuePair<Element, FilterSideScreenRow> item in filterRowMap)
				{
					bool flag = item.Key == element;
					item.Value.SetSelected(flag);
					if (flag && element.id != SimHashes.Void && element.id != SimHashes.Vacuum)
					{
						currentSelectionLabel.text = string.Format(loc_string, element.name);
					}
				}
			}
		}
	}
}
