using System.Collections.Generic;
using TUNING;

public static class ModUtil
{
	public static void AddBuildingToPlanScreen(PlanScreen.PlanCategory category, string building_id)
	{
		int num = BUILDINGS.PLANORDER.FindIndex((PlanScreen.PlanInfo x) => x.category == category);
		if (num > 0)
		{
			PlanScreen.PlanInfo planInfo = BUILDINGS.PLANORDER[num];
			IList<string> list = planInfo.data as IList<string>;
			list.Add(building_id);
		}
	}

	public static void AddBuildingToHotkeyBuildMenu(BuildMenu.Category category, string building_id, Action hotkey)
	{
		BuildMenu.DisplayInfo info = BuildMenu.OrderedBuildings.GetInfo(category);
		if (info.category == category)
		{
			IList<BuildMenu.BuildingInfo> list = info.data as IList<BuildMenu.BuildingInfo>;
			list.Add(new BuildMenu.BuildingInfo(building_id, hotkey));
		}
	}
}
