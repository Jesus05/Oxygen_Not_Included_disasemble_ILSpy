using KSerialization;
using System.Collections.Generic;

[SerializationConfig(MemberSerialization.OptIn)]
public class MaterialSelectorSerializer : KMonoBehaviour
{
	[Serialize]
	private List<Dictionary<Tag, Tag>> previouslySelectedElements;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (previouslySelectedElements == null)
		{
			previouslySelectedElements = new List<Dictionary<Tag, Tag>>();
		}
	}

	public void SetSelectedElement(int selectorIndex, Tag recipe, Tag element)
	{
		while (previouslySelectedElements.Count <= selectorIndex)
		{
			previouslySelectedElements.Add(new Dictionary<Tag, Tag>());
		}
		previouslySelectedElements[selectorIndex][recipe] = element;
	}

	public Tag GetPreviousElement(int selectorIndex, Tag recipe)
	{
		Tag value = Tag.Invalid;
		if (previouslySelectedElements.Count <= selectorIndex)
		{
			return value;
		}
		previouslySelectedElements[selectorIndex].TryGetValue(recipe, out value);
		return value;
	}
}
