using System.Collections.Generic;
using UnityEngine;

public class SandboxFloodTool : FloodTool
{
	public static SandboxFloodTool instance;

	protected HashSet<int> recentlyAffectedCells = new HashSet<int>();

	protected HashSet<int> cellsToAffect = new HashSet<int>();

	protected Color recentlyAffectedCellColor = new Color(1f, 1f, 1f, 0.1f);

	private SandboxSettings settings => SandboxToolParameterMenu.instance.settings;

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		instance = this;
		floodCriteria = ((int cell) => Grid.IsValidCell(cell) && Grid.Element[cell] == Grid.Element[mouseCell]);
		paintArea = delegate(HashSet<int> cells)
		{
			foreach (int cell in cells)
			{
				PaintCell(cell);
			}
		};
	}

	private void PaintCell(int cell)
	{
		recentlyAffectedCells.Add(cell);
		Game.CallbackInfo item = new Game.CallbackInfo(delegate
		{
			recentlyAffectedCells.Remove(cell);
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

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		SandboxToolParameterMenu.instance.gameObject.SetActive(true);
		SandboxToolParameterMenu.instance.DisableParameters();
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
			colors.Add(new ToolMenu.CellColorData(recentlyAffectedCell, recentlyAffectedCellColor));
		}
		foreach (int item in cellsToAffect)
		{
			colors.Add(new ToolMenu.CellColorData(item, areaColour));
		}
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		base.OnMouseMove(cursorPos);
		cellsToAffect = Flood(Grid.PosToCell(cursorPos));
	}
}
