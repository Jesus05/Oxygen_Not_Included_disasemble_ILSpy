using Klei.AI;

public class RoleAttributePerk : RolePerk
{
	private AttributeModifier modifier;

	public RoleAttributePerk(string id, string description, string attributeId, float modifierBonus, string modifierDesc)
		: base(id, string.Format(description, modifierBonus), null, null, delegate
		{
		}, false)
	{
		modifier = new AttributeModifier(attributeId, modifierBonus, modifierDesc, false, false, true);
		base.OnApply = delegate(MinionResume identity)
		{
			if (identity.GetAttributes().Get(modifier.AttributeId).Modifiers.FindIndex((AttributeModifier mod) => mod == modifier) == -1)
			{
				identity.GetAttributes().Add(modifier);
			}
		};
		base.OnRemove = delegate(MinionResume identity)
		{
			identity.GetAttributes().Remove(modifier);
		};
	}
}
