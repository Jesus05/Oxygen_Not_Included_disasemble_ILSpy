public class CreatureBrain : Brain
{
	public string symbolPrefix;

	public Tag species;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		GetComponent<Navigator>().SetAbilities(new CreaturePathFinderAbilities(GetComponent<Navigator>()));
	}
}
