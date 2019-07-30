using Klei.AI;
using STRINGS;

namespace Database
{
	public class SkillAttributePerk : SkillPerk
	{
		public AttributeModifier modifier;

		public SkillAttributePerk(string id, string attributeId, float modifierBonus, string modifierDesc)
			: base(id, string.Empty, null, null, delegate
			{
			}, false)
		{
			Attribute attribute = Db.Get().Attributes.Get(attributeId);
			modifier = new AttributeModifier(attributeId, modifierBonus, modifierDesc, false, false, true);
			Name = string.Format(UI.ROLES_SCREEN.PERKS.ATTRIBUTE_EFFECT_FMT, modifier.GetFormattedString(null), attribute.Name);
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
}
