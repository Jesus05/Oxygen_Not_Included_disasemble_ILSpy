namespace FMOD.Studio
{
	public struct USER_PROPERTY
	{
		public StringWrapper name;

		public USER_PROPERTY_TYPE type;

		private Union_IntBoolFloatString value;

		public int intValue()
		{
			return (type != 0) ? (-1) : value.intvalue;
		}

		public bool boolValue()
		{
			return type == USER_PROPERTY_TYPE.BOOLEAN && value.boolvalue;
		}

		public float floatValue()
		{
			return (type != USER_PROPERTY_TYPE.FLOAT) ? (-1f) : value.floatvalue;
		}

		public string stringValue()
		{
			return (type != USER_PROPERTY_TYPE.STRING) ? string.Empty : ((string)value.stringvalue);
		}
	}
}
