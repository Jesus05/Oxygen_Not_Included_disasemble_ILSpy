using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class ConduitElementSensor : ConduitSensor
{
	[MyCmpGet]
	private Filterable filterable;

	private SimHashes desiredElement = SimHashes.Void;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		filterable.onFilterChanged += OnFilterChanged;
		OnFilterChanged(filterable.SelectedTag);
	}

	private void OnFilterChanged(Tag tag)
	{
		desiredElement = SimHashes.Void;
		if (tag.IsValid)
		{
			Element element = ElementLoader.GetElement(tag);
			if (element != null)
			{
				desiredElement = element.id;
			}
		}
	}

	protected override void ConduitUpdate(float dt)
	{
		ConduitFlow flowManager = Conduit.GetFlowManager(conduitType);
		int cell = Grid.PosToCell(base.transform.GetPosition());
		ConduitFlow.ConduitContents contents = flowManager.GetContents(cell);
		if (base.IsSwitchedOn)
		{
			if (contents.element != desiredElement)
			{
				Toggle();
			}
		}
		else if (contents.element == desiredElement)
		{
			Toggle();
		}
	}
}
