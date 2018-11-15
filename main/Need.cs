using Klei.AI;

public abstract class Need : KMonoBehaviour
{
	protected class ModifierType
	{
		public AttributeModifier modifier;

		public StatusItem statusItem;

		public Thought thought;
	}

	protected AttributeInstance expectationAttribute;

	protected ModifierType stressBonus;

	protected ModifierType stressNeutral;

	protected ModifierType stressPenalty;

	protected ModifierType currentStressModifier;

	public string Name
	{
		get;
		protected set;
	}

	public string ExpectationTooltip
	{
		get;
		protected set;
	}

	public string Tooltip
	{
		get;
		protected set;
	}

	public Attribute GetExpectationAttribute()
	{
		return expectationAttribute.Attribute;
	}

	protected void SetModifier(ModifierType modifier)
	{
		if (currentStressModifier != modifier)
		{
			if (currentStressModifier != null)
			{
				UnapplyModifier(currentStressModifier);
			}
			if (modifier != null)
			{
				ApplyModifier(modifier);
			}
			currentStressModifier = modifier;
		}
	}

	private void ApplyModifier(ModifierType modifier)
	{
		if (modifier.modifier != null)
		{
			Attributes attributes = this.GetAttributes();
			attributes.Add(modifier.modifier);
		}
		if (modifier.statusItem != null)
		{
			KSelectable component = GetComponent<KSelectable>();
			component.AddStatusItem(modifier.statusItem, null);
		}
		if (modifier.thought != null)
		{
			ThoughtGraph.Instance sMI = this.GetSMI<ThoughtGraph.Instance>();
			sMI.AddThought(modifier.thought);
		}
	}

	private void UnapplyModifier(ModifierType modifier)
	{
		if (modifier.modifier != null)
		{
			Attributes attributes = this.GetAttributes();
			attributes.Remove(modifier.modifier);
		}
		if (modifier.statusItem != null)
		{
			KSelectable component = GetComponent<KSelectable>();
			component.RemoveStatusItem(modifier.statusItem, false);
		}
		if (modifier.thought != null)
		{
			ThoughtGraph.Instance sMI = this.GetSMI<ThoughtGraph.Instance>();
			sMI.RemoveThought(modifier.thought);
		}
	}
}
