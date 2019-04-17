using UnityEngine;

public class HierarchyReferences : KMonoBehaviour
{
	public ElementReference[] references;

	public bool HasReference(string name)
	{
		ElementReference[] array = references;
		for (int i = 0; i < array.Length; i++)
		{
			ElementReference elementReference = array[i];
			if (elementReference.Name == name)
			{
				return true;
			}
		}
		return false;
	}

	public SpecifiedType GetReference<SpecifiedType>(string name) where SpecifiedType : Component
	{
		ElementReference[] array = references;
		for (int i = 0; i < array.Length; i++)
		{
			ElementReference elementReference = array[i];
			if (elementReference.Name == name)
			{
				if (elementReference.behaviour is SpecifiedType)
				{
					return (SpecifiedType)elementReference.behaviour;
				}
				Debug.LogError(string.Format("Behavior is not specified type"));
			}
		}
		Debug.LogError($"Could not find UI reference '{name}' or convert to specified type)");
		return (SpecifiedType)null;
	}

	public Component GetReference(string name)
	{
		ElementReference[] array = references;
		for (int i = 0; i < array.Length; i++)
		{
			ElementReference elementReference = array[i];
			if (elementReference.Name == name)
			{
				return elementReference.behaviour;
			}
		}
		Debug.LogWarning("Couldn't find reference to object named {0} Make sure the name matches the field in the inspector.");
		return null;
	}
}
