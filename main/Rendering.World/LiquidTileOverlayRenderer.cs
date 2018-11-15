using UnityEngine;

namespace Rendering.World
{
	public class LiquidTileOverlayRenderer : TileRenderer
	{
		private enum LiquidConnections
		{
			Left = 1,
			Right = 2,
			Both = 3,
			Empty = 0x80
		}

		public static LiquidTileOverlayRenderer Instance;

		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			ShaderReloader.Register(OnShadersReloaded);
			Instance = this;
		}

		protected override Mask[] GetMasks()
		{
			return new Mask[3]
			{
				new Mask(Atlas, 0, false, false, false, false),
				new Mask(Atlas, 0, false, true, false, false),
				new Mask(Atlas, 1, false, false, false, false)
			};
		}

		public void OnShadersReloaded()
		{
			foreach (Element element in ElementLoader.elements)
			{
				if (element.IsLiquid && element.substance != null && (Object)element.substance.material != (Object)null)
				{
					Material material = new Material(element.substance.material);
					InitAlphaMaterial(material, element);
					int idx = element.substance.idx;
					for (int i = 0; i < Masks.Length; i++)
					{
						int num = idx * Masks.Length + i;
						element.substance.RefreshPropertyBlock();
						Brushes[num].SetMaterial(material, element.substance.propertyBlock);
					}
				}
			}
		}

		public override void LoadBrushes()
		{
			Brushes = new Brush[ElementLoader.elements.Count * Masks.Length];
			foreach (Element element in ElementLoader.elements)
			{
				if (element.IsLiquid && element.substance != null && (Object)element.substance.material != (Object)null)
				{
					Material material = new Material(element.substance.material);
					InitAlphaMaterial(material, element);
					int idx = element.substance.idx;
					for (int i = 0; i < Masks.Length; i++)
					{
						int num = idx * Masks.Length + i;
						element.substance.RefreshPropertyBlock();
						Brushes[num] = new Brush(num, element.id.ToString(), material, Masks[i], ActiveBrushes, DirtyBrushes, TileGridWidth, element.substance.propertyBlock);
					}
				}
			}
		}

		private void InitAlphaMaterial(Material alpha_material, Element element)
		{
			alpha_material.name = element.name;
			alpha_material.renderQueue = RenderQueues.BlockTiles + element.substance.idx;
			alpha_material.EnableKeyword("ALPHA");
			alpha_material.DisableKeyword("OPAQUE");
			alpha_material.SetTexture("_AlphaTestMap", Atlas.texture);
			alpha_material.SetInt("_SrcAlpha", 5);
			alpha_material.SetInt("_DstAlpha", 10);
			alpha_material.SetInt("_ZWrite", 0);
			alpha_material.SetColor("_Colour", element.substance.colour);
		}

		private bool RenderLiquid(int cell, int cell_above)
		{
			bool result = false;
			if (Grid.Element[cell].IsSolid)
			{
				Element element = Grid.Element[cell_above];
				if (element.IsLiquid && (Object)element.substance.material != (Object)null)
				{
					result = true;
				}
			}
			return result;
		}

		private void SetBrushIdx(int i, ref Tile tile, int substance_idx, LiquidConnections connections, Brush[] brush_array, int[] brush_grid)
		{
			if (connections == LiquidConnections.Empty)
			{
				brush_grid[tile.Idx * 4 + i] = -1;
			}
			else
			{
				Brush brush = brush_array[(int)(substance_idx * tile.MaskCount + connections - 1)];
				brush.Add(tile.Idx);
				brush_grid[tile.Idx * 4 + i] = brush.Id;
			}
		}

		public override void MarkDirty(ref Tile tile, Brush[] brush_array, int[] brush_grid)
		{
			if (RenderLiquid(tile.TileCells.Cell0, tile.TileCells.Cell2))
			{
				if (RenderLiquid(tile.TileCells.Cell1, tile.TileCells.Cell3))
				{
					SetBrushIdx(0, ref tile, Grid.Element[tile.TileCells.Cell2].substance.idx, LiquidConnections.Both, brush_array, brush_grid);
				}
				else
				{
					SetBrushIdx(0, ref tile, Grid.Element[tile.TileCells.Cell2].substance.idx, LiquidConnections.Left, brush_array, brush_grid);
				}
			}
			else if (RenderLiquid(tile.TileCells.Cell1, tile.TileCells.Cell3))
			{
				SetBrushIdx(1, ref tile, Grid.Element[tile.TileCells.Cell3].substance.idx, LiquidConnections.Right, brush_array, brush_grid);
			}
		}
	}
}
