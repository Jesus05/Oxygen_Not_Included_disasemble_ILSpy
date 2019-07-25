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

	public bool isLogicFilter = false;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		filterRowMap.Clear();
		PopulateElements();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		bool flag = false;
		flag = ((!isLogicFilter) ? ((Object)target.GetComponent<ElementFilter>() != (Object)null) : ((Object)target.GetComponent<ConduitElementSensor>() != (Object)null || (Object)target.GetComponent<LogicElementSensor>() != (Object)null));
		return flag && (Object)target.GetComponent<Filterable>() != (Object)null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		Filterable component = target.GetComponent<Filterable>();
		if (!((Object)component == (Object)null))
		{
			everythingElseHeaderLabel.text = ((component.filterElementState != Filterable.ElementState.Gas) ? UI.UISIDESCREENS.FILTERSIDESCREEN.UNFILTEREDELEMENTS.LIQUID : UI.UISIDESCREENS.FILTERSIDESCREEN.UNFILTEREDELEMENTS.GAS);
			Element filterElement = (!component.SelectedTag.IsValid) ? ElementLoader.FindElementByHash(SimHashes.Void) : ElementLoader.GetElement(component.SelectedTag);
			SetFilterElement(filterElement);
			Configure(component);
		}
	}

	private void PopulateElements()
	{
		List<Element> list = new List<Element>(ElementLoader.elements);
		list.Sort(delegate(Element a, Element b)
		{
			if (a.id != SimHashes.Void)
			{
				if (b.id != SimHashes.Void)
				{
					return a.name.CompareTo(b.name);
				}
				return 1;
			}
			return -1;
		});
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
					if (flag)
					{
						if (element.id != SimHashes.Void && element.id != SimHashes.Vacuum)
						{
							currentSelectionLabel.text = string.Format(loc_string, element.name);
						}
						else
						{
							currentSelectionLabel.text = UI.UISIDESCREENS.FILTERSIDESCREEN.NO_SELECTION;
						}
					}
				}
			}
		}
	}
}
