using UnityEngine;

public class ObjectLayerListItem
{
	private int cell = Grid.InvalidCell;

	private ObjectLayer layer;

	public ObjectLayerListItem previousItem
	{
		get;
		private set;
	}

	public ObjectLayerListItem nextItem
	{
		get;
		private set;
	}

	public GameObject gameObject
	{
		get;
		private set;
	}

	public ObjectLayerListItem(GameObject gameObject, ObjectLayer layer, int new_cell)
	{
		this.gameObject = gameObject;
		this.layer = layer;
		Refresh(new_cell);
	}

	public void Clear()
	{
		Refresh(Grid.InvalidCell);
	}

	public bool Refresh(int new_cell)
	{
		if (cell != new_cell)
		{
			if (cell != Grid.InvalidCell && (Object)Grid.Objects[cell, (int)layer] == (Object)this.gameObject)
			{
				GameObject value = null;
				if (nextItem != null && (Object)nextItem.gameObject != (Object)null)
				{
					value = nextItem.gameObject;
				}
				Grid.Objects[cell, (int)layer] = value;
			}
			if (previousItem != null)
			{
				previousItem.nextItem = nextItem;
			}
			if (nextItem != null)
			{
				nextItem.previousItem = previousItem;
			}
			previousItem = null;
			nextItem = null;
			cell = new_cell;
			if (cell != Grid.InvalidCell)
			{
				GameObject gameObject = Grid.Objects[cell, (int)layer];
				if ((Object)gameObject != (Object)null && (Object)gameObject != (Object)this.gameObject)
				{
					ObjectLayerListItem objectLayerListItem2 = nextItem = gameObject.GetComponent<Pickupable>().objectLayerListItem;
					objectLayerListItem2.previousItem = this;
				}
				Grid.Objects[cell, (int)layer] = this.gameObject;
			}
			return true;
		}
		return false;
	}

	public bool Update(int cell)
	{
		return Refresh(cell);
	}
}
