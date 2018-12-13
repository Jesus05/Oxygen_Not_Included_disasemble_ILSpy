using System;

namespace MIConvexHull
{
	internal class MathHelper
	{
		private readonly int Dimension;

		private readonly int[] matrixPivots;

		private readonly double[] nDMatrix;

		private readonly double[] nDNormalHelperVector;

		private readonly double[] ntX;

		private readonly double[] ntY;

		private readonly double[] ntZ;

		private readonly double[] PositionData;

		internal MathHelper(int dimension, double[] positions)
		{
			PositionData = positions;
			Dimension = dimension;
			ntX = new double[Dimension];
			ntY = new double[Dimension];
			ntZ = new double[Dimension];
			nDNormalHelperVector = new double[Dimension];
			nDMatrix = new double[Dimension * Dimension];
			matrixPivots = new int[Dimension];
		}

		internal bool CalculateFacePlane(ConvexFaceInternal face, double[] center)
		{
			int[] vertices = face.Vertices;
			double[] normal = face.Normal;
			FindNormalVector(vertices, normal);
			if (double.IsNaN(normal[0]))
			{
				return false;
			}
			double num = 0.0;
			double num2 = 0.0;
			int num3 = vertices[0] * Dimension;
			for (int i = 0; i < Dimension; i++)
			{
				double num4 = normal[i];
				num += num4 * PositionData[num3 + i];
				num2 += num4 * center[i];
			}
			face.Offset = 0.0 - num;
			num2 -= num;
			if (num2 > 0.0)
			{
				for (int j = 0; j < Dimension; j++)
				{
					normal[j] = 0.0 - normal[j];
				}
				face.Offset = num;
				face.IsNormalFlipped = true;
			}
			else
			{
				face.IsNormalFlipped = false;
			}
			return true;
		}

		internal double GetVertexDistance(int v, ConvexFaceInternal f)
		{
			double[] normal = f.Normal;
			int num = v * Dimension;
			double num2 = f.Offset;
			for (int i = 0; i < normal.Length; i++)
			{
				num2 += normal[i] * PositionData[num + i];
			}
			return num2;
		}

		internal double[] VectorBetweenVertices(int toIndex, int fromIndex)
		{
			double[] array = new double[Dimension];
			VectorBetweenVertices(toIndex, fromIndex, array);
			return array;
		}

		private void VectorBetweenVertices(int toIndex, int fromIndex, double[] target)
		{
			int num = toIndex * Dimension;
			int num2 = fromIndex * Dimension;
			for (int i = 0; i < Dimension; i++)
			{
				target[i] = PositionData[num + i] - PositionData[num2 + i];
			}
		}

		internal void RandomOffsetToLift(int index)
		{
			Random random = new Random();
			int num = index * Dimension + Dimension - 1;
			PositionData[num] += PositionData[num] * random.NextDouble();
		}

		private void FindNormalVector(int[] vertices, double[] normalData)
		{
			switch (Dimension)
			{
			case 2:
				FindNormalVector2D(vertices, normalData);
				break;
			case 3:
				FindNormalVector3D(vertices, normalData);
				break;
			case 4:
				FindNormalVector4D(vertices, normalData);
				break;
			default:
				FindNormalVectorND(vertices, normalData);
				break;
			}
		}

		private void FindNormalVector2D(int[] vertices, double[] normal)
		{
			VectorBetweenVertices(vertices[1], vertices[0], ntX);
			double num = 0.0 - ntX[1];
			double num2 = ntX[0];
			double num3 = Math.Sqrt(num * num + num2 * num2);
			double num4 = 1.0 / num3;
			normal[0] = num4 * num;
			normal[1] = num4 * num2;
		}

		private void FindNormalVector3D(int[] vertices, double[] normal)
		{
			VectorBetweenVertices(vertices[1], vertices[0], ntX);
			VectorBetweenVertices(vertices[2], vertices[1], ntY);
			double num = ntX[1] * ntY[2] - ntX[2] * ntY[1];
			double num2 = ntX[2] * ntY[0] - ntX[0] * ntY[2];
			double num3 = ntX[0] * ntY[1] - ntX[1] * ntY[0];
			double num4 = Math.Sqrt(num * num + num2 * num2 + num3 * num3);
			double num5 = 1.0 / num4;
			normal[0] = num5 * num;
			normal[1] = num5 * num2;
			normal[2] = num5 * num3;
		}

		private void FindNormalVector4D(int[] vertices, double[] normal)
		{
			VectorBetweenVertices(vertices[1], vertices[0], ntX);
			VectorBetweenVertices(vertices[2], vertices[1], ntY);
			VectorBetweenVertices(vertices[3], vertices[2], ntZ);
			double[] array = ntX;
			double[] array2 = ntY;
			double[] array3 = ntZ;
			double num = array[3] * (array2[2] * array3[1] - array2[1] * array3[2]) + array[2] * (array2[1] * array3[3] - array2[3] * array3[1]) + array[1] * (array2[3] * array3[2] - array2[2] * array3[3]);
			double num2 = array[3] * (array2[0] * array3[2] - array2[2] * array3[0]) + array[2] * (array2[3] * array3[0] - array2[0] * array3[3]) + array[0] * (array2[2] * array3[3] - array2[3] * array3[2]);
			double num3 = array[3] * (array2[1] * array3[0] - array2[0] * array3[1]) + array[1] * (array2[0] * array3[3] - array2[3] * array3[0]) + array[0] * (array2[3] * array3[1] - array2[1] * array3[3]);
			double num4 = array[2] * (array2[0] * array3[1] - array2[1] * array3[0]) + array[1] * (array2[2] * array3[0] - array2[0] * array3[2]) + array[0] * (array2[1] * array3[2] - array2[2] * array3[1]);
			double num5 = Math.Sqrt(num * num + num2 * num2 + num3 * num3 + num4 * num4);
			double num6 = 1.0 / num5;
			normal[0] = num6 * num;
			normal[1] = num6 * num2;
			normal[2] = num6 * num3;
			normal[3] = num6 * num4;
		}

		private void FindNormalVectorND(int[] vertices, double[] normal)
		{
			int[] array = matrixPivots;
			double[] array2 = nDMatrix;
			double num = 0.0;
			for (int i = 0; i < Dimension; i++)
			{
				for (int j = 0; j < Dimension; j++)
				{
					int num2 = vertices[j] * Dimension;
					for (int k = 0; k < Dimension; k++)
					{
						array2[Dimension * j + k] = ((k != i) ? PositionData[num2 + k] : 1.0);
					}
				}
				LUFactor(array2, Dimension, array, nDNormalHelperVector);
				double num3 = 1.0;
				for (int l = 0; l < Dimension; l++)
				{
					num3 = ((array[l] == l) ? (num3 * array2[Dimension * l + l]) : (num3 * (0.0 - array2[Dimension * l + l])));
				}
				normal[i] = num3;
				num += num3 * num3;
			}
			double num4 = 1.0 / Math.Sqrt(num);
			for (int m = 0; m < normal.Length; m++)
			{
				normal[m] *= num4;
			}
		}

		internal double GetSimplexVolume(double[][] edgeVectors, int lastIndex, double bigNumber)
		{
			double[] array = new double[Dimension * Dimension];
			int num = 0;
			for (int i = 0; i < Dimension; i++)
			{
				for (int j = 0; j < Dimension; j++)
				{
					if (i <= lastIndex)
					{
						array[num++] = edgeVectors[i][j];
					}
					else
					{
						array[num] = Math.Pow(-1.0, (double)num) * (double)num++ / bigNumber;
					}
				}
			}
			return Math.Abs(DeterminantDestructive(array));
		}

		private double DeterminantDestructive(double[] A)
		{
			switch (Dimension)
			{
			case 0:
				return 0.0;
			case 1:
				return A[0];
			case 2:
				return A[0] * A[3] - A[1] * A[2];
			case 3:
				return A[0] * A[4] * A[8] + A[1] * A[5] * A[6] + A[2] * A[3] * A[7] - A[0] * A[5] * A[7] - A[1] * A[3] * A[8] - A[2] * A[4] * A[6];
			default:
			{
				int[] array = new int[Dimension];
				double[] vecLUcolj = new double[Dimension];
				LUFactor(A, Dimension, array, vecLUcolj);
				double num = 1.0;
				for (int i = 0; i < array.Length; i++)
				{
					num *= A[Dimension * i + i];
					if (array[i] != i)
					{
						num *= -1.0;
					}
				}
				return num;
			}
			}
		}

		private static void LUFactor(double[] data, int order, int[] ipiv, double[] vecLUcolj)
		{
			for (int i = 0; i < order; i++)
			{
				ipiv[i] = i;
			}
			for (int j = 0; j < order; j++)
			{
				int num = j * order;
				int num2 = num + j;
				for (int k = 0; k < order; k++)
				{
					vecLUcolj[k] = data[num + k];
				}
				for (int l = 0; l < order; l++)
				{
					int num3 = Math.Min(l, j);
					double num4 = 0.0;
					for (int m = 0; m < num3; m++)
					{
						num4 += data[m * order + l] * vecLUcolj[m];
					}
					data[num + l] = (vecLUcolj[l] -= num4);
				}
				int num5 = j;
				for (int n = j + 1; n < order; n++)
				{
					if (Math.Abs(vecLUcolj[n]) > Math.Abs(vecLUcolj[num5]))
					{
						num5 = n;
					}
				}
				if (num5 != j)
				{
					for (int num6 = 0; num6 < order; num6++)
					{
						int num7 = num6 * order;
						int num8 = num7 + num5;
						int num9 = num7 + j;
						double num10 = data[num8];
						data[num8] = data[num9];
						data[num9] = num10;
					}
					ipiv[j] = num5;
				}
				if ((j < order) & (data[num2] != 0.0))
				{
					for (int num11 = j + 1; num11 < order; num11++)
					{
						data[num + num11] /= data[num2];
					}
				}
			}
		}
	}
}
