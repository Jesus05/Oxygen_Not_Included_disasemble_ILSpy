using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReportScreenEntryRow : KMonoBehaviour
{
	[SerializeField]
	public new LocText name;

	[SerializeField]
	public LocText added;

	[SerializeField]
	public LocText removed;

	[SerializeField]
	public LocText net;

	private float addedValue = float.NegativeInfinity;

	private float removedValue = float.NegativeInfinity;

	private float netValue = float.NegativeInfinity;

	[SerializeField]
	public MultiToggle toggle;

	[SerializeField]
	private LayoutElement spacer;

	[SerializeField]
	private Image bgImage;

	public float groupSpacerWidth;

	public float contextSpacerWidth;

	private float nameWidth = 164f;

	private float indentWidth = 6f;

	[SerializeField]
	private Color oddRowColor;

	private static List<ReportManager.ReportEntry.Note> notes = new List<ReportManager.ReportEntry.Note>();

	private ReportManager.ReportEntry entry;

	private ReportManager.ReportGroup reportGroup;

	private List<ReportManager.ReportEntry.Note> Sort(List<ReportManager.ReportEntry.Note> notes, ReportManager.ReportEntry.Order order)
	{
		switch (order)
		{
		case ReportManager.ReportEntry.Order.Ascending:
			notes.Sort((ReportManager.ReportEntry.Note x, ReportManager.ReportEntry.Note y) => x.value.CompareTo(y.value));
			break;
		case ReportManager.ReportEntry.Order.Descending:
			notes.Sort((ReportManager.ReportEntry.Note x, ReportManager.ReportEntry.Note y) => y.value.CompareTo(x.value));
			break;
		}
		return notes;
	}

	public static void DestroyStatics()
	{
		notes = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		added.GetComponent<ToolTip>().OnToolTip = OnPositiveNoteTooltip;
		removed.GetComponent<ToolTip>().OnToolTip = OnNegativeNoteTooltip;
		net.GetComponent<ToolTip>().OnToolTip = OnNetNoteTooltip;
		name.GetComponent<ToolTip>().OnToolTip = OnNetNoteTooltip;
	}

	private string OnNoteTooltip(float total_accumulation, string tooltip_text, ReportManager.ReportEntry.Order order, ReportManager.FormattingFn format_fn, Func<ReportManager.ReportEntry.Note, bool> is_note_applicable_cb)
	{
		notes.Clear();
		entry.IterateNotes(delegate(ReportManager.ReportEntry.Note note)
		{
			if (is_note_applicable_cb(note))
			{
				notes.Add(note);
			}
		});
		string text = "";
		foreach (ReportManager.ReportEntry.Note item in Sort(notes, reportGroup.posNoteOrder))
		{
			ReportManager.ReportEntry.Note current = item;
			text = string.Format(UI.ENDOFDAYREPORT.NOTES.NOTE_ENTRY_LINE_ITEM, text, current.note, format_fn(current.value));
		}
		return string.Format(tooltip_text + "\n" + text, format_fn(total_accumulation));
	}

	private string OnNegativeNoteTooltip()
	{
		return OnNoteTooltip(entry.Negative, reportGroup.negativeTooltip, reportGroup.negNoteOrder, reportGroup.formatfn, delegate(ReportManager.ReportEntry.Note note)
		{
			if (!(note.value < 0f))
			{
				return false;
			}
			return true;
		});
	}

	private string OnPositiveNoteTooltip()
	{
		return OnNoteTooltip(entry.Positive, reportGroup.positiveTooltip, reportGroup.posNoteOrder, reportGroup.formatfn, delegate(ReportManager.ReportEntry.Note note)
		{
			if (!(note.value > 0f))
			{
				return false;
			}
			return true;
		});
	}

	private string OnNetNoteTooltip()
	{
		if (!(entry.Net > 0f))
		{
			return OnNegativeNoteTooltip();
		}
		return OnPositiveNoteTooltip();
	}

	public void SetLine(ReportManager.ReportEntry entry, ReportManager.ReportGroup reportGroup)
	{
		this.entry = entry;
		this.reportGroup = reportGroup;
		LayoutElement component = name.GetComponent<LayoutElement>();
		float num3;
		if (entry.context == null)
		{
			num3 = (component.minWidth = (component.preferredWidth = nameWidth));
			if (entry.HasContextEntries())
			{
				toggle.gameObject.SetActive(true);
				spacer.minWidth = groupSpacerWidth;
			}
			else
			{
				toggle.gameObject.SetActive(false);
				spacer.minWidth = groupSpacerWidth + toggle.GetComponent<LayoutElement>().minWidth;
			}
			name.text = reportGroup.stringKey;
		}
		else
		{
			toggle.gameObject.SetActive(false);
			spacer.minWidth = contextSpacerWidth;
			name.text = entry.context;
			num3 = (component.minWidth = (component.preferredWidth = nameWidth - indentWidth));
			if (base.transform.GetSiblingIndex() % 2 != 0)
			{
				bgImage.color = oddRowColor;
			}
		}
		if (addedValue != entry.Positive)
		{
			added.text = reportGroup.formatfn(entry.Positive);
			addedValue = entry.Positive;
		}
		if (removedValue != entry.Negative)
		{
			removed.text = reportGroup.formatfn(entry.Negative);
			removedValue = entry.Negative;
		}
		if (netValue != entry.Net)
		{
			LocText locText = net;
			object text;
			if (reportGroup.formatfn == null)
			{
				num3 = entry.Net;
				text = num3.ToString();
			}
			else
			{
				text = reportGroup.formatfn(entry.Net);
			}
			locText.text = (string)text;
			netValue = entry.Net;
		}
	}
}
