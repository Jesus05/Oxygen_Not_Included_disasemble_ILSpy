using System;

internal class EntityConfigOrder : Attribute
{
	public int sortOrder;

	public EntityConfigOrder(int sort_order)
	{
		sortOrder = sort_order;
	}
}
