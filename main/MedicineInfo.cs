using System;
using System.Collections.Generic;

[Serializable]
public class MedicineInfo
{
	public enum MedicineType
	{
		Booster,
		CureAny,
		CureSpecific
	}

	public string id;

	public string effect;

	public MedicineType medicineType;

	public List<string> curedSicknesses;

	public MedicineInfo(string id, string effect, MedicineType medicineType, string[] curedDiseases = null)
	{
		Debug.Assert(!string.IsNullOrEmpty(effect) || (curedDiseases != null && curedDiseases.Length > 0), "Medicine should have an effect or cure diseases");
		this.id = id;
		this.effect = effect;
		this.medicineType = medicineType;
		if (curedDiseases != null)
		{
			curedSicknesses = new List<string>(curedDiseases);
		}
		else
		{
			curedSicknesses = new List<string>();
		}
	}
}
