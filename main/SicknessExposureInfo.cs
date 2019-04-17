using System;

[Serializable]
public struct SicknessExposureInfo
{
	public string sicknessID;

	public string sourceInfo;

	public SicknessExposureInfo(string id, string infection_source_info)
	{
		sicknessID = id;
		sourceInfo = infection_source_info;
	}
}
