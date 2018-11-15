using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
	public class Diseases : Modifications<Disease, DiseaseInstance>
	{
		public Diseases(GameObject go)
			: base(go, (ResourceSet<Disease>)Db.Get().Diseases)
		{
		}

		public void Infect(DiseaseExposureInfo exposure_info)
		{
			Disease modifier = Db.Get().Diseases.Get(exposure_info.diseaseID);
			if (!Has(modifier))
			{
				DiseaseInstance diseaseInstance = CreateInstance(modifier);
				diseaseInstance.ExposureInfo = exposure_info;
			}
		}

		public override DiseaseInstance CreateInstance(Disease disease)
		{
			DiseaseInstance diseaseInstance = new DiseaseInstance(base.gameObject, disease);
			Add(diseaseInstance);
			Trigger(GameHashes.DiseaseAdded, diseaseInstance);
			ReportManager.Instance.ReportValue(ReportManager.ReportType.DiseaseAdded, 1f, base.gameObject.GetProperName(), null);
			return diseaseInstance;
		}

		public bool IsInfected()
		{
			return base.Count > 0;
		}

		public bool Cure(Disease disease)
		{
			DiseaseInstance diseaseInstance = null;
			using (IEnumerator<DiseaseInstance> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DiseaseInstance current = enumerator.Current;
					if (current.modifier.Id == disease.Id)
					{
						diseaseInstance = current;
						break;
					}
				}
			}
			bool result = false;
			if (diseaseInstance != null)
			{
				Remove(diseaseInstance);
				result = true;
				Trigger(GameHashes.DiseaseCured, diseaseInstance);
				ReportManager.Instance.ReportValue(ReportManager.ReportType.DiseaseAdded, -1f, base.gameObject.GetProperName(), null);
			}
			return result;
		}
	}
}
