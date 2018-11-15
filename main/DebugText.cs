using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : KMonoBehaviour
{
	private struct Entry
	{
		public string text;

		public Vector3 pos;

		public Color color;
	}

	public static DebugText Instance;

	private List<Entry> entries = new List<Entry>();

	private List<Text> texts = new List<Text>();

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
	}

	public void Draw(string text, Vector3 pos, Color color)
	{
		Entry entry = default(Entry);
		entry.text = text;
		entry.pos = pos;
		entry.color = color;
		Entry item = entry;
		entries.Add(item);
	}

	private void LateUpdate()
	{
		foreach (Text text2 in texts)
		{
			Object.Destroy(text2.gameObject);
		}
		texts.Clear();
		foreach (Entry entry in entries)
		{
			Entry current2 = entry;
			GameObject gameObject = new GameObject();
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.SetParent(GameScreenManager.Instance.worldSpaceCanvas.GetComponent<RectTransform>());
			gameObject.transform.SetPosition(current2.pos);
			rectTransform.localScale = new Vector3(0.02f, 0.02f, 1f);
			Text text = gameObject.AddComponent<Text>();
			text.font = Assets.DebugFont;
			text.text = current2.text;
			text.color = current2.color;
			text.horizontalOverflow = HorizontalWrapMode.Overflow;
			text.verticalOverflow = VerticalWrapMode.Overflow;
			text.alignment = TextAnchor.MiddleCenter;
			texts.Add(text);
		}
		entries.Clear();
	}
}
