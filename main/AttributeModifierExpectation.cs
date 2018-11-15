using Klei.AI;
using UnityEngine;

public class AttributeModifierExpectation : Expectation
{
	public AttributeModifier modifier;

	public Sprite icon;

	public AttributeModifierExpectation(string id, string name, string description, AttributeModifier modifier, Sprite icon)
		: base(id, name, description, delegate(MinionResume resume)
		{
			resume.GetAttributes().Get(modifier.AttributeId).Add(modifier);
		}, delegate(MinionResume resume)
		{
			resume.GetAttributes().Get(modifier.AttributeId).Remove(modifier);
		})
	{
		this.modifier = modifier;
		this.icon = icon;
	}
}
