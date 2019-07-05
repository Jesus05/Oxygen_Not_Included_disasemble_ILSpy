#define UNITY_ASSERTIONS
using Klei;
using Rendering;
using System;
using System.Collections.Generic;
using UnityEngine;

public class World : KMonoBehaviour
{
	public Action<int> OnSolidChanged;

	public Action<int> OnLiquidChanged;

	public BlockTileRenderer blockTileRenderer;

	[NonSerialized]
	[MyCmpGet]
	public GroundRenderer groundRenderer;

	private List<int> revealedCells = new List<int>();

	public static int DebugCellID = -1;

	private List<int> changedCells = new List<int>();

	public static World Instance
	{
		get;
		private set;
	}

	public SubworldZoneRenderData zoneRenderData
	{
		get;
		private set;
	}

	protected override void OnPrefabInit()
	{
		Debug.Assert((UnityEngine.Object)Instance == (UnityEngine.Object)null);
		Instance = this;
		blockTileRenderer = GetComponent<BlockTileRenderer>();
	}

	protected override void OnSpawn()
	{
		GetComponent<SimDebugView>().OnReset();
		GetComponent<PropertyTextures>().OnReset(null);
		zoneRenderData = GetComponent<SubworldZoneRenderData>();
		Grid.OnReveal = (Action<int>)Delegate.Combine(Grid.OnReveal, new Action<int>(OnReveal));
	}

	protected override void OnLoadLevel()
	{
		Instance = null;
		if ((UnityEngine.Object)blockTileRenderer != (UnityEngine.Object)null)
		{
			blockTileRenderer.FreeResources();
		}
		blockTileRenderer = null;
		if ((UnityEngine.Object)groundRenderer != (UnityEngine.Object)null)
		{
			groundRenderer.FreeResources();
		}
		groundRenderer = null;
		revealedCells.Clear();
		revealedCells = null;
		base.OnLoadLevel();
	}

	public unsafe void UpdateCellInfo(List<SolidInfo> solidInfo, List<CallbackInfo> callbackInfo, int num_solid_substance_change_info, Sim.SolidSubstanceChangeInfo* solid_substance_change_info, int num_liquid_change_info, Sim.LiquidChangeInfo* liquid_change_info)
	{
		int count = solidInfo.Count;
		changedCells.Clear();
		for (int i = 0; i < count; i++)
		{
			SolidInfo solidInfo2 = solidInfo[i];
			int cellIdx = solidInfo2.cellIdx;
			if (!changedCells.Contains(cellIdx))
			{
				changedCells.Add(cellIdx);
			}
			Pathfinding.Instance.AddDirtyNavGridCell(cellIdx);
			WorldDamage.Instance.OnSolidStateChanged(cellIdx);
			if (OnSolidChanged != null)
			{
				OnSolidChanged(cellIdx);
			}
		}
		if (changedCells.Count != 0)
		{
			SaveGame.Instance.entombedItemManager.OnSolidChanged(changedCells);
			GameScenePartitioner.Instance.TriggerEvent(changedCells, GameScenePartitioner.Instance.solidChangedLayer, null);
		}
		int count2 = callbackInfo.Count;
		for (int j = 0; j < count2; j++)
		{
			callbackInfo[j].Release();
		}
		for (int k = 0; k < num_solid_substance_change_info; k++)
		{
			int cellIdx2 = solid_substance_change_info[k].cellIdx;
			if (!Grid.IsValidCell(cellIdx2))
			{
				Debug.LogError(cellIdx2);
			}
			else
			{
				Grid.RenderedByWorld[cellIdx2] = (Grid.Element[cellIdx2].substance.renderedByWorld && (UnityEngine.Object)Grid.Objects[cellIdx2, 9] == (UnityEngine.Object)null);
				groundRenderer.MarkDirty(cellIdx2);
			}
		}
		GameScenePartitioner instance = GameScenePartitioner.Instance;
		changedCells.Clear();
		for (int l = 0; l < num_liquid_change_info; l++)
		{
			int cellIdx3 = liquid_change_info[l].cellIdx;
			UnityEngine.Debug.Assert(Grid.IsValidCell(cellIdx3));
			changedCells.Add(cellIdx3);
			if (OnLiquidChanged != null)
			{
				OnLiquidChanged(cellIdx3);
			}
		}
		instance.TriggerEvent(changedCells, GameScenePartitioner.Instance.liquidChangedLayer, null);
	}

	private void OnReveal(int cell)
	{
		revealedCells.Add(cell);
	}

	private void LateUpdate()
	{
		if (!Game.IsQuitting())
		{
			if (GameUtil.IsCapturingTimeLapse())
			{
				Game.Instance.UpdateGameActiveRegion(0, 0, Grid.WidthInCells, Grid.HeightInCells);
				groundRenderer.RenderAll();
				KAnimBatchManager.Instance().UpdateActiveArea(new Vector2I(0, 0), new Vector2I(9999, 9999));
				KAnimBatchManager.Instance().UpdateDirty(Time.frameCount);
				KAnimBatchManager.Instance().Render();
			}
			else
			{
				GridArea visibleArea = GridVisibleArea.GetVisibleArea();
				groundRenderer.Render(visibleArea.Min, visibleArea.Max, false);
				Singleton<KBatchedAnimUpdater>.Instance.GetVisibleArea(out Vector2I vis_chunk_min, out Vector2I vis_chunk_max);
				KAnimBatchManager.Instance().UpdateActiveArea(vis_chunk_min, vis_chunk_max);
				KAnimBatchManager.Instance().UpdateDirty(Time.frameCount);
				KAnimBatchManager.Instance().Render();
			}
			if ((UnityEngine.Object)Camera.main != (UnityEngine.Object)null)
			{
				Camera main = Camera.main;
				Vector3 mousePos = KInputManager.GetMousePos();
				float x = mousePos.x;
				Vector3 mousePos2 = KInputManager.GetMousePos();
				float y = mousePos2.y;
				Vector3 position = Camera.main.transform.GetPosition();
				Vector3 vector = main.ScreenToWorldPoint(new Vector3(x, y, 0f - position.z));
				Shader.SetGlobalVector("_CursorPos", new Vector4(vector.x, vector.y, vector.z, 0f));
			}
			FallingWater.instance.UpdateParticles(Time.deltaTime);
			FallingWater.instance.Render();
			if (revealedCells.Count > 0)
			{
				GameScenePartitioner.Instance.TriggerEvent(revealedCells, GameScenePartitioner.Instance.fogOfWarChangedLayer, null);
				revealedCells.Clear();
			}
		}
	}
}
