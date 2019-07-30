using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameInputMapping
{
	public static BindingEntry[] KeyBindings;

	public static BindingEntry[] DefaultBindings
	{
		get;
		private set;
	}

	private static string BindingsFilename => Path.Combine(Util.RootFolder(), "keybindings.json");

	public static HashSet<KeyCode> GetKeyCodes()
	{
		HashSet<KeyCode> hashSet = new HashSet<KeyCode>();
		BindingEntry[] bindingEntries = GetBindingEntries();
		for (int i = 0; i < bindingEntries.Length; i++)
		{
			BindingEntry bindingEntry = bindingEntries[i];
			if (bindingEntry.mKeyCode < KKeyCode.KleiKeys)
			{
				hashSet.Add((KeyCode)bindingEntry.mKeyCode);
			}
		}
		hashSet.Add(KeyCode.LeftAlt);
		hashSet.Add(KeyCode.LeftControl);
		hashSet.Add(KeyCode.LeftShift);
		hashSet.Add(KeyCode.CapsLock);
		return hashSet;
	}

	public static HashSet<string> GetAxis()
	{
		HashSet<string> hashSet = new HashSet<string>();
		hashSet.Add("Mouse X");
		hashSet.Add("Mouse Y");
		hashSet.Add("Mouse ScrollWheel");
		return hashSet;
	}

	public static void SetDefaultKeyBindings(BindingEntry[] default_keybindings)
	{
		DefaultBindings = default_keybindings;
		KeyBindings = (BindingEntry[])default_keybindings.Clone();
	}

	public static BindingEntry[] GetBindingEntries()
	{
		return KeyBindings;
	}

	public static BindingEntry FindEntry(Action mAction)
	{
		BindingEntry[] keyBindings = KeyBindings;
		for (int i = 0; i < keyBindings.Length; i++)
		{
			BindingEntry result = keyBindings[i];
			if (result.mAction == mAction)
			{
				return result;
			}
		}
		Debug.Assert(false, "Unbound action " + mAction.ToString());
		return KeyBindings[0];
	}

	public static bool CompareActionKeyCodes(Action a, Action b)
	{
		BindingEntry bindingEntry = FindEntry(a);
		BindingEntry bindingEntry2 = FindEntry(b);
		return bindingEntry.mKeyCode == bindingEntry2.mKeyCode && bindingEntry.mModifier == bindingEntry2.mModifier;
	}

	public static BindingEntry[] FindEntriesByKeyCode(KKeyCode keycode)
	{
		List<BindingEntry> list = new List<BindingEntry>();
		BindingEntry[] keyBindings = KeyBindings;
		for (int i = 0; i < keyBindings.Length; i++)
		{
			BindingEntry item = keyBindings[i];
			if (item.mKeyCode == keycode)
			{
				list.Add(item);
			}
		}
		return list.ToArray();
	}

	public static void SaveBindings()
	{
		if (!Directory.Exists(Util.RootFolder()))
		{
			Directory.CreateDirectory(Util.RootFolder());
		}
		List<BindingEntry> list = new List<BindingEntry>();
		BindingEntry[] keyBindings = KeyBindings;
		for (int i = 0; i < keyBindings.Length; i++)
		{
			BindingEntry bindingEntry = keyBindings[i];
			bool flag = false;
			BindingEntry[] defaultBindings = DefaultBindings;
			foreach (BindingEntry b in defaultBindings)
			{
				if (bindingEntry == b)
				{
					flag = true;
					break;
				}
			}
			if (!flag && bindingEntry.mRebindable)
			{
				list.Add(bindingEntry);
			}
		}
		if (list.Count > 0)
		{
			string contents = JsonConvert.SerializeObject(list);
			File.WriteAllText(BindingsFilename, contents);
		}
		else if (File.Exists(BindingsFilename))
		{
			File.Delete(BindingsFilename);
		}
	}

	public static void LoadBindings()
	{
		KeyBindings = (BindingEntry[])DefaultBindings.Clone();
		if (File.Exists(BindingsFilename))
		{
			string text = File.ReadAllText(BindingsFilename);
			if (text != null && !(text == string.Empty))
			{
				BindingEntry[] array = null;
				try
				{
					array = JsonConvert.DeserializeObject<BindingEntry[]>(text);
				}
				catch
				{
					DebugUtil.LogErrorArgs("Error parsing", BindingsFilename);
				}
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < KeyBindings.Length; i++)
					{
						BindingEntry bindingEntry = KeyBindings[i];
						BindingEntry[] array2 = array;
						for (int j = 0; j < array2.Length; j++)
						{
							BindingEntry bindingEntry2 = array2[j];
							if (bindingEntry2.mAction == bindingEntry.mAction && bindingEntry.mRebindable)
							{
								BindingEntry bindingEntry3 = bindingEntry;
								bindingEntry3.mButton = bindingEntry2.mButton;
								bindingEntry3.mKeyCode = bindingEntry2.mKeyCode;
								bindingEntry3.mModifier = bindingEntry2.mModifier;
								KeyBindings[i] = bindingEntry3;
								break;
							}
						}
					}
				}
			}
		}
	}
}
