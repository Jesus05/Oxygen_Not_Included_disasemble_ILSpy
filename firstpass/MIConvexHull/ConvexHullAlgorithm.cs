using System;
using System.Collections.Generic;
using System.Linq;

namespace MIConvexHull
{
	internal class ConvexHullAlgorithm
	{
		internal readonly int NumOfDimensions;

		private readonly bool IsLifted;

		private readonly double PlaneDistanceTolerance;

		private readonly IVertex[] Vertices;

		private double[] Positions;

		private readonly bool[] VertexVisited;

		private readonly int NumberOfVertices;

		internal ConvexFaceInternal[] FacePool;

		internal bool[] AffectedFaceFlags;

		private int ConvexHullSize;

		private readonly FaceList UnprocessedFaces;

		private readonly IndexBuffer ConvexFaces;

		private int CurrentVertex;

		private double MaxDistance;

		private int FurthestVertex;

		private readonly double[] Center;

		private readonly int[] UpdateBuffer;

		private readonly int[] UpdateIndices;

		private readonly IndexBuffer TraverseStack;

		private readonly IndexBuffer EmptyBuffer;

		private IndexBuffer BeyondBuffer;

		private readonly IndexBuffer AffectedFaceBuffer;

		private readonly SimpleList<DeferredFace> ConeFaceBuffer;

		private readonly HashSet<int> SingularVertices;

		private readonly ConnectorList[] ConnectorTable;

		private readonly ObjectManager ObjectManager;

		private readonly MathHelper mathHelper;

		private readonly List<int>[] boundingBoxPoints;

		private int indexOfDimensionWithLeastExtremes;

		private readonly double[] minima;

		private readonly double[] maxima;

		private ConvexHullAlgorithm(IVertex[] vertices, bool lift, double PlaneDistanceTolerance)
		{
			IsLifted = lift;
			Vertices = vertices;
			NumberOfVertices = vertices.Length;
			NumOfDimensions = DetermineDimension();
			if (IsLifted)
			{
				NumOfDimensions++;
			}
			if (NumOfDimensions < 2)
			{
				throw new InvalidOperationException("Dimension of the input must be 2 or greater.");
			}
			if (NumberOfVertices <= NumOfDimensions)
			{
				throw new ArgumentException("There are too few vertices (m) for the n-dimensional space. (m must be greater than the n, but m is " + NumberOfVertices + " and n is " + NumOfDimensions);
			}
			this.PlaneDistanceTolerance = PlaneDistanceTolerance;
			UnprocessedFaces = new FaceList();
			ConvexFaces = new IndexBuffer();
			FacePool = new ConvexFaceInternal[(NumOfDimensions + 1) * 10];
			AffectedFaceFlags = new bool[(NumOfDimensions + 1) * 10];
			ObjectManager = new ObjectManager(this);
			Center = new double[NumOfDimensions];
			TraverseStack = new IndexBuffer();
			UpdateBuffer = new int[NumOfDimensions];
			UpdateIndices = new int[NumOfDimensions];
			EmptyBuffer = new IndexBuffer();
			AffectedFaceBuffer = new IndexBuffer();
			ConeFaceBuffer = new SimpleList<DeferredFace>();
			SingularVertices = new HashSet<int>();
			BeyondBuffer = new IndexBuffer();
			ConnectorTable = new ConnectorList[2017];
			for (int i = 0; i < 2017; i++)
			{
				ConnectorTable[i] = new ConnectorList();
			}
			VertexVisited = new bool[NumberOfVertices];
			Positions = new double[NumberOfVertices * NumOfDimensions];
			boundingBoxPoints = new List<int>[NumOfDimensions];
			minima = new double[NumOfDimensions];
			maxima = new double[NumOfDimensions];
			mathHelper = new MathHelper(NumOfDimensions, Positions);
		}

		internal static ConvexHull<TVertex, TFace> GetConvexHull<TVertex, TFace>(IList<TVertex> data, double PlaneDistanceTolerance) where TVertex : IVertex where TFace : ConvexFace<TVertex, TFace>, new()
		{
			ConvexHullAlgorithm convexHullAlgorithm = new ConvexHullAlgorithm(data.Cast<IVertex>().ToArray(), false, PlaneDistanceTolerance);
			convexHullAlgorithm.GetConvexHull();
			if (convexHullAlgorithm.NumOfDimensions != 2)
			{
				ConvexHull<TVertex, TFace> convexHull = new ConvexHull<TVertex, TFace>();
				convexHull.Points = convexHullAlgorithm.GetHullVertices(data);
				convexHull.Faces = convexHullAlgorithm.GetConvexFaces<TVertex, TFace>();
				return convexHull;
			}
			return convexHullAlgorithm.Return2DResultInOrder<TVertex, TFace>(data);
		}

		private int DetermineDimension()
		{
			Random random = new Random();
			List<int> list = new List<int>();
			for (int i = 0; i < 10; i++)
			{
				list.Add(Vertices[random.Next(NumberOfVertices)].Position.Length);
			}
			int num = list.Min();
			if (num != list.Max())
			{
				throw new ArgumentException("Invalid input data (non-uniform dimension).");
			}
			return num;
		}

		private void GetConvexHull()
		{
			SerializeVerticesToPositions();
			FindBoundingBoxPoints();
			ShiftAndScalePositions();
			CreateInitialSimplex();
			while (UnprocessedFaces.First != null)
			{
				ConvexFaceInternal first = UnprocessedFaces.First;
				CurrentVertex = first.FurthestVertex;
				UpdateCenter();
				TagAffectedFaces(first);
				if (!SingularVertices.Contains(CurrentVertex) && CreateCone())
				{
					CommitCone();
				}
				else
				{
					HandleSingular();
				}
				int count = AffectedFaceBuffer.Count;
				for (int i = 0; i < count; i++)
				{
					AffectedFaceFlags[AffectedFaceBuffer[i]] = false;
				}
			}
		}

		private void SerializeVerticesToPositions()
		{
			int num = 0;
			if (IsLifted)
			{
				IVertex[] vertices = Vertices;
				foreach (IVertex vertex in vertices)
				{
					double num2 = 0.0;
					int num3 = NumOfDimensions - 1;
					for (int j = 0; j < num3; j++)
					{
						double num4 = vertex.Position[j];
						Positions[num++] = num4;
						num2 += num4 * num4;
					}
					Positions[num++] = num2;
				}
			}
			else
			{
				IVertex[] vertices2 = Vertices;
				foreach (IVertex vertex2 in vertices2)
				{
					for (int l = 0; l < NumOfDimensions; l++)
					{
						Positions[num++] = vertex2.Position[l];
					}
				}
			}
		}

		private void FindBoundingBoxPoints()
		{
			indexOfDimensionWithLeastExtremes = -1;
			int num = 2147483647;
			int i;
			for (i = 0; i < NumOfDimensions; i++)
			{
				List<int> list = new List<int>();
				List<int> list2 = new List<int>();
				double min = double.PositiveInfinity;
				double num2 = double.NegativeInfinity;
				for (int j = 0; j < NumberOfVertices; j++)
				{
					double coordinate = GetCoordinate(j, i);
					double num3 = min - coordinate;
					if (num3 >= PlaneDistanceTolerance)
					{
						min = coordinate;
						list.Clear();
						list.Add(j);
					}
					else if (num3 > 0.0)
					{
						min = coordinate;
						list.RemoveAll((int index) => min - GetCoordinate(index, i) > PlaneDistanceTolerance);
						list.Add(j);
					}
					else if (num3 > 0.0 - PlaneDistanceTolerance)
					{
						list.Add(j);
					}
					num3 = coordinate - num2;
					if (num3 >= PlaneDistanceTolerance)
					{
						num2 = coordinate;
						list2.Clear();
						list2.Add(j);
					}
					else if (num3 > 0.0)
					{
						num2 = coordinate;
						list2.RemoveAll((int index) => min - GetCoordinate(index, i) > PlaneDistanceTolerance);
						list2.Add(j);
					}
					else if (num3 > 0.0 - PlaneDistanceTolerance)
					{
						list2.Add(j);
					}
				}
				minima[i] = min;
				maxima[i] = num2;
				list.AddRange(list2);
				if (list.Count < num)
				{
					num = list.Count;
					indexOfDimensionWithLeastExtremes = i;
				}
				boundingBoxPoints[i] = list;
			}
		}

		private void ShiftAndScalePositions()
		{
			int num = Positions.Length;
			if (IsLifted)
			{
				int num2 = NumOfDimensions - 1;
				double num3 = 2.0 / (minima.Sum((double x) => Math.Abs(x)) + maxima.Sum((double x) => Math.Abs(x)) - Math.Abs(maxima[num2]) - Math.Abs(minima[num2]));
				minima[num2] *= num3;
				maxima[num2] *= num3;
				for (int i = num2; i < num; i += NumOfDimensions)
				{
					Positions[i] *= num3;
				}
			}
			double[] array = new double[NumOfDimensions];
			for (int j = 0; j < NumOfDimensions; j++)
			{
				if (maxima[j] == minima[j])
				{
					array[j] = 0.0;
				}
				else
				{
					array[j] = maxima[j] - minima[j] - minima[j];
				}
			}
			for (int k = 0; k < num; k++)
			{
				Positions[k] += array[k % NumOfDimensions];
			}
		}

		private void CreateInitialSimplex()
		{
			List<int> list = FindInitialPoints();
			int[] array = new int[NumOfDimensions + 1];
			for (int i = 0; i < NumOfDimensions + 1; i++)
			{
				int[] array2 = new int[NumOfDimensions];
				int j = 0;
				int num = 0;
				for (; j <= NumOfDimensions; j++)
				{
					if (i != j)
					{
						if (j == list.Count)
						{
							int num2 = 0;
							num2++;
						}
						int num3 = list[j];
						array2[num++] = num3;
					}
				}
				ConvexFaceInternal convexFaceInternal = FacePool[ObjectManager.GetFace()];
				convexFaceInternal.Vertices = array2;
				Array.Sort(array2);
				mathHelper.CalculateFacePlane(convexFaceInternal, Center);
				array[i] = convexFaceInternal.Index;
			}
			for (int k = 0; k < NumOfDimensions; k++)
			{
				for (int l = k + 1; l < NumOfDimensions + 1; l++)
				{
					UpdateAdjacency(FacePool[array[k]], FacePool[array[l]]);
				}
			}
			int[] array3 = array;
			foreach (int num5 in array3)
			{
				ConvexFaceInternal convexFaceInternal2 = FacePool[num5];
				FindBeyondVertices(convexFaceInternal2);
				if (convexFaceInternal2.VerticesBeyond.Count == 0)
				{
					ConvexFaces.Add(convexFaceInternal2.Index);
				}
				else
				{
					UnprocessedFaces.Add(convexFaceInternal2);
				}
			}
			foreach (int item in list)
			{
				VertexVisited[item] = false;
			}
		}

		private List<int> FindInitialPoints()
		{
			double bigNumber = maxima.Sum() * (double)NumOfDimensions * (double)NumberOfVertices;
			int num = boundingBoxPoints[indexOfDimensionWithLeastExtremes].First();
			int num2 = boundingBoxPoints[indexOfDimensionWithLeastExtremes].Last();
			boundingBoxPoints[indexOfDimensionWithLeastExtremes].RemoveAt(0);
			boundingBoxPoints[indexOfDimensionWithLeastExtremes].RemoveAt(boundingBoxPoints[indexOfDimensionWithLeastExtremes].Count - 1);
			List<int> list = new List<int>();
			list.Add(num);
			list.Add(num2);
			List<int> list2 = list;
			VertexVisited[num] = (VertexVisited[num2] = true);
			CurrentVertex = num;
			UpdateCenter();
			CurrentVertex = num2;
			UpdateCenter();
			double[][] array = new double[NumOfDimensions][];
			array[0] = mathHelper.VectorBetweenVertices(num2, num);
			List<int> list3 = boundingBoxPoints.SelectMany((List<int> x) => x).ToList();
			int num3 = 1;
			while (num3 < NumOfDimensions)
			{
				if (!list3.Any())
				{
					break;
				}
				int num4 = -1;
				double[] array2 = new double[0];
				double num5 = 0.0;
				for (int num6 = list3.Count - 1; num6 >= 0; num6--)
				{
					int num7 = list3[num6];
					if (list2.Contains(num7))
					{
						list3.RemoveAt(num6);
					}
					else
					{
						array[num3] = mathHelper.VectorBetweenVertices(num7, num);
						double simplexVolume = mathHelper.GetSimplexVolume(array, num3, bigNumber);
						if (num5 < simplexVolume)
						{
							num5 = simplexVolume;
							num4 = num7;
							array2 = array[num3];
						}
					}
				}
				list3.Remove(num4);
				if (num4 == -1)
				{
					break;
				}
				list2.Add(num4);
				array[num3++] = array2;
				CurrentVertex = num4;
				UpdateCenter();
			}
			if (list2.Count <= NumOfDimensions)
			{
				List<int> list4 = Enumerable.Range(0, NumberOfVertices).ToList();
				while (num3 < NumOfDimensions)
				{
					if (!list4.Any())
					{
						break;
					}
					int num9 = -1;
					double[] array3 = new double[0];
					double num10 = 0.0;
					for (int num11 = list4.Count - 1; num11 >= 0; num11--)
					{
						int num12 = list4[num11];
						if (list2.Contains(num12))
						{
							list4.RemoveAt(num11);
						}
						else
						{
							array[num3] = mathHelper.VectorBetweenVertices(num12, num);
							double simplexVolume2 = mathHelper.GetSimplexVolume(array, num3, bigNumber);
							if (num10 < simplexVolume2)
							{
								num10 = simplexVolume2;
								num9 = num12;
								array3 = array[num3];
							}
						}
					}
					list4.Remove(num9);
					if (num9 == -1)
					{
						break;
					}
					list2.Add(num9);
					array[num3++] = array3;
					CurrentVertex = num9;
					UpdateCenter();
				}
			}
			if (list2.Count <= NumOfDimensions && IsLifted)
			{
				List<int> list5 = Enumerable.Range(0, NumberOfVertices).ToList();
				while (num3 < NumOfDimensions)
				{
					if (!list5.Any())
					{
						break;
					}
					int num14 = -1;
					double[] array4 = new double[0];
					double num15 = 0.0;
					for (int num16 = list5.Count - 1; num16 >= 0; num16--)
					{
						int num17 = list5[num16];
						if (list2.Contains(num17))
						{
							list5.RemoveAt(num16);
						}
						else
						{
							mathHelper.RandomOffsetToLift(num17);
							array[num3] = mathHelper.VectorBetweenVertices(num17, num);
							double simplexVolume3 = mathHelper.GetSimplexVolume(array, num3, bigNumber);
							if (num15 < simplexVolume3)
							{
								num15 = simplexVolume3;
								num14 = num17;
								array4 = array[num3];
							}
						}
					}
					list5.Remove(num14);
					if (num14 == -1)
					{
						break;
					}
					list2.Add(num14);
					array[num3++] = array4;
					CurrentVertex = num14;
					UpdateCenter();
				}
			}
			if (list2.Count <= NumOfDimensions && IsLifted)
			{
				throw new ArgumentException("The input data is degenerate. It appears to exist in " + NumOfDimensions + " dimensions, but it is a " + (NumOfDimensions - 1) + " dimensional set (i.e. the point of collinear, coplanar, or co-hyperplanar.)");
			}
			return list2;
		}

		private void UpdateAdjacency(ConvexFaceInternal l, ConvexFaceInternal r)
		{
			int[] vertices = l.Vertices;
			int[] vertices2 = r.Vertices;
			int i;
			for (i = 0; i < vertices.Length; i++)
			{
				VertexVisited[vertices[i]] = false;
			}
			for (i = 0; i < vertices2.Length; i++)
			{
				VertexVisited[vertices2[i]] = true;
			}
			for (i = 0; i < vertices.Length && VertexVisited[vertices[i]]; i++)
			{
			}
			if (i != NumOfDimensions)
			{
				for (int j = i + 1; j < vertices.Length; j++)
				{
					if (!VertexVisited[vertices[j]])
					{
						return;
					}
				}
				l.AdjacentFaces[i] = r.Index;
				for (i = 0; i < vertices.Length; i++)
				{
					VertexVisited[vertices[i]] = false;
				}
				for (i = 0; i < vertices2.Length && !VertexVisited[vertices2[i]]; i++)
				{
				}
				r.AdjacentFaces[i] = l.Index;
			}
		}

		private void FindBeyondVertices(ConvexFaceInternal face)
		{
			IndexBuffer verticesBeyond = face.VerticesBeyond;
			MaxDistance = double.NegativeInfinity;
			FurthestVertex = 0;
			for (int i = 0; i < NumberOfVertices; i++)
			{
				if (!VertexVisited[i])
				{
					IsBeyond(face, verticesBeyond, i);
				}
			}
			face.FurthestVertex = FurthestVertex;
		}

		private void TagAffectedFaces(ConvexFaceInternal currentFace)
		{
			AffectedFaceBuffer.Clear();
			AffectedFaceBuffer.Add(currentFace.Index);
			TraverseAffectedFaces(currentFace.Index);
		}

		private void TraverseAffectedFaces(int currentFace)
		{
			TraverseStack.Clear();
			TraverseStack.Push(currentFace);
			AffectedFaceFlags[currentFace] = true;
			while (TraverseStack.Count > 0)
			{
				ConvexFaceInternal convexFaceInternal = FacePool[TraverseStack.Pop()];
				for (int i = 0; i < NumOfDimensions; i++)
				{
					int num = convexFaceInternal.AdjacentFaces[i];
					if (!AffectedFaceFlags[num] && mathHelper.GetVertexDistance(CurrentVertex, FacePool[num]) >= PlaneDistanceTolerance)
					{
						AffectedFaceBuffer.Add(num);
						AffectedFaceFlags[num] = true;
						TraverseStack.Push(num);
					}
				}
			}
		}

		private DeferredFace MakeDeferredFace(ConvexFaceInternal face, int faceIndex, ConvexFaceInternal pivot, int pivotIndex, ConvexFaceInternal oldFace)
		{
			DeferredFace deferredFace = ObjectManager.GetDeferredFace();
			deferredFace.Face = face;
			deferredFace.FaceIndex = faceIndex;
			deferredFace.Pivot = pivot;
			deferredFace.PivotIndex = pivotIndex;
			deferredFace.OldFace = oldFace;
			return deferredFace;
		}

		private void ConnectFace(FaceConnector connector)
		{
			uint num = connector.HashCode % 2017u;
			ConnectorList connectorList = ConnectorTable[num];
			for (FaceConnector faceConnector = connectorList.First; faceConnector != null; faceConnector = faceConnector.Next)
			{
				if (FaceConnector.AreConnectable(connector, faceConnector, NumOfDimensions))
				{
					connectorList.Remove(faceConnector);
					FaceConnector.Connect(faceConnector, connector);
					faceConnector.Face = null;
					connector.Face = null;
					ObjectManager.DepositConnector(faceConnector);
					ObjectManager.DepositConnector(connector);
					return;
				}
			}
			connectorList.Add(connector);
		}

		private bool CreateCone()
		{
			int currentVertex = CurrentVertex;
			ConeFaceBuffer.Clear();
			for (int i = 0; i < AffectedFaceBuffer.Count; i++)
			{
				int num = AffectedFaceBuffer[i];
				ConvexFaceInternal convexFaceInternal = FacePool[num];
				int num2 = 0;
				for (int j = 0; j < NumOfDimensions; j++)
				{
					int num3 = convexFaceInternal.AdjacentFaces[j];
					if (!AffectedFaceFlags[num3])
					{
						UpdateBuffer[num2] = num3;
						UpdateIndices[num2] = j;
						num2++;
					}
				}
				for (int k = 0; k < num2; k++)
				{
					ConvexFaceInternal convexFaceInternal2 = FacePool[UpdateBuffer[k]];
					int pivotIndex = 0;
					int[] adjacentFaces = convexFaceInternal2.AdjacentFaces;
					for (int l = 0; l < adjacentFaces.Length; l++)
					{
						if (num == adjacentFaces[l])
						{
							pivotIndex = l;
							break;
						}
					}
					int num4 = UpdateIndices[k];
					int face = ObjectManager.GetFace();
					ConvexFaceInternal convexFaceInternal3 = FacePool[face];
					int[] vertices = convexFaceInternal3.Vertices;
					for (int m = 0; m < NumOfDimensions; m++)
					{
						vertices[m] = convexFaceInternal.Vertices[m];
					}
					int num5 = vertices[num4];
					int num6;
					if (currentVertex < num5)
					{
						num6 = 0;
						for (int num7 = num4 - 1; num7 >= 0; num7--)
						{
							if (vertices[num7] <= currentVertex)
							{
								num6 = num7 + 1;
								break;
							}
							vertices[num7 + 1] = vertices[num7];
						}
					}
					else
					{
						num6 = NumOfDimensions - 1;
						for (int n = num4 + 1; n < NumOfDimensions; n++)
						{
							if (vertices[n] >= currentVertex)
							{
								num6 = n - 1;
								break;
							}
							vertices[n - 1] = vertices[n];
						}
					}
					vertices[num6] = CurrentVertex;
					if (!mathHelper.CalculateFacePlane(convexFaceInternal3, Center))
					{
						return false;
					}
					ConeFaceBuffer.Add(MakeDeferredFace(convexFaceInternal3, num6, convexFaceInternal2, pivotIndex, convexFaceInternal));
				}
			}
			return true;
		}

		private void CommitCone()
		{
			for (int i = 0; i < ConeFaceBuffer.Count; i++)
			{
				DeferredFace deferredFace = ConeFaceBuffer[i];
				ConvexFaceInternal face = deferredFace.Face;
				ConvexFaceInternal pivot = deferredFace.Pivot;
				ConvexFaceInternal oldFace = deferredFace.OldFace;
				int faceIndex = deferredFace.FaceIndex;
				face.AdjacentFaces[faceIndex] = pivot.Index;
				pivot.AdjacentFaces[deferredFace.PivotIndex] = face.Index;
				for (int j = 0; j < NumOfDimensions; j++)
				{
					if (j != faceIndex)
					{
						FaceConnector connector = ObjectManager.GetConnector();
						connector.Update(face, j, NumOfDimensions);
						ConnectFace(connector);
					}
				}
				if (pivot.VerticesBeyond.Count == 0)
				{
					FindBeyondVertices(face, oldFace.VerticesBeyond);
				}
				else if (pivot.VerticesBeyond.Count < oldFace.VerticesBeyond.Count)
				{
					FindBeyondVertices(face, pivot.VerticesBeyond, oldFace.VerticesBeyond);
				}
				else
				{
					FindBeyondVertices(face, oldFace.VerticesBeyond, pivot.VerticesBeyond);
				}
				if (face.VerticesBeyond.Count == 0)
				{
					ConvexFaces.Add(face.Index);
					UnprocessedFaces.Remove(face);
					ObjectManager.DepositVertexBuffer(face.VerticesBeyond);
					face.VerticesBeyond = EmptyBuffer;
				}
				else
				{
					UnprocessedFaces.Add(face);
				}
				ObjectManager.DepositDeferredFace(deferredFace);
			}
			for (int k = 0; k < AffectedFaceBuffer.Count; k++)
			{
				int num = AffectedFaceBuffer[k];
				UnprocessedFaces.Remove(FacePool[num]);
				ObjectManager.DepositFace(num);
			}
		}

		private void IsBeyond(ConvexFaceInternal face, IndexBuffer beyondVertices, int v)
		{
			double vertexDistance = mathHelper.GetVertexDistance(v, face);
			if (vertexDistance >= PlaneDistanceTolerance)
			{
				if (vertexDistance > MaxDistance)
				{
					if (vertexDistance - MaxDistance < PlaneDistanceTolerance)
					{
						if (LexCompare(v, FurthestVertex) > 0)
						{
							MaxDistance = vertexDistance;
							FurthestVertex = v;
						}
					}
					else
					{
						MaxDistance = vertexDistance;
						FurthestVertex = v;
					}
				}
				beyondVertices.Add(v);
			}
		}

		private int LexCompare(int u, int v)
		{
			int num = u * NumOfDimensions;
			int num2 = v * NumOfDimensions;
			for (int i = 0; i < NumOfDimensions; i++)
			{
				double num3 = Positions[num + i];
				double value = Positions[num2 + i];
				int num4 = num3.CompareTo(value);
				if (num4 != 0)
				{
					return num4;
				}
			}
			return 0;
		}

		private void FindBeyondVertices(ConvexFaceInternal face, IndexBuffer beyond, IndexBuffer beyond1)
		{
			IndexBuffer beyondBuffer = BeyondBuffer;
			MaxDistance = double.NegativeInfinity;
			FurthestVertex = 0;
			for (int i = 0; i < beyond1.Count; i++)
			{
				VertexVisited[beyond1[i]] = true;
			}
			VertexVisited[CurrentVertex] = false;
			for (int j = 0; j < beyond.Count; j++)
			{
				int num = beyond[j];
				if (num != CurrentVertex)
				{
					VertexVisited[num] = false;
					IsBeyond(face, beyondBuffer, num);
				}
			}
			for (int k = 0; k < beyond1.Count; k++)
			{
				int num = beyond1[k];
				if (VertexVisited[num])
				{
					IsBeyond(face, beyondBuffer, num);
				}
			}
			face.FurthestVertex = FurthestVertex;
			IndexBuffer verticesBeyond = face.VerticesBeyond;
			face.VerticesBeyond = beyondBuffer;
			if (verticesBeyond.Count > 0)
			{
				verticesBeyond.Clear();
			}
			BeyondBuffer = verticesBeyond;
		}

		private void FindBeyondVertices(ConvexFaceInternal face, IndexBuffer beyond)
		{
			IndexBuffer beyondBuffer = BeyondBuffer;
			MaxDistance = double.NegativeInfinity;
			FurthestVertex = 0;
			for (int i = 0; i < beyond.Count; i++)
			{
				int num = beyond[i];
				if (num != CurrentVertex)
				{
					IsBeyond(face, beyondBuffer, num);
				}
			}
			face.FurthestVertex = FurthestVertex;
			IndexBuffer verticesBeyond = face.VerticesBeyond;
			face.VerticesBeyond = beyondBuffer;
			if (verticesBeyond.Count > 0)
			{
				verticesBeyond.Clear();
			}
			BeyondBuffer = verticesBeyond;
		}

		private void UpdateCenter()
		{
			for (int i = 0; i < NumOfDimensions; i++)
			{
				Center[i] *= (double)ConvexHullSize;
			}
			ConvexHullSize++;
			double num = 1.0 / (double)ConvexHullSize;
			int num2 = CurrentVertex * NumOfDimensions;
			for (int j = 0; j < NumOfDimensions; j++)
			{
				Center[j] = num * (Center[j] + Positions[num2 + j]);
			}
		}

		private void RollbackCenter()
		{
			for (int i = 0; i < NumOfDimensions; i++)
			{
				Center[i] *= (double)ConvexHullSize;
			}
			ConvexHullSize--;
			double num = (ConvexHullSize <= 0) ? 0.0 : (1.0 / (double)ConvexHullSize);
			int num2 = CurrentVertex * NumOfDimensions;
			for (int j = 0; j < NumOfDimensions; j++)
			{
				Center[j] = num * (Center[j] - Positions[num2 + j]);
			}
		}

		private void HandleSingular()
		{
			RollbackCenter();
			SingularVertices.Add(CurrentVertex);
			for (int i = 0; i < AffectedFaceBuffer.Count; i++)
			{
				ConvexFaceInternal convexFaceInternal = FacePool[AffectedFaceBuffer[i]];
				IndexBuffer verticesBeyond = convexFaceInternal.VerticesBeyond;
				for (int j = 0; j < verticesBeyond.Count; j++)
				{
					SingularVertices.Add(verticesBeyond[j]);
				}
				ConvexFaces.Add(convexFaceInternal.Index);
				UnprocessedFaces.Remove(convexFaceInternal);
				ObjectManager.DepositVertexBuffer(convexFaceInternal.VerticesBeyond);
				convexFaceInternal.VerticesBeyond = EmptyBuffer;
			}
		}

		private double GetCoordinate(int vIndex, int dimension)
		{
			return Positions[vIndex * NumOfDimensions + dimension];
		}

		private TVertex[] GetHullVertices<TVertex>(IList<TVertex> data)
		{
			int count = ((SimpleList<int>)ConvexFaces).Count;
			int num = 0;
			for (int i = 0; i < NumberOfVertices; i++)
			{
				VertexVisited[i] = false;
			}
			for (int j = 0; j < count; j++)
			{
				int[] vertices = FacePool[((SimpleList<int>)ConvexFaces)[j]].Vertices;
				foreach (int num2 in vertices)
				{
					if (!VertexVisited[num2])
					{
						VertexVisited[num2] = true;
						num++;
					}
				}
			}
			TVertex[] array = new TVertex[num];
			for (int l = 0; l < NumberOfVertices; l++)
			{
				if (VertexVisited[l])
				{
					array[--num] = data[l];
				}
			}
			return array;
		}

		private TFace[] GetConvexFaces<TVertex, TFace>() where TVertex : IVertex where TFace : ConvexFace<TVertex, TFace>, new()
		{
			IndexBuffer convexFaces = ConvexFaces;
			int count = ((SimpleList<int>)convexFaces).Count;
			TFace[] array = new TFace[count];
			for (int i = 0; i < count; i++)
			{
				ConvexFaceInternal convexFaceInternal = FacePool[((SimpleList<int>)convexFaces)[i]];
				TVertex[] array2 = new TVertex[NumOfDimensions];
				for (int j = 0; j < NumOfDimensions; j++)
				{
					array2[j] = (TVertex)Vertices[convexFaceInternal.Vertices[j]];
				}
				TFace[] array3 = array;
				int num = i;
				TFace val = new TFace();
				((ConvexFace<TVertex, TFace>)val).Vertices = array2;
				((ConvexFace<TVertex, TFace>)val).Adjacency = new TFace[NumOfDimensions];
				((ConvexFace<TVertex, TFace>)val).Normal = ((!IsLifted) ? convexFaceInternal.Normal : null);
				array3[num] = val;
				convexFaceInternal.Tag = i;
			}
			for (int k = 0; k < count; k++)
			{
				ConvexFaceInternal convexFaceInternal2 = FacePool[((SimpleList<int>)convexFaces)[k]];
				TFace val2 = array[k];
				for (int l = 0; l < NumOfDimensions; l++)
				{
					if (convexFaceInternal2.AdjacentFaces[l] >= 0)
					{
						((ConvexFace<TVertex, TFace>)val2).Adjacency[l] = array[FacePool[convexFaceInternal2.AdjacentFaces[l]].Tag];
					}
				}
				if (convexFaceInternal2.IsNormalFlipped)
				{
					TVertex val3 = ((ConvexFace<TVertex, TFace>)val2).Vertices[0];
					((ConvexFace<TVertex, TFace>)val2).Vertices[0] = ((ConvexFace<TVertex, TFace>)val2).Vertices[NumOfDimensions - 1];
					((ConvexFace<TVertex, TFace>)val2).Vertices[NumOfDimensions - 1] = val3;
					TFace val4 = ((ConvexFace<TVertex, TFace>)val2).Adjacency[0];
					((ConvexFace<TVertex, TFace>)val2).Adjacency[0] = ((ConvexFace<TVertex, TFace>)val2).Adjacency[NumOfDimensions - 1];
					((ConvexFace<TVertex, TFace>)val2).Adjacency[NumOfDimensions - 1] = val4;
				}
			}
			return array;
		}

		private ConvexHull<TVertex, TFace> Return2DResultInOrder<TVertex, TFace>(IList<TVertex> data) where TVertex : IVertex where TFace : ConvexFace<TVertex, TFace>, new()
		{
			TFace[] convexFaces = GetConvexFaces<TVertex, TFace>();
			int num = convexFaces.Length;
			Dictionary<TVertex, TFace> dictionary = new Dictionary<TVertex, TFace>();
			TFace[] array = convexFaces;
			for (int i = 0; i < array.Length; i++)
			{
				TFace value = array[i];
				dictionary.Add(((ConvexFace<TVertex, TFace>)value).Vertices[1], value);
			}
			TVertex val = ((ConvexFace<TVertex, TFace>)convexFaces[0]).Vertices[1];
			TVertex val2 = ((ConvexFace<TVertex, TFace>)convexFaces[0]).Vertices[0];
			List<TVertex> list = new List<TVertex>();
			list.Add(val);
			List<TFace> list2 = new List<TFace>();
			list2.Add(convexFaces[1]);
			int num2 = 0;
			int num3 = 0;
			while (!val2.Equals(val))
			{
				list.Add(val2);
				TFace item = dictionary[val2];
				list2.Add(item);
				if (val2.Position[0] < list[num2].Position[0] || (val2.Position[0] == list[num2].Position[0] && val2.Position[1] <= list[num2].Position[1]))
				{
					num2 = num3;
				}
				num3++;
				val2 = ((ConvexFace<TVertex, TFace>)item).Vertices[0];
			}
			TVertex[] array2 = new TVertex[num];
			for (int j = 0; j < num; j++)
			{
				int index = (j + num2) % num;
				array2[j] = list[index];
				convexFaces[j] = list2[index];
			}
			ConvexHull<TVertex, TFace> convexHull = new ConvexHull<TVertex, TFace>();
			convexHull.Points = array2;
			convexHull.Faces = convexFaces;
			return convexHull;
		}

		internal static TCell[] GetDelaunayTriangulation<TVertex, TCell>(IList<TVertex> data) where TVertex : IVertex where TCell : TriangulationCell<TVertex, TCell>, new()
		{
			ConvexHullAlgorithm convexHullAlgorithm = new ConvexHullAlgorithm(data.Cast<IVertex>().ToArray(), true, 1E-10);
			convexHullAlgorithm.GetConvexHull();
			convexHullAlgorithm.RemoveUpperFaces();
			return convexHullAlgorithm.GetConvexFaces<TVertex, TCell>();
		}

		private void RemoveUpperFaces()
		{
			IndexBuffer convexFaces = ConvexFaces;
			int num = NumOfDimensions - 1;
			for (int num2 = convexFaces.Count - 1; num2 >= 0; num2--)
			{
				int num3 = convexFaces[num2];
				ConvexFaceInternal convexFaceInternal = FacePool[num3];
				if (convexFaceInternal.Normal[num] >= 0.0)
				{
					for (int i = 0; i < convexFaceInternal.AdjacentFaces.Length; i++)
					{
						int num4 = convexFaceInternal.AdjacentFaces[i];
						if (num4 >= 0)
						{
							ConvexFaceInternal convexFaceInternal2 = FacePool[num4];
							for (int j = 0; j < convexFaceInternal2.AdjacentFaces.Length; j++)
							{
								if (convexFaceInternal2.AdjacentFaces[j] == num3)
								{
									convexFaceInternal2.AdjacentFaces[j] = -1;
								}
							}
						}
					}
					convexFaces[num2] = convexFaces[convexFaces.Count - 1];
					convexFaces.Pop();
				}
			}
		}
	}
}
