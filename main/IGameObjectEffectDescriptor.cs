using System.Collections.Generic;
using UnityEngine;

public interface IGameObjectEffectDescriptor
{
	List<Descriptor> GetDescriptors(GameObject go);
}
