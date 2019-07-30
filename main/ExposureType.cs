using System.Collections.Generic;

public class ExposureType
{
	public string germ_id;

	public string sickness_id;

	public string infection_effect;

	public int exposure_threshold;

	public bool infect_immediately;

	public List<string> required_traits;

	public List<string> excluded_traits;

	public List<string> excluded_effects;

	public int base_resistance;
}
