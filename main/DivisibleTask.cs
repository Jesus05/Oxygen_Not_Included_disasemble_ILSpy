internal abstract class DivisibleTask<SharedData> : IWorkItem<SharedData>
{
	public string name;

	public int start;

	public int end;

	protected DivisibleTask(string name)
	{
		this.name = name;
	}

	public void Run(SharedData sharedData)
	{
		RunDivision(sharedData);
	}

	protected abstract void RunDivision(SharedData sharedData);
}
