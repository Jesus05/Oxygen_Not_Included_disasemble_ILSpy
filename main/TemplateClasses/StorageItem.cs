using Klei;
using System;

namespace TemplateClasses
{
	[Serializable]
	public class StorageItem : YamlIO<StorageItem>, ICloneable
	{
		public string id
		{
			get;
			set;
		}

		public SimHashes element
		{
			get;
			set;
		}

		public float units
		{
			get;
			set;
		}

		public bool isOre
		{
			get;
			set;
		}

		public float temperature
		{
			get;
			set;
		}

		public string diseaseName
		{
			get;
			set;
		}

		public int diseaseCount
		{
			get;
			set;
		}

		public Rottable rottable
		{
			get;
			set;
		}

		public StorageItem()
		{
			rottable = new Rottable();
		}

		public StorageItem(string _id, float _units, float _temp, SimHashes _element, string _disease, int _disease_count, bool _isOre)
		{
			rottable = new Rottable();
			id = _id;
			element = _element;
			units = _units;
			diseaseName = _disease;
			diseaseCount = _disease_count;
			isOre = _isOre;
			temperature = _temp;
		}

		public object Clone()
		{
			StorageItem storageItem = new StorageItem(id, units, temperature, element, diseaseName, diseaseCount, isOre);
			storageItem.rottable.rotAmount = rottable.rotAmount;
			return storageItem;
		}
	}
}
