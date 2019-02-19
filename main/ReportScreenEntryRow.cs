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

	private string OnNoteTooltip(float total_accumulation, string tooltip_text, ReportManager.ReportEntry.Order order, ReportManager.FormattingFn format_fn, Func<ReportManager.ReportEntry.Note, bool> is_note_applicable_cb, ReportManager.GroupFormattingFn group_format_fn = null)
	{
		notes.Clear();
		entry.IterateNotes(delegate(ReportManager.ReportEntry.Note note)
		{
			if (is_note_applicable_cb(note))
			{
				notes.Add(note);
			}
		});
		string text = string.Empty;
		float num = 0f;
		num = ((entry.contextEntries.Count <= 0) ? ((float)notes.Count) : ((float)entry.contextEntries.Count));
		num = Mathf.Max(num, 1f);
		foreach (ReportManager.ReportEntry.Note item in Sort(notes, reportGroup.posNoteOrder))
		{
			ReportManager.ReportEntry.Note current = item;
			string arg = format_fn(current.value);
			if (toggle.gameObject.activeInHierarchy && group_format_fn != null)
			{
				arg = group_format_fn(current.value, num);
			}
			text = string.Format(UI.ENDOFDAYREPORT.NOTES.NOTE_ENTRY_LINE_ITEM, text, current.note, arg);
		}
		string arg2 = format_fn(total_accumulation);
		if (group_format_fn != null && entry.context == null)
		{
			arg2 = group_format_fn(total_accumulation, num);
		}
		return string.Format(tooltip_text + "\n" + text, arg2);
	}

	private string OnNegativeNoteTooltip()
	{
		return OnNoteTooltip(entry.Negative, reportGroup.negativeTooltip, reportGroup.negNoteOrder, reportGroup.formatfn, (ReportManager.ReportEntry.Note note) => IsNegativeNote(note), reportGroup.groupFormatfn);
	}

	private string OnPositiveNoteTooltip()
	{
		return OnNoteTooltip(entry.Positive, reportGroup.positiveTooltip, reportGroup.posNoteOrder, reportGroup.formatfn, (ReportManager.ReportEntry.Note note) => IsPositiveNote(note), reportGroup.groupFormatfn);
	}

	private string OnNetNoteTooltip()
	{
		if (entry.Net > 0f)
		{
			return OnPositiveNoteTooltip();
		}
		return OnNegativeNoteTooltip();
	}

	private bool IsPositiveNote(ReportManager.ReportEntry.Note note)
	{
		if (note.value > 0f)
		{
			return true;
		}
		return false;
	}

	private bool IsNegativeNote(ReportManager.ReportEntry.Note note)
	{
		if (note.value < 0f)
		{
			return true;
		}
		return false;
	}

	public void SetLine(ReportManager.ReportEntry entry, ReportManager.ReportGroup reportGroup)
	{
		this.entry = entry;
		this.reportGroup = reportGroup;
		ListPool<ReportManager.ReportEntry.Note, ReportScreenEntryRow>.PooledList pos_notes = ListPool<ReportManager.ReportEntry.Note, ReportScreenEntryRow>.Allocate();
		entry.IterateNotes(delegate(ReportManager.ReportEntry.Note note)
		{
			if (IsPositiveNote(note))
			{
				pos_notes.Add(note);
			}
		});
		ListPool<ReportManager.ReportEntry.Note, ReportScreenEntryRow>.PooledList neg_notes = ListPool<ReportManager.ReportEntry.Note, ReportScreenEntryRow>.Allocate();
		entry.IterateNotes(delegate(ReportManager.ReportEntry.Note note)
		{
			if (IsNegativeNote(note))
			{
				neg_notes.Add(note);
			}
		});
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
			string text = reportGroup.formatfn(entry.Positive);
			if (reportGroup.groupFormatfn != null && entry.context == null)
			{
				float num6 = 0f;
				num6 = ((entry.contextEntries.Count <= 0) ? ((float)pos_notes.Count) : ((float)entry.contextEntries.Count));
				num6 = Mathf.Max(num6, 1f);
				text = reportGroup.groupFormatfn(entry.Positive, num6);
			}
			added.text = text;
			addedValue = entry.Positive;
		}
		if (removedValue != entry.Negative)
		{
			string text2 = reportGroup.formatfn(entry.Negative);
			if (reportGroup.groupFormatfn != null && entry.context == null)
			{
				float num7 = 0f;
				num7 = ((entry.contextEntries.Count <= 0) ? ((float)neg_notes.Count) : ((float)entry.contextEntries.Count));
				num7 = Mathf.Max(num7, 1f);
				text2 = reportGroup.groupFormatfn(entry.Negative, num7);
			}
			removed.text = text2;
			removedValue = entry.Negative;
		}
		if (netValue != entry.Net)
		{
			object text3;
			if (reportGroup.formatfn == null)
			{
				num3 = entry.Net;
				text3 = num3.ToString();
			}
			else
			{
				text3 = reportGroup.formatfn(entry.Net);
			}
			string text4 = (string)text3;
			if (reportGroup.groupFormatfn != null && entry.context == null)
			{
				float num8 = 0f;
				num8 = ((entry.contextEntries.Count <= 0) ? ((float)(pos_notes.Count + neg_notes.Count)) : ((float)entry.contextEntries.Count));
				num8 = Mathf.Max(num8, 1f);
				text4 = reportGroup.groupFormatfn(entry.Net, num8);
			}
			net.text = text4;
			netValue = entry.Net;
		}
		pos_notes.Recycle();
		neg_notes.Recycle();
	}
}
