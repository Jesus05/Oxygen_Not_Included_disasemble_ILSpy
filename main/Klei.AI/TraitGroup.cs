namespace Klei.AI
{
	public class TraitGroup : ModifierGroup<Trait>
	{
		public bool IsSpawnTrait;

		public TraitGroup(string id, string name, bool is_spawn_trait)
			: base(id, name)
		{
			IsSpawnTrait = is_spawn_trait;
		}
	}
}
