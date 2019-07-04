using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class LocText : TextMeshProUGUI
{
	public string key;

	public TextStyleSetting textStyleSetting;

	public bool allowOverride = false;

	public bool staticLayout = false;

	private TextLinkHandler textLinkHandler;

	[SerializeField]
	private bool allowLinksInternal;

	private static string[] splits;

	public bool AllowLinks
	{
		get
		{
			return allowLinksInternal;
		}
		set
		{
			allowLinksInternal = value;
			RefreshLinkHandler();
			raycastTarget = (raycastTarget || allowLinksInternal);
		}
	}

	public override string text
	{
		get
		{
			return base.text;
		}
		set
		{
			base.text = FilterInput(value);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
	}

	[ContextMenu("Apply Settings")]
	public void ApplySettings()
	{
		if (this.key != "" && Application.isPlaying)
		{
			StringKey key = new StringKey(this.key);
			text = Strings.Get(key);
		}
		if ((Object)textStyleSetting != (Object)null)
		{
			SetTextStyleSetting.ApplyStyle(this, textStyleSetting);
		}
	}

	private new void Awake()
	{
		base.Awake();
		if (Application.isPlaying)
		{
			if (this.key != "")
			{
				StringKey key = new StringKey(this.key);
				StringEntry stringEntry = Strings.Get(key);
				text = stringEntry.String;
			}
			text = Localization.Fixup(text);
			base.isRightToLeftText = Localization.IsRightToLeft;
			SetTextStyleSetting setTextStyleSetting = base.gameObject.GetComponent<SetTextStyleSetting>();
			if ((Object)setTextStyleSetting == (Object)null)
			{
				setTextStyleSetting = base.gameObject.AddComponent<SetTextStyleSetting>();
			}
			if (!allowOverride)
			{
				setTextStyleSetting.SetStyle(textStyleSetting);
			}
			textLinkHandler = GetComponent<TextLinkHandler>();
		}
	}

	private new void Start()
	{
		base.Start();
		RefreshLinkHandler();
	}

	public override void SetLayoutDirty()
	{
		if (!staticLayout)
		{
			base.SetLayoutDirty();
		}
	}

	public override void SetText(string text)
	{
		text = FilterInput(text);
		base.SetText(text);
	}

	private string FilterInput(string input)
	{
		if (!AllowLinks)
		{
			return input;
		}
		return ModifyLinkStrings(input);
	}

	protected override void GenerateTextMesh()
	{
		base.GenerateTextMesh();
	}

	internal void SwapFont(TMP_FontAsset font, bool isRightToLeft)
	{
		base.font = font;
		if (this.key != "")
		{
			StringKey key = new StringKey(this.key);
			StringEntry stringEntry = Strings.Get(key);
			text = stringEntry.String;
		}
		text = Localization.Fixup(text);
		base.isRightToLeftText = isRightToLeft;
	}

	private static string ModifyLinkStrings(string input)
	{
		string text = "<link=\"";
		string pattern = "</link>";
		string text2 = "<b><style=\"KLink\">";
		string value = "</style></b>";
		string pattern2 = text2 + text;
		if (input != null && Regex.Split(input, pattern2).Length <= 1)
		{
			splits = Regex.Split(input, text);
			if (splits.Length > 1)
			{
				for (int i = 1; i < splits.Length; i++)
				{
					if (!(splits[i] == ""))
					{
						int num = input.IndexOf(splits[i]);
						input = input.Insert(num - text.Length, text2);
					}
				}
			}
			splits = Regex.Split(input, pattern);
			if (splits.Length > 1)
			{
				for (int j = 0; j < splits.Length; j++)
				{
					if (!(splits[j] == ""))
					{
						int num2 = input.IndexOf(splits[j]);
						if (num2 != 0)
						{
							input = input.Insert(num2, value);
						}
					}
				}
			}
			return input;
		}
		return input;
	}

	private void RefreshLinkHandler()
	{
		if ((Object)textLinkHandler == (Object)null && allowLinksInternal)
		{
			textLinkHandler = GetComponent<TextLinkHandler>();
			if ((Object)textLinkHandler == (Object)null)
			{
				textLinkHandler = base.gameObject.AddComponent<TextLinkHandler>();
			}
		}
		else if (!allowLinksInternal && (Object)textLinkHandler != (Object)null)
		{
			Object.Destroy(textLinkHandler);
			textLinkHandler = null;
		}
		if ((Object)textLinkHandler != (Object)null)
		{
			textLinkHandler.CheckMouseOver();
		}
	}
}
