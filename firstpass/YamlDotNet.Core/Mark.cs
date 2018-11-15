using System;

namespace YamlDotNet.Core
{
	[Serializable]
	public class Mark : IEquatable<Mark>, IComparable<Mark>, IComparable
	{
		public static readonly Mark Empty = new Mark();

		public int Index
		{
			get;
			private set;
		}

		public int Line
		{
			get;
			private set;
		}

		public int Column
		{
			get;
			private set;
		}

		public Mark()
		{
			Line = 1;
			Column = 1;
		}

		public Mark(int index, int line, int column)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "Index must be greater than or equal to zero.");
			}
			if (line < 1)
			{
				throw new ArgumentOutOfRangeException("line", "Line must be greater than or equal to 1.");
			}
			if (column < 1)
			{
				throw new ArgumentOutOfRangeException("column", "Column must be greater than or equal to 1.");
			}
			Index = index;
			Line = line;
			Column = column;
		}

		public override string ToString()
		{
			return $"Line: {Line}, Col: {Column}, Idx: {Index}";
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Mark);
		}

		public bool Equals(Mark other)
		{
			return other != null && Index == other.Index && Line == other.Line && Column == other.Column;
		}

		public override int GetHashCode()
		{
			return HashCode.CombineHashCodes(Index.GetHashCode(), HashCode.CombineHashCodes(Line.GetHashCode(), Column.GetHashCode()));
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			return CompareTo(obj as Mark);
		}

		public int CompareTo(Mark other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			int num = Line.CompareTo(other.Line);
			if (num == 0)
			{
				num = Column.CompareTo(other.Column);
			}
			return num;
		}
	}
}
