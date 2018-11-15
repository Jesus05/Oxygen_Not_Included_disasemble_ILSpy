using System;
using System.Collections.Generic;

namespace ClipperLib
{
	public class ClipperOffset
	{
		private List<List<IntPoint>> m_destPolys;

		private List<IntPoint> m_srcPoly;

		private List<IntPoint> m_destPoly;

		private List<DoublePoint> m_normals = new List<DoublePoint>();

		private double m_delta;

		private double m_sinA;

		private double m_sin;

		private double m_cos;

		private double m_miterLim;

		private double m_StepsPerRad;

		private IntPoint m_lowest;

		private PolyNode m_polyNodes = new PolyNode();

		private const double two_pi = 6.2831853071795862;

		private const double def_arc_tolerance = 0.25;

		public double ArcTolerance
		{
			get;
			set;
		}

		public double MiterLimit
		{
			get;
			set;
		}

		public ClipperOffset(double miterLimit = 2.0, double arcTolerance = 0.25)
		{
			MiterLimit = miterLimit;
			ArcTolerance = arcTolerance;
			m_lowest.X = -1L;
		}

		public void Clear()
		{
			m_polyNodes.Childs.Clear();
			m_lowest.X = -1L;
		}

		internal static long Round(double value)
		{
			return (!(value < 0.0)) ? ((long)(value + 0.5)) : ((long)(value - 0.5));
		}

		public void AddPath(List<IntPoint> path, JoinType joinType, EndType endType)
		{
			int num = path.Count - 1;
			if (num >= 0)
			{
				PolyNode polyNode = new PolyNode();
				polyNode.m_jointype = joinType;
				polyNode.m_endtype = endType;
				if (endType == EndType.etClosedLine || endType == EndType.etClosedPolygon)
				{
					while (num > 0 && path[0] == path[num])
					{
						num--;
					}
				}
				polyNode.m_polygon.Capacity = num + 1;
				polyNode.m_polygon.Add(path[0]);
				int num2 = 0;
				int num3 = 0;
				for (int i = 1; i <= num; i++)
				{
					if (polyNode.m_polygon[num2] != path[i])
					{
						num2++;
						polyNode.m_polygon.Add(path[i]);
						IntPoint intPoint = path[i];
						long y = intPoint.Y;
						IntPoint intPoint2 = polyNode.m_polygon[num3];
						if (y <= intPoint2.Y)
						{
							IntPoint intPoint3 = path[i];
							long y2 = intPoint3.Y;
							IntPoint intPoint4 = polyNode.m_polygon[num3];
							if (y2 != intPoint4.Y)
							{
								continue;
							}
							IntPoint intPoint5 = path[i];
							long x = intPoint5.X;
							IntPoint intPoint6 = polyNode.m_polygon[num3];
							if (x >= intPoint6.X)
							{
								continue;
							}
						}
						num3 = num2;
					}
				}
				if (endType != 0 || num2 >= 2)
				{
					m_polyNodes.AddChild(polyNode);
					if (endType == EndType.etClosedPolygon)
					{
						if (m_lowest.X < 0)
						{
							m_lowest = new IntPoint(m_polyNodes.ChildCount - 1, num3);
						}
						else
						{
							IntPoint intPoint7 = m_polyNodes.Childs[(int)m_lowest.X].m_polygon[(int)m_lowest.Y];
							IntPoint intPoint8 = polyNode.m_polygon[num3];
							if (intPoint8.Y <= intPoint7.Y)
							{
								IntPoint intPoint9 = polyNode.m_polygon[num3];
								if (intPoint9.Y != intPoint7.Y)
								{
									return;
								}
								IntPoint intPoint10 = polyNode.m_polygon[num3];
								if (intPoint10.X >= intPoint7.X)
								{
									return;
								}
							}
							m_lowest = new IntPoint(m_polyNodes.ChildCount - 1, num3);
						}
					}
				}
			}
		}

		public void AddPaths(List<List<IntPoint>> paths, JoinType joinType, EndType endType)
		{
			foreach (List<IntPoint> path in paths)
			{
				AddPath(path, joinType, endType);
			}
		}

		private void FixOrientations()
		{
			if (m_lowest.X >= 0 && !Clipper.Orientation(m_polyNodes.Childs[(int)m_lowest.X].m_polygon))
			{
				for (int i = 0; i < m_polyNodes.ChildCount; i++)
				{
					PolyNode polyNode = m_polyNodes.Childs[i];
					if (polyNode.m_endtype == EndType.etClosedPolygon || (polyNode.m_endtype == EndType.etClosedLine && Clipper.Orientation(polyNode.m_polygon)))
					{
						polyNode.m_polygon.Reverse();
					}
				}
			}
			else
			{
				for (int j = 0; j < m_polyNodes.ChildCount; j++)
				{
					PolyNode polyNode2 = m_polyNodes.Childs[j];
					if (polyNode2.m_endtype == EndType.etClosedLine && !Clipper.Orientation(polyNode2.m_polygon))
					{
						polyNode2.m_polygon.Reverse();
					}
				}
			}
		}

		internal static DoublePoint GetUnitNormal(IntPoint pt1, IntPoint pt2)
		{
			double num = (double)(pt2.X - pt1.X);
			double num2 = (double)(pt2.Y - pt1.Y);
			if (num == 0.0 && num2 == 0.0)
			{
				return default(DoublePoint);
			}
			double num3 = 1.0 / Math.Sqrt(num * num + num2 * num2);
			num *= num3;
			num2 *= num3;
			return new DoublePoint(num2, 0.0 - num);
		}

		private void DoOffset(double delta)
		{
			m_destPolys = new List<List<IntPoint>>();
			m_delta = delta;
			if (ClipperBase.near_zero(delta))
			{
				m_destPolys.Capacity = m_polyNodes.ChildCount;
				for (int i = 0; i < m_polyNodes.ChildCount; i++)
				{
					PolyNode polyNode = m_polyNodes.Childs[i];
					if (polyNode.m_endtype == EndType.etClosedPolygon)
					{
						m_destPolys.Add(polyNode.m_polygon);
					}
				}
			}
			else
			{
				if (MiterLimit > 2.0)
				{
					m_miterLim = 2.0 / (MiterLimit * MiterLimit);
				}
				else
				{
					m_miterLim = 0.5;
				}
				double num = (ArcTolerance <= 0.0) ? 0.25 : ((!(ArcTolerance > Math.Abs(delta) * 0.25)) ? ArcTolerance : (Math.Abs(delta) * 0.25));
				double num2 = 3.1415926535897931 / Math.Acos(1.0 - num / Math.Abs(delta));
				m_sin = Math.Sin(6.2831853071795862 / num2);
				m_cos = Math.Cos(6.2831853071795862 / num2);
				m_StepsPerRad = num2 / 6.2831853071795862;
				if (delta < 0.0)
				{
					m_sin = 0.0 - m_sin;
				}
				m_destPolys.Capacity = m_polyNodes.ChildCount * 2;
				for (int j = 0; j < m_polyNodes.ChildCount; j++)
				{
					PolyNode polyNode2 = m_polyNodes.Childs[j];
					m_srcPoly = polyNode2.m_polygon;
					int count = m_srcPoly.Count;
					if (count != 0 && (!(delta <= 0.0) || (count >= 3 && polyNode2.m_endtype == EndType.etClosedPolygon)))
					{
						m_destPoly = new List<IntPoint>();
						if (count == 1)
						{
							if (polyNode2.m_jointype == JoinType.jtRound)
							{
								double num3 = 1.0;
								double num4 = 0.0;
								for (int k = 1; (double)k <= num2; k++)
								{
									List<IntPoint> destPoly = m_destPoly;
									IntPoint intPoint = m_srcPoly[0];
									long x = Round((double)intPoint.X + num3 * delta);
									IntPoint intPoint2 = m_srcPoly[0];
									destPoly.Add(new IntPoint(x, Round((double)intPoint2.Y + num4 * delta)));
									double num5 = num3;
									num3 = num3 * m_cos - m_sin * num4;
									num4 = num5 * m_sin + num4 * m_cos;
								}
							}
							else
							{
								double num6 = -1.0;
								double num7 = -1.0;
								for (int l = 0; l < 4; l++)
								{
									List<IntPoint> destPoly2 = m_destPoly;
									IntPoint intPoint3 = m_srcPoly[0];
									long x2 = Round((double)intPoint3.X + num6 * delta);
									IntPoint intPoint4 = m_srcPoly[0];
									destPoly2.Add(new IntPoint(x2, Round((double)intPoint4.Y + num7 * delta)));
									if (num6 < 0.0)
									{
										num6 = 1.0;
									}
									else if (num7 < 0.0)
									{
										num7 = 1.0;
									}
									else
									{
										num6 = -1.0;
									}
								}
							}
							m_destPolys.Add(m_destPoly);
						}
						else
						{
							m_normals.Clear();
							m_normals.Capacity = count;
							for (int m = 0; m < count - 1; m++)
							{
								m_normals.Add(GetUnitNormal(m_srcPoly[m], m_srcPoly[m + 1]));
							}
							if (polyNode2.m_endtype == EndType.etClosedLine || polyNode2.m_endtype == EndType.etClosedPolygon)
							{
								m_normals.Add(GetUnitNormal(m_srcPoly[count - 1], m_srcPoly[0]));
							}
							else
							{
								m_normals.Add(new DoublePoint(m_normals[count - 2]));
							}
							if (polyNode2.m_endtype == EndType.etClosedPolygon)
							{
								int k2 = count - 1;
								for (int n = 0; n < count; n++)
								{
									OffsetPoint(n, ref k2, polyNode2.m_jointype);
								}
								m_destPolys.Add(m_destPoly);
							}
							else if (polyNode2.m_endtype == EndType.etClosedLine)
							{
								int k3 = count - 1;
								for (int num8 = 0; num8 < count; num8++)
								{
									OffsetPoint(num8, ref k3, polyNode2.m_jointype);
								}
								m_destPolys.Add(m_destPoly);
								m_destPoly = new List<IntPoint>();
								DoublePoint doublePoint = m_normals[count - 1];
								for (int num9 = count - 1; num9 > 0; num9--)
								{
									List<DoublePoint> normals = m_normals;
									int index = num9;
									DoublePoint doublePoint2 = m_normals[num9 - 1];
									double x3 = 0.0 - doublePoint2.X;
									DoublePoint doublePoint3 = m_normals[num9 - 1];
									normals[index] = new DoublePoint(x3, 0.0 - doublePoint3.Y);
								}
								m_normals[0] = new DoublePoint(0.0 - doublePoint.X, 0.0 - doublePoint.Y);
								k3 = 0;
								for (int num10 = count - 1; num10 >= 0; num10--)
								{
									OffsetPoint(num10, ref k3, polyNode2.m_jointype);
								}
								m_destPolys.Add(m_destPoly);
							}
							else
							{
								int k4 = 0;
								for (int num11 = 1; num11 < count - 1; num11++)
								{
									OffsetPoint(num11, ref k4, polyNode2.m_jointype);
								}
								IntPoint item = default(IntPoint);
								if (polyNode2.m_endtype == EndType.etOpenButt)
								{
									int index2 = count - 1;
									IntPoint intPoint5 = m_srcPoly[index2];
									double num12 = (double)intPoint5.X;
									DoublePoint doublePoint4 = m_normals[index2];
									long x4 = Round(num12 + doublePoint4.X * delta);
									IntPoint intPoint6 = m_srcPoly[index2];
									double num13 = (double)intPoint6.Y;
									DoublePoint doublePoint5 = m_normals[index2];
									item = new IntPoint(x4, Round(num13 + doublePoint5.Y * delta));
									m_destPoly.Add(item);
									IntPoint intPoint7 = m_srcPoly[index2];
									double num14 = (double)intPoint7.X;
									DoublePoint doublePoint6 = m_normals[index2];
									long x5 = Round(num14 - doublePoint6.X * delta);
									IntPoint intPoint8 = m_srcPoly[index2];
									double num15 = (double)intPoint8.Y;
									DoublePoint doublePoint7 = m_normals[index2];
									item = new IntPoint(x5, Round(num15 - doublePoint7.Y * delta));
									m_destPoly.Add(item);
								}
								else
								{
									int num16 = count - 1;
									k4 = count - 2;
									m_sinA = 0.0;
									List<DoublePoint> normals2 = m_normals;
									int index3 = num16;
									DoublePoint doublePoint8 = m_normals[num16];
									double x6 = 0.0 - doublePoint8.X;
									DoublePoint doublePoint9 = m_normals[num16];
									normals2[index3] = new DoublePoint(x6, 0.0 - doublePoint9.Y);
									if (polyNode2.m_endtype == EndType.etOpenSquare)
									{
										DoSquare(num16, k4);
									}
									else
									{
										DoRound(num16, k4);
									}
								}
								for (int num17 = count - 1; num17 > 0; num17--)
								{
									List<DoublePoint> normals3 = m_normals;
									int index4 = num17;
									DoublePoint doublePoint10 = m_normals[num17 - 1];
									double x7 = 0.0 - doublePoint10.X;
									DoublePoint doublePoint11 = m_normals[num17 - 1];
									normals3[index4] = new DoublePoint(x7, 0.0 - doublePoint11.Y);
								}
								List<DoublePoint> normals4 = m_normals;
								DoublePoint doublePoint12 = m_normals[1];
								double x8 = 0.0 - doublePoint12.X;
								DoublePoint doublePoint13 = m_normals[1];
								normals4[0] = new DoublePoint(x8, 0.0 - doublePoint13.Y);
								k4 = count - 1;
								for (int num18 = k4 - 1; num18 > 0; num18--)
								{
									OffsetPoint(num18, ref k4, polyNode2.m_jointype);
								}
								if (polyNode2.m_endtype == EndType.etOpenButt)
								{
									IntPoint intPoint9 = m_srcPoly[0];
									double num19 = (double)intPoint9.X;
									DoublePoint doublePoint14 = m_normals[0];
									long x9 = Round(num19 - doublePoint14.X * delta);
									IntPoint intPoint10 = m_srcPoly[0];
									double num20 = (double)intPoint10.Y;
									DoublePoint doublePoint15 = m_normals[0];
									item = new IntPoint(x9, Round(num20 - doublePoint15.Y * delta));
									m_destPoly.Add(item);
									IntPoint intPoint11 = m_srcPoly[0];
									double num21 = (double)intPoint11.X;
									DoublePoint doublePoint16 = m_normals[0];
									long x10 = Round(num21 + doublePoint16.X * delta);
									IntPoint intPoint12 = m_srcPoly[0];
									double num22 = (double)intPoint12.Y;
									DoublePoint doublePoint17 = m_normals[0];
									item = new IntPoint(x10, Round(num22 + doublePoint17.Y * delta));
									m_destPoly.Add(item);
								}
								else
								{
									k4 = 1;
									m_sinA = 0.0;
									if (polyNode2.m_endtype == EndType.etOpenSquare)
									{
										DoSquare(0, 1);
									}
									else
									{
										DoRound(0, 1);
									}
								}
								m_destPolys.Add(m_destPoly);
							}
						}
					}
				}
			}
		}

		public void Execute(ref List<List<IntPoint>> solution, double delta)
		{
			solution.Clear();
			FixOrientations();
			DoOffset(delta);
			Clipper clipper = new Clipper(0);
			clipper.AddPaths(m_destPolys, PolyType.ptSubject, true);
			if (delta > 0.0)
			{
				clipper.Execute(ClipType.ctUnion, solution, PolyFillType.pftPositive, PolyFillType.pftPositive);
			}
			else
			{
				IntRect bounds = ClipperBase.GetBounds(m_destPolys);
				List<IntPoint> list = new List<IntPoint>(4);
				list.Add(new IntPoint(bounds.left - 10, bounds.bottom + 10));
				list.Add(new IntPoint(bounds.right + 10, bounds.bottom + 10));
				list.Add(new IntPoint(bounds.right + 10, bounds.top - 10));
				list.Add(new IntPoint(bounds.left - 10, bounds.top - 10));
				clipper.AddPath(list, PolyType.ptSubject, true);
				clipper.ReverseSolution = true;
				clipper.Execute(ClipType.ctUnion, solution, PolyFillType.pftNegative, PolyFillType.pftNegative);
				if (solution.Count > 0)
				{
					solution.RemoveAt(0);
				}
			}
		}

		public void Execute(ref PolyTree solution, double delta)
		{
			solution.Clear();
			FixOrientations();
			DoOffset(delta);
			Clipper clipper = new Clipper(0);
			clipper.AddPaths(m_destPolys, PolyType.ptSubject, true);
			if (delta > 0.0)
			{
				clipper.Execute(ClipType.ctUnion, solution, PolyFillType.pftPositive, PolyFillType.pftPositive);
			}
			else
			{
				IntRect bounds = ClipperBase.GetBounds(m_destPolys);
				List<IntPoint> list = new List<IntPoint>(4);
				list.Add(new IntPoint(bounds.left - 10, bounds.bottom + 10));
				list.Add(new IntPoint(bounds.right + 10, bounds.bottom + 10));
				list.Add(new IntPoint(bounds.right + 10, bounds.top - 10));
				list.Add(new IntPoint(bounds.left - 10, bounds.top - 10));
				clipper.AddPath(list, PolyType.ptSubject, true);
				clipper.ReverseSolution = true;
				clipper.Execute(ClipType.ctUnion, solution, PolyFillType.pftNegative, PolyFillType.pftNegative);
				if (solution.ChildCount == 1 && solution.Childs[0].ChildCount > 0)
				{
					PolyNode polyNode = solution.Childs[0];
					solution.Childs.Capacity = polyNode.ChildCount;
					solution.Childs[0] = polyNode.Childs[0];
					solution.Childs[0].m_Parent = solution;
					for (int i = 1; i < polyNode.ChildCount; i++)
					{
						solution.AddChild(polyNode.Childs[i]);
					}
				}
				else
				{
					solution.Clear();
				}
			}
		}

		private void OffsetPoint(int j, ref int k, JoinType jointype)
		{
			DoublePoint doublePoint = m_normals[k];
			double x = doublePoint.X;
			DoublePoint doublePoint2 = m_normals[j];
			double num = x * doublePoint2.Y;
			DoublePoint doublePoint3 = m_normals[j];
			double x2 = doublePoint3.X;
			DoublePoint doublePoint4 = m_normals[k];
			m_sinA = num - x2 * doublePoint4.Y;
			if (Math.Abs(m_sinA * m_delta) < 1.0)
			{
				DoublePoint doublePoint5 = m_normals[k];
				double x3 = doublePoint5.X;
				DoublePoint doublePoint6 = m_normals[j];
				double num2 = x3 * doublePoint6.X;
				DoublePoint doublePoint7 = m_normals[j];
				double y = doublePoint7.Y;
				DoublePoint doublePoint8 = m_normals[k];
				double num3 = num2 + y * doublePoint8.Y;
				if (num3 > 0.0)
				{
					List<IntPoint> destPoly = m_destPoly;
					IntPoint intPoint = m_srcPoly[j];
					double num4 = (double)intPoint.X;
					DoublePoint doublePoint9 = m_normals[k];
					long x4 = Round(num4 + doublePoint9.X * m_delta);
					IntPoint intPoint2 = m_srcPoly[j];
					double num5 = (double)intPoint2.Y;
					DoublePoint doublePoint10 = m_normals[k];
					destPoly.Add(new IntPoint(x4, Round(num5 + doublePoint10.Y * m_delta)));
					return;
				}
			}
			else if (m_sinA > 1.0)
			{
				m_sinA = 1.0;
			}
			else if (m_sinA < -1.0)
			{
				m_sinA = -1.0;
			}
			if (!(m_sinA * m_delta < 0.0))
			{
				switch (jointype)
				{
				case JoinType.jtMiter:
				{
					DoublePoint doublePoint11 = m_normals[j];
					double x5 = doublePoint11.X;
					DoublePoint doublePoint12 = m_normals[k];
					double num6 = x5 * doublePoint12.X;
					DoublePoint doublePoint13 = m_normals[j];
					double y2 = doublePoint13.Y;
					DoublePoint doublePoint14 = m_normals[k];
					double num7 = 1.0 + (num6 + y2 * doublePoint14.Y);
					if (num7 >= m_miterLim)
					{
						DoMiter(j, k, num7);
					}
					else
					{
						DoSquare(j, k);
					}
					break;
				}
				case JoinType.jtSquare:
					DoSquare(j, k);
					break;
				case JoinType.jtRound:
					DoRound(j, k);
					break;
				}
			}
			else
			{
				List<IntPoint> destPoly2 = m_destPoly;
				IntPoint intPoint3 = m_srcPoly[j];
				double num8 = (double)intPoint3.X;
				DoublePoint doublePoint15 = m_normals[k];
				long x6 = Round(num8 + doublePoint15.X * m_delta);
				IntPoint intPoint4 = m_srcPoly[j];
				double num9 = (double)intPoint4.Y;
				DoublePoint doublePoint16 = m_normals[k];
				destPoly2.Add(new IntPoint(x6, Round(num9 + doublePoint16.Y * m_delta)));
				m_destPoly.Add(m_srcPoly[j]);
				List<IntPoint> destPoly3 = m_destPoly;
				IntPoint intPoint5 = m_srcPoly[j];
				double num10 = (double)intPoint5.X;
				DoublePoint doublePoint17 = m_normals[j];
				long x7 = Round(num10 + doublePoint17.X * m_delta);
				IntPoint intPoint6 = m_srcPoly[j];
				double num11 = (double)intPoint6.Y;
				DoublePoint doublePoint18 = m_normals[j];
				destPoly3.Add(new IntPoint(x7, Round(num11 + doublePoint18.Y * m_delta)));
			}
			k = j;
		}

		internal void DoSquare(int j, int k)
		{
			double sinA = m_sinA;
			DoublePoint doublePoint = m_normals[k];
			double x = doublePoint.X;
			DoublePoint doublePoint2 = m_normals[j];
			double num = x * doublePoint2.X;
			DoublePoint doublePoint3 = m_normals[k];
			double y = doublePoint3.Y;
			DoublePoint doublePoint4 = m_normals[j];
			double num2 = Math.Tan(Math.Atan2(sinA, num + y * doublePoint4.Y) / 4.0);
			List<IntPoint> destPoly = m_destPoly;
			IntPoint intPoint = m_srcPoly[j];
			double num3 = (double)intPoint.X;
			double delta = m_delta;
			DoublePoint doublePoint5 = m_normals[k];
			double x2 = doublePoint5.X;
			DoublePoint doublePoint6 = m_normals[k];
			long x3 = Round(num3 + delta * (x2 - doublePoint6.Y * num2));
			IntPoint intPoint2 = m_srcPoly[j];
			double num4 = (double)intPoint2.Y;
			double delta2 = m_delta;
			DoublePoint doublePoint7 = m_normals[k];
			double y2 = doublePoint7.Y;
			DoublePoint doublePoint8 = m_normals[k];
			destPoly.Add(new IntPoint(x3, Round(num4 + delta2 * (y2 + doublePoint8.X * num2))));
			List<IntPoint> destPoly2 = m_destPoly;
			IntPoint intPoint3 = m_srcPoly[j];
			double num5 = (double)intPoint3.X;
			double delta3 = m_delta;
			DoublePoint doublePoint9 = m_normals[j];
			double x4 = doublePoint9.X;
			DoublePoint doublePoint10 = m_normals[j];
			long x5 = Round(num5 + delta3 * (x4 + doublePoint10.Y * num2));
			IntPoint intPoint4 = m_srcPoly[j];
			double num6 = (double)intPoint4.Y;
			double delta4 = m_delta;
			DoublePoint doublePoint11 = m_normals[j];
			double y3 = doublePoint11.Y;
			DoublePoint doublePoint12 = m_normals[j];
			destPoly2.Add(new IntPoint(x5, Round(num6 + delta4 * (y3 - doublePoint12.X * num2))));
		}

		internal void DoMiter(int j, int k, double r)
		{
			double num = m_delta / r;
			List<IntPoint> destPoly = m_destPoly;
			IntPoint intPoint = m_srcPoly[j];
			double num2 = (double)intPoint.X;
			DoublePoint doublePoint = m_normals[k];
			double x = doublePoint.X;
			DoublePoint doublePoint2 = m_normals[j];
			long x2 = Round(num2 + (x + doublePoint2.X) * num);
			IntPoint intPoint2 = m_srcPoly[j];
			double num3 = (double)intPoint2.Y;
			DoublePoint doublePoint3 = m_normals[k];
			double y = doublePoint3.Y;
			DoublePoint doublePoint4 = m_normals[j];
			destPoly.Add(new IntPoint(x2, Round(num3 + (y + doublePoint4.Y) * num)));
		}

		internal void DoRound(int j, int k)
		{
			double sinA = m_sinA;
			DoublePoint doublePoint = m_normals[k];
			double x = doublePoint.X;
			DoublePoint doublePoint2 = m_normals[j];
			double num = x * doublePoint2.X;
			DoublePoint doublePoint3 = m_normals[k];
			double y = doublePoint3.Y;
			DoublePoint doublePoint4 = m_normals[j];
			double value = Math.Atan2(sinA, num + y * doublePoint4.Y);
			int num2 = Math.Max((int)Round(m_StepsPerRad * Math.Abs(value)), 1);
			DoublePoint doublePoint5 = m_normals[k];
			double num3 = doublePoint5.X;
			DoublePoint doublePoint6 = m_normals[k];
			double num4 = doublePoint6.Y;
			for (int i = 0; i < num2; i++)
			{
				List<IntPoint> destPoly = m_destPoly;
				IntPoint intPoint = m_srcPoly[j];
				long x2 = Round((double)intPoint.X + num3 * m_delta);
				IntPoint intPoint2 = m_srcPoly[j];
				destPoly.Add(new IntPoint(x2, Round((double)intPoint2.Y + num4 * m_delta)));
				double num5 = num3;
				num3 = num3 * m_cos - m_sin * num4;
				num4 = num5 * m_sin + num4 * m_cos;
			}
			List<IntPoint> destPoly2 = m_destPoly;
			IntPoint intPoint3 = m_srcPoly[j];
			double num6 = (double)intPoint3.X;
			DoublePoint doublePoint7 = m_normals[j];
			long x3 = Round(num6 + doublePoint7.X * m_delta);
			IntPoint intPoint4 = m_srcPoly[j];
			double num7 = (double)intPoint4.Y;
			DoublePoint doublePoint8 = m_normals[j];
			destPoly2.Add(new IntPoint(x3, Round(num7 + doublePoint8.Y * m_delta)));
		}
	}
}
