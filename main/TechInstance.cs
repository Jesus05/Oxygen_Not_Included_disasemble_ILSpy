public class TechInstance
{
	public struct SaveData
	{
		public string techId;

		public bool complete;

		public string[] inventoryIDs;

		public float[] inventoryValues;
	}

	public Tech tech;

	private bool complete;

	public ResearchPointInventory progressInventory = new ResearchPointInventory();

	public TechInstance(Tech tech)
	{
		this.tech = tech;
	}

	public bool IsComplete()
	{
		return complete;
	}

	public void Purchased()
	{
		if (!complete)
		{
			complete = true;
		}
	}

	public SaveData Save()
	{
		string[] array = new string[progressInventory.PointsByTypeID.Count];
		progressInventory.PointsByTypeID.Keys.CopyTo(array, 0);
		float[] array2 = new float[progressInventory.PointsByTypeID.Count];
		progressInventory.PointsByTypeID.Values.CopyTo(array2, 0);
		SaveData result = default(SaveData);
		result.techId = tech.Id;
		result.complete = complete;
		result.inventoryIDs = array;
		result.inventoryValues = array2;
		return result;
	}

	public void Load(SaveData save_data)
	{
		complete = save_data.complete;
		for (int i = 0; i < save_data.inventoryIDs.Length; i++)
		{
			progressInventory.AddResearchPoints(save_data.inventoryIDs[i], save_data.inventoryValues[i]);
		}
	}
}
