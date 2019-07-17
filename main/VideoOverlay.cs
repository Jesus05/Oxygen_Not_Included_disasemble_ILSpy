using System.Collections.Generic;

public class VideoOverlay : KMonoBehaviour
{
	public List<LocText> textFields;

	public void SetText(List<string> strings)
	{
		if (strings.Count != textFields.Count)
		{
			DebugUtil.LogErrorArgs(base.name, "expects", textFields.Count, "strings passed to it, got", strings.Count);
		}
		for (int i = 0; i < textFields.Count; i++)
		{
			textFields[i].text = strings[i];
		}
	}
}
