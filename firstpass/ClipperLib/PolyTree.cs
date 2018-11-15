using System.Collections.Generic;

namespace ClipperLib
{
	public class PolyTree : PolyNode
	{
		internal List<PolyNode> m_AllPolys = new List<PolyNode>();

		public int Total
		{
			get
			{
				int num = m_AllPolys.Count;
				if (num > 0 && m_Childs[0] != m_AllPolys[0])
				{
					num--;
				}
				return num;
			}
		}

		~PolyTree()
		{
			Clear();
		}

		public void Clear()
		{
			for (int i = 0; i < m_AllPolys.Count; i++)
			{
				m_AllPolys[i] = null;
			}
			m_AllPolys.Clear();
			m_Childs.Clear();
		}

		public PolyNode GetFirst()
		{
			if (m_Childs.Count > 0)
			{
				return m_Childs[0];
			}
			return null;
		}
	}
}
