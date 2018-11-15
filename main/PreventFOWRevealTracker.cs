using KSerialization;
using System.Collections.Generic;
using System.Runtime.Serialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class PreventFOWRevealTracker : KMonoBehaviour
{
	[Serialize]
	public List<int> preventFOWRevealCells;

	[OnSerializing]
	private void OnSerialize()
	{
		preventFOWRevealCells.Clear();
		for (int i = 0; i < Grid.PreventFogOfWarReveal.Length; i++)
		{
			if (Grid.PreventFogOfWarReveal[i])
			{
				preventFOWRevealCells.Add(i);
			}
		}
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		foreach (int preventFOWRevealCell in preventFOWRevealCells)
		{
			Grid.PreventFogOfWarReveal[preventFOWRevealCell] = true;
		}
	}
}
