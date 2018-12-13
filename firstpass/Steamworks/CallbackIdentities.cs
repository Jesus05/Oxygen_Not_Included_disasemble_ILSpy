using System;

namespace Steamworks
{
	internal class CallbackIdentities
	{
		public static int GetCallbackIdentity(Type callbackStruct)
		{
			object[] customAttributes = callbackStruct.GetCustomAttributes(typeof(CallbackIdentityAttribute), false);
			int num = 0;
			goto IL_002d;
			IL_002d:
			if (num < customAttributes.Length)
			{
				CallbackIdentityAttribute callbackIdentityAttribute = (CallbackIdentityAttribute)customAttributes[num];
				return callbackIdentityAttribute.Identity;
			}
			throw new Exception("Callback number not found for struct " + callbackStruct);
			IL_0029:
			num++;
			goto IL_002d;
		}
	}
}
