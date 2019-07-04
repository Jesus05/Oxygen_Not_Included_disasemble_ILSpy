using Klei.AI;
using STRINGS;

public class QualityOfLifeAttributeFormatter : StandardAttributeFormatter
{
	public QualityOfLifeAttributeFormatter()
		: base(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None)
	{
	}

	public override string GetFormattedAttribute(AttributeInstance instance)
	{
		AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(instance.gameObject);
		return string.Format(DUPLICANTS.ATTRIBUTES.QUALITYOFLIFE.DESC_FORMAT, GetFormattedValue(instance.GetTotalDisplayValue(), GameUtil.TimeSlice.None, instance.gameObject), GetFormattedValue(attributeInstance.GetTotalDisplayValue(), GameUtil.TimeSlice.None, instance.gameObject));
	}

	public override string GetTooltip(Attribute master, AttributeInstance instance)
	{
		string tooltip = base.GetTooltip(master, instance);
		AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(instance.gameObject);
		tooltip = tooltip + "\n\n" + string.Format(DUPLICANTS.ATTRIBUTES.QUALITYOFLIFE.TOOLTIP_EXPECTATION, GetFormattedValue(attributeInstance.GetTotalDisplayValue(), GameUtil.TimeSlice.None, instance.gameObject));
		float num = instance.GetTotalDisplayValue() - attributeInstance.GetTotalDisplayValue();
		if (!(num >= 0f))
		{
			return tooltip + "\n\n" + DUPLICANTS.ATTRIBUTES.QUALITYOFLIFE.TOOLTIP_EXPECTATION_UNDER;
		}
		return tooltip + "\n\n" + DUPLICANTS.ATTRIBUTES.QUALITYOFLIFE.TOOLTIP_EXPECTATION_OVER;
	}
}
