using UnityEngine;

public abstract class AsyncCsvLoader<LoaderType, CsvEntryType> : GlobalAsyncLoader<LoaderType> where LoaderType : class where CsvEntryType : Resource, new()
{
	private string text;

	private string name;

	public CsvEntryType[] entries;

	public AsyncCsvLoader(TextAsset asset)
	{
		text = asset.text;
		name = asset.name;
	}

	public override void Run()
	{
		entries = new ResourceLoader<CsvEntryType>(text, name).resources.ToArray();
		text = null;
		name = null;
	}
}
