using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using YamlDotNet.Helpers;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.ObjectGraphTraversalStrategies
{
	public class FullObjectGraphTraversalStrategy : IObjectGraphTraversalStrategy
	{
		private readonly int maxRecursion;

		private readonly ITypeInspector typeDescriptor;

		private readonly ITypeResolver typeResolver;

		private INamingConvention namingConvention;

		public FullObjectGraphTraversalStrategy(ITypeInspector typeDescriptor, ITypeResolver typeResolver, int maxRecursion, INamingConvention namingConvention)
		{
			if (maxRecursion <= 0)
			{
				throw new ArgumentOutOfRangeException("maxRecursion", maxRecursion, "maxRecursion must be greater than 1");
			}
			if (typeDescriptor == null)
			{
				throw new ArgumentNullException("typeDescriptor");
			}
			this.typeDescriptor = typeDescriptor;
			if (typeResolver == null)
			{
				throw new ArgumentNullException("typeResolver");
			}
			this.typeResolver = typeResolver;
			this.maxRecursion = maxRecursion;
			this.namingConvention = namingConvention;
		}

		void IObjectGraphTraversalStrategy.Traverse<TContext>(IObjectDescriptor graph, IObjectGraphVisitor<TContext> visitor, TContext context)
		{
			Traverse(graph, visitor, 0, context);
		}

		protected virtual void Traverse<TContext>(IObjectDescriptor value, IObjectGraphVisitor<TContext> visitor, int currentDepth, TContext context)
		{
			if (++currentDepth > maxRecursion)
			{
				throw new InvalidOperationException("Too much recursion when traversing the object graph");
			}
			if (visitor.Enter(value, context))
			{
				TypeCode typeCode = value.Type.GetTypeCode();
				switch (typeCode)
				{
				case TypeCode.Boolean:
				case TypeCode.Char:
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
				case TypeCode.DateTime:
				case TypeCode.String:
					visitor.VisitScalar(value, context);
					break;
				case TypeCode.Empty:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "TypeCode.{0} is not supported.", typeCode));
				default:
					if (value.IsDbNull())
					{
						visitor.VisitScalar((IObjectDescriptor)new ObjectDescriptor(null, typeof(object), typeof(object)), context);
					}
					if (value.Value == null || value.Type == typeof(TimeSpan))
					{
						visitor.VisitScalar(value, context);
					}
					else
					{
						Type underlyingType = Nullable.GetUnderlyingType(value.Type);
						if (underlyingType != null)
						{
							Traverse(new ObjectDescriptor(value.Value, underlyingType, value.Type, value.ScalarStyle), visitor, currentDepth, context);
						}
						else
						{
							TraverseObject(value, visitor, currentDepth, context);
						}
					}
					break;
				}
			}
		}

		protected virtual void TraverseObject<TContext>(IObjectDescriptor value, IObjectGraphVisitor<TContext> visitor, int currentDepth, TContext context)
		{
			if (typeof(IDictionary).IsAssignableFrom(value.Type))
			{
				TraverseDictionary(value, visitor, currentDepth, typeof(object), typeof(object), context);
			}
			else
			{
				Type implementedGenericInterface = ReflectionUtility.GetImplementedGenericInterface(value.Type, typeof(IDictionary<, >));
				if (implementedGenericInterface != null)
				{
					GenericDictionaryToNonGenericAdapter value2 = new GenericDictionaryToNonGenericAdapter(value.Value, implementedGenericInterface);
					Type[] genericArguments = implementedGenericInterface.GetGenericArguments();
					TraverseDictionary(new ObjectDescriptor(value2, value.Type, value.StaticType, value.ScalarStyle), visitor, currentDepth, genericArguments[0], genericArguments[1], context);
				}
				else if (typeof(IEnumerable).IsAssignableFrom(value.Type))
				{
					TraverseList(value, visitor, currentDepth, context);
				}
				else
				{
					TraverseProperties(value, visitor, currentDepth, context);
				}
			}
		}

		protected virtual void TraverseDictionary<TContext>(IObjectDescriptor dictionary, IObjectGraphVisitor<TContext> visitor, int currentDepth, Type keyType, Type valueType, TContext context)
		{
			visitor.VisitMappingStart(dictionary, keyType, valueType, context);
			bool flag = dictionary.Type.FullName.Equals("System.Dynamic.ExpandoObject");
			IDictionaryEnumerator enumerator = ((IDictionary)dictionary.Value).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
					object value = (!flag) ? dictionaryEntry.Key : namingConvention.Apply(dictionaryEntry.Key.ToString());
					IObjectDescriptor objectDescriptor = GetObjectDescriptor(value, keyType);
					IObjectDescriptor objectDescriptor2 = GetObjectDescriptor(dictionaryEntry.Value, valueType);
					if (visitor.EnterMapping(objectDescriptor, objectDescriptor2, context))
					{
						Traverse(objectDescriptor, visitor, currentDepth, context);
						Traverse(objectDescriptor2, visitor, currentDepth, context);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			visitor.VisitMappingEnd(dictionary, context);
		}

		private void TraverseList<TContext>(IObjectDescriptor value, IObjectGraphVisitor<TContext> visitor, int currentDepth, TContext context)
		{
			Type implementedGenericInterface = ReflectionUtility.GetImplementedGenericInterface(value.Type, typeof(IEnumerable<>));
			Type type = (implementedGenericInterface == null) ? typeof(object) : implementedGenericInterface.GetGenericArguments()[0];
			visitor.VisitSequenceStart(value, type, context);
			IEnumerator enumerator = ((IEnumerable)value.Value).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					Traverse(GetObjectDescriptor(current, type), visitor, currentDepth, context);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			visitor.VisitSequenceEnd(value, context);
		}

		protected virtual void TraverseProperties<TContext>(IObjectDescriptor value, IObjectGraphVisitor<TContext> visitor, int currentDepth, TContext context)
		{
			visitor.VisitMappingStart(value, typeof(string), typeof(object), context);
			foreach (IPropertyDescriptor property in typeDescriptor.GetProperties(value.Type, value.Value))
			{
				IObjectDescriptor value2 = property.Read(value.Value);
				if (visitor.EnterMapping(property, value2, context))
				{
					Traverse(new ObjectDescriptor(property.Name, typeof(string), typeof(string)), visitor, currentDepth, context);
					Traverse(value2, visitor, currentDepth, context);
				}
			}
			visitor.VisitMappingEnd(value, context);
		}

		private IObjectDescriptor GetObjectDescriptor(object value, Type staticType)
		{
			return new ObjectDescriptor(value, typeResolver.Resolve(staticType, value), staticType);
		}
	}
}
