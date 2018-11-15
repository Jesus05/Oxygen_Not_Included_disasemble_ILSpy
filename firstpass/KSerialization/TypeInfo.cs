using System;

namespace KSerialization
{
	public class TypeInfo
	{
		public Type type;

		public SerializationTypeInfo info;

		public TypeInfo[] subTypes;

		public Type genericInstantiationType;

		public Type[] genericTypeArgs;

		public void BuildGenericArgs()
		{
			if (type.IsGenericType)
			{
				Type genericTypeDefinition = type.GetGenericTypeDefinition();
				genericTypeArgs = type.GetGenericArguments();
				genericInstantiationType = genericTypeDefinition.MakeGenericType(genericTypeArgs);
			}
			if (subTypes != null)
			{
				for (int i = 0; i < subTypes.Length; i++)
				{
					subTypes[i].BuildGenericArgs();
				}
			}
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is TypeInfo)
			{
				TypeInfo other = (TypeInfo)obj;
				return Equals(other);
			}
			return false;
		}

		public bool Equals(TypeInfo other)
		{
			if (info == other.info)
			{
				if (subTypes != null && other.subTypes != null)
				{
					if (subTypes.Length == other.subTypes.Length)
					{
						for (int i = 0; i < subTypes.Length; i++)
						{
							if (!subTypes[i].Equals(other.subTypes[i]))
							{
								return false;
							}
						}
						return true;
					}
					return false;
				}
				if (subTypes == null && other.subTypes == null)
				{
					return type == other.type;
				}
				return false;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return type.GetHashCode();
		}
	}
}
