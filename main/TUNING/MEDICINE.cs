namespace TUNING
{
	public class MEDICINE
	{
		public const float DEFAULT_MASS = 1f;

		public const float RECUPERATION_DISEASE_MULTIPLIER = 1.1f;

		public const float RECUPERATION_DOCTORED_DISEASE_MULTIPLIER = 1.2f;

		public const float WORK_TIME = 10f;

		public static readonly MedicineInfo GENERICPILL = new MedicineInfo("genericpill", "Medicine_GenericPill", MedicineInfo.MedicineType.CureAny, null);

		public static readonly MedicineInfo VITAMINSUPPLEMENT = new MedicineInfo("vitaminsupplement", "Medicine_VitaminSupplement", MedicineInfo.MedicineType.Booster, null);
	}
}
