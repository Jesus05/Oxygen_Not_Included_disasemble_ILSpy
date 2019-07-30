[SkipSaveFileSerialization]
public class InfoDescription : KMonoBehaviour
{
	public string nameLocString = string.Empty;

	public string descriptionLocString = string.Empty;

	public string description;

	public string displayName;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (!string.IsNullOrEmpty(nameLocString))
		{
			displayName = Strings.Get(nameLocString);
		}
		if (!string.IsNullOrEmpty(descriptionLocString))
		{
			description = Strings.Get(descriptionLocString);
		}
	}
}
