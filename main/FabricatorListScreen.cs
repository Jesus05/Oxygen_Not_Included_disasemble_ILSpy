using System.Collections.Generic;

public class FabricatorListScreen : KToggleMenu
{
	private void Refresh()
	{
		List<ToggleInfo> list = new List<ToggleInfo>();
		foreach (Fabricator item in Components.Fabricators.Items)
		{
			Fabricator fabricator = item;
			KSelectable component = fabricator.GetComponent<KSelectable>();
			list.Add(new ToggleInfo(component.GetName(), fabricator, Action.NumActions));
		}
		Setup(list);
	}

	protected override void OnSpawn()
	{
		base.onSelect += OnClickFabricator;
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		Refresh();
	}

	private void OnClickFabricator(ToggleInfo toggle_info)
	{
		Fabricator fabricator = (Fabricator)toggle_info.userData;
		SelectTool.Instance.Select(fabricator.GetComponent<KSelectable>(), false);
	}
}
