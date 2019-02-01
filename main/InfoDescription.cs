[SkipSaveFileSerialization]
public class InfoDescription : KMonoBehaviour
{
	public string nameLocString = "";

	public string descriptionLocString = "";

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
