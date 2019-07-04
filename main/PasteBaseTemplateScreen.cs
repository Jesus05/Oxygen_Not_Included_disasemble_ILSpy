using System.Collections.Generic;
using UnityEngine;

public class PasteBaseTemplateScreen : KScreen
{
	public static PasteBaseTemplateScreen Instance;

	public GameObject button_list_container;

	public GameObject prefab_paste_button;

	private List<string> base_template_assets;

	private List<GameObject> template_buttons = new List<GameObject>();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		TemplateCache.Init();
		ConsumeMouseScroll = true;
		RefreshStampButtons();
	}

	public void RefreshStampButtons()
	{
		foreach (GameObject template_button in template_buttons)
		{
			Object.Destroy(template_button);
		}
		template_buttons.Clear();
		base_template_assets = TemplateCache.CollectBaseTemplateNames("bases");
		base_template_assets.AddRange(TemplateCache.CollectBaseTemplateNames("poi"));
		base_template_assets.AddRange(TemplateCache.CollectBaseTemplateNames(""));
		foreach (string base_template_asset in base_template_assets)
		{
			GameObject gameObject = Util.KInstantiateUI(prefab_paste_button, button_list_container, true);
			KButton component = gameObject.GetComponent<KButton>();
			string template_name = base_template_asset;
			component.onClick += delegate
			{
				OnClickPasteButton(template_name);
			};
			LocText componentInChildren = gameObject.GetComponentInChildren<LocText>();
			componentInChildren.text = template_name;
			template_buttons.Add(gameObject);
		}
	}

	private void OnClickPasteButton(string template_name)
	{
		if (template_name != null)
		{
			DebugTool.Instance.DeactivateTool(null);
			DebugBaseTemplateButton.Instance.ClearSelection();
			DebugBaseTemplateButton.Instance.nameField.text = template_name;
			TemplateContainer template = TemplateCache.GetTemplate(template_name);
			StampTool.Instance.Activate(template, true, false);
		}
	}
}
