using System.Collections.Generic;
using UnityEngine;

namespace KMod
{
	public class ModErrorsScreen : KScreen
	{
		[SerializeField]
		private KButton closeButtonTitle;

		[SerializeField]
		private KButton closeButton;

		[SerializeField]
		private GameObject entryPrefab;

		[SerializeField]
		private Transform entryParent;

		public static bool ShowErrors(List<Event> events)
		{
			if (Global.Instance.modManager.events.Count == 0)
			{
				return false;
			}
			GameObject parent = GameObject.Find("Canvas");
			ModErrorsScreen modErrorsScreen = Util.KInstantiateUI<ModErrorsScreen>(Global.Instance.modErrorsPrefab, parent, false);
			modErrorsScreen.Initialize(events);
			modErrorsScreen.gameObject.SetActive(true);
			return true;
		}

		private void Initialize(List<Event> events)
		{
			foreach (Event @event in events)
			{
				Event current = @event;
				HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(entryPrefab, entryParent.gameObject, true);
				LocText reference = hierarchyReferences.GetReference<LocText>("Title");
				LocText reference2 = hierarchyReferences.GetReference<LocText>("Description");
				KButton reference3 = hierarchyReferences.GetReference<KButton>("Details");
				Event.GetUIStrings(current.event_type, out string title, out string title_tooltip);
				reference.text = title;
				reference.GetComponent<ToolTip>().toolTip = title_tooltip;
				reference2.text = current.mod.title;
				ToolTip component = reference2.GetComponent<ToolTip>();
				if ((Object)component != (Object)null)
				{
					component.toolTip = current.mod.ToString();
				}
				reference3.isInteractable = false;
				Mod mod = Global.Instance.modManager.FindMod(current.mod);
				if (mod != null)
				{
					if ((Object)component != (Object)null && !string.IsNullOrEmpty(mod.description))
					{
						component.toolTip = mod.description;
					}
					if (mod.on_managed != null)
					{
						reference3.onClick += mod.on_managed;
						reference3.isInteractable = true;
					}
				}
			}
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			closeButtonTitle.onClick += Deactivate;
			closeButton.onClick += Deactivate;
		}
	}
}
