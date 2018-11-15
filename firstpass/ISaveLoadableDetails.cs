using System.IO;

public interface ISaveLoadableDetails
{
	void Serialize(BinaryWriter writer);

	void Deserialize(IReader reader);
}
