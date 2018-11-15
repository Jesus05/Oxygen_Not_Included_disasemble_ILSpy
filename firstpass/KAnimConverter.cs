public class KAnimConverter
{
	public interface IAnimConverter
	{
		SymbolInstanceGpuData symbolInstanceGpuData
		{
			get;
		}

		SymbolOverrideInfoGpuData symbolOverrideInfoGpuData
		{
			get;
		}

		int GetMaxVisible();

		HashedString GetBatchGroupID(bool isEditorWindow = false);

		KAnimBatch GetBatch();

		void SetBatch(KAnimBatch id);

		Vector2I GetCellXY();

		float GetZ();

		int GetLayer();

		string GetName();

		bool IsActive();

		bool IsVisible();

		int GetCurrentNumFrames();

		int GetFirstFrameIndex();

		int GetCurrentFrameIndex();

		Matrix2x3 GetTransformMatrix();

		KBatchedAnimInstanceData GetBatchInstanceData();

		KAnimBatchGroup.MaterialType GetMaterialType();

		bool ApplySymbolOverrides();
	}
}
