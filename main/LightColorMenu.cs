using System;
using UnityEngine;

public class LightColorMenu : KMonoBehaviour
{
	[Serializable]
	public struct LightColor
	{
		public string name;

		public Color color;

		public LightColor(string name, Color color)
		{
			this.name = name;
			this.color = color;
		}
	}

	public LightColor[] lightColors;

	private int currentColor;

	private static readonly EventSystem.IntraObjectHandler<LightColorMenu> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<LightColorMenu>(delegate(LightColorMenu component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	protected override void OnPrefabInit()
	{
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		SetColor(0);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (lightColors.Length > 0)
		{
			int num = lightColors.Length;
			for (int i = 0; i < num; i++)
			{
				if (i != currentColor)
				{
					int new_color = i;
					Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo(lightColors[i].name, lightColors[i].name, delegate
					{
						SetColor(new_color);
					}, Action.NumActions, null, null, null, string.Empty, true), 1f);
				}
			}
		}
	}

	private void SetColor(int color_index)
	{
		if (lightColors.Length > 0 && color_index < lightColors.Length)
		{
			Light2D[] componentsInChildren = GetComponentsInChildren<Light2D>(true);
			foreach (Light2D light2D in componentsInChildren)
			{
				light2D.Color = lightColors[color_index].color;
			}
			MeshRenderer[] componentsInChildren2 = GetComponentsInChildren<MeshRenderer>(true);
			foreach (MeshRenderer meshRenderer in componentsInChildren2)
			{
				Material[] materials = meshRenderer.materials;
				foreach (Material material in materials)
				{
					if (material.name.StartsWith("matScriptedGlow01"))
					{
						material.color = lightColors[color_index].color;
					}
				}
			}
		}
		currentColor = color_index;
	}
}
