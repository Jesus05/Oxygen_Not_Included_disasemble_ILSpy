using UnityEngine;

public abstract class IBuildingConfig
{
	public abstract BuildingDef CreateBuildingDef();

	public virtual void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
	}

	public abstract void DoPostConfigureComplete(GameObject go);

	public virtual void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}

	public virtual void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public virtual void ConfigurePost(BuildingDef def)
	{
	}
}
