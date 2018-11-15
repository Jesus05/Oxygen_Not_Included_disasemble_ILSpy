using System;

namespace YamlDotNet.Core
{
	[Serializable]
	internal class SimpleKey
	{
		private readonly Cursor cursor;

		public bool IsPossible
		{
			get;
			set;
		}

		public bool IsRequired
		{
			get;
			private set;
		}

		public int TokenNumber
		{
			get;
			private set;
		}

		public int Index => cursor.Index;

		public int Line => cursor.Line;

		public int LineOffset => cursor.LineOffset;

		public Mark Mark => cursor.Mark();

		public SimpleKey()
		{
			cursor = new Cursor();
		}

		public SimpleKey(bool isPossible, bool isRequired, int tokenNumber, Cursor cursor)
		{
			IsPossible = isPossible;
			IsRequired = isRequired;
			TokenNumber = tokenNumber;
			this.cursor = new Cursor(cursor);
		}
	}
}
