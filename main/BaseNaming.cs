using STRINGS;
using TMPro;
using UnityEngine;

public class BaseNaming : KMonoBehaviour
{
	[SerializeField]
	private TMP_InputField inputField;

	[SerializeField]
	private KButton shuffleBaseNameButton;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GenerateBaseName();
		inputField.onValueChanged.AddListener(delegate
		{
			Util.ScrubInputField(inputField, false);
		});
		shuffleBaseNameButton.onClick += GenerateBaseName;
		inputField.onEndEdit.AddListener(OnEndEdit);
	}

	private void OnEndEdit(string newName)
	{
		if (Localization.HasDirtyWords(newName))
		{
			inputField.text = GenerateBaseNameString();
			newName = inputField.text;
		}
		if (!string.IsNullOrEmpty(newName))
		{
			inputField.text = newName;
			SaveGame.Instance.SetBaseName(newName);
			string text = newName;
			if (!text.Contains(".sav"))
			{
				text += ".sav";
			}
			string savePrefixAndCreateFolder = SaveLoader.GetSavePrefixAndCreateFolder();
			if (!text.Contains(savePrefixAndCreateFolder))
			{
				text = savePrefixAndCreateFolder + text;
			}
			SaveLoader.SetActiveSaveFilePath(text);
		}
	}

	private void GenerateBaseName()
	{
		string text = GenerateBaseNameString();
		((LocText)inputField.placeholder).text = text;
		inputField.text = text;
		OnEndEdit(text);
	}

	private string GenerateBaseNameString()
	{
		string random = LocString.GetStrings(typeof(NAMEGEN.COLONY.FORMATS)).GetRandom();
		random = ReplaceStringWithRandom(random, "{noun}", LocString.GetStrings(typeof(NAMEGEN.COLONY.NOUN)));
		string[] strings = LocString.GetStrings(typeof(NAMEGEN.COLONY.ADJECTIVE));
		random = ReplaceStringWithRandom(random, "{adjective}", strings);
		random = ReplaceStringWithRandom(random, "{adjective2}", strings);
		random = ReplaceStringWithRandom(random, "{adjective3}", strings);
		return ReplaceStringWithRandom(random, "{adjective4}", strings);
	}

	private string ReplaceStringWithRandom(string fullString, string replacementKey, string[] replacementValues)
	{
		if (fullString.Contains(replacementKey))
		{
			return fullString.Replace(replacementKey, replacementValues.GetRandom());
		}
		return fullString;
	}
}
