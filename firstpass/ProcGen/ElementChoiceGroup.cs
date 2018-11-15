using KSerialization.Converters;
using System.Collections.Generic;

namespace ProcGen
{
	public class ElementChoiceGroup<T>
	{
		[StringEnumConverter]
		public Room.Selection selectionMethod
		{
			get;
			private set;
		}

		public List<T> choices
		{
			get;
			private set;
		}

		public ElementChoiceGroup()
		{
			choices = new List<T>();
		}

		public ElementChoiceGroup(List<T> choices, Room.Selection selectionMethod)
		{
			this.choices = choices;
			this.selectionMethod = selectionMethod;
		}
	}
}
