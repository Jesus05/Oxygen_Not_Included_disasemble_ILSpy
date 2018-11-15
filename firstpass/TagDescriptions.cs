using System.Text;

public class TagDescriptions
{
	public TagDescriptions(string csv_data)
	{
	}

	public static string GetDescription(string tag)
	{
		return Strings.Get("STRINGS.MISC.TAGS." + tag.ToUpper());
	}

	public static string GetDescription(Tag tag)
	{
		return Strings.Get("STRINGS.MISC.TAGS." + tag.Name.ToUpper());
	}

	public static string ReplaceTags(string text)
	{
		int num = text.IndexOf('{');
		int num2 = text.IndexOf('}');
		if (0 <= num && num < num2)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num3 = 0;
			while (0 <= num)
			{
				string value = text.Substring(num3, num - num3);
				stringBuilder.Append(value);
				num2 = text.IndexOf('}', num);
				if (num >= num2)
				{
					break;
				}
				string tag = text.Substring(num + 1, num2 - num - 1);
				string description = GetDescription(tag);
				stringBuilder.Append(description);
				num3 = num2 + 1;
				num = text.IndexOf('{', num2);
			}
			stringBuilder.Append(text.Substring(num3, text.Length - num3));
			return stringBuilder.ToString();
		}
		return text;
	}
}
