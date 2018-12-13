using System;
using System.Collections.Generic;

namespace ClipperLib
{
	public class Clipper : ClipperBase
	{
		internal enum NodeType
		{
			ntAny,
			ntOpen,
			ntClosed
		}

		public const int ioReverseSolution = 1;

		public const int ioStrictlySimple = 2;

		public const int ioPreserveCollinear = 4;

		private List<OutRec> m_PolyOuts;

		private ClipType m_ClipType;

		private Scanbeam m_Scanbeam;

		private TEdge m_ActiveEdges;

		private TEdge m_SortedEdges;

		private List<IntersectNode> m_IntersectList;

		private IComparer<IntersectNode> m_IntersectNodeComparer;

		private bool m_ExecuteLocked;

		private PolyFillType m_ClipFillType;

		private PolyFillType m_SubjFillType;

		private List<Join> m_Joins;

		private List<Join> m_GhostJoins;

		private bool m_UsingPolyTree;

		public bool ReverseSolution
		{
			get;
			set;
		}

		public bool StrictlySimple
		{
			get;
			set;
		}

		public Clipper(int InitOptions = 0)
		{
			m_Scanbeam = null;
			m_ActiveEdges = null;
			m_SortedEdges = null;
			m_IntersectList = new List<IntersectNode>();
			m_IntersectNodeComparer = new MyIntersectNodeSort();
			m_ExecuteLocked = false;
			m_UsingPolyTree = false;
			m_PolyOuts = new List<OutRec>();
			m_Joins = new List<Join>();
			m_GhostJoins = new List<Join>();
			ReverseSolution = ((1 & InitOptions) != 0);
			StrictlySimple = ((2 & InitOptions) != 0);
			base.PreserveCollinear = ((4 & InitOptions) != 0);
		}

		private void DisposeScanbeamList()
		{
			while (m_Scanbeam != null)
			{
				Scanbeam next = m_Scanbeam.Next;
				m_Scanbeam = null;
				m_Scanbeam = next;
			}
		}

		protected override void Reset()
		{
			base.Reset();
			m_Scanbeam = null;
			m_ActiveEdges = null;
			m_SortedEdges = null;
			for (LocalMinima localMinima = m_MinimaList; localMinima != null; localMinima = localMinima.Next)
			{
				InsertScanbeam(localMinima.Y);
			}
		}

		private void InsertScanbeam(long Y)
		{
			if (m_Scanbeam == null)
			{
				m_Scanbeam = new Scanbeam();
				m_Scanbeam.Next = null;
				m_Scanbeam.Y = Y;
			}
			else if (Y > m_Scanbeam.Y)
			{
				Scanbeam scanbeam = new Scanbeam();
				scanbeam.Y = Y;
				scanbeam.Next = m_Scanbeam;
				m_Scanbeam = scanbeam;
			}
			else
			{
				Scanbeam scanbeam2 = m_Scanbeam;
				while (scanbeam2.Next != null && Y <= scanbeam2.Next.Y)
				{
					scanbeam2 = scanbeam2.Next;
				}
				if (Y != scanbeam2.Y)
				{
					Scanbeam scanbeam3 = new Scanbeam();
					scanbeam3.Y = Y;
					scanbeam3.Next = scanbeam2.Next;
					scanbeam2.Next = scanbeam3;
				}
			}
		}

		public bool Execute(ClipType clipType, List<List<IntPoint>> solution, PolyFillType subjFillType, PolyFillType clipFillType)
		{
			if (m_ExecuteLocked)
			{
				return false;
			}
			if (!m_HasOpenPaths)
			{
				m_ExecuteLocked = true;
				solution.Clear();
				m_SubjFillType = subjFillType;
				m_ClipFillType = clipFillType;
				m_ClipType = clipType;
				m_UsingPolyTree = false;
				try
				{
					bool flag = ExecuteInternal();
					if (!flag)
					{
						return flag;
					}
					BuildResult(solution);
					return flag;
				}
				finally
				{
					DisposeAllPolyPts();
					m_ExecuteLocked = false;
				}
			}
			throw new ClipperException("Error: PolyTree struct is need for open path clipping.");
		}

		public bool Execute(ClipType clipType, PolyTree polytree, PolyFillType subjFillType, PolyFillType clipFillType)
		{
			if (!m_ExecuteLocked)
			{
				m_ExecuteLocked = true;
				m_SubjFillType = subjFillType;
				m_ClipFillType = clipFillType;
				m_ClipType = clipType;
				m_UsingPolyTree = true;
				try
				{
					bool flag = ExecuteInternal();
					if (!flag)
					{
						return flag;
					}
					BuildResult2(polytree);
					return flag;
				}
				finally
				{
					DisposeAllPolyPts();
					m_ExecuteLocked = false;
				}
			}
			return false;
		}

		public bool Execute(ClipType clipType, List<List<IntPoint>> solution)
		{
			return Execute(clipType, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);
		}

		public bool Execute(ClipType clipType, PolyTree polytree)
		{
			return Execute(clipType, polytree, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);
		}

		internal void FixHoleLinkage(OutRec outRec)
		{
			if (outRec.FirstLeft != null && (outRec.IsHole == outRec.FirstLeft.IsHole || outRec.FirstLeft.Pts == null))
			{
				OutRec firstLeft = outRec.FirstLeft;
				while (firstLeft != null && (firstLeft.IsHole == outRec.IsHole || firstLeft.Pts == null))
				{
					firstLeft = firstLeft.FirstLeft;
				}
				outRec.FirstLeft = firstLeft;
			}
		}

		private bool ExecuteInternal()
		{
			try
			{
				Reset();
				if (m_CurrentLM != null)
				{
					long botY = PopScanbeam();
					do
					{
						InsertLocalMinimaIntoAEL(botY);
						m_GhostJoins.Clear();
						ProcessHorizontals(false);
						if (m_Scanbeam == null)
						{
							break;
						}
						long num = PopScanbeam();
						if (!ProcessIntersections(num))
						{
							return false;
						}
						ProcessEdgesAtTopOfScanbeam(num);
						botY = num;
					}
					while (m_Scanbeam != null || m_CurrentLM != null);
					for (int i = 0; i < m_PolyOuts.Count; i++)
					{
						OutRec outRec = m_PolyOuts[i];
						if (outRec.Pts != null && !outRec.IsOpen && (outRec.IsHole ^ ReverseSolution) == Area(outRec) > 0.0)
						{
							ReversePolyPtLinks(outRec.Pts);
						}
					}
					JoinCommonEdges();
					for (int j = 0; j < m_PolyOuts.Count; j++)
					{
						OutRec outRec2 = m_PolyOuts[j];
						if (outRec2.Pts != null && !outRec2.IsOpen)
						{
							FixupOutPolygon(outRec2);
						}
					}
					if (StrictlySimple)
					{
						DoSimplePolygons();
					}
					return true;
				}
				return false;
			}
			finally
			{
				m_Joins.Clear();
				m_GhostJoins.Clear();
			}
		}

		private long PopScanbeam()
		{
			long y = m_Scanbeam.Y;
			m_Scanbeam = m_Scanbeam.Next;
			return y;
		}

		private void DisposeAllPolyPts()
		{
			for (int i = 0; i < m_PolyOuts.Count; i++)
			{
				DisposeOutRec(i);
			}
			m_PolyOuts.Clear();
		}

		private void DisposeOutRec(int index)
		{
			OutRec outRec = m_PolyOuts[index];
			outRec.Pts = null;
			outRec = null;
			m_PolyOuts[index] = null;
		}

		private void AddJoin(OutPt Op1, OutPt Op2, IntPoint OffPt)
		{
			Join join = new Join();
			join.OutPt1 = Op1;
			join.OutPt2 = Op2;
			join.OffPt = OffPt;
			m_Joins.Add(join);
		}

		private void AddGhostJoin(OutPt Op, IntPoint OffPt)
		{
			Join join = new Join();
			join.OutPt1 = Op;
			join.OffPt = OffPt;
			m_GhostJoins.Add(join);
		}

		private void InsertLocalMinimaIntoAEL(long botY)
		{
			while (m_CurrentLM != null && m_CurrentLM.Y == botY)
			{
				TEdge leftBound = m_CurrentLM.LeftBound;
				TEdge rightBound = m_CurrentLM.RightBound;
				PopLocalMinima();
				OutPt outPt = null;
				if (leftBound == null)
				{
					InsertEdgeIntoAEL(rightBound, null);
					SetWindingCount(rightBound);
					if (IsContributing(rightBound))
					{
						outPt = AddOutPt(rightBound, rightBound.Bot);
					}
				}
				else if (rightBound == null)
				{
					InsertEdgeIntoAEL(leftBound, null);
					SetWindingCount(leftBound);
					if (IsContributing(leftBound))
					{
						outPt = AddOutPt(leftBound, leftBound.Bot);
					}
					InsertScanbeam(leftBound.Top.Y);
				}
				else
				{
					InsertEdgeIntoAEL(leftBound, null);
					InsertEdgeIntoAEL(rightBound, leftBound);
					SetWindingCount(leftBound);
					rightBound.WindCnt = leftBound.WindCnt;
					rightBound.WindCnt2 = leftBound.WindCnt2;
					if (IsContributing(leftBound))
					{
						outPt = AddLocalMinPoly(leftBound, rightBound, leftBound.Bot);
					}
					InsertScanbeam(leftBound.Top.Y);
				}
				if (rightBound != null)
				{
					if (ClipperBase.IsHorizontal(rightBound))
					{
						AddEdgeToSEL(rightBound);
					}
					else
					{
						InsertScanbeam(rightBound.Top.Y);
					}
				}
				if (leftBound != null && rightBound != null)
				{
					if (outPt != null && ClipperBase.IsHorizontal(rightBound) && m_GhostJoins.Count > 0 && rightBound.WindDelta != 0)
					{
						for (int i = 0; i < m_GhostJoins.Count; i++)
						{
							Join join = m_GhostJoins[i];
							if (HorzSegmentsOverlap(join.OutPt1.Pt.X, join.OffPt.X, rightBound.Bot.X, rightBound.Top.X))
							{
								AddJoin(join.OutPt1, outPt, join.OffPt);
							}
						}
					}
					if (leftBound.OutIdx >= 0 && leftBound.PrevInAEL != null && leftBound.PrevInAEL.Curr.X == leftBound.Bot.X && leftBound.PrevInAEL.OutIdx >= 0 && ClipperBase.SlopesEqual(leftBound.PrevInAEL, leftBound, m_UseFullRange) && leftBound.WindDelta != 0 && leftBound.PrevInAEL.WindDelta != 0)
					{
						OutPt op = AddOutPt(leftBound.PrevInAEL, leftBound.Bot);
						AddJoin(outPt, op, leftBound.Top);
					}
					if (leftBound.NextInAEL != rightBound)
					{
						if (rightBound.OutIdx >= 0 && rightBound.PrevInAEL.OutIdx >= 0 && ClipperBase.SlopesEqual(rightBound.PrevInAEL, rightBound, m_UseFullRange) && rightBound.WindDelta != 0 && rightBound.PrevInAEL.WindDelta != 0)
						{
							OutPt op2 = AddOutPt(rightBound.PrevInAEL, rightBound.Bot);
							AddJoin(outPt, op2, rightBound.Top);
						}
						TEdge nextInAEL = leftBound.NextInAEL;
						if (nextInAEL != null)
						{
							while (nextInAEL != rightBound)
							{
								IntersectEdges(rightBound, nextInAEL, leftBound.Curr);
								nextInAEL = nextInAEL.NextInAEL;
							}
						}
					}
				}
			}
		}

		private void InsertEdgeIntoAEL(TEdge edge, TEdge startEdge)
		{
			if (m_ActiveEdges == null)
			{
				edge.PrevInAEL = null;
				edge.NextInAEL = null;
				m_ActiveEdges = edge;
			}
			else if (startEdge == null && E2InsertsBeforeE1(m_ActiveEdges, edge))
			{
				edge.PrevInAEL = null;
				edge.NextInAEL = m_ActiveEdges;
				m_ActiveEdges.PrevInAEL = edge;
				m_ActiveEdges = edge;
			}
			else
			{
				if (startEdge == null)
				{
					startEdge = m_ActiveEdges;
				}
				while (startEdge.NextInAEL != null && !E2InsertsBeforeE1(startEdge.NextInAEL, edge))
				{
					startEdge = startEdge.NextInAEL;
				}
				edge.NextInAEL = startEdge.NextInAEL;
				if (startEdge.NextInAEL != null)
				{
					startEdge.NextInAEL.PrevInAEL = edge;
				}
				edge.PrevInAEL = startEdge;
				startEdge.NextInAEL = edge;
			}
		}

		private bool E2InsertsBeforeE1(TEdge e1, TEdge e2)
		{
			if (e2.Curr.X == e1.Curr.X)
			{
				if (e2.Top.Y > e1.Top.Y)
				{
					return e2.Top.X < TopX(e1, e2.Top.Y);
				}
				return e1.Top.X > TopX(e2, e1.Top.Y);
			}
			return e2.Curr.X < e1.Curr.X;
		}

		private bool IsEvenOddFillType(TEdge edge)
		{
			if (edge.PolyTyp == PolyType.ptSubject)
			{
				return m_SubjFillType == PolyFillType.pftEvenOdd;
			}
			return m_ClipFillType == PolyFillType.pftEvenOdd;
		}

		private bool IsEvenOddAltFillType(TEdge edge)
		{
			if (edge.PolyTyp == PolyType.ptSubject)
			{
				return m_ClipFillType == PolyFillType.pftEvenOdd;
			}
			return m_SubjFillType == PolyFillType.pftEvenOdd;
		}

		private bool IsContributing(TEdge edge)
		{
			PolyFillType polyFillType;
			PolyFillType polyFillType2;
			if (edge.PolyTyp == PolyType.ptSubject)
			{
				polyFillType = m_SubjFillType;
				polyFillType2 = m_ClipFillType;
			}
			else
			{
				polyFillType = m_ClipFillType;
				polyFillType2 = m_SubjFillType;
			}
			switch (polyFillType)
			{
			case PolyFillType.pftEvenOdd:
				if (edge.WindDelta == 0 && edge.WindCnt != 1)
				{
					return false;
				}
				break;
			case PolyFillType.pftNonZero:
				if (Math.Abs(edge.WindCnt) != 1)
				{
					return false;
				}
				break;
			case PolyFillType.pftPositive:
				if (edge.WindCnt != 1)
				{
					return false;
				}
				break;
			default:
				if (edge.WindCnt != -1)
				{
					return false;
				}
				break;
			}
			switch (m_ClipType)
			{
			case ClipType.ctIntersection:
				switch (polyFillType2)
				{
				case PolyFillType.pftEvenOdd:
				case PolyFillType.pftNonZero:
					return edge.WindCnt2 != 0;
				case PolyFillType.pftPositive:
					return edge.WindCnt2 > 0;
				default:
					return edge.WindCnt2 < 0;
				}
			case ClipType.ctUnion:
				switch (polyFillType2)
				{
				case PolyFillType.pftEvenOdd:
				case PolyFillType.pftNonZero:
					return edge.WindCnt2 == 0;
				case PolyFillType.pftPositive:
					return edge.WindCnt2 <= 0;
				default:
					return edge.WindCnt2 >= 0;
				}
			case ClipType.ctDifference:
				if (edge.PolyTyp != 0)
				{
					switch (polyFillType2)
					{
					case PolyFillType.pftEvenOdd:
					case PolyFillType.pftNonZero:
						return edge.WindCnt2 != 0;
					case PolyFillType.pftPositive:
						return edge.WindCnt2 > 0;
					default:
						return edge.WindCnt2 < 0;
					}
				}
				switch (polyFillType2)
				{
				case PolyFillType.pftEvenOdd:
				case PolyFillType.pftNonZero:
					return edge.WindCnt2 == 0;
				case PolyFillType.pftPositive:
					return edge.WindCnt2 <= 0;
				default:
					return edge.WindCnt2 >= 0;
				}
			case ClipType.ctXor:
				if (edge.WindDelta == 0)
				{
					switch (polyFillType2)
					{
					case PolyFillType.pftEvenOdd:
					case PolyFillType.pftNonZero:
						return edge.WindCnt2 == 0;
					case PolyFillType.pftPositive:
						return edge.WindCnt2 <= 0;
					default:
						return edge.WindCnt2 >= 0;
					}
				}
				return true;
			default:
				return true;
			}
		}

		private void SetWindingCount(TEdge edge)
		{
			TEdge prevInAEL = edge.PrevInAEL;
			while (prevInAEL != null && (prevInAEL.PolyTyp != edge.PolyTyp || prevInAEL.WindDelta == 0))
			{
				prevInAEL = prevInAEL.PrevInAEL;
			}
			if (prevInAEL == null)
			{
				edge.WindCnt = ((edge.WindDelta == 0) ? 1 : edge.WindDelta);
				edge.WindCnt2 = 0;
				prevInAEL = m_ActiveEdges;
			}
			else if (edge.WindDelta == 0 && m_ClipType != ClipType.ctUnion)
			{
				edge.WindCnt = 1;
				edge.WindCnt2 = prevInAEL.WindCnt2;
				prevInAEL = prevInAEL.NextInAEL;
			}
			else if (IsEvenOddFillType(edge))
			{
				if (edge.WindDelta == 0)
				{
					bool flag = true;
					for (TEdge prevInAEL2 = prevInAEL.PrevInAEL; prevInAEL2 != null; prevInAEL2 = prevInAEL2.PrevInAEL)
					{
						if (prevInAEL2.PolyTyp == prevInAEL.PolyTyp && prevInAEL2.WindDelta != 0)
						{
							flag = !flag;
						}
					}
					edge.WindCnt = ((!flag) ? 1 : 0);
				}
				else
				{
					edge.WindCnt = edge.WindDelta;
				}
				edge.WindCnt2 = prevInAEL.WindCnt2;
				prevInAEL = prevInAEL.NextInAEL;
			}
			else
			{
				if (prevInAEL.WindCnt * prevInAEL.WindDelta < 0)
				{
					if (Math.Abs(prevInAEL.WindCnt) > 1)
					{
						if (prevInAEL.WindDelta * edge.WindDelta < 0)
						{
							edge.WindCnt = prevInAEL.WindCnt;
						}
						else
						{
							edge.WindCnt = prevInAEL.WindCnt + edge.WindDelta;
						}
					}
					else
					{
						edge.WindCnt = ((edge.WindDelta == 0) ? 1 : edge.WindDelta);
					}
				}
				else if (edge.WindDelta == 0)
				{
					edge.WindCnt = ((prevInAEL.WindCnt >= 0) ? (prevInAEL.WindCnt + 1) : (prevInAEL.WindCnt - 1));
				}
				else if (prevInAEL.WindDelta * edge.WindDelta < 0)
				{
					edge.WindCnt = prevInAEL.WindCnt;
				}
				else
				{
					edge.WindCnt = prevInAEL.WindCnt + edge.WindDelta;
				}
				edge.WindCnt2 = prevInAEL.WindCnt2;
				prevInAEL = prevInAEL.NextInAEL;
			}
			if (IsEvenOddAltFillType(edge))
			{
				while (prevInAEL != edge)
				{
					if (prevInAEL.WindDelta != 0)
					{
						edge.WindCnt2 = ((edge.WindCnt2 == 0) ? 1 : 0);
					}
					prevInAEL = prevInAEL.NextInAEL;
				}
			}
			else
			{
				while (prevInAEL != edge)
				{
					edge.WindCnt2 += prevInAEL.WindDelta;
					prevInAEL = prevInAEL.NextInAEL;
				}
			}
		}

		private void AddEdgeToSEL(TEdge edge)
		{
			if (m_SortedEdges == null)
			{
				m_SortedEdges = edge;
				edge.PrevInSEL = null;
				edge.NextInSEL = null;
			}
			else
			{
				edge.NextInSEL = m_SortedEdges;
				edge.PrevInSEL = null;
				m_SortedEdges.PrevInSEL = edge;
				m_SortedEdges = edge;
			}
		}

		private void CopyAELToSEL()
		{
			for (TEdge tEdge = m_SortedEdges = m_ActiveEdges; tEdge != null; tEdge = tEdge.NextInAEL)
			{
				tEdge.PrevInSEL = tEdge.PrevInAEL;
				tEdge.NextInSEL = tEdge.NextInAEL;
			}
		}

		private void SwapPositionsInAEL(TEdge edge1, TEdge edge2)
		{
			if (edge1.NextInAEL != edge1.PrevInAEL && edge2.NextInAEL != edge2.PrevInAEL)
			{
				if (edge1.NextInAEL == edge2)
				{
					TEdge nextInAEL = edge2.NextInAEL;
					if (nextInAEL != null)
					{
						nextInAEL.PrevInAEL = edge1;
					}
					TEdge prevInAEL = edge1.PrevInAEL;
					if (prevInAEL != null)
					{
						prevInAEL.NextInAEL = edge2;
					}
					edge2.PrevInAEL = prevInAEL;
					edge2.NextInAEL = edge1;
					edge1.PrevInAEL = edge2;
					edge1.NextInAEL = nextInAEL;
				}
				else if (edge2.NextInAEL == edge1)
				{
					TEdge nextInAEL2 = edge1.NextInAEL;
					if (nextInAEL2 != null)
					{
						nextInAEL2.PrevInAEL = edge2;
					}
					TEdge prevInAEL2 = edge2.PrevInAEL;
					if (prevInAEL2 != null)
					{
						prevInAEL2.NextInAEL = edge1;
					}
					edge1.PrevInAEL = prevInAEL2;
					edge1.NextInAEL = edge2;
					edge2.PrevInAEL = edge1;
					edge2.NextInAEL = nextInAEL2;
				}
				else
				{
					TEdge nextInAEL3 = edge1.NextInAEL;
					TEdge prevInAEL3 = edge1.PrevInAEL;
					edge1.NextInAEL = edge2.NextInAEL;
					if (edge1.NextInAEL != null)
					{
						edge1.NextInAEL.PrevInAEL = edge1;
					}
					edge1.PrevInAEL = edge2.PrevInAEL;
					if (edge1.PrevInAEL != null)
					{
						edge1.PrevInAEL.NextInAEL = edge1;
					}
					edge2.NextInAEL = nextInAEL3;
					if (edge2.NextInAEL != null)
					{
						edge2.NextInAEL.PrevInAEL = edge2;
					}
					edge2.PrevInAEL = prevInAEL3;
					if (edge2.PrevInAEL != null)
					{
						edge2.PrevInAEL.NextInAEL = edge2;
					}
				}
				if (edge1.PrevInAEL == null)
				{
					m_ActiveEdges = edge1;
				}
				else if (edge2.PrevInAEL == null)
				{
					m_ActiveEdges = edge2;
				}
			}
		}

		private void SwapPositionsInSEL(TEdge edge1, TEdge edge2)
		{
			if ((edge1.NextInSEL != null || edge1.PrevInSEL != null) && (edge2.NextInSEL != null || edge2.PrevInSEL != null))
			{
				if (edge1.NextInSEL == edge2)
				{
					TEdge nextInSEL = edge2.NextInSEL;
					if (nextInSEL != null)
					{
						nextInSEL.PrevInSEL = edge1;
					}
					TEdge prevInSEL = edge1.PrevInSEL;
					if (prevInSEL != null)
					{
						prevInSEL.NextInSEL = edge2;
					}
					edge2.PrevInSEL = prevInSEL;
					edge2.NextInSEL = edge1;
					edge1.PrevInSEL = edge2;
					edge1.NextInSEL = nextInSEL;
				}
				else if (edge2.NextInSEL == edge1)
				{
					TEdge nextInSEL2 = edge1.NextInSEL;
					if (nextInSEL2 != null)
					{
						nextInSEL2.PrevInSEL = edge2;
					}
					TEdge prevInSEL2 = edge2.PrevInSEL;
					if (prevInSEL2 != null)
					{
						prevInSEL2.NextInSEL = edge1;
					}
					edge1.PrevInSEL = prevInSEL2;
					edge1.NextInSEL = edge2;
					edge2.PrevInSEL = edge1;
					edge2.NextInSEL = nextInSEL2;
				}
				else
				{
					TEdge nextInSEL3 = edge1.NextInSEL;
					TEdge prevInSEL3 = edge1.PrevInSEL;
					edge1.NextInSEL = edge2.NextInSEL;
					if (edge1.NextInSEL != null)
					{
						edge1.NextInSEL.PrevInSEL = edge1;
					}
					edge1.PrevInSEL = edge2.PrevInSEL;
					if (edge1.PrevInSEL != null)
					{
						edge1.PrevInSEL.NextInSEL = edge1;
					}
					edge2.NextInSEL = nextInSEL3;
					if (edge2.NextInSEL != null)
					{
						edge2.NextInSEL.PrevInSEL = edge2;
					}
					edge2.PrevInSEL = prevInSEL3;
					if (edge2.PrevInSEL != null)
					{
						edge2.PrevInSEL.NextInSEL = edge2;
					}
				}
				if (edge1.PrevInSEL == null)
				{
					m_SortedEdges = edge1;
				}
				else if (edge2.PrevInSEL == null)
				{
					m_SortedEdges = edge2;
				}
			}
		}

		private void AddLocalMaxPoly(TEdge e1, TEdge e2, IntPoint pt)
		{
			AddOutPt(e1, pt);
			if (e2.WindDelta == 0)
			{
				AddOutPt(e2, pt);
			}
			if (e1.OutIdx == e2.OutIdx)
			{
				e1.OutIdx = -1;
				e2.OutIdx = -1;
			}
			else if (e1.OutIdx < e2.OutIdx)
			{
				AppendPolygon(e1, e2);
			}
			else
			{
				AppendPolygon(e2, e1);
			}
		}

		private OutPt AddLocalMinPoly(TEdge e1, TEdge e2, IntPoint pt)
		{
			OutPt outPt;
			TEdge tEdge;
			TEdge tEdge2;
			if (ClipperBase.IsHorizontal(e2) || e1.Dx > e2.Dx)
			{
				outPt = AddOutPt(e1, pt);
				e2.OutIdx = e1.OutIdx;
				e1.Side = EdgeSide.esLeft;
				e2.Side = EdgeSide.esRight;
				tEdge = e1;
				tEdge2 = ((tEdge.PrevInAEL != e2) ? tEdge.PrevInAEL : e2.PrevInAEL);
			}
			else
			{
				outPt = AddOutPt(e2, pt);
				e1.OutIdx = e2.OutIdx;
				e1.Side = EdgeSide.esRight;
				e2.Side = EdgeSide.esLeft;
				tEdge = e2;
				tEdge2 = ((tEdge.PrevInAEL != e1) ? tEdge.PrevInAEL : e1.PrevInAEL);
			}
			if (tEdge2 != null && tEdge2.OutIdx >= 0 && TopX(tEdge2, pt.Y) == TopX(tEdge, pt.Y) && ClipperBase.SlopesEqual(tEdge, tEdge2, m_UseFullRange) && tEdge.WindDelta != 0 && tEdge2.WindDelta != 0)
			{
				OutPt op = AddOutPt(tEdge2, pt);
				AddJoin(outPt, op, tEdge.Top);
			}
			return outPt;
		}

		private OutRec CreateOutRec()
		{
			OutRec outRec = new OutRec();
			outRec.Idx = -1;
			outRec.IsHole = false;
			outRec.IsOpen = false;
			outRec.FirstLeft = null;
			outRec.Pts = null;
			outRec.BottomPt = null;
			outRec.PolyNode = null;
			m_PolyOuts.Add(outRec);
			outRec.Idx = m_PolyOuts.Count - 1;
			return outRec;
		}

		private OutPt AddOutPt(TEdge e, IntPoint pt)
		{
			bool flag = e.Side == EdgeSide.esLeft;
			if (e.OutIdx < 0)
			{
				OutRec outRec = CreateOutRec();
				outRec.IsOpen = (e.WindDelta == 0);
				OutPt outPt = outRec.Pts = new OutPt();
				outPt.Idx = outRec.Idx;
				outPt.Pt = pt;
				outPt.Next = outPt;
				outPt.Prev = outPt;
				if (!outRec.IsOpen)
				{
					SetHoleState(e, outRec);
				}
				e.OutIdx = outRec.Idx;
				return outPt;
			}
			OutRec outRec2 = m_PolyOuts[e.OutIdx];
			OutPt pts = outRec2.Pts;
			if (flag && pt == pts.Pt)
			{
				return pts;
			}
			if (!flag && pt == pts.Prev.Pt)
			{
				return pts.Prev;
			}
			OutPt outPt2 = new OutPt();
			outPt2.Idx = outRec2.Idx;
			outPt2.Pt = pt;
			outPt2.Next = pts;
			outPt2.Prev = pts.Prev;
			outPt2.Prev.Next = outPt2;
			pts.Prev = outPt2;
			if (flag)
			{
				outRec2.Pts = outPt2;
			}
			return outPt2;
		}

		internal void SwapPoints(ref IntPoint pt1, ref IntPoint pt2)
		{
			IntPoint intPoint = new IntPoint(pt1);
			pt1 = pt2;
			pt2 = intPoint;
		}

		private bool HorzSegmentsOverlap(long seg1a, long seg1b, long seg2a, long seg2b)
		{
			if (seg1a > seg1b)
			{
				Swap(ref seg1a, ref seg1b);
			}
			if (seg2a > seg2b)
			{
				Swap(ref seg2a, ref seg2b);
			}
			return seg1a < seg2b && seg2a < seg1b;
		}

		private void SetHoleState(TEdge e, OutRec outRec)
		{
			bool flag = false;
			for (TEdge prevInAEL = e.PrevInAEL; prevInAEL != null; prevInAEL = prevInAEL.PrevInAEL)
			{
				if (prevInAEL.OutIdx >= 0 && prevInAEL.WindDelta != 0)
				{
					flag = !flag;
					if (outRec.FirstLeft == null)
					{
						outRec.FirstLeft = m_PolyOuts[prevInAEL.OutIdx];
					}
				}
			}
			if (flag)
			{
				outRec.IsHole = true;
			}
		}

		private double GetDx(IntPoint pt1, IntPoint pt2)
		{
			if (pt1.Y == pt2.Y)
			{
				return -3.4E+38;
			}
			return (double)(pt2.X - pt1.X) / (double)(pt2.Y - pt1.Y);
		}

		private bool FirstIsBottomPt(OutPt btmPt1, OutPt btmPt2)
		{
			OutPt prev = btmPt1.Prev;
			while (prev.Pt == btmPt1.Pt && prev != btmPt1)
			{
				prev = prev.Prev;
			}
			double num = Math.Abs(GetDx(btmPt1.Pt, prev.Pt));
			prev = btmPt1.Next;
			while (prev.Pt == btmPt1.Pt && prev != btmPt1)
			{
				prev = prev.Next;
			}
			double num2 = Math.Abs(GetDx(btmPt1.Pt, prev.Pt));
			prev = btmPt2.Prev;
			while (prev.Pt == btmPt2.Pt && prev != btmPt2)
			{
				prev = prev.Prev;
			}
			double num3 = Math.Abs(GetDx(btmPt2.Pt, prev.Pt));
			prev = btmPt2.Next;
			while (prev.Pt == btmPt2.Pt && prev != btmPt2)
			{
				prev = prev.Next;
			}
			double num4 = Math.Abs(GetDx(btmPt2.Pt, prev.Pt));
			return (num >= num3 && num >= num4) || (num2 >= num3 && num2 >= num4);
		}

		private OutPt GetBottomPt(OutPt pp)
		{
			OutPt outPt = null;
			OutPt next;
			for (next = pp.Next; next != pp; next = next.Next)
			{
				if (next.Pt.Y > pp.Pt.Y)
				{
					pp = next;
					outPt = null;
				}
				else if (next.Pt.Y == pp.Pt.Y && next.Pt.X <= pp.Pt.X)
				{
					if (next.Pt.X < pp.Pt.X)
					{
						outPt = null;
						pp = next;
					}
					else if (next.Next != pp && next.Prev != pp)
					{
						outPt = next;
					}
				}
			}
			if (outPt != null)
			{
				while (outPt != next)
				{
					if (!FirstIsBottomPt(next, outPt))
					{
						pp = outPt;
					}
					outPt = outPt.Next;
					while (outPt.Pt != pp.Pt)
					{
						outPt = outPt.Next;
					}
				}
			}
			return pp;
		}

		private OutRec GetLowermostRec(OutRec outRec1, OutRec outRec2)
		{
			if (outRec1.BottomPt == null)
			{
				outRec1.BottomPt = GetBottomPt(outRec1.Pts);
			}
			if (outRec2.BottomPt == null)
			{
				outRec2.BottomPt = GetBottomPt(outRec2.Pts);
			}
			OutPt bottomPt = outRec1.BottomPt;
			OutPt bottomPt2 = outRec2.BottomPt;
			if (bottomPt.Pt.Y > bottomPt2.Pt.Y)
			{
				return outRec1;
			}
			if (bottomPt.Pt.Y < bottomPt2.Pt.Y)
			{
				return outRec2;
			}
			if (bottomPt.Pt.X < bottomPt2.Pt.X)
			{
				return outRec1;
			}
			if (bottomPt.Pt.X > bottomPt2.Pt.X)
			{
				return outRec2;
			}
			if (bottomPt.Next == bottomPt)
			{
				return outRec2;
			}
			if (bottomPt2.Next == bottomPt2)
			{
				return outRec1;
			}
			if (FirstIsBottomPt(bottomPt, bottomPt2))
			{
				return outRec1;
			}
			return outRec2;
		}

		private bool Param1RightOfParam2(OutRec outRec1, OutRec outRec2)
		{
			do
			{
				outRec1 = outRec1.FirstLeft;
				if (outRec1 == outRec2)
				{
					return true;
				}
			}
			while (outRec1 != null);
			return false;
		}

		private OutRec GetOutRec(int idx)
		{
			OutRec outRec;
			for (outRec = m_PolyOuts[idx]; outRec != m_PolyOuts[outRec.Idx]; outRec = m_PolyOuts[outRec.Idx])
			{
			}
			return outRec;
		}

		private void AppendPolygon(TEdge e1, TEdge e2)
		{
			OutRec outRec = m_PolyOuts[e1.OutIdx];
			OutRec outRec2 = m_PolyOuts[e2.OutIdx];
			OutRec outRec3 = Param1RightOfParam2(outRec, outRec2) ? outRec2 : ((!Param1RightOfParam2(outRec2, outRec)) ? GetLowermostRec(outRec, outRec2) : outRec);
			OutPt pts = outRec.Pts;
			OutPt prev = pts.Prev;
			OutPt pts2 = outRec2.Pts;
			OutPt prev2 = pts2.Prev;
			EdgeSide side;
			if (e1.Side == EdgeSide.esLeft)
			{
				if (e2.Side == EdgeSide.esLeft)
				{
					ReversePolyPtLinks(pts2);
					pts2.Next = pts;
					pts.Prev = pts2;
					prev.Next = prev2;
					prev2.Prev = prev;
					outRec.Pts = prev2;
				}
				else
				{
					prev2.Next = pts;
					pts.Prev = prev2;
					pts2.Prev = prev;
					prev.Next = pts2;
					outRec.Pts = pts2;
				}
				side = EdgeSide.esLeft;
			}
			else
			{
				if (e2.Side == EdgeSide.esRight)
				{
					ReversePolyPtLinks(pts2);
					prev.Next = prev2;
					prev2.Prev = prev;
					pts2.Next = pts;
					pts.Prev = pts2;
				}
				else
				{
					prev.Next = pts2;
					pts2.Prev = prev;
					pts.Prev = prev2;
					prev2.Next = pts;
				}
				side = EdgeSide.esRight;
			}
			outRec.BottomPt = null;
			if (outRec3 == outRec2)
			{
				if (outRec2.FirstLeft != outRec)
				{
					outRec.FirstLeft = outRec2.FirstLeft;
				}
				outRec.IsHole = outRec2.IsHole;
			}
			outRec2.Pts = null;
			outRec2.BottomPt = null;
			outRec2.FirstLeft = outRec;
			int outIdx = e1.OutIdx;
			int outIdx2 = e2.OutIdx;
			e1.OutIdx = -1;
			e2.OutIdx = -1;
			for (TEdge tEdge = m_ActiveEdges; tEdge != null; tEdge = tEdge.NextInAEL)
			{
				if (tEdge.OutIdx == outIdx2)
				{
					tEdge.OutIdx = outIdx;
					tEdge.Side = side;
					break;
				}
			}
			outRec2.Idx = outRec.Idx;
		}

		private void ReversePolyPtLinks(OutPt pp)
		{
			if (pp != null)
			{
				OutPt outPt = pp;
				do
				{
					OutPt next = outPt.Next;
					outPt.Next = outPt.Prev;
					outPt.Prev = next;
					outPt = next;
				}
				while (outPt != pp);
			}
		}

		private static void SwapSides(TEdge edge1, TEdge edge2)
		{
			EdgeSide side = edge1.Side;
			edge1.Side = edge2.Side;
			edge2.Side = side;
		}

		private static void SwapPolyIndexes(TEdge edge1, TEdge edge2)
		{
			int outIdx = edge1.OutIdx;
			edge1.OutIdx = edge2.OutIdx;
			edge2.OutIdx = outIdx;
		}

		private void IntersectEdges(TEdge e1, TEdge e2, IntPoint pt)
		{
			bool flag = e1.OutIdx >= 0;
			bool flag2 = e2.OutIdx >= 0;
			if (e1.PolyTyp == e2.PolyTyp)
			{
				if (IsEvenOddFillType(e1))
				{
					int windCnt = e1.WindCnt;
					e1.WindCnt = e2.WindCnt;
					e2.WindCnt = windCnt;
				}
				else
				{
					if (e1.WindCnt + e2.WindDelta == 0)
					{
						e1.WindCnt = -e1.WindCnt;
					}
					else
					{
						e1.WindCnt += e2.WindDelta;
					}
					if (e2.WindCnt - e1.WindDelta == 0)
					{
						e2.WindCnt = -e2.WindCnt;
					}
					else
					{
						e2.WindCnt -= e1.WindDelta;
					}
				}
			}
			else
			{
				if (!IsEvenOddFillType(e2))
				{
					e1.WindCnt2 += e2.WindDelta;
				}
				else
				{
					e1.WindCnt2 = ((e1.WindCnt2 == 0) ? 1 : 0);
				}
				if (!IsEvenOddFillType(e1))
				{
					e2.WindCnt2 -= e1.WindDelta;
				}
				else
				{
					e2.WindCnt2 = ((e2.WindCnt2 == 0) ? 1 : 0);
				}
			}
			PolyFillType polyFillType;
			PolyFillType polyFillType2;
			if (e1.PolyTyp == PolyType.ptSubject)
			{
				polyFillType = m_SubjFillType;
				polyFillType2 = m_ClipFillType;
			}
			else
			{
				polyFillType = m_ClipFillType;
				polyFillType2 = m_SubjFillType;
			}
			PolyFillType polyFillType3;
			PolyFillType polyFillType4;
			if (e2.PolyTyp == PolyType.ptSubject)
			{
				polyFillType3 = m_SubjFillType;
				polyFillType4 = m_ClipFillType;
			}
			else
			{
				polyFillType3 = m_ClipFillType;
				polyFillType4 = m_SubjFillType;
			}
			int num;
			switch (polyFillType)
			{
			case PolyFillType.pftPositive:
				num = e1.WindCnt;
				break;
			case PolyFillType.pftNegative:
				num = -e1.WindCnt;
				break;
			default:
				num = Math.Abs(e1.WindCnt);
				break;
			}
			int num2;
			switch (polyFillType3)
			{
			case PolyFillType.pftPositive:
				num2 = e2.WindCnt;
				break;
			case PolyFillType.pftNegative:
				num2 = -e2.WindCnt;
				break;
			default:
				num2 = Math.Abs(e2.WindCnt);
				break;
			}
			if (flag && flag2)
			{
				if ((num != 0 && num != 1) || (num2 != 0 && num2 != 1) || (e1.PolyTyp != e2.PolyTyp && m_ClipType != ClipType.ctXor))
				{
					AddLocalMaxPoly(e1, e2, pt);
				}
				else
				{
					AddOutPt(e1, pt);
					AddOutPt(e2, pt);
					SwapSides(e1, e2);
					SwapPolyIndexes(e1, e2);
				}
			}
			else if (flag)
			{
				if (num2 == 0 || num2 == 1)
				{
					AddOutPt(e1, pt);
					SwapSides(e1, e2);
					SwapPolyIndexes(e1, e2);
				}
			}
			else if (flag2)
			{
				if (num == 0 || num == 1)
				{
					AddOutPt(e2, pt);
					SwapSides(e1, e2);
					SwapPolyIndexes(e1, e2);
				}
			}
			else if ((num == 0 || num == 1) && (num2 == 0 || num2 == 1))
			{
				long num3;
				switch (polyFillType2)
				{
				case PolyFillType.pftPositive:
					num3 = e1.WindCnt2;
					break;
				case PolyFillType.pftNegative:
					num3 = -e1.WindCnt2;
					break;
				default:
					num3 = Math.Abs(e1.WindCnt2);
					break;
				}
				long num4;
				switch (polyFillType4)
				{
				case PolyFillType.pftPositive:
					num4 = e2.WindCnt2;
					break;
				case PolyFillType.pftNegative:
					num4 = -e2.WindCnt2;
					break;
				default:
					num4 = Math.Abs(e2.WindCnt2);
					break;
				}
				if (e1.PolyTyp != e2.PolyTyp)
				{
					AddLocalMinPoly(e1, e2, pt);
				}
				else if (num != 1 || num2 != 1)
				{
					SwapSides(e1, e2);
				}
				else
				{
					switch (m_ClipType)
					{
					case ClipType.ctIntersection:
						if (num3 > 0 && num4 > 0)
						{
							AddLocalMinPoly(e1, e2, pt);
						}
						break;
					case ClipType.ctUnion:
						if (num3 <= 0 && num4 <= 0)
						{
							AddLocalMinPoly(e1, e2, pt);
						}
						break;
					case ClipType.ctDifference:
						if ((e1.PolyTyp == PolyType.ptClip && num3 > 0 && num4 > 0) || (e1.PolyTyp == PolyType.ptSubject && num3 <= 0 && num4 <= 0))
						{
							AddLocalMinPoly(e1, e2, pt);
						}
						break;
					case ClipType.ctXor:
						AddLocalMinPoly(e1, e2, pt);
						break;
					}
				}
			}
		}

		private void DeleteFromAEL(TEdge e)
		{
			TEdge prevInAEL = e.PrevInAEL;
			TEdge nextInAEL = e.NextInAEL;
			if (prevInAEL != null || nextInAEL != null || e == m_ActiveEdges)
			{
				if (prevInAEL != null)
				{
					prevInAEL.NextInAEL = nextInAEL;
				}
				else
				{
					m_ActiveEdges = nextInAEL;
				}
				if (nextInAEL != null)
				{
					nextInAEL.PrevInAEL = prevInAEL;
				}
				e.NextInAEL = null;
				e.PrevInAEL = null;
			}
		}

		private void DeleteFromSEL(TEdge e)
		{
			TEdge prevInSEL = e.PrevInSEL;
			TEdge nextInSEL = e.NextInSEL;
			if (prevInSEL != null || nextInSEL != null || e == m_SortedEdges)
			{
				if (prevInSEL != null)
				{
					prevInSEL.NextInSEL = nextInSEL;
				}
				else
				{
					m_SortedEdges = nextInSEL;
				}
				if (nextInSEL != null)
				{
					nextInSEL.PrevInSEL = prevInSEL;
				}
				e.NextInSEL = null;
				e.PrevInSEL = null;
			}
		}

		private void UpdateEdgeIntoAEL(ref TEdge e)
		{
			if (e.NextInLML == null)
			{
				throw new ClipperException("UpdateEdgeIntoAEL: invalid call");
			}
			TEdge prevInAEL = e.PrevInAEL;
			TEdge nextInAEL = e.NextInAEL;
			e.NextInLML.OutIdx = e.OutIdx;
			if (prevInAEL != null)
			{
				prevInAEL.NextInAEL = e.NextInLML;
			}
			else
			{
				m_ActiveEdges = e.NextInLML;
			}
			if (nextInAEL != null)
			{
				nextInAEL.PrevInAEL = e.NextInLML;
			}
			e.NextInLML.Side = e.Side;
			e.NextInLML.WindDelta = e.WindDelta;
			e.NextInLML.WindCnt = e.WindCnt;
			e.NextInLML.WindCnt2 = e.WindCnt2;
			e = e.NextInLML;
			e.Curr = e.Bot;
			e.PrevInAEL = prevInAEL;
			e.NextInAEL = nextInAEL;
			if (!ClipperBase.IsHorizontal(e))
			{
				InsertScanbeam(e.Top.Y);
			}
		}

		private void ProcessHorizontals(bool isTopOfScanbeam)
		{
			for (TEdge sortedEdges = m_SortedEdges; sortedEdges != null; sortedEdges = m_SortedEdges)
			{
				DeleteFromSEL(sortedEdges);
				ProcessHorizontal(sortedEdges, isTopOfScanbeam);
			}
		}

		private void GetHorzDirection(TEdge HorzEdge, out Direction Dir, out long Left, out long Right)
		{
			if (HorzEdge.Bot.X < HorzEdge.Top.X)
			{
				Left = HorzEdge.Bot.X;
				Right = HorzEdge.Top.X;
				Dir = Direction.dLeftToRight;
			}
			else
			{
				Left = HorzEdge.Top.X;
				Right = HorzEdge.Bot.X;
				Dir = Direction.dRightToLeft;
			}
		}

		private void ProcessHorizontal(TEdge horzEdge, bool isTopOfScanbeam)
		{
			GetHorzDirection(horzEdge, out Direction Dir, out long Left, out long Right);
			TEdge tEdge = horzEdge;
			TEdge tEdge2 = null;
			while (tEdge.NextInLML != null && ClipperBase.IsHorizontal(tEdge.NextInLML))
			{
				tEdge = tEdge.NextInLML;
			}
			if (tEdge.NextInLML == null)
			{
				tEdge2 = GetMaximaPair(tEdge);
			}
			while (true)
			{
				bool flag = horzEdge == tEdge;
				TEdge tEdge3 = GetNextInAEL(horzEdge, Dir);
				while (tEdge3 != null && (tEdge3.Curr.X != horzEdge.Top.X || horzEdge.NextInLML == null || !(tEdge3.Dx < horzEdge.NextInLML.Dx)))
				{
					TEdge nextInAEL = GetNextInAEL(tEdge3, Dir);
					if ((Dir == Direction.dLeftToRight && tEdge3.Curr.X <= Right) || (Dir == Direction.dRightToLeft && tEdge3.Curr.X >= Left))
					{
						if (tEdge3 == tEdge2 && flag)
						{
							if (horzEdge.OutIdx >= 0)
							{
								OutPt outPt = AddOutPt(horzEdge, horzEdge.Top);
								for (TEdge tEdge4 = m_SortedEdges; tEdge4 != null; tEdge4 = tEdge4.NextInSEL)
								{
									if (tEdge4.OutIdx >= 0 && HorzSegmentsOverlap(horzEdge.Bot.X, horzEdge.Top.X, tEdge4.Bot.X, tEdge4.Top.X))
									{
										OutPt op = AddOutPt(tEdge4, tEdge4.Bot);
										AddJoin(op, outPt, tEdge4.Top);
									}
								}
								AddGhostJoin(outPt, horzEdge.Bot);
								AddLocalMaxPoly(horzEdge, tEdge2, horzEdge.Top);
							}
							DeleteFromAEL(horzEdge);
							DeleteFromAEL(tEdge2);
							return;
						}
						if (Dir != Direction.dLeftToRight)
						{
							IntersectEdges(pt: new IntPoint(tEdge3.Curr.X, horzEdge.Curr.Y), e1: tEdge3, e2: horzEdge);
						}
						else
						{
							IntersectEdges(pt: new IntPoint(tEdge3.Curr.X, horzEdge.Curr.Y), e1: horzEdge, e2: tEdge3);
						}
						SwapPositionsInAEL(horzEdge, tEdge3);
					}
					else if ((Dir == Direction.dLeftToRight && tEdge3.Curr.X >= Right) || (Dir == Direction.dRightToLeft && tEdge3.Curr.X <= Left))
					{
						break;
					}
					tEdge3 = nextInAEL;
				}
				if (horzEdge.NextInLML == null || !ClipperBase.IsHorizontal(horzEdge.NextInLML))
				{
					break;
				}
				UpdateEdgeIntoAEL(ref horzEdge);
				if (horzEdge.OutIdx >= 0)
				{
					AddOutPt(horzEdge, horzEdge.Bot);
				}
				GetHorzDirection(horzEdge, out Dir, out Left, out Right);
			}
			if (horzEdge.NextInLML != null)
			{
				if (horzEdge.OutIdx >= 0)
				{
					OutPt outPt2 = AddOutPt(horzEdge, horzEdge.Top);
					if (isTopOfScanbeam)
					{
						AddGhostJoin(outPt2, horzEdge.Bot);
					}
					UpdateEdgeIntoAEL(ref horzEdge);
					if (horzEdge.WindDelta != 0)
					{
						TEdge prevInAEL = horzEdge.PrevInAEL;
						TEdge nextInAEL2 = horzEdge.NextInAEL;
						if (prevInAEL != null && prevInAEL.Curr.X == horzEdge.Bot.X && prevInAEL.Curr.Y == horzEdge.Bot.Y && prevInAEL.WindDelta != 0 && prevInAEL.OutIdx >= 0 && prevInAEL.Curr.Y > prevInAEL.Top.Y && ClipperBase.SlopesEqual(horzEdge, prevInAEL, m_UseFullRange))
						{
							OutPt op2 = AddOutPt(prevInAEL, horzEdge.Bot);
							AddJoin(outPt2, op2, horzEdge.Top);
						}
						else if (nextInAEL2 != null && nextInAEL2.Curr.X == horzEdge.Bot.X && nextInAEL2.Curr.Y == horzEdge.Bot.Y && nextInAEL2.WindDelta != 0 && nextInAEL2.OutIdx >= 0 && nextInAEL2.Curr.Y > nextInAEL2.Top.Y && ClipperBase.SlopesEqual(horzEdge, nextInAEL2, m_UseFullRange))
						{
							OutPt op3 = AddOutPt(nextInAEL2, horzEdge.Bot);
							AddJoin(outPt2, op3, horzEdge.Top);
						}
					}
				}
				else
				{
					UpdateEdgeIntoAEL(ref horzEdge);
				}
			}
			else
			{
				if (horzEdge.OutIdx >= 0)
				{
					AddOutPt(horzEdge, horzEdge.Top);
				}
				DeleteFromAEL(horzEdge);
			}
		}

		private TEdge GetNextInAEL(TEdge e, Direction Direction)
		{
			return (Direction != Direction.dLeftToRight) ? e.PrevInAEL : e.NextInAEL;
		}

		private bool IsMinima(TEdge e)
		{
			return e != null && e.Prev.NextInLML != e && e.Next.NextInLML != e;
		}

		private bool IsMaxima(TEdge e, double Y)
		{
			return e != null && (double)e.Top.Y == Y && e.NextInLML == null;
		}

		private bool IsIntermediate(TEdge e, double Y)
		{
			return (double)e.Top.Y == Y && e.NextInLML != null;
		}

		private TEdge GetMaximaPair(TEdge e)
		{
			TEdge tEdge = null;
			if (e.Next.Top == e.Top && e.Next.NextInLML == null)
			{
				tEdge = e.Next;
			}
			else if (e.Prev.Top == e.Top && e.Prev.NextInLML == null)
			{
				tEdge = e.Prev;
			}
			if (tEdge != null && (tEdge.OutIdx == -2 || (tEdge.NextInAEL == tEdge.PrevInAEL && !ClipperBase.IsHorizontal(tEdge))))
			{
				return null;
			}
			return tEdge;
		}

		private bool ProcessIntersections(long topY)
		{
			if (m_ActiveEdges == null)
			{
				return true;
			}
			try
			{
				BuildIntersectList(topY);
				if (m_IntersectList.Count == 0)
				{
					return true;
				}
				if (m_IntersectList.Count != 1 && !FixupIntersectionOrder())
				{
					return false;
				}
				ProcessIntersectList();
			}
			catch
			{
				m_SortedEdges = null;
				m_IntersectList.Clear();
				throw new ClipperException("ProcessIntersections error");
			}
			m_SortedEdges = null;
			return true;
		}

		private void BuildIntersectList(long topY)
		{
			if (m_ActiveEdges != null)
			{
				for (TEdge tEdge = m_SortedEdges = m_ActiveEdges; tEdge != null; tEdge = tEdge.NextInAEL)
				{
					tEdge.PrevInSEL = tEdge.PrevInAEL;
					tEdge.NextInSEL = tEdge.NextInAEL;
					tEdge.Curr.X = TopX(tEdge, topY);
				}
				bool flag = true;
				while (flag)
				{
					if (m_SortedEdges == null)
					{
						break;
					}
					flag = false;
					TEdge tEdge = m_SortedEdges;
					while (tEdge.NextInSEL != null)
					{
						TEdge nextInSEL = tEdge.NextInSEL;
						if (tEdge.Curr.X > nextInSEL.Curr.X)
						{
							IntersectPoint(tEdge, nextInSEL, out IntPoint ip);
							IntersectNode intersectNode = new IntersectNode();
							intersectNode.Edge1 = tEdge;
							intersectNode.Edge2 = nextInSEL;
							intersectNode.Pt = ip;
							m_IntersectList.Add(intersectNode);
							SwapPositionsInSEL(tEdge, nextInSEL);
							flag = true;
						}
						else
						{
							tEdge = nextInSEL;
						}
					}
					if (tEdge.PrevInSEL == null)
					{
						break;
					}
					tEdge.PrevInSEL.NextInSEL = null;
				}
				m_SortedEdges = null;
			}
		}

		private bool EdgesAdjacent(IntersectNode inode)
		{
			return inode.Edge1.NextInSEL == inode.Edge2 || inode.Edge1.PrevInSEL == inode.Edge2;
		}

		private static int IntersectNodeSort(IntersectNode node1, IntersectNode node2)
		{
			return (int)(node2.Pt.Y - node1.Pt.Y);
		}

		private bool FixupIntersectionOrder()
		{
			m_IntersectList.Sort(m_IntersectNodeComparer);
			CopyAELToSEL();
			int count = m_IntersectList.Count;
			for (int i = 0; i < count; i++)
			{
				if (!EdgesAdjacent(m_IntersectList[i]))
				{
					int j;
					for (j = i + 1; j < count && !EdgesAdjacent(m_IntersectList[j]); j++)
					{
					}
					if (j == count)
					{
						return false;
					}
					IntersectNode value = m_IntersectList[i];
					m_IntersectList[i] = m_IntersectList[j];
					m_IntersectList[j] = value;
				}
				SwapPositionsInSEL(m_IntersectList[i].Edge1, m_IntersectList[i].Edge2);
			}
			return true;
		}

		private void ProcessIntersectList()
		{
			for (int i = 0; i < m_IntersectList.Count; i++)
			{
				IntersectNode intersectNode = m_IntersectList[i];
				IntersectEdges(intersectNode.Edge1, intersectNode.Edge2, intersectNode.Pt);
				SwapPositionsInAEL(intersectNode.Edge1, intersectNode.Edge2);
			}
			m_IntersectList.Clear();
		}

		internal static long Round(double value)
		{
			return (!(value < 0.0)) ? ((long)(value + 0.5)) : ((long)(value - 0.5));
		}

		private static long TopX(TEdge edge, long currentY)
		{
			if (currentY == edge.Top.Y)
			{
				return edge.Top.X;
			}
			return edge.Bot.X + Round(edge.Dx * (double)(currentY - edge.Bot.Y));
		}

		private void IntersectPoint(TEdge edge1, TEdge edge2, out IntPoint ip)
		{
			ip = default(IntPoint);
			if (edge1.Dx == edge2.Dx)
			{
				ip.Y = edge1.Curr.Y;
				ip.X = TopX(edge1, ip.Y);
			}
			else
			{
				if (edge1.Delta.X == 0)
				{
					ip.X = edge1.Bot.X;
					if (ClipperBase.IsHorizontal(edge2))
					{
						ip.Y = edge2.Bot.Y;
					}
					else
					{
						double num = (double)edge2.Bot.Y - (double)edge2.Bot.X / edge2.Dx;
						ip.Y = Round((double)ip.X / edge2.Dx + num);
					}
				}
				else if (edge2.Delta.X == 0)
				{
					ip.X = edge2.Bot.X;
					if (ClipperBase.IsHorizontal(edge1))
					{
						ip.Y = edge1.Bot.Y;
					}
					else
					{
						double num2 = (double)edge1.Bot.Y - (double)edge1.Bot.X / edge1.Dx;
						ip.Y = Round((double)ip.X / edge1.Dx + num2);
					}
				}
				else
				{
					double num2 = (double)edge1.Bot.X - (double)edge1.Bot.Y * edge1.Dx;
					double num = (double)edge2.Bot.X - (double)edge2.Bot.Y * edge2.Dx;
					double num3 = (num - num2) / (edge1.Dx - edge2.Dx);
					ip.Y = Round(num3);
					if (Math.Abs(edge1.Dx) < Math.Abs(edge2.Dx))
					{
						ip.X = Round(edge1.Dx * num3 + num2);
					}
					else
					{
						ip.X = Round(edge2.Dx * num3 + num);
					}
				}
				if (ip.Y < edge1.Top.Y || ip.Y < edge2.Top.Y)
				{
					if (edge1.Top.Y > edge2.Top.Y)
					{
						ip.Y = edge1.Top.Y;
					}
					else
					{
						ip.Y = edge2.Top.Y;
					}
					if (Math.Abs(edge1.Dx) < Math.Abs(edge2.Dx))
					{
						ip.X = TopX(edge1, ip.Y);
					}
					else
					{
						ip.X = TopX(edge2, ip.Y);
					}
				}
				if (ip.Y > edge1.Curr.Y)
				{
					ip.Y = edge1.Curr.Y;
					if (Math.Abs(edge1.Dx) > Math.Abs(edge2.Dx))
					{
						ip.X = TopX(edge2, ip.Y);
					}
					else
					{
						ip.X = TopX(edge1, ip.Y);
					}
				}
			}
		}

		private void ProcessEdgesAtTopOfScanbeam(long topY)
		{
			TEdge e = m_ActiveEdges;
			while (e != null)
			{
				bool flag = IsMaxima(e, (double)topY);
				if (flag)
				{
					TEdge maximaPair = GetMaximaPair(e);
					flag = (maximaPair == null || !ClipperBase.IsHorizontal(maximaPair));
				}
				if (flag)
				{
					TEdge prevInAEL = e.PrevInAEL;
					DoMaxima(e);
					e = ((prevInAEL != null) ? prevInAEL.NextInAEL : m_ActiveEdges);
				}
				else
				{
					if (IsIntermediate(e, (double)topY) && ClipperBase.IsHorizontal(e.NextInLML))
					{
						UpdateEdgeIntoAEL(ref e);
						if (e.OutIdx >= 0)
						{
							AddOutPt(e, e.Bot);
						}
						AddEdgeToSEL(e);
					}
					else
					{
						e.Curr.X = TopX(e, topY);
						e.Curr.Y = topY;
					}
					if (StrictlySimple)
					{
						TEdge prevInAEL2 = e.PrevInAEL;
						if (e.OutIdx >= 0 && e.WindDelta != 0 && prevInAEL2 != null && prevInAEL2.OutIdx >= 0 && prevInAEL2.Curr.X == e.Curr.X && prevInAEL2.WindDelta != 0)
						{
							IntPoint intPoint = new IntPoint(e.Curr);
							OutPt op = AddOutPt(prevInAEL2, intPoint);
							OutPt op2 = AddOutPt(e, intPoint);
							AddJoin(op, op2, intPoint);
						}
					}
					e = e.NextInAEL;
				}
			}
			ProcessHorizontals(true);
			for (e = m_ActiveEdges; e != null; e = e.NextInAEL)
			{
				if (IsIntermediate(e, (double)topY))
				{
					OutPt outPt = null;
					if (e.OutIdx >= 0)
					{
						outPt = AddOutPt(e, e.Top);
					}
					UpdateEdgeIntoAEL(ref e);
					TEdge prevInAEL3 = e.PrevInAEL;
					TEdge nextInAEL = e.NextInAEL;
					if (prevInAEL3 != null && prevInAEL3.Curr.X == e.Bot.X && prevInAEL3.Curr.Y == e.Bot.Y && outPt != null && prevInAEL3.OutIdx >= 0 && prevInAEL3.Curr.Y > prevInAEL3.Top.Y && ClipperBase.SlopesEqual(e, prevInAEL3, m_UseFullRange) && e.WindDelta != 0 && prevInAEL3.WindDelta != 0)
					{
						OutPt op3 = AddOutPt(prevInAEL3, e.Bot);
						AddJoin(outPt, op3, e.Top);
					}
					else if (nextInAEL != null && nextInAEL.Curr.X == e.Bot.X && nextInAEL.Curr.Y == e.Bot.Y && outPt != null && nextInAEL.OutIdx >= 0 && nextInAEL.Curr.Y > nextInAEL.Top.Y && ClipperBase.SlopesEqual(e, nextInAEL, m_UseFullRange) && e.WindDelta != 0 && nextInAEL.WindDelta != 0)
					{
						OutPt op4 = AddOutPt(nextInAEL, e.Bot);
						AddJoin(outPt, op4, e.Top);
					}
				}
			}
		}

		private void DoMaxima(TEdge e)
		{
			TEdge maximaPair = GetMaximaPair(e);
			if (maximaPair == null)
			{
				if (e.OutIdx >= 0)
				{
					AddOutPt(e, e.Top);
				}
				DeleteFromAEL(e);
			}
			else
			{
				TEdge nextInAEL = e.NextInAEL;
				while (nextInAEL != null && nextInAEL != maximaPair)
				{
					IntersectEdges(e, nextInAEL, e.Top);
					SwapPositionsInAEL(e, nextInAEL);
					nextInAEL = e.NextInAEL;
				}
				if (e.OutIdx == -1 && maximaPair.OutIdx == -1)
				{
					DeleteFromAEL(e);
					DeleteFromAEL(maximaPair);
				}
				else
				{
					if (e.OutIdx < 0 || maximaPair.OutIdx < 0)
					{
						throw new ClipperException("DoMaxima error");
					}
					if (e.OutIdx >= 0)
					{
						AddLocalMaxPoly(e, maximaPair, e.Top);
					}
					DeleteFromAEL(e);
					DeleteFromAEL(maximaPair);
				}
			}
		}

		public static void ReversePaths(List<List<IntPoint>> polys)
		{
			foreach (List<IntPoint> poly in polys)
			{
				poly.Reverse();
			}
		}

		public static bool Orientation(List<IntPoint> poly)
		{
			return Area(poly) >= 0.0;
		}

		private int PointCount(OutPt pts)
		{
			if (pts == null)
			{
				return 0;
			}
			int num = 0;
			OutPt outPt = pts;
			do
			{
				num++;
				outPt = outPt.Next;
			}
			while (outPt != pts);
			return num;
		}

		private void BuildResult(List<List<IntPoint>> polyg)
		{
			polyg.Clear();
			polyg.Capacity = m_PolyOuts.Count;
			for (int i = 0; i < m_PolyOuts.Count; i++)
			{
				OutRec outRec = m_PolyOuts[i];
				if (outRec.Pts != null)
				{
					OutPt prev = outRec.Pts.Prev;
					int num = PointCount(prev);
					if (num >= 2)
					{
						List<IntPoint> list = new List<IntPoint>(num);
						for (int j = 0; j < num; j++)
						{
							list.Add(prev.Pt);
							prev = prev.Prev;
						}
						polyg.Add(list);
					}
				}
			}
		}

		private void BuildResult2(PolyTree polytree)
		{
			polytree.Clear();
			polytree.m_AllPolys.Capacity = m_PolyOuts.Count;
			for (int i = 0; i < m_PolyOuts.Count; i++)
			{
				OutRec outRec = m_PolyOuts[i];
				int num = PointCount(outRec.Pts);
				if ((!outRec.IsOpen || num >= 2) && (outRec.IsOpen || num >= 3))
				{
					FixHoleLinkage(outRec);
					PolyNode polyNode = new PolyNode();
					polytree.m_AllPolys.Add(polyNode);
					outRec.PolyNode = polyNode;
					polyNode.m_polygon.Capacity = num;
					OutPt prev = outRec.Pts.Prev;
					for (int j = 0; j < num; j++)
					{
						polyNode.m_polygon.Add(prev.Pt);
						prev = prev.Prev;
					}
				}
			}
			polytree.m_Childs.Capacity = m_PolyOuts.Count;
			for (int k = 0; k < m_PolyOuts.Count; k++)
			{
				OutRec outRec2 = m_PolyOuts[k];
				if (outRec2.PolyNode != null)
				{
					if (outRec2.IsOpen)
					{
						outRec2.PolyNode.IsOpen = true;
						polytree.AddChild(outRec2.PolyNode);
					}
					else if (outRec2.FirstLeft != null && outRec2.FirstLeft.PolyNode != null)
					{
						outRec2.FirstLeft.PolyNode.AddChild(outRec2.PolyNode);
					}
					else
					{
						polytree.AddChild(outRec2.PolyNode);
					}
				}
			}
		}

		private void FixupOutPolygon(OutRec outRec)
		{
			OutPt outPt = null;
			outRec.BottomPt = null;
			OutPt outPt2 = outRec.Pts;
			while (true)
			{
				if (outPt2.Prev == outPt2 || outPt2.Prev == outPt2.Next)
				{
					outRec.Pts = null;
					return;
				}
				if (outPt2.Pt == outPt2.Next.Pt || outPt2.Pt == outPt2.Prev.Pt || (ClipperBase.SlopesEqual(outPt2.Prev.Pt, outPt2.Pt, outPt2.Next.Pt, m_UseFullRange) && (!base.PreserveCollinear || !Pt2IsBetweenPt1AndPt3(outPt2.Prev.Pt, outPt2.Pt, outPt2.Next.Pt))))
				{
					outPt = null;
					outPt2.Prev.Next = outPt2.Next;
					outPt2.Next.Prev = outPt2.Prev;
					outPt2 = outPt2.Prev;
				}
				else
				{
					if (outPt2 == outPt)
					{
						break;
					}
					if (outPt == null)
					{
						outPt = outPt2;
					}
					outPt2 = outPt2.Next;
				}
			}
			outRec.Pts = outPt2;
		}

		private OutPt DupOutPt(OutPt outPt, bool InsertAfter)
		{
			OutPt outPt2 = new OutPt();
			outPt2.Pt = outPt.Pt;
			outPt2.Idx = outPt.Idx;
			if (InsertAfter)
			{
				outPt2.Next = outPt.Next;
				outPt2.Prev = outPt;
				outPt.Next.Prev = outPt2;
				outPt.Next = outPt2;
			}
			else
			{
				outPt2.Prev = outPt.Prev;
				outPt2.Next = outPt;
				outPt.Prev.Next = outPt2;
				outPt.Prev = outPt2;
			}
			return outPt2;
		}

		private bool GetOverlap(long a1, long a2, long b1, long b2, out long Left, out long Right)
		{
			if (a1 < a2)
			{
				if (b1 < b2)
				{
					Left = Math.Max(a1, b1);
					Right = Math.Min(a2, b2);
				}
				else
				{
					Left = Math.Max(a1, b2);
					Right = Math.Min(a2, b1);
				}
			}
			else if (b1 < b2)
			{
				Left = Math.Max(a2, b1);
				Right = Math.Min(a1, b2);
			}
			else
			{
				Left = Math.Max(a2, b2);
				Right = Math.Min(a1, b1);
			}
			return Left < Right;
		}

		private bool JoinHorz(OutPt op1, OutPt op1b, OutPt op2, OutPt op2b, IntPoint Pt, bool DiscardLeft)
		{
			Direction direction = (op1.Pt.X <= op1b.Pt.X) ? Direction.dLeftToRight : Direction.dRightToLeft;
			Direction direction2 = (op2.Pt.X <= op2b.Pt.X) ? Direction.dLeftToRight : Direction.dRightToLeft;
			if (direction == direction2)
			{
				return false;
			}
			if (direction == Direction.dLeftToRight)
			{
				while (op1.Next.Pt.X <= Pt.X && op1.Next.Pt.X >= op1.Pt.X && op1.Next.Pt.Y == Pt.Y)
				{
					op1 = op1.Next;
				}
				if (DiscardLeft && op1.Pt.X != Pt.X)
				{
					op1 = op1.Next;
				}
				op1b = DupOutPt(op1, !DiscardLeft);
				if (op1b.Pt != Pt)
				{
					op1 = op1b;
					op1.Pt = Pt;
					op1b = DupOutPt(op1, !DiscardLeft);
				}
			}
			else
			{
				while (op1.Next.Pt.X >= Pt.X && op1.Next.Pt.X <= op1.Pt.X && op1.Next.Pt.Y == Pt.Y)
				{
					op1 = op1.Next;
				}
				if (!DiscardLeft && op1.Pt.X != Pt.X)
				{
					op1 = op1.Next;
				}
				op1b = DupOutPt(op1, DiscardLeft);
				if (op1b.Pt != Pt)
				{
					op1 = op1b;
					op1.Pt = Pt;
					op1b = DupOutPt(op1, DiscardLeft);
				}
			}
			if (direction2 == Direction.dLeftToRight)
			{
				while (op2.Next.Pt.X <= Pt.X && op2.Next.Pt.X >= op2.Pt.X && op2.Next.Pt.Y == Pt.Y)
				{
					op2 = op2.Next;
				}
				if (DiscardLeft && op2.Pt.X != Pt.X)
				{
					op2 = op2.Next;
				}
				op2b = DupOutPt(op2, !DiscardLeft);
				if (op2b.Pt != Pt)
				{
					op2 = op2b;
					op2.Pt = Pt;
					op2b = DupOutPt(op2, !DiscardLeft);
				}
			}
			else
			{
				while (op2.Next.Pt.X >= Pt.X && op2.Next.Pt.X <= op2.Pt.X && op2.Next.Pt.Y == Pt.Y)
				{
					op2 = op2.Next;
				}
				if (!DiscardLeft && op2.Pt.X != Pt.X)
				{
					op2 = op2.Next;
				}
				op2b = DupOutPt(op2, DiscardLeft);
				if (op2b.Pt != Pt)
				{
					op2 = op2b;
					op2.Pt = Pt;
					op2b = DupOutPt(op2, DiscardLeft);
				}
			}
			if (direction == Direction.dLeftToRight == DiscardLeft)
			{
				op1.Prev = op2;
				op2.Next = op1;
				op1b.Next = op2b;
				op2b.Prev = op1b;
			}
			else
			{
				op1.Next = op2;
				op2.Prev = op1;
				op1b.Prev = op2b;
				op2b.Next = op1b;
			}
			return true;
		}

		private bool JoinPoints(Join j, OutRec outRec1, OutRec outRec2)
		{
			OutPt outPt = j.OutPt1;
			OutPt outPt2 = j.OutPt2;
			bool flag = j.OutPt1.Pt.Y == j.OffPt.Y;
			OutPt next;
			OutPt next2;
			if (flag && j.OffPt == j.OutPt1.Pt && j.OffPt == j.OutPt2.Pt)
			{
				if (outRec1 != outRec2)
				{
					return false;
				}
				next = j.OutPt1.Next;
				while (next != outPt && next.Pt == j.OffPt)
				{
					next = next.Next;
				}
				bool flag2 = next.Pt.Y > j.OffPt.Y;
				next2 = j.OutPt2.Next;
				while (next2 != outPt2 && next2.Pt == j.OffPt)
				{
					next2 = next2.Next;
				}
				bool flag3 = next2.Pt.Y > j.OffPt.Y;
				if (flag2 == flag3)
				{
					return false;
				}
				if (flag2)
				{
					next = DupOutPt(outPt, false);
					next2 = DupOutPt(outPt2, true);
					outPt.Prev = outPt2;
					outPt2.Next = outPt;
					next.Next = next2;
					next2.Prev = next;
					j.OutPt1 = outPt;
					j.OutPt2 = next;
					return true;
				}
				next = DupOutPt(outPt, true);
				next2 = DupOutPt(outPt2, false);
				outPt.Next = outPt2;
				outPt2.Prev = outPt;
				next.Prev = next2;
				next2.Next = next;
				j.OutPt1 = outPt;
				j.OutPt2 = next;
				return true;
			}
			if (flag)
			{
				next = outPt;
				while (outPt.Prev.Pt.Y == outPt.Pt.Y && outPt.Prev != next && outPt.Prev != outPt2)
				{
					outPt = outPt.Prev;
				}
				while (next.Next.Pt.Y == next.Pt.Y && next.Next != outPt && next.Next != outPt2)
				{
					next = next.Next;
				}
				if (next.Next == outPt || next.Next == outPt2)
				{
					return false;
				}
				next2 = outPt2;
				while (outPt2.Prev.Pt.Y == outPt2.Pt.Y && outPt2.Prev != next2 && outPt2.Prev != next)
				{
					outPt2 = outPt2.Prev;
				}
				while (next2.Next.Pt.Y == next2.Pt.Y && next2.Next != outPt2 && next2.Next != outPt)
				{
					next2 = next2.Next;
				}
				if (next2.Next == outPt2 || next2.Next == outPt)
				{
					return false;
				}
				if (!GetOverlap(outPt.Pt.X, next.Pt.X, outPt2.Pt.X, next2.Pt.X, out long Left, out long Right))
				{
					return false;
				}
				IntPoint pt;
				bool discardLeft;
				if (outPt.Pt.X >= Left && outPt.Pt.X <= Right)
				{
					pt = outPt.Pt;
					discardLeft = (outPt.Pt.X > next.Pt.X);
				}
				else if (outPt2.Pt.X >= Left && outPt2.Pt.X <= Right)
				{
					pt = outPt2.Pt;
					discardLeft = (outPt2.Pt.X > next2.Pt.X);
				}
				else if (next.Pt.X >= Left && next.Pt.X <= Right)
				{
					pt = next.Pt;
					discardLeft = (next.Pt.X > outPt.Pt.X);
				}
				else
				{
					pt = next2.Pt;
					discardLeft = (next2.Pt.X > outPt2.Pt.X);
				}
				j.OutPt1 = outPt;
				j.OutPt2 = outPt2;
				return JoinHorz(outPt, next, outPt2, next2, pt, discardLeft);
			}
			next = outPt.Next;
			while (next.Pt == outPt.Pt && next != outPt)
			{
				next = next.Next;
			}
			bool flag4 = next.Pt.Y > outPt.Pt.Y || !ClipperBase.SlopesEqual(outPt.Pt, next.Pt, j.OffPt, m_UseFullRange);
			if (flag4)
			{
				next = outPt.Prev;
				while (next.Pt == outPt.Pt && next != outPt)
				{
					next = next.Prev;
				}
				if (next.Pt.Y > outPt.Pt.Y || !ClipperBase.SlopesEqual(outPt.Pt, next.Pt, j.OffPt, m_UseFullRange))
				{
					return false;
				}
			}
			next2 = outPt2.Next;
			while (next2.Pt == outPt2.Pt && next2 != outPt2)
			{
				next2 = next2.Next;
			}
			bool flag5 = next2.Pt.Y > outPt2.Pt.Y || !ClipperBase.SlopesEqual(outPt2.Pt, next2.Pt, j.OffPt, m_UseFullRange);
			if (flag5)
			{
				next2 = outPt2.Prev;
				while (next2.Pt == outPt2.Pt && next2 != outPt2)
				{
					next2 = next2.Prev;
				}
				if (next2.Pt.Y > outPt2.Pt.Y || !ClipperBase.SlopesEqual(outPt2.Pt, next2.Pt, j.OffPt, m_UseFullRange))
				{
					return false;
				}
			}
			if (next == outPt || next2 == outPt2 || next == next2 || (outRec1 == outRec2 && flag4 == flag5))
			{
				return false;
			}
			if (flag4)
			{
				next = DupOutPt(outPt, false);
				next2 = DupOutPt(outPt2, true);
				outPt.Prev = outPt2;
				outPt2.Next = outPt;
				next.Next = next2;
				next2.Prev = next;
				j.OutPt1 = outPt;
				j.OutPt2 = next;
				return true;
			}
			next = DupOutPt(outPt, true);
			next2 = DupOutPt(outPt2, false);
			outPt.Next = outPt2;
			outPt2.Prev = outPt;
			next.Prev = next2;
			next2.Next = next;
			j.OutPt1 = outPt;
			j.OutPt2 = next;
			return true;
		}

		public static int PointInPolygon(IntPoint pt, List<IntPoint> path)
		{
			int num = 0;
			int count = path.Count;
			if (count < 3)
			{
				return 0;
			}
			IntPoint intPoint = path[0];
			for (int i = 1; i <= count; i++)
			{
				IntPoint intPoint2 = (i != count) ? path[i] : path[0];
				if (intPoint2.Y == pt.Y && (intPoint2.X == pt.X || (intPoint.Y == pt.Y && intPoint2.X > pt.X == intPoint.X < pt.X)))
				{
					return -1;
				}
				if (intPoint.Y < pt.Y != intPoint2.Y < pt.Y)
				{
					if (intPoint.X >= pt.X)
					{
						if (intPoint2.X > pt.X)
						{
							num = 1 - num;
						}
						else
						{
							double num2 = (double)(intPoint.X - pt.X) * (double)(intPoint2.Y - pt.Y) - (double)(intPoint2.X - pt.X) * (double)(intPoint.Y - pt.Y);
							if (num2 == 0.0)
							{
								return -1;
							}
							if (num2 > 0.0 == intPoint2.Y > intPoint.Y)
							{
								num = 1 - num;
							}
						}
					}
					else if (intPoint2.X > pt.X)
					{
						double num3 = (double)(intPoint.X - pt.X) * (double)(intPoint2.Y - pt.Y) - (double)(intPoint2.X - pt.X) * (double)(intPoint.Y - pt.Y);
						if (num3 == 0.0)
						{
							return -1;
						}
						if (num3 > 0.0 == intPoint2.Y > intPoint.Y)
						{
							num = 1 - num;
						}
					}
				}
				intPoint = intPoint2;
			}
			return num;
		}

		private static int PointInPolygon(IntPoint pt, OutPt op)
		{
			int num = 0;
			OutPt outPt = op;
			long x = pt.X;
			long y = pt.Y;
			long num2 = op.Pt.X;
			long num3 = op.Pt.Y;
			do
			{
				op = op.Next;
				long x2 = op.Pt.X;
				long y2 = op.Pt.Y;
				if (y2 == y && (x2 == x || (num3 == y && x2 > x == num2 < x)))
				{
					return -1;
				}
				if (num3 < y != y2 < y)
				{
					if (num2 >= x)
					{
						if (x2 > x)
						{
							num = 1 - num;
						}
						else
						{
							double num4 = (double)(num2 - x) * (double)(y2 - y) - (double)(x2 - x) * (double)(num3 - y);
							if (num4 == 0.0)
							{
								return -1;
							}
							if (num4 > 0.0 == y2 > num3)
							{
								num = 1 - num;
							}
						}
					}
					else if (x2 > x)
					{
						double num5 = (double)(num2 - x) * (double)(y2 - y) - (double)(x2 - x) * (double)(num3 - y);
						if (num5 == 0.0)
						{
							return -1;
						}
						if (num5 > 0.0 == y2 > num3)
						{
							num = 1 - num;
						}
					}
				}
				num2 = x2;
				num3 = y2;
			}
			while (outPt != op);
			return num;
		}

		private static bool Poly2ContainsPoly1(OutPt outPt1, OutPt outPt2)
		{
			OutPt outPt3 = outPt1;
			do
			{
				int num = PointInPolygon(outPt3.Pt, outPt2);
				if (num >= 0)
				{
					return num > 0;
				}
				outPt3 = outPt3.Next;
			}
			while (outPt3 != outPt1);
			return true;
		}

		private void FixupFirstLefts1(OutRec OldOutRec, OutRec NewOutRec)
		{
			for (int i = 0; i < m_PolyOuts.Count; i++)
			{
				OutRec outRec = m_PolyOuts[i];
				if (outRec.Pts != null && outRec.FirstLeft != null)
				{
					OutRec outRec2 = ParseFirstLeft(outRec.FirstLeft);
					if (outRec2 == OldOutRec && Poly2ContainsPoly1(outRec.Pts, NewOutRec.Pts))
					{
						outRec.FirstLeft = NewOutRec;
					}
				}
			}
		}

		private void FixupFirstLefts2(OutRec OldOutRec, OutRec NewOutRec)
		{
			foreach (OutRec polyOut in m_PolyOuts)
			{
				if (polyOut.FirstLeft == OldOutRec)
				{
					polyOut.FirstLeft = NewOutRec;
				}
			}
		}

		private static OutRec ParseFirstLeft(OutRec FirstLeft)
		{
			while (FirstLeft != null && FirstLeft.Pts == null)
			{
				FirstLeft = FirstLeft.FirstLeft;
			}
			return FirstLeft;
		}

		private void JoinCommonEdges()
		{
			for (int i = 0; i < m_Joins.Count; i++)
			{
				Join join = m_Joins[i];
				OutRec outRec = GetOutRec(join.OutPt1.Idx);
				OutRec outRec2 = GetOutRec(join.OutPt2.Idx);
				if (outRec.Pts != null && outRec2.Pts != null)
				{
					OutRec outRec3 = (outRec == outRec2) ? outRec : (Param1RightOfParam2(outRec, outRec2) ? outRec2 : ((!Param1RightOfParam2(outRec2, outRec)) ? GetLowermostRec(outRec, outRec2) : outRec));
					if (JoinPoints(join, outRec, outRec2))
					{
						if (outRec == outRec2)
						{
							outRec.Pts = join.OutPt1;
							outRec.BottomPt = null;
							outRec2 = CreateOutRec();
							outRec2.Pts = join.OutPt2;
							UpdateOutPtIdxs(outRec2);
							if (m_UsingPolyTree)
							{
								for (int j = 0; j < m_PolyOuts.Count - 1; j++)
								{
									OutRec outRec4 = m_PolyOuts[j];
									if (outRec4.Pts != null && ParseFirstLeft(outRec4.FirstLeft) == outRec && outRec4.IsHole != outRec.IsHole && Poly2ContainsPoly1(outRec4.Pts, join.OutPt2))
									{
										outRec4.FirstLeft = outRec2;
									}
								}
							}
							if (Poly2ContainsPoly1(outRec2.Pts, outRec.Pts))
							{
								outRec2.IsHole = !outRec.IsHole;
								outRec2.FirstLeft = outRec;
								if (m_UsingPolyTree)
								{
									FixupFirstLefts2(outRec2, outRec);
								}
								if ((outRec2.IsHole ^ ReverseSolution) == Area(outRec2) > 0.0)
								{
									ReversePolyPtLinks(outRec2.Pts);
								}
							}
							else if (Poly2ContainsPoly1(outRec.Pts, outRec2.Pts))
							{
								outRec2.IsHole = outRec.IsHole;
								outRec.IsHole = !outRec2.IsHole;
								outRec2.FirstLeft = outRec.FirstLeft;
								outRec.FirstLeft = outRec2;
								if (m_UsingPolyTree)
								{
									FixupFirstLefts2(outRec, outRec2);
								}
								if ((outRec.IsHole ^ ReverseSolution) == Area(outRec) > 0.0)
								{
									ReversePolyPtLinks(outRec.Pts);
								}
							}
							else
							{
								outRec2.IsHole = outRec.IsHole;
								outRec2.FirstLeft = outRec.FirstLeft;
								if (m_UsingPolyTree)
								{
									FixupFirstLefts1(outRec, outRec2);
								}
							}
						}
						else
						{
							outRec2.Pts = null;
							outRec2.BottomPt = null;
							outRec2.Idx = outRec.Idx;
							outRec.IsHole = outRec3.IsHole;
							if (outRec3 == outRec2)
							{
								outRec.FirstLeft = outRec2.FirstLeft;
							}
							outRec2.FirstLeft = outRec;
							if (m_UsingPolyTree)
							{
								FixupFirstLefts2(outRec2, outRec);
							}
						}
					}
				}
			}
		}

		private void UpdateOutPtIdxs(OutRec outrec)
		{
			OutPt outPt = outrec.Pts;
			do
			{
				outPt.Idx = outrec.Idx;
				outPt = outPt.Prev;
			}
			while (outPt != outrec.Pts);
		}

		private void DoSimplePolygons()
		{
			int num = 0;
			while (num < m_PolyOuts.Count)
			{
				OutRec outRec = m_PolyOuts[num++];
				OutPt outPt = outRec.Pts;
				if (outPt != null && !outRec.IsOpen)
				{
					do
					{
						for (OutPt outPt2 = outPt.Next; outPt2 != outRec.Pts; outPt2 = outPt2.Next)
						{
							if (outPt.Pt == outPt2.Pt && outPt2.Next != outPt && outPt2.Prev != outPt)
							{
								OutPt prev = outPt.Prev;
								(outPt.Prev = outPt2.Prev).Next = outPt;
								outPt2.Prev = prev;
								prev.Next = outPt2;
								outRec.Pts = outPt;
								OutRec outRec2 = CreateOutRec();
								outRec2.Pts = outPt2;
								UpdateOutPtIdxs(outRec2);
								if (Poly2ContainsPoly1(outRec2.Pts, outRec.Pts))
								{
									outRec2.IsHole = !outRec.IsHole;
									outRec2.FirstLeft = outRec;
									if (m_UsingPolyTree)
									{
										FixupFirstLefts2(outRec2, outRec);
									}
								}
								else if (Poly2ContainsPoly1(outRec.Pts, outRec2.Pts))
								{
									outRec2.IsHole = outRec.IsHole;
									outRec.IsHole = !outRec2.IsHole;
									outRec2.FirstLeft = outRec.FirstLeft;
									outRec.FirstLeft = outRec2;
									if (m_UsingPolyTree)
									{
										FixupFirstLefts2(outRec, outRec2);
									}
								}
								else
								{
									outRec2.IsHole = outRec.IsHole;
									outRec2.FirstLeft = outRec.FirstLeft;
									if (m_UsingPolyTree)
									{
										FixupFirstLefts1(outRec, outRec2);
									}
								}
								outPt2 = outPt;
							}
						}
						outPt = outPt.Next;
					}
					while (outPt != outRec.Pts);
				}
			}
		}

		public static double Area(List<IntPoint> poly)
		{
			int count = poly.Count;
			if (count < 3)
			{
				return 0.0;
			}
			double num = 0.0;
			int i = 0;
			int index = count - 1;
			for (; i < count; i++)
			{
				double num2 = num;
				IntPoint intPoint = poly[index];
				double num3 = (double)intPoint.X;
				IntPoint intPoint2 = poly[i];
				double num4 = num3 + (double)intPoint2.X;
				IntPoint intPoint3 = poly[index];
				double num5 = (double)intPoint3.Y;
				IntPoint intPoint4 = poly[i];
				num = num2 + num4 * (num5 - (double)intPoint4.Y);
				index = i;
			}
			return (0.0 - num) * 0.5;
		}

		private double Area(OutRec outRec)
		{
			OutPt outPt = outRec.Pts;
			if (outPt == null)
			{
				return 0.0;
			}
			double num = 0.0;
			do
			{
				num += (double)(outPt.Prev.Pt.X + outPt.Pt.X) * (double)(outPt.Prev.Pt.Y - outPt.Pt.Y);
				outPt = outPt.Next;
			}
			while (outPt != outRec.Pts);
			return num * 0.5;
		}

		public static List<List<IntPoint>> SimplifyPolygon(List<IntPoint> poly, PolyFillType fillType = PolyFillType.pftEvenOdd)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			Clipper clipper = new Clipper(0);
			clipper.StrictlySimple = true;
			clipper.AddPath(poly, PolyType.ptSubject, true);
			clipper.Execute(ClipType.ctUnion, list, fillType, fillType);
			return list;
		}

		public static List<List<IntPoint>> SimplifyPolygons(List<List<IntPoint>> polys, PolyFillType fillType = PolyFillType.pftEvenOdd)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			Clipper clipper = new Clipper(0);
			clipper.StrictlySimple = true;
			clipper.AddPaths(polys, PolyType.ptSubject, true);
			clipper.Execute(ClipType.ctUnion, list, fillType, fillType);
			return list;
		}

		private static double DistanceSqrd(IntPoint pt1, IntPoint pt2)
		{
			double num = (double)pt1.X - (double)pt2.X;
			double num2 = (double)pt1.Y - (double)pt2.Y;
			return num * num + num2 * num2;
		}

		private static double DistanceFromLineSqrd(IntPoint pt, IntPoint ln1, IntPoint ln2)
		{
			double num = (double)(ln1.Y - ln2.Y);
			double num2 = (double)(ln2.X - ln1.X);
			double num3 = num * (double)ln1.X + num2 * (double)ln1.Y;
			num3 = num * (double)pt.X + num2 * (double)pt.Y - num3;
			return num3 * num3 / (num * num + num2 * num2);
		}

		private static bool SlopesNearCollinear(IntPoint pt1, IntPoint pt2, IntPoint pt3, double distSqrd)
		{
			if (Math.Abs(pt1.X - pt2.X) > Math.Abs(pt1.Y - pt2.Y))
			{
				if (pt1.X > pt2.X == pt1.X < pt3.X)
				{
					return DistanceFromLineSqrd(pt1, pt2, pt3) < distSqrd;
				}
				if (pt2.X > pt1.X == pt2.X < pt3.X)
				{
					return DistanceFromLineSqrd(pt2, pt1, pt3) < distSqrd;
				}
				return DistanceFromLineSqrd(pt3, pt1, pt2) < distSqrd;
			}
			if (pt1.Y > pt2.Y == pt1.Y < pt3.Y)
			{
				return DistanceFromLineSqrd(pt1, pt2, pt3) < distSqrd;
			}
			if (pt2.Y > pt1.Y == pt2.Y < pt3.Y)
			{
				return DistanceFromLineSqrd(pt2, pt1, pt3) < distSqrd;
			}
			return DistanceFromLineSqrd(pt3, pt1, pt2) < distSqrd;
		}

		private static bool PointsAreClose(IntPoint pt1, IntPoint pt2, double distSqrd)
		{
			double num = (double)pt1.X - (double)pt2.X;
			double num2 = (double)pt1.Y - (double)pt2.Y;
			return num * num + num2 * num2 <= distSqrd;
		}

		private static OutPt ExcludeOp(OutPt op)
		{
			OutPt prev = op.Prev;
			prev.Next = op.Next;
			op.Next.Prev = prev;
			prev.Idx = 0;
			return prev;
		}

		public static List<IntPoint> CleanPolygon(List<IntPoint> path, double distance = 1.415)
		{
			int num = path.Count;
			if (num == 0)
			{
				return new List<IntPoint>();
			}
			OutPt[] array = new OutPt[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = new OutPt();
			}
			for (int j = 0; j < num; j++)
			{
				array[j].Pt = path[j];
				array[j].Next = array[(j + 1) % num];
				array[j].Next.Prev = array[j];
				array[j].Idx = 0;
			}
			double distSqrd = distance * distance;
			OutPt outPt = array[0];
			while (outPt.Idx == 0 && outPt.Next != outPt.Prev)
			{
				if (PointsAreClose(outPt.Pt, outPt.Prev.Pt, distSqrd))
				{
					outPt = ExcludeOp(outPt);
					num--;
				}
				else if (PointsAreClose(outPt.Prev.Pt, outPt.Next.Pt, distSqrd))
				{
					ExcludeOp(outPt.Next);
					outPt = ExcludeOp(outPt);
					num -= 2;
				}
				else if (SlopesNearCollinear(outPt.Prev.Pt, outPt.Pt, outPt.Next.Pt, distSqrd))
				{
					outPt = ExcludeOp(outPt);
					num--;
				}
				else
				{
					outPt.Idx = 1;
					outPt = outPt.Next;
				}
			}
			if (num < 3)
			{
				num = 0;
			}
			List<IntPoint> list = new List<IntPoint>(num);
			for (int k = 0; k < num; k++)
			{
				list.Add(outPt.Pt);
				outPt = outPt.Next;
			}
			array = null;
			return list;
		}

		public static List<List<IntPoint>> CleanPolygons(List<List<IntPoint>> polys, double distance = 1.415)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>(polys.Count);
			for (int i = 0; i < polys.Count; i++)
			{
				list.Add(CleanPolygon(polys[i], distance));
			}
			return list;
		}

		internal static List<List<IntPoint>> Minkowski(List<IntPoint> pattern, List<IntPoint> path, bool IsSum, bool IsClosed)
		{
			int num = IsClosed ? 1 : 0;
			int count = pattern.Count;
			int count2 = path.Count;
			List<List<IntPoint>> list = new List<List<IntPoint>>(count2);
			if (IsSum)
			{
				for (int i = 0; i < count2; i++)
				{
					List<IntPoint> list2 = new List<IntPoint>(count);
					foreach (IntPoint item in pattern)
					{
						IntPoint current = item;
						List<IntPoint> list3 = list2;
						IntPoint intPoint = path[i];
						long x = intPoint.X + current.X;
						IntPoint intPoint2 = path[i];
						list3.Add(new IntPoint(x, intPoint2.Y + current.Y));
					}
					list.Add(list2);
				}
			}
			else
			{
				for (int j = 0; j < count2; j++)
				{
					List<IntPoint> list4 = new List<IntPoint>(count);
					foreach (IntPoint item2 in pattern)
					{
						IntPoint current2 = item2;
						List<IntPoint> list5 = list4;
						IntPoint intPoint3 = path[j];
						long x2 = intPoint3.X - current2.X;
						IntPoint intPoint4 = path[j];
						list5.Add(new IntPoint(x2, intPoint4.Y - current2.Y));
					}
					list.Add(list4);
				}
			}
			List<List<IntPoint>> list6 = new List<List<IntPoint>>((count2 + num) * (count + 1));
			for (int k = 0; k < count2 - 1 + num; k++)
			{
				for (int l = 0; l < count; l++)
				{
					List<IntPoint> list7 = new List<IntPoint>(4);
					list7.Add(list[k % count2][l % count]);
					list7.Add(list[(k + 1) % count2][l % count]);
					list7.Add(list[(k + 1) % count2][(l + 1) % count]);
					list7.Add(list[k % count2][(l + 1) % count]);
					if (!Orientation(list7))
					{
						list7.Reverse();
					}
					list6.Add(list7);
				}
			}
			return list6;
		}

		public static List<List<IntPoint>> MinkowskiSum(List<IntPoint> pattern, List<IntPoint> path, bool pathIsClosed)
		{
			List<List<IntPoint>> list = Minkowski(pattern, path, true, pathIsClosed);
			Clipper clipper = new Clipper(0);
			clipper.AddPaths(list, PolyType.ptSubject, true);
			clipper.Execute(ClipType.ctUnion, list, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
			return list;
		}

		private static List<IntPoint> TranslatePath(List<IntPoint> path, IntPoint delta)
		{
			List<IntPoint> list = new List<IntPoint>(path.Count);
			for (int i = 0; i < path.Count; i++)
			{
				List<IntPoint> list2 = list;
				IntPoint intPoint = path[i];
				long x = intPoint.X + delta.X;
				IntPoint intPoint2 = path[i];
				list2.Add(new IntPoint(x, intPoint2.Y + delta.Y));
			}
			return list;
		}

		public static List<List<IntPoint>> MinkowskiSum(List<IntPoint> pattern, List<List<IntPoint>> paths, bool pathIsClosed)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			Clipper clipper = new Clipper(0);
			for (int i = 0; i < paths.Count; i++)
			{
				List<List<IntPoint>> ppg = Minkowski(pattern, paths[i], true, pathIsClosed);
				clipper.AddPaths(ppg, PolyType.ptSubject, true);
				if (pathIsClosed)
				{
					List<IntPoint> pg = TranslatePath(paths[i], pattern[0]);
					clipper.AddPath(pg, PolyType.ptClip, true);
				}
			}
			clipper.Execute(ClipType.ctUnion, list, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
			return list;
		}

		public static List<List<IntPoint>> MinkowskiDiff(List<IntPoint> poly1, List<IntPoint> poly2)
		{
			List<List<IntPoint>> list = Minkowski(poly1, poly2, false, true);
			Clipper clipper = new Clipper(0);
			clipper.AddPaths(list, PolyType.ptSubject, true);
			clipper.Execute(ClipType.ctUnion, list, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
			return list;
		}

		public static List<List<IntPoint>> PolyTreeToPaths(PolyTree polytree)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			list.Capacity = polytree.Total;
			AddPolyNodeToPaths(polytree, NodeType.ntAny, list);
			return list;
		}

		internal static void AddPolyNodeToPaths(PolyNode polynode, NodeType nt, List<List<IntPoint>> paths)
		{
			bool flag = true;
			switch (nt)
			{
			case NodeType.ntOpen:
				return;
			case NodeType.ntClosed:
				flag = !polynode.IsOpen;
				break;
			}
			if (polynode.m_polygon.Count > 0 && flag)
			{
				paths.Add(polynode.m_polygon);
			}
			foreach (PolyNode child in polynode.Childs)
			{
				AddPolyNodeToPaths(child, nt, paths);
			}
		}

		public static List<List<IntPoint>> OpenPathsFromPolyTree(PolyTree polytree)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			list.Capacity = polytree.ChildCount;
			for (int i = 0; i < polytree.ChildCount; i++)
			{
				if (polytree.Childs[i].IsOpen)
				{
					list.Add(polytree.Childs[i].m_polygon);
				}
			}
			return list;
		}

		public static List<List<IntPoint>> ClosedPathsFromPolyTree(PolyTree polytree)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			list.Capacity = polytree.Total;
			AddPolyNodeToPaths(polytree, NodeType.ntClosed, list);
			return list;
		}
	}
}
