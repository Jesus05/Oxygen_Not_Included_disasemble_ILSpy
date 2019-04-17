using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
	public class Sicknesses : Modifications<Sickness, SicknessInstance>
	{
		public Sicknesses(GameObject go)
			: base(go, (ResourceSet<Sickness>)Db.Get().Sicknesses)
		{
		}

		public void Infect(SicknessExposureInfo exposure_info)
		{
			Sickness modifier = Db.Get().Sicknesses.Get(exposure_info.sicknessID);
			if (!Has(modifier))
			{
				SicknessInstance sicknessInstance = CreateInstance(modifier);
				sicknessInstance.ExposureInfo = exposure_info;
			}
		}

		public override SicknessInstance CreateInstance(Sickness sickness)
		{
			SicknessInstance sicknessInstance = new SicknessInstance(base.gameObject, sickness);
			Add(sicknessInstance);
			Trigger(GameHashes.SicknessAdded, sicknessInstance);
			ReportManager.Instance.ReportValue(ReportManager.ReportType.DiseaseAdded, 1f, base.gameObject.GetProperName(), null);
			return sicknessInstance;
		}

		public bool IsInfected()
		{
			return base.Count > 0;
		}

		public bool Cure(Sickness sickness)
		{
			return Cure(sickness.Id);
		}

		public bool Cure(string sickness_id)
		{
			SicknessInstance sicknessInstance = null;
			using (IEnumerator<SicknessInstance> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SicknessInstance current = enumerator.Current;
					if (current.modifier.Id == sickness_id)
					{
						sicknessInstance = current;
						break;
					}
				}
			}
			if (sicknessInstance != null)
			{
				Remove(sicknessInstance);
				Trigger(GameHashes.SicknessCured, sicknessInstance);
				ReportManager.Instance.ReportValue(ReportManager.ReportType.DiseaseAdded, -1f, base.gameObject.GetProperName(), null);
				return true;
			}
			return false;
		}
	}
}
