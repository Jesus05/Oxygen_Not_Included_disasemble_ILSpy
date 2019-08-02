using System;

namespace TemplateClasses
{
	[Serializable]
	public class Cell : ICloneable
	{
		public SimHashes element
		{
			get;
			set;
		}

		public float mass
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

		public int location_x
		{
			get;
			set;
		}

		public int location_y
		{
			get;
			set;
		}

		public bool preventFoWReveal
		{
			get;
			set;
		}

		public Cell()
		{
		}

		public Cell(int loc_x, int loc_y)
		{
			location_x = loc_x;
			location_y = loc_y;
			element = SimHashes.Oxygen;
			temperature = SaveGame.Instance.worldGen.Settings.GetFloatSetting("StartAreaTemperatureOffset");
			mass = SaveGame.Instance.worldGen.Settings.GetFloatSetting("StartAreaPressureMultiplier");
			diseaseName = null;
			diseaseCount = 0;
		}

		public Cell(int loc_x, int loc_y, SimHashes _element, float _temperature, float _mass, string _diseaseName, int _diseaseCount, bool _preventFoWReveal = false)
		{
			location_x = loc_x;
			location_y = loc_y;
			element = _element;
			temperature = _temperature;
			mass = _mass;
			diseaseName = _diseaseName;
			diseaseCount = _diseaseCount;
			preventFoWReveal = _preventFoWReveal;
		}

		public object Clone()
		{
			return new Cell(location_x, location_y, element, temperature, mass, diseaseName, diseaseCount, preventFoWReveal);
		}

		public object Clone(int offset_x, int offset_y)
		{
			Cell cell = (Cell)Clone();
			cell.location_x += offset_x;
			cell.location_y += offset_y;
			return cell;
		}
	}
}
