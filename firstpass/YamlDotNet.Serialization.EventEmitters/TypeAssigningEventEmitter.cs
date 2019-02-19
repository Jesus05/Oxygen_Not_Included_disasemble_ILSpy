using System;
using System.Collections.Generic;
using System.Globalization;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.EventEmitters
{
	public sealed class TypeAssigningEventEmitter : ChainedEventEmitter
	{
		private readonly bool requireTagWhenStaticAndActualTypesAreDifferent;

		private IDictionary<Type, string> tagMappings;

		public TypeAssigningEventEmitter(IEventEmitter nextEmitter, bool requireTagWhenStaticAndActualTypesAreDifferent, IDictionary<Type, string> tagMappings)
			: base(nextEmitter)
		{
			this.requireTagWhenStaticAndActualTypesAreDifferent = requireTagWhenStaticAndActualTypesAreDifferent;
			this.tagMappings = tagMappings;
		}

		public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
		{
			ScalarStyle style = ScalarStyle.Plain;
			TypeCode typeCode = (eventInfo.Source.Value != null) ? eventInfo.Source.Type.GetTypeCode() : TypeCode.Empty;
			switch (typeCode)
			{
			case TypeCode.Boolean:
				eventInfo.Tag = "tag:yaml.org,2002:bool";
				eventInfo.RenderedValue = YamlFormatter.FormatBoolean(eventInfo.Source.Value);
				break;
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
				eventInfo.Tag = "tag:yaml.org,2002:int";
				eventInfo.RenderedValue = YamlFormatter.FormatNumber(eventInfo.Source.Value);
				break;
			case TypeCode.Single:
				eventInfo.Tag = "tag:yaml.org,2002:float";
				eventInfo.RenderedValue = YamlFormatter.FormatNumber((float)eventInfo.Source.Value);
				break;
			case TypeCode.Double:
				eventInfo.Tag = "tag:yaml.org,2002:float";
				eventInfo.RenderedValue = YamlFormatter.FormatNumber((double)eventInfo.Source.Value);
				break;
			case TypeCode.Decimal:
				eventInfo.Tag = "tag:yaml.org,2002:float";
				eventInfo.RenderedValue = YamlFormatter.FormatNumber(eventInfo.Source.Value);
				break;
			case TypeCode.Char:
			case TypeCode.String:
				eventInfo.Tag = "tag:yaml.org,2002:str";
				eventInfo.RenderedValue = eventInfo.Source.Value.ToString();
				style = ScalarStyle.Any;
				break;
			case TypeCode.DateTime:
				eventInfo.Tag = "tag:yaml.org,2002:timestamp";
				eventInfo.RenderedValue = YamlFormatter.FormatDateTime(eventInfo.Source.Value);
				break;
			case TypeCode.Empty:
				eventInfo.Tag = "tag:yaml.org,2002:null";
				eventInfo.RenderedValue = string.Empty;
				break;
			default:
				if (eventInfo.Source.Type != typeof(TimeSpan))
				{
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "TypeCode.{0} is not supported.", typeCode));
				}
				eventInfo.RenderedValue = YamlFormatter.FormatTimeSpan(eventInfo.Source.Value);
				break;
			}
			eventInfo.IsPlainImplicit = true;
			if (eventInfo.Style == ScalarStyle.Any)
			{
				eventInfo.Style = style;
			}
			base.Emit(eventInfo, emitter);
		}

		public override void Emit(MappingStartEventInfo eventInfo, IEmitter emitter)
		{
			AssignTypeIfNeeded(eventInfo);
			base.Emit(eventInfo, emitter);
		}

		public override void Emit(SequenceStartEventInfo eventInfo, IEmitter emitter)
		{
			AssignTypeIfNeeded(eventInfo);
			base.Emit(eventInfo, emitter);
		}

		private void AssignTypeIfNeeded(ObjectEventInfo eventInfo)
		{
			string value = null;
			if (tagMappings.TryGetValue(eventInfo.Source.Type, out value))
			{
				eventInfo.Tag = value;
			}
			else if (requireTagWhenStaticAndActualTypesAreDifferent && eventInfo.Source.Value != null && eventInfo.Source.Type != eventInfo.Source.StaticType)
			{
				throw new YamlException("Cannot serialize type " + eventInfo.Source.Type.FullName + " where a " + eventInfo.Source.StaticType.FullName + " was expected because no tag mapping has been registered for " + eventInfo.Source.Type.FullName + "which means that it won't be possible to deserialize the document.\nRegister a tag mapping using the SerializerBuilder.WithTagMapping method.\n\nE.g: builder.WithTagMapping(\"!" + eventInfo.Source.Type.Name + "\", typeof({" + eventInfo.Source.Type.FullName + "}));");
			}
		}
	}
}
