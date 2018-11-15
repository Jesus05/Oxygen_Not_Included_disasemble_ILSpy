using System;
using System.Collections.Generic;
using UnityEngine;

public class UniformPoissonDiskSampler
{
	private struct Settings
	{
		public Vector2 TopLeft;

		public Vector2 LowerRight;

		public Vector2 Center;

		public Vector2 Dimensions;

		public float? RejectionSqDistance;

		public float MinimumDistance;

		public float CellSize;

		public int GridWidth;

		public int GridHeight;
	}

	private struct State
	{
		public Vector2?[,] Grid;

		public List<Vector2> ActivePoints;

		public List<Vector2> Points;
	}

	public const int DefaultPointsPerIteration = 30;

	private static readonly float SquareRootTwo = (float)Math.Sqrt(2.0);

	private SeededRandom myRandom;

	public UniformPoissonDiskSampler(SeededRandom seed)
	{
		myRandom = seed;
	}

	public List<Vector2> SampleCircle(Vector2 center, float radius, float minimumDistance)
	{
		return SampleCircle(center, radius, minimumDistance, 30);
	}

	public List<Vector2> SampleCircle(Vector2 center, float radius, float minimumDistance, int pointsPerIteration)
	{
		return Sample(center - new Vector2(radius, radius), center + new Vector2(radius, radius), radius, minimumDistance, pointsPerIteration);
	}

	public List<Vector2> SampleRectangle(Vector2 topLeft, Vector2 lowerRight, float minimumDistance)
	{
		return SampleRectangle(topLeft, lowerRight, minimumDistance, 30);
	}

	public List<Vector2> SampleRectangle(Vector2 topLeft, Vector2 lowerRight, float minimumDistance, int pointsPerIteration)
	{
		return Sample(topLeft, lowerRight, null, minimumDistance, pointsPerIteration);
	}

	private List<Vector2> Sample(Vector2 topLeft, Vector2 lowerRight, float? rejectionDistance, float minimumDistance, int pointsPerIteration)
	{
		Settings settings = default(Settings);
		settings.TopLeft = topLeft;
		settings.LowerRight = lowerRight;
		settings.Dimensions = lowerRight - topLeft;
		settings.Center = (topLeft + lowerRight) / 2f;
		settings.CellSize = minimumDistance / SquareRootTwo;
		settings.MinimumDistance = minimumDistance;
		settings.RejectionSqDistance = ((!rejectionDistance.HasValue) ? null : (rejectionDistance * rejectionDistance));
		Settings settings2 = settings;
		settings2.GridWidth = (int)(settings2.Dimensions.x / settings2.CellSize) + 1;
		settings2.GridHeight = (int)(settings2.Dimensions.y / settings2.CellSize) + 1;
		State state = default(State);
		state.Grid = new Vector2?[settings2.GridWidth, settings2.GridHeight];
		state.ActivePoints = new List<Vector2>();
		state.Points = new List<Vector2>();
		State state2 = state;
		AddFirstPoint(ref settings2, ref state2);
		while (state2.ActivePoints.Count != 0)
		{
			int index = myRandom.RandomRange(0, state2.ActivePoints.Count - 1);
			Vector2 point = state2.ActivePoints[index];
			bool flag = false;
			for (int i = 0; i < pointsPerIteration; i++)
			{
				flag |= AddNextPoint(point, ref settings2, ref state2);
			}
			if (!flag)
			{
				state2.ActivePoints.RemoveAt(index);
			}
		}
		return state2.Points;
	}

	private void AddFirstPoint(ref Settings settings, ref State state)
	{
		bool flag = false;
		while (!flag)
		{
			float num = myRandom.RandomValue();
			float num2 = settings.TopLeft.x + settings.Dimensions.x * num;
			num = myRandom.RandomValue();
			float num3 = settings.TopLeft.y + settings.Dimensions.y * num;
			Vector2 vector = new Vector2(num2, num3);
			float? rejectionSqDistance = settings.RejectionSqDistance;
			if (rejectionSqDistance.HasValue)
			{
				float? rejectionSqDistance2 = settings.RejectionSqDistance;
				if (rejectionSqDistance2.HasValue && Vector2.SqrMagnitude(settings.Center - vector) > rejectionSqDistance2.GetValueOrDefault())
				{
					continue;
				}
			}
			flag = true;
			Vector2 vector2 = Denormalize(vector, settings.TopLeft, (double)settings.CellSize);
			state.Grid[(int)vector2.x, (int)vector2.y] = vector;
			state.ActivePoints.Add(vector);
			state.Points.Add(vector);
		}
	}

	private bool AddNextPoint(Vector2 point, ref Settings settings, ref State state)
	{
		bool result = false;
		Vector2 vector = GenerateRandomAround(point, settings.MinimumDistance);
		if (vector.x >= settings.TopLeft.x && vector.x < settings.LowerRight.x && vector.y > settings.TopLeft.y && vector.y < settings.LowerRight.y)
		{
			float? rejectionSqDistance = settings.RejectionSqDistance;
			if (rejectionSqDistance.HasValue)
			{
				float? rejectionSqDistance2 = settings.RejectionSqDistance;
				if (!rejectionSqDistance2.HasValue || !(Vector2.SqrMagnitude(settings.Center - vector) <= rejectionSqDistance2.GetValueOrDefault()))
				{
					goto IL_01f4;
				}
			}
			Vector2 vector2 = Denormalize(vector, settings.TopLeft, (double)settings.CellSize);
			bool flag = false;
			for (int i = (int)Math.Max(0f, vector2.x - 2f); (float)i < Math.Min((float)settings.GridWidth, vector2.x + 3f); i++)
			{
				if (flag)
				{
					break;
				}
				for (int j = (int)Math.Max(0f, vector2.y - 2f); (float)j < Math.Min((float)settings.GridHeight, vector2.y + 3f); j++)
				{
					if (flag)
					{
						break;
					}
					if (state.Grid[i, j].HasValue && Vector2.Distance(state.Grid[i, j].Value, vector) < settings.MinimumDistance)
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				result = true;
				state.ActivePoints.Add(vector);
				state.Points.Add(vector);
				state.Grid[(int)vector2.x, (int)vector2.y] = vector;
			}
		}
		goto IL_01f4;
		IL_01f4:
		return result;
	}

	private Vector2 GenerateRandomAround(Vector2 center, float minimumDistance)
	{
		float num = myRandom.RandomValue();
		float num2 = minimumDistance + minimumDistance * num;
		num = myRandom.RandomValue();
		float num3 = 6.28318548f * num;
		float num4 = num2 * (float)Math.Sin((double)num3);
		float num5 = num2 * (float)Math.Cos((double)num3);
		return new Vector2(center.x + num4, center.y + num5);
	}

	private static Vector2 Denormalize(Vector2 point, Vector2 origin, double cellSize)
	{
		return new Vector2((float)(int)((double)(point.x - origin.x) / cellSize), (float)(int)((double)(point.y - origin.y) / cellSize));
	}
}
