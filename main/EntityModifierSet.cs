using Database;

public class EntityModifierSet : ModifierSet
{
	public DuplicantStatusItems DuplicantStatusItems;

	public ChoreGroups ChoreGroups;

	public override void Initialize()
	{
		base.Initialize();
		DuplicantStatusItems = new DuplicantStatusItems(Root);
		ChoreGroups = new ChoreGroups(Root);
		LoadTraits();
	}
}
