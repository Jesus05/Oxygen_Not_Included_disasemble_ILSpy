using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniformGrid<T> where T : IUniformGridObject
{
	private List<T>[] cells;

	private List<T> items;

	private int cellWidth;

	private int cellHeight;

	private int numXCells;

	private int numYCells;

	public UniformGrid()
	{
	}

	public UniformGrid(int width, int height, int cellWidth, int cellHeight)
	{
		Reset(width, height, cellWidth, cellHeight);
	}

	public void Reset(int width, int height, int cellWidth, int cellHeight)
	{
		this.cellWidth = cellWidth;
		this.cellHeight = cellHeight;
		numXCells = (int)Math.Ceiling((double)((float)width / (float)cellWidth));
		numYCells = (int)Math.Ceiling((double)((float)height / (float)cellHeight));
		int num = numXCells * numYCells;
		cells = new List<T>[num];
		items = new List<T>();
	}

	public void Clear()
	{
		cellWidth = 0;
		cellHeight = 0;
		numXCells = 0;
		numYCells = 0;
		cells = null;
	}

	public void Add(T item)
	{
		Vector2 vector = item.PosMin();
		Vector2 vector2 = item.PosMax();
		int num = (int)Math.Max(vector.x / (float)cellWidth, 0f);
		int num2 = (int)Math.Max(vector2.y / (float)cellHeight, 0f);
		int num3 = Math.Min(numXCells - 1, (int)Math.Ceiling((double)(vector2.x / (float)cellWidth)));
		int num4 = Math.Min(numYCells - 1, (int)Math.Ceiling((double)(vector2.y / (float)cellHeight)));
		for (int i = num2; i <= num4; i++)
		{
			for (int j = num; j <= num3; j++)
			{
				int num5 = i * numXCells + j;
				List<T> list = cells[num5];
				if (list == null)
				{
					list = new List<T>();
					cells[num5] = list;
				}
				list.Add(item);
				items.Add(item);
			}
		}
	}

	public void Remove(T item)
	{
		Vector2 vector = item.PosMin();
		Vector2 vector2 = item.PosMax();
		int num = (int)Math.Max(vector.x / (float)cellWidth, 0f);
		int num2 = (int)Math.Max(vector2.y / (float)cellHeight, 0f);
		int num3 = Math.Min(numXCells - 1, (int)Math.Ceiling((double)(vector2.x / (float)cellWidth)));
		int num4 = Math.Min(numYCells - 1, (int)Math.Ceiling((double)(vector2.y / (float)cellHeight)));
		for (int i = num2; i <= num4; i++)
		{
			for (int j = num; j <= num3; j++)
			{
				List<T> list = cells[i * numXCells + j];
				if (list != null)
				{
					int num5 = list.IndexOf(item);
					if (num5 != -1)
					{
						list.Remove(item);
						items.Remove(item);
					}
				}
			}
		}
	}

	public IEnumerable GetAllIntersecting(IUniformGridObject item)
	{
		Vector2 min = item.PosMin();
		Vector2 max = item.PosMax();
		return GetAllIntersecting(min, max);
	}

	public IEnumerable GetAllIntersecting(Vector2 pos)
	{
		return GetAllIntersecting(pos, pos);
	}

	public IEnumerable GetAllIntersecting(Vector2 min, Vector2 max)
	{
		HashSet<T> hashSet = new HashSet<T>();
		GetAllIntersecting(min, max, hashSet);
		return hashSet;
	}

	public void GetAllIntersecting(Vector2 min, Vector2 max, ICollection<T> results)
	{
		int num = Math.Max(0, Math.Min((int)(min.x / (float)cellWidth), numXCells - 1));
		int num2 = Math.Max(0, Math.Min((int)Math.Ceiling((double)(max.x / (float)cellWidth)), numXCells - 1));
		int num3 = Math.Max(0, Math.Min((int)(min.y / (float)cellHeight), numYCells - 1));
		int num4 = Math.Max(0, Math.Min((int)Math.Ceiling((double)(max.y / (float)cellHeight)), numYCells - 1));
		for (int i = num3; i <= num4; i++)
		{
			for (int j = num; j <= num2; j++)
			{
				List<T> list = cells[i * numXCells + j];
				if (list != null)
				{
					for (int k = 0; k < list.Count; k++)
					{
						results.Add(list[k]);
					}
				}
			}
		}
	}

	public ICollection<T> GetAllItems()
	{
		return items;
	}
}
