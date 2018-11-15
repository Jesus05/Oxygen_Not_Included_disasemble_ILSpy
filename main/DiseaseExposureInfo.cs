using System;

[Serializable]
public struct DiseaseExposureInfo
{
	public string diseaseID;

	public string infectionSourceInfo;

	public DiseaseExposureInfo(string id, string infection_source_info)
	{
		diseaseID = id;
		infectionSourceInfo = infection_source_info;
	}
}
