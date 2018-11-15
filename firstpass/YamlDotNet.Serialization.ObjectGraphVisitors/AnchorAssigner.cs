using System;
using System.Collections.Generic;
using System.Globalization;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
	public sealed class AnchorAssigner : PreProcessingPhaseObjectGraphVisitorSkeleton, IAliasProvider
	{
		private class AnchorAssignment
		{
			public string Anchor;
		}

		private readonly IDictionary<object, AnchorAssignment> assignments = new Dictionary<object, AnchorAssignment>();

		private uint nextId;

		public AnchorAssigner(IEnumerable<IYamlTypeConverter> typeConverters)
			: base(typeConverters)
		{
		}

		protected override bool Enter(IObjectDescriptor value)
		{
			if (value.Value != null && assignments.TryGetValue(value.Value, out AnchorAssignment value2))
			{
				if (value2.Anchor == null)
				{
					value2.Anchor = "o" + nextId.ToString(CultureInfo.InvariantCulture);
					nextId++;
				}
				return false;
			}
			return true;
		}

		protected override bool EnterMapping(IObjectDescriptor key, IObjectDescriptor value)
		{
			return true;
		}

		protected override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value)
		{
			return true;
		}

		protected override void VisitScalar(IObjectDescriptor scalar)
		{
		}

		protected override void VisitMappingStart(IObjectDescriptor mapping, Type keyType, Type valueType)
		{
			VisitObject(mapping);
		}

		protected override void VisitMappingEnd(IObjectDescriptor mapping)
		{
		}

		protected override void VisitSequenceStart(IObjectDescriptor sequence, Type elementType)
		{
			VisitObject(sequence);
		}

		protected override void VisitSequenceEnd(IObjectDescriptor sequence)
		{
		}

		private void VisitObject(IObjectDescriptor value)
		{
			if (value.Value != null)
			{
				assignments.Add(value.Value, new AnchorAssignment());
			}
		}

		string IAliasProvider.GetAlias(object target)
		{
			if (target != null && assignments.TryGetValue(target, out AnchorAssignment value))
			{
				return value.Anchor;
			}
			return null;
		}
	}
}
