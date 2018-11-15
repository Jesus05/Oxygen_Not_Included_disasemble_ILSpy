namespace MIConvexHull
{
	internal sealed class FaceList
	{
		private ConvexFaceInternal last;

		public ConvexFaceInternal First
		{
			get;
			private set;
		}

		private void AddFirst(ConvexFaceInternal face)
		{
			face.InList = true;
			First.Previous = face;
			face.Next = First;
			First = face;
		}

		public void Add(ConvexFaceInternal face)
		{
			if (face.InList)
			{
				if (First.VerticesBeyond.Count < face.VerticesBeyond.Count)
				{
					Remove(face);
					AddFirst(face);
				}
			}
			else
			{
				face.InList = true;
				if (First != null && First.VerticesBeyond.Count < face.VerticesBeyond.Count)
				{
					First.Previous = face;
					face.Next = First;
					First = face;
				}
				else
				{
					if (last != null)
					{
						last.Next = face;
					}
					face.Previous = last;
					last = face;
					if (First == null)
					{
						First = face;
					}
				}
			}
		}

		public void Remove(ConvexFaceInternal face)
		{
			if (face.InList)
			{
				face.InList = false;
				if (face.Previous != null)
				{
					face.Previous.Next = face.Next;
				}
				else if (face.Previous == null)
				{
					First = face.Next;
				}
				if (face.Next != null)
				{
					face.Next.Previous = face.Previous;
				}
				else if (face.Next == null)
				{
					last = face.Previous;
				}
				face.Next = null;
				face.Previous = null;
			}
		}
	}
}
