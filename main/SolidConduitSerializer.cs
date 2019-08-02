using KSerialization;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SolidConduitSerializer : KMonoBehaviour, ISaveLoadableDetails
{
	protected override void OnPrefabInit()
	{
	}

	protected override void OnSpawn()
	{
	}

	public void Serialize(BinaryWriter writer)
	{
		SolidConduitFlow solidConduitFlow = Game.Instance.solidConduitFlow;
		SolidConduitFlow.SOAInfo sOAInfo = solidConduitFlow.GetSOAInfo();
		List<int> cells = sOAInfo.Cells;
		int num = 0;
		for (int i = 0; i < cells.Count; i++)
		{
			int cell = cells[i];
			SolidConduitFlow.ConduitContents contents = solidConduitFlow.GetContents(cell);
			if (contents.pickupableHandle.IsValid())
			{
				Pickupable pickupable = solidConduitFlow.GetPickupable(contents.pickupableHandle);
				if ((bool)pickupable)
				{
					num++;
				}
			}
		}
		writer.Write(num);
		for (int j = 0; j < cells.Count; j++)
		{
			int num2 = cells[j];
			SolidConduitFlow.ConduitContents contents2 = solidConduitFlow.GetContents(num2);
			if (contents2.pickupableHandle.IsValid())
			{
				Pickupable pickupable2 = solidConduitFlow.GetPickupable(contents2.pickupableHandle);
				if ((bool)pickupable2)
				{
					writer.Write(num2);
					SaveLoadRoot component = pickupable2.GetComponent<SaveLoadRoot>();
					if ((Object)component != (Object)null)
					{
						string name = pickupable2.GetComponent<KPrefabID>().GetSaveLoadTag().Name;
						writer.WriteKleiString(name);
						component.Save(writer);
					}
					else
					{
						Debug.Log("Tried to save obj in solid conduit but obj has no SaveLoadRoot", pickupable2.gameObject);
					}
				}
			}
		}
	}

	public void Deserialize(IReader reader)
	{
		SolidConduitFlow solidConduitFlow = Game.Instance.solidConduitFlow;
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			int cell = reader.ReadInt32();
			string tag_string = reader.ReadKleiString();
			Tag tag = TagManager.Create(tag_string);
			SaveLoadRoot saveLoadRoot = SaveLoadRoot.Load(tag, reader);
			if ((Object)saveLoadRoot != (Object)null)
			{
				Pickupable component = saveLoadRoot.GetComponent<Pickupable>();
				if ((Object)component != (Object)null)
				{
					solidConduitFlow.SetContents(cell, component);
				}
			}
			else
			{
				Debug.Log("Tried to deserialize " + tag.ToString() + " into storage but failed", base.gameObject);
			}
		}
	}
}
