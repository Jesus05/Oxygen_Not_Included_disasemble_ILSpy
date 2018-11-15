using System.IO;
using UnityEngine;

public interface ISerializableComponentManager : IComponentManager
{
	void Serialize(GameObject go, BinaryWriter writer);

	void Deserialize(GameObject go, IReader reader);
}
