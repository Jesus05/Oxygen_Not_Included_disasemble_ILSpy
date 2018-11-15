using System.Collections.Generic;
using UnityEngine;

public class SandboxBrushTool : BrushTool
{
	public static SandboxBrushTool instance;

	protected HashSet<int> recentlyAffectedCells = new HashSet<int>();

	private Dictionary<int, Color> recentAffectedCellColor = new Dictionary<int, Color>();

	private SandboxSettings settings => SandboxToolParameterMenu.instance.settings;

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		instance = this;
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		SandboxToolParameterMenu.instance.gameObject.SetActive(true);
		SandboxToolParameterMenu.instance.DisableParameters();
		SandboxToolParameterMenu.instance.brushRadiusSlider.row.SetActive(true);
		SandboxToolParameterMenu.instance.massSlider.row.SetActive(true);
		SandboxToolParameterMenu.instance.temperatureSlider.row.SetActive(true);
		SandboxToolParameterMenu.instance.elementSelector.row.SetActive(true);
		SandboxToolParameterMenu.instance.diseaseSelector.row.SetActive(true);
		SandboxToolParameterMenu.instance.diseaseCountSlider.row.SetActive(true);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		SandboxToolParameterMenu.instance.gameObject.SetActive(false);
	}

	public override void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors)
	{
		colors = new HashSet<ToolMenu.CellColorData>();
		foreach (int recentlyAffectedCell in recentlyAffectedCells)
		{
			Color color = recentAffectedCellColor[recentlyAffectedCell];
			float r = color.r;
			Color color2 = recentAffectedCellColor[recentlyAffectedCell];
			float g = color2.g;
			Color color3 = recentAffectedCellColor[recentlyAffectedCell];
			Color color4 = new Color(r, g, color3.b, MathUtil.ReRange(Mathf.Sin(Time.realtimeSinceStartup * 10f), -1f, 1f, 0.1f, 0.2f));
			colors.Add(new ToolMenu.CellColorData(recentlyAffectedCell, color4));
		}
		foreach (int item in cellsInRadius)
		{
			colors.Add(new ToolMenu.CellColorData(item, radiusIndicatorColor));
		}
	}

	public override void SetBrushSize(int radius)
	{
		brushRadius = radius;
		brushOffsets.Clear();
		for (int i = 0; i < brushRadius * 2; i++)
		{
			for (int j = 0; j < brushRadius * 2; j++)
			{
				float num = Vector2.Distance(new Vector2((float)i, (float)j), new Vector2((float)brushRadius, (float)brushRadius));
				if (num < (float)brushRadius - 0.8f)
				{
					brushOffsets.Add(new Vector2((float)(i - brushRadius), (float)(j - brushRadius)));
				}
			}
		}
	}

	protected override void OnPaintCell(int cell, int distFromOrigin)
	{
		base.OnPaintCell(cell, distFromOrigin);
		recentlyAffectedCells.Add(cell);
		if (!recentAffectedCellColor.ContainsKey(cell))
		{
			recentAffectedCellColor.Add(cell, settings.Element.substance.debugColour);
		}
		else
		{
			recentAffectedCellColor[cell] = settings.Element.substance.debugColour;
		}
		Game.CallbackInfo item = new Game.CallbackInfo(delegate
		{
			recentlyAffectedCells.Remove(cell);
			recentAffectedCellColor.Remove(cell);
		}, false);
		int index = Game.Instance.callbackManager.Add(item).index;
		int gameCell = cell;
		SimHashes id = settings.Element.id;
		CellElementEvent sandBoxTool = CellEventLogger.Instance.SandBoxTool;
		float mass = settings.Mass;
		float temperature = settings.temperature;
		int callbackIdx = index;
		SimMessages.ReplaceElement(gameCell, id, sandBoxTool, mass, temperature, Db.Get().Diseases.GetIndex(settings.Disease.IdHash), settings.diseaseCount, callbackIdx);
	}
}
