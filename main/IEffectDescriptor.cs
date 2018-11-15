using System.Collections.Generic;

public interface IEffectDescriptor
{
	List<Descriptor> GetDescriptors(BuildingDef def);
}
